using Microsoft.AspNetCore.Mvc;
using Ranking.Aplicacao.DTOs;
using Ranking.Aplicacao.Interfaces;
using EstatisticasRankingDto = Ranking.Aplicacao.DTOs.EstatisticasRankingDto;

namespace RankingVendedores.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Ranking")]
    public class RankingController : ControllerBase
    {
        private readonly IRankingServico _servicoRanking;

        public RankingController(IRankingServico servicoRanking)
        {
            _servicoRanking = servicoRanking ?? throw new ArgumentNullException(nameof(servicoRanking));
        }

        [HttpGet("periodo-atual")]
        public IActionResult ObterPeriodoAtual()
        {
            var inicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var fim = inicio.AddMonths(1).AddDays(-1);
            return Ok(new { inicio, fim });
        }
        [HttpGet]
        [ProducesResponseType(typeof(RespostaApi<IEnumerable<RankingDto>>), 200)]
        [ProducesResponseType(typeof(RespostaApi<object>), 400)]
        [ProducesResponseType(typeof(RespostaApi<object>), 500)]
        public async Task<IActionResult> CalcularRankingAsync(
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim,
            [FromQuery] TipoOrdenacaoRanking tipoOrdenacao = TipoOrdenacaoRanking.DesempenhoGeral)
        {
            try
            {
                if (dataInicio.HasValue && dataFim.HasValue && dataInicio.Value > dataFim.Value)
                    return CriarRespostaErro("A data de início deve ser anterior à data de fim.", 400);

                var filtro = new FiltroRankingDto(dataInicio, dataFim)
                {
                    TipoOrdenacao = tipoOrdenacao
                };

                var resultado = await _servicoRanking.CalcularRankingAsync(filtro);
                return ApartirDe(resultado);
            }
            catch (Exception ex)
            {
                return TratarExcecao(ex);
            }
        }

        [HttpGet("por-periodo")]
        [ProducesResponseType(typeof(RespostaApi<IEnumerable<RankingDto>>), 200)]
        [ProducesResponseType(typeof(RespostaApi<object>), 400)]
        [ProducesResponseType(typeof(RespostaApi<object>), 500)]
        public async Task<IActionResult> CalcularRankingPorPeriodo(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim)
        {
            try
            {
                if (dataInicio > dataFim)
                    return CriarRespostaErro("A data de início deve ser anterior à data de fim.", 400);

                var resultado = await _servicoRanking.CalcularRankingPorPeriodoAsync(dataInicio, dataFim);
                return ApartirDe(resultado);
            }
            catch (Exception ex)
            {
                return TratarExcecao(ex);
            }
        }

        [HttpGet("funcionario/{funcionarioId:int}")]
        [ProducesResponseType(typeof(RespostaApi<RankingDto>), 200)]
        [ProducesResponseType(typeof(RespostaApi<object>), 400)]
        [ProducesResponseType(typeof(RespostaApi<object>), 404)]
        [ProducesResponseType(typeof(RespostaApi<object>), 500)]
        public async Task<IActionResult> ObterDesempenhoFuncionario(
            int funcionarioId,
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim)
        {
            try
            {
                if (funcionarioId <= 0)
                    return CriarRespostaErro("ID do funcionário deve ser maior que zero.", 400);

                if (dataInicio.HasValue && dataFim.HasValue && dataInicio.Value > dataFim.Value)
                    return CriarRespostaErro("A data de início deve ser anterior à data de fim.", 400);

                var resultado = await _servicoRanking.ObterDesempenhoFuncionarioAsync(funcionarioId, dataInicio, dataFim);
                return ApartirDe(resultado);
            }
            catch (Exception ex)
            {
                return TratarExcecao(ex);
            }
        }

        [HttpGet("estatisticas")]
        [ProducesResponseType(typeof(RespostaApi<EstatisticasRankingDto>), 200)]
        [ProducesResponseType(typeof(RespostaApi<object>), 400)]
        [ProducesResponseType(typeof(RespostaApi<object>), 500)]
        public async Task<IActionResult> CalcularEstatisticas(
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim)
        {
            try
            {
                if (dataInicio.HasValue && dataFim.HasValue && dataInicio.Value > dataFim.Value)
                    return CriarRespostaErro("A data de início deve ser anterior à data de fim.", 400);

                var resultado = await _servicoRanking.CalcularEstatisticasAsync(dataInicio, dataFim);
                return ApartirDe(resultado);
            }
            catch (Exception ex)
            {
                return TratarExcecao(ex);
            }
        }

        [HttpGet("atingiram-metas")]
        [ProducesResponseType(typeof(RespostaApi<IEnumerable<RankingDto>>), 200)]
        [ProducesResponseType(typeof(RespostaApi<object>), 400)]
        [ProducesResponseType(typeof(RespostaApi<object>), 500)]
        public async Task<IActionResult> ObterFuncionariosQueAtingiramMetas(
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim)
        {
            try
            {
                if (dataInicio.HasValue && dataFim.HasValue && dataInicio.Value > dataFim.Value)
                    return CriarRespostaErro("A data de início deve ser anterior à data de fim.", 400);

                var resultado = await _servicoRanking.ObterFuncionariosQueAtingiramMetasAsync(dataInicio, dataFim);
                return ApartirDe(resultado);
            }
            catch (Exception ex)
            {
                return TratarExcecao(ex);
            }
        }

        [HttpGet("nao-atingiram-metas")]
        [ProducesResponseType(typeof(RespostaApi<IEnumerable<RankingDto>>), 200)]
        [ProducesResponseType(typeof(RespostaApi<object>), 400)]
        [ProducesResponseType(typeof(RespostaApi<object>), 500)]
        public async Task<IActionResult> ObterFuncionariosQueNaoAtingiramMetas(
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim)
        {
            try
            {
                if (dataInicio.HasValue && dataFim.HasValue && dataInicio.Value > dataFim.Value)
                    return CriarRespostaErro("A data de início deve ser anterior à data de fim.", 400);

                var resultado = await _servicoRanking.ObterFuncionariosQueNaoAtingiramMetasAsync(dataInicio, dataFim);
                return ApartirDe(resultado);
            }
            catch (Exception ex)
            {
                return TratarExcecao(ex);
            }
        }

        [HttpGet("metas-ativas")]
        [ProducesResponseType(typeof(RespostaApi<bool>), 200)]
        [ProducesResponseType(typeof(RespostaApi<object>), 500)]
        public async Task<IActionResult> VerificarMetasAtivas([FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFim)
        {
            if (!dataInicio.HasValue || !dataFim.HasValue)
                return CriarRespostaErro("Período inválido para verificação.", 400);

            var resultado = await _servicoRanking.ExistemMetasAtivasAsync(dataInicio.Value, dataFim.Value);

            if (resultado == null || resultado.Dados == null)
                return CriarRespostaErro("Não foi possível verificar as metas ativas.", 500);

            return ApartirDe(resultado);
        }
    }
}
