using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace A2.Migrations
{
    /// <inheritdoc />
    public partial class AddCommit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Despesas_Fornecedores_FornecedorId",
                table: "Despesas");

            migrationBuilder.DropForeignKey(
                name: "FK_LogsCotacao_Moedas_MoedaBaseId",
                table: "LogsCotacao");

            migrationBuilder.DropForeignKey(
                name: "FK_LogsCotacao_Moedas_MoedaCotadaId",
                table: "LogsCotacao");

            migrationBuilder.DropTable(
                name: "AprovacoesAdiantamento");

            migrationBuilder.DropTable(
                name: "AprovacoesPrestacao");

            migrationBuilder.DropTable(
                name: "Feriados");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LogsCotacao",
                table: "LogsCotacao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Fornecedores",
                table: "Fornecedores");

            migrationBuilder.RenameTable(
                name: "LogsCotacao",
                newName: "LogCotacao");

            migrationBuilder.RenameTable(
                name: "Fornecedores",
                newName: "Fornecedor");

            migrationBuilder.RenameIndex(
                name: "IX_LogsCotacao_MoedaCotadaId",
                table: "LogCotacao",
                newName: "IX_LogCotacao_MoedaCotadaId");

            migrationBuilder.RenameIndex(
                name: "IX_LogsCotacao_MoedaBaseId",
                table: "LogCotacao",
                newName: "IX_LogCotacao_MoedaBaseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LogCotacao",
                table: "LogCotacao",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fornecedor",
                table: "Fornecedor",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Despesas_Fornecedor_FornecedorId",
                table: "Despesas",
                column: "FornecedorId",
                principalTable: "Fornecedor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LogCotacao_Moedas_MoedaBaseId",
                table: "LogCotacao",
                column: "MoedaBaseId",
                principalTable: "Moedas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LogCotacao_Moedas_MoedaCotadaId",
                table: "LogCotacao",
                column: "MoedaCotadaId",
                principalTable: "Moedas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Despesas_Fornecedor_FornecedorId",
                table: "Despesas");

            migrationBuilder.DropForeignKey(
                name: "FK_LogCotacao_Moedas_MoedaBaseId",
                table: "LogCotacao");

            migrationBuilder.DropForeignKey(
                name: "FK_LogCotacao_Moedas_MoedaCotadaId",
                table: "LogCotacao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LogCotacao",
                table: "LogCotacao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Fornecedor",
                table: "Fornecedor");

            migrationBuilder.RenameTable(
                name: "LogCotacao",
                newName: "LogsCotacao");

            migrationBuilder.RenameTable(
                name: "Fornecedor",
                newName: "Fornecedores");

            migrationBuilder.RenameIndex(
                name: "IX_LogCotacao_MoedaCotadaId",
                table: "LogsCotacao",
                newName: "IX_LogsCotacao_MoedaCotadaId");

            migrationBuilder.RenameIndex(
                name: "IX_LogCotacao_MoedaBaseId",
                table: "LogsCotacao",
                newName: "IX_LogsCotacao_MoedaBaseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LogsCotacao",
                table: "LogsCotacao",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fornecedores",
                table: "Fornecedores",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AprovacoesAdiantamento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AprovadorId = table.Column<int>(type: "int", nullable: false),
                    SolicitacaoAdiantamentoId = table.Column<int>(type: "int", nullable: false),
                    Aprovado = table.Column<bool>(type: "bit", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataAprovacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AprovacoesAdiantamento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AprovacoesAdiantamento_SolicitacoesAdiantamento_SolicitacaoAdiantamentoId",
                        column: x => x.SolicitacaoAdiantamentoId,
                        principalTable: "SolicitacoesAdiantamento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AprovacoesAdiantamento_Usuarios_AprovadorId",
                        column: x => x.AprovadorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AprovacoesPrestacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AprovadorId = table.Column<int>(type: "int", nullable: false),
                    PrestacaoContasId = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataAprovacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
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
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Uf = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feriados", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AprovacoesAdiantamento_AprovadorId",
                table: "AprovacoesAdiantamento",
                column: "AprovadorId");

            migrationBuilder.CreateIndex(
                name: "IX_AprovacoesAdiantamento_SolicitacaoAdiantamentoId",
                table: "AprovacoesAdiantamento",
                column: "SolicitacaoAdiantamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_AprovacoesPrestacao_AprovadorId",
                table: "AprovacoesPrestacao",
                column: "AprovadorId");

            migrationBuilder.CreateIndex(
                name: "IX_AprovacoesPrestacao_PrestacaoContasId",
                table: "AprovacoesPrestacao",
                column: "PrestacaoContasId");

            migrationBuilder.AddForeignKey(
                name: "FK_Despesas_Fornecedores_FornecedorId",
                table: "Despesas",
                column: "FornecedorId",
                principalTable: "Fornecedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LogsCotacao_Moedas_MoedaBaseId",
                table: "LogsCotacao",
                column: "MoedaBaseId",
                principalTable: "Moedas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LogsCotacao_Moedas_MoedaCotadaId",
                table: "LogsCotacao",
                column: "MoedaCotadaId",
                principalTable: "Moedas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
