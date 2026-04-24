# PetRadar

Sistema de catalogação de animais perdidos em tempo real. Um usuário fotografa um
animal na rua, registra com localização e descrição. Outros usuários veem o feed
ao vivo, podendo marcar o animal como encontrado ou entrar em contato com o publicador.

---

## Tecnologias

| Camada              | Tecnologia                        |
|---------------------|-----------------------------------|
| Frontend            | Vue 3                             |
| Backend             | .NET 10                           |
| Banco de dados      | MongoDB 8                         |
| Tempo real          | SignalR                           |
| Cache               | Redis 7                           |
| Containerização     | Docker / Docker Compose           |

---

## Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (inclui Docker Compose)

Não é necessário instalar .NET, Node.js ou qualquer outra dependência localmente.
Tudo roda dentro dos containers.

---

## Configuração inicial

1. Clone o repositório e entre na pasta:

   ```bash
   git clone <url-do-repositorio>
   cd PetRadar
   ```

2. Copie o arquivo de variáveis de ambiente:

   ```bash
   # Linux / macOS
   cp .env.example .env

   # Windows (PowerShell)
   Copy-Item .env.example .env
   ```

3. Edite o `.env` e preencha os valores reais.
   Consulte [`docs/setup-env.md`](docs/setup-env.md) para instruções
   detalhadas sobre como gerar o `JWT_SECRET` e as credenciais do MongoDB.

---

## Rodando o projeto

```bash
docker-compose up --build
```

| Serviço   | URL local               |
|-----------|-------------------------|
| Frontend  | http://localhost:5173   |
| Backend   | http://localhost:5000   |
| MongoDB   | interno (sem porta exposta) |
| Redis     | interno (sem porta exposta) |

Para subir apenas os serviços de infraestrutura (MongoDB + Redis):

```bash
docker-compose up mongo redis -d
```

Para parar tudo:

```bash
docker-compose down
```

Para parar e remover os volumes (apaga os dados):

```bash
docker-compose down -v
```

---

## Testes e coverage (backend)

Para medir cobertura do backend localmente, execute na pasta `backend`:

```powershell
Set-Location .\backend; dotnet test PetRadar.slnx --collect:"XPlat Code Coverage" --results-directory .\TestResults; reportgenerator -reports:".\TestResults\**\coverage.cobertura.xml" -targetdir:".\TestResults\CoverageReportFiltered" -reporttypes:"Html;TextSummary" -classfilters:"+*;-Microsoft.AspNetCore.OpenApi.Generated*" -filefilters:"+*;-*obj*;-*.generated.cs;-*AssemblyInfo.cs"
```

O comando acima:

1. Executa todos os testes da solução.
2. Gera arquivos Cobertura (`coverage.cobertura.xml`) em `backend/TestResults`.
3. Consolida um relatório filtrado em `backend/TestResults/CoverageReportFiltered`.

### Como ler os resultados

1. Abra `backend/TestResults/CoverageReportFiltered/index.html` no navegador.
2. Verifique os indicadores principais:
   - **Line coverage**: percentual de linhas executadas.
   - **Branch coverage**: percentual de caminhos condicionais cobertos.
   - **Risk Hotspots**: classes/métodos com alta complexidade e baixa cobertura.
3. Consulte também `backend/TestResults/CoverageReportFiltered/Summary.txt` para um resumo textual útil em CI/logs.

---

## Variáveis de ambiente

As variáveis são lidas do arquivo `.env` na raiz do projeto.
O arquivo `.env` nunca é commitado — use `.env.example` como referência.

| Variável                      | Descrição                                    |
|-------------------------------|----------------------------------------------|
| `JWT_SECRET`                  | Segredo de assinatura dos tokens JWT (≥32 chars) |
| `MONGO_INITDB_ROOT_USERNAME`  | Usuário root do MongoDB                      |
| `MONGO_INITDB_ROOT_PASSWORD`  | Senha do usuário root do MongoDB             |
| `MEDIA_BASE_PATH`             | Caminho de armazenamento de mídia no container |
| `FRONTEND_ORIGIN`             | Origem HTTP/HTTPS do frontend permitida no CORS |

---

## Estrutura de pastas

```
PetRadar/
├── backend/                  ← .NET 10 (Monolito Modular)
│   ├── Dockerfile
│   ├── src/
│   │   ├── PetRadar.API/
│   │   ├── PetRadar.SharedKernel/
│   │   └── Modules/
│   │       ├── Animals/
│   │       ├── Identity/
│   │       ├── Notifications/
│   │       └── Media/
│   └── docs/
├── frontend/                 ← Vue 3
│   └── Dockerfile
├── docker-compose.yml
├── .env.example
└── README.md
```

---

## Arquitetura

O backend segue **Monolito Modular** com Clean Architecture por módulo.
Comunicação entre módulos via Domain Events (MediatR). Isolamento de dados
via convenção de prefixo de coleção no MongoDB.

### Notifications API

- `GET /api/notifications`: lista notificações do usuário autenticado com
   paginação cursor-based (`nextPageToken`, `hasNextPage`).
- `PATCH /api/notifications/{id}/read`: marca notificação como lida.

Regra de consistência eventual: como a notificação é gerada a partir de eventos de
integração, ela pode aparecer com pequeno atraso após a ação que originou o evento.
Não existe consulta cross-module para montar a resposta do endpoint.

Detalhes completos em [`../architecture.md`](../architecture.md).
