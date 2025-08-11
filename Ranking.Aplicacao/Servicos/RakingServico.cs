using Aplicacao.Utils;
using AutoMapper;
using Ranking.Aplicacao.DTOs;
using Ranking.Aplicacao.Interfaces;
using Ranking.Dominio.Entidades;
using Ranking.Dominio.Interfaces.Repositorios;

public class RankingServico : IRankingServico
{
    private readonly IRepositorioFuncionario _repositorioFuncionario;
    private readonly IRepositorioVenda _repositorioVenda;
    private readonly IRepositorioMeta _repositorioMeta;
    private readonly ICalculadoraDesempenhoServico _calculadoraDesempenho;
    private readonly IMapper _mapper;

    public RankingServico(
        IRepositorioFuncionario repositorioFuncionario,
        IRepositorioVenda repositorioVenda,
        IRepositorioMeta repositorioMeta,
        ICalculadoraDesempenhoServico calculadoraDesempenho,
        IMapper mapper)
    {
        _repositorioFuncionario = repositorioFuncionario;
        _repositorioVenda = repositorioVenda;
        _repositorioMeta = repositorioMeta;
        _calculadoraDesempenho = calculadoraDesempenho;
        _mapper = mapper;
    }

    public async Task<ResultadoOperacao<IEnumerable<RankingDto>>> CalcularRankingAsync(FiltroRankingDto? filtro = null)
    {
        filtro ??= new FiltroRankingDto
        {
            TipoOrdenacao = TipoOrdenacaoRanking.DesempenhoGeral
        };

        var funcionarios = await _repositorioFuncionario.ObterTodosAsync();
        var vendas = await FiltrarVendasAsync(filtro.DataInicio, filtro.DataFim);
        var metas = await FiltrarMetasAsync(filtro.DataInicio, filtro.DataFim);

        var listaRanking = funcionarios.Select(funcionario =>
        {
            var vendasFuncionario = vendas.Where(v => v.FuncionarioId == funcionario.Id).ToList();
            return _calculadoraDesempenho.CalcularRanking(funcionario, vendasFuncionario, metas, filtro.DataInicio, filtro.DataFim);
        }).ToList();

        var rankingOrdenado = (filtro.TipoOrdenacao switch
        {
            TipoOrdenacaoRanking.DesempenhoGeral => listaRanking.OrderByDescending(dto => dto.Desempenho.Geral),
            TipoOrdenacaoRanking.QuantidadeVendas => listaRanking.OrderByDescending(dto => dto.Desempenho.QuantidadeVendas),
            TipoOrdenacaoRanking.PercentualAtingimento => listaRanking.OrderByDescending(dto => dto.Desempenho.PercentualAtingimentoMetas),
            _ => listaRanking.OrderByDescending(dto => dto.Desempenho.ValorVendas)
        }).ToList();

        for (int i = 0; i < rankingOrdenado.Count; i++)
            rankingOrdenado[i].Posicao = i + 1;

        return ResultadoOperacao<IEnumerable<RankingDto>>.CriarSucesso(rankingOrdenado);
    }

    public Task<ResultadoOperacao<IEnumerable<RankingDto>>> CalcularRankingPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        => CalcularRankingAsync(new FiltroRankingDto { DataInicio = dataInicio, DataFim = dataFim });

    public async Task<ResultadoOperacao<DesempenhoDto?>> ObterDesempenhoFuncionarioAsync(int funcionarioId, DateTime? dataInicio = null, DateTime? dataFim = null)
    {
        var funcionario = await _repositorioFuncionario.ObterPorIdAsync(funcionarioId);
        if (funcionario == null)
            return ResultadoOperacao<DesempenhoDto?>.CriarFalha("Funcionário não encontrado.");

        var vendas = (await FiltrarVendasAsync(dataInicio, dataFim))
            .Where(v => v.FuncionarioId == funcionarioId)
            .ToList();

        var metas = (await FiltrarMetasAsync(dataInicio, dataFim))
            .Where(m => m.Ativa)
            .ToList();

        var desempenho = new DesempenhoDto(funcionario, vendas, metas, dataInicio, dataFim);
        return ResultadoOperacao<DesempenhoDto>.CriarSucesso(desempenho);
    }

    public async Task<ResultadoOperacao<EstatisticasRankingDto>> CalcularEstatisticasAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
    {
        var rankingResultado = await CalcularRankingAsync(new FiltroRankingDto
        {
            DataInicio = dataInicio,
            DataFim = dataFim
        });

        if (!rankingResultado.Sucesso || rankingResultado.Dados == null)
            return ResultadoOperacao<EstatisticasRankingDto>.CriarFalha("Falha ao calcular estatísticas.");

        var ranking = rankingResultado.Dados.ToList();

        var estatisticas = new EstatisticasRankingDto
        {
            TotalFuncionarios = ranking.Count,
            TotalVendas = ranking.Sum(r => r.Desempenho.ValorVendas),
            MediaVendasPorFuncionario = ranking.Average(r => r.Desempenho.ValorVendas),
            MediaDesempenho = ranking.Average(r => r.Desempenho.PercentualAtingimentoMetas),
            MelhorDesempenho = ranking.Max(r => r.Desempenho.MetaValor),
            FuncionariosQueAtingiramMetas = ranking.Count(r => r.Desempenho.PercentualAtingimentoMetas >= 100),
            PercentualAtingimentoMetas = (decimal)(ranking.Count > 0
                ? ranking.Count(r => r.Desempenho.PercentualAtingimentoMetas >= 100) * 100.0 / ranking.Count
                : 0)
        };

        return ResultadoOperacao<EstatisticasRankingDto>.CriarSucesso(estatisticas);
    }

    public async Task<ResultadoOperacao<IEnumerable<RankingDto>>> ObterFuncionariosQueAtingiramMetasAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
    {
        var ranking = await CalcularRankingAsync(new FiltroRankingDto
        {
            DataInicio = dataInicio,
            DataFim = dataFim
        });

        if (!ranking.Sucesso || ranking.Dados == null)
            return ResultadoOperacao<IEnumerable<RankingDto>>.CriarFalha("Erro ao calcular ranking.");

        var atingiram = ranking.Dados.Where(r => r.Desempenho.PercentualAtingimentoMetas >= 100).ToList();
        return ResultadoOperacao<IEnumerable<RankingDto>>.CriarSucesso(atingiram);
    }

    public async Task<ResultadoOperacao<IEnumerable<RankingDto>>> ObterFuncionariosQueNaoAtingiramMetasAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
    {
        var ranking = await CalcularRankingAsync(new FiltroRankingDto
        {
            DataInicio = dataInicio,
            DataFim = dataFim
        });

        if (!ranking.Sucesso || ranking.Dados == null)
            return ResultadoOperacao<IEnumerable<RankingDto>>.CriarFalha("Erro ao calcular ranking.");

        var naoAtingiram = ranking.Dados.Where(r => r.Desempenho.PercentualAtingimentoMetas < 100).ToList();
        return ResultadoOperacao<IEnumerable<RankingDto>>.CriarSucesso(naoAtingiram);
    }

    public async Task<ResultadoOperacao<bool>> ExistemMetasAtivasAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
    {
        var inicio = dataInicio ?? DateTime.MinValue;
        var fim = dataFim ?? DateTime.MaxValue;

        var existe = await _repositorioMeta.ExisteMetaAtivaNoPeriodo(inicio, fim);

        var metas = await FiltrarMetasAsync(dataInicio, dataFim);
        Console.WriteLine($"[DEBUG API] Existem {metas.Count} metas ativas no período? {existe}");

        return ResultadoOperacao<bool>.CriarSucesso(existe);
    }

    // Métodos privados para evitar duplicação
    private async Task<List<Venda>> FiltrarVendasAsync(DateTime? inicio, DateTime? fim)
    {
        var vendas = await _repositorioVenda.ObterTodosAsync();

        if (inicio.HasValue)
            vendas = vendas.Where(v => v.DataVenda >= inicio.Value).ToList();

        if (fim.HasValue)
            vendas = vendas.Where(v => v.DataVenda <= fim.Value).ToList();

        return vendas.ToList();
    }

    private async Task<List<Meta>> FiltrarMetasAsync(DateTime? inicio, DateTime? fim)
    {
        var metas = await _repositorioMeta.ObterTodosAsync();

        return metas
            .Where(m =>
                (!inicio.HasValue || m.DataFim == null || m.DataFim >= inicio.Value) &&
                (!fim.HasValue || m.DataInicio <= fim.Value))
            .ToList();
    }
}
