using System;
using System.Collections.Generic;

namespace StyloApp.API.Entities;

public partial class VanDon
{
    public int VanDonId { get; set; }

    public int DonHangId { get; set; }

    public string? Dvvc { get; set; }

    public string? MaVanDon { get; set; }

    public string? TrangThaiGiao { get; set; } = null!;

    public decimal? PhiVanChuyen { get; set; }

    public DateOnly? NgayGui { get; set; }

    public DateOnly? NgayGiaoDuKien { get; set; }

    public virtual DonHang DonHang { get; set; } = null!;
}
