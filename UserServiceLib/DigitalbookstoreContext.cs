using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CommonLib.Models;

namespace UserServiceLib;

public partial class DigitalbookstoreContext : DbContext
{
    public DigitalbookstoreContext()
    {
    }

    public DigitalbookstoreContext(DbContextOptions<DigitalbookstoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=CTSDOTNET931; Database=DIGITALBOOKSTORE; User ID=sa; Password=pass@word1;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("Book");

            entity.Property(e => e.BookId).ValueGeneratedNever();
            entity.Property(e => e.Auther).HasMaxLength(100);
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.PublishedDate).HasMaxLength(50);
            entity.Property(e => e.PublisherName).HasMaxLength(100);

            entity.HasOne(d => d.Category).WithMany(p => p.Books)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Book__CategoryId__2E1BDC42");

            entity.HasOne(d => d.Publisher).WithMany(p => p.Books)
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Book__PublisherI__2F10007B");

            entity.HasOne(d => d.User).WithMany(p => p.Books)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Book__UserId__300424B4");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.CategoryId).ValueGeneratedNever();
            entity.Property(e => e.CategoryName).HasMaxLength(50);
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.ToTable("Publisher");

            entity.Property(e => e.PublisherId).ValueGeneratedNever();
            entity.Property(e => e.PublisherName).HasMaxLength(50);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.RoleId).ValueGeneratedNever();
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.ToTable("Subscription");

            entity.Property(e => e.SubscriptionId).ValueGeneratedNever();
            entity.Property(e => e.SubscriptionDate).HasMaxLength(50);

            entity.HasOne(d => d.Book).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subscript__BookI__30F848ED");

            entity.HasOne(d => d.User).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subscript__UserI__31EC6D26");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_User");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.UserType).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__32E0915F");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
