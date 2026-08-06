using Microsoft.EntityFrameworkCore;
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

            await context.SaveChangesAsync();

            // Vínculos de autorização
            await SeedPerfilPermissoes(context);

            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Erro ao popular banco de dados inicial.",
                ex);
        }
    }

    /// <summary>
    /// Popula a tabela de perfis.
    /// </summary>
    private static async Task SeedPerfis(ApplicationDbContext context)
    {
        var perfisDesejados = new[]
        {
            (Nome: "ADMIN", Descricao: "Administrador do sistema"),
            (Nome: "GERENTE", Descricao: "Gerente da oficina"),
            (Nome: "MECANICO", Descricao: "Mecânico técnico"),
            (Nome: "ATENDENTE", Descricao: "Atendente de clientes"),
            (Nome: "CLIENTE", Descricao: "Cliente da oficina"),
            (Nome: "CONSULTOR", Descricao: "Consultor de agendamentos")
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
            (Email: "admin@ofichina.com.br", Perfil: "ADMIN"),
            (Email: "gerente@ofichina.com.br", Perfil: "GERENTE"),
            (Email: "mecanico.silva@ofichina.com.br", Perfil: "MECANICO"),
            (Email: "mecanico.santos@ofichina.com.br", Perfil: "MECANICO"),
            (Email: "atendente.maria@ofichina.com.br", Perfil: "ATENDENTE"),
            (Email: "atendente.joao@ofichina.com.br", Perfil: "ATENDENTE"),
            (Email: "cliente.pedro@ofichina.com.br", Perfil: "CLIENTE"),
            (Email: "cliente.ana@ofichina.com.br", Perfil: "CLIENTE"),
            (Email: "cliente.carlos@ofichina.com.br", Perfil: "CLIENTE"),
            (Email: "consultor.lucas@ofichina.com.br", Perfil: "CONSULTOR"),
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

        var pessoasDesejadas = new[]
        {
            (
                Nome: "Pedro Silva",
                Documento: new Cpf("90351003010"),
                Telefone: new Telefone("(11) 98765-4321"),
                Endereco: new Endereco("Rua das Flores", "123", "Apto 101", "Centro", "São Paulo", "SP", new Cep("01310-100")),
                EmailUsuario: "cliente.pedro@email.com"
            ),
            (
                Nome: "Ana Costa",
                Documento: new Cpf("90115400001"),
                Telefone: new Telefone("(11) 97654-3210"),
                Endereco: new Endereco("Avenida Paulista", "1000", null, "Bela Vista", "São Paulo", "SP", new Cep("01311-100")),
                EmailUsuario: "cliente.ana@email.com"
            ),
            (
                Nome: "Carlos Oliveira",
                Documento: new Cpf("77891063095"),
                Telefone: new Telefone("(11) 96543-2109"),
                Endereco: new Endereco("Rua Augusta", "500", "Loja 5", "Consolação", "São Paulo", "SP", new Cep("01305-100")),
                EmailUsuario: "cliente.carlos@email.com"
            ),
        };

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
            (DocumentoPessoa: "90351003010", Placa: "ABC1D23", Marca: "Toyota", Modelo: "Corolla", AnoFabricacao: 2020, Cor: "Preto", Hodometro: 9122),
            (DocumentoPessoa: "90115400001", Placa: "XYZ9K87", Marca: "Honda", Modelo: "Civic", AnoFabricacao: 2019, Cor: "Prata", Hodometro: 15000),
            (DocumentoPessoa: "77891063095", Placa: "LMN5Q34", Marca: "Volkswagen", Modelo: "Gol", AnoFabricacao: 2021, Cor: "Vermelho", Hodometro: 110),
            (DocumentoPessoa: "90351003010", Placa: "OPQ8R56", Marca: "Hyundai", Modelo: "HB20", AnoFabricacao: 2022, Cor: "Branco", Hodometro: 24150),
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

#pragma warning disable S1192
        var permissoesDesejadas = new[]
        {
            (Codigo: "usuarios.criar", Descricao: "Criar usuários"),
            (Codigo: "usuarios.editar", Descricao: "Editar usuários"),
            (Codigo: "usuarios.deletar", Descricao: "Deletar usuários"),
            (Codigo: "usuarios.listar", Descricao: "Listar usuários"),
            (Codigo: "pessoas.criar", Descricao: "Criar pessoas"),
            (Codigo: "pessoas.editar", Descricao: "Editar pessoas"),
            (Codigo: "pessoas.deletar", Descricao: "Deletar pessoas"),
            (Codigo: "pessoas.listar", Descricao: "Listar pessoas"),
            (Codigo: "veiculos.criar", Descricao: "Criar veículos"),
            (Codigo: "veiculos.editar", Descricao: "Editar veículos"),
            (Codigo: "veiculos.deletar", Descricao: "Deletar veículos"),
            (Codigo: "veiculos.listar", Descricao: "Listar veículos"),
            (Codigo: "ordensservico.criar", Descricao: "Criar ordens de serviço"),
            (Codigo: "ordensservico.editar", Descricao: "Editar ordens de serviço"),
            (Codigo: "ordensservico.deletar", Descricao: "Deletar ordens de serviço"),
            (Codigo: "ordensservico.listar", Descricao: "Listar ordens de serviço"),
            (Codigo: "orcamentos.criar", Descricao: "Criar orçamentos"),
            (Codigo: "orcamentos.editar", Descricao: "Editar orçamentos"),
            (Codigo: "orcamentos.deletar", Descricao: "Deletar orçamentos"),
            (Codigo: "orcamentos.listar", Descricao: "Listar orçamentos"),
            (Codigo: "agendamentos.criar", Descricao: "Criar agendamentos"),
            (Codigo: "agendamentos.editar", Descricao: "Editar agendamentos"),
            (Codigo: "agendamentos.deletar", Descricao: "Deletar agendamentos"),
            (Codigo: "agendamentos.listar", Descricao: "Listar agendamentos"),
            (Codigo: "servicos.criar", Descricao: "Criar serviços"),
            (Codigo: "servicos.editar", Descricao: "Editar serviços"),
            (Codigo: "servicos.listar", Descricao: "Listar serviços"),
            (Codigo: "pecas.criar", Descricao: "Criar peças"),
            (Codigo: "pecas.editar", Descricao: "Editar peças"),
            (Codigo: "pecas.listar", Descricao: "Listar peças"),
        };
#pragma warning restore S1192

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
            ["ADMIN"] = permissoes.Select(x => x.Codigo).ToArray(),
            ["GERENTE"] = [
                "pessoas.criar",
                "pessoas.editar",
                "pessoas.deletar",
                "pessoas.listar",
                "veiculos.criar",
                "veiculos.editar",
                "veiculos.deletar",
                "veiculos.listar",
                "ordensservico.criar",
                "ordensservico.editar",
                "ordensservico.deletar",
                "ordensservico.listar",
                "orcamentos.criar",
                "orcamentos.editar",
                "orcamentos.deletar",
                "orcamentos.listar",
                "agendamentos.criar",
                "agendamentos.editar",
                "agendamentos.deletar",
                "agendamentos.listar",
                "servicos.criar",
                "servicos.editar",
                "servicos.listar",
                "pecas.criar",
                "pecas.editar",
                "pecas.listar"
            ],
            ["MECANICO"] = [
                "pessoas.listar",
                "veiculos.listar",
                "ordensservico.criar",
                "ordensservico.editar",
                "ordensservico.listar",
                "orcamentos.listar",
                "agendamentos.listar",
                "servicos.listar",
                "pecas.listar"
            ],
            ["ATENDENTE"] = [
                "pessoas.criar",
                "pessoas.editar",
                "pessoas.listar",
                "veiculos.criar",
                "veiculos.editar",
                "veiculos.listar",
                "agendamentos.criar",
                "agendamentos.editar",
                "agendamentos.listar",
                "orcamentos.listar"
            ],
            ["CLIENTE"] = [
                "agendamentos.criar",
                "agendamentos.listar",
                "orcamentos.listar",
                "veiculos.listar"
            ],
            ["CONSULTOR"] = [
                "pessoas.listar",
                "veiculos.listar",
                "agendamentos.criar",
                "agendamentos.editar",
                "agendamentos.listar",
                "orcamentos.listar"
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
}