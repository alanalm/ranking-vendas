using Ranking.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ranking.Aplicacao.DTOs
{
    public class ConfiguracoesDto
    {
        // Gerais
        public PeriodoPadrao PeriodoPadrao { get; set; } = PeriodoPadrao.Mensal;
        public int MetaMinima { get; set; } = 80;
        public bool SomenteAtivos { get; set; } = true;

        // Aparência
        public TemaApp Tema { get; set; } = TemaApp.Claro;
        public string CorDestaque { get; set; } = "#4CAF50"; // verde
        public string CorAlerta { get; set; } = "#F44336";   // vermelho

        // Ranking
        public int TopFuncionarios { get; set; } = 10;
    }

}
