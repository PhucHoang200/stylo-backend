using System;
using System.Collections.Generic;

namespace StyloApp.API.Entities;

public partial class ThuongHieu
{
    public int ThuongHieuId { get; set; }

    public string Ten { get; set; } = null!;

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
