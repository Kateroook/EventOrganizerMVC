using EventOrganizerDomain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventOrganizerInfrastructure.EntityConfigurations
{
    internal class TagEntityTypeConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(e => e.Id).HasColumnName("ID");
            builder.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            builder.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false);

            builder.Metadata
                .FindNavigation(nameof(Tag.Events))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            //builder.HasMany(d => d.Events).WithMany(p => p.Tags)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "EventsTag",
            //        r => r.HasOne<Event>().WithMany()
            //            .HasForeignKey("EventId")
            //            .OnDelete(DeleteBehavior.ClientSetNull)
            //            .HasConstraintName("FK_EventsTags_Events"),
            //        l => l.HasOne<Tag>().WithMany()
            //            .HasForeignKey("TagId")
            //            .OnDelete(DeleteBehavior.ClientSetNull)
            //            .HasConstraintName("FK_EventsTags_Tags"),
            //        j =>
            //        {
            //            j.HasKey("TagId", "EventId");
            //            j.ToTable("EventsTags");
            //            j.IndexerProperty<int>("TagId")
            //                .ValueGeneratedOnAdd()
            //                .HasColumnName("TagID");
            //            j.IndexerProperty<int>("EventId").HasColumnName("EventID");
            //        });
        }
    }
}
