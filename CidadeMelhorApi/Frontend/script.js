const API_DENUNCIAS = "/api/Denuncias";
const API_USUARIOS = "/api/Usuarios";
const API_ADMINS = "/api/Admins";

function mostrarMensagem(texto, erro = false) {
    const mensagem = document.getElementById("mensagem");

    if (!mensagem) {
        return;
    }

    mensagem.textContent = texto;
    mensagem.className = erro ? "erro" : "sucesso";
}

function usuarioAtual() {
    const dados = localStorage.getItem("usuario");
    return dados ? JSON.parse(dados) : null;
}

function adminAtual() {
    return localStorage.getItem("admin") === "true";
}

function protegerPaginaUsuario() {
    if (!usuarioAtual()) {
        window.location.href = "login.html";
    }
}

function protegerPaginaAdmin() {
    if (!adminAtual()) {
        window.location.href = "admin-login.html";
    }
}

function sairUsuario() {
    localStorage.removeItem("usuario");
    window.location.href = "login.html";
}

function sairAdmin() {
    localStorage.removeItem("admin");
    window.location.href = "admin-login.html";
}

async function cadastrarUsuario(event) {
    event.preventDefault();

    const usuario = {
        nome: document.getElementById("nomeCadastro").value,
        email: document.getElementById("emailCadastro").value,
        senha: document.getElementById("senhaCadastro").value
    };

    try {
        const response = await fetch(API_USUARIOS, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(usuario)
        });

        if (!response.ok) {
            const erro = await response.text();
            throw new Error(erro || "Erro ao cadastrar usuario.");
        }

        mostrarMensagem("Usuario cadastrado com sucesso.");
        setTimeout(() => {
            window.location.href = "login.html";
        }, 800);
    } catch (error) {
        mostrarMensagem(error.message, true);
    }
}

async function loginUsuario(event) {
    event.preventDefault();

    const login = {
        email: document.getElementById("emailLogin").value,
        senha: document.getElementById("senhaLogin").value
    };

    try {
        const response = await fetch(`${API_USUARIOS}/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(login)
        });

        if (!response.ok) {
            throw new Error("Email ou senha invalidos.");
        }

        const usuario = await response.json();
        localStorage.setItem("usuario", JSON.stringify(usuario));
        window.location.href = "index.html";
    } catch (error) {
        mostrarMensagem(error.message, true);
    }
}

async function loginAdmin(event) {
    event.preventDefault();

    const login = {
        email: document.getElementById("emailAdmin").value,
        senha: document.getElementById("senhaAdmin").value
    };

    try {
        const response = await fetch(`${API_ADMINS}/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(login)
        });

        if (!response.ok) {
            throw new Error("Email ou senha do admin invalidos.");
        }

        localStorage.setItem("admin", "true");
        window.location.href = "admin.html";
    } catch (error) {
        mostrarMensagem(error.message, true);
    }
}

async function carregarDenuncias() {
    const lista = document.getElementById("lista");

    if (!lista) {
        return;
    }

    protegerPaginaUsuario();

    const usuario = usuarioAtual();
    const usuarioLogado = document.getElementById("usuarioLogado");

    if (usuarioLogado && usuario) {
        usuarioLogado.textContent = `Usuario logado: ${usuario.nome} (${usuario.email})`;
    }

    try {
        const response = await fetch(API_DENUNCIAS);

        if (!response.ok) {
            throw new Error("Nao foi possivel carregar as denuncias.");
        }

        const denuncias = await response.json();
        lista.innerHTML = "";

        denuncias.forEach((denuncia) => {
            lista.innerHTML += `
                <li>
                    <strong>${denuncia.categoria}</strong><br>
                    ${denuncia.descricao ?? ""}<br>
                    Endereco: ${denuncia.endereco ?? ""}<br>
                    Status: ${denuncia.status ?? "Aberto"}
                </li>
            `;
        });
    } catch (error) {
        mostrarMensagem(error.message, true);
    }
}

async function carregarAdmin() {
    const lista = document.getElementById("adminLista");

    if (!lista) {
        return;
    }

    protegerPaginaAdmin();

    const categoria = document.getElementById("filtroCategoria")?.value ?? "";
    const status = document.getElementById("filtroStatus")?.value ?? "";
    const endereco = document.getElementById("filtroEndereco")?.value ?? "";
    const parametros = new URLSearchParams();

    if (categoria) parametros.append("categoria", categoria);
    if (status) parametros.append("status", status);
    if (endereco) parametros.append("Endereco", endereco);

    const url = parametros.toString()
        ? `${API_DENUNCIAS}/filter?${parametros.toString()}`
        : API_DENUNCIAS;

    try {
        const response = await fetch(url);

        if (!response.ok) {
            throw new Error("Nao foi possivel carregar as denuncias.");
        }

        const denuncias = await response.json();
        lista.innerHTML = "";

        if (denuncias.length === 0) {
            lista.innerHTML = "<li>Nenhuma denuncia encontrada.</li>";
            return;
        }

        denuncias.forEach((denuncia) => {
            lista.innerHTML += `
                <li>
                    <strong>${denuncia.categoria}</strong><br>
                    ${denuncia.descricao ?? ""}<br>
                    Endereco: ${denuncia.endereco ?? ""}<br>
                    Status: ${denuncia.status ?? "Aberto"}<br>
                    <div class="acoes">
                        <select id="status-${denuncia.id}">
                            <option value="Aberto" ${denuncia.status === "Aberto" ? "selected" : ""}>Aberto</option>
                            <option value="Em andamento" ${denuncia.status === "Em andamento" ? "selected" : ""}>Em andamento</option>
                            <option value="Resolvido" ${denuncia.status === "Resolvido" ? "selected" : ""}>Resolvido</option>
                        </select>
                        <button type="button" onclick="alterarStatus(${denuncia.id})">Salvar status</button>
                        <button type="button" class="perigo" onclick="excluirDenuncia(${denuncia.id})">Excluir</button>
                    </div>
                </li>
            `;
        });
    } catch (error) {
        mostrarMensagem(error.message, true);
    }
}

async function criarDenuncia(event) {
    event.preventDefault();

    const usuario = usuarioAtual();

    if (!usuario) {
        window.location.href = "login.html";
        return;
    }

    const denuncia = {
        descricao: document.getElementById("descricao").value,
        categoria: document.getElementById("categoria").value,
        endereco: document.getElementById("endereco").value,
        usuarioId: usuario.id,
        status: "Aberto"
    };

    try {
        const response = await fetch(API_DENUNCIAS, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(denuncia)
        });

        if (!response.ok) {
            throw new Error("Erro ao criar denuncia.");
        }

        event.target.reset();
        mostrarMensagem("Denuncia criada com sucesso.");
        await carregarDenuncias();
    } catch (error) {
        mostrarMensagem(error.message, true);
    }
}

async function alterarStatus(id) {
    try {
        const response = await fetch(`${API_DENUNCIAS}/${id}`);

        if (!response.ok) {
            throw new Error("Denuncia nao encontrada.");
        }

        const denuncia = await response.json();
        denuncia.status = document.getElementById(`status-${id}`).value;

        const update = await fetch(`${API_DENUNCIAS}/${id}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(denuncia)
        });

        if (!update.ok) {
            throw new Error("Erro ao atualizar denuncia.");
        }

        mostrarMensagem("Status atualizado.");
        await carregarAdmin();
    } catch (error) {
        mostrarMensagem(error.message, true);
    }
}

async function excluirDenuncia(id) {
    try {
        const response = await fetch(`${API_DENUNCIAS}/${id}`, {
            method: "DELETE"
        });

        if (!response.ok) {
            throw new Error("Erro ao excluir denuncia.");
        }

        mostrarMensagem("Denuncia excluida.");
        await carregarAdmin();
    } catch (error) {
        mostrarMensagem(error.message, true);
    }
}

document.getElementById("formCadastroUsuario")?.addEventListener("submit", cadastrarUsuario);
document.getElementById("formLoginUsuario")?.addEventListener("submit", loginUsuario);
document.getElementById("formLoginAdmin")?.addEventListener("submit", loginAdmin);
document.getElementById("formDenuncia")?.addEventListener("submit", criarDenuncia);

carregarDenuncias();
carregarAdmin();
