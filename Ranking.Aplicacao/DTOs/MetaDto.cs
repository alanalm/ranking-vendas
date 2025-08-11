using Ranking.Aplicacao.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Ranking.Aplicacao.DTOs;

public class MetaDto
{
    public int Id { get; set; }
    public int IndicadorId { get; set; }
    public string NomeIndicador { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public bool Ativa { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
}

public class CriarMetaDto : IMetaEditavel
{
    [Required]
    public int IndicadorId { get; set; }

    [Required]
    public decimal Valor { get; set; }

    public DateTime? DataInicio { get; set; }

    public DateTime? DataFim { get; set; }

    public bool Ativa { get; set; } = true; // Padrão para novas metas é ativa
    int IMetaEditavel.IndicadorId { get => IndicadorId; set => IndicadorId = value; }
    decimal IMetaEditavel.Valor { get => Valor; set => Valor = value; }
    DateTime IMetaEditavel.Data { get => DataInicio ?? default; set => DataInicio = value; }

}

public class AtualizarMetaDto : IMetaEditavel
{
    [Required]
    public int Id { get; set; }

    [Required]
    public int IndicadorId { get; set; }

    [Required]
    public decimal Valor { get; set; }

    public DateTime? DataInicio { get; set; }

    public DateTime? DataFim { get; set; }

    public bool Ativa { get; set; }
    int IMetaEditavel.IndicadorId { get => IndicadorId; set => IndicadorId = value; }
    decimal IMetaEditavel.Valor { get => Valor; set => Valor = value; }
    DateTime IMetaEditavel.Data { get => DataInicio ?? default; set => DataInicio = value; }

}