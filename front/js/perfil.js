// perfil.js - controles do perfil (sidebar, dropdown, exibir/ocultar senha, validação)

// Sidebar toggle (mobile)
const sidebar = document.getElementById("sidebar");
const menuToggle = document.getElementById("menuToggle");
if (menuToggle) {
    menuToggle.addEventListener("click", () => {
        sidebar.classList.toggle("show");
    });
}

// Dropdown do perfil
const avatarBtn = document.getElementById("avatarBtn");
const profileMenu = document.getElementById("profileMenu");
if (avatarBtn) {
    avatarBtn.addEventListener("click", (e) => {
        e.stopPropagation();
        const isVisible = profileMenu.style.display === "flex";
        profileMenu.style.display = isVisible ? "none" : "flex";
        profileMenu.setAttribute('aria-hidden', isVisible ? 'true' : 'false');
    });
}

// fechar dropdown ao clicar fora
document.addEventListener("click", (event) => {
    if (profileMenu && avatarBtn) {
        if (!avatarBtn.contains(event.target) && !profileMenu.contains(event.target)) {
            profileMenu.style.display = "none";
            profileMenu.setAttribute('aria-hidden', 'true');
        }
    }
});

// Mostrar / Ocultar senhas (pega botões .eye-icon que têm data-target)
document.querySelectorAll('.eye-icon').forEach(btn => {
    btn.addEventListener('click', (e) => {
        e.preventDefault();
        const targetId = btn.getAttribute('data-target');
        if (!targetId) return;
        const input = document.getElementById(targetId);
        if (!input) return;

        const isPassword = input.type === 'password';
        input.type = isPassword ? 'text' : 'password';

        // troca do símbolo - usar ícones simples (pode substituir por svg se quiser)
        btn.textContent = isPassword ? '❌' : '👁️';
    });
});

// Form submit (frontend validation)
const profileForm = document.getElementById('profileForm');
if (profileForm) {
    profileForm.addEventListener('submit', (e) => {
        e.preventDefault();

        const current = document.getElementById('currentPassword').value.trim();
        const newPass = document.getElementById('newPassword').value.trim();
        const confirm = document.getElementById('confirmPassword').value.trim();

        // If user tries to change password, validate the three fields
        if (current !== '' || newPass !== '' || confirm !== '') {
            if (current === '') {
                alert('Informe sua senha atual para alterá-la.');
                return;
            }
            if (newPass.length < 4) {
                alert('A nova senha deve ter pelo menos 4 caracteres.');
                return;
            }
            if (newPass !== confirm) {
                alert('A nova senha e a confirmação não coincidem.');
                return;
            }

            // Here you'd call backend API to validate current password and replace it
            // For now, simulate success:
            alert('Senha alterada com sucesso (simulado).');
        }

        // Save other profile fields (simulated)
        alert('Dados do perfil atualizados (simulado).');
    });
}
