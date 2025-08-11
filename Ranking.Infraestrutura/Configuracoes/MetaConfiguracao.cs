using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ranking.Dominio.Entidades;

namespace Ranking.Infraestrutura.Configuracoes
{
    /// Configuração da entidade Meta para o Entity Framework.
    /// Implementa as regras de mapeamento objeto-relacional.
    public class MetaConfiguracao : IEntityTypeConfiguration<Meta>
    {
        /// Configura o mapeamento da entidade Meta.
        /// <param name="builder">Construtor de configuração da entidade.</param>
        public void Configure(EntityTypeBuilder<Meta> builder)
        {
            // Configuração da tabela
            builder.ToTable("Metas");

            // Configuração da chave primária
            builder.HasKey(m => m.Id);

            // Configuração das propriedades
            builder.Property(m => m.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(m => m.IndicadorId)
                .HasColumnName("IndicadorId")
                .IsRequired();

            builder.Property(m => m.Valor)
                .HasColumnName("Valor")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(m => m.DataInicio)
                .HasColumnName("DataInicio")
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(m => m.DataFim)
                .HasColumnName("DataFim")
                .HasColumnType("datetime2")
                .IsRequired(false);

            builder.Property(m => m.Ativa)
                .HasColumnName("Ativa")
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(m => m.DataCriacao)
                .HasColumnName("DataCriacao")
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(m => m.DataAtualizacao)
                .HasColumnName("DataAtualizacao")
                .HasColumnType("datetime2")
                .IsRequired(false);

            // Configuração de índices
            builder.HasIndex(m => m.IndicadorId)
                .HasDatabaseName("IX_Metas_IndicadorId");

            builder.HasIndex(m => m.Ativa)
                .HasDatabaseName("IX_Metas_Ativa");

            builder.HasIndex(m => new { m.IndicadorId, m.Ativa })
                .HasDatabaseName("IX_Metas_IndicadorId_Ativa");

            builder.HasIndex(m => m.DataInicio)
                .HasDatabaseName("IX_Metas_DataInicio");

            builder.HasIndex(m => m.DataFim)
                .HasDatabaseName("IX_Metas_DataFim");

            builder.HasIndex(m => m.DataCriacao)
                .HasDatabaseName("IX_Metas_DataCriacao");

            // Configuração de relacionamentos
            builder.HasOne(m => m.Indicador)
                .WithMany(i => i.Metas)
                .HasForeignKey(m => m.IndicadorId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Metas_Indicadores");

            // Configuração de validações no banco
            builder.HasCheckConstraint("CK_Metas_Valor_Positivo", "[Valor] > 0");
            builder.HasCheckConstraint("CK_Metas_DataFim_Posterior", "[DataFim] IS NULL OR [DataFim] > [DataInicio]");
        }
    }
}
