using System.ComponentModel.DataAnnotations;

namespace Ranking.Aplicacao.DTOs
{
    public class EstatisticasRankingDto
    {
        public int TotalFuncionarios { get; set; }
        public int FuncionariosQueAtingiramMetas { get; set; }
        public int FuncionariosQueNaoAtingiramMetas { get; set; }
        public decimal PercentualAtingimentoMetas { get; set; }
        public decimal MediaDesempenho { get; set; }
        public decimal MelhorDesempenho { get; set; }
        public decimal PiorDesempenho { get; set; }
        public decimal TotalVendas { get; set; }
        public int QuantidadeTotalVendas { get; set; }
        public decimal MediaVendasPorFuncionario { get; set; }
        public bool AtingiuTodasAsMetas { get; set; }
        public DateTime? DataInicioPeriodo { get; set; }
        public DateTime? DataFimPeriodo { get; set; }
    }
}
