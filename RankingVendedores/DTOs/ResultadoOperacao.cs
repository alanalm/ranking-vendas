namespace RankingVendedores.DTOs
{
    public class ResultadoOperacao<T>
    {

        public ResultadoOperacao() { }

        public bool Sucesso { get; set; }
        public T Dados { get; set; }
        public string Mensagem { get; set; }
        public List<string>? ErrosValidacao { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
