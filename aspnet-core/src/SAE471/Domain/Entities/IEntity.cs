

namespace SAE471.Domain.Entities
{
    public interface IEntity<TKey> 
    {
        TKey Id { get; }
    }
}