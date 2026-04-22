# MongoDB — Instalação Local e Configuração Docker

---

## Instalação Local (Windows)

### 1. MongoDB Community Server

1. Acesse [mongodb.com/try/download/community](https://www.mongodb.com/try/download/community)
2. Selecione **Version: 8.x**, **Platform: Windows**, **Package: msi**
3. Execute o instalador e escolha **Complete**
4. Marque **Install MongoDB as a Service** (roda automaticamente no boot)
5. Marque **Install MongoDB Compass** (GUI opcional, útil para inspecionar dados)

Após a instalação, o serviço `MongoDB` estará rodando na porta **27017**.

### 2. mongosh (MongoDB Shell)

> **Importante:** desde o MongoDB 6.0 o `mongosh` **não é mais incluído** no instalador do servidor.
> É um download separado.

1. Acesse [mongodb.com/try/download/shell](https://www.mongodb.com/try/download/shell)
2. Selecione **Platform: Windows**, **Package: msi** e instale
3. O instalador adiciona o `mongosh` ao PATH automaticamente

Verifique (abra um **novo** terminal após a instalação):

```powershell
mongosh --version
```

Teste a conexão local:

```powershell
mongosh
```

Dentro do shell:

```js
db.runCommand({ ping: 1 })
// Esperado: { ok: 1 }
```

### 3. Caminhos padrão

| Item               | Caminho padrão                            |
|--------------------|-------------------------------------------|
| Binários           | `C:\Program Files\MongoDB\Server\8.0\bin` |
| Dados              | `C:\Program Files\MongoDB\Server\8.0\data`|
| Logs               | `C:\Program Files\MongoDB\Server\8.0\log` |

### 4. Gerenciar o serviço Windows

> **Requer PowerShell como Administrador.** Clique com o botão direito no ícone do PowerShell → "Executar como administrador".

```powershell
# Iniciar
Start-Service MongoDB

# Parar
Stop-Service MongoDB

# Verificar status
Get-Service MongoDB
```

> O MongoDB é configurado como serviço automático no boot — em condições normais
> você não precisará iniciá-lo manualmente. Se `Get-Service MongoDB` mostrar `Running`,
> ele já está ativo.

---

## String de Conexão Local

```
mongodb://localhost:27017
```

Banco usado pelo PetRadar:

```
mongodb://localhost:27017/petradar
```

---

## Configuração no docker-compose

O `docker-compose.yml` na raiz do repositório já inclui o serviço `mongo`:

```yaml
services:
  mongo:
    image: mongo:8
    environment:
      MONGO_INITDB_ROOT_USERNAME: ${MONGO_INITDB_ROOT_USERNAME}
      MONGO_INITDB_ROOT_PASSWORD: ${MONGO_INITDB_ROOT_PASSWORD}
    volumes:
      - mongo_data:/data/db  # persiste os dados entre reinicializações
    # Sem 'ports:' — acessível apenas dentro da rede Docker

volumes:
  mongo_data:
```

### Credenciais de autenticação

O MongoDB **não tem usuário ou senha por padrão**. Ao usar a imagem Docker oficial,
as variáveis `MONGO_INITDB_ROOT_USERNAME` e `MONGO_INITDB_ROOT_PASSWORD` instruem
o container a **criar um usuário root** com os valores que você definir, na primeira
vez que o container sobe. Sem essas variáveis, o banco subiria sem autenticação.

O backend usa esses mesmos valores para se conectar — é por isso que eles precisam
coincidir entre o serviço `mongo` e o serviço `backend` no `docker-compose.yml`.

**Exemplo do fluxo:**
```
.env  →  MONGO_INITDB_ROOT_USERNAME=petradar_root
         MONGO_INITDB_ROOT_PASSWORD=MinhaS3nh4

docker-compose sobe o mongo  →  cria o usuário "petradar_root" com a senha "MinhaS3nh4"
docker-compose sobe o backend  →  conecta com mongodb://petradar_root:MinhaS3nh4@mongo:27017/...
```

**Como configurar:**

1. Copie o `.env.example` para `.env` (se ainda não fez):
   ```bash
   # Linux / macOS
   cp .env.example .env

   # Windows (PowerShell)
   Copy-Item .env.example .env
   ```
2. Abra o `.env` e defina o nome de usuário e senha que desejar:
   ```
   MONGO_INITDB_ROOT_USERNAME=petradar_root
   MONGO_INITDB_ROOT_PASSWORD=SuaSenhaForteAqui
   ```
3. Consulte `backend/docs/setup-env.md` para instruções de geração de senhas seguras.

> **Importante:** essas variáveis só têm efeito na **primeira** inicialização do
> container (quando o volume `mongo_data` ainda não existe). Se o volume já foi
> criado com outras credenciais, remova-o antes de alterar os valores:
> `docker-compose down -v` (apaga todos os dados).

### Variáveis de ambiente do backend no compose

```yaml
backend:
  environment:
    - MongoDB__ConnectionString=mongodb://${MONGO_INITDB_ROOT_USERNAME}:${MONGO_INITDB_ROOT_PASSWORD}@mongo:27017/petradar?authSource=admin
    - MongoDB__DatabaseName=petradar
```

> No Docker, o hostname é `mongo` (nome do serviço), não `localhost`.

### Inspecionar dados dentro do container

Execute a partir da raiz do repositório (onde está o `docker-compose.yml`).
Substitua os valores pelo que você definiu no seu `.env`:

```bash
docker-compose exec mongo mongosh \
  -u SEU_MONGO_INITDB_ROOT_USERNAME \
  -p SEU_MONGO_INITDB_ROOT_PASSWORD \
  --authenticationDatabase admin
```

Dentro do shell:

```js
use petradar
show collections
```

---

## Índices

Os índices são criados automaticamente no startup da aplicação (dentro de cada
`AddXxxModule()`). Não é necessário criá-los manualmente.

Para verificar os índices existentes:

```js
db.animals_posts.getIndexes()
db.identity_users.getIndexes()
db.notifications_items.getIndexes()
```
