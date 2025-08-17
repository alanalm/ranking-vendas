using Microsoft.AspNetCore.Mvc;
using Ranking.Aplicacao.DTOs;
using Ranking.Aplicacao.Interfaces;
using Ranking.Aplicacao.Validacoes;
using Ranking.Infraestrutura.Persistencia;

namespace RankingVendedores.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Funcionários")]
    public class FuncionariosController : ControllerBase
    {
        private readonly IFuncionarioServico _servicoFuncionario;
        private readonly RankingVendasContexto _context;
        private readonly ILogger<FuncionariosController> _logger;


        public FuncionariosController(IFuncionarioServico servicoFuncionario, RankingVendasContexto context, ILogger<FuncionariosController> logger)
        {
            _servicoFuncionario = servicoFuncionario ?? throw new ArgumentNullException(nameof(servicoFuncionario));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger;
        }


        [HttpGet]
        [ProducesResponseType(typeof(RespostaApi<IEnumerable<FuncionarioDto>>), 200)]
        [ProducesResponseType(typeof(RespostaApi<object>), 500)]
        public async Task<IActionResult> ObterTodos()
        {
            try
            {
                var resultado = await _servicoFuncionario.ObterTodosAsync();
                if (!resultado.Sucesso)
                    return CriarRespostaErro(resultado.Mensagem, 500);

                var dados = resultado.Dados ?? new List<FuncionarioDto>();
                return CriarRespostaSucesso(dados, "Funcionários obtidos com sucesso.");
            }
            catch (Exception ex)
            {
                return TratarExcecao(ex);
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(RespostaApi<FuncionarioDto>), 200)]
        [ProducesResponseType(typeof(RespostaApi<object>), 404)]
        [ProducesResponseType(typeof(RespostaApi<object>), 500)]
        public async Task<IActionResult> ObterPorId(int id)
        {
            try
            {
                if (id <= 0)
                    return CriarRespostaErro("ID do funcionário deve ser maior que zero.", 400);

                var resultado = await _servicoFuncionario.ObterPorIdAsync(id);

                if (!resultado.Sucesso)
                    return CriarRespostaErro(resultado.Mensagem, 500);

                if (resultado.Dados == null)
                    return CriarRespostaNaoEncontrada($"Funcionário com ID {id} não encontrado.");

                return CriarRespostaSucesso(resultado.Dados, "Funcionário obtido com sucesso.");
            }
            catch (Exception ex)
            {
                return TratarExcecao(ex);
            }
        }

        [HttpGet("pesquisar")]
        [ProducesResponseType(typeof(RespostaApi<IEnumerable<FuncionarioDto>>), 200)]
        [ProducesResponseType(typeof(RespostaApi<object>), 500)]
        public async Task<IActionResult> PesquisarPorNome([FromQuery] string nome)
        {
            try
            {
                var resultado = await _servicoFuncionario.PesquisarPorNomeAsync(nome ?? string.Empty);
                if (!resultado.Sucesso)
                    return CriarRespostaErro(resultado.Mensagem, 500);

                return CriarRespostaSucesso(resultado.Dados, "Pesquisa realizada com sucesso.");
            }
            catch (Exception ex)
            {
                return TratarExcecao(ex);
            }
        }

        [HttpGet("com-estatisticas")]
        [ProducesResponseType(typeof(RespostaApi<IEnumerable<FuncionarioDto>>), 200)]
        [ProducesResponseType(typeof(RespostaApi<object>), 500)]
        public async Task<IActionResult> ObterComEstatisticas([FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFim)
        {
            try
            {
                if (dataInicio.HasValue && dataFim.HasValue && dataInicio.Value > dataFim.Value)
                    return CriarRespostaErro("A data de início deve ser anterior à data de fim.", 400);

                var resultado = await _servicoFuncionario.ObterComEstatisticasAsync(dataInicio, dataFim);
                if (!resultado.Sucesso)
                    return CriarRespostaErro(resultado.Mensagem, 500);

                return CriarRespostaSucesso(resultado.Dados, "Funcionários com estatísticas obtidos com sucesso.");
            }
            catch (Exception ex)
            {
                return TratarExcecao(ex);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(RespostaApi<FuncionarioDto>), 201)]
        [ProducesResponseType(typeof(RespostaApi<object>), 400)]
        [ProducesResponseType(typeof(RespostaApi<object>), 500)]
        public async Task<IActionResult> Criar([FromBody] CriarFuncionarioDto criarFuncionarioDto)
        {
            try
            {
                var validador = new ValidadorCriarFuncionario();
                var resultadoValidacao = await validador.ValidateAsync(criarFuncionarioDto);

                if (!resultadoValidacao.IsValid)
                {
                    var mensagens = resultadoValidacao.Errors.Select(e => e.ErrorMessage);
                    return BadRequest(new { Erros = mensagens });
                }

                var jaExiste = await _servicoFuncionario.JaExisteComNomeAsync(criarFuncionarioDto.Nome);
                if (!jaExiste.Sucesso)
                    return CriarRespostaErro(jaExiste.Mensagem, 500);

                if (jaExiste.Dados)
                    return CriarRespostaErro($"Já existe um funcionário com o nome '{criarFuncionarioDto.Nome}'.", 400);

                var resultado = await _servicoFuncionario.CriarAsync(criarFuncionarioDto);
                if (!resultado.Sucesso)
                    return CriarRespostaErro(resultado.Mensagem, 400);

                return CreatedAtAction(
                    nameof(ObterPorId),
                    new { id = resultado.Dados?.Id ?? 0 },
                    CriarRespostaSucesso(resultado.Dados, "Funcionário criado com sucesso."));
            }
            catch (Exception ex)
            {
                return TratarExcecao(ex);
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(RespostaApi<FuncionarioDto>), 200)]
        [ProducesResponseType(typeof(RespostaApi<object>), 400)]
        [ProducesResponseType(typeof(RespostaApi<object>), 404)]
        [ProducesResponseType(typeof(RespostaApi<object>), 500)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarFuncionarioDto atualizarFuncionarioDto)
        {
            try
            {
                if (id <= 0)
                    return CriarRespostaErro("ID do funcionário deve ser maior que zero.", 400);

                var erroValidacao = ValidarModelo();
                if (erroValidacao != null)
                    return erroValidacao;

                if (id != atualizarFuncionarioDto.Id)
                    return CriarRespostaErro("ID da URL não confere com o ID do corpo da requisição.", 400);

                var funcionarioExistente = await _servicoFuncionario.ObterPorIdAsync(id);
                if (!funcionarioExistente.Sucesso)
                    return CriarRespostaErro(funcionarioExistente.Mensagem, 500);

                if (funcionarioExistente.Dados == null)
                    return CriarRespostaNaoEncontrada($"Funcionário com ID {id} não encontrado.");

                var jaExiste = await _servicoFuncionario.JaExisteComNomeAsync(atualizarFuncionarioDto.Nome, id);
                if (!jaExiste.Sucesso)
                    return CriarRespostaErro(jaExiste.Mensagem, 500);

                if (jaExiste.Dados)
                    return CriarRespostaErro($"Já existe outro funcionário com o nome '{atualizarFuncionarioDto.Nome}'.", 400);

                var resultado = await _servicoFuncionario.AtualizarAsync(atualizarFuncionarioDto);
                if (!resultado.Sucesso)
                    return CriarRespostaErro(resultado.Mensagem, 400);

                return CriarRespostaSucesso(resultado.Dados, "Funcionário atualizado com sucesso.");
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
                    return CriarRespostaErro("ID do funcionário deve ser maior que zero.", 400);

                var resultado = await _servicoFuncionario.RemoverAsync(id);
                if (!resultado.Sucesso)
                    return CriarRespostaErro(resultado.Mensagem, 400);

                if (!resultado.Dados)
                    return CriarRespostaNaoEncontrada($"Funcionário com ID {id} não encontrado.");

                return CriarRespostaSucesso("Funcionário removido com sucesso.");
            }
            catch (Exception ex)
            {
                return TratarExcecao(ex);
            }
        }

        [HttpGet("{id:int}/pode-remover")]
        [ProducesResponseType(typeof(RespostaApi<bool>), 200)]
        [ProducesResponseType(typeof(RespostaApi<object>), 400)]
        [ProducesResponseType(typeof(RespostaApi<object>), 500)]
        public async Task<IActionResult> PodeSerRemovido(int id)
        {
            try
            {
                var resultado = await _servicoFuncionario.PodeSerRemovidoAsync(id);

                if (!resultado.Sucesso)
                    return BadRequest(resultado.Mensagem);

                Console.WriteLine($"DEBUG - Pode remover funcionário {id}? {resultado.Dados}");

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar se funcionário pode ser removido");
                return StatusCode(500, "Erro interno ao verificar remoção");
            }
        }
    }
}
