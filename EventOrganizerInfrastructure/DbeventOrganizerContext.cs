using System;
using System.Collections.Generic;
using EventOrganizerDomain.Model;
using EventOrganizerInfrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace EventOrganizerInfrastructure;

public partial class DbeventOrganizerContext : DbContext
{
    public DbeventOrganizerContext()
    {
    }

    public DbeventOrganizerContext(DbContextOptions<DbeventOrganizerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Place> Places { get; set; }

    public virtual DbSet<PlaceType> PlaceTypes { get; set; }

    public virtual DbSet<Registration> Registrations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-TF20TG4\\SQLEXPRESS; Database=DBEventOrganizer; Trusted_Connection=True; TrustServerCertificate=True; ", x => x.UseNetTopologySuite());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CityEntityTypeConfiguration());
        //modelBuilder.Entity<City>(entity =>
        //{
        //    entity.HasKey(e => e.Id).HasName("PK_Cities_1");

        //    entity.Property(e => e.Id).HasColumnName("ID");
        //    entity.Property(e => e.CountryId).HasColumnName("CountryID");
        //    entity.Property(e => e.Name)
        //        .HasMaxLength(200)
        //        .IsUnicode(false);

        //    entity.HasOne(d => d.Country).WithMany(p => p.Cities)
        //        .HasForeignKey(d => d.CountryId)
        //        .OnDelete(DeleteBehavior.ClientSetNull)
        //        .HasConstraintName("FK_Cities_Countries");
        //});

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedAt).HasColumnType("smalldatetime");
            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.LastUpdatedAt).HasColumnType("smalldatetime");
            entity.Property(e => e.Text).HasColumnType("text");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Event).WithMany(p => p.Comments)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Events");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Users");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Capacity)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CreatedAt).HasColumnType("smalldatetime");
            entity.Property(e => e.DateTimeEnd).HasColumnType("smalldatetime");
            entity.Property(e => e.DateTimeStart).HasColumnType("smalldatetime");
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.LastUpdatedAt).HasColumnType("smalldatetime");
            entity.Property(e => e.PictureUrl).IsUnicode(false);
            entity.Property(e => e.PlaceId).HasColumnName("PlaceID");
            entity.Property(e => e.Speaker)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.Place).WithMany(p => p.Events)
                .HasForeignKey(d => d.PlaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Events_Places");
        });

        modelBuilder.Entity<Place>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AddressLine1)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.AddressLine2)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.ContactEmail)
                .HasMaxLength(254)
                .IsUnicode(false);
            entity.Property(e => e.CoordinatesCol2).HasComputedColumnSql("([CoordinatesCol1].[STAsText]())", false);
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.ImageUrl).IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.PlaceTypeId).HasColumnName("PlaceTypeID");
            entity.Property(e => e.UnitNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Zip)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ZIP");

            entity.HasOne(d => d.City).WithMany(p => p.Places)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_Places_Cities");

            entity.HasOne(d => d.PlaceType).WithMany(p => p.Places)
                .HasForeignKey(d => d.PlaceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Places_PlaceTypes");
        });

        modelBuilder.Entity<PlaceType>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedAt).HasColumnType("smalldatetime");
            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Event).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Registrations_Events");

            entity.HasOne(d => d.User).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Registrations_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasMany(d => d.Events).WithMany(p => p.Tags)
                .UsingEntity<Dictionary<string, object>>(
                    "EventsTag",
                    r => r.HasOne<Event>().WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_EventsTags_Events"),
                    l => l.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_EventsTags_Tags"),
                    j =>
                    {
                        j.HasKey("TagId", "EventId");
                        j.ToTable("EventsTags");
                        j.IndexerProperty<int>("TagId")
                            .ValueGeneratedOnAdd()
                            .HasColumnName("TagID");
                        j.IndexerProperty<int>("EventId").HasColumnName("EventID");
                    });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email)
                .HasMaxLength(254)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Info)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.OrganizationName)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");

            entity.HasMany(d => d.Events).WithMany(p => p.Organizers)
                .UsingEntity<Dictionary<string, object>>(
                    "EventsOrganizer",
                    r => r.HasOne<Event>().WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_EventsOrganizers_Events"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("OrganizerId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_EventsOrganizers_Users"),
                    j =>
                    {
                        j.HasKey("OrganizerId", "EventId");
                        j.ToTable("EventsOrganizers");
                        j.IndexerProperty<int>("OrganizerId").HasColumnName("OrganizerID");
                        j.IndexerProperty<int>("EventId")
                            .ValueGeneratedOnAdd()
                            .HasColumnName("EventID");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

