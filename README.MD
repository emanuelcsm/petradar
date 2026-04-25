# PetRadar

Sistema de catalogação e localização de animais perdidos em tempo real. Um usuário
fotografa um animal na rua, registra o posto com localização e descrição. Outros
usuários veem o feed ao vivo da sua região, podem enviar dicas ao publicador, marcar
o animal como encontrado e receber notificações push no mesmo instante.

**Motivação:** não existe hoje uma ferramenta única que centralize relatos de animais
perdidos com precisão geográfica e comunicação em tempo real entre quem perdeu e quem
encontrou. O PetRadar resolve esse gap conectando pessoas dentro da mesma região por
meio de posts geolocalizados e notificações instantâneas via WebSocket.

---

## Funcionalidades

| Funcionalidade | Descrição |
|---|---|
| Cadastro e login | Criação de conta com e-mail e senha; sessão mantida via JWT armazenado localmente |
| Logout com invalidação | Token revogado no Redis ao sair; requisições subsequentes são rejeitadas |
| Feed regional | Lista paginada (cursor-based) de posts próximos filtrada por raio geográfico; atualizada em tempo real via SignalR |
| Mapa interativo | Visualização geoespacial dos posts via Leaflet com pin por coordenada GPS |
| Publicar animal | Formulário com upload de foto, descrição e captura de coordenadas GPS do navegador |
| Detalhe do animal | Página individual com galeria de imagens e status atual (perdido / encontrado) |
| Enviar dica | Usuário autenticado envia mensagem direta ao publicador; notificação chega em tempo real |
| Marcar como encontrado | Ação exclusiva do dono do post para encerrar o caso; propagada em tempo real para a região |
| Remover post | Dono pode excluir o post; arquivos de mídia são removidos automaticamente |
| Notificações em tempo real | Sino atualizado via SignalR; modal exibe dicas recebidas com conteúdo e remetente |
| Upload de mídia | Envio `multipart/form-data` com preview antes de publicar; retorna `mediaId` para reuso |

---

## Tecnologias e justificativas

| Camada | Tecnologia | Versão | Justificativa |
|---|---|---|---|
| Frontend | Vue 3 + TypeScript | Vite 6 | Composition API garante reatividade granular; tipagem estática reduz erros em runtime |
| Estado | Pinia | – | API simples e tipada, substituto oficial do Vuex |
| Backend | ASP.NET Core | .NET 10 | Performance, tipagem estática e ecossistema maduro para APIs REST e SignalR integrado |
| Banco de dados | MongoDB | 8 | Esquema flexível para posts com campos opcionais; índice nativo 2dsphere para consultas de proximidade |
| Cache / sessão | Redis | 7 | Blacklist de JWTs revogados, cache de resultados de busca geoespacial e backplane do SignalR |
| Tempo real | SignalR | – | Abstração sobre WebSocket com reconexão automática; integrado ao .NET sem dependência extra |
| Containerização | Docker Compose | – | Ambiente reproduzível; único comando para subir todos os serviços |

### Decisão arquitetural — Monolito Modular

O backend foi construído como um **monolito modular** (não microsserviços) de forma
deliberada: demonstra que separação de responsabilidades, desacoplamento e comunicação
orientada a eventos não são exclusividade de arquiteturas distribuídas. Cada módulo
(`Animals`, `Identity`, `Media`, `Notifications`) é estritamente isolado e poderia ser
extraído para um microsserviço independente sem alteração de contrato.

O frontend segue o modelo de **feature modules** (`src/modules/animals`, `src/modules/auth`,
`src/modules/notifications`), com cada feature responsável pelos seus próprios componentes,
stores (Pinia), serviços HTTP e tipos — aplicando SRP, DRY, SOLID e separação por
coesão em vez de separação por tipo técnico.

---

## Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) — inclui Docker Compose v2.

Não é necessário instalar .NET, Node.js ou qualquer outra dependência localmente.
Tudo roda dentro dos containers.

---

## Configuração e execução

1. Clone o repositório:

   ```bash
   git clone <url-do-repositorio>
   cd PetRadar
   ```

2. Crie o arquivo de variáveis de ambiente:

   ```bash
   # Linux / macOS
   cp .env.example .env

   # Windows (PowerShell)
   Copy-Item .env.example .env
   ```

3. Edite o `.env` com valores reais (veja seção [Variáveis de ambiente](#variáveis-de-ambiente)).

4. Suba todos os serviços:

   ```bash
   docker-compose up --build
   ```

| Serviço | URL local |
|---|---|
| Frontend | http://localhost:5173 |
| Backend (API) | http://localhost:5000 |
| MongoDB | interno — sem porta exposta |
| Redis | interno — sem porta exposta |

**Comandos úteis:**

```bash
# Somente infraestrutura (MongoDB + Redis), útil para dev local
docker-compose up mongo redis -d

# Parar tudo
docker-compose down

# Parar e apagar volumes (remove todos os dados)
docker-compose down -v
```

---

## Variáveis de ambiente

O arquivo `.env` nunca é commitado. Use `.env.example` como referência.

| Variável | Descrição | Exemplo |
|---|---|---|
| `JWT_SECRET` | Segredo de assinatura dos tokens JWT (mínimo 32 caracteres) | `change-me-use-a-long-random-string` |
| `MONGO_INITDB_ROOT_USERNAME` | Usuário root do MongoDB | `petradar_root` |
| `MONGO_INITDB_ROOT_PASSWORD` | Senha root do MongoDB | `strongpassword` |
| `MONGO_DB_NAME` | Nome do banco de dados da aplicação | `petradar` |
| `REDIS_CONNECTION_STRING` | Host e porta do Redis (rede interna Docker) | `redis:6379` |
| `ASPNETCORE_ENVIRONMENT` | Ambiente do ASP.NET Core | `Production` |
| `JWT_EXPIRES_IN_MINUTES` | Expiração do token em minutos | `60` |
| `MEDIA_BASE_PATH` | Caminho de armazenamento de arquivos no container | `/app/media` |
| `FRONTEND_ORIGIN` | Origem permitida no CORS | `http://localhost:5173` |

---

## Estrutura de pastas

```
PetRadar/
├── backend/
│   ├── Dockerfile                           ← Multi-stage: SDK build → ASP.NET runtime
│   └── src/
│       ├── PetRadar.API/                    ← Entry point: controllers, middleware, DI, hubs
│       ├── PetRadar.SharedKernel/           ← Abstrações base: Entity, DomainEvent, IRepository
│       ├── BuildingBlocks/
│       │   └── PetRadar.IntegrationEvents/  ← Contratos de eventos cross-module (único shared)
│       └── Modules/
│           ├── Animals/                     ← Posts de animais perdidos
│           │   ├── Animals.Domain/          ← Entidades, value objects, domain events
│           │   ├── Animals.Application/     ← Commands, queries, bridges de eventos, cache
│           │   └── Animals.Infrastructure/  ← MongoDB, Redis cache
│           ├── Identity/                    ← Autenticação e usuários
│           │   ├── Identity.Domain/
│           │   ├── Identity.Application/
│           │   └── Identity.Infrastructure/ ← MongoDB, JWT, blacklist Redis
│           ├── Media/                       ← Upload e armazenamento de arquivos
│           │   ├── Media.Domain/
│           │   ├── Media.Application/
│           │   └── Media.Infrastructure/    ← MongoDB, armazenamento local
│           └── Notifications/               ← Notificações em tempo real
│               ├── Notifications.Application/   ← Consumidores de IntegrationEvents
│               └── Notifications.Infrastructure/← MongoDB, SignalR hub
├── frontend/
│   ├── Dockerfile                           ← Multi-stage: Node/Vite build → Nginx
│   ├── nginx.conf                           ← Proxy reverso para /api/, /media/, /hubs/
│   └── src/
│       ├── modules/                         ← Features por domínio
│       │   ├── animals/                     ← Feed, detalhe, publicação
│       │   ├── auth/                        ← Login, cadastro
│       │   └── notifications/               ← Sino, modal de dicas
│       ├── components/                      ← Componentes UI reutilizáveis (AppButton, AppCard…)
│       ├── composables/                     ← useSignalR, useGeolocation, usePagination, useToast
│       ├── router/                          ← Vue Router com guards de autenticação
│       └── services/                        ← Axios instance e clientes HTTP por módulo
├── docs/                                    ← Guias de setup e documentação complementar
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
| `POST` | `/api/auth/login` | Não | Autentica; retorna JWT, userId, nome e e-mail |
| `POST` | `/api/auth/logout` | Sim | Invalida o token JWT (blacklist Redis por JTI) |

### Animais — `/api/animals`

| Método | Rota | Auth | Descrição |
|---|---|---|---|
| `POST` | `/api/animals` | Sim | Cria post com localização e lista de `mediaIds` |
| `GET` | `/api/animals/{id}` | Não | Retorna post por ID |
| `GET` | `/api/animals/nearby` | Não | Lista posts próximos (`lat`, `lng`, `radiusKm`); paginação cursor-based |
| `DELETE` | `/api/animals/{id}` | Sim | Remove post (somente o dono) |
| `PATCH` | `/api/animals/{id}/found` | Sim | Marca como encontrado (somente o dono) |
| `POST` | `/api/animals/{id}/tips` | Sim | Envia dica ao publicador do post |

### Mídia — `/api/media`

| Método | Rota | Auth | Descrição |
|---|---|---|---|
| `POST` | `/api/media` | Sim | Upload `multipart/form-data`; retorna `{ mediaId }` |

### Notificações — `/api/notifications`

| Método | Rota | Auth | Descrição |
|---|---|---|---|
| `GET` | `/api/notifications` | Sim | Lista notificações do usuário com cursor-based pagination |
| `PATCH` | `/api/notifications/{id}/read` | Sim | Marca notificação como lida |

### Real-time — SignalR

| Hub | Rota | Tipo | Evento | Descrição |
|---|---|---|---|---|
| `AnimalHub` | `/hubs/animals` | Broadcast regional | `animal-posted` | Novo post criado na região |
| `AnimalHub` | `/hubs/animals` | Broadcast regional | `animal-found` | Animal marcado como encontrado |
| `AnimalHub` | `/hubs/animals` | Broadcast regional | `animal-deleted` | Post removido da região |
| `AnimalHub` | `/hubs/animals` | Direto ao usuário | `new-notification` | Dica recebida no post do usuário |

---

## Arquitetura

### Monolito modular com Clean Architecture

Cada módulo (`Animals`, `Identity`, `Media`, `Notifications`) é organizado em três
camadas internas:

```
<Módulo>.Domain         ← Entidades, value objects, domain events (sem dependência externa)
<Módulo>.Application    ← Commands, queries, handlers, bridges DomainEvent→IntegrationEvent
<Módulo>.Infrastructure ← Repositórios MongoDB, serviços de cache e storage
```

**Regra de isolamento:** nenhum `ProjectReference` cruza a fronteira entre módulos de
negócio. `Animals.Application` não referencia `Identity.*`, `Media.*` nem
`Notifications.*` — e vice-versa. O único projeto compartilhado entre módulos é
`PetRadar.IntegrationEvents`.

---

### Comunicação cross-module via IntegrationEvents

A comunicação entre módulos ocorre **exclusivamente** por `IntegrationEvent` publicado
in-process via MediatR. Isso elimina acoplamento direto sem exigir broker externo.

```
DomainEvent (Animals.Domain)
    │
    ▼  INotificationHandler<DomainEvent> — bridge em Animals.Application
IntegrationEvent (PetRadar.IntegrationEvents.Animals)
    │
    ▼  IPublisher.Publish() — MediatR in-process
INotificationHandler<IntegrationEvent> (Notifications.Application / Media.Application)
```

---

### Bridges DomainEvent → IntegrationEvent

Cada bridge é um `INotificationHandler<DomainEvent>` registrado na camada Application
do módulo de origem. Ele recebe o evento de domínio interno e publica o evento de
integração correspondente via `IMediator.Publish()`.

| DomainEvent (origem) | Módulo | IntegrationEvent publicado |
|---|---|---|
| `AnimalPostedEvent` | Animals.Application | `AnimalPostedIntegrationEvent` |
| `AnimalFoundEvent` | Animals.Application | `AnimalFoundIntegrationEvent` |
| `AnimalPostDeletedDomainEvent` | Animals.Application | `AnimalPostDeletedIntegrationEvent` |
| `AnimalTipSentDomainEvent` | Animals.Application | `AnimalTipSentIntegrationEvent` |
| `UserRegisteredEvent` | Identity.Application | `UserRegisteredIntegrationEvent` |
| `MediaUploadedDomainEvent` | Media.Application | `MediaUploadedIntegrationEvent` |

---

### Projeções locais e consistência eventual

O módulo `Notifications` mantém sua própria coleção `notifications` no MongoDB,
alimentada pelos `IntegrationEvent` que consome. Nenhum endpoint faz consulta
cross-module. O módulo `Animals` mantém uma projeção local de mídias conhecidas
(`IKnownMediaRepository`) alimentada por `MediaUploadedIntegrationEvent`, evitando
chamadas síncronas ao módulo Media durante a criação de posts.

---

### Como o MediatR é usado

MediatR é o eixo central da aplicação. Ele opera em dois papéis distintos:

**1. CQRS — Commands e Queries**

Todos os casos de uso são expressos como `IRequest<TResponse>` e resolvidos por um
`IRequestHandler` correspondente. Os controllers apenas constroem o objeto de
request e chamam `IMediator.Send()` — nenhuma lógica de negócio vive no controller.

**2. Pipeline de eventos em três camadas**

Os agregados acumulam `DomainEvent` internamente. Após a persistência, o
`MediatRDomainEventDispatcher` chama `IPublisher.Publish()` para cada evento coletado.
A partir daí, o MediatR encadeia três camadas de handlers via multicasting:

```
Aggregate → CollectDomainEvents()
              │
              ▼ MediatRDomainEventDispatcher.DispatchAsync()
         INotificationHandler<DomainEvent>          ← handlers internos ao módulo
              │  e também
              ▼ INotificationHandler<DomainEvent>   ← bridge publica IntegrationEvent
         IPublisher.Publish(integrationEvent)
              │
              ▼ INotificationHandler<IntegrationEvent>  ← consumidores em outros módulos
```

Como `INotification` suporta N handlers para o mesmo tipo, o mesmo `DomainEvent`
pode ser recebido simultaneamente pelo bridge e por outros handlers internos (ex.:
`AnimalListingCacheInvalidator` e `AnimalPostedIntegrationEventPublisher` ambos ouvem
`AnimalPostedEvent`). Tudo ocorre in-process, sem broker externo.

---

### Como o MongoDB é usado

Quatro coleções independentes, cada uma de propriedade exclusiva do seu módulo:

| Coleção | Módulo | Destaque |
|---|---|---|
| `animals` | Animals | Índice `2dsphere` no campo `Location`; consultas `$nearSphere` para raio geográfico |
| `users` | Identity | Busca por e-mail normalizado; upsert por ID |
| `media` | Media | Registro de arquivos enviados; deletado em cascata por IntegrationEvent |
| `notifications` | Notifications | Projeção própria; paginação por cursor (CreatedAt, Id) |

**Paginação cursor-based:** as listagens de animais próximos e de notificações usam
paginação por par `(CreatedAt DESC, Id DESC)` como cursor opaco serializado em Base64.
Isso garante estabilidade de página mesmo com inserções concorrentes.

**Upsert:** o padrão de save em todos os repositórios usa
`ReplaceOneAsync(filter, document, new ReplaceOptions { IsUpsert = true })`, permitindo
que o mesmo handler seja idempotente para criação e atualização.

---

### Como o Redis é usado

Redis serve três finalidades independentes:

**1. Cache de listagem geoespacial**

`GET /api/animals/nearby` é cacheado por chave composta de região + versão + parâmetros.
A versão por região é armazenada em `animals:region_version:{lat:F1}:{lng:F1}` e
incrementada pelo `AnimalListingCacheInvalidator` toda vez que `AnimalPostedEvent` ou
`AnimalFoundEvent` ocorre. Isso invalida automaticamente o cache sem precisar enumerar
chaves — qualquer nova query inclui a versão atualizada na chave e gera um cache miss.
Falhas de Redis são absorvidas silenciosamente; o sistema continua funcionando.

**2. Blacklist de JWTs revogados**

No logout, o `LogoutCommandHandler` extrai o `jti` (JWT ID) do token e o armazena no
Redis via `RedisTokenBlacklist` com chave `jti_blacklist:{jti}` e TTL igual ao tempo
restante de vida do token. O middleware de autenticação verifica a blacklist em cada
request autenticado — um token logado fora deixa de funcionar imediatamente.

**3. Backplane do SignalR**

`AddStackExchangeRedis(connectionString)` transforma o Redis em um backplane de
pub/sub para o SignalR. Em um cenário com múltiplas instâncias do backend, mensagens
publicadas em qualquer instância são entregues a clientes conectados em qualquer outra
instância — o Redis garante a fan-out cross-server.

---

### Como o SignalR é usado

**Hub:** `AnimalHub` mapeado em `/hubs/animals`. O cliente se autentica passando o JWT
via `accessTokenFactory` (protocolo padrão do SignalR sobre WebSocket). O provedor de
identidade `SubClaimUserIdProvider` extrai o `sub` do JWT para identificar o usuário
no `IHubContext`.

**Grupos regionais:** o frontend chama `JoinRegion(regionKey)` ao abrir o feed.
A `regionKey` segue o mesmo esquema de tiles do cache:
`region:{lat_arredondado_1_decimal}:{lng_arredondado_1_decimal}` (células de ~11 km).
Ao fechar o feed, `LeaveRegion(regionKey)` remove o cliente do grupo.

**Eventos pushed pelo servidor:**

| Evento | Destinatário | Disparado por |
|---|---|---|
| `animal-posted` | Grupo regional | `AnimalPostedIntegrationEventHandler` |
| `animal-found` | Grupo regional | `AnimalFoundIntegrationEventHandler` |
| `animal-deleted` | Grupo regional | `AnimalPostDeletedIntegrationEventHandler` |
| `new-notification` | Usuário específico | `AnimalTipSentIntegrationEventHandler` |

**Frontend (`useSignalR.ts`):** composable com estado singleton por instância de app.
Mantém um `Set` de regiões ativas e reingressa automaticamente em todos os grupos após
reconexão (`onreconnected`). O ref count garante que a conexão seja encerrada apenas
quando o último componente que depende do composable for desmontado.

---

## Testes (backend)

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

Abra `backend/TestResults/CoverageReportFiltered/index.html` para o relatório completo.
