

using SAE471.Application.Services;
using SAE471.Sample.Domain.Aggregates.CustomerAggregate;
using SAE471.Domain.Services;
using SAE471.Sample.Domain.Aggregates;
using SAE471.Sample.Application.Contracts;

namespace SAE471.Sample.Application.Services
{
    public class CustomerAppService : BaseAppService<Customer, CustomerDTO, CustomerUpsertDTO>, ICustomerAppService
    {
        private CustomerManager CustomerManager => (CustomerManager)LazyServiceProvider.LazyGetService<IDomainService<Customer>>();

        public override async Task<CustomerDTO> UpsertAsync(CustomerUpsertDTO input)
        {
            var isNew = false;
            var customer = await Repository.FindAsync(input.Id);
            if (customer == null)
            {
                isNew = true;
                customer = CustomerManager.New(input.FirstName, input.LastName, input.PhoneNumber, input.EmailAddress, input.DateOfBirth);
            }
            CustomerManager.SetBankAccountNumber(customer, input.BankAccountNumber);
            if (!isNew)
            {
                CustomerManager.DuplicateValidation(input.Id, input.FirstName, input.LastName, input.DateOfBirth);
                customer.DateOfBirth = input.DateOfBirth;
                CustomerManager.SetFirstName(customer, input.FirstName);
                CustomerManager.SetLastName(customer, input.LastName);
                CustomerManager.SetPhoneNumber(customer, input.PhoneNumber);
                CustomerManager.SetEmailAddress(customer, input.EmailAddress);
                return Mapper.Map<Customer, CustomerDTO>(await Repository.UpdateAsync(customer));
            }
            return Mapper.Map<Customer, CustomerDTO>(await Repository.InsertAsync(customer));
        }
    }
}