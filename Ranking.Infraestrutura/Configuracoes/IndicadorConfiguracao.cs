using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ranking.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ranking.Infraestrutura.Configuracoes
{
    /// Configuração da entidade Indicador para o Entity Framework.
    /// Implementa as regras de mapeamento objeto-relacional.
    public class IndicadorConfiguracao : IEntityTypeConfiguration<Indicador>
    {
        /// Configura o mapeamento da entidade Indicador.
        public void Configure(EntityTypeBuilder<Indicador> builder)
        {
            // Configuração da tabela
            builder.ToTable("Indicadores");

            // Configuração da chave primária
            builder.HasKey(i => i.Id);

            // Configuração das propriedades
            builder.Property(i => i.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(i => i.Nome)
                .HasColumnName("Nome")
                .HasMaxLength(100)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(i => i.Descricao)
                .HasColumnName("Descricao")
                .HasMaxLength(500)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(i => i.DataCriacao)
                .HasColumnName("DataCriacao")
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(i => i.DataAtualizacao)
                .HasColumnName("DataAtualizacao")
                .HasColumnType("datetime2")
                .IsRequired(false);

            builder.Property(i => i.Tipo)
               .HasColumnName("Tipo")
               .IsRequired();

            // Configuração de índices
            builder.HasIndex(i => i.Nome)
                .HasDatabaseName("IX_Indicadores_Nome")
                .IsUnique();

            builder.HasIndex(i => i.DataCriacao)
                .HasDatabaseName("IX_Indicadores_DataCriacao");

            // Configuração de relacionamentos
            builder.HasMany(i => i.Metas)
                .WithOne(m => m.Indicador)
                .HasForeignKey(m => m.IndicadorId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Metas_Indicadores");

            // Configuração de validações no banco
            builder.HasCheckConstraint("CK_Indicadores_Nome_NaoVazio", "[Nome] <> ''");
            builder.HasCheckConstraint("CK_Indicadores_Descricao_NaoVazia", "[Descricao] <> ''");
        }
    }
}
