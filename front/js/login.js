// Elementos
const form = document.getElementById("loginForm");
const email = document.getElementById("email");
const password = document.getElementById("password");

const emailError = document.getElementById("emailError");
const passwordError = document.getElementById("passwordError");

const togglePassword = document.getElementById("togglePassword");

// ======= Mostrar/Ocultar Senha =======
togglePassword.addEventListener("click", () => {
    const type = password.getAttribute("type") === "password" ? "text" : "password";
    password.setAttribute("type", type);

    // Troca o ícone
    togglePassword.textContent = type === "password" ? "👁️" : "❌";
});

// ======= Validação do formulário =======
form.addEventListener("submit", (event) => {
    event.preventDefault();

    let valid = true;

    emailError.style.display = "none";
    passwordError.style.display = "none";

    // E-mail
    if (!email.value.includes("@") || !email.value.includes(".")) {
        emailError.textContent = "Digite um e-mail válido.";
        emailError.style.display = "block";
        valid = false;
    }

    // Senha
    if (password.value.length < 4) {
        passwordError.textContent = "A senha deve ter no mínimo 4 caracteres.";
        passwordError.style.display = "block";
        valid = false;
    }

    if (valid) {
        alert("Login realizado com sucesso (frontend)!");
        console.log("ENVIANDO (futuro backend):", {
            email: email.value,
            password: password.value
        });
    }
});
