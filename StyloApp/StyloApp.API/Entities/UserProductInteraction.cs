using System;
using System.Collections.Generic;

namespace StyloApp.API.Entities;

public partial class UserProductInteraction
{
    public long InteractionId { get; set; }

    public int KhachHangId { get; set; }

    public int SanPhamId { get; set; }

    public int? BienTheId { get; set; }

    public string EventType { get; set; } = null!;

    public int? ViewTimeSec { get; set; }

    public int? Quantity { get; set; }

    public string? Channel { get; set; }

    public string? Referrer { get; set; }

    public string? Device { get; set; }

    public string? SessionId { get; set; }

    public DateTime OccurredAt { get; set; }

    public virtual SanPhamBienThe? BienThe { get; set; }

    public virtual KhachHang KhachHang { get; set; } = null!;

    public virtual SanPham SanPham { get; set; } = null!;
}
