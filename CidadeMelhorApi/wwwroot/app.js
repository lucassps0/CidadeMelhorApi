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
        cpf: document.getElementById("cpfCadastro")?.value ?? "",
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
        window.location.href = "/";
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
            const imagem = denuncia.imagemUrl
                ? `<img class="denuncia-imagem" src="${denuncia.imagemUrl}" alt="Foto da denuncia">`
                : "";

            lista.innerHTML += `
                <li>
                    <strong>${denuncia.categoria}</strong><br>
                    ${denuncia.descricao ?? ""}<br>
                    ${imagem}
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
    if (endereco) parametros.append("endereco", endereco);

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
            const imagem = denuncia.imagemUrl
                ? `<img class="denuncia-imagem" src="${denuncia.imagemUrl}" alt="Foto da denuncia">`
                : "";

            lista.innerHTML += `
                <li>
                    <strong>${denuncia.categoria}</strong><br>
                    Nome: ${denuncia.nomeUsuario ?? "Nao informado"}<br>
                    CPF: ${denuncia.cpfUsuario || "Nao informado"}<br>
                    ${denuncia.descricao ?? ""}<br>
                    ${imagem}
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

    const endereco = montarEndereco();

    const imagem = document.getElementById("imagem")?.files[0];
    const denuncia = new FormData();

    denuncia.append("descricao", document.getElementById("descricao").value);
    denuncia.append("categoria", document.getElementById("categoria").value);
    denuncia.append("endereco", endereco);
    denuncia.append("usuarioId", usuario.id);
    denuncia.append("status", "Aberto");

    if (imagem) {
        denuncia.append("imagem", imagem);
    }

    try {
        const response = await fetch(API_DENUNCIAS, {
            method: "POST",
            body: denuncia
        });

        if (!response.ok) {
            const erro = await response.text();
            throw new Error(erro || "Erro ao criar denuncia.");
        }

        event.target.reset();
        mostrarMensagem("Denuncia criada com sucesso.");
        await carregarDenuncias();
    } catch (error) {
        mostrarMensagem(error.message, true);
    }
}

function montarEndereco() {
    const cep = document.getElementById("cep").value;
    const logradouro = document.getElementById("logradouro").value;
    const numero = document.getElementById("numero").value;
    const complemento = document.getElementById("complemento").value;
    const bairro = document.getElementById("bairro").value;
    const cidade = document.getElementById("cidade").value;
    const uf = document.getElementById("uf").value;

    return [
        logradouro,
        numero,
        complemento,
        bairro,
        cidade && uf ? `${cidade}/${uf}` : cidade || uf,
        `CEP: ${cep}`
    ].filter(Boolean).join(", ");
}

async function buscarCep() {
    const cepInput = document.getElementById("cep");

    if (!cepInput) {
        return;
    }

    const cep = cepInput.value.replace(/\D/g, "");
    cepInput.value = formatarCep(cep);

    if (cep.length !== 8) {
        return;
    }

    try {
        mostrarMensagem("Buscando CEP...");
        const response = await fetch(`/api/cep/${cep}`);

        if (!response.ok) {
            throw new Error("Nao foi possivel buscar o CEP.");
        }

        const endereco = await response.json();

        if (endereco.erro) {
            throw new Error("CEP nao encontrado.");
        }

        document.getElementById("logradouro").value = endereco.logradouro ?? "";
        document.getElementById("bairro").value = endereco.bairro ?? "";
        document.getElementById("cidade").value = endereco.localidade ?? "";
        document.getElementById("uf").value = endereco.uf ?? "";
        mostrarMensagem("Endereco preenchido pelo CEP.");
        document.getElementById("numero").focus();
    } catch (error) {
        mostrarMensagem(error.message, true);
    }
}

function formatarCep(cep) {
    if (cep.length <= 5) {
        return cep;
    }

    return `${cep.substring(0, 5)}-${cep.substring(5, 8)}`;
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
document.getElementById("cep")?.addEventListener("blur", buscarCep);

carregarDenuncias();
carregarAdmin();
