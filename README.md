# EventHub

Plataforma de eventos baseada em microserviços, ASP.NET Core 10, Angular 22,
PostgreSQL, RabbitMQ, YARP, SignalR e integração com Asaas.

## Executar com Docker Compose

Pré-requisitos:

- Docker Desktop com Docker Compose;
- portas `4200`, `5050`, `5432`, `5672` e `15672` disponíveis.

Crie o arquivo local de variáveis:

```powershell
Copy-Item .env.example .env
```

Edite o `.env` e defina, no mínimo, valores próprios para:

```text
POSTGRES_PASSWORD
RABBITMQ_PASSWORD
JWT_SECRET
```

Para testar pagamentos e e-mails, preencha também as variáveis do Asaas e
Mailtrap. O arquivo `.env` está no `.gitignore` e não deve ser versionado.

Suba toda a aplicação:

```powershell
docker compose up --build -d
```

Na primeira inicialização, o PostgreSQL cria um banco por microserviço e cada
API aplica suas migrations. Acompanhe a inicialização com:

```powershell
docker compose ps
docker compose logs -f api-gateway
```

Endereços principais:

| Recurso | URL |
| --- | --- |
| Frontend | http://localhost:4200 |
| API Gateway | http://localhost:5050 |
| Saúde do Gateway | http://localhost:5050/health |
| Saúde dos serviços | http://localhost:5050/health/services |
| RabbitMQ Management | http://localhost:15672 |

As APIs também ficam expostas individualmente nas portas usadas pelos perfis
locais: Identity `5165`, Events `5228`, Ticketing `61577`, Orders `52576`,
Payments `53576`, Admission `50576` e Notifications `54576`.

Para encerrar:

```powershell
docker compose down
```

Para apagar também bancos e filas e começar do zero:

```powershell
docker compose down --volumes
```

O último comando remove permanentemente os dados locais dos volumes Docker.

## Webhook do Asaas

O endpoint público continua sendo:

```text
POST /api/webhooks/asaas
```

Ao usar ngrok, encaminhe para o Gateway:

```powershell
ngrok http 5050
```

Cadastre no Asaas a URL gerada, seguida de `/api/webhooks/asaas`, e use no
Asaas o mesmo token informado em `ASAAS_WEBHOOK_TOKEN`.
