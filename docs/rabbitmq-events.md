# RabbitMQ Events Integration

## Visão Geral

O sistema Cash Flow agora integra com RabbitMQ para publicação de eventos de domínio. Quando uma transação é criada (crédito ou débito), um evento `TransactionCreatedEvent` é automaticamente publicado no RabbitMQ.

## Configuração

### Docker Compose
O RabbitMQ está configurado no `docker-compose.yml` com:
- **Porta AMQP**: 5672
- **Porta Management UI**: 15672
- **Usuário**: admin
- **Senha**: password123

### Configurações da Aplicação
As configurações do RabbitMQ estão nos arquivos `appsettings.json` e `appsettings.Development.json`:

```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "admin",
    "Password": "password123",
    "VirtualHost": "/",
    "ExchangeName": "cashflow.events",
    "QueueName": "transaction.created"
  }
}
```

## Estrutura dos Eventos

### TransactionCreatedEvent
```csharp
public record TransactionCreatedEvent(
    Guid Id,
    Guid CustomerId,
    string Type,
    DateTime ReferenceDate,
    decimal Value);
```

### Exchange e Queue
- **Exchange**: `cashflow.events` (Tipo: Topic)
- **Queue**: `transaction.created`
- **Routing Key**: `transaction.created`

## Como Executar

1. **Iniciar os serviços**:
   ```bash
   docker-compose up -d
   ```

2. **Executar a aplicação**:
   ```bash
   cd src/CashFlow.Transaction.Api
   dotnet run
   ```

3. **Acessar o Management UI**:
   - URL: http://localhost:15672
   - Usuário: admin
   - Senha: password123

## Monitoramento

### RabbitMQ Management UI
- Acesse http://localhost:15672
- Navegue para "Queues" para ver a fila `transaction.created`
- Navegue para "Exchanges" para ver o exchange `cashflow.events`

### Logs da Aplicação
A aplicação registra logs quando eventos são publicados:
```
[Information] Event published successfully. RoutingKey: transaction.created, EventType: TransactionCreatedEvent
```

## Testando a Integração

1. **Criar uma transação de crédito**:
   ```bash
   curl -X POST "https://localhost:7000/transactions/credit" \
        -H "Content-Type: application/json" \
        -d '{
          "customerId": "123e4567-e89b-12d3-a456-426614174000",
          "value": 100.50,
          "referenceDate": "2024-01-15T10:30:00Z"
        }'
   ```

2. **Criar uma transação de débito**:
   ```bash
   curl -X POST "https://localhost:7000/transactions/debit" \
        -H "Content-Type: application/json" \
        -d '{
          "customerId": "123e4567-e89b-12d3-a456-426614174000",
          "value": 50.25,
          "referenceDate": "2024-01-15T11:00:00Z"
        }'
   ```

3. **Verificar no RabbitMQ Management UI**:
   - Acesse a fila `transaction.created`
   - Verifique se as mensagens foram recebidas
   - Clique em "Get Messages" para ver o conteúdo dos eventos

## Tratamento de Erros

- Se a publicação do evento falhar, um erro será logado mas não afetará a criação da transação
- A publicação é feita de forma assíncrona para não impactar a performance
- Eventos são publicados com `Persistent = true` para garantir durabilidade

## Próximos Passos

- Implementar consumidores para processar os eventos
- Adicionar retry policy para falhas de publicação
- Implementar dead letter queue para mensagens com erro
- Adicionar métricas de monitoramento
