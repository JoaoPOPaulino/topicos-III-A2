using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace A2.Migrations
{
    /// <inheritdoc />
    public partial class LogCotacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogCotacao_Moedas_MoedaBaseId",
                table: "LogCotacao");

            migrationBuilder.DropForeignKey(
                name: "FK_LogCotacao_Moedas_MoedaCotadaId",
                table: "LogCotacao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LogCotacao",
                table: "LogCotacao");

            migrationBuilder.RenameTable(
                name: "LogCotacao",
                newName: "LogsCotacao");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogsCotacao_Moedas_MoedaBaseId",
                table: "LogsCotacao");

            migrationBuilder.DropForeignKey(
                name: "FK_LogsCotacao_Moedas_MoedaCotadaId",
                table: "LogsCotacao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LogsCotacao",
                table: "LogsCotacao");

            migrationBuilder.RenameTable(
                name: "LogsCotacao",
                newName: "LogCotacao");

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
    }
}
