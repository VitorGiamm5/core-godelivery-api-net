# Go Core Delivery

API para gerenciar motocicletas, aluguéis, entregadores e imagens de CNH

## Tecnologias

- .NET 10 LTS, Entity Framework Core 10, PostgreSQL, RabbitMQ, MinIO
- CQRS com Mediator, validação de documentos brasileiros e xUnit
- JWT Bearer para validar tokens emitidos por um provedor OAuth 2.0 / OpenID Connect

## Configuração local

Requisitos: .NET SDK 10 e Docker Compose

1. Copie `.env.example` para `.env`
2. Gere valores diferentes para `POSTGRES_PASSWORD`, `RABBITMQ_DEFAULT_PASS`, `MINIO_ROOT_PASSWORD` e `KEYCLOAK_ADMIN_PASSWORD`; por exemplo, no PowerShell:

   ```powershell
   $bytes = [byte[]]::new(32)
   [System.Security.Cryptography.RandomNumberGenerator]::Fill($bytes)
   [Convert]::ToBase64String($bytes)
   ```

   Cole cada valor em seu campo no `.env`. O exemplo já configura o emissor e a audiência do Keycloak local
3. Inicie a infraestrutura e a API:

   ```bash
   ./local-infrastructure/start.sh
   ```

4. O console do Keycloak fica em `http://keycloak.localhost:8080`. Crie um usuário no realm `godelivery` e use o client `coregodelivery-api` para obter tokens
5. A documentação Swagger fica em `http://localhost:9005/swagger`. Use **Authorize** e informe um access token do realm `godelivery`

Para iniciar a API fora do Docker, configure `ConnectionStrings__Postgres`, `RabbitMQ__Username`, `RabbitMQ__Password`, `MinIOSettings__AccessKey`, `MinIOSettings__SecretKey`, `Jwt__Authority` e `Jwt__Audience` como variáveis de ambiente ou User Secrets

## Autenticação

Todos os endpoints da API exigem um JWT válido. A API valida assinatura, emissor, audiência e expiração. O Keycloak local atende somente ao ambiente Development; ambientes compartilhados e produção devem usar um emissor OAuth/OIDC com HTTPS. A API não emite tokens nem mantém contas de usuário

O Compose local importa o realm `godelivery` de `deploy/keycloak/realm-godelivery.json` e configura `JWT_AUTHORITY` e `JWT_AUDIENCE` para o client da API. `KEYCLOAK_HOSTNAME` deve corresponder ao host e à porta usados por `JWT_AUTHORITY`

## Valores por ambiente e constantes de domínio

Credenciais e endereços dependentes do ambiente ficam fora do código versionado. As variáveis de execução incluem conexão PostgreSQL, credenciais e filas RabbitMQ, endpoint e credenciais MinIO, porta Kestrel, parâmetros do emissor JWT e timeout do banco. O arquivo `.env` é local e ignorado pelo Git; use um gerenciador de segredos em ambientes compartilhados

Regras do domínio permanecem no código: preço e multa de aluguel, duração dos planos, limites de validação, paginação, padrões de placa/arquivo e mensagens de negócio. Elas afetam o comportamento do produto, portanto não são configurações de infraestrutura

## Migrações de banco

No ambiente Development, a API aplica migrações pendentes ao iniciar. Em outros ambientes, aplique-as em uma etapa controlada de publicação:

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update --project src/CoreGoDelivery.Infrastructure --startup-project src/CoreGoDelivery.Api
```

Para criar uma nova migração:

```bash
dotnet ef migrations add NomeDaMigracao --project src/CoreGoDelivery.Infrastructure --startup-project src/CoreGoDelivery.Api
```

## Portas locais

| Serviço | Porta local |
| --- | ---: |
| PostgreSQL | 9000 (configurável) |
| RabbitMQ AMQP | 9001 (configurável) |
| RabbitMQ Management | 9002 (configurável) |
| MinIO API | 9003 (configurável) |
| MinIO Console | 9004 (configurável) |
| Keycloak | 8080 (configurável) |
| API | 9005 (configurável) |

As portas publicadas pelo Compose ficam limitadas a `127.0.0.1` por padrão (`DOCKER_BIND_ADDRESS`), evitando expor banco, broker, storage e API para a rede local

## Stack Docker local

O Docker Compose usa o projeto `local-godelivery-core`. PostgreSQL, RabbitMQ, MinIO, Keycloak, API pertencem a essa stack e compartilham a rede `local-godelivery-core-network`. Os containers e volumes persistentes têm nomes correspondentes:

| Container | Volume persistente |
| --- | --- |
| `local-godelivery-core-postgres` | `local-godelivery-core-postgres` |
| `local-godelivery-core-rabbitmq` | `local-godelivery-core-rabbitmq` |
| `local-godelivery-core-minio` | `local-godelivery-core-minio` |
| `local-godelivery-core-keycloak` | `local-godelivery-core-keycloak` |
| `local-godelivery-core-api` | — |


Os scripts em `local-infrastructure` selecionam explicitamente esse nome de projeto. A troca dos nomes de projeto e volumes não transfere dados da configuração anterior; faça backup e migração dos volumes antigos antes de removê-los

### Versões da stack

| Serviço | Versão fixada |
| --- | --- |
| PostgreSQL | `18.6` |
| RabbitMQ | `4.4.0-management` |
| MinIO Community | `RELEASE.2025-10-15T17-29-55Z`, compilado do commit fixado no `deploy/minio.Dockerfile` |
| MinIO build/runtime | Go `1.27.3`, Debian `13.7-slim`, executado como UID `10001` |
| Keycloak | `26.8.1` |
| API | .NET SDK `10.0.403`, ASP.NET Runtime `10.0.14` |

O volume local desta máquina foi migrado de PostgreSQL 16 para 18 com dump e restore. Para um volume PostgreSQL 16 existente em outra máquina, faça essa migração antes de iniciar a imagem 18; os diretórios de dados entre versões principais não são intercambiáveis. O Compose 18 falha explicitamente ao encontrar o formato antigo, preservando o volume para a migração

## Atualização de pacotes

As versões diretas ficam centralizadas em `Directory.Packages.props`; os arquivos `packages.lock.json` registram também as dependências transitivas. Em CI, valide a resolução reproduzível com `dotnet restore CoreGoDelivery.sln --locked-mode`. MediatR foi substituído por Mediator 3.0.2, com licença MIT e geração de código; os handlers mantêm o padrão request/handler e usam `ValueTask`

O projeto compila o release comunitário MinIO fixado em `deploy/minio.Dockerfile`, porque o upstream não fornece mais imagens comunitárias pré-compiladas. O container usa Debian estável e usuário não-root. O repositório comunitário foi arquivado após esse release; para ambientes compartilhados ou produção, configure um serviço S3 com suporte ativo por `MinIOSettings__Endpoint` e `MinIOSettings__Port`

## Segurança

Credenciais que estavam nos arquivos antigos foram removidas da configuração atual. Como esses valores já fizeram parte do histórico do Git, troque as senhas correspondentes nos serviços e nos ambientes onde tenham sido reutilizadas; remover valores do checkout não apaga cópias do histórico
