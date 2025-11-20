using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrangChu.Migrations
{
    public partial class AddRecruitmentTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NhaTuyenDung",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenCongTy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaSoThue = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuyMo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LinhVuc = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaTuyenDung", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TinTuyenDung",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TieuDe = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MoTaCongViec = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YeuCau = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuyenLoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MucLuong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DiaDiemLamViec = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    KinhNghiem = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HocVan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SoLuongTuyen = table.Column<int>(type: "int", nullable: true),
                    HanNopHoSo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NhaTuyenDungId = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinTuyenDung", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TinTuyenDung_NhaTuyenDung_NhaTuyenDungId",
                        column: x => x.NhaTuyenDungId,
                        principalTable: "NhaTuyenDung",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HoSoUngTuyen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TinTuyenDungId = table.Column<int>(type: "int", nullable: false),
                    TaiKhoanId = table.Column<int>(type: "int", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CvUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ThuXinViec = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayNop = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoUngTuyen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoSoUngTuyen_TaiKhoan_TaiKhoanId",
                        column: x => x.TaiKhoanId,
                        principalTable: "TaiKhoan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HoSoUngTuyen_TinTuyenDung_TinTuyenDungId",
                        column: x => x.TinTuyenDungId,
                        principalTable: "TinTuyenDung",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuaTrinhPhongVan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoSoUngTuyenId = table.Column<int>(type: "int", nullable: false),
                    NgayPhongVan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioPhongVan = table.Column<TimeSpan>(type: "time", nullable: true),
                    DiaDiem = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NguoiPhongVan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HinhThuc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuaTrinhPhongVan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuaTrinhPhongVan_HoSoUngTuyen_HoSoUngTuyenId",
                        column: x => x.HoSoUngTuyenId,
                        principalTable: "HoSoUngTuyen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KetQuaPhongVan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuaTrinhPhongVanId = table.Column<int>(type: "int", nullable: false),
                    KetQua = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DiemSo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DanhGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiemManh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiemYeu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KhuyenNghi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NguoiDanhGia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgayDanhGia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KetQuaPhongVan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KetQuaPhongVan_QuaTrinhPhongVan_QuaTrinhPhongVanId",
                        column: x => x.QuaTrinhPhongVanId,
                        principalTable: "QuaTrinhPhongVan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TinTuyenDung_NhaTuyenDungId",
                table: "TinTuyenDung",
                column: "NhaTuyenDungId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoUngTuyen_TaiKhoanId",
                table: "HoSoUngTuyen",
                column: "TaiKhoanId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoUngTuyen_TinTuyenDungId",
                table: "HoSoUngTuyen",
                column: "TinTuyenDungId");

            migrationBuilder.CreateIndex(
                name: "IX_QuaTrinhPhongVan_HoSoUngTuyenId",
                table: "QuaTrinhPhongVan",
                column: "HoSoUngTuyenId");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaPhongVan_QuaTrinhPhongVanId",
                table: "KetQuaPhongVan",
                column: "QuaTrinhPhongVanId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KetQuaPhongVan");

            migrationBuilder.DropTable(
                name: "QuaTrinhPhongVan");

            migrationBuilder.DropTable(
                name: "HoSoUngTuyen");

            migrationBuilder.DropTable(
                name: "TinTuyenDung");

            migrationBuilder.DropTable(
                name: "NhaTuyenDung");
        }
    }
}
