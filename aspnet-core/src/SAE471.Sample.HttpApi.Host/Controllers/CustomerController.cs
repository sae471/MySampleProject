
using SAE471.Sample.Application.Contracts;
using SAE471.Sample.Domain.Aggregates;

namespace SAE471.Sample.HttpApi.Host.Controllers;

public class CustomerController : BaseController<Customer, CustomerDTO, CustomerUpsertDTO>
{
    public new ICustomerAppService AppService
    {
        get
        {
            base.AppService = LazyServiceProvider.LazyGetService<ICustomerAppService>();
            return (ICustomerAppService)base.AppService;
        }
    }
}
