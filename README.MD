# PetRadar

Sistema de catalogação de animais perdidos em tempo real. Um usuário fotografa um
animal na rua, registra com localização e descrição. Outros usuários veem o feed
ao vivo, podendo enviar dicas ao publicador, marcar o animal como encontrado e
receber notificações push no mesmo instante.

---

## Funcionalidades do frontend

| Funcionalidade | Descrição |
|---|---|
| Registro e login | Criação de conta com e-mail/senha; sessão mantida via JWT armazenado no cliente |
| Feed de animais | Lista paginada (cursor-based) de posts próximos; filtragem por raio geográfico |
| Mapa interativo | Visualização geoespacial dos posts no mapa (Leaflet) com pin por localização |
| Publicar animal | Formulário para registrar animal perdido/encontrado com foto e coordenadas GPS |
| Detalhe do animal | Página individual do post com galeria de imagens e status (perdido/encontrado) |
| Enviar dica | Envio de mensagem direta ao publicador a partir da página de detalhe |
| Marcar como encontrado | Ação disponível ao dono do post para encerrar o caso |
| Notificações em tempo real | Sino de notificações atualizado via SignalR; modal exibe dicas recebidas |
| Upload de mídia | Envio de imagem `multipart/form-data` com preview antes de publicar |

---

## Tecnologias

| Camada | Tecnologia | Justificativa |
|---|---|---|
| Frontend | Vue 3 + TypeScript | Reatividade granular (Composition API), tipagem estática reduz erros em runtime |
| Build frontend | Vite | HMR instantâneo, bundle otimizado para SPA |
| Estado | Pinia | API simples e fortemente tipada, substituto oficial do Vuex |
| Backend | ASP.NET Core 10 | Performance, tipagem estática, ecossistema maduro para APIs REST |
| Banco de dados | MongoDB 8 | Esquema flexível para posts com campos opcionais; suporte nativo a índices geoespaciais (2dsphere) para consultas de proximidade |
| Cache / sessão | Redis 7 | Armazenamento de blacklist de tokens JWT e backplane do SignalR |
| Tempo real | SignalR | Abstração sobre WebSocket com fallback automático; integrada ao .NET sem dependência extra |
| Containerização | Docker / Docker Compose | Ambiente reproduzível; único comando para subir todos os serviços |

---

## Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (inclui Docker Compose v2)

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

| Serviço | URL local |
|---|---|
| Frontend | http://localhost:5173 |
| Backend (API) | http://localhost:5000 |
| MongoDB | interno (sem porta exposta) |
| Redis | interno (sem porta exposta) |

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

## Variáveis de ambiente

As variáveis são lidas do arquivo `.env` na raiz do projeto.
O arquivo `.env` nunca é commitado — use `.env.example` como referência.

| Variável | Descrição | Exemplo |
|---|---|---|
| `JWT_SECRET` | Segredo de assinatura dos tokens JWT (mínimo 32 caracteres) | `change-me-use-a-long-random-string` |
| `MONGO_INITDB_ROOT_USERNAME` | Usuário root do MongoDB | `petradar_root` |
| `MONGO_INITDB_ROOT_PASSWORD` | Senha do usuário root do MongoDB | `strongpassword` |
| `MONGO_DB_NAME` | Nome do banco de dados da aplicação | `petradar` |
| `REDIS_CONNECTION_STRING` | Host e porta do Redis | `redis:6379` |
| `ASPNETCORE_ENVIRONMENT` | Ambiente do ASP.NET Core | `Production` |
| `JWT_EXPIRES_IN_MINUTES` | Tempo de expiração do token JWT em minutos | `60` |
| `MEDIA_BASE_PATH` | Caminho de armazenamento de mídia no container | `/app/media` |
| `FRONTEND_ORIGIN` | Origem permitida no CORS | `http://localhost:5173` |

---

## Estrutura de pastas

```
PetRadar/
├── backend/
│   ├── Dockerfile                        ← Multi-stage: SDK (build) → ASP.NET runtime
│   └── src/
│       ├── PetRadar.API/                 ← Entry point: controllers, middleware, DI
│       ├── PetRadar.SharedKernel/        ← Abstrações base: Entity, DomainEvent, IRepository
│       ├── BuildingBlocks/
│       │   └── PetRadar.IntegrationEvents/  ← Contratos de eventos cross-module
│       └── Modules/
│           ├── Animals/                  ← Módulo de posts de animais
│           │   ├── Animals.Domain/
│           │   ├── Animals.Application/
│           │   └── Animals.Infrastructure/
│           ├── Identity/                 ← Módulo de autenticação e usuários
│           │   ├── Identity.Domain/
│           │   ├── Identity.Application/
│           │   └── Identity.Infrastructure/
│           ├── Media/                    ← Módulo de upload e armazenamento de arquivos
│           │   ├── Media.Domain/
│           │   ├── Media.Application/
│           │   └── Media.Infrastructure/
│           └── Notifications/            ← Módulo de notificações em tempo real
│               ├── Notifications.Application/
│               └── Notifications.Infrastructure/
├── frontend/
│   ├── Dockerfile                        ← Multi-stage: Node/Vite (build) → Nginx
│   ├── nginx.conf                        ← Proxy reverso para /api/, /media/, /hubs/
│   └── src/
│       ├── modules/                      ← Funcionalidades por domínio (animals, auth, notifications)
│       ├── components/                   ← Componentes UI reutilizáveis
│       ├── composables/                  ← Hooks de Composition API
│       ├── router/
│       └── services/                     ← Clientes HTTP e SignalR
├── docs/                                 ← Guias de setup e coleções Postman
├── docker-compose.yml
├── .env.example
└── README.MD
```

---

## Endpoints da API

### Autenticação — `/api/auth`

| Método | Rota | Auth | Descrição |
|---|---|---|---|
| `POST` | `/api/auth/register` | Não | Registra novo usuário (nome, e-mail, senha) |
| `POST` | `/api/auth/login` | Não | Autentica usuário; retorna JWT e dados do perfil |
| `POST` | `/api/auth/logout` | Sim | Invalida o token JWT (blacklist no Redis) |

### Animais — `/api/animals`

| Método | Rota | Auth | Descrição |
|---|---|---|---|
| `POST` | `/api/animals` | Sim | Cria post de animal perdido/encontrado com localização e mídia |
| `GET` | `/api/animals/{id}` | Não | Retorna post por ID |
| `GET` | `/api/animals/nearby` | Não | Lista posts próximos (params: `lat`, `lng`, `radiusKm`); paginação cursor-based |
| `DELETE` | `/api/animals/{id}` | Sim | Remove post (somente o dono) |
| `PATCH` | `/api/animals/{id}/found` | Sim | Marca animal como encontrado |
| `POST` | `/api/animals/{id}/tips` | Sim | Envia dica/mensagem ao publicador do post |

### Mídia — `/api/media`

| Método | Rota | Auth | Descrição |
|---|---|---|---|
| `POST` | `/api/media` | Sim | Faz upload de arquivo (`multipart/form-data`); retorna `mediaId` |

### Notificações — `/api/notifications`

| Método | Rota | Auth | Descrição |
|---|---|---|---|
| `GET` | `/api/notifications` | Sim | Lista notificações do usuário autenticado (cursor-based: `nextPageToken`, `hasNextPage`) |
| `PATCH` | `/api/notifications/{id}/read` | Sim | Marca notificação como lida |

### Real-time — SignalR

| Hub | Rota | Evento | Descrição |
|---|---|---|---|
| `AnimalHub` | `/hubs/animals` | `ReceiveNotification` | Entrega notificação em tempo real ao cliente conectado |

---

## Arquitetura

### Monolito Modular com Clean Architecture

O backend é um **Monolito Modular**: todos os módulos rodam no mesmo processo, mas
são estritamente isolados entre si. Cada módulo (`Animals`, `Identity`, `Media`,
`Notifications`) possui três camadas internas independentes:

```
<Módulo>.Domain        ← Entidades, value objects, domain events (sem dependência externa)
<Módulo>.Application   ← Commands, queries, handlers, bridges de eventos
<Módulo>.Infrastructure← Repositórios MongoDB, serviços externos
```

**Regra de isolamento:** nenhum `ProjectReference` cruza a fronteira entre módulos de
negócio. Um módulo nunca importa diretamente o namespace de outro módulo.

---

### Comunicação cross-module via IntegrationEvents

A comunicação entre módulos ocorre **exclusivamente** por meio de
`IntegrationEvent`, definidos no projeto `PetRadar.IntegrationEvents`
(pertencente a `BuildingBlocks`). Esse projeto é o único contrato compartilhado
entre módulos.

Fluxo de publicação:

```
DomainEvent (Animals.Domain)
    │
    ▼  bridge no Application
IntegrationEvent (PetRadar.IntegrationEvents)
    │
    ▼  IPublisher.Publish() via MediatR (in-process)
INotificationHandler<TIntegrationEvent> (Notifications.Application)
```

O MediatR despacha o evento de integração **in-process**, sem broker externo.
Isso mantém consistência dentro da transação lógica e simplifica o deploy.

---

### Bridges DomainEvent → IntegrationEvent

Cada módulo que precisa notificar outros define um handler na camada Application
que escuta um `DomainEvent` interno e publica o `IntegrationEvent` correspondente:

| DomainEvent (origem) | Bridge (módulo) | IntegrationEvent publicado |
|---|---|---|
| `AnimalPostedEvent` | Animals.Application | `AnimalPostedIntegrationEvent` |
| `AnimalFoundEvent` | Animals.Application | `AnimalFoundIntegrationEvent` |
| `AnimalPostDeletedDomainEvent` | Animals.Application | `AnimalPostDeletedIntegrationEvent` |
| `AnimalTipSentDomainEvent` | Animals.Application | `AnimalTipSentIntegrationEvent` |
| `UserRegisteredEvent` | Identity.Application | `UserRegisteredIntegrationEvent` |
| `MediaUploadedDomainEvent` | Media.Application | `MediaUploadedIntegrationEvent` |

---

### Projeções locais e consistência eventual

O módulo `Notifications` não acessa diretamente coleções de outros módulos.
Ele mantém sua **própria projeção** (`notifications` collection no MongoDB),
alimentada pelos `IntegrationEvent` recebidos via MediatR.

Consequência: uma notificação pode aparecer com pequeno atraso após a ação que
a originou — comportamento esperado de consistência eventual. Não há consulta
cross-module para montar a resposta de nenhum endpoint.

---

## Testes e coverage (backend)

Execute na raiz do repositório (requer .NET SDK instalado localmente ou container de desenvolvimento):

```powershell
Set-Location .\backend
dotnet test PetRadar.slnx --collect:"XPlat Code Coverage" --results-directory .\TestResults
reportgenerator `
  -reports:".\TestResults\**\coverage.cobertura.xml" `
  -targetdir:".\TestResults\CoverageReportFiltered" `
  -reporttypes:"Html;TextSummary" `
  -classfilters:"+*;-Microsoft.AspNetCore.OpenApi.Generated*" `
  -filefilters:"+*;-*obj*;-*.generated.cs;-*AssemblyInfo.cs"
```

Abra `backend/TestResults/CoverageReportFiltered/index.html` para ver o relatório completo.
