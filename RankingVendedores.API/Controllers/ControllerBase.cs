using Aplicacao.Utils;
using Microsoft.AspNetCore.Mvc;

namespace RankingVendedores.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class ControllerBase : Microsoft.AspNetCore.Mvc.Controller
    {
        protected IActionResult CriarRespostaSucesso<T>(T dados, string? mensagem = null)
        {
            var resposta = new RespostaApi<T>
            {
                Dados = dados,
                Mensagem = mensagem,
                Sucesso = true
            };
            return Ok(resposta);
        }

        protected IActionResult CriarRespostaSucesso(string mensagem)
        {
            var resposta = new RespostaApi<object>
            {
                Sucesso = true,
                Dados = null,
                Mensagem = mensagem
            };

            return Ok(resposta);
        }

        protected IActionResult CriarRespostaErro(string mensagem, int statusCode = 400)
        {
            var resposta = new RespostaApi<object>
            {
                Sucesso = false,
                Dados = null,
                Mensagem = mensagem
            };

            return StatusCode(statusCode, resposta);
        }

        protected IActionResult CriarRespostaErroValidacao(string mensagem, Dictionary<string, string[]> erroValidacao)
        {
            var resposta = new RespostaApi<object>
            {
                Sucesso = false,
                Dados = null,
                Mensagem = mensagem,
                ErrosValidacao = erroValidacao
            };

            return BadRequest(resposta);
        }

        protected IActionResult CriarRespostaNaoEncontrada(string? mensagem = null)
        {
            var resposta = new RespostaApi<object>
            {
                Sucesso = false,
                Dados = null,
                Mensagem = mensagem ?? "Recurso não encontrado."
            };
            return NotFound(resposta);
        }

        protected IActionResult TratarExcecao(Exception ex)
        {
            Console.WriteLine($"Erro na API: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");

            return ex switch
            {
                ArgumentException or InvalidOperationException => CriarRespostaErro(ex.Message, 400),
                UnauthorizedAccessException => CriarRespostaErro("Acesso não autorizado.", 401),
                _ => CriarRespostaErro("Erro interno do servidor. Tente novamente mais tarde.", 500)
            };
        }

        protected IActionResult? ValidarModelo()
        {
            if (!ModelState.IsValid)
            {
                var erros = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                    );

                return CriarRespostaErroValidacao("Dados inválidos fornecidos.", erros);
            }

            return null;
        }

        // NOVO: Lida com ResultadoOperacao sem dados
        protected IActionResult ApartirDe(ResultadoOperacao resultado)
        {
            if (resultado.Sucesso)
                return CriarRespostaSucesso(resultado.Mensagem ?? "Operação realizada com sucesso.");
            return CriarRespostaErro(resultado.Mensagem);
        }

        protected IActionResult ApartirDe<T>(ResultadoOperacao<T> resultado)
        {
            if (resultado.Sucesso)
                return CriarRespostaSucesso(resultado.Dados!, resultado.Mensagem);
            return CriarRespostaErro(resultado.Mensagem);
        }

        protected IActionResult TratarResultado<T>(ResultadoOperacao<T> resultado, string mensagemSucesso, int erroStatusCode = 400)
        {
            if (resultado.Sucesso)
                return CriarRespostaSucesso(resultado.Dados, mensagemSucesso);
            else
                return CriarRespostaErro(resultado.Mensagem ?? "Erro na operação", erroStatusCode);
        }

        protected IActionResult TratarResultado(ResultadoOperacao resultado, string mensagemSucesso, int erroStatusCode = 400)
        {
            if (resultado.Sucesso)
                return CriarRespostaSucesso(mensagemSucesso);
            else
                return CriarRespostaErro(resultado.Mensagem ?? "Erro na operação", erroStatusCode);
        }

        public class RespostaApi<T>
        {
            public bool Sucesso { get; set; }
            public T? Dados { get; set; }
            public string Mensagem { get; set; } = string.Empty;
            public Dictionary<string, string[]>? ErrosValidacao { get; set; }
            public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        }
    }
}