using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Ranking.Dominio.Entidades;
using Ranking.Dominio.Enums;
using Ranking.Infraestrutura.Configuracoes;

namespace Ranking.Infraestrutura.Persistencia
{
    public class RankingVendasContexto : DbContext
    {
        /// Construtor que recebe as opções de configuração do contexto.
        /// <param name="options">Opções de configuração do DbContext.</param>
        public RankingVendasContexto(DbContextOptions<RankingVendasContexto> options) : base(options)
        {
        }

        /// <summary>
        /// DbSet para a entidade Funcionario.
        /// </summary>
        public DbSet<Funcionario> Funcionarios { get; set; } = null!;

        /// <summary>
        /// DbSet para a entidade Indicador.
        /// </summary>
        public DbSet<Indicador> Indicadores { get; set; } = null!;

        /// <summary>
        /// DbSet para a entidade Meta.
        /// </summary>
        public DbSet<Meta> Metas { get; set; } = null!;

        /// <summary>
        /// DbSet para a entidade Venda.
        /// </summary>
        public DbSet<Venda> Vendas { get; set; } = null!;

        /// <summary>
        /// Configuração do modelo de dados.
        /// Aplica as configurações específicas de cada entidade.
        /// </summary>
        /// <param name="modelBuilder">Construtor do modelo.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplicar configurações das entidades
            modelBuilder.ApplyConfiguration(new FuncionarioConfiguracao());
            modelBuilder.ApplyConfiguration(new IndicadorConfiguracao());
            modelBuilder.ApplyConfiguration(new MetaConfiguracao());
            modelBuilder.ApplyConfiguration(new VendaConfiguracao());

            // Configurações globais
            ConfigurarConvencoesGlobais(modelBuilder);

            // Dados iniciais (seed data)
            ConfigurarDadosIniciais(modelBuilder);
        }

        /// <summary>
        /// Configurações globais aplicadas a todas as entidades.
        /// </summary>
        /// <param name="modelBuilder">Construtor do modelo.</param>
        private static void ConfigurarConvencoesGlobais(ModelBuilder modelBuilder)
        {
            // Configurar precisão decimal para propriedades monetárias
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                    {
                        property.SetPrecision(18);
                        property.SetScale(2);
                    }

                    // Configurar strings para não serem unicode por padrão (otimização)
                    if (property.ClrType == typeof(string))
                    {
                        property.SetIsUnicode(false);
                    }
                }
            }

            // Configurar comportamento de exclusão em cascata
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        /// <summary>
        /// Configuração de dados iniciais para o sistema.
        /// </summary>
        /// <param name="modelBuilder">Construtor do modelo.</param>
        private static void ConfigurarDadosIniciais(ModelBuilder modelBuilder)
        {
            // Indicadores padrão do sistema
            modelBuilder.Entity<Indicador>().HasData(
                new
                {
                    Id = 1,
                    Nome = "Quantidade de vendas",
                    Descricao = "Mede a quantidade de vendas realizada pelo funcionário.",
                    DataCriacao = DateTime.UtcNow,
                    Tipo = TipoIndicador.Quantidade
                },
                new
                {
                    Id = 2,
                    Nome = "Receita de Vendas",
                    Descricao = "Mede o valor das vendas realizado pelo funcionário.",
                    DataCriacao = DateTime.UtcNow,
                    Tipo = TipoIndicador.Receita
                }
            );

            // Metas padrão do sistema
            modelBuilder.Entity<Meta>().HasData(
                new
                {
                    Id = 1,
                    IndicadorId = 1,
                    Valor = 5m,
                    DataInicio = DateTime.UtcNow,
                    Ativa = true,
                    DataCriacao = DateTime.UtcNow
                },
                new
                {
                    Id = 2,
                    IndicadorId = 2,
                    Valor = 200.00m,
                    DataInicio = DateTime.UtcNow,
                    Ativa = true,
                    DataCriacao = DateTime.UtcNow
                }
            );
        }

        /// <summary>
        /// Sobrescrita do método SaveChanges para aplicar auditoria automática.
        /// </summary>
        /// <returns>Número de entidades afetadas.</returns>
        public override int SaveChanges()
        {
            AplicarAuditoriaAutomatica();
            return base.SaveChanges();
        }

        /// <summary>
        /// Sobrescrita do método SaveChangesAsync para aplicar auditoria automática.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Número de entidades afetadas.</returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AplicarAuditoriaAutomatica();
            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Aplica auditoria automática às entidades modificadas.
        /// Atualiza automaticamente as datas de criação e modificação.
        /// </summary>
        private void AplicarAuditoriaAutomatica()
        {
            var entradas = ChangeTracker.Entries<EntidadeBase>();

            foreach (var entrada in entradas)
            {
                switch (entrada.State)
                {
                    case EntityState.Added:
                        // Para entidades adicionadas, definir a data de criação
                        entrada.Property(e => e.DataCriacao).CurrentValue = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        // Para entidades modificadas, atualizar a data de modificação
                        entrada.Property(e => e.DataAtualizacao).CurrentValue = DateTime.UtcNow;
                        // Garantir que a data de criação não seja alterada
                        entrada.Property(e => e.DataCriacao).IsModified = false;
                        break;
                }
            }
        }
    }
}
