using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudDrive.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChunkUploadSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChunkUploadSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TotalSize = table.Column<long>(type: "bigint", nullable: false),
                    ChunkSize = table.Column<long>(type: "bigint", nullable: false),
                    TotalChunks = table.Column<int>(type: "int", nullable: false),
                    UploadedChunks = table.Column<int>(type: "int", nullable: false),
                    UploadedChunkIndices = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    FileHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ParentFolderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TempDirectory = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChunkUploadSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChunkUploadSessions_ExpiresAt",
                table: "ChunkUploadSessions",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_ChunkUploadSessions_OwnerId",
                table: "ChunkUploadSessions",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ChunkUploadSessions_Status",
                table: "ChunkUploadSessions",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChunkUploadSessions");
        }
    }
}
