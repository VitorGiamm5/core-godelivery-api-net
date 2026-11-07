using CoreGoDelivery.Domain.Entities.GoDelivery.Rental;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreGoDelivery.Infrastructure.Database.Configuration.GoDelivery;

public class RentalConfiguration : IEntityTypeConfiguration<RentalEntity>
{
    public void Configure(EntityTypeBuilder<RentalEntity> entity)
    {
        entity.ToTable("tb_rental");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id).HasColumnName("ID_RENTAL");
        entity.Property(e => e.StartDate).HasColumnName("DATE_START").HasColumnType("timestamptz");
        entity.Property(e => e.EndDate).HasColumnName("DATE_END").HasColumnType("timestamptz");
        entity.Property(e => e.EstimatedReturnDate).HasColumnName("DATE_ESTIMATED_RETURN").HasColumnType("timestamptz");
        entity.Property(e => e.ReturnedToBaseDate).HasColumnName("DATE_RETURNED_TO_BASE").HasColumnType("timestamptz").IsRequired(false);
        entity.Property(e => e.DeliverierId).HasColumnName("ID_FK_DELIVERIER");
        entity.Property(e => e.MotorcycleId).HasColumnName("ID_FK_MOTORCYCLE").IsRequired(false);
        entity.Property(e => e.RentalPlanId).HasColumnName("ID_FK_RENTAL_PLAN");
        entity.HasIndex(e => e.MotorcycleId)
            .IsUnique()
            .HasFilter("\"DATE_RETURNED_TO_BASE\" IS NULL")
            .HasDatabaseName("UX_tb_rental_active_motorcycle");

        entity.Property(e => e.DateCreated).HasColumnName("DATE_CREATED").HasColumnType("timestamptz");
        entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");
    }
}
