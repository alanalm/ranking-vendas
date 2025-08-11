using Ranking.Aplicacao.DTOs;

namespace Ranking.Aplicacao.Validacoes
{
    public class ValidadorBase
    {
        public static class ValidacoesComuns
        {
            public static bool DataValida(DateTime data) =>
                data >= new DateTime(2000, 1, 1) && data <= DateTime.UtcNow.AddYears(10);

            public static bool DataValidaOuNula(DateTime? data) =>
                !data.HasValue || DataValida(data.Value);

            public static bool DataVendaValida(DateTime data) =>
                data >= new DateTime(2000, 1, 1) && data <= DateTime.UtcNow.AddDays(1);

            public static string RegexNome => @"^[a-zA-ZÀ-ÿ\s'\-]+$";

            public static bool TemPeriodoValido(FiltroRankingDto filtro)
            {
                if (!filtro.DataInicio.HasValue || !filtro.DataFim.HasValue)
                    return true;

                var diferenca = filtro.DataFim.Value - filtro.DataInicio.Value;
                return diferenca.TotalDays <= 365 * 5; // Máximo de 5 anos
            }
        }
    }
}
