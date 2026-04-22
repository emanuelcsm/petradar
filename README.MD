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
   Consulte [`backend/docs/setup-env.md`](backend/docs/setup-env.md) para instruções
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

## Variáveis de ambiente

As variáveis são lidas do arquivo `.env` na raiz do projeto.
O arquivo `.env` nunca é commitado — use `.env.example` como referência.

| Variável                      | Descrição                                    |
|-------------------------------|----------------------------------------------|
| `JWT_SECRET`                  | Segredo de assinatura dos tokens JWT (≥32 chars) |
| `MONGO_INITDB_ROOT_USERNAME`  | Usuário root do MongoDB                      |
| `MONGO_INITDB_ROOT_PASSWORD`  | Senha do usuário root do MongoDB             |

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

Detalhes completos em [`../architecture.md`](../architecture.md).
