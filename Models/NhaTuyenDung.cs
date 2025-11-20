using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrangChu.Models
{
    [Table("NhaTuyenDung")]
    public class NhaTuyenDung
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Tên công ty là bắt buộc")]
        [Display(Name = "Tên công ty")]
        [StringLength(200, ErrorMessage = "Tên công ty không được vượt quá 200 ký tự")]
        public string TenCongTy { get; set; } = string.Empty;
        
        [Display(Name = "Mã số thuế")]
        [StringLength(20, ErrorMessage = "Mã số thuế không được vượt quá 20 ký tự")]
        public string? MaSoThue { get; set; }
        
        [Display(Name = "Địa chỉ")]
        [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự")]
        public string? DiaChi { get; set; }
        
        [Display(Name = "Số điện thoại")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string? SoDienThoai { get; set; }
        
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string? Email { get; set; }
        
        [Display(Name = "Website")]
        [StringLength(200, ErrorMessage = "Website không được vượt quá 200 ký tự")]
        public string? Website { get; set; }
        
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }
        
        [Display(Name = "Quy mô công ty")]
        [StringLength(50, ErrorMessage = "Quy mô công ty không được vượt quá 50 ký tự")]
        public string? QuyMo { get; set; }
        
        [Display(Name = "Lĩnh vực hoạt động")]
        [StringLength(200, ErrorMessage = "Lĩnh vực hoạt động không được vượt quá 200 ký tự")]
        public string? LinhVuc { get; set; }
        
        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; }
        
        [Display(Name = "Ngày cập nhật")]
        public DateTime? NgayCapNhat { get; set; }
        
        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; } = true;
        
        public ICollection<TinTuyenDung>? TinTuyenDungs { get; set; }
    }
}

