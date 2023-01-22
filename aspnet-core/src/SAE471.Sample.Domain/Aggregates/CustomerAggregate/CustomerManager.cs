using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Text.RegularExpressions;
using SAE471.Domain.Repositories;
using SAE471.Domain.Services;
using PhoneNumbers;

namespace SAE471.Sample.Domain.Aggregates.CustomerAggregate
{
    public class CustomerManager : DomainService<Customer>
    {
        public Customer New(string firstName, string lastName, Int64 phoneNumber, string emailAddress, DateTime dateOfBirth)
        {
            var id = new Guid();
            NullorEmptyValidation(firstName, "Firstname");
            NullorEmptyValidation(lastName, "Lastname");
            DuplicateValidation(id, firstName, lastName, dateOfBirth);
            PhoneNumberValidation(phoneNumber);
            EmailAddressValidation(emailAddress);
            var customer = new Customer(id, firstName, lastName, phoneNumber, emailAddress);
            customer.DateOfBirth = dateOfBirth;
            return customer;
        }

        public void SetFirstName(Customer customer, string firstName)
        {
            NullorEmptyValidation(firstName, "Firstname");
            customer.FirstName = firstName;
        }

        public void SetLastName(Customer customer, string lastName)
        {
            NullorEmptyValidation(lastName, "Lastname");
            customer.LastName = lastName;
        }

        public void SetPhoneNumber(Customer customer, Int64 phoneNumber)
        {
            PhoneNumberValidation(phoneNumber);
            customer.PhoneNumber = phoneNumber;
        }

        public void SetEmailAddress(Customer customer, string emailAddress)
        {
            EmailAddressValidation(emailAddress);
            customer.EmailAddress = emailAddress;
        }

        public void SetBankAccountNumber(Customer customer, string bankAccountNumber)
        {
            BankAccountNumberValidation(bankAccountNumber);
            customer.BankAccountNumber = bankAccountNumber;
        }

        public void DuplicateValidation(Guid id, string firstName, string lastName, DateTime dateOfBirth)
        {
            var isDuplicated = Repository.GetQueryableAsync().Result.Any(it =>
            !it.Id.Equals(id)
            && it.FirstName.Trim().ToLower().Equals(firstName.Trim().ToLower())
            && it.LastName.Trim().ToLower().Equals(lastName.Trim().ToLower())
            && it.DateOfBirth.Date.Equals(dateOfBirth.Date));

            if (isDuplicated)
            {
                throw new ValidationException("Customer is already to exist!!");
            }
        }

        public void NullorEmptyValidation(object input, string fieldName)
        {
            if (input is null || String.IsNullOrWhiteSpace(input.ToString()))
            {
                throw new ValidationException($"{fieldName} Could not be null!!");
            }
        }

        public void PhoneNumberValidation(Int64 phoneNumber)
        {
            NullorEmptyValidation(phoneNumber, "Phone number");
            var phoneUtil = PhoneNumberUtil.GetInstance();
            var _phoneNumber = phoneUtil.Parse(phoneNumber.ToString(), "IR");

            if (phoneUtil.GetNumberType(_phoneNumber) != PhoneNumberType.MOBILE)
            {
                throw new ValidationException($"The phone number is not a valid mobile number format!!");
            }
        }

        public void EmailAddressValidation(string emailAddress)
        {
            NullorEmptyValidation(emailAddress, "Email address");
            try
            {
                MailAddress m = new MailAddress(emailAddress);
            }
            catch (FormatException)
            {
                throw new ValidationException("The email address is incorrect fomrat!!");
            }
        }

        public void BankAccountNumberValidation(string bankAccountNumber)
        {
            if (Regex.IsMatch(bankAccountNumber, "((\\d{4})-){3}\\d{4}"))
            {
                throw new ValidationException("The account number is incorrect fomrat!!");
            }
        }
    }
}