using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StyloApp.API.Entities
{
    public class DiaChi
    {
        public int DiaChiId { get; set; }

        public int TaiKhoanId { get; set; }

        public string? DiaChiChiTiet { get; set; }

        public string? LoaiDiaChi { get; set; }

        public bool? IsDefault { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public virtual TaiKhoan TaiKhoan { get; set; } = null!;
    }
}
