using System;
using System.Collections.Generic;

namespace StyloApp.API.Entities;

public partial class SanPham
{
    public int SanPhamId { get; set; }

    public string TenSanPham { get; set; } = null!;

    public string? MoTa { get; set; }

    public int? DanhMucId { get; set; }

    public int? ThuongHieuId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? ExternalId { get; set; }

    public virtual ICollection<AnhSanPham> AnhSanPhams { get; set; } = new List<AnhSanPham>();

    public virtual ICollection<DanhGia> DanhGia { get; set; } = new List<DanhGia>();

    public virtual ICollection<SanPhamBienThe> SanPhamBienThes { get; set; } = new List<SanPhamBienThe>();

    public virtual ThuongHieu? ThuongHieu { get; set; }

    public virtual ICollection<UserProductInteraction> UserProductInteractions { get; set; } = new List<UserProductInteraction>();
}
