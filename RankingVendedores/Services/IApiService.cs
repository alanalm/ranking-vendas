using Ranking.Aplicacao.DTOs;
using RankingVendedores.DTOs;
using System.Threading.Tasks;

namespace RankingVendedores.Services
{
    // Interface para serviço de comunicação com a API.
    // Define operações para todas as entidades do sistema.
    // Implementa abstração da camada de comunicação.
    public interface IApiService
    {
        #region Funcionários

        Task<ResultadoOperacao<List<FuncionarioDto>>> ObterFuncionariosAsync();

        Task<FuncionarioDto?> ObterFuncionarioPorIdAsync(int id);

        Task<List<FuncionarioDto>> PesquisarFuncionariosAsync(string nome);

        Task<FuncionarioDto> CriarFuncionarioAsync(CriarFuncionarioDto funcionario);

        Task<FuncionarioDto> AtualizarFuncionarioAsync(AtualizarFuncionarioDto funcionario);

        Task RemoverFuncionarioAsync(int id);

        Task<bool> PodeFuncionarioSerRemovidoAsync(int id);

        #endregion

        #region Indicadores

        Task<ResultadoOperacao<List<IndicadorDto>>> ObterIndicadoresAsync();

        Task<ResultadoOperacao<IndicadorDto>> ObterIndicadorPorIdAsync(int id);

        Task<ResultadoOperacao<IEnumerable<IndicadorDto>>> PesquisarIndicadoresAsync(string nome);

        Task<ResultadoOperacao<IndicadorDto>> CriarIndicadorAsync(CriarIndicadorDto indicador);

        Task<ResultadoOperacao<IndicadorDto>> AtualizarIndicadorAsync(AtualizarIndicadorDto indicador);

        Task<ResultadoOperacao<bool>> RemoverIndicadorAsync(int id);

        Task<ResultadoOperacao<bool>> PodeIndicadorSerRemovidoAsync(int id);

        #endregion

        #region Metas

        Task<List<MetaDto>> ObterMetasAsync();

        Task<ResultadoOperacao<MetaDto>> ObterMetaPorIdAsync(int id);
        Task<ResultadoOperacao<List<MetaDto>>> ObterMetasPorIndicadorAsync(int indicadorId);

        Task<ResultadoOperacao<List<MetaDto>>> ObterMetasComResultadoAsync();
        //Task<IEnumerable<MetaDto>> ObterMetasAtivasAsync();

        Task<ResultadoOperacao<MetaDto>> CriarMetaAsync(CriarMetaDto meta);

        Task<ResultadoOperacao<MetaDto>> AtualizarMetaAsync(AtualizarMetaDto meta);

        Task RemoverMetaAsync(int id);

        Task<ResultadoOperacao<bool>> AtivarMetaAsync(int id);

        Task<ResultadoOperacao<bool>> DesativarMetaAsync(int id);

        #endregion

        #region Vendas
        Task<IEnumerable<VendaDto>> ObterVendasAsync();

        Task<VendaDto?> ObterVendaPorIdAsync(int id);

        Task<IEnumerable<VendaDto>> ObterVendasPorFuncionarioAsync(int funcionarioId);

        Task<IEnumerable<VendaDto>> ObterVendasPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);

        Task<VendaDto> CriarVendaAsync(CriarVendaDto venda);

        Task<VendaDto> AtualizarVendaAsync(AtualizarVendaDto venda);

        Task<List<VendaDto>> PesquisarVendasAsync(string texto, DateTime? dataInicio, DateTime? dataFim);

        Task RemoverVendaAsync(int id);

        Task<EstatisticasVendasDto> ObterEstatisticasVendasAsync(DateTime? dataInicio, DateTime? dataFim);

        #endregion

        #region Ranking

        Task<IEnumerable<RankingDto>> CalcularRankingAsync(FiltroRankingDto? filtro = null);

        Task<IEnumerable<RankingDto>> CalcularRankingPeriodoAtualAsync();

        Task<RankingDto?> ObterDesempenhoFuncionarioAsync(int funcionarioId, DateTime? dataInicio = null, DateTime? dataFim = null);

        Task<EstatisticasRankingDto> CalcularEstatisticasRankingAsync(DateTime? dataInicio = null, DateTime? dataFim = null);

        Task<IEnumerable<RankingDto>> ObterFuncionariosQueAtingiramMetasAsync(DateTime? dataInicio = null, DateTime? dataFim = null);

        Task<IEnumerable<RankingDto>> ObterFuncionariosQueNaoAtingiramMetasAsync(DateTime? dataInicio = null, DateTime? dataFim = null);

        Task<bool> VerificarMetasAtivasAsync(DateTime? dataInicio, DateTime? dataFim);

        #endregion
    }
}