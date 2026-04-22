# Redis — Instalação Local e Configuração Docker

---

## Instalação Local (Windows)

Redis não tem suporte nativo oficial no Windows. Há duas opções:

---

### Opção A — WSL2 (Recomendada)

WSL2 roda um kernel Linux real dentro do Windows — Redis funciona sem diferença alguma.

#### 1. Instalar WSL2

No PowerShell como Administrador:

```powershell
wsl --install
```

Reinicie o computador quando solicitado. O Ubuntu será instalado por padrão.

Verifique:

```powershell
wsl --list --verbose
```

#### 2. Instalar Redis dentro do Ubuntu (WSL2)

Abra o terminal Ubuntu e execute:

```bash
sudo apt update
sudo apt install redis-server -y
```

#### 3. Iniciar o servidor Redis

```bash
sudo service redis-server start
```

#### 4. Verificar

```bash
redis-cli ping
# Esperado: PONG
```

#### Iniciar automaticamente no boot do WSL2

Habilite o systemd no WSL2 (recomendado, disponível desde WSL 0.67.6):

```bash
# Dentro do Ubuntu (WSL2)
sudo nano /etc/wsl.conf
```

Adicione o conteúdo abaixo e salve:

```ini
[boot]
systemd=true
```

Reinicie o WSL2 no PowerShell:

```powershell
wsl --shutdown
```

Abra o Ubuntu novamente e habilite o Redis:

```bash
sudo systemctl enable redis-server
sudo systemctl start redis-server
```

A partir daí o Redis inicia automaticamente com o WSL2.

---

### Opção B — Memurai (Redis nativo para Windows)

Memurai é um Redis-compatible server que roda como serviço Windows sem WSL2.

1. Acesse [memurai.com/get-memurai](https://www.memurai.com/get-memurai)
2. Baixe e execute o instalador (registra como serviço Windows automaticamente)
3. A porta padrão é **6379**

Verifique (com `redis-cli` ou `memurai-cli`):

```powershell
memurai-cli ping
# Esperado: PONG
```

> A edição Developer é gratuita e adequada para desenvolvimento local.

---

## String de Conexão Local

```
localhost:6379
```

---

## Configuração no docker-compose

O `docker-compose.yml` na raiz do repositório já inclui o serviço `redis`:

```yaml
services:
  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"   # expõe localmente para debug com redis-cli
```

> `redis:7-alpine` — imagem mínima (~10 MB), sem configuração extra necessária para desenvolvimento.

### Variáveis de ambiente do backend no compose

```yaml
backend:
  environment:
    - Redis__ConnectionString=redis:6379
```

> No Docker, o hostname é `redis` (nome do serviço), não `localhost`.

---

## Usos do Redis neste projeto

| Uso                         | Justificativa                                          |
|-----------------------------|--------------------------------------------------------|
| Cache de listagens por região | Evita queries geoespaciais repetidas (TTL ~30s)      |
| Backplane do SignalR         | Suporte a múltiplas instâncias do backend              |
| Cache de JWT revogados       | Logout seguro sem blacklist no MongoDB                 |

---

## Inspecionar dados localmente

Conectar ao Redis local via CLI:

```bash
redis-cli
```

Comandos úteis:

```
KEYS *              # lista todas as chaves (cuidado em produção)
GET <chave>         # lê o valor de uma chave
TTL <chave>         # tempo restante de expiração em segundos
FLUSHDB             # limpa o banco atual (dev only)
```

Conectar ao container Redis:

```bash
docker exec -it <nome_do_container_redis> redis-cli
```
