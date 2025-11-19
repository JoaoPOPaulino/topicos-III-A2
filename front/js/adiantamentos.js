// ---------- SIDEBAR MOBILE ----------
const sidebar = document.getElementById("sidebar");
const menuToggle = document.getElementById("menuToggle");

menuToggle.addEventListener("click", () => {
    sidebar.classList.toggle("show");
});

// ---------- DROPDOWN ----------
const avatarBtn = document.getElementById("avatarBtn");
const profileMenu = document.getElementById("profileMenu");

avatarBtn.addEventListener("click", (e) => {
    e.stopPropagation();
    profileMenu.style.display =
        profileMenu.style.display === "flex" ? "none" : "flex";
});

document.addEventListener("click", () => {
    profileMenu.style.display = "none";
});

// -------------- DADOS MOCK --------------
const dados = [
    { id: 1001, nome: "Lucas Henderson", desc: "Viagem Brasília", valor: "R$ 850,00", moeda: "BRL", data: "2025-01-31", status: "pendente" },
    { id: 1002, nome: "Ana Costa", desc: "Compra materiais", valor: "$ 320.00", moeda: "USD", data: "2025-02-02", status: "revisão" },
    { id: 1003, nome: "Carlos Silva", desc: "Alimentação", valor: "R$ 150,00", moeda: "BRL", data: "2025-02-02", status: "aprovado" },
    { id: 1004, nome: "Mariana Torres", desc: "Hotel", valor: "€ 210,00", moeda: "EUR", data: "2025-01-28", status: "pago" },
    { id: 1005, nome: "João Paulo", desc: "Taxi", valor: "R$ 45,00", moeda: "BRL", data: "2025-02-04", status: "pendente" },

    { id: 1006, nome: "Eduardo Melo", desc: "Reunião SP", valor: "€ 120,00", moeda: "EUR", data: "2025-02-01", status: "aprovado" },
    { id: 1007, nome: "Bianca Souza", desc: "Uber", valor: "R$ 40,00", moeda: "BRL", data: "2025-02-03", status: "revisão" },
    { id: 1008, nome: "Ricardo Lima", desc: "Hospedagem", valor: "$ 200.00", moeda: "USD", data: "2025-01-24", status: "pago" },
    { id: 1009, nome: "Juliana Prado", desc: "Material Escritório", valor: "R$ 95,00", moeda: "BRL", data: "2025-02-05", status: "pendente" },
    { id: 1010, nome: "Ana Souza", desc: "Passagem Aérea", valor: "$ 540.00", moeda: "USD", data: "2025-02-05", status: "aprovado" },

    { id: 1011, nome: "Thiago Ramos", desc: "Combustível", valor: "R$ 200,00", moeda: "BRL", data: "2025-02-02", status: "revisão" },
    { id: 1012, nome: "Cristina Alves", desc: "Hospedagem", valor: "R$ 310,00", moeda: "BRL", data: "2025-01-29", status: "pago" },
    { id: 1013, nome: "Diego Santos", desc: "Almoço equipe", valor: "R$ 180,00", moeda: "BRL", data: "2025-02-04", status: "pendente" },
    { id: 1014, nome: "Larissa Fonseca", desc: "Transporte", valor: "$ 64.00", moeda: "USD", data: "2025-02-03", status: "aprovado" },
    { id: 1015, nome: "Renato Silva", desc: "Alimentação", valor: "€ 48,00", moeda: "EUR", data: "2025-02-05", status: "revisão" },

    { id: 1016, nome: "Marcos Tavares", desc: "Uber", valor: "R$ 32,00", moeda: "BRL", data: "2025-02-05", status: "pendente" },
    { id: 1017, nome: "Alice Martins", desc: "Passagem", valor: "$ 233.00", moeda: "USD", data: "2025-02-02", status: "pago" },
    { id: 1018, nome: "Felipe Rocha", desc: "Taxi", valor: "R$ 22,00", moeda: "BRL", data: "2025-02-04", status: "pendente" },
    { id: 1019, nome: "Heloísa Cruz", desc: "Hotel", valor: "€ 135,00", moeda: "EUR", data: "2025-01-28", status: "aprovado" },
    { id: 1020, nome: "Bruna Ferreira", desc: "Material", valor: "R$ 80,00", moeda: "BRL", data: "2025-02-01", status: "revisão" },
];

// -------------- PAGINAÇÃO --------------
let paginaAtual = 1;
const itensPorPagina = 10;

const tableBody = document.getElementById("tableBody");
const pagination = document.getElementById("pagination");

// -------------- FILTROS --------------
const searchInput = document.getElementById("searchInput");
const statusFilter = document.getElementById("statusFilter");
const dataInicial = document.getElementById("dataInicial");
const dataFinal = document.getElementById("dataFinal");

function aplicarFiltros() {
    const busca = searchInput.value.toLowerCase();
    const statusSel = statusFilter.value;
    const inicio = dataInicial.value ? new Date(dataInicial.value) : null;
    const fim = dataFinal.value ? new Date(dataFinal.value) : null;

    return dados.filter(item => {
        let texto = `${item.nome} ${item.desc} ${item.valor} ${item.status}`.toLowerCase();
        if (!texto.includes(busca)) return false;

        if (statusSel && item.status !== statusSel) return false;

        const dataItem = new Date(item.data);
        if (inicio && dataItem < inicio) return false;
        if (fim && dataItem > fim) return false;

        return true;
    });
}

function renderTabela() {
    const filtrados = aplicarFiltros();

    const inicio = (paginaAtual - 1) * itensPorPagina;
    const fim = inicio + itensPorPagina;
    const paginados = filtrados.slice(inicio, fim);

    tableBody.innerHTML = paginados.map(item => `
        <tr>
            <td>${item.id}</td>
            <td>${item.nome}</td>
            <td>${item.desc}</td>
            <td>${item.valor}</td>
            <td>${item.moeda}</td>
            <td>${new Date(item.data).toLocaleDateString("pt-BR")}</td>
            <td><span class="tag ${item.status}">${item.status}</span></td>
            <td><button class="btn-view" onclick="verAdiantamento()">Ver</button></td>
        </tr>
    `).join("");

    renderPaginacao(filtrados.length);
}

function renderPaginacao(totalItens) {
    const totalPaginas = Math.ceil(totalItens / itensPorPagina);

    pagination.innerHTML = "";

    // Anterior
    const btnPrev = document.createElement("button");
    btnPrev.textContent = "<";
    btnPrev.classList.add("arrow-btn");
    btnPrev.disabled = paginaAtual === 1;
    btnPrev.addEventListener("click", () => {
        paginaAtual--;
        renderTabela();
    });
    pagination.appendChild(btnPrev);

    // Número da página
    const pageNum = document.createElement("div");
    pageNum.textContent = paginaAtual;
    pageNum.classList.add("page-number");
    pagination.appendChild(pageNum);

    // Próximo
    const btnNext = document.createElement("button");
    btnNext.textContent = ">";
    btnNext.classList.add("arrow-btn");
    btnNext.disabled = paginaAtual === totalPaginas;
    btnNext.addEventListener("click", () => {
        paginaAtual++;
        renderTabela();
    });
    pagination.appendChild(btnNext);
}

// Eventos dos filtros
[searchInput, statusFilter, dataInicial, dataFinal].forEach(el => {
    el.addEventListener("input", () => {
        paginaAtual = 1;
        renderTabela();
    });
});

// ---------- BOTÃO NOVO ----------
document.getElementById("novoAdiantamentoBtn").addEventListener("click", () => {
    window.location.href = "../html/novo-adiantamento.html";
});

// ---------- BOTÃO VER ADIANTAMENTO ----------
function verAdiantamento() {
  window.location.href = "../html/ver-adiantamento.html";
}

// ---------- INICIA ----------
renderTabela();
