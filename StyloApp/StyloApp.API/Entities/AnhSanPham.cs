using System;
using System.Collections.Generic;

namespace StyloApp.API.Entities;

public partial class AnhSanPham
{
    public int AnhId { get; set; }

    public int? SanPhamId { get; set; }

    public int? BienTheId { get; set; }

    public string Url { get; set; } = null!;

    public bool IsPrimary { get; set; }

    public virtual SanPhamBienThe? BienThe { get; set; }

    public virtual SanPham? SanPham { get; set; }
}
