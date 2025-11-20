using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrangChu.Models
{
    [Table("TaiKhoan")]
    public class TaiKhoan
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [Display(Name = "Họ và tên")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string HoTen { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Display(Name = "Số điện thoại")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string SoDienThoai { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [Display(Name = "Mật khẩu")]
        [StringLength(255, ErrorMessage = "Mật khẩu không được vượt quá 255 ký tự")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string MatKhau { get; set; } = string.Empty;
        
        [Display(Name = "Địa chỉ")]
        [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
        public string? DiaChi { get; set; }
        
        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; }
        
        [Display(Name = "Ngày cập nhật")]
        public DateTime? NgayCapNhat { get; set; }
        
        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; } = true;
        
        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        [Display(Name = "Vai trò")]
        [StringLength(20, ErrorMessage = "Vai trò không được vượt quá 20 ký tự")]
        public string Role { get; set; } = "Candidate";
        
        [Display(Name = "Nhà tuyển dụng")]
        public int? NhaTuyenDungId { get; set; }
        
        [ForeignKey(nameof(NhaTuyenDungId))]
        public NhaTuyenDung? NhaTuyenDung { get; set; }
        
        [NotMapped]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string? XacNhanMatKhau { get; set; }
    }
}

