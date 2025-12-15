using System;
using System.Collections.Generic;

namespace StyloApp.API.Entities;

public partial class PhanLoai
{
    public int PhanLoaiId { get; set; }

    public string Ten { get; set; } = null!;

    public virtual ICollection<DanhMuc> DanhMucs { get; set; } = new List<DanhMuc>();
}
