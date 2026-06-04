using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SentinelApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_SEN_REGIAO",
                columns: table => new
                {
                    ID_REGIAO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_REGIAO = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    NM_ESTADO = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    NM_PAIS = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    RE_LATITUDE = table.Column<decimal>(type: "DECIMAL(10,6)", precision: 10, scale: 6, nullable: false),
                    RE_LONGITUDE = table.Column<decimal>(type: "DECIMAL(10,6)", precision: 10, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_SEN_REGIAO", x => x.ID_REGIAO);
                });

            migrationBuilder.CreateTable(
                name: "T_SEN_USUARIO",
                columns: table => new
                {
                    ID_USUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    SENHA_HASH = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    FCM_TOKEN = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    UID_FIREBASE = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: true),
                    LATITUDE = table.Column<decimal>(type: "DECIMAL(10,6)", precision: 10, scale: 6, nullable: true),
                    LONGITUDE = table.Column<decimal>(type: "DECIMAL(10,6)", precision: 10, scale: 6, nullable: true),
                    RAIO_KM = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DATA_CADASTRO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ATIVO = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_SEN_USUARIO", x => x.ID_USUARIO);
                });

            migrationBuilder.CreateTable(
                name: "T_SEN_USUARIO_REGIAO",
                columns: table => new
                {
                    T_SEN_USUARIO_ID_USUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    T_SEN_REGIAO_ID_REGIAO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DATA_INSCRICAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    US_RE_ATIVO = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_SEN_USUARIO_REGIAO", x => new { x.T_SEN_USUARIO_ID_USUARIO, x.T_SEN_REGIAO_ID_REGIAO });
                    table.ForeignKey(
                        name: "FK_T_SEN_USUARIO_REGIAO_T_SEN_REGIAO_T_SEN_REGIAO_ID_REGIAO",
                        column: x => x.T_SEN_REGIAO_ID_REGIAO,
                        principalTable: "T_SEN_REGIAO",
                        principalColumn: "ID_REGIAO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_SEN_USUARIO_REGIAO_T_SEN_USUARIO_T_SEN_USUARIO_ID_USUARIO",
                        column: x => x.T_SEN_USUARIO_ID_USUARIO,
                        principalTable: "T_SEN_USUARIO",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_SEN_USUARIO_REGIAO_T_SEN_REGIAO_ID_REGIAO",
                table: "T_SEN_USUARIO_REGIAO",
                column: "T_SEN_REGIAO_ID_REGIAO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_SEN_USUARIO_REGIAO");

            migrationBuilder.DropTable(
                name: "T_SEN_REGIAO");

            migrationBuilder.DropTable(
                name: "T_SEN_USUARIO");
        }
    }
}
