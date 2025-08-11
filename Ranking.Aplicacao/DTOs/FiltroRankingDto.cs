using System.ComponentModel.DataAnnotations;

namespace Ranking.Aplicacao.DTOs
{
    public class FiltroRankingDto
    {
        public DateTime? DataInicio { get; set; }

        public DateTime? DataFim { get; set; }

        public TipoOrdenacaoRanking TipoOrdenacao { get; set; } = TipoOrdenacaoRanking.DesempenhoGeral;

        public FiltroRankingDto() { }

        public FiltroRankingDto(DateTime? dataInicio, DateTime? dataFim, TipoOrdenacaoRanking tipoOrdenacao = TipoOrdenacaoRanking.DesempenhoGeral)
        {
            DataInicio = dataInicio;
            DataFim = dataFim;
            TipoOrdenacao = tipoOrdenacao;
        }
    }

    /// Enumeração para tipos de ordenação do ranking.
    public enum TipoOrdenacaoRanking
    {
        /// Ordenação por desempenho geral (média dos indicadores).
        [Display(Name = "Desempenho Geral")]
        DesempenhoGeral = 0,

        /// Ordenação por quantidade de vendas.
        [Display(Name = "Quantidade de Vendas")]
        QuantidadeVendas = 1,

        /// Ordenação por valor total de vendas.
        [Display(Name = "Valor Total de Vendas")]
        ValorTotalVendas = 2,

        /// Ordenação por percentual de atingimento das metas.
        [Display(Name = "Percentual de Atingimento")]
        PercentualAtingimento = 3,

        /// Ordenação alfabética por nome do funcionário.
        [Display(Name = "Nome do Funcionário")]
        NomeFuncionario = 4
    }
}
