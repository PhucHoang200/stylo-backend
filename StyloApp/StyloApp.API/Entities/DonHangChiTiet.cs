using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StyloApp.API.Entities;

[Table("DonHang_ChiTiet")]
public partial class DonHangChiTiet
{
    // 1. Nếu bảng trong SQL KHÔNG CÓ cột tăng tự động (Identity), 
    // bạn phải dùng [Key, Column(Order = 0)] cho DonHangId và Order = 1 cho BienTheId.
    // Nhưng tốt nhất là báo cho EF biết bảng này dùng khóa ngoại làm định danh.

    [Column("DonHangID")]
    public int DonHangId { get; set; }

    [Column("BienTheID")]
    public int BienTheId { get; set; }

    public int? SoLuong { get; set; }

    public decimal? DonGia { get; set; }

    public decimal? GiamGia { get; set; }

    public decimal? Thue { get; set; }

    // 2. Chỉ giữ lại DUY NHẤT 2 thuộc tính điều hướng này
    [ForeignKey("DonHangId")]
    public virtual DonHang DonHang { get; set; } = null!;

    [ForeignKey("BienTheId")]
    public virtual SanPhamBienThe BienThe { get; set; } = null!;
}