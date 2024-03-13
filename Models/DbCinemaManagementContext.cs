using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CinemaManagement.Models;

public partial class DbCinemaManagementContext : DbContext
{
    public DbCinemaManagementContext()
    {
    }

    public DbCinemaManagementContext(DbContextOptions<DbCinemaManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AgeCertificate> AgeCertificates { get; set; }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<BillVoucher> BillVouchers { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<ShowTime> ShowTimes { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.\\SqlExpress; Trusted_Connection=Yes; Initial Catalog=db_CinemaManagement; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__349DA5A667912836");

            entity.ToTable("Account");

            entity.Property(e => e.AccountId).ValueGeneratedNever();
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.Fullname).HasMaxLength(30);
            entity.Property(e => e.Gender).HasMaxLength(8);
            entity.Property(e => e.Password).HasMaxLength(30);
            entity.Property(e => e.Username).HasMaxLength(30);
        });

        modelBuilder.Entity<AgeCertificate>(entity =>
        {
            entity.HasKey(e => e.AgeCertificateId).HasName("PK__AgeCerti__64F2799DB0BE1E10");

            entity.ToTable("AgeCertificate");

            entity.Property(e => e.AgeCertificateId).ValueGeneratedNever();
            entity.Property(e => e.BackgroundColor).HasMaxLength(30);
            entity.Property(e => e.DisplayContent).HasMaxLength(30);
            entity.Property(e => e.ForegroundColor).HasMaxLength(30);
        });

        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.BillId).HasName("PK__Bill__11F2FC6AD3D07370");

            entity.ToTable("Bill");

            entity.Property(e => e.BillId).ValueGeneratedNever();
            entity.Property(e => e.BookingTime).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.Bills)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Bill_Account");
        });

        modelBuilder.Entity<BillVoucher>(entity =>
        {
            entity.HasKey(e => new { e.BillId, e.VoucherId }).HasName("PK__BillVouc__125C1BF855451402");

            entity.ToTable("BillVoucher");

            entity.Property(e => e.AppliedTime).HasColumnType("datetime");

            entity.HasOne(d => d.Bill).WithMany(p => p.BillVouchers)
                .HasForeignKey(d => d.BillId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BillVoucher_Bill");

            entity.HasOne(d => d.Voucher).WithMany(p => p.BillVouchers)
                .HasForeignKey(d => d.VoucherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BillVoucher_Voucher");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("PK__Genre__0385057EA0DE1C9C");

            entity.ToTable("Genre");

            entity.Property(e => e.GenreId).ValueGeneratedNever();
            entity.Property(e => e.GenreName).HasMaxLength(30);
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieId).HasName("PK__Movie__4BD2941AF49CB936");

            entity.ToTable("Movie");

            entity.Property(e => e.MovieId).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Imdbrating).HasColumnName("IMDBRating");
            entity.Property(e => e.PosterPath).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(30);
            entity.Property(e => e.TrailerPath).HasMaxLength(100);

            entity.HasOne(d => d.AgeCertificate).WithMany(p => p.Movies)
                .HasForeignKey(d => d.AgeCertificateId)
                .HasConstraintName("FK_AgeCertificate");

            entity.HasOne(d => d.Director).WithMany(p => p.Movies)
                .HasForeignKey(d => d.DirectorId)
                .HasConstraintName("FK_Movie_Person");

            entity.HasMany(d => d.Genres).WithMany(p => p.Movies)
                .UsingEntity<Dictionary<string, object>>(
                    "MovieGenre",
                    r => r.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_MovieGenre_Genre"),
                    l => l.HasOne<Movie>().WithMany()
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_MovieGenre_Movie"),
                    j =>
                    {
                        j.HasKey("MovieId", "GenreId").HasName("PK__MovieGen__BBEAC44DCAF5C5C6");
                        j.ToTable("MovieGenre");
                    });

            entity.HasMany(d => d.People).WithMany(p => p.MoviesNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "MovieActor",
                    r => r.HasOne<Person>().WithMany()
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_MovieActor_Person"),
                    l => l.HasOne<Movie>().WithMany()
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_MovieActor_Movie"),
                    j =>
                    {
                        j.HasKey("MovieId", "PersonId").HasName("PK__MovieAct__01706BA491BCC65B");
                        j.ToTable("MovieActor");
                    });
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.PersonId).HasName("PK__Person__AA2FFBE540A5C4D3");

            entity.ToTable("Person");

            entity.Property(e => e.PersonId).ValueGeneratedNever();
            entity.Property(e => e.AvatarPath).HasMaxLength(100);
            entity.Property(e => e.Biography).HasMaxLength(1000);
            entity.Property(e => e.Fullname).HasMaxLength(30);
        });

        modelBuilder.Entity<ShowTime>(entity =>
        {
            entity.HasKey(e => e.ShowTimeId).HasName("PK__ShowTime__DF1BC81FC1DEB901");

            entity.ToTable("ShowTime");

            entity.Property(e => e.ShowTimeId).ValueGeneratedNever();
            entity.Property(e => e.ShowDate).HasColumnType("datetime");

            entity.HasOne(d => d.Movie).WithMany(p => p.ShowTimes)
                .HasForeignKey(d => d.MovieId)
                .HasConstraintName("FK_ShowTime_Movie");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__Ticket__712CC6071004B117");

            entity.ToTable("Ticket");

            entity.Property(e => e.TicketId).ValueGeneratedNever();
            entity.Property(e => e.Row).HasMaxLength(5);

            entity.HasOne(d => d.Bill).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.BillId)
                .HasConstraintName("FK_Ticket_Bill");

            entity.HasOne(d => d.ShowTime).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ShowTimeId)
                .HasConstraintName("FK_Ticket_ShowTime");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.VoucherId).HasName("PK__Voucher__3AEE7921C3AF3DD6");

            entity.ToTable("Voucher");

            entity.Property(e => e.VoucherId).ValueGeneratedNever();
            entity.Property(e => e.VoucherCode).HasMaxLength(30);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
