using System;
using System.Collections.Generic;

namespace StyloApp.API.Entities;

public partial class TonKho
{
    public int BienTheId { get; set; }

    public int OnHand { get; set; }

    public int Reserved { get; set; }

    public decimal? AvgCost { get; set; }

    public int? Available { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual SanPhamBienThe BienThe { get; set; } = null!;
}
