using System;
using System.Collections.Generic;

namespace StyloApp.API.Entities;

public partial class SanPhamBienThe
{
    public int BienTheId { get; set; }

    public int SanPhamId { get; set; }

    public int MauId { get; set; }

    public int SizeId { get; set; }

    public string Sku { get; set; } = null!;

    public string? Barcode { get; set; }

    public decimal GiaBan { get; set; }

    public decimal? GiaNhap { get; set; }

    public string TrangThai { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<AnhSanPham> AnhSanPhams { get; set; } = new List<AnhSanPham>();

    public virtual ICollection<DonHangChiTiet> DonHangChiTiets { get; set; } = new List<DonHangChiTiet>();

    public virtual MauSac Mau { get; set; } = null!;

    public virtual SanPham SanPham { get; set; } = null!;

    public virtual Size Size { get; set; } = null!;

    public virtual TonKho? TonKho { get; set; }

    public virtual ICollection<UserProductInteraction> UserProductInteractions { get; set; } = new List<UserProductInteraction>();
}
