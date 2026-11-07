using CoreGoDelivery.Domain.Entities.GoDelivery.Deliverier;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreGoDelivery.Infrastructure.Database.Configuration.GoDelivery;

public class DeliverierConfiguration : IEntityTypeConfiguration<DeliverierEntity>
{
    public void Configure(EntityTypeBuilder<DeliverierEntity> entity)
    {
        entity.ToTable("tb_deliverier");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id).HasColumnName("ID_DELIVERIER");
        entity.Property(e => e.FullName).HasColumnName("FULL_NAME");
        entity.Property(e => e.Cnpj).HasColumnName("CNPJ");
        entity.HasIndex(e => e.Cnpj)
            .IsUnique()
            .HasDatabaseName("UX_tb_deliverier_cnpj");
        entity.Property(e => e.BirthDate).HasColumnName("DATE_BIRTH").HasColumnType("timestamptz");
        entity.Property(e => e.LicenceDriverId).HasColumnName("ID_FK_LICENSE_DRIVER");

        entity.Property(e => e.DateCreated).HasColumnName("DATE_CREATED").HasColumnType("timestamptz");
        entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");
    }
}
