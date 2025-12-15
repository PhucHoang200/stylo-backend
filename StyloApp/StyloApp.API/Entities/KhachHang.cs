using System;
using System.Collections.Generic;

namespace StyloApp.API.Entities;

public partial class KhachHang
{
    public int KhachHangId { get; set; }

    public string? HoTen { get; set; } = null!;

    public string? GioiTinh { get; set; } = null!;

    public DateOnly? NgaySinh { get; set; }

    public string Email { get; set; } = null!;

    public string? SoDienThoai { get; set; }

    public int? TaiKhoanId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<DanhGium> DanhGia { get; set; } = new List<DanhGium>();

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual TaiKhoan? TaiKhoan { get; set; }

    public virtual ICollection<UserProductInteraction> UserProductInteractions { get; set; } = new List<UserProductInteraction>();
}
