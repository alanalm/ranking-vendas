namespace Aplicacao.Utils
{
    public class ResultadoOperacao
    {
        public bool Sucesso { get; private set; }
        public string? Mensagem { get; private set; }
        public List<string> Erros { get; private set; } = new();

        public ResultadoOperacao() { }

        protected ResultadoOperacao(bool sucesso, string? mensagem = null, List<string>? erros = null)
        {
            Sucesso = sucesso;
            Mensagem = mensagem;
            if (erros is not null)
                Erros = erros;
        }

        public static ResultadoOperacao CriarSucesso(string? mensagem = null)
        {
            return new ResultadoOperacao(true, mensagem);
        }

        public static ResultadoOperacao CriarFalha(string mensagem)
        {
            return new ResultadoOperacao(false, mensagem, new List<string> { mensagem });
        }

        public static ResultadoOperacao CriarFalha(List<string> erros, string? mensagem = null)
        {
            return new ResultadoOperacao(false, mensagem, erros);
        }
    }

    public class ResultadoOperacao<T> : ResultadoOperacao
    {
        public T? Dados { get; private set; }

        public ResultadoOperacao() { }

        private ResultadoOperacao(bool sucesso, T? dados = default, string? mensagem = null, List<string>? erros = null)
            : base(sucesso, mensagem, erros)
        {
            Dados = dados;
        }

        public static ResultadoOperacao<T> CriarSucesso(T dados, string? mensagem = null)
        {
            return new ResultadoOperacao<T>(true, dados, mensagem);
        }

        public static new ResultadoOperacao<T> CriarFalha(string mensagem)
        {
            return new ResultadoOperacao<T>(false, default, mensagem, new List<string> { mensagem });
        }

        public static new ResultadoOperacao<T> CriarFalha(List<string> erros, string? mensagem = null)
        {
            return new ResultadoOperacao<T>(false, default, mensagem, erros);
        }
    }
}
