using Ranking.Aplicacao.DTOs;
using Ranking.Dominio.Entidades;
using Ranking.Dominio.Enums;

namespace Ranking.Aplicacao.Interfaces
{
    public interface ICalculadoraDesempenhoServico
    {
        DesempenhoDto Calcular(Funcionario funcionario, List<Venda> vendas, List<Meta> metas, int metaMinima, TipoIndicador? indicadorSelecionado = null);

        RankingDto CalcularRanking(Funcionario funcionario, List<Venda> vendas, List<Meta> metas, DateTime? dataInicio, DateTime? dataFim,
        int metaMinima, TipoIndicador? indicadorSelecionado = null);
    }
}
