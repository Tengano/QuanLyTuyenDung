using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrangChu.Models
{
    [Table("TinTuyenDung")]
    public class TinTuyenDung
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Tiêu đề tin tuyển dụng là bắt buộc")]
        [Display(Name = "Tiêu đề")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string TieuDe { get; set; } = string.Empty;
        
        [Display(Name = "Mô tả công việc")]
        public string? MoTaCongViec { get; set; }
        
        [Display(Name = "Yêu cầu")]
        public string? YeuCau { get; set; }
        
        [Display(Name = "Quyền lợi")]
        public string? QuyenLoi { get; set; }
        
        [Display(Name = "Mức lương")]
        [StringLength(100, ErrorMessage = "Mức lương không được vượt quá 100 ký tự")]
        public string? MucLuong { get; set; }
        
        [Display(Name = "Địa điểm làm việc")]
        [StringLength(200, ErrorMessage = "Địa điểm làm việc không được vượt quá 200 ký tự")]
        public string? DiaDiemLamViec { get; set; }
        
        [Display(Name = "Kinh nghiệm")]
        [StringLength(50, ErrorMessage = "Kinh nghiệm không được vượt quá 50 ký tự")]
        public string? KinhNghiem { get; set; }
        
        [Display(Name = "Học vấn")]
        [StringLength(50, ErrorMessage = "Học vấn không được vượt quá 50 ký tự")]
        public string? HocVan { get; set; }
        
        [Display(Name = "Số lượng tuyển")]
        public int? SoLuongTuyen { get; set; }
        
        [Display(Name = "Hạn nộp hồ sơ")]
        [DataType(DataType.Date)]
        public DateTime? HanNopHoSo { get; set; }
        
        [Required(ErrorMessage = "Nhà tuyển dụng là bắt buộc")]
        [Display(Name = "Nhà tuyển dụng")]
        public int NhaTuyenDungId { get; set; }
        
        [ForeignKey("NhaTuyenDungId")]
        public NhaTuyenDung? NhaTuyenDung { get; set; }
        
        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; }
        
        [Display(Name = "Ngày cập nhật")]
        public DateTime? NgayCapNhat { get; set; }
        
        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; } = true;
        
        public ICollection<HoSoUngTuyen>? HoSoUngTuyens { get; set; }
    }
}

