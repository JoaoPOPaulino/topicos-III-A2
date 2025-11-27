using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 

namespace A2.Migrations
{
    public partial class DataSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CategoriasDespesa",
                columns: new[] { "Id", "Ativo", "Descricao", "Nome" },
                values: new object[,]
                {
                    { 1, true, "Despesas com locomoção", "Transporte" },
                    { 2, true, "Despesas com refeições", "Alimentação" },
                    { 3, true, "Despesas com estadias", "Hospedagem" },
                    { 4, true, "Material de Escritório", "Material" },
                    { 5, true, "Outras despesas gerais", "Outros" }
                });

            migrationBuilder.InsertData(
                table: "Departamentos",
                columns: new[] { "Id", "Ativo", "CentroDeCusto", "EmpresaId", "GestorId", "Nome" },
                values: new object[] { 1, true, "CC001", null, null, "Recursos Humanos" });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Ativo", "Cpf", "CriadoEm", "DepartamentoId", "Email", "NomeCompleto", "Perfil" },
                values: new object[,]
                {
                    { 1, true, "00000000000", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "admin@finops.com", "Admin FinOps (RH)", 2 },
                    { 2, true, "11111111111", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "lucas@finops.com", "Lucas Henderson", 1 },
                    { 3, true, "22222222222", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "ana@finops.com", "Ana Costa", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CategoriasDespesa",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CategoriasDespesa",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CategoriasDespesa",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CategoriasDespesa",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "CategoriasDespesa",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Departamentos",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
