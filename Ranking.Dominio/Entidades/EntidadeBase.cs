namespace Ranking.Dominio.Entidades
{
    public abstract class EntidadeBase
    {
        public int Id { get; protected set; }
        public DateTime DataCriacao { get; protected set; }
        public DateTime? DataAtualizacao { get; protected set; }

        /// Construtor protegido para garantir que apenas classes derivadas possam instanciar.
        protected EntidadeBase()
        {
            DataCriacao = DateTime.UtcNow;
        }

        /// Método para atualizar a data de modificação.
        /// Deve ser chamado sempre que a entidade for modificada.
        protected void AtualizarDataModificacao()
        {
            DataAtualizacao = DateTime.UtcNow;
        }

        //protected void Set<T>(ref T campo, T valor, Action? extra = null)
        //{
        //    if (!EqualityComparer<T>.Default.Equals(campo, valor))
        //    {
        //        campo = valor;
        //        AtualizarDataModificacao();
        //        extra?.Invoke();
        //    }
        //}

        //set => Set(ref _descricao, ValidarDescricao(value));

        public virtual bool EhValida()
        {
            return Id >= 0;
        }

        //public override bool Equals(object? obj)
        //{
        //    if (obj is not EntidadeBase entidade)
        //        return false;

        //    if (ReferenceEquals(this, entidade))
        //        return true;

        //    if (GetType() != entidade.GetType())
        //        return false;

        //    return Id != 0 && Id == entidade.Id;
        //}

        //public override int GetHashCode()
        //{
        //    return HashCode.Combine(GetType(), Id);
        //}

        public static bool operator ==(EntidadeBase? esquerda, EntidadeBase? direita)
        {
            return Equals(esquerda, direita);
        }

        public static bool operator !=(EntidadeBase? esquerda, EntidadeBase? direita)
        {
            return !Equals(esquerda, direita);
        }
    }
}
