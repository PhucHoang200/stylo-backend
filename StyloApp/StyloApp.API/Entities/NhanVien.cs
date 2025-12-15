using System;
using System.Collections.Generic;

namespace StyloApp.API.Entities;

public partial class NhanVien
{
    public int NhanVienId { get; set; }

    public string HoTen { get; set; } = null!;

    public string GioiTinh { get; set; } = null!;

    public DateOnly NgaySinh { get; set; }

    public string? Email { get; set; }

    public string? SoDienThoai { get; set; }

    public string? ChucVu { get; set; }

    public int? TaiKhoanId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual TaiKhoan? TaiKhoan { get; set; }
}
