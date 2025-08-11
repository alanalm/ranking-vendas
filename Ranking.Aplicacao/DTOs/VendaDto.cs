using Ranking.Aplicacao.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Ranking.Aplicacao.DTOs;

public class VendaDto : IVendaEditavel
{
    public int Id { get; set; }
    public int FuncionarioId { get; set; }
    public string NomeFuncionario { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataVenda { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    DateTime? IVendaEditavel.DataVenda { get => DataVenda ; set => DataVenda = value.Value ; }
}

public class CriarVendaDto : IVendaEditavel
{
    [Required]
    public int FuncionarioId { get; set; }

    [Required]
    public decimal Valor { get; set; }

    [Required]
    public DateTime? DataVenda { get; set; }

    public string Descricao { get; set; } = string.Empty;
}

public class AtualizarVendaDto : IVendaEditavel
{
    [Required]
    public int Id { get; set; }

    [Required]
    public int FuncionarioId { get; set; }

    [Required]
    public decimal Valor { get; set; }

    [Required]
    public DateTime? DataVenda { get; set; }

    public string Descricao { get; set; } = string.Empty;
}

