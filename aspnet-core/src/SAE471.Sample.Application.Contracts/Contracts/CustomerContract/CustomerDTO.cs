using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE471.Application.DTOs;

namespace SAE471.Sample.Application.Contracts
{
    public class CustomerDTO : EntityDTO<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string BankAccountNumber { get; set; }
    }
}