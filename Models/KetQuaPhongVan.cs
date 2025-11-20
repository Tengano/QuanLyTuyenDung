using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrangChu.Models
{
    [Table("KetQuaPhongVan")]
    public class KetQuaPhongVan
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Quá trình phỏng vấn là bắt buộc")]
        [Display(Name = "Quá trình phỏng vấn")]
        public int QuaTrinhPhongVanId { get; set; }
        
        [ForeignKey("QuaTrinhPhongVanId")]
        public QuaTrinhPhongVan? QuaTrinhPhongVan { get; set; }
        
        [Display(Name = "Kết quả")]
        [StringLength(50, ErrorMessage = "Kết quả không được vượt quá 50 ký tự")]
        public string? KetQua { get; set; } = "Chưa có kết quả";
        
        [Display(Name = "Điểm số")]
        [Range(0, 100, ErrorMessage = "Điểm số phải từ 0 đến 100")]
        public decimal? DiemSo { get; set; }
        
        [Display(Name = "Đánh giá")]
        public string? DanhGia { get; set; }
        
        [Display(Name = "Điểm mạnh")]
        public string? DiemManh { get; set; }
        
        [Display(Name = "Điểm yếu")]
        public string? DiemYeu { get; set; }
        
        [Display(Name = "Khuyến nghị")]
        public string? KhuyenNghi { get; set; }
        
        [Display(Name = "Người đánh giá")]
        [StringLength(100, ErrorMessage = "Người đánh giá không được vượt quá 100 ký tự")]
        public string? NguoiDanhGia { get; set; }
        
        [Display(Name = "Ngày đánh giá")]
        [DataType(DataType.Date)]
        public DateTime? NgayDanhGia { get; set; }
        
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }
        
        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; }
        
        [Display(Name = "Ngày cập nhật")]
        public DateTime? NgayCapNhat { get; set; }
    }
}


