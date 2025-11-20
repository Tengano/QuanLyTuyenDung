using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrangChu.Migrations
{
    public partial class LinkTaiKhoanToNhaTuyenDung : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NhaTuyenDungId",
                table: "TaiKhoan",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_NhaTuyenDungId",
                table: "TaiKhoan",
                column: "NhaTuyenDungId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaiKhoan_NhaTuyenDung_NhaTuyenDungId",
                table: "TaiKhoan",
                column: "NhaTuyenDungId",
                principalTable: "NhaTuyenDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaiKhoan_NhaTuyenDung_NhaTuyenDungId",
                table: "TaiKhoan");

            migrationBuilder.DropIndex(
                name: "IX_TaiKhoan_NhaTuyenDungId",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "NhaTuyenDungId",
                table: "TaiKhoan");
        }
    }
}
