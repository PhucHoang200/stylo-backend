using System;
using System.Collections.Generic;

namespace StyloApp.API.Entities;

public partial class MauSac
{
    public int MauId { get; set; }

    public string Ten { get; set; } = null!;

    public string? MaHex { get; set; }

    public virtual ICollection<SanPhamBienThe> SanPhamBienThes { get; set; } = new List<SanPhamBienThe>();
}
