namespace Ranking.Aplicacao.Interfaces
{
    public interface IVendaEditavel
    {
        int FuncionarioId { get; set; }
        decimal Valor { get; set; }
        DateTime? DataVenda { get; set; }
        string Descricao { get; set; }
    }
}
