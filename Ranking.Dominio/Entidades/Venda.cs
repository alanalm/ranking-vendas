namespace Ranking.Dominio.Entidades
{
    public class Venda : EntidadeBase
    {
        private decimal _valor;
        private DateTime _dataVenda;
        private string _descricao = string.Empty;

        public int FuncionarioId { get; private set; }

        public decimal Valor
        {
            get => _valor;
            private set
            {
                if (value <= 0)
                    throw new ArgumentException("O valor da venda deve ser maior que zero.", nameof(value));

                if (_valor != value)
                {
                    _valor = value;
                    AtualizarDataModificacao();
                }
            }
        }

        public DateTime DataVenda
        {
            get => _dataVenda;
            private set
            {
                if (value > DateTime.UtcNow)
                    throw new ArgumentException("A data da venda não pode ser no futuro.", nameof(value));

                _dataVenda = value;
                AtualizarDataModificacao();
            }
        }

        public string Descricao
        {
            get => _descricao;
            private set
            {
                if (value != null && value.Length > 500)
                    throw new ArgumentException("A descrição da venda não pode ter mais de 500 caracteres.", nameof(value));

                _descricao = value?.Trim() ?? string.Empty;
                AtualizarDataModificacao();
            }
        }

        /// Funcionário que realizou a venda.
        /// Propriedade de navegação para o Entity Framework.
        public virtual Funcionario? Funcionario { get; private set; }

        /// Construtor privado para o Entity Framework.
        private Venda() : base() { }

        /// Construtor público para criação de uma nova venda.
        public Venda(int funcionarioId, decimal valor, DateTime dataVenda, string? descricao = null) : base()
        {
            if (funcionarioId <= 0)
                throw new ArgumentException("O identificador do funcionário deve ser maior que zero.", nameof(funcionarioId));

            FuncionarioId = funcionarioId;
            Valor = valor;
            DataVenda = dataVenda;
            Descricao = descricao ?? string.Empty;
        }

        /// Sobrescrita do método de validação da entidade base.
        public override bool EhValida()
        {
            return base.EhValida() &&
                   FuncionarioId > 0 &&
                   Valor > 0 &&
                   DataVenda <= DateTime.UtcNow;
        }
    }
}
