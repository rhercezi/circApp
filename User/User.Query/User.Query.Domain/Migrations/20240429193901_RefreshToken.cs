using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace User.Query.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    userid = table.Column<string>(name: "user_id", type: "nvarchar(max)", nullable: false),
                    iat = table.Column<long>(type: "bigint", nullable: false),
                    nbf = table.Column<long>(type: "bigint", nullable: false),
                    exp = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");
        }
    }
}
