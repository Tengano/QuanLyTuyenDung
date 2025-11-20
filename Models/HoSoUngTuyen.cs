using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrangChu.Models
{
    [Table("HoSoUngTuyen")]
    public class HoSoUngTuyen
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Tin tuyển dụng là bắt buộc")]
        [Display(Name = "Tin tuyển dụng")]
        public int TinTuyenDungId { get; set; }
        
        [ForeignKey("TinTuyenDungId")]
        public TinTuyenDung? TinTuyenDung { get; set; }
        
        [Required(ErrorMessage = "Ứng viên là bắt buộc")]
        [Display(Name = "Ứng viên")]
        public int TaiKhoanId { get; set; }
        
        [ForeignKey("TaiKhoanId")]
        public TaiKhoan? TaiKhoan { get; set; }
        
        [Display(Name = "Họ và tên")]
        [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự")]
        public string? HoTen { get; set; }
        
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string? Email { get; set; }
        
        [Display(Name = "Số điện thoại")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string? SoDienThoai { get; set; }
        
        [Display(Name = "CV/Resume")]
        [StringLength(500, ErrorMessage = "Đường dẫn CV không được vượt quá 500 ký tự")]
        public string? CvUrl { get; set; }
        
        [Display(Name = "Thư xin việc")]
        public string? ThuXinViec { get; set; }
        
        [Display(Name = "Trạng thái")]
        [StringLength(50, ErrorMessage = "Trạng thái không được vượt quá 50 ký tự")]
        public string? TrangThai { get; set; } = "Chờ xử lý";
        
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }
        
        [Display(Name = "Ngày nộp")]
        public DateTime NgayNop { get; set; }
        
        [Display(Name = "Ngày cập nhật")]
        public DateTime? NgayCapNhat { get; set; }
        
        public ICollection<QuaTrinhPhongVan>? QuaTrinhPhongVans { get; set; }
    }
}

