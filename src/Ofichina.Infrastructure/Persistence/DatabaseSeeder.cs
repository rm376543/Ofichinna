using Microsoft.EntityFrameworkCore;
using Ofichina.Contracts.Enums;
using Ofichina.Domain.Aggregates;
using Ofichina.Domain.Entities;
using Ofichina.Domain.ValueObjects;
using System.Security.Cryptography;

namespace Ofichina.Infrastructure.Persistence;

/// <summary>
/// Responsável por popular dados iniciais no banco de dados.
/// Contém dados mockados para desenvolvimento e testes.
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Popula o banco de dados com dados iniciais.
    /// </summary>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        try
        {
            // Perfis
            await SeedPerfis(context);
            await context.SaveChangesAsync();

            // Usuários
            await SeedUsuarios(context);
            await context.SaveChangesAsync();

            // Pessoas
            await SeedPessoas(context);
            await context.SaveChangesAsync();

            // Veículos
            await SeedVeiculos(context);
            await context.SaveChangesAsync();

            // Dados independentes
            await SeedServicos(context);
            await SeedPecas(context);
            await SeedPermissoes(context);

            await SeedDiasDisponibilidade(context);
            await SeedHorariosDisponibilidade(context);

            await context.SaveChangesAsync();

            // Vínculos de autorização
            await SeedPerfilPermissoes(context);

            // Vínculos de agenda
            await SeedDiasHorariosDisponibilidade(context);
            await SeedHorariosConsultores(context);
            await SeedHorariosConsultorDisponibilidade(context);

            await context.SaveChangesAsync();

            // Agendamentos
            await SeedAgendamentos(context);
            await context.SaveChangesAsync();

            await SeedViews(context);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Erro ao popular banco de dados inicial: Verifique se a migration foi executada corretamente.",
                ex);
        }
    }

    #region Seeders

    /// <summary>
    /// Popula a tabela de perfis.
    /// </summary>
    private static async Task SeedPerfis(ApplicationDbContext context)
    {
        var perfisDesejados = new[]
        {
            (Nome: PerfilEnum.Administrador, Descricao: "Administrador do sistema"),
            (Nome: PerfilEnum.Gerente, Descricao: "Gerente da oficina"),
            (Nome: PerfilEnum.Mecanico, Descricao: "Mecânico técnico"),
            (Nome: PerfilEnum.Recepcionista, Descricao: "Atendente de clientes"),
            (Nome: PerfilEnum.Cliente, Descricao: "Cliente da oficina"),
            (Nome: PerfilEnum.Consultor, Descricao: "Consultor de agendamentos")
        };

        var perfisExistentes = await context.Perfis
            .IgnoreQueryFilters()
            .ToListAsync();

        var perfisPorNome = perfisExistentes
            .GroupBy(x => x.NomePerfil, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);

        foreach (var perfil in perfisDesejados)
        {
            if (perfisPorNome.ContainsKey(perfil.Nome))
                continue;

            await context.Perfis.AddAsync(new Perfil(perfil.Nome, perfil.Descricao));
        }
    }

    /// <summary>
    /// Popula a tabela de usuários com dados mockados.
    /// </summary>
    private static async Task SeedUsuarios(ApplicationDbContext context)
    {
        var perfis = await context.Perfis
            .IgnoreQueryFilters()
            .ToListAsync();

        if (!perfis.Any())
            return;

        var perfisPorNome = perfis
            .GroupBy(x => x.NomePerfil, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);

        var usuariosDesejados = new[]
        {
            (Email: "admin@ofichina.com.br", Perfil: PerfilEnum.Administrador),
            (Email: "gerente@ofichina.com.br", Perfil: PerfilEnum.Gerente),
            (Email: "mecanico.silva@ofichina.com.br", Perfil: PerfilEnum.Mecanico),
            (Email: "mecanico.santos@ofichina.com.br", Perfil: PerfilEnum.Mecanico),
            (Email: "atendente.maria@ofichina.com.br", Perfil: PerfilEnum.Recepcionista),
            (Email: "atendente.joao@ofichina.com.br", Perfil: PerfilEnum.Recepcionista),
            (Email: "cliente.pedro@ofichina.com.br", Perfil: PerfilEnum.Cliente),
            (Email: "cliente.ana@ofichina.com.br", Perfil: PerfilEnum.Cliente),
            (Email: "cliente.carlos@ofichina.com.br", Perfil: PerfilEnum.Cliente),
            (Email: "consultor.lucas@ofichina.com.br", Perfil: PerfilEnum.Consultor),
        };

        var senhaPadrao = HashPassword("111111");

        var usuarios = await context.Usuarios
            .IgnoreQueryFilters()
            .Include(x => x.Perfis)
            .ToListAsync();

        var usuariosPorEmail = usuarios
            .GroupBy(x => x.Email.Value, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);

        foreach (var usuarioDesejado in usuariosDesejados)
        {
            if (!usuariosPorEmail.TryGetValue(usuarioDesejado.Email, out var usuario))
            {
                usuario = new Usuario(new Email(usuarioDesejado.Email), senhaPadrao);
                await context.Usuarios.AddAsync(usuario);
                usuariosPorEmail[usuarioDesejado.Email] = usuario;
            }

            if (!perfisPorNome.TryGetValue(usuarioDesejado.Perfil, out var perfil))
                continue;

            if (usuario.Perfis.Any(x => x.PerfilId == perfil.Id))
                continue;

            usuario.AdicionarPerfil(new UsuarioPerfil(usuario.Id, perfil.Id));
        }
    }

    /// <summary>
    /// Popula a tabela de pessoas (clientes e funcionários).
    /// </summary>
    private static async Task SeedPessoas(ApplicationDbContext context)
    {
        var usuarios = await context.Usuarios
            .IgnoreQueryFilters()
            .ToListAsync();

        if (!usuarios.Any())
            return;

        var usuariosPorEmail = usuarios
            .GroupBy(x => x.Email.Value, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);

        var pessoasExistentes = await context.Pessoas
            .IgnoreQueryFilters()
            .ToListAsync();

        var pessoasPorDocumento = pessoasExistentes
            .GroupBy(x => x.Documento.Numero, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);

        var pessoasPorUsuarioId = pessoasExistentes
            .GroupBy(x => x.UsuarioId)
            .ToDictionary(x => x.Key, x => x.First());

#pragma warning disable S1192
        var pessoasDesejadas = new[]
        {
            (
                Nome: "Roberto Almeida",
                Documento: new Cpf("33653787076"),
                Telefone: new Telefone("(11) 98888-0001"),
                Endereco: new Endereco("Rua do Administrador", "10", "Sala 1", "Centro", "São Paulo", "SP", new Cep("01000-001")),
                EmailUsuario: "admin@ofichina.com.br"
            ),
            (
                Nome: "Mariana Ferreira",
                Documento: new Cpf("32780420006"),
                Telefone: new Telefone("(11) 98888-0002"),
                Endereco: new Endereco("Avenida da Gestão", "200", null, "Bela Vista", "São Paulo", "SP", new Cep("01310-200")),
                EmailUsuario: "gerente@ofichina.com.br"
            ),
            (
                Nome: "Carlos Silva",
                Documento: new Cpf("36381910011"),
                Telefone: new Telefone("(11) 98888-0003"),
                Endereco: new Endereco("Rua da Oficina", "300", "Fundos", "Mooca", "São Paulo", "SP", new Cep("03100-300")),
                EmailUsuario: "mecanico.silva@ofichina.com.br"
            ),
            (
                Nome: "Juliana Santos",
                Documento: new Cpf("35959541068"),
                Telefone: new Telefone("(11) 98888-0004"),
                Endereco: new Endereco("Rua das Ferramentas", "400", null, "Tatuapé", "São Paulo", "SP", new Cep("03040-400")),
                EmailUsuario: "mecanico.santos@ofichina.com.br"
            ),
            (
                Nome: "Maria Souza",
                Documento: new Cpf("85736502062"),
                Telefone: new Telefone("(11) 98888-0005"),
                Endereco: new Endereco("Avenida do Atendimento", "500", "Loja A", "Centro", "São Paulo", "SP", new Cep("01111-500")),
                EmailUsuario: "atendente.maria@ofichina.com.br"
            ),
            (
                Nome: "João Lima",
                Documento: new Cpf("65778065000"),
                Telefone: new Telefone("(11) 98888-0006"),
                Endereco: new Endereco("Rua do Suporte", "600", null, "Santana", "São Paulo", "SP", new Cep("02020-600")),
                EmailUsuario: "atendente.joao@ofichina.com.br"
            ),
            (
                Nome: "Lucas Ferreira",
                Documento: new Cpf("86419674000"),
                Telefone: new Telefone("(11) 98888-0007"),
                Endereco: new Endereco("Rua do Consultor", "700", "Conjunto 7", "Pinheiros", "São Paulo", "SP", new Cep("05432-700")),
                EmailUsuario: "consultor.lucas@ofichina.com.br"
            ),
            (
                Nome: "Pedro Silva",
                Documento: new Cpf("25228444076"),
                Telefone: new Telefone("(11) 98765-4321"),
                Endereco: new Endereco("Rua das Flores", "123", "Apto 101", "Centro", "São Paulo", "SP", new Cep("01310-100")),
                EmailUsuario: "cliente.pedro@ofichina.com.br"
            ),
            (
                Nome: "Ana Costa",
                Documento: new Cpf("60174914075"),
                Telefone: new Telefone("(11) 97654-3210"),
                Endereco: new Endereco("Avenida Paulista", "1000", null, "Bela Vista", "São Paulo", "SP", new Cep("01311-100")),
                EmailUsuario: "cliente.ana@ofichina.com.br"
            ),
            (
                Nome: "Carlos Oliveira",
                Documento: new Cpf("79266825000"),
                Telefone: new Telefone("(11) 96543-2109"),
                Endereco: new Endereco("Rua Augusta", "500", "Loja 5", "Consolação", "São Paulo", "SP", new Cep("01305-100")),
                EmailUsuario: "cliente.carlos@ofichina.com.br"
            ),
        };
#pragma warning restore S1192

        foreach (var pessoaDesejada in pessoasDesejadas)
        {
            if (!usuariosPorEmail.TryGetValue(pessoaDesejada.EmailUsuario, out var usuario))
                continue;

            if (pessoasPorDocumento.ContainsKey(pessoaDesejada.Documento.Numero))
                continue;

            if (pessoasPorUsuarioId.ContainsKey(usuario.Id))
                continue;

            await context.Pessoas.AddAsync(new Pessoa(
                pessoaDesejada.Nome,
                pessoaDesejada.Documento,
                pessoaDesejada.Telefone,
                pessoaDesejada.Endereco,
                usuario.Id));
        }
    }

    /// <summary>
    /// Popula a tabela de veículos.
    /// </summary>
    private static async Task SeedVeiculos(ApplicationDbContext context)
    {
        var pessoas = await context.Pessoas
            .IgnoreQueryFilters()
            .ToListAsync();

        if (!pessoas.Any())
            return;

        var pessoasPorDocumento = pessoas
            .GroupBy(x => x.Documento.Numero, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);

        var veiculosExistentes = await context.Veiculos
            .IgnoreQueryFilters()
            .ToListAsync();

        var veiculosPorPlaca = veiculosExistentes
            .GroupBy(x => x.Placa.Numero, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);

        var veiculosDesejados = new[]
        {
            (DocumentoPessoa: "25228444076", Placa: "ABC1D23", Marca: "Toyota", Modelo: "Corolla", AnoFabricacao: 2020, Cor: "Preto", Hodometro: 9122),
            (DocumentoPessoa: "60174914075", Placa: "XYZ9K87", Marca: "Honda", Modelo: "Civic", AnoFabricacao: 2019, Cor: "Prata", Hodometro: 15000),
            (DocumentoPessoa: "79266825000", Placa: "LMN5Q34", Marca: "Volkswagen", Modelo: "Gol", AnoFabricacao: 2021, Cor: "Vermelho", Hodometro: 110),
            (DocumentoPessoa: "25228444076", Placa: "OPQ8R56", Marca: "Hyundai", Modelo: "HB20", AnoFabricacao: 2022, Cor: "Branco", Hodometro: 24150),
        };

        foreach (var veiculoDesejado in veiculosDesejados)
        {
            if (veiculosPorPlaca.ContainsKey(veiculoDesejado.Placa))
                continue;

            if (!pessoasPorDocumento.TryGetValue(veiculoDesejado.DocumentoPessoa, out var pessoa))
                continue;

            await context.Veiculos.AddAsync(new Veiculo(
                pessoa.Id,
                new Placa(veiculoDesejado.Placa),
                veiculoDesejado.Marca,
                veiculoDesejado.Modelo,
                veiculoDesejado.AnoFabricacao,
                veiculoDesejado.Cor,
                new Hodometro(veiculoDesejado.Hodometro)));
        }
    }

    /// <summary>
    /// Popula a tabela de serviços.
    /// </summary>
    private static async Task SeedServicos(ApplicationDbContext context)
    {
        var servicosExistentes = await context.Servicos
            .IgnoreQueryFilters()
            .ToListAsync();

        var servicosPorNome = servicosExistentes
            .GroupBy(x => x.Nome, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);

        var servicosDesejados = new[]
        {
            (Nome: "Troca de Óleo", Descricao: "Troca completa de óleo e filtro", Valor: 150.00m),
            (Nome: "Revisão Completa", Descricao: "Revisão geral do veículo", Valor: 500.00m),
            (Nome: "Troca de Pneus", Descricao: "Troca e balanceamento de pneus", Valor: 400.00m),
            (Nome: "Alinhamento", Descricao: "Alinhamento e cambagem de rodas", Valor: 250.00m),
            (Nome: "Freios", Descricao: "Revisão e manutenção de freios", Valor: 350.00m),
            (Nome: "Suspensão", Descricao: "Manutenção e reparo de suspensão", Valor: 600.00m),
            (Nome: "Ar Condicionado", Descricao: "Recarga e limpeza de ar condicionado", Valor: 200.00m),
            (Nome: "Bateria", Descricao: "Troca de bateria", Valor: 300.00m),
            (Nome: "Embreagem", Descricao: "Troca de discos e placa de embreagem", Valor: 800.00m),
            (Nome: "Transmissão", Descricao: "Reparo e revisão de transmissão", Valor: 1200.00m),
        };

        foreach (var servico in servicosDesejados)
        {
            if (servicosPorNome.ContainsKey(servico.Nome))
                continue;

            await context.Servicos.AddAsync(new Servico(servico.Nome, servico.Descricao, servico.Valor));
        }
    }

    /// <summary>
    /// Popula a tabela de peças.
    /// </summary>
    private static async Task SeedPecas(ApplicationDbContext context)
    {
        var pecasExistentes = await context.Pecas
            .IgnoreQueryFilters()
            .ToListAsync();

        var pecasPorCodigo = pecasExistentes
            .GroupBy(x => x.Codigo, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);

        var pecasDesejadas = new[]
        {
            (Nome: "Óleo Sintético 5W30", Descricao: "Óleo motor sintético premium", Codigo: "PE-001", Valor: 80.00m, QuantidadeEstoque: 50),
            (Nome: "Filtro de Ar", Descricao: "Filtro de ar do motor", Codigo: "PE-002", Valor: 45.00m, QuantidadeEstoque: 100),
            (Nome: "Filtro de Óleo", Descricao: "Filtro de óleo do motor", Codigo: "PE-003", Valor: 35.00m, QuantidadeEstoque: 80),
            (Nome: "Pneu Michelin 185/65R15", Descricao: "Pneu para carros compactos", Codigo: "PE-004", Valor: 250.00m, QuantidadeEstoque: 30),
            (Nome: "Pastilha de Freio", Descricao: "Pastilha de freio dianteira", Codigo: "PE-005", Valor: 120.00m, QuantidadeEstoque: 60),
            (Nome: "Disco de Freio", Descricao: "Disco de freio ventilado", Codigo: "PE-006", Valor: 180.00m, QuantidadeEstoque: 40),
            (Nome: "Amortecedor", Descricao: "Amortecedor hidráulico", Codigo: "PE-007", Valor: 350.00m, QuantidadeEstoque: 20),
            (Nome: "Bateria Automotiva 60Ah", Descricao: "Bateria 60amperes hora", Codigo: "PE-008", Valor: 450.00m, QuantidadeEstoque: 15),
            (Nome: "Vela de Ignição", Descricao: "Vela de ignição de cobre", Codigo: "PE-009", Valor: 25.00m, QuantidadeEstoque: 200),
            (Nome: "Correia Dentada", Descricao: "Correia de distribuição", Codigo: "PE-010", Valor: 150.00m, QuantidadeEstoque: 10),
        };

        foreach (var peca in pecasDesejadas)
        {
            if (pecasPorCodigo.ContainsKey(peca.Codigo))
                continue;

            await context.Pecas.AddAsync(new Peca(peca.Nome, peca.Descricao, peca.Codigo, peca.Valor, peca.QuantidadeEstoque));
        }
    }

    /// <summary>
    /// Popula a tabela de permissões.
    /// </summary>
    private static async Task SeedPermissoes(ApplicationDbContext context)
    {
        var permissoesExistentes = await context.Permissoes
            .IgnoreQueryFilters()
            .ToListAsync();

        var permissoesPorCodigo = permissoesExistentes
            .GroupBy(x => x.Codigo, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);

        var permissoesDesejadas = new[]
        {
            (Codigo: PolicyEnum.UsuariosCriar, Descricao: "Criar usuários"),
            (Codigo: PolicyEnum.UsuariosEditar, Descricao: "Editar usuários"),
            (Codigo: PolicyEnum.UsuariosDeletar, Descricao: "Deletar usuários"),
            (Codigo: PolicyEnum.UsuariosListar, Descricao: "Listar usuários"),
            (Codigo: PolicyEnum.PessoasCriar, Descricao: "Criar pessoas"),
            (Codigo: PolicyEnum.PessoasEditar, Descricao: "Editar pessoas"),
            (Codigo: PolicyEnum.PessoasDeletar, Descricao: "Deletar pessoas"),
            (Codigo: PolicyEnum.PessoasListar, Descricao: "Listar pessoas"),
            (Codigo: PolicyEnum.VeiculosCriar, Descricao: "Criar veículos"),
            (Codigo: PolicyEnum.VeiculosEditar, Descricao: "Editar veículos"),
            (Codigo: PolicyEnum.VeiculosDeletar, Descricao: "Deletar veículos"),
            (Codigo: PolicyEnum.VeiculosListar, Descricao: "Listar veículos"),
            (Codigo: PolicyEnum.OrdensServicoCriar, Descricao: "Criar ordens de serviço"),
            (Codigo: PolicyEnum.OrdensServicoEditar, Descricao: "Editar ordens de serviço"),
            (Codigo: PolicyEnum.OrdensServicoDeletar, Descricao: "Deletar ordens de serviço"),
            (Codigo: PolicyEnum.OrdensServicoListar, Descricao: "Listar ordens de serviço"),
            (Codigo: PolicyEnum.OrcamentosCriar, Descricao: "Criar orçamentos"),
            (Codigo: PolicyEnum.OrcamentosEditar, Descricao: "Editar orçamentos"),
            (Codigo: PolicyEnum.OrcamentosDeletar, Descricao: "Deletar orçamentos"),
            (Codigo: PolicyEnum.OrcamentosListar, Descricao: "Listar orçamentos"),
            (Codigo: PolicyEnum.AgendamentosCriar, Descricao: "Criar agendamentos"),
            (Codigo: PolicyEnum.AgendamentosEditar, Descricao: "Editar agendamentos"),
            (Codigo: PolicyEnum.AgendamentosDeletar, Descricao: "Deletar agendamentos"),
            (Codigo: PolicyEnum.AgendamentosListar, Descricao: "Listar agendamentos"),
            (Codigo: PolicyEnum.ServicosCriar, Descricao: "Criar serviços"),
            (Codigo: PolicyEnum.ServicosEditar, Descricao: "Editar serviços"),
            (Codigo: PolicyEnum.ServicosListar, Descricao: "Listar serviços"),
            (Codigo: PolicyEnum.PecasCriar, Descricao: "Criar peças"),
            (Codigo: PolicyEnum.PecasEditar, Descricao: "Editar peças"),
            (Codigo: PolicyEnum.PecasListar, Descricao: "Listar peças"),
        };

        foreach (var permissao in permissoesDesejadas)
        {
            if (permissoesPorCodigo.ContainsKey(permissao.Codigo))
                continue;

            await context.Permissoes.AddAsync(new Permissao(permissao.Codigo, permissao.Descricao));
        }
    }

    /// <summary>
    /// Popula os vínculos entre perfis e permissões.
    /// </summary>
    private static async Task SeedPerfilPermissoes(ApplicationDbContext context)
    {
        var permissoes = await context.Permissoes
            .IgnoreQueryFilters()
            .ToListAsync();

        if (!permissoes.Any())
            return;

        var perfis = await context.Perfis
            .IgnoreQueryFilters()
            .ToListAsync();

        if (!perfis.Any())
            return;

        var perfisPorNome = perfis
            .GroupBy(x => x.NomePerfil, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);

        var permissoesPorCodigo = permissoes
            .GroupBy(x => x.Codigo, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);

        var matrizPermissoes = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            [PerfilEnum.Administrador] = permissoes.Select(x => x.Codigo).ToArray(),
            [PerfilEnum.Gerente] = [
                PolicyEnum.UsuariosCriar,
                PolicyEnum.UsuariosEditar,
                PolicyEnum.UsuariosDeletar,
                PolicyEnum.UsuariosListar,
                PolicyEnum.VeiculosCriar,
                PolicyEnum.VeiculosEditar,
                PolicyEnum.VeiculosDeletar,
                PolicyEnum.VeiculosListar,
                PolicyEnum.OrdensServicoCriar,
                PolicyEnum.OrdensServicoEditar,
                PolicyEnum.OrdensServicoDeletar,
                PolicyEnum.OrdensServicoListar,
                PolicyEnum.OrcamentosCriar,
                PolicyEnum.OrcamentosEditar,
                PolicyEnum.OrcamentosDeletar,
                PolicyEnum.OrcamentosListar,
                PolicyEnum.AgendamentosCriar,
                PolicyEnum.AgendamentosEditar,
                PolicyEnum.AgendamentosDeletar,
                PolicyEnum.AgendamentosListar,
                PolicyEnum.ServicosCriar,
                PolicyEnum.ServicosEditar,
                PolicyEnum.ServicosListar,
                PolicyEnum.PecasCriar,
                PolicyEnum.PecasEditar,
                PolicyEnum.PecasListar
            ],
            [PerfilEnum.Mecanico] = [
                PolicyEnum.UsuariosListar,
                PolicyEnum.VeiculosListar,
                PolicyEnum.OrdensServicoCriar,
                PolicyEnum.OrdensServicoEditar,
                PolicyEnum.OrdensServicoListar,
                PolicyEnum.OrcamentosListar,
                PolicyEnum.AgendamentosListar,
                PolicyEnum.ServicosListar,
                PolicyEnum.PecasListar
            ],
            [PerfilEnum.Recepcionista] = [
                PolicyEnum.UsuariosCriar,
                PolicyEnum.UsuariosEditar,
                PolicyEnum.UsuariosListar,
                PolicyEnum.VeiculosCriar,
                PolicyEnum.VeiculosEditar,
                PolicyEnum.VeiculosListar,
                PolicyEnum.AgendamentosCriar,
                PolicyEnum.AgendamentosEditar,
                PolicyEnum.AgendamentosListar,
                PolicyEnum.OrcamentosListar
            ],
            [PerfilEnum.Cliente] = [
                PolicyEnum.AgendamentosCriar,
                PolicyEnum.AgendamentosListar,
                PolicyEnum.OrcamentosListar,
                PolicyEnum.VeiculosListar
            ],
            [PerfilEnum.Consultor] = [
                PolicyEnum.UsuariosListar,
                PolicyEnum.VeiculosListar,
                PolicyEnum.AgendamentosCriar,
                PolicyEnum.AgendamentosEditar,
                PolicyEnum.AgendamentosListar,
                PolicyEnum.OrcamentosListar
            ]
        };

        foreach (var (nomePerfil, codigosPermissoes) in matrizPermissoes)
        {
            if (!perfisPorNome.TryGetValue(nomePerfil, out var perfil))
                continue;

            var permissaoIdsExistentes = await context.PerfisPermissoes
                .IgnoreQueryFilters()
                .Where(x => x.PerfilId == perfil.Id)
                .Select(x => x.PermissaoId)
                .ToListAsync();

            var permissaoIdsJaVinculados = permissaoIdsExistentes.ToHashSet();

            foreach (var codigoPermissao in codigosPermissoes)
            {
                if (!permissoesPorCodigo.TryGetValue(codigoPermissao, out var permissao))
                    continue;

                if (permissaoIdsJaVinculados.Contains(permissao.Id))
                    continue;

                await context.PerfisPermissoes.AddAsync(new PerfilPermissao(perfil.Id, permissao.Id));
                permissaoIdsJaVinculados.Add(permissao.Id);
            }
        }
    }

    /// <summary>
    /// Popula a tabela de dias de disponibilidade.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private static async Task SeedDiasDisponibilidade(ApplicationDbContext context)
    {
        var diasExistentes = await context.DiasDisponibilidade
            .IgnoreQueryFilters()
            .ToListAsync();

        var diasPorData = diasExistentes
            .GroupBy(x => x.Data)
            .ToDictionary(x => x.Key, x => x.First());

        var diasDesejados = ObterProximosDiasUteis(10);

        foreach (var data in diasDesejados)
        {
            if (diasPorData.ContainsKey(data))
                continue;

            await context.DiasDisponibilidade.AddAsync(new DiaDisponibilidade(data));
        }
    }

    /// <summary>
    /// Popula a tabela de horários de disponibilidade.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private static async Task SeedHorariosDisponibilidade(ApplicationDbContext context)
    {
        var horariosExistentes = await context.HorariosDisponibilidade
            .IgnoreQueryFilters()
            .ToListAsync();

        var horariosPorHora = horariosExistentes
            .GroupBy(x => x.Hora)
            .ToDictionary(x => x.Key, x => x.First());

        var horariosDesejados = new[]
        {
        new TimeOnly(8, 0),
        new TimeOnly(9, 0),
        new TimeOnly(10, 0),
        new TimeOnly(11, 0),
        new TimeOnly(13, 0),
        new TimeOnly(14, 0),
        new TimeOnly(15, 0),
        new TimeOnly(16, 0),
        new TimeOnly(17, 0),
    };

        foreach (var hora in horariosDesejados)
        {
            if (horariosPorHora.ContainsKey(hora))
                continue;

            await context.HorariosDisponibilidade.AddAsync(new HorarioDisponibilidade(hora));
        }
    }

    /// <summary>
    /// Popula a tabela de vínculos entre dias e horários de disponibilidade.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
#pragma warning disable S3776
    private static async Task SeedDiasHorariosDisponibilidade(ApplicationDbContext context)
#pragma warning restore S3776
    {
        var dias = await context.DiasDisponibilidade
            .IgnoreQueryFilters()
            .ToListAsync();

        var horarios = await context.HorariosDisponibilidade
            .IgnoreQueryFilters()
            .ToListAsync();

        if (!dias.Any() || !horarios.Any())
            return;

        var vinculosExistentes = await context.DiasHorariosDisponibilidade
            .IgnoreQueryFilters()
            .Select(x => new { x.DiaDisponibilidadeId, x.HorarioDisponibilidadeId })
            .ToListAsync();

        var vinculos = vinculosExistentes
            .Select(x => (x.DiaDisponibilidadeId, x.HorarioDisponibilidadeId))
            .ToHashSet();

        var horarioInicioAlmoco = new TimeOnly(12, 0);

        foreach (var dia in dias)
        {
            if (dia.Data.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                continue;

            foreach (var horario in horarios)
            {
                if (horario.Hora == horarioInicioAlmoco)
                    continue;

                if (horario.Hora < new TimeOnly(8, 0) || horario.Hora > new TimeOnly(17, 0))
                    continue;

                var chave = (dia.Id, horario.Id);

                if (vinculos.Contains(chave))
                    continue;

                await context.DiasHorariosDisponibilidade.AddAsync(
                    new DiaHorarioDisponibilidade(dia.Id, horario.Id));

                vinculos.Add(chave);
            }
        }
    }

    /// <summary>
    /// Popula a tabela de horários dos consultores, vinculando horários aos consultores.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private static async Task SeedHorariosConsultores(ApplicationDbContext context)
    {
        var pessoasConsultoras = await ObterConsultoresAsync(context);

        if (!pessoasConsultoras.Any())
            return;

        var horarios = await context.HorariosDisponibilidade
            .IgnoreQueryFilters()
            .ToListAsync();

        var horariosOperacionais = horarios
            .Where(x => x.Hora >= new TimeOnly(8, 0) && x.Hora <= new TimeOnly(17, 0))
            .Where(x => x.Hora != new TimeOnly(12, 0))
            .OrderBy(x => x.Hora)
            .ToList();

        if (!horariosOperacionais.Any())
            return;

        var vinculosExistentes = await context.HorariosConsultores
            .IgnoreQueryFilters()
            .Select(x => new { x.HorarioDisponibilidadeId, x.PessoaId })
            .ToListAsync();

        var vinculos = vinculosExistentes
            .Select(x => (x.HorarioDisponibilidadeId, x.PessoaId))
            .ToHashSet();

        foreach (var consultor in pessoasConsultoras)
        {
            foreach (var horario in horariosOperacionais)
            {
                var chave = (horario.Id, consultor.Id);

                if (vinculos.Contains(chave))
                    continue;

                await context.HorariosConsultores.AddAsync(
                    new HorarioConsultor(horario.Id, consultor.Id));

                vinculos.Add(chave);
            }
        }
    }

    /// <summary>
    /// Popula a tabela de horários de disponibilidade dos consultores, vinculando dias e horários aos consultores.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private static async Task SeedHorariosConsultorDisponibilidade(ApplicationDbContext context)
    {
        var pessoasConsultoras = await ObterConsultoresAsync(context);

        if (!pessoasConsultoras.Any())
            return;

        var dias = await context.DiasDisponibilidade
            .IgnoreQueryFilters()
            .ToListAsync();

        var horarios = await context.HorariosDisponibilidade
            .IgnoreQueryFilters()
            .ToListAsync();

        if (!dias.Any() || !horarios.Any())
            return;

        var diasUteis = dias
            .Where(x => x.Data.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday)
            .OrderBy(x => x.Data)
            .ToList();

        var horariosOperacionais = horarios
            .Where(x => x.Hora >= new TimeOnly(8, 0) && x.Hora <= new TimeOnly(17, 0))
            .Where(x => x.Hora != new TimeOnly(12, 0))
            .OrderBy(x => x.Hora)
            .ToList();

        var vinculosExistentes = await context.HorariosConsultorDisponibilidade
            .IgnoreQueryFilters()
            .Select(x => new { x.DiaDisponibilidadeId, x.HorarioDisponibilidadeId, x.ConsultorPessoaId })
            .ToListAsync();

        var vinculos = vinculosExistentes
            .Select(x => (x.DiaDisponibilidadeId, x.HorarioDisponibilidadeId, x.ConsultorPessoaId))
            .ToHashSet();

        foreach (var consultor in pessoasConsultoras)
        {
            foreach (var dia in diasUteis)
            {
                foreach (var horario in horariosOperacionais)
                {
                    var chave = (dia.Id, horario.Id, consultor.Id);

                    if (vinculos.Contains(chave))
                        continue;

                    await context.HorariosConsultorDisponibilidade.AddAsync(
                        new AgendaConsultor(dia.Id, horario.Id, consultor.Id));

                    vinculos.Add(chave);
                }
            }
        }
    }

    /// <summary>
    /// Popula a tabela de agendamentos.
    /// </summary>
    private static async Task SeedAgendamentos(ApplicationDbContext context)
    {
        var clientes = await ObterClientesAsync(context);
        var veiculos = await context.Veiculos
            .IgnoreQueryFilters()
            .ToListAsync();

        var agendas = await context.HorariosConsultorDisponibilidade
            .IgnoreQueryFilters()
            .Include(x => x.DiaDisponibilidade)
            .Include(x => x.HorarioDisponibilidade)
            .OrderBy(x => x.DiaDisponibilidade.Data)
            .ThenBy(x => x.HorarioDisponibilidade.Hora)
            .ToListAsync();

        if (!clientes.Any() || !veiculos.Any() || !agendas.Any())
            return;

        var clientesPorDocumento = clientes
            .GroupBy(x => x.Documento.Numero, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);

        var veiculosPorPlaca = veiculos
            .GroupBy(x => x.Placa.Numero, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);

        var agendasOcupadas = (await context.Agendamentos
            .IgnoreQueryFilters()
            .Select(x => x.AgendaConsultorId)
            .ToListAsync())
            .ToHashSet();

        var slots = agendas
            .GroupBy(x => x.DiaDisponibilidade.Data)
            .Select(x => x.OrderBy(y => y.HorarioDisponibilidade.Hora).First())
            .Take(4)
            .ToList();

        if (slots.Count < 4)
            return;

        var agendamentosDesejados = new[]
        {
            (ClienteDocumento: "25228444076", Placa: "ABC1D23", Slot: slots[0], Hodometro: 9200, Descricao: "Revisão preventiva"),
            (ClienteDocumento: "60174914075", Placa: "XYZ9K87", Slot: slots[1], Hodometro: 15100, Descricao: "Troca de óleo e filtros"),
            (ClienteDocumento: "79266825000", Placa: "LMN5Q34", Slot: slots[2], Hodometro: 250, Descricao: "Diagnóstico de suspensão"),
            (ClienteDocumento: "25228444076", Placa: "OPQ8R56", Slot: slots[3], Hodometro: 24200, Descricao: "Alinhamento e freios"),
        };

        foreach (var desejado in agendamentosDesejados)
        {
            if (agendasOcupadas.Contains(desejado.Slot.Id))
                continue;

            if (!clientesPorDocumento.TryGetValue(desejado.ClienteDocumento, out var cliente))
                continue;

            if (!veiculosPorPlaca.TryGetValue(desejado.Placa, out var veiculo))
                continue;

            await context.Agendamentos.AddAsync(new Agendamento(
                cliente.Id,
                desejado.Slot.Id,
                veiculo.Id,
                desejado.Hodometro,
                desejado.Descricao));

            agendasOcupadas.Add(desejado.Slot.Id);
        }
    }

    /// <summary>
    /// Obtém os clientes cadastrados no sistema.
    /// </summary>
    private static async Task<IReadOnlyCollection<Pessoa>> ObterClientesAsync(ApplicationDbContext context)
    {
        var pessoas = await context.Pessoas
            .IgnoreQueryFilters()
            .Include(x => x.Usuario)
                .ThenInclude(x => x.Perfis)
                    .ThenInclude(x => x.Perfil)
            .ToListAsync();

        return pessoas
            .Where(x => x.Usuario.Perfis.Any(p => p.Perfil.NomePerfil.Equals(PerfilEnum.Cliente, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    /// <summary>
    /// Cria ou atualiza as views utilizadas pela aplicação.
    /// </summary>
    private static async Task SeedViews(ApplicationDbContext context)
    {
        await SeedViewAgendamentoPessoa(context);
    }
    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Cria hash de senha usando PBKDF2.
    /// Nota: Em produção, use BCrypt ou Argon2 via biblioteca dedicada.
    /// </summary>
    private static string HashPassword(string password)
    {
        const int saltSize = 16;
        const int keySize = 32;
        const int iterations = 100_000;

        var salt = RandomNumberGenerator.GetBytes(saltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            keySize);

        return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    /// <summary>
    /// Obtém os próximos dias úteis a partir da data atual.
    /// </summary>
    /// <param name="quantidade"></param>
    /// <returns></returns>
    private static IReadOnlyCollection<DateOnly> ObterProximosDiasUteis(int quantidade)
    {
        var dias = new List<DateOnly>();
        var dataAtual = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);

        while (dias.Count < quantidade)
        {
            if (dataAtual.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday)
                dias.Add(dataAtual);

            dataAtual = dataAtual.AddDays(1);
        }

        return dias;
    }

    /// <summary>
    /// Obtém a lista de pessoas que possuem o perfil de consultor.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private static async Task<IReadOnlyCollection<Pessoa>> ObterConsultoresAsync(ApplicationDbContext context)
    {
        var pessoas = await context.Pessoas
            .IgnoreQueryFilters()
            .Include(x => x.Usuario)
                .ThenInclude(x => x.Perfis)
                    .ThenInclude(x => x.Perfil)
            .ToListAsync();

        return pessoas
            .Where(x => x.Usuario.Perfis.Any(p => p.Perfil.NomePerfil.Equals("CONSULTOR", StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    #endregion

    /// <summary>
    /// Cria a view de agendamentos da pessoa apenas se ela ainda não existir.
    /// </summary>
    private static async Task SeedViewAgendamentoPessoa(ApplicationDbContext context)
    {
        const string sql = """
        IF OBJECT_ID(N'dbo.vwAgendamentoPessoa', N'V') IS NULL
        EXEC(N'
            CREATE VIEW dbo.vwAgendamentoPessoa
            AS
            SELECT
                a.AgendamentosId,
                p.PessoaId,
                v.VeiculoId,
                a.Status as StatusAgendamento,
                p.Nome,
                p.Documento,
                p.Telefone,
                v.Placa,
                v.Marca,
                v.Modelo,
                v.AnoFabricacao,
                v.Cor,
                v.Hodometro,
                consultor.Nome AS Consultor,
                dd.Data AS DtAgendamento,
                CONVERT(TIME(0), hd.Hora) AS HorarioAgendamento,
                a.CreatedAt,
                a.UpdatedAt,
                a.DeletedAt
            FROM Agendamentos AS a
            INNER JOIN Pessoas AS p
                ON p.PessoaId = a.ClientePessoaId
            INNER JOIN Veiculos AS v
                ON v.VeiculoId = a.VeiculoId
            INNER JOIN AgendaConsultor AS ac
                ON ac.AgendamentoConsultorId = a.AgendaConsultorId
            INNER JOIN DiasDisponibilidade AS dd
                ON dd.DiaDisponibilidadeId = ac.DiaDisponibilidadeId
            INNER JOIN HorariosDisponibilidade AS hd
                ON hd.HorarioDisponibilidadeId = ac.HorarioDisponibilidadeId
            INNER JOIN Pessoas AS consultor
                ON consultor.PessoaId = ac.ConsultorPessoaId
        ');
        """;

        await context.Database.ExecuteSqlRawAsync(sql);
    }
}