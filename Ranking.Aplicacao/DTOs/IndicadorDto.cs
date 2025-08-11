using Ranking.Dominio.Enums;
using System.ComponentModel.DataAnnotations;

namespace Ranking.Aplicacao.DTOs
{
    public class IndicadorDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public TipoIndicador Tipo { get; set; }
        public MetaDto? MetaAtiva { get; set; }
    }

    public class CriarIndicadorDto
    {
        [Required]
        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public TipoIndicador Tipo { get; set; } = TipoIndicador.Nenhum;
    }

    public class AtualizarIndicadorDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public TipoIndicador Tipo { get; set; }
    }
}
