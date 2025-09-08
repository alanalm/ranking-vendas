using Ranking.Aplicacao.DTOs;
using Ranking.Aplicacao.Interfaces;
using Ranking.Dominio.Entidades;
using Ranking.Dominio.Enums;

public class CalculadoraDesempenhoServico : ICalculadoraDesempenhoServico
{
    public DesempenhoDto Calcular(Funcionario funcionario, List<Venda> vendas, List<Meta> metas, int metaMinima, TipoIndicador? indicadorSelecionado = null)
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

        bool atingiuMeta;
        if (indicadorSelecionado == null) // Geral
        {
            atingiuMeta = percentualQuantidade >= metaMinima && percentualValor >= metaMinima;
        }
        else if (indicadorSelecionado == TipoIndicador.Quantidade)
        {
            atingiuMeta = percentualQuantidade >= metaMinima;
        }
        else // Receita
        {
            atingiuMeta = percentualValor >= metaMinima;
        }

        var percentualAtingimento = Math.Round(Math.Max(percentualQuantidade, percentualValor), 2);
        var atingiuQuantidade = metaQuantidade > 0 ? percentualQuantidade >= metaMinima : true;
        var atingiuValor = metaValor > 0 ? percentualValor >= metaMinima : true;

        Console.WriteLine($"Funcionario: {funcionario.Nome}, MetaQtd: {metaQuantidade}, MetaVal: {metaValor}, PercentQtd: {percentualQuantidade}, PercentVal: {percentualValor}, AtingiuTodas: {atingiuQuantidade && atingiuValor}");


        return new DesempenhoDto
        {
            Geral = Math.Max(percentualQuantidade, percentualValor),
            QuantidadeVendas = quantidadeVendas,
            ValorVendas = valorTotalVendas,
            MetaQuantidade = metaQuantidade,
            MetaValor = metaValor,
            PercentualQuantidade = percentualQuantidade,
            PercentualValor = percentualValor,
            PercentualAtingimentoMetas = (metaQuantidade + metaValor) > 0
            ? Math.Round((percentualQuantidade + percentualValor) / 2, 2)
            : 0,
            AtingiuMetaQuantidade = atingiuQuantidade,
            AtingiuMetaValor = atingiuValor,
            AtingiuTodasAsMetas = atingiuQuantidade && atingiuValor
        };
    }

    public RankingDto CalcularRanking(Funcionario funcionario, List<Venda> vendasFuncionario, List<Meta> metas, DateTime? dataInicio, DateTime? dataFim,
        int metaMinima, TipoIndicador? indicadorSelecionado = null)
    {
        var desempenho = Calcular(funcionario, vendasFuncionario, metas, metaMinima, indicadorSelecionado);

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
