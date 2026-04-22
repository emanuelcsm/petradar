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
    ports:
      - "27017:27017"        # expõe localmente para debug com Compass
    volumes:
      - mongo_data:/data/db  # persiste os dados entre reinicializações

volumes:
  mongo_data:
```

### Variáveis de ambiente do backend no compose

```yaml
backend:
  environment:
    - MongoDB__ConnectionString=mongodb://mongo:27017
    - MongoDB__DatabaseName=petradar
```

> No Docker, o hostname é `mongo` (nome do serviço), não `localhost`.

### Inspecionar dados dentro do container

```bash
docker exec -it <nome_do_container_mongo> mongosh
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
