using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BasicStructure.Migrations
{
    public partial class permissionmatrix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "APIS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndPoint = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsBackend = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APIS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentityRoleId = table.Column<int>(type: "int", nullable: false),
                    APIId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_APIS_APIId",
                        column: x => x.APIId,
                        principalTable: "APIS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Permissions_AspNetRoles_IdentityRoleId",
                        column: x => x.IdentityRoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "APIS",
                columns: new[] { "Id", "EndPoint", "IsBackend", "Name" },
                values: new object[,]
                {
                    { 1, "post/api/User", true, "CreateUser" },
                    { 2, "put/api/User", true, "UpdateUser" },
                    { 3, "get/api/User", true, "GetAllUsers" },
                    { 4, "get/api/User/id", true, "GetUserById" },
                    { 5, "delete/api/User/id", true, "DeleteUserById" }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "APIId", "IdentityRoleId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 1 },
                    { 3, 3, 1 },
                    { 4, 4, 1 },
                    { 5, 5, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_APIId",
                table: "Permissions",
                column: "APIId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_IdentityRoleId",
                table: "Permissions",
                column: "IdentityRoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "APIS");
        }
    }
}
