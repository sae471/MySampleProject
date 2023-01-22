
using System;
using SAE471.Domain.Entities;

namespace SAE471.Sample.Domain.Aggregates
{
    public class Customer : AggregateRoot<Guid>
    {
        public virtual string FirstName { get; internal set; }
        public virtual string LastName { get; internal set; }
        public virtual DateTime DateOfBirth { get; set; }
        public virtual Int64 PhoneNumber { get; internal set; }
        public virtual string EmailAddress { get; internal set; }
        public virtual string BankAccountNumber { get; internal set; }

        protected Customer()
        {

        }

        public Customer(Guid id, string firstName, string lastName, Int64 phone, string emailAddress) : base(id)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.PhoneNumber = phone;
            this.EmailAddress = emailAddress;
        }
    }
}