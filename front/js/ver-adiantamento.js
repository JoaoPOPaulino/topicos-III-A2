// SIDEBAR MOBILE
const sidebar = document.getElementById("sidebar");
const menuToggle = document.getElementById("menuToggle");

menuToggle?.addEventListener("click", () => {
    sidebar.classList.toggle("show");
});

// PROFILE DROPDOWN
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

// EXEMPLO DE DADOS (virá da API no futuro)
const dadosExemplo = {
    id: 1001,
    solicitante: "Lucas Henderson",
    descricao: "Viagem Brasília",
    data: "2025-01-30",
    categoria: "Transporte",
    valor: "R$ 850,00",
    moeda: "BRL",
    ptax: "5.12",
    status: "pendente",
    anexos: ["Recibo1.pdf", "NotaHotel.png"]
};

// PREENCHER DADOS
document.getElementById("det-id").textContent = dadosExemplo.id;
document.getElementById("det-solicitante").textContent = dadosExemplo.solicitante;
document.getElementById("det-descricao").textContent = dadosExemplo.descricao;
document.getElementById("det-categoria").textContent = dadosExemplo.categoria;
document.getElementById("det-valor").textContent = dadosExemplo.valor;
document.getElementById("det-moeda").textContent = dadosExemplo.moeda;
document.getElementById("det-ptax").textContent = dadosExemplo.ptax;

document.getElementById("det-data").textContent =
    new Date(dadosExemplo.data).toLocaleDateString("pt-BR");

// STATUS
const st = document.getElementById("det-status");
st.textContent = dadosExemplo.status;
st.classList.add(dadosExemplo.status);

// MOSTRAR BOTÃO EDITAR SE STATUS = pendente OU revisão
if (dadosExemplo.status === "pendente" || dadosExemplo.status === "revisão") {
    document.getElementById("editBtn").style.display = "inline-block";
}

// ANEXOS COMO BOTÕES DE DOWNLOAD
const anexosArea = document.getElementById("anexosArea");

dadosExemplo.anexos.forEach(nome => {
    const link = document.createElement("a");
    link.href = "../arquivos/" + nome; // caminho futuro
    link.download = nome;
    link.textContent = nome;
    anexosArea.appendChild(link);
});

// AÇÃO DO BOTÃO EDITAR
document.getElementById("editBtn").addEventListener("click", () => {
    window.location.href = "editar-adiantamento.html?id=" + dadosExemplo.id;
});
