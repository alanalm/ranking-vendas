using Microsoft.AspNetCore.Mvc;
using Ranking.Aplicacao.DTOs;
using Ranking.Aplicacao.Interfaces;

namespace RankingVendedores.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Metas")]
public class MetasController : ControllerBase
{
    private readonly IMetaServico _servicoMeta;

    public MetasController(IMetaServico servicoMeta)
    {
        _servicoMeta = servicoMeta ?? throw new ArgumentNullException(nameof(servicoMeta));
    }

    [HttpGet]
    [ProducesResponseType(typeof(RespostaApi<IEnumerable<MetaDto>>), 200)]
    [ProducesResponseType(typeof(RespostaApi<object>), 500)]
    [HttpGet]
    public async Task<IActionResult> ObterTodos()
    {
        try
        {
            var resultado = await _servicoMeta.ObterTodosAsync();
            return ApartirDe(resultado);
        }
        catch (Exception ex)
        {
            return TratarExcecao(ex);
        }
    }


    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RespostaApi<MetaDto>), 200)]
    [ProducesResponseType(typeof(RespostaApi<object>), 404)]
    [ProducesResponseType(typeof(RespostaApi<object>), 500)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        try
        {
            if (id <= 0)
                return CriarRespostaErro("ID da meta deve ser maior que zero.", 400);

            var meta = await _servicoMeta.ObterPorIdAsync(id);

            if (meta == null)
                return CriarRespostaNaoEncontrada($"Meta com ID {id} não encontrada.");

            return CriarRespostaSucesso(meta, "Meta obtida com sucesso.");
        }
        catch (Exception ex)
        {
            return TratarExcecao(ex);
        }
    }

    [HttpGet("por-indicador/{indicadorId:int}")]
    [ProducesResponseType(typeof(RespostaApi<IEnumerable<MetaDto>>), 200)]
    [ProducesResponseType(typeof(RespostaApi<object>), 400)]
    [ProducesResponseType(typeof(RespostaApi<object>), 500)]
    public async Task<IActionResult> ObterPorIndicador(int indicadorId)
    {
        try
        {
            if (indicadorId <= 0)
                return CriarRespostaErro("ID do indicador deve ser maior que zero.", 400);

            var metas = await _servicoMeta.ObterPorIndicadorAsync(indicadorId);
            return CriarRespostaSucesso(metas, "Metas do indicador obtidas com sucesso.");
        }
        catch (Exception ex)
        {
            return TratarExcecao(ex);
        }
    }

    [HttpGet("ativas")]
    [ProducesResponseType(typeof(RespostaApi<IEnumerable<MetaDto>>), 200)]
    [ProducesResponseType(typeof(RespostaApi<object>), 500)]
    public async Task<IActionResult> ObterMetasAtivas([FromQuery] DateTime? data)
    {
        try
        {
            var metas = await _servicoMeta.ObterMetasAtivasAsync(data);
            return CriarRespostaSucesso(metas, "Metas ativas obtidas com sucesso.");
        }
        catch (Exception ex)
        {
            return TratarExcecao(ex);
        }
    }

    [HttpGet("ativa-atual/{indicadorId:int}")]
    [ProducesResponseType(typeof(RespostaApi<MetaDto>), 200)]
    [ProducesResponseType(typeof(RespostaApi<object>), 400)]
    [ProducesResponseType(typeof(RespostaApi<object>), 404)]
    [ProducesResponseType(typeof(RespostaApi<object>), 500)]
    public async Task<IActionResult> ObterMetaAtivaAtualPorIndicador(int indicadorId, [FromQuery] DateTime? data)
    {
        try
        {
            if (indicadorId <= 0)
                return CriarRespostaErro("ID do indicador deve ser maior que zero.", 400);

            var meta = await _servicoMeta.ObterMetaAtivaAtualPorIndicadorAsync(indicadorId, data);

            if (meta == null)
                return CriarRespostaNaoEncontrada($"Não existe meta ativa para o indicador com ID {indicadorId}.");

            return CriarRespostaSucesso(meta, "Meta ativa atual obtida com sucesso.");
        }
        catch (Exception ex)
        {
            return TratarExcecao(ex);
        }
    }

    [HttpGet("vigentes")]
    [ProducesResponseType(typeof(RespostaApi<IEnumerable<MetaDto>>), 200)]
    [ProducesResponseType(typeof(RespostaApi<object>), 400)]
    [ProducesResponseType(typeof(RespostaApi<object>), 500)]
    public async Task<IActionResult> ObterPorPeriodoAsync([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
    {
        try
        {
            if (dataInicio > dataFim)
                return CriarRespostaErro("A data de início deve ser anterior à data de fim.", 400);

            var metas = await _servicoMeta.ObterPorPeriodoAsync(dataInicio, dataFim);
            return CriarRespostaSucesso(metas, $"Metas vigentes no período de {dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy} obtidas com sucesso.");
        }
        catch (Exception ex)
        {
            return TratarExcecao(ex);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(RespostaApi<MetaDto>), 201)]
    [ProducesResponseType(typeof(RespostaApi<object>), 400)]
    [ProducesResponseType(typeof(RespostaApi<object>), 500)]
    public async Task<IActionResult> Criar([FromBody] CriarMetaDto criarMetaDto)
    {
        try
        {
            var erroValidacao = ValidarModelo();
            if (erroValidacao != null)
                return erroValidacao;

            var resultado = await _servicoMeta.CriarAsync(criarMetaDto);

            if (!resultado.Sucesso)
                return CriarRespostaErro(resultado.Mensagem, 400);

            return CreatedAtAction(
                nameof(ObterPorId),
                new { id = resultado.Dados.Id },
                CriarRespostaSucesso(resultado.Dados, "Meta criada com sucesso."));
        }
        catch (Exception ex)
        {
            return TratarExcecao(ex);
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(RespostaApi<MetaDto>), 200)]
    [ProducesResponseType(typeof(RespostaApi<object>), 400)]
    [ProducesResponseType(typeof(RespostaApi<object>), 404)]
    [ProducesResponseType(typeof(RespostaApi<object>), 500)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarMetaDto atualizarMetaDto)
    {
        try
        {
            if (id <= 0)
                return CriarRespostaErro("ID da meta deve ser maior que zero.", 400);

            var erroValidacao = ValidarModelo();
            if (erroValidacao != null)
                return erroValidacao;

            if (id != atualizarMetaDto.Id)
                return CriarRespostaErro("ID da URL não confere com o ID do corpo da requisição.", 400);

            var resultado = await _servicoMeta.AtualizarAsync(atualizarMetaDto);

            if (!resultado.Sucesso)
                return CriarRespostaErro(resultado.Mensagem, 400);

            return CriarRespostaSucesso(resultado.Dados, "Meta atualizada com sucesso.");
        }
        catch (Exception ex)
        {
            return TratarExcecao(ex);
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(RespostaApi<object>), 200)]
    [ProducesResponseType(typeof(RespostaApi<object>), 400)]
    [ProducesResponseType(typeof(RespostaApi<object>), 404)]
    [ProducesResponseType(typeof(RespostaApi<object>), 500)]
    public async Task<IActionResult> Remover(int id)
    {
        try
        {
            if (id <= 0)
                return CriarRespostaErro("ID da meta deve ser maior que zero.", 400);

            var resultado = await _servicoMeta.RemoverAsync(id);

            if (!resultado.Sucesso)
                return CriarRespostaNaoEncontrada(resultado.Mensagem ?? "Meta não encontrada.");

            return CriarRespostaSucesso("Meta removida com sucesso.");
        }
        catch (Exception ex)
        {
            return TratarExcecao(ex);
        }
    }

    [HttpPatch("{id:int}/ativar")]
    [ProducesResponseType(typeof(RespostaApi<object>), 200)]
    [ProducesResponseType(typeof(RespostaApi<object>), 400)]
    [ProducesResponseType(typeof(RespostaApi<object>), 404)]
    [ProducesResponseType(typeof(RespostaApi<object>), 500)]
    public async Task<IActionResult> Ativar(int id)
    {
        try
        {
            if (id <= 0)
                return CriarRespostaErro("ID da meta deve ser maior que zero.", 400);

            var resultado = await _servicoMeta.AtivarAsync(id);

            if (!resultado.Sucesso)
                return CriarRespostaNaoEncontrada(resultado.Mensagem ?? "Meta não encontrada.");

            return CriarRespostaSucesso("Meta ativada com sucesso.");
        }
        catch (Exception ex)
        {
            return TratarExcecao(ex);
        }
    }

    [HttpPatch("{id:int}/desativar")]
    [ProducesResponseType(typeof(RespostaApi<object>), 200)]
    [ProducesResponseType(typeof(RespostaApi<object>), 400)]
    [ProducesResponseType(typeof(RespostaApi<object>), 404)]
    [ProducesResponseType(typeof(RespostaApi<object>), 500)]
    public async Task<IActionResult> Desativar(int id)
    {
        try
        {
            if (id <= 0)
                return CriarRespostaErro("ID da meta deve ser maior que zero.", 400);

            var resultado = await _servicoMeta.DesativarAsync(id);

            if (!resultado.Sucesso)
                return CriarRespostaNaoEncontrada(resultado.Mensagem ?? "Meta não encontrada.");

            return CriarRespostaSucesso("Meta desativada com sucesso.");
        }
        catch (Exception ex)
        {
            return TratarExcecao(ex);
        }
    }

    [HttpPatch("desativar-por-indicador/{indicadorId:int}")]
    [ProducesResponseType(typeof(RespostaApi<int>), 200)]
    [ProducesResponseType(typeof(RespostaApi<object>), 400)]
    [ProducesResponseType(typeof(RespostaApi<object>), 500)]
    public async Task<IActionResult> DesativarMetasDoIndicador(int indicadorId)
    {
        try
        {
            if (indicadorId <= 0)
                return CriarRespostaErro("ID do indicador deve ser maior que zero.", 400);

            var resultado = await _servicoMeta.DesativarMetasDoIndicadorAsync(indicadorId);

            if (!resultado.Sucesso)
                return CriarRespostaErro(resultado.Mensagem, 400);

            return CriarRespostaSucesso(resultado.Dados, $"{resultado.Dados} meta(s) desativada(s) com sucesso.");
        }
        catch (Exception ex)
        {
            return TratarExcecao(ex);
        }
    }
}
