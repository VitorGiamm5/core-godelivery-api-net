using CoreGoDelivery.Domain.Entities.GoDelivery.Motorcycle;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreGoDelivery.Infrastructure.Database.Configuration.GoDelivery;

public class MotorcycleConfiguration : IEntityTypeConfiguration<MotorcycleEntity>
{
    public void Configure(EntityTypeBuilder<MotorcycleEntity> entity)
    {
        entity.ToTable("tb_motorcycle");
        entity.HasKey(t => t.Id);

        entity.Property(t => t.Id).HasColumnName("ID_MOTORCYCLE");
        entity.Property(t => t.YearManufacture).HasColumnName("YEAR_MANUFACTURE");
        entity.Property(t => t.PlateNormalized).HasColumnName("PLATE_NORMALIZED");
        entity.Property(t => t.MotorcycleModelId).HasColumnName("ID_FK_MODEL_MOTORCYCLE");
        entity.HasIndex(t => t.PlateNormalized)
            .IsUnique()
            .HasDatabaseName("UX_tb_motorcycle_plate_normalized");

        entity.Property(e => e.DateCreated).HasColumnName("DATE_CREATED").HasColumnType("timestamptz");
        entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");
    }
}
