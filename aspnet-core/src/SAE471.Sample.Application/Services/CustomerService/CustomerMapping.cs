
using AutoMapper;
using PhoneNumbers;
using SAE471.Sample.Application.Contracts;
using SAE471.Sample.Domain.Aggregates;

namespace SAE471.Sample.Application.Services.CustomerService
{
    public class CustomerMapping : Profile
    {
        public CustomerMapping()
        {
            var phoneUtil = PhoneNumberUtil.GetInstance();
            CreateMap<Customer, CustomerDTO>()
                .ForMember(des => des.PhoneNumber, opts => opts.MapFrom(src => phoneUtil.Format(phoneUtil.Parse(src.PhoneNumber.ToString(), "IR"), PhoneNumberFormat.INTERNATIONAL)));
        }


    }
}