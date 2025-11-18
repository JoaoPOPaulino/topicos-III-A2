// Sidebar toggle
const sidebar = document.getElementById("sidebar");
const menuToggle = document.getElementById("menuToggle");

menuToggle.addEventListener("click", () => {
    sidebar.classList.toggle("show");
});

// Dropdown do perfil
const avatarBtn = document.getElementById("avatarBtn");
const profileMenu = document.getElementById("profileMenu");

avatarBtn.addEventListener("click", () => {
    profileMenu.style.display =
        profileMenu.style.display === "flex" ? "none" : "flex";
});

// Fechar dropdown ao clicar fora
document.addEventListener("click", (event) => {
    if (!avatarBtn.contains(event.target) && !profileMenu.contains(event.target)) {
        profileMenu.style.display = "none";
    }
});
