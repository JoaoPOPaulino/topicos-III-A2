using A2.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using A2.Models.Enums;

namespace A2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ------------------------------------
        // --- 1. SETS DE DADOS (TABELAS) ---
        // ------------------------------------
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Moeda> Moedas { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<CategoriaDespesa> CategoriasDespesa { get; set; }

        // Módulo Adiantamentos
        public DbSet<SolicitacaoAdiantamento> SolicitacoesAdiantamento { get; set; }
        public DbSet<AprovacaoAdiantamento> AprovacoesAdiantamento { get; set; }

        // Módulo Prestação de Contas
        public DbSet<PrestacaoContas> PrestacoesContas { get; set; }
        public DbSet<Despesa> Despesas { get; set; }
        public DbSet<ComprovanteDespesa> ComprovantesDespesa { get; set; }
        public DbSet<AprovacaoPrestacao> AprovacoesPrestacao { get; set; }

        // Módulo Financeiro
        public DbSet<Pagamento> Pagamentos { get; set; }

        // Módulo Auditoria e Integrações
        public DbSet<LogCotacao> LogsCotacao { get; set; }
        public DbSet<Feriado> Feriados { get; set; }
        public DbSet<LogAuditoria> LogsAuditoria { get; set; }


        // ------------------------------------
        // --- 2. CONFIGURAÇÃO DO MODELO ---
        // ------------------------------------
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Configurações Específicas de Relacionamentos ---

            // 1. SolicitacaoAdiantamento (Colaborador e CriadoPor referenciam Usuario)
            modelBuilder.Entity<SolicitacaoAdiantamento>()
                .HasOne(s => s.Colaborador)
                .WithMany()
                .HasForeignKey(s => s.ColaboradorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SolicitacaoAdiantamento>()
                .HasOne(s => s.CriadoPor)
                .WithMany()
                .HasForeignKey(s => s.CriadoPorId)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. Pagamento (Beneficiario e ProcessadoPor referenciam Usuario)
            modelBuilder.Entity<Pagamento>()
                .HasOne(p => p.Beneficiario)
                .WithMany()
                .HasForeignKey(p => p.BeneficiarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pagamento>()
                .HasOne(p => p.ProcessadoPor)
                .WithMany()
                .HasForeignKey(p => p.ProcessadoPorId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. LogCotacao (Moedas)
            modelBuilder.Entity<LogCotacao>()
                .HasOne(lc => lc.MoedaBase)
                .WithMany()
                .HasForeignKey(lc => lc.MoedaBaseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LogCotacao>()
                .HasOne(lc => lc.MoedaCotada)
                .WithMany()
                .HasForeignKey(lc => lc.MoedaCotadaId)
                .OnDelete(DeleteBehavior.Restrict);


            // --- SEED INICIAL DE DADOS ---

            // Seed 1: Empresa
            modelBuilder.Entity<Empresa>().HasData(
                new Empresa
                {
                    Id = 1,
                    Nome = "FinOps Pro Ltda",
                    Cnpj = "00.000.000/0001-00",
                    RazaoSocial = "FinOps Pro Ltda"
                }
            );

            // Seed 2: Moedas
            modelBuilder.Entity<Moeda>().HasData(
                new Moeda { Id = 1, Codigo = "BRL", Nome = "Real Brasileiro", Simbolo = "R$" },
                new Moeda { Id = 2, Codigo = "USD", Nome = "Dólar Americano", Simbolo = "$" },
                new Moeda { Id = 3, Codigo = "EUR", Nome = "Euro", Simbolo = "€" }
            );

            // Seed 3: Departamento
            modelBuilder.Entity<Departamento>().HasData(
                new Departamento { Id = 1, Nome = "Recursos Humanos", CentroDeCusto = "CC001", Ativo = true }
            );

            // Seed 4: Usuários (Dados básicos para testar a criação e listagem)
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    DepartamentoId = 1,
                    NomeCompleto = "Admin FinOps (RH)",
                    Email = "admin@finops.com",
                    Cpf = "00000000000",
                    Perfil = PerfilUsuario.RH, // Perfil RH para criação
                    Ativo = true,
                    CriadoEm = DateTime.Parse("2025-01-01")
                },
                new Usuario
                {
                    Id = 2,
                    DepartamentoId = 1,
                    NomeCompleto = "Lucas Henderson",
                    Email = "lucas@finops.com",
                    Cpf = "11111111111",
                    Perfil = PerfilUsuario.Colaborador,
                    Ativo = true,
                    CriadoEm = DateTime.Parse("2025-01-01")
                },
                new Usuario
                {
                    Id = 3,
                    DepartamentoId = 1,
                    NomeCompleto = "Ana Costa",
                    Email = "ana@finops.com",
                    Cpf = "22222222222",
                    Perfil = PerfilUsuario.Colaborador,
                    Ativo = true,
                    CriadoEm = DateTime.Parse("2025-01-01")
                }
            );

            // Seed 5: Categoria de Despesa
            modelBuilder.Entity<CategoriaDespesa>().HasData(
                new CategoriaDespesa { Id = 1, Nome = "Transporte", Descricao = "Despesas com locomoção", Ativo = true },
                new CategoriaDespesa { Id = 2, Nome = "Alimentação", Descricao = "Despesas com refeições", Ativo = true },
                new CategoriaDespesa { Id = 3, Nome = "Hospedagem", Descricao = "Despesas com estadias", Ativo = true },
                new CategoriaDespesa { Id = 4, Nome = "Material", Descricao = "Material de Escritório", Ativo = true },
                new CategoriaDespesa { Id = 5, Nome = "Outros", Descricao = "Outras despesas gerais", Ativo = true }
            );


            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}