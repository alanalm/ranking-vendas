using Aplicacao.Utils;
using Microsoft.AspNetCore.Mvc;
using Ranking.Aplicacao.DTOs;
using Ranking.Aplicacao.Interfaces;
using ControllerBase = RankingVendedores.API.Controllers.ControllerBase;

namespace RankingVendas.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Indicadores")]
public class IndicadoresController : ControllerBase
{
    private readonly IIndicadorServico _servicoIndicador;

    public IndicadoresController(IIndicadorServico servicoIndicador)
    {
        _servicoIndicador = servicoIndicador ?? throw new ArgumentNullException(nameof(servicoIndicador));
    }

    [HttpGet]
    [ProducesResponseType(typeof(RespostaApi<IEnumerable<IndicadorDto>>), 200)]
    public async Task<IActionResult> ObterTodos()
    {
        try
        {
            var resultado = await _servicoIndicador.ObterTodosAsync();
            return ApartirDe(resultado);
        }
        catch (Exception ex)
        {
            return TratarExcecao(ex);
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RespostaApi<IndicadorDto>), 200)]
    [ProducesResponseType(typeof(RespostaApi<object>), 404)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        try
        {
            if (id <= 0)
                return CriarRespostaErro("ID do indicador deve ser maior que zero.", 400);

            var resultado = await _servicoIndicador.ObterPorIdAsync(id);
            return ApartirDe(resultado);
        }
        catch (Exception ex)
        {
            return TratarExcecao(ex);
        }
    }

    [HttpGet("pesquisar")]
    [ProducesResponseType(typeof(RespostaApi<IEnumerable<IndicadorDto>>), 200)]
    public async Task<IActionResult> PesquisarPorNome([FromQuery] string nome)
    {
        try
        {
            var resultado = await _servicoIndicador.PesquisarPorNomeAsync(nome ?? string.Empty);
            return ApartirDe(resultado);
        }
        catch (Exception ex)
        {
            return TratarExcecao(ex);
        }
    }

    [HttpGet("com-metas-ativas")]
    [ProducesResponseType(typeof(RespostaApi<IEnumerable<IndicadorDto>>), 200)]
    public async Task<IActionResult> ObterComMetasAtivas()
    {
        try
        {
            var resultado = await _servicoIndicador.ObterComMetasAtivasAsync();
            return ApartirDe(resultado);
        }
        catch (Exception ex)
        {
            return TratarExcecao(ex);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(RespostaApi<IndicadorDto>), 201)]
    [ProducesResponseType(typeof(RespostaApi<object>), 400)]
    public async Task<IActionResult> Criar([FromBody] CriarIndicadorDto dto)
    {
        var erroValidacao = ValidarModelo();
        if (erroValidacao != null)
            return erroValidacao;

        var resultado = await _servicoIndicador.CriarAsync(dto);
        return ApartirDe(resultado);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(RespostaApi<IndicadorDto>), 200)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarIndicadorDto dto)
    {
        try
        {
            if (id <= 0)
                return CriarRespostaErro("ID inválido.", 400);

            if (id != dto.Id)
                return CriarRespostaErro("ID da URL não confere com o corpo da requisição.", 400);

            var erroValidacao = ValidarModelo();
            if (erroValidacao != null)
                return erroValidacao;

            var resultado = await _servicoIndicador.AtualizarAsync(dto);
            return ApartirDe(resultado);
        }
        catch (Exception ex)
        {
            return TratarExcecao(ex);
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(RespostaApi<object>), 200)]
    public async Task<IActionResult> Remover(int id)
    {
        try
        {
            if (id <= 0)
                return CriarRespostaErro("ID inválido.", 400);

            var resultado = await _servicoIndicador.RemoverAsync(id);
            return Ok(ResultadoOperacao.CriarSucesso("Indicador removido com sucesso."));
        }
        catch (Exception ex)
        {
            return TratarExcecao(ex);
        }
    }

    [HttpGet("{id:int}/pode-remover")]
    [ProducesResponseType(typeof(RespostaApi<bool>), 200)]
    public async Task<IActionResult> PodeSerRemovido(int id)
    {
        try
        {
            if (id <= 0)
                return CriarRespostaErro("ID inválido.", 400);

            var resultado = await _servicoIndicador.PodeSerRemovidoAsync(id);
            return ApartirDe(resultado);
        }
        catch (Exception ex)
        {
            return TratarExcecao(ex);
        }
    }
}