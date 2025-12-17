using System;
using System.Collections.Generic;

namespace StyloApp.API.Entities;

public partial class TaiKhoan
{
    public int TaiKhoanId { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhauHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int? RoleId { get; set; }

    public bool EmailConfirmed { get; set; } = false;

    public virtual ICollection<KhachHang> KhachHangs { get; set; } = new List<KhachHang>();

    public virtual ICollection<DiaChi> DiaChis { get; set; } = new List<DiaChi>();

    public virtual NhanVien? NhanVien { get; set; }

    public virtual Role? Role { get; set; }
}
