using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ranking.Aplicacao.DTOs
{
    public class EstatisticasVendasDto
    {
        public int TotalVendas { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorMedio { get; set; }
        public int QuantidadeVendas { get; set; }
        public int VendasHoje { get; set; }
        public int VendasUltimos7Dias { get; set; }
        public string MelhorFuncionarioNome { get; set; } = string.Empty;
        public decimal MelhorFuncionarioValor { get; set; }
    }

}
