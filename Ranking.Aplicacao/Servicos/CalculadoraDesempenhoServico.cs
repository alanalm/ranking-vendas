using Ranking.Aplicacao.DTOs;
using Ranking.Aplicacao.Interfaces;
using Ranking.Dominio.Entidades;
using Ranking.Dominio.Enums;

public class CalculadoraDesempenhoServico : ICalculadoraDesempenhoServico
{
    public DesempenhoDto Calcular(Funcionario funcionario, List<Venda> vendas, List<Meta> metas)
    {
        var quantidadeVendas = vendas.Count;
        var valorTotalVendas = vendas.Sum(v => v.Valor);

        var metasAtivas = metas.Where(m => m.Ativa).ToList();
        var metasAtivasComIndicador = metasAtivas.Where(m => m.Indicador != null).ToList();

        var metaQuantidade = metasAtivasComIndicador
            .Where(m => m.Indicador.Tipo == TipoIndicador.Quantidade)
            .Sum(m => m.Valor);

        var metaValor = metasAtivasComIndicador
            .Where(m => m.Indicador.Tipo == TipoIndicador.Receita)
            .Sum(m => m.Valor);

        var percentualQuantidade = metaQuantidade > 0
            ? ((decimal)quantidadeVendas / metaQuantidade) * 100
            : 0;

        var percentualValor = metaValor > 0
            ? (valorTotalVendas / metaValor) * 100
            : 0;

        var percentualAtingimento = Math.Round(Math.Max(percentualQuantidade, percentualValor), 2);
        var atingiuTodasAsMetas = percentualQuantidade >= 100 && percentualValor >= 100;

        return new DesempenhoDto
        {
            Geral = percentualAtingimento,
            QuantidadeVendas = quantidadeVendas,
            ValorVendas = valorTotalVendas,
            MetaQuantidade = metaQuantidade,
            MetaValor = metaValor,
            PercentualQuantidade = percentualQuantidade,
            PercentualValor = percentualValor,
            PercentualAtingimentoMetas = Math.Round((percentualQuantidade + percentualValor) / 2, 2),
            AtingiuTodasAsMetas = atingiuTodasAsMetas
        };
    }

    public RankingDto CalcularRanking(Funcionario funcionario, List<Venda> vendasFuncionario, List<Meta> metas, DateTime? dataInicio, DateTime? dataFim)
    {
        var desempenho = Calcular(funcionario, vendasFuncionario, metas);

        return new RankingDto
        {
            Funcionario = new FuncionarioDto
            {
                Id = funcionario.Id,
                Nome = funcionario.Nome
            },
            Vendas = new VendasDto
            {
                Quantidade = (int)desempenho.QuantidadeVendas,
                ValorTotal = desempenho.ValorVendas
            },
            Metas = new MetasDto
            {
                QuantidadeVendas = desempenho.MetaQuantidade,
                ValorVendas = desempenho.MetaValor,
                Ativa = metas.Any(m => m.Ativa)
            },
            Desempenho = desempenho,
            DataInicio = dataInicio,
            DataFim = dataFim
        };
    }
}
