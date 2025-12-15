using System;
using System.Collections.Generic;

namespace StyloApp.API.Entities;

public partial class DanhMuc
{
    public int DanhMucId { get; set; }

    public string Ten { get; set; } = null!;

    public int PhanLoaiId { get; set; }

    public virtual PhanLoai PhanLoai { get; set; } = null!;
}
