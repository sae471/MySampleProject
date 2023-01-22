
namespace SAE471.Domain.Entities
{
    public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey>
    {
        protected AggregateRoot(){}
        protected AggregateRoot(TKey id){}
    }
}