
using SAE471.Application.Services;
using SAE471.Sample.Domain.Aggregates;

namespace SAE471.Sample.Application.Contracts
{
    public interface ICustomerAppService : IBaseAppService<Customer, CustomerDTO, CustomerUpsertDTO>
    {

    }
}