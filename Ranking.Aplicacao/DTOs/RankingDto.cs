using Ranking.Dominio.Entidades;

namespace Ranking.Aplicacao.DTOs;

public class RankingDto
 {
   public int Posicao { get; set; }
   public FuncionarioDto Funcionario { get; set; }

   public DesempenhoDto Desempenho { get; set; }
   public VendasDto Vendas { get; set; }
   public MetasDto Metas { get; set; }

   public DateTime? DataInicio { get; set; }
   public DateTime? DataFim { get; set; }

}
public class DesempenhoDto
{
    private Funcionario funcionario;
    private List<Venda> vendas;
    private List<Meta> metas;
    private DateTime? dataInicio;
    private DateTime? dataFim;

    public DesempenhoDto() { }

    public DesempenhoDto(Funcionario funcionario, List<Venda> vendas, List<Meta> metas, DateTime? dataInicio, DateTime? dataFim)
    {
        this.funcionario = funcionario;
        this.vendas = vendas;
        this.metas = metas;
        this.dataInicio = dataInicio;
        this.dataFim = dataFim;
    }

    public decimal Geral { get; set; }
   public decimal QuantidadeVendas { get; set; }
   public decimal ValorVendas { get; set; }
    public decimal MetaQuantidade { get; set; }
    public decimal MetaValor { get; set; }

    public decimal PercentualQuantidade { get; set; } 
    public decimal PercentualValor { get; set; }

    public decimal PercentualAtingimentoMetas { get; set; }
   public bool AtingiuTodasAsMetas { get; set; }
}

public class VendasDto
{
 public int Quantidade { get; set; }
 public decimal ValorTotal { get; set; }
}

public class MetasDto
{
 public decimal QuantidadeVendas { get; set; }
 public decimal ValorVendas { get; set; }
 public bool Ativa { get; set; }
}
