// MOBILE SIDEBAR
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

// FORM SUBMIT
document.getElementById("adiantamentoForm").addEventListener("submit", (e) => {
    e.preventDefault();
    alert("Adiantamento salvo com sucesso!");
});

// VALUE MASK (simples)
document.getElementById("valor").addEventListener("input", function () {
    let v = this.value.replace(/\D/g, "");
    v = (v / 100).toFixed(2) + "";
    v = v.replace(".", ",");
    this.value = "R$ " + v;
});
