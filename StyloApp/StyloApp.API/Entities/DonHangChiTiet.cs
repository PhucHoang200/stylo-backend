using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StyloApp.API.Entities;

public partial class DonHangChiTiet
{
    public int DonHangId { get; set; }

    public int BienTheId { get; set; }

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public decimal GiamGia { get; set; }

    public decimal Thue { get; set; }

    public virtual SanPhamBienThe BienThe { get; set; } = null!;

    [ForeignKey("DonHangId")]
    public virtual DonHang DonHang { get; set; } = null!;

    [ForeignKey("BienTheId")]
    public virtual SanPhamBienThe SanPhamBienThe { get; set; }

    [ForeignKey("SanPhamId")]
    public virtual SanPham SanPham { get; set; }

    public int SanPhamId { get; set; }
}
