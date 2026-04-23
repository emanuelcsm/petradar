# PetRadar — Configuração de Variáveis de Ambiente

Este guia explica como configurar as credenciais necessárias para rodar o PetRadar
localmente via Docker Compose. Nenhuma credencial deve ser commitada no repositório.

---

## 1. Criar o arquivo `.env`

Na raiz do repositório (`PetRadar/`), copie o exemplo:

```
# Windows (PowerShell)
Copy-Item .env.example .env

# Linux / macOS
cp .env.example .env
```

O arquivo `.env` está no `.gitignore` e nunca será rastreado pelo Git.

---

## 2. Gerar o `JWT_SECRET`

O segredo JWT deve ter **no mínimo 32 caracteres** e ser aleatório.

**Linux / macOS:**
```bash
openssl rand -base64 32
```

**Windows (PowerShell):**
```powershell
[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Maximum 256 }))
```

Cole o valor gerado no campo `JWT_SECRET` do seu `.env`:

```
JWT_SECRET=<valor gerado aqui>
```

---

## 3. Definir as credenciais do MongoDB

Escolha um nome de usuário e uma senha fortes para o usuário root do container MongoDB.
Esses valores são usados pelo `docker-compose.yml` para:

1. Inicializar o container MongoDB com autenticação habilitada (via `MONGO_INITDB_ROOT_*`)
2. Montar a connection string do backend:
   `mongodb://<username>:<password>@mongo:27017/petradar?authSource=admin`

Edite o `.env`:

```
MONGO_INITDB_ROOT_USERNAME=petradar_root
MONGO_INITDB_ROOT_PASSWORD=<senha forte aqui>
```

> Os valores padrão do `.env.example` são apenas placeholders — troque-os antes
> de subir o ambiente.

---

## 4. Verificar o `.env` final

O arquivo `.env` deve ter este formato (com valores reais):

```
JWT_SECRET=<string aleatória ≥32 chars>
MONGO_INITDB_ROOT_USERNAME=petradar_root
MONGO_INITDB_ROOT_PASSWORD=<senha forte>
MEDIA_BASE_PATH=/app/media
FRONTEND_ORIGIN=http://localhost:5173
```

---

## 5. Como o Docker Compose usa o `.env`

O Docker Compose lê o arquivo `.env` automaticamente quando está na mesma pasta do
`docker-compose.yml`. As variáveis são substituídas nos campos `${VAR_NAME}` do
compose sem nenhuma configuração adicional.

Não é necessário passar `--env-file` nem exportar as variáveis no shell.

---

## Variáveis de ambiente do `docker-compose.yml`

| Variável                       | Usado por         | Descrição                                     |
|-------------------------------|-------------------|-----------------------------------------------|
| `JWT_SECRET`                  | backend           | Segredo de assinatura dos tokens JWT          |
| `MONGO_INITDB_ROOT_USERNAME`  | mongo, backend    | Usuário root do MongoDB                       |
| `MONGO_INITDB_ROOT_PASSWORD`  | mongo, backend    | Senha do usuário root do MongoDB              |
| `MEDIA_BASE_PATH`             | backend           | Caminho de persistência de uploads no container |
| `FRONTEND_ORIGIN`             | backend           | Origem permitida no CORS (ex.: `http://localhost:5173`) |
