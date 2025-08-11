using Ranking.Aplicacao.DTOs;
using Ranking.Dominio.Entidades;

namespace Ranking.Aplicacao.Interfaces
{
    public interface ICalculadoraDesempenhoServico
    {
        DesempenhoDto Calcular(Funcionario funcionario, List<Venda> vendas, List<Meta> metas);

        RankingDto CalcularRanking(Funcionario funcionario, List<Venda> vendas, List<Meta> metas, DateTime? dataInicio, DateTime? dataFim);
    }
}
