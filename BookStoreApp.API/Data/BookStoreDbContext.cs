using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BookStoreApp.API.Data
{
    public partial class BookStoreDbContext: DbContext/* : IdentityDbContext<ApiUser>*/
    {
        public BookStoreDbContext()
        {
        }

        public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Author> Authors { get; set; } = null!;
        public virtual DbSet<Book> Books { get; set; } = null!;
        

        //        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //        {
        //            if (!optionsBuilder.IsConfigured)
        //            {
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        //                optionsBuilder.UseSqlServer("server=.;database=BookStoreDb;integrated security=True");
        //            }
        //        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Bio).HasMaxLength(255);

                entity.Property(e => e.CreatedOnUtc).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(100);
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasIndex(e => e.Isbn, "Unique_Books")
                    .IsUnique();

                entity.Property(e => e.Image).HasMaxLength(511);

                entity.Property(e => e.Isbn)
                    .HasMaxLength(50)
                    .HasColumnName("ISBN");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Summary).HasMaxLength(1023);

                entity.Property(e => e.Title).HasMaxLength(511);

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.AuthorId)
                    .HasConstraintName("FK_Books_Authors");
            });
            //modelBuilder.Entity<IdentityRole>().HasData(
            //    new IdentityRole
            //    {
            //        Name = "User",
            //        NormalizedName = "USER",
            //        Id = "8343074e-8623-4e1a-b0c1-84fb8678c8f3"
            //    },
            //    new IdentityRole
            //    {
            //        Name = "Administrator",
            //        NormalizedName = "ADMINISTRATOR",
            //        Id = "c7ac6cfe-1f10-4baf-b604-cde350db9554"
            //    }
            //);
            //var hasher = new PasswordHasher<ApiUser>();
            //modelBuilder.Entity<ApiUser>().HasData(
            //    new ApiUser
            //    {
            //        Id = "8e448afa-f008-446e-a52f-13c449803c2e",
            //        Email = "admin@bookstore.com",
            //        NormalizedEmail = "ADMIN@BOOKSTORE.COM",
            //        UserName = "admin@bookstore.com",
            //        NormalizedUserName = "ADMIN@BOOKSTORE.COM",
            //        FirstName = "System",
            //        LastName = "Admin",
            //        PasswordHash = hasher.HashPassword(null, "P@ssword1")
            //    },
            //    new ApiUser
            //    {
            //        Id = "30a24107-d279-4e37-96fd-01af5b38cb27",
            //        Email = "user@bookstore.com",
            //        NormalizedEmail = "USER@BOOKSTORE.COM",
            //        UserName = "user@bookstore.com",
            //        NormalizedUserName = "USER@BOOKSTORE.COM",
            //        FirstName = "System",
            //        LastName = "User",
            //        PasswordHash = hasher.HashPassword(null, "P@ssword1")
            //    }
            //);
            //modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            //    new IdentityUserRole<string>
            //    {
            //        RoleId = "8343074e-8623-4e1a-b0c1-84fb8678c8f3",
            //        UserId = "30a24107-d279-4e37-96fd-01af5b38cb27"
            //    },
            //    new IdentityUserRole<string>
            //    {
            //        RoleId = "c7ac6cfe-1f10-4baf-b604-cde350db9554",
            //        UserId = "8e448afa-f008-446e-a52f-13c449803c2e"
            //    }
            //);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
