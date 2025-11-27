using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace A2.Migrations
{
    /// <inheritdoc />
    public partial class Att : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AprovacoesPrestacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrestacaoContasId = table.Column<int>(type: "int", nullable: false),
                    AprovadorId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataAprovacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AprovacoesPrestacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AprovacoesPrestacao_PrestacoesContas_PrestacaoContasId",
                        column: x => x.PrestacaoContasId,
                        principalTable: "PrestacoesContas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AprovacoesPrestacao_Usuarios_AprovadorId",
                        column: x => x.AprovadorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Feriados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Uf = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feriados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogsAuditoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    TipoEntidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntidadeId = table.Column<int>(type: "int", nullable: false),
                    Acao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DadosAnteriores = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DadosPosteriores = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogsAuditoria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogsAuditoria_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LogsCotacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MoedaBaseId = table.Column<int>(type: "int", nullable: false),
                    MoedaCotadaId = table.Column<int>(type: "int", nullable: false),
                    Taxa = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    DataReferencia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CapturadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogsCotacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogsCotacao_Moedas_MoedaBaseId",
                        column: x => x.MoedaBaseId,
                        principalTable: "Moedas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LogsCotacao_Moedas_MoedaCotadaId",
                        column: x => x.MoedaCotadaId,
                        principalTable: "Moedas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AprovacoesPrestacao_AprovadorId",
                table: "AprovacoesPrestacao",
                column: "AprovadorId");

            migrationBuilder.CreateIndex(
                name: "IX_AprovacoesPrestacao_PrestacaoContasId",
                table: "AprovacoesPrestacao",
                column: "PrestacaoContasId");

            migrationBuilder.CreateIndex(
                name: "IX_LogsAuditoria_UsuarioId",
                table: "LogsAuditoria",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_LogsCotacao_MoedaBaseId",
                table: "LogsCotacao",
                column: "MoedaBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_LogsCotacao_MoedaCotadaId",
                table: "LogsCotacao",
                column: "MoedaCotadaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AprovacoesPrestacao");

            migrationBuilder.DropTable(
                name: "Feriados");

            migrationBuilder.DropTable(
                name: "LogsAuditoria");

            migrationBuilder.DropTable(
                name: "LogsCotacao");
        }
    }
}
