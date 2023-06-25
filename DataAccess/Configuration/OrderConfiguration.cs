using BussinessObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configuration;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Order");
        builder.HasKey(x => x.OrderId);
        builder.HasOne(x => x.User)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.MemberId);
        builder.Property(x => x.OrderDate);
        builder.Property(x => x.RequiredDate);
        builder.Property(x => x.ShippedDate);
        builder.Property(x => x.Freight);
    }
}