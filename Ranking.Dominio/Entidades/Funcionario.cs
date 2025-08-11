using Ranking.Dominio.Interfaces;

namespace Ranking.Dominio.Entidades
{
    public class Funcionario : EntidadeBase
    {
        private string _nome = string.Empty;

        public string Nome
        {
            get => _nome;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("O nome do funcionário não pode ser vazio ou nulo.", nameof(value));

                if (value.Length > 100)
                    throw new ArgumentException("O nome do funcionário não pode ter mais de 100 caracteres.", nameof(value));
                _nome = value.Trim();
            }
        }

        public virtual ICollection<Venda> Vendas { get; private set; } = new List<Venda>();

        public Funcionario() : base() { }

        public Funcionario(string nome) : base()
        {
            Nome = nome;
            AtualizarDataModificacao();
        }

        public override bool EhValida()
        {
            return base.EhValida() && !string.IsNullOrWhiteSpace(Nome);
        }
    }
}
