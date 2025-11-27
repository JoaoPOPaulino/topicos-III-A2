using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace A2.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriasDespesa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasDespesa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cnpj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RazaoSocial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Endereco = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fornecedores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CnpjCpf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Endereco = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pais = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fornecedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Moedas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Simbolo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moedas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CentroDeCusto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GestorId = table.Column<int>(type: "int", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departamentos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartamentoId = table.Column<int>(type: "int", nullable: false),
                    NomeCompleto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cpf = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Perfil = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Departamentos_DepartamentoId",
                        column: x => x.DepartamentoId,
                        principalTable: "Departamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitacoesAdiantamento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColaboradorId = table.Column<int>(type: "int", nullable: false),
                    CriadoPorId = table.Column<int>(type: "int", nullable: false),
                    DepartamentoId = table.Column<int>(type: "int", nullable: false),
                    MoedaId = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Justificativa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataPagamentoRequerida = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataPagamentoAjustada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observacoes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitacoesAdiantamento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitacoesAdiantamento_Departamentos_DepartamentoId",
                        column: x => x.DepartamentoId,
                        principalTable: "Departamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitacoesAdiantamento_Moedas_MoedaId",
                        column: x => x.MoedaId,
                        principalTable: "Moedas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitacoesAdiantamento_Usuarios_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitacoesAdiantamento_Usuarios_CriadoPorId",
                        column: x => x.CriadoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AprovacoesAdiantamento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitacaoAdiantamentoId = table.Column<int>(type: "int", nullable: false),
                    AprovadorId = table.Column<int>(type: "int", nullable: false),
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
                name: "PrestacoesContas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitacaoAdiantamentoId = table.Column<int>(type: "int", nullable: false),
                    CriadoPorId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalDespesas = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SaldoReembolso = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SaldoDevolucao = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EnviadoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrestacoesContas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrestacoesContas_SolicitacoesAdiantamento_SolicitacaoAdiantamentoId",
                        column: x => x.SolicitacaoAdiantamentoId,
                        principalTable: "SolicitacoesAdiantamento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrestacoesContas_Usuarios_CriadoPorId",
                        column: x => x.CriadoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Despesas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrestacaoContasId = table.Column<int>(type: "int", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    FornecedorId = table.Column<int>(type: "int", nullable: true),
                    MoedaId = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataDespesa = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxaCambio = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ValorEmBrl = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Despesas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Despesas_CategoriasDespesa_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "CategoriasDespesa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Despesas_Fornecedores_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Despesas_Moedas_MoedaId",
                        column: x => x.MoedaId,
                        principalTable: "Moedas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Despesas_PrestacoesContas_PrestacaoContasId",
                        column: x => x.PrestacaoContasId,
                        principalTable: "PrestacoesContas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pagamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    SolicitacaoAdiantamentoId = table.Column<int>(type: "int", nullable: true),
                    PrestacaoContasId = table.Column<int>(type: "int", nullable: true),
                    BeneficiarioId = table.Column<int>(type: "int", nullable: false),
                    MoedaId = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataPagamentoPrevista = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataPagamentoEfetivada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessadoPorId = table.Column<int>(type: "int", nullable: true),
                    Observacoes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagamentos_Moedas_MoedaId",
                        column: x => x.MoedaId,
                        principalTable: "Moedas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pagamentos_PrestacoesContas_PrestacaoContasId",
                        column: x => x.PrestacaoContasId,
                        principalTable: "PrestacoesContas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pagamentos_SolicitacoesAdiantamento_SolicitacaoAdiantamentoId",
                        column: x => x.SolicitacaoAdiantamentoId,
                        principalTable: "SolicitacoesAdiantamento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pagamentos_Usuarios_BeneficiarioId",
                        column: x => x.BeneficiarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pagamentos_Usuarios_ProcessadoPorId",
                        column: x => x.ProcessadoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComprovantesDespesa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DespesaId = table.Column<int>(type: "int", nullable: false),
                    NomeArquivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaminhoArquivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoArquivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TamanhoArquivo = table.Column<long>(type: "bigint", nullable: false),
                    UploadEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprovantesDespesa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComprovantesDespesa_Despesas_DespesaId",
                        column: x => x.DespesaId,
                        principalTable: "Despesas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Empresas",
                columns: new[] { "Id", "Cnpj", "Endereco", "Nome", "RazaoSocial", "Telefone" },
                values: new object[] { 1, "00.000.000/0001-00", null, "FinOps Pro Ltda", "FinOps Pro Ltda", null });

            migrationBuilder.InsertData(
                table: "Moedas",
                columns: new[] { "Id", "Codigo", "Nome", "Simbolo" },
                values: new object[,]
                {
                    { 1, "BRL", "Real Brasileiro", "R$" },
                    { 2, "USD", "Dólar Americano", "$" },
                    { 3, "EUR", "Euro", "€" }
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
                name: "IX_ComprovantesDespesa_DespesaId",
                table: "ComprovantesDespesa",
                column: "DespesaId");

            migrationBuilder.CreateIndex(
                name: "IX_Departamentos_EmpresaId",
                table: "Departamentos",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Despesas_CategoriaId",
                table: "Despesas",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Despesas_FornecedorId",
                table: "Despesas",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Despesas_MoedaId",
                table: "Despesas",
                column: "MoedaId");

            migrationBuilder.CreateIndex(
                name: "IX_Despesas_PrestacaoContasId",
                table: "Despesas",
                column: "PrestacaoContasId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_BeneficiarioId",
                table: "Pagamentos",
                column: "BeneficiarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_MoedaId",
                table: "Pagamentos",
                column: "MoedaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_PrestacaoContasId",
                table: "Pagamentos",
                column: "PrestacaoContasId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_ProcessadoPorId",
                table: "Pagamentos",
                column: "ProcessadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_SolicitacaoAdiantamentoId",
                table: "Pagamentos",
                column: "SolicitacaoAdiantamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrestacoesContas_CriadoPorId",
                table: "PrestacoesContas",
                column: "CriadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_PrestacoesContas_SolicitacaoAdiantamentoId",
                table: "PrestacoesContas",
                column: "SolicitacaoAdiantamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesAdiantamento_ColaboradorId",
                table: "SolicitacoesAdiantamento",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesAdiantamento_CriadoPorId",
                table: "SolicitacoesAdiantamento",
                column: "CriadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesAdiantamento_DepartamentoId",
                table: "SolicitacoesAdiantamento",
                column: "DepartamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesAdiantamento_MoedaId",
                table: "SolicitacoesAdiantamento",
                column: "MoedaId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_DepartamentoId",
                table: "Usuarios",
                column: "DepartamentoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AprovacoesAdiantamento");

            migrationBuilder.DropTable(
                name: "ComprovantesDespesa");

            migrationBuilder.DropTable(
                name: "Pagamentos");

            migrationBuilder.DropTable(
                name: "Despesas");

            migrationBuilder.DropTable(
                name: "CategoriasDespesa");

            migrationBuilder.DropTable(
                name: "Fornecedores");

            migrationBuilder.DropTable(
                name: "PrestacoesContas");

            migrationBuilder.DropTable(
                name: "SolicitacoesAdiantamento");

            migrationBuilder.DropTable(
                name: "Moedas");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Departamentos");

            migrationBuilder.DropTable(
                name: "Empresas");
        }
    }
}
