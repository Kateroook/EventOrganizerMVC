using EventOrganizerDomain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventOrganizerInfrastructure.EntityConfigurations
{
    internal class CityEntityTypeConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(e => e.Id).HasColumnName("ID");
            builder.Property(e => e.CountryId).HasColumnName("CountryID");
            builder.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);

            builder.HasOne(d => d.Country).WithMany(p => p.Cities)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cities_Countries");
        }
    }
}
