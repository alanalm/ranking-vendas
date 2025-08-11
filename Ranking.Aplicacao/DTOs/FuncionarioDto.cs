using System.ComponentModel.DataAnnotations;

namespace Ranking.Aplicacao.DTOs;

public class FuncionarioDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public decimal TotalVendas { get; set; }
    public int QuantidadeVendas { get; set; }
}

public class CriarFuncionarioDto
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    public string Nome { get; set; } = string.Empty;
}

public class AtualizarFuncionarioDto
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório")]
    public string Nome { get; set; } = string.Empty;
}