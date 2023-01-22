using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SAE471.Application.DTOs;

namespace SAE471.Sample.Application.Contracts
{
    public class CustomerUpsertDTO : EntityDTO<Guid>
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        [Required]
        public Int64 PhoneNumber { get; set; }
        [Required]
        [MaxLength(200)]
        public string EmailAddress { get; set; }
        public string BankAccountNumber { get; set; }
    }
}