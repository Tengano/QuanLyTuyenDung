using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrangChu.Models
{
    [Table("QuaTrinhPhongVan")]
    public class QuaTrinhPhongVan
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Hồ sơ ứng tuyển là bắt buộc")]
        [Display(Name = "Hồ sơ ứng tuyển")]
        public int HoSoUngTuyenId { get; set; }
        
        [ForeignKey("HoSoUngTuyenId")]
        public HoSoUngTuyen? HoSoUngTuyen { get; set; }
        
        [Required(ErrorMessage = "Ngày phỏng vấn là bắt buộc")]
        [Display(Name = "Ngày phỏng vấn")]
        [DataType(DataType.DateTime)]
        public DateTime NgayPhongVan { get; set; }
        
        [Display(Name = "Giờ phỏng vấn")]
        [DataType(DataType.Time)]
        public TimeSpan? GioPhongVan { get; set; }
        
        [Display(Name = "Địa điểm")]
        [StringLength(500, ErrorMessage = "Địa điểm không được vượt quá 500 ký tự")]
        public string? DiaDiem { get; set; }
        
        [Display(Name = "Người phỏng vấn")]
        [StringLength(100, ErrorMessage = "Người phỏng vấn không được vượt quá 100 ký tự")]
        public string? NguoiPhongVan { get; set; }
        
        [Display(Name = "Hình thức phỏng vấn")]
        [StringLength(50, ErrorMessage = "Hình thức phỏng vấn không được vượt quá 50 ký tự")]
        public string? HinhThuc { get; set; }
        
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }
        
        [Display(Name = "Trạng thái")]
        [StringLength(50, ErrorMessage = "Trạng thái không được vượt quá 50 ký tự")]
        public string? TrangThai { get; set; } = "Đã lên lịch";
        
        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; }
        
        [Display(Name = "Ngày cập nhật")]
        public DateTime? NgayCapNhat { get; set; }
        
        public KetQuaPhongVan? KetQuaPhongVan { get; set; }
    }
}

