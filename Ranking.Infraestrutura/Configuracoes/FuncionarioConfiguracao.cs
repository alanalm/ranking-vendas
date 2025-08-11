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
    /// Configuração da entidade Funcionario para o Entity Framework.
    /// Implementa as regras de mapeamento objeto-relacional.
    public class FuncionarioConfiguracao : IEntityTypeConfiguration<Funcionario>
    {
        /// Configura o mapeamento da entidade Funcionario.
        /// <param name="builder">Construtor de configuração da entidade.</param>
        public void Configure(EntityTypeBuilder<Funcionario> builder)
        {
            // Configuração da tabela
            builder.ToTable("Funcionarios");

            // Configuração da chave primária
            builder.HasKey(f => f.Id);

            // Configuração das propriedades
            builder.Property(f => f.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(f => f.Nome)
                .HasColumnName("Nome")
                .HasMaxLength(100)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(f => f.DataCriacao)
                .HasColumnName("DataCriacao")
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(f => f.DataAtualizacao)
                .HasColumnName("DataAtualizacao")
                .HasColumnType("datetime2")
                .IsRequired(false);

            // Configuração de índices
            builder.HasIndex(f => f.Nome)
                .HasDatabaseName("IX_Funcionarios_Nome")
                .IsUnique();

            builder.HasIndex(f => f.DataCriacao)
                .HasDatabaseName("IX_Funcionarios_DataCriacao");

            // Configuração de relacionamentos
            builder.HasMany(f => f.Vendas)
                .WithOne(v => v.Funcionario)
                .HasForeignKey(v => v.FuncionarioId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Vendas_Funcionarios");

            // Configuração de validações no banco
            builder.HasCheckConstraint("CK_Funcionarios_Nome_NaoVazio", "[Nome] <> ''");
        }
    }
}
