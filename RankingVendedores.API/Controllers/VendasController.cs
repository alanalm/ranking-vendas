using Aplicacao.Utils;
using Microsoft.AspNetCore.Mvc;
using Ranking.Aplicacao.DTOs;
using Ranking.Aplicacao.Interfaces;

namespace RankingVendedores.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Vendas")]
    public class VendasController : ControllerBase
    {
        private readonly IVendaServico _servicoVenda;

        public VendasController(IVendaServico servicoVenda)
        {
            _servicoVenda = servicoVenda ?? throw new ArgumentNullException(nameof(servicoVenda));
        }

        [HttpGet]
        [ProducesResponseType(typeof(RespostaApi<IEnumerable<VendaDto>>), 200)]
        public async Task<IActionResult> ObterTodos()
        {
            var resultado = await _servicoVenda.ObterTodosAsync();
            return CriarRespostaSucesso(resultado);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            if (id <= 0)
                return CriarRespostaErro("ID da venda deve ser maior que zero.", 400);

            var resultado = await _servicoVenda.ObterPorIdAsync(id);
            return TratarResultado(resultado, "Venda obtida com sucesso.", 404);
        }

        [HttpGet("por-funcionario/{funcionarioId:int}")]
        public async Task<IActionResult> ObterPorFuncionario(int funcionarioId)
        {
            if (funcionarioId <= 0)
                return CriarRespostaErro("ID do funcionário deve ser maior que zero.", 400);

            var resultado = await _servicoVenda.ObterPorFuncionarioAsync(funcionarioId);
            return TratarResultado(resultado, "Vendas do funcionário obtidas com sucesso.");
        }

        [HttpGet("periodo")]
        public async Task<IActionResult> ObterPorPeriodo([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
        {
            var vendas = await _servicoVenda.ObterPorPeriodoAsync(dataInicio, dataFim);
            return Ok(vendas); 
        }


        [HttpGet("por-funcionario-e-periodo/{funcionarioId:int}")]
        public async Task<IActionResult> ObterPorFuncionarioEPeriodo(int funcionarioId, DateTime dataInicio, DateTime dataFim)
        {
            if (funcionarioId <= 0 || dataInicio > dataFim)
                return CriarRespostaErro("Dados inválidos para consulta.", 400);

            var resultado = await _servicoVenda.ObterPorFuncionarioEPeriodoAsync(funcionarioId, dataInicio, dataFim);
            return TratarResultado(resultado, "Vendas do funcionário no período obtidas com sucesso.");
        }

        [HttpGet("por-mes")]
        public async Task<IActionResult> ObterPorMes(int mes, int ano)
        {
            if (mes < 1 || mes > 12 || ano < 1900 || ano > DateTime.UtcNow.Year + 10)
                return CriarRespostaErro("Mês ou ano inválido.", 400);

            var resultado = await _servicoVenda.ObterPorMesAsync(mes, ano);
            return TratarResultado(resultado, $"Vendas de {mes:D2}/{ano} obtidas com sucesso.");
        }

        [HttpGet("por-ano")]
        public async Task<IActionResult> ObterPorAno(int ano)
        {
            if (ano < 1900 || ano > DateTime.UtcNow.Year + 10)
                return CriarRespostaErro("Ano inválido.", 400);

            var resultado = await _servicoVenda.ObterPorAnoAsync(ano);
            return TratarResultado(resultado, $"Vendas de {ano} obtidas com sucesso.");
        }

        [HttpGet("maiores")]
        public async Task<IActionResult> ObterMaioresVendas(int quantidade = 10, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            if (quantidade <= 0 || (dataInicio > dataFim))
                return CriarRespostaErro("Parâmetros inválidos.", 400);

            var resultado = await _servicoVenda.ObterMaioresVendasAsync(quantidade, dataInicio, dataFim);
            return TratarResultado(resultado, $"Top {quantidade} maiores vendas obtidas com sucesso.");
        }

        [HttpGet("recentes")]
        public async Task<IActionResult> ObterVendasRecentes(int? quantidade = null)
        {
            if (quantidade.HasValue && quantidade.Value <= 0)
                return CriarRespostaErro("A quantidade deve ser maior que zero.", 400);

            var resultado = await _servicoVenda.ObterVendasRecentesAsync(quantidade);
            return TratarResultado(resultado, "Vendas recentes obtidas com sucesso.");
        }

        [HttpGet("estatisticas/funcionario")]
        public async Task<IActionResult> CalcularEstatisticasPorFuncionario(DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            if (dataInicio > dataFim)
                return CriarRespostaErro("A data de início deve ser anterior à data de fim.", 400);

            var resultado = await _servicoVenda.CalcularEstatisticasPorFuncionarioAsync(dataInicio, dataFim);
            return TratarResultado(resultado, "Estatísticas calculadas com sucesso.");
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarVendaDto dto)
        {
            var validacao = ValidarModelo();
            if (validacao != null) return validacao;

            var resultado = await _servicoVenda.CriarAsync(dto);
            if (!resultado.Sucesso)
                return CriarRespostaErro(resultado.Mensagem);

            return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Dados?.Id }, CriarRespostaSucesso(resultado.Dados, "Venda criada com sucesso."));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarVendaDto dto)
        {
            if (id <= 0 || id != dto.Id)
                return CriarRespostaErro("ID inválido.", 400);

            var validacao = ValidarModelo();
            if (validacao != null) return validacao;

            var resultado = await _servicoVenda.AtualizarAsync(dto);
            return TratarResultado(resultado, "Venda atualizada com sucesso.", 404);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Remover(int id)
        {
            if (id <= 0)
                return CriarRespostaErro("ID inválido.", 400);

            var resultado = await _servicoVenda.RemoverAsync(id);
            return TratarResultado(resultado, "Venda removida com sucesso.", 404);
        }

        [HttpGet("estatisticas")]
        public async Task<ActionResult<EstatisticasVendasDto>> ObterEstatisticas([FromQuery] DateTime inicio, [FromQuery] DateTime? fim)
        {
            var estatisticas = await _servicoVenda.CalcularEstatisticasAsync(inicio, fim);
            return Ok(estatisticas);
        }

        [HttpGet("pesquisar")]
        public async Task<ActionResult<IEnumerable<VendaDto>>> PesquisarVendas([FromQuery] string? termo,[FromQuery] DateTime? inicio,
        [FromQuery] DateTime? fim)
        {
            var resultado = await _servicoVenda.PesquisarAsync(termo, inicio, fim);
            return Ok(ResultadoOperacao<List<VendaDto>>.CriarSucesso(resultado.ToList()));

        }

        [HttpGet("estatisticas-detalhadas")]
        public async Task<IActionResult> ObterEstatisticasVendas(DateTime? dataInicio, DateTime? dataFim)
        {
            var estatisticas = await _servicoVenda.ObterEstatisticasVendasAsync(dataInicio, dataFim);
            return Ok(estatisticas); 
        }
    }
}
