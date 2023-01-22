using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAE471.Sample.Domain.Aggregates;

namespace SAE471.Sample.EFCore.Configurations
{
    public class CustomerConfigure : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(it => it.FirstName).HasMaxLength(100).IsRequired();
            builder.Property(it => it.LastName).HasMaxLength(100).IsRequired();
            builder.Property(it => it.PhoneNumber).HasColumnType("bigint").IsRequired();
            builder.Property(it => it.EmailAddress).HasColumnType("varchar(200)").IsRequired();
            builder.Property(it => it.BankAccountNumber).HasColumnType("varchar(200)").IsRequired(false);
        }
    }
}