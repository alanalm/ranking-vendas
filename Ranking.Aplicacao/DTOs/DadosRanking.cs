using Ranking.Dominio.Entidades;

namespace Ranking.Aplicacao.DTOs
{
    public class DadosRanking
    {
        public Funcionario Funcionario { get; set; }
        public VendasDto Vendas { get; set; }
        public MetasDto Metas { get; set; }
        public DesempenhoDto Desempenho { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
    }
}
