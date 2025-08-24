using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Demo1.Models;

public partial class QuanlysukienContext : DbContext
{
    public QuanlysukienContext()
    {
    }

    public QuanlysukienContext(DbContextOptions<QuanlysukienContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Dangkysukien> Dangkysukiens { get; set; }

    public virtual DbSet<Khoa> Khoas { get; set; }

    public virtual DbSet<Nguoidung> Nguoidungs { get; set; }

    public virtual DbSet<Sukien> Sukiens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
    "Data Source=KHANG\\SQLEXPRESS;Initial Catalog=QUANLYSUKIEN;Persist Security Info=True;User ID=TienKhang23520699;Password=khang09022005;Trust Server Certificate=True",
    options => options.EnableRetryOnFailure(
        maxRetryCount: 5,
        maxRetryDelay: TimeSpan.FromSeconds(10),
        errorNumbersToAdd: null));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dangkysukien>(entity =>
        {
            entity.HasKey(e => e.Madk).HasName("PK_DKSK");

            entity.ToTable("DANGKYSUKIEN");

            entity.Property(e => e.Madk)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MADK");
            entity.Property(e => e.Mand)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MAND");
            entity.Property(e => e.Mask)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MASK");
            entity.Property(e => e.Thoigiandangky)
                .HasColumnType("smalldatetime")
                .HasColumnName("THOIGIANDANGKY");
            entity.Property(e => e.Xacnhanthamgia)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("XACNHANTHAMGIA");

            entity.HasOne(d => d.MandNavigation).WithMany(p => p.Dangkysukiens)
                .HasForeignKey(d => d.Mand)
                .HasConstraintName("FK_NDDK_ND");

            entity.HasOne(d => d.MaskNavigation).WithMany(p => p.Dangkysukiens)
                .HasForeignKey(d => d.Mask)
                .HasConstraintName("FK_DKSK_SK");
        });

        modelBuilder.Entity<Khoa>(entity =>
        {
            entity.HasKey(e => e.Makhoa);

            entity.ToTable("KHOA");

            entity.Property(e => e.Makhoa)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MAKHOA");
            entity.Property(e => e.Tenkhoa).HasColumnName("TENKHOA");
        });

        modelBuilder.Entity<Nguoidung>(entity =>
        {
            entity.HasKey(e => e.Mand).HasName("PK_ND");

            entity.ToTable("NGUOIDUNG");

            entity.Property(e => e.Mand)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MAND");
            entity.Property(e => e.Email)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Gioitinh)
                .HasMaxLength(4)
                .HasColumnName("GIOITINH");
            entity.Property(e => e.Hoten)
                .HasMaxLength(30)
                .HasColumnName("HOTEN");
            entity.Property(e => e.Imageuser).HasColumnName("IMAGEUSER");
            entity.Property(e => e.Makhoa)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MAKHOA");
            entity.Property(e => e.Masvgv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MASVGV");
            entity.Property(e => e.Passworduser)
                .IsUnicode(false)
                .HasColumnName("PASSWORDUSER");
            entity.Property(e => e.Roleuser).HasColumnName("ROLEUSER");
            entity.Property(e => e.Sdt)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("SDT");

            entity.HasOne(d => d.MakhoaNavigation).WithMany(p => p.Nguoidungs)
                .HasForeignKey(d => d.Makhoa)
                .HasConstraintName("FK_ND_KHOA");
        });

        modelBuilder.Entity<Sukien>(entity =>
        {
            entity.HasKey(e => e.Mask).HasName("PK_SK");

            entity.ToTable("SUKIEN");

            entity.Property(e => e.Mask)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MASK");
            entity.Property(e => e.Duyet).HasColumnName("DUYET");
            entity.Property(e => e.Dvtc)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("DVTC");
            entity.Property(e => e.Imageevent).HasColumnName("IMAGEEVENT");
            entity.Property(e => e.Mandb)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MANDB");
            entity.Property(e => e.Mota).HasColumnName("MOTA");
            entity.Property(e => e.Ngaybatdau)
                .HasColumnType("smalldatetime")
                .HasColumnName("NGAYBATDAU");
            entity.Property(e => e.Ngaydongdangky)
                .HasColumnType("smalldatetime")
                .HasColumnName("NGAYDONGDANGKY");
            entity.Property(e => e.Ngayketthuc)
                .HasColumnType("smalldatetime")
                .HasColumnName("NGAYKETTHUC");
            entity.Property(e => e.Ngaymodangky)
                .HasColumnType("smalldatetime")
                .HasColumnName("NGAYMODANGKY");
            entity.Property(e => e.Nhataitro).HasColumnName("NHATAITRO");
            entity.Property(e => e.Phanthuong).HasColumnName("PHANTHUONG");
            entity.Property(e => e.Soluongthamgia).HasColumnName("SOLUONGTHAMGIA");
            entity.Property(e => e.Tensk).HasColumnName("TENSK");
            entity.Property(e => e.Theloai).HasColumnName("THELOAI");
            entity.Property(e => e.Venue).HasColumnName("VENUE");

            entity.HasOne(d => d.DvtcNavigation).WithMany(p => p.Sukiens)
                .HasForeignKey(d => d.Dvtc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SUKIEN_KHOA");

            entity.HasOne(d => d.MandbNavigation).WithMany(p => p.Sukiens)
                .HasForeignKey(d => d.Mandb)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MANDB_ND");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
