using Ranking.Dominio.Enums;

namespace Ranking.Dominio.Entidades
{
    public class Indicador : EntidadeBase
    {
        private string _nome = string.Empty;
        private string _descricao = string.Empty;
        private TipoIndicador _tipo { get; set; }

        public string Nome
        {
            get => _nome;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("O nome do indicador não pode ser vazio ou nulo.", nameof(value));

                if (value.Length > 100)
                    throw new ArgumentException("O nome do indicador não pode ter mais de 100 caracteres.", nameof(value));

                _nome = value.Trim();
            }
        }

        public string Descricao
        {
            get => _descricao;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("A descrição do indicador não pode ser vazia ou nula.", nameof(value));

                if (value.Length > 500)
                    throw new ArgumentException("A descrição do indicador não pode ter mais de 500 caracteres.", nameof(value));

                _descricao = value.Trim();
                AtualizarDataModificacao();
            }
        }

        public TipoIndicador Tipo
        {
            get => _tipo;
            private set
            {
                if (!Enum.IsDefined(typeof(TipoIndicador), value))
                    throw new ArgumentException("Tipo de indicador inválido.", nameof(value));

                if (_tipo != value)
                {
                    _tipo = value;
                    AtualizarDataModificacao();
                }
            }
        }

        public virtual ICollection<Meta> Metas { get; private set; } = new List<Meta>();

        private Indicador(object nome) : base() { }

        public Indicador(string nome, string descricao, TipoIndicador tipo) : base()
        {
            Nome = nome;
            Descricao = descricao;
            Tipo = tipo;
        }

        public override bool EhValida()
        {
            return base.EhValida() &&
                   !string.IsNullOrWhiteSpace(Nome) &&
                   !string.IsNullOrWhiteSpace(Descricao) &&
                   Enum.IsDefined(typeof(TipoIndicador), Tipo);
        }
    }
}