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
    /// Configuração da entidade Venda para o Entity Framework.
    /// Implementa as regras de mapeamento objeto-relacional.
    public class VendaConfiguracao : IEntityTypeConfiguration<Venda>
    {
        /// Configura o mapeamento da entidade Venda.
        /// <param name="builder">Construtor de configuração da entidade.</param>
        public void Configure(EntityTypeBuilder<Venda> builder)
        {
            // Configuração da tabela
            builder.ToTable("Vendas");

            // Configuração da chave primária
            builder.HasKey(v => v.Id);

            // Configuração das propriedades
            builder.Property(v => v.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(v => v.FuncionarioId)
                .HasColumnName("FuncionarioId")
                .IsRequired();

            builder.Property(v => v.Valor)
                .HasColumnName("Valor")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(v => v.DataVenda)
                .HasColumnName("DataVenda")
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(v => v.Descricao)
                .HasColumnName("Descricao")
                .HasMaxLength(500)
                .IsRequired(false)
                .IsUnicode(false)
                .HasDefaultValue(string.Empty);

            builder.Property(v => v.DataCriacao)
                .HasColumnName("DataCriacao")
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(v => v.DataAtualizacao)
                .HasColumnName("DataAtualizacao")
                .HasColumnType("datetime2")
                .IsRequired(false);

            // Configuração de índices
            builder.HasIndex(v => v.FuncionarioId)
                .HasDatabaseName("IX_Vendas_FuncionarioId");

            builder.HasIndex(v => v.DataVenda)
                .HasDatabaseName("IX_Vendas_DataVenda");

            builder.HasIndex(v => v.Valor)
                .HasDatabaseName("IX_Vendas_Valor");

            builder.HasIndex(v => new { v.FuncionarioId, v.DataVenda })
                .HasDatabaseName("IX_Vendas_FuncionarioId_DataVenda");

            builder.HasIndex(v => new { v.DataVenda, v.Valor })
                .HasDatabaseName("IX_Vendas_DataVenda_Valor");

            builder.HasIndex(v => v.DataCriacao)
                .HasDatabaseName("IX_Vendas_DataCriacao");

            // Configuração de relacionamentos
            builder.HasOne(v => v.Funcionario)
                .WithMany(f => f.Vendas)
                .HasForeignKey(v => v.FuncionarioId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Vendas_Funcionarios");

            // Configuração de validações no banco
            builder.HasCheckConstraint("CK_Vendas_Valor_Positivo", "[Valor] > 0");
            builder.HasCheckConstraint("CK_Vendas_DataVenda_NaoFuturo", "[DataVenda] <= GETUTCDATE()");
        }
    }
}
