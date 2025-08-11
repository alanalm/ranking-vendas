using Ranking.Dominio.Interfaces;

namespace Ranking.Dominio.Entidades
{
    public class Meta : EntidadeBase
    {
        private decimal _valor;
        private DateTime _dataInicio;
        private DateTime? _dataFim;

        public int IndicadorId { get; private set; }

        public decimal Valor
        {
            get => _valor;
            private set
            {
                if (value <= 0)
                    throw new ArgumentException("O valor da meta deve ser maior que zero.", nameof(value));

                if (_valor != value)
                {
                    _valor = value;
                    AtualizarDataModificacao();
                }
            }
        }

        public DateTime DataInicio
        {
            get => _dataInicio;
            private set
            {
                _dataInicio = value;
                AtualizarDataModificacao();
            }
        }

        public DateTime? DataFim
        {
            get => _dataFim;
            private set
            {
                if (value.HasValue && value.Value <= DataInicio)
                    throw new ArgumentException("A data de fim deve ser posterior à data de início.", nameof(value));

                _dataFim = value;
                AtualizarDataModificacao();
            }
        }

        public bool Ativa { get; set; }

        public virtual Indicador? Indicador { get; private set; }

        private Meta() : base() { }

        public Meta(int indicadorId, decimal valor, bool ativa, DateTime dataInicio, DateTime? dataFim = null) : base()
        {
            if (indicadorId <= 0)
                throw new ArgumentException("O identificador do indicador deve ser maior que zero.", nameof(indicadorId));

            IndicadorId = indicadorId;
            Valor = valor;
            Ativa = ativa;
            DataInicio = dataInicio;
            DataFim = dataFim;
        }

        public void Atualizar(int indicadorId, decimal valor, DateTime dataInicio, DateTime? dataFim, bool ativa)
        {
            if (dataFim.HasValue && dataFim < dataInicio)
                throw new ArgumentException("Data final não pode ser menor que a data inicial.");

            IndicadorId = indicadorId;
            Valor = valor;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Ativa = ativa;
        }

        public void Ativar()
        {
            Ativa = true;
            AtualizarDataModificacao();
        }

        public void Desativar()
        {
            Ativa = false;
            AtualizarDataModificacao();
        }

        public override bool EhValida()
        {
            return base.EhValida() &&
                   IndicadorId > 0 &&
                   Valor > 0 &&
                   (!DataFim.HasValue || DataFim.Value > DataInicio);
        }
    }
}
