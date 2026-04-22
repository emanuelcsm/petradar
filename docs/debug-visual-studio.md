# Rodando e Depurando via Visual Studio (F5)

Este guia descreve como executar o backend diretamente pelo Visual Studio com F5,
conectando-se ao mesmo MongoDB e Redis usados via Docker — **sem alterar o
`docker-compose.yml`**.

---

## Por que não basta usar o docker-compose normal?

O `docker-compose.yml` sobe `mongo` e `redis` **sem expor portas para o host**:

```yaml
mongo:
  image: mongo:8
  # sem "ports:" → inacessível fora da rede Docker

redis:
  image: redis:7-alpine
  # sem "ports:" → inacessível fora da rede Docker
```

O Visual Studio roda no Windows (fora da rede Docker), então `localhost:27017` e
`localhost:6379` não alcançam esses containers. A solução é subir o Mongo e o Redis
em containers **separados, com porta exposta**, exclusivamente para desenvolvimento local.

---

## Pré-requisitos

- Docker Desktop instalado e em execução
- .NET SDK compatível com o projeto (`global.json` define a versão exata)
- Visual Studio 2022 com a carga de trabalho **ASP.NET e desenvolvimento Web**

---

## Passo 1 — Subir MongoDB e Redis para dev local

Execute os dois comandos no terminal (uma única vez; os containers sobrevivem a
reinicializações do Docker Desktop se não forem removidos):

```bash
# MongoDB sem autenticação — porta padrão exposta no host
docker run -d -p 27017:27017 --name petradar-mongo mongo:8

# Redis — porta padrão exposta no host
docker run -d -p 6379:6379 --name petradar-redis redis:7-alpine
```

Para verificar que estão no ar:

```bash
docker ps --filter "name=petradar-mongo" --filter "name=petradar-redis"
```

> **Nota:** esses containers são independentes do `docker-compose.yml`. Você pode
> subir o stack completo via compose normalmente para testar o ambiente de produção;
> os dois comandos acima são apenas para o fluxo de debug local.

### Reiniciando após uma reinicialização do Docker Desktop

Se o Docker Desktop reiniciou e os containers pararam:

```bash
docker start petradar-mongo petradar-redis
```

---

## Passo 2 — Verificar o appsettings.Development.json

O arquivo `backend/src/PetRadar.API/appsettings.Development.json` já está configurado
para apontar para `localhost`:

```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "petradar"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "Jwt": {
    "Secret": "dev-only-secret-key-must-be-at-least-32-chars!",
    "Issuer": "PetRadar",
    "Audience": "PetRadar",
    "ExpiresInMinutes": 1440
  }
}
```

Nenhuma alteração é necessária. O `launchSettings.json` já define
`ASPNETCORE_ENVIRONMENT=Development`, então esse arquivo será carregado
automaticamente ao pressionar F5.

> **Atenção:** o `Jwt__Secret` presente neste arquivo é exclusivo para ambiente de
> desenvolvimento. Nunca use esse valor em produção.

---

## Passo 3 — Rodar com F5 no Visual Studio

1. Abra a solução `backend/PetRadar.slnx` no Visual Studio.
2. Defina `PetRadar.API` como projeto de inicialização (clique com o botão direito →
   *Set as Startup Project*).
3. Selecione o perfil `http` ou `https` na barra de debug.
4. Pressione **F5**.

A API estará disponível em:

| Perfil  | URL                                               |
|---------|---------------------------------------------------|
| `http`  | `http://localhost:5268`                           |
| `https` | `https://localhost:7127` / `http://localhost:5268`|

---

## Passo 4 — Testar a conexão

Use o arquivo `PetRadar.API.http` (na raiz de `PetRadar.API/`) para disparar
requisições diretamente do Visual Studio via HTTP Client, ou importe no Postman/Insomnia.

Você também pode verificar a saída do console: se a conexão com o MongoDB falhar,
será logado um erro de `TimeoutException` na inicialização.

---

## Resumo do fluxo

```
┌──────────────────────────────────────┐
│  Windows Host                        │
│                                      │
│  Visual Studio (F5)                  │
│  └─ PetRadar.API                     │
│      ├─ localhost:27017  ──────────► │──► container petradar-mongo
│      └─ localhost:6379   ──────────► │──► container petradar-redis
└──────────────────────────────────────┘
```

O `docker-compose.yml` permanece **inalterado** e continua funcional para o ambiente
completo (`docker-compose up`).
