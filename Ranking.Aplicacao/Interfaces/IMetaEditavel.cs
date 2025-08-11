namespace Ranking.Aplicacao.Interfaces
{
    public interface IMetaEditavel
    {
        int IndicadorId { get; set; }
        decimal Valor { get; set; }
        DateTime Data { get; set; }
    }
}
