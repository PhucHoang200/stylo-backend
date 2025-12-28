using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StyloApp.API.Entities;

namespace StyloApp.API.Data;

public partial class FashionShopContext : DbContext
{
    public FashionShopContext()
    {
    }

    public FashionShopContext(DbContextOptions<FashionShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AnhSanPham> AnhSanPhams { get; set; }

    public virtual DbSet<DanhGia> DanhGia { get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<DonHangChiTiet> DonHangChiTiets { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<MauSac> MauSacs { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<PhanLoai> PhanLoais { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<SanPhamBienThe> SanPhamBienThes { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<StagingProductDatum> StagingProductData { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<ThuongHieu> ThuongHieus { get; set; }

    public virtual DbSet<TonKho> TonKhos { get; set; }

    public virtual DbSet<UserProductInteraction> UserProductInteractions { get; set; }

    public virtual DbSet<VanDon> VanDons { get; set; }

    public virtual DbSet<DiaChi> DiaChis { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-DE6G2CH\\SQLEXPRESS;Database=fashion_shop;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnhSanPham>(entity =>
        {
            entity.HasKey(e => e.AnhId).HasName("PK__AnhSanPh__13C3343E011E0757");

            entity.ToTable("AnhSanPham");

            entity.Property(e => e.AnhId).HasColumnName("AnhID");
            entity.Property(e => e.BienTheId).HasColumnName("BienTheID");
            entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .HasColumnName("URL");

            entity.HasOne(d => d.BienThe).WithMany(p => p.AnhSanPhams)
                .HasForeignKey(d => d.BienTheId)
                .HasConstraintName("fk_img_bt");

            entity.HasOne(d => d.SanPham).WithMany(p => p.AnhSanPhams)
                .HasForeignKey(d => d.SanPhamId)
                .HasConstraintName("fk_img_sp");
        });

        modelBuilder.Entity<DanhGia>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__DanhGia__74BC79AEAA9C99C6");

            entity.HasIndex(e => new { e.SanPhamId, e.ReviewTime }, "IX_DanhGia_SanPham_Time").IsDescending(false, true);

            entity.Property(e => e.ReviewId).HasColumnName("ReviewID");
            entity.Property(e => e.ExternalUserId)
                .HasMaxLength(100)
                .HasColumnName("ExternalUserID");
            entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");
            entity.Property(e => e.Rating).HasColumnType("decimal(3, 2)");
            entity.Property(e => e.ReviewSource).HasMaxLength(20);
            entity.Property(e => e.ReviewTime)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ReviewTitle).HasMaxLength(255);
            entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");

            entity.HasOne(d => d.KhachHang).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.KhachHangId)
                .HasConstraintName("fk_dg_kh");

            entity.HasOne(d => d.SanPham).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.SanPhamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dg_sp");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.DanhMucId).HasName("PK__DanhMuc__1C53BA7B8D6EB6F6");

            entity.ToTable("DanhMuc");

            entity.Property(e => e.DanhMucId).HasColumnName("DanhMucID");
            entity.Property(e => e.PhanLoaiId).HasColumnName("PhanLoaiID");
            entity.Property(e => e.Ten).HasMaxLength(120);

            entity.HasOne(d => d.PhanLoai).WithMany(p => p.DanhMucs)
                .HasForeignKey(d => d.PhanLoaiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dm_pl");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.DonHangId).HasName("PK__DonHang__D159F4DE310E3494");

            entity.ToTable("DonHang");

            entity.Property(e => e.DonHangId).HasColumnName("DonHangID");
            entity.Property(e => e.KenhBan).HasMaxLength(10);
            entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");
            entity.Property(e => e.NgayDat)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.PhiVanChuyen).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Thue).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.TongGiamGia).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.TongThanhToan).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.TongTienHang).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.TrangThai).HasMaxLength(10);
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.KhachHang).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.KhachHangId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dh_kh");
        });

        modelBuilder.Entity<DonHangChiTiet>(entity =>
        {
            // 1. Khai báo khóa chính tổ hợp (Composite Key)
            entity.HasKey(e => new { e.DonHangId, e.BienTheId }).HasName("pk_dhct");

            entity.ToTable("DonHang_ChiTiet");

            // 2. Map tên cột chính xác với Database (viết hoa ID)
            entity.Property(e => e.DonHangId).HasColumnName("DonHangID");
            entity.Property(e => e.BienTheId).HasColumnName("BienTheID");

            entity.Property(e => e.DonGia).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.GiamGia).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Thue).HasColumnType("decimal(12, 2)");

            // 3. Cấu hình quan hệ với bảng Biến Thể - CHỈ GIỮ LẠI MỘT CÁI
            entity.HasOne(d => d.BienThe)
                .WithMany(p => p.DonHangChiTiets)
                .HasForeignKey(d => d.BienTheId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dhct_bt");

            // 4. Cấu hình quan hệ với bảng Đơn Hàng
            entity.HasOne(d => d.DonHang)
                .WithMany(p => p.DonHangChiTiets)
                .HasForeignKey(d => d.DonHangId)
                .OnDelete(DeleteBehavior.Cascade) // Thường đơn hàng xóa thì chi tiết xóa theo
                .HasConstraintName("fk_dhct_dh");

            // TRIỆT TIÊU: Xóa bỏ hoàn toàn đoạn .HasOne(d => d.BienThe).WithMany() ở cuối
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.KhachHangId).HasName("PK__KhachHan__880F211BDF521FE6");

            entity.ToTable("KhachHang");

            entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasPrecision(3)
                .HasColumnName("deleted_at");
            entity.Property(e => e.Email).HasMaxLength(120);
            entity.Property(e => e.GioiTinh).HasMaxLength(5);
            entity.Property(e => e.HoTen).HasMaxLength(120);
            entity.Property(e => e.NgaySinh).HasColumnName("ngaySinh");
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.TaiKhoanId).HasColumnName("TaiKhoanID");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.TaiKhoan).WithMany(p => p.KhachHangs)
                .HasForeignKey(d => d.TaiKhoanId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_kh_tk");
        });

        modelBuilder.Entity<MauSac>(entity =>
        {
            entity.HasKey(e => e.MauId).HasName("PK__MauSac__28166A682FC447BE");

            entity.ToTable("MauSac");

            entity.HasIndex(e => e.Ten, "UQ__MauSac__C451FA83886645AC").IsUnique();

            entity.Property(e => e.MauId).HasColumnName("MauID");
            entity.Property(e => e.MaHex)
                .HasMaxLength(7)
                .IsFixedLength();
            entity.Property(e => e.Ten).HasMaxLength(50);
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.NhanVienId).HasName("PK__NhanVien__E27FD7EA2508AB4A");

            entity.ToTable("NhanVien");

            entity.HasIndex(e => e.TaiKhoanId, "UQ__NhanVien__9A124B64070A5AE7").IsUnique();

            entity.Property(e => e.NhanVienId).HasColumnName("NhanVienID");
            entity.Property(e => e.ChucVu).HasMaxLength(60);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Email).HasMaxLength(120);
            entity.Property(e => e.GioiTinh).HasMaxLength(5);
            entity.Property(e => e.HoTen).HasMaxLength(120);
            entity.Property(e => e.NgaySinh).HasColumnName("ngaySinh");
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.TaiKhoanId).HasColumnName("TaiKhoanID");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.TaiKhoan).WithOne(p => p.NhanVien)
                .HasForeignKey<NhanVien>(d => d.TaiKhoanId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_nv_tk");
        });

        modelBuilder.Entity<PhanLoai>(entity =>
        {
            entity.HasKey(e => e.PhanLoaiId).HasName("PK__PhanLoai__759236B90BD9F2ED");

            entity.ToTable("PhanLoai");

            entity.HasIndex(e => e.Ten, "UQ__PhanLoai__C451FA83123764B4").IsUnique();

            entity.Property(e => e.PhanLoaiId).HasColumnName("PhanLoaiID");
            entity.Property(e => e.Ten).HasMaxLength(120);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1AECA22481");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.SanPhamId).HasName("PK__SanPham__05180FF4622C61F2");

            entity.ToTable("SanPham");

            entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DanhMucId).HasColumnName("DanhMucID");
            entity.Property(e => e.ExternalId)
                .HasMaxLength(50)
                .HasColumnName("ExternalID");
            entity.Property(e => e.TenSanPham).HasMaxLength(160);
            entity.Property(e => e.ThuongHieuId).HasColumnName("ThuongHieuID");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.ThuongHieu).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.ThuongHieuId)
                .HasConstraintName("fk_sp_th");
        });

        modelBuilder.Entity<SanPhamBienThe>(entity =>
        {
            entity.HasKey(e => e.BienTheId).HasName("PK__SanPham___B570862269961211");

            entity.ToTable("SanPham_BienThe");

            entity.HasIndex(e => e.Sku, "UQ__SanPham___CA1ECF0D363956D3").IsUnique();

            entity.HasIndex(e => new { e.SanPhamId, e.MauId, e.SizeId }, "uq_bt_combo").IsUnique();

            entity.Property(e => e.BienTheId).HasColumnName("BienTheID");
            entity.Property(e => e.Barcode).HasMaxLength(64);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.GiaBan).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.GiaNhap).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.MauId).HasColumnName("MauID");
            entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");
            entity.Property(e => e.SizeId).HasColumnName("SizeID");
            entity.Property(e => e.Sku)
                .HasMaxLength(64)
                .HasColumnName("SKU");
            entity.Property(e => e.TrangThai).HasMaxLength(10);
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Mau).WithMany(p => p.SanPhamBienThes)
                .HasForeignKey(d => d.MauId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bt_mau");

            entity.HasOne(d => d.SanPham).WithMany(p => p.SanPhamBienThes)
                .HasForeignKey(d => d.SanPhamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bt_sp");

            entity.HasOne(d => d.Size).WithMany(p => p.SanPhamBienThes)
                .HasForeignKey(d => d.SizeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bt_size");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.SizeId).HasName("PK__Size__83BD095AC9C7E427");

            entity.ToTable("Size");

            entity.HasIndex(e => e.KyHieu, "UQ__Size__1AC4F8FA5E4EC3CF").IsUnique();

            entity.Property(e => e.SizeId).HasColumnName("SizeID");
            entity.Property(e => e.KyHieu).HasMaxLength(10);
        });

        modelBuilder.Entity<StagingProductDatum>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Staging_ProductData");

            entity.Property(e => e.Brand)
                .HasMaxLength(255)
                .HasColumnName("brand");
            entity.Property(e => e.Category)
                .HasMaxLength(255)
                .HasColumnName("category");
            entity.Property(e => e.Cost)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("cost");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductId)
                .HasMaxLength(100)
                .HasColumnName("product_id");
            entity.Property(e => e.ProductName)
                .HasMaxLength(255)
                .HasColumnName("product_name");
            entity.Property(e => e.Rating)
                .HasColumnType("decimal(3, 2)")
                .HasColumnName("rating");
            entity.Property(e => e.SubCategory)
                .HasMaxLength(255)
                .HasColumnName("sub_category");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.TaiKhoanId).HasName("PK__TaiKhoan__9A124B65AB518721");

            entity.ToTable("TaiKhoan");

            entity.HasIndex(e => e.TenDangNhap, "uq_tk_username").IsUnique();

            entity.Property(e => e.TaiKhoanId).HasColumnName("TaiKhoanID");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.MatKhauHash).HasMaxLength(255);
            entity.Property(e => e.TenDangNhap).HasMaxLength(60);
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Role).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("fk_tk_role");
        });

        modelBuilder.Entity<ThuongHieu>(entity =>
        {
            entity.HasKey(e => e.ThuongHieuId).HasName("PK__ThuongHi__4430751C847B04F9");

            entity.ToTable("ThuongHieu");

            entity.HasIndex(e => e.Ten, "UQ__ThuongHi__C451FA834F72A26D").IsUnique();

            entity.Property(e => e.ThuongHieuId).HasColumnName("ThuongHieuID");
            entity.Property(e => e.Ten).HasMaxLength(120);
        });

        modelBuilder.Entity<TonKho>(entity =>
        {
            entity.HasKey(e => e.BienTheId).HasName("PK__TonKho__B5708622F8B02283");

            entity.ToTable("TonKho");

            entity.Property(e => e.BienTheId)
                .ValueGeneratedNever()
                .HasColumnName("BienTheID");
            entity.Property(e => e.Available).HasComputedColumnSql("([OnHand]-[Reserved])", true);
            entity.Property(e => e.AvgCost).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.BienThe).WithOne(p => p.TonKho)
                .HasForeignKey<TonKho>(d => d.BienTheId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ton_bt");
        });

        modelBuilder.Entity<UserProductInteraction>(entity =>
        {
            entity.HasKey(e => e.InteractionId).HasName("PK__User_Pro__922C03762E6BD2AE");

            entity.ToTable("User_Product_Interaction");

            entity.HasIndex(e => new { e.SanPhamId, e.OccurredAt }, "IX_EVT_Product_Time").IsDescending(false, true);

            entity.HasIndex(e => new { e.KhachHangId, e.OccurredAt }, "IX_EVT_User_Time").IsDescending(false, true);

            entity.Property(e => e.InteractionId).HasColumnName("InteractionID");
            entity.Property(e => e.BienTheId).HasColumnName("BienTheID");
            entity.Property(e => e.Channel).HasMaxLength(20);
            entity.Property(e => e.Device).HasMaxLength(30);
            entity.Property(e => e.EventType).HasMaxLength(16);
            entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");
            entity.Property(e => e.OccurredAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Referrer).HasMaxLength(60);
            entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");
            entity.Property(e => e.SessionId)
                .HasMaxLength(64)
                .HasColumnName("SessionID");

            entity.HasOne(d => d.BienThe).WithMany(p => p.UserProductInteractions)
                .HasForeignKey(d => d.BienTheId)
                .HasConstraintName("fk_evt_bt");

            entity.HasOne(d => d.KhachHang).WithMany(p => p.UserProductInteractions)
                .HasForeignKey(d => d.KhachHangId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_evt_kh");

            entity.HasOne(d => d.SanPham).WithMany(p => p.UserProductInteractions)
                .HasForeignKey(d => d.SanPhamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_evt_sp");
        });

        modelBuilder.Entity<VanDon>(entity =>
        {
            entity.HasKey(e => e.VanDonId).HasName("PK__VanDon__27AB7C5F653F0BD2");

            entity.ToTable("VanDon");

            entity.Property(e => e.VanDonId).HasColumnName("VanDonID");
            entity.Property(e => e.DonHangId).HasColumnName("DonHangID");
            entity.Property(e => e.Dvvc)
                .HasMaxLength(80)
                .HasColumnName("DVVC");
            entity.Property(e => e.MaVanDon).HasMaxLength(120);
            entity.Property(e => e.PhiVanChuyen).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.TrangThaiGiao).HasMaxLength(12);

            entity.HasOne(d => d.DonHang).WithMany(p => p.VanDons)
                .HasForeignKey(d => d.DonHangId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vd_dh");
        });

        modelBuilder.Entity<DiaChi>(entity =>
        {
            entity.HasKey(e => e.DiaChiId).HasName("PK__DiaChi__PK"); // Tên PK tùy chọn

            entity.ToTable("DiaChi");

            entity.Property(e => e.DiaChiId).HasColumnName("DiaChiID");
            entity.Property(e => e.TaiKhoanId).HasColumnName("TaiKhoanID");

            entity.Property(e => e.DiaChiChiTiet)
                .HasMaxLength(255);

            entity.Property(e => e.LoaiDiaChi)
                .HasMaxLength(20)
                .HasDefaultValue("Home");

            entity.Property(e => e.IsDefault)
                .HasDefaultValue(false);

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            // Thiết lập mối quan hệ ngoại với TaiKhoan
            entity.HasOne(d => d.TaiKhoan)
                .WithMany(p => p.DiaChis)
                .HasForeignKey(d => d.TaiKhoanId)
                .OnDelete(DeleteBehavior.Cascade) // Xóa tài khoản thì xóa luôn địa chỉ
                .HasConstraintName("FK_DiaChi_TaiKhoan");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
