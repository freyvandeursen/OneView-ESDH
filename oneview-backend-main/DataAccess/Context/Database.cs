using DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Context {
    internal class Database : DbContext {
        public DbSet<DBVersion> Versions { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Model.Context> Context { get; set; }
        public DbSet<ContextData> ContextData { get; set; }
        public DbSet<ContextDataType> ContextDataType { get; set; }
        public DbSet<ContextType> ContextType { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Label> Labels { get; set; }

        public Database(DbContextOptions<Database> options) : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            modelBuilder.Entity<Model.Context>()
                .ToTable("Context")
                .Property(c => c.CustomerId)
                .HasColumnName("fk_CustomerId");

            modelBuilder.Entity<Model.Context>()
                .Property(c => c.ContextTypeId)
                .HasColumnName("fk_ContextTypeId");

            modelBuilder.Entity<ContextData>()
                .Property(cd => cd.ContextId)
                .HasColumnName("fk_ContextId");

            modelBuilder.Entity<ContextData>()
                .Property(cd => cd.ContextDataTypeId)
                .HasColumnName("fk_ContextDataTypeId");

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("Contact");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CustomerId)
                      .HasColumnName("fk_CustomerId")
                      .IsRequired();
                entity.Property(e => e.Name)
                      .IsRequired();
                entity.Property(e => e.Title);
                entity.Property(e => e.PhoneNumber);
                entity.Property(e => e.Email);
            });
            
            modelBuilder.Entity<User>(entity => {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired();
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.Role).IsRequired();
            });

          
           base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Label>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("vw_Label");

                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.CustomerId).HasColumnName("CustomerId");
                entity.Property(e => e.ContextId).HasColumnName("ContextId");
                entity.Property(e => e.Name).HasColumnName("Name");
                entity.Property(e => e.Description).HasColumnName("Description");
                entity.Property(e => e.Value).HasColumnName("Value");
            });

            base.OnModelCreating(modelBuilder);

        }

    }
}