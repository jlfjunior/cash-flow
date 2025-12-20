# Métricas CK (Chidamber e Kemerer) - Exemplos do Projeto Cash Flow

Este documento apresenta exemplos de código do projeto Cash Flow que ilustram as 6 métricas CK (Chidamber e Kemerer), utilizadas para medir a qualidade e complexidade do design orientado a objetos.

Para cada métrica, são apresentados um exemplo **bom** (valor ideal) e um exemplo **ruim** (valor acima do ideal), facilitando a comparação e compreensão.

---

## 1. WMC (Weighted Methods per Class) - Peso dos Métodos por Classe

**Definição**: Mede a complexidade de uma classe através do número e complexidade de seus métodos. Os autores sugerem duas formas de cálculo:

1. **Complexidade Ciclomática**: Soma da complexidade ciclomática de todos os métodos da classe
2. **Contagem Simples**: Soma de 1 ponto para cada método público e privado da classe

### ✅ Exemplo BOM: Classe `Customer` (WMC Baixo)

**Arquivo**: `src/CashFlow.Customers.Domain/Entities/Customer.cs`

```csharp
public class Customer
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; }

    public Customer(string fullName)
    {
        Id = Guid.CreateVersion7();
        FullName = fullName;
    }

    public void WithFullName(string fullName) => FullName = fullName;
}
```

**Cálculo do WMC**:
- **Complexidade Ciclomática**: Construtor (1) + `WithFullName()` (1) = **2**
- **Contagem Simples**: 2 métodos = **2**

**Análise**: **WMC = 2** indica baixa complexidade. A classe possui métodos simples e lineares, sem condicionais complexas. Este é um valor ideal para entidades de domínio.

### ❌ Exemplo RUIM: Classe `Account` (WMC Alto)

**Arquivo**: `src/CashFlow.Transactions.Domain/Entities/Account.cs`

```csharp
public class Account : Entity
{
    public Guid Id { get; private set; }
    public int Version { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal Balance { get; private set; }
    public ICollection<Transaction>? Transactions { get; set; }

    public Account(Guid customerId) { ... }
    
    public void AddDebit(decimal amount)
    {
        ProcessDebit(new WithdrawTransaction(Id, amount));
    }
    
    public void AddCredit(decimal amount)
    {
        ProcessCredit(new DepositTransaction(Id, amount));
    }
    
    public void PayBill(decimal amount)
    {
        ProcessDebit(new BillPaymentTransaction(Id, amount));
    }

    public void AddTransaction(string direction, decimal amount)
    {
        if (string.Equals(direction, "Credit", StringComparison.OrdinalIgnoreCase))
        {
            AddCredit(amount);
        }
        else if (string.Equals(direction, "Debit", StringComparison.OrdinalIgnoreCase))
        {
            AddDebit(amount);
        }
        else
        {
            throw new ArgumentException(...);
        }
    }

    private void ProcessCredit(Transaction transaction) { ... }
    private void ProcessDebit(Transaction transaction) { ... }
}
```

**Cálculo do WMC**:
- **Complexidade Ciclomática**: 
  - Construtor (1) 
  - `AddDebit()` (1) 
  - `AddCredit()` (1) 
  - `PayBill()` (1) 
  - `AddTransaction()` (3 - if/else if/else)
  - `ProcessCredit()` (1)
  - `ProcessDebit()` (2 - if condicional)
  - **Total = 10**
- **Contagem Simples**: 7 métodos = **7**

**Análise**: **WMC = 10** (complexidade ciclomática) está dentro do aceitável (< 20), mas é **5x maior** que o exemplo bom. A diferença mostra que métodos com condicionais (`if`, `else if`, `else`) aumentam significativamente a complexidade. O método `AddTransaction` contribui com 3 pontos devido à estrutura condicional. Valores acima de 20 indicariam necessidade de refatoração.

---

## 2. DIT (Depth of Inheritance Tree) - Profundidade da Árvore de Herança

**Definição**: Mede o número de níveis na hierarquia de herança, desde a classe atual até a raiz da árvore de herança.

### ✅ Exemplo BOM: Classe `Customer` (DIT = 0)

**Arquivo**: `src/CashFlow.Customers.Domain/Entities/Customer.cs`

```csharp
public class Customer
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; }

    public Customer(string fullName) { ... }
    public void WithFullName(string fullName) => FullName = fullName;
}
```

**Cálculo do DIT**:
- `Customer` não herda de nenhuma classe

**DIT = 0**

**Análise**: **DIT = 0** indica que a classe não usa herança. Para entidades simples, isso é ideal, evitando complexidade desnecessária da hierarquia.

### ❌ Exemplo RUIM: Classes de Transação (DIT = 2)

**Arquivos**: 
- `src/CashFlow.Transactions.Domain/Entities/Transaction.cs`
- `src/CashFlow.Transactions.Domain/Entities/DepositTransaction.cs`
- `src/CashFlow.Transactions.Domain/Entities/WithdrawTransaction.cs`
- `src/CashFlow.Transactions.Domain/Entities/BillPaymentTransaction.cs`

```csharp
// Transaction.cs
public abstract class Transaction : Entity
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public Direction Direction { get; private set; }
    public TransactionType TransactionType { get; protected set; }
    public DateTime ReferenceDate { get; private set; }
    public decimal Value { get; private set; }
    
    protected Transaction() { }

    protected Transaction(Guid accountId, Direction direction, decimal value)
    {
        if (value.IsLessThanOrEqualTo(decimal.Zero)) 
            throw new ArgumentException("Transaction value must be greater than zero", nameof(value));

        Id = Guid.CreateVersion7();
        AccountId = accountId;
        Direction = direction;
        Value = value;
        ReferenceDate = DateTime.UtcNow;
    }
}

// DepositTransaction.cs
public class DepositTransaction : Transaction
{
    protected DepositTransaction() { }

    public DepositTransaction(Guid accountId, decimal value) 
        : base(accountId, Direction.Credit, value)
    {
        TransactionType = TransactionType.Deposit;
    }
}

// WithdrawTransaction.cs
public class WithdrawTransaction : Transaction
{
    protected WithdrawTransaction() { }

    public WithdrawTransaction(Guid accountId, decimal value) 
        : base(accountId, Direction.Debit, value)
    {
        TransactionType = TransactionType.Withdraw;
    }
}

// BillPaymentTransaction.cs
public class BillPaymentTransaction : Transaction
{
    protected BillPaymentTransaction() { }

    public BillPaymentTransaction(Guid accountId, decimal value) 
        : base(accountId, Direction.Debit, value)
    {
        TransactionType = TransactionType.BillPayment;
    }
}
```

**Hierarquia**:
```
Entity (DIT = 0)
  └── Transaction (DIT = 1)
      └── DepositTransaction (DIT = 2)
      └── WithdrawTransaction (DIT = 2)
      └── BillPaymentTransaction (DIT = 2)
```

**Cálculo do DIT**:
- `Entity`: DIT = 0 (classe raiz)
- `Transaction`: DIT = 1 (herda de `Entity`)
- `DepositTransaction`, `WithdrawTransaction`, `BillPaymentTransaction`: DIT = 2 (herdam de `Transaction`)

**DIT = 2**

**Análise**: **DIT = 2** ainda está dentro do ideal (1-4), mas representa uma hierarquia mais profunda que `Account` (DIT = 1). A hierarquia de 3 níveis (`Entity` → `Transaction` → subclasses) é justificada pelo uso de herança para especialização de tipos de transação, mas valores acima de 4 indicariam hierarquias muito profundas, dificultando manutenção e compreensão.

---

## 3. NOC (Number of Children) - Número de Filhos

**Definição**: Conta o número de classes filhas (subclasses) diretas de uma classe.

### ✅ Exemplo BOM: Classe `Entity` (NOC = 2)

**Arquivo**: `src/CashFlow.Lib.Sharable/Entity.cs`

```csharp
public abstract class Entity
{
    private IList<IEvent> _events = new List<IEvent>();

    public void AddEvent(IEvent @event) => _events.Add(@event);
    public void ClearEvents() => _events.Clear();
}
```

**Classes Filhas**:
1. `Account` (`src/CashFlow.Transactions.Domain/Entities/Account.cs`)
2. `Transaction` (`src/CashFlow.Transactions.Domain/Entities/Transaction.cs`)

```csharp
// Account.cs
public class Account : Entity
{
    public Guid Id { get; private set; }
    public int Version { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal Balance { get; private set; }
    public ICollection<Transaction>? Transactions { get; set; }

    public Account(Guid customerId)
    {
        Id = Guid.NewGuid();
        Version = 1;
        CustomerId = customerId;
        Balance = decimal.Zero;
    }
    
    // ... outros métodos
}

// Transaction.cs
public abstract class Transaction : Entity
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public Direction Direction { get; private set; }
    public TransactionType TransactionType { get; protected set; }
    public DateTime ReferenceDate { get; private set; }
    public decimal Value { get; private set; }
    
    protected Transaction() { }

    protected Transaction(Guid accountId, Direction direction, decimal value)
    {
        if (value.IsLessThanOrEqualTo(decimal.Zero)) 
            throw new ArgumentException("Transaction value must be greater than zero", nameof(value));

        Id = Guid.CreateVersion7();
        AccountId = accountId;
        Direction = direction;
        Value = value;
        ReferenceDate = DateTime.UtcNow;
    }
}
```

**Cálculo do NOC**:
- Classes que herdam diretamente de `Entity`: `Account`, `Transaction`

**NOC = 2**

**Análise**: **NOC = 2** está dentro do ideal (1-5). Indica reutilização moderada sem excessiva especialização. Valores muito altos (acima de 5) sugeririam que a classe base está sendo usada de forma excessiva, possivelmente violando o princípio de responsabilidade única.

### ❌ Exemplo RUIM: Classe `Transaction` (NOC = 3)

**Arquivo**: `src/CashFlow.Transactions.Domain/Entities/Transaction.cs`

```csharp
public abstract class Transaction : Entity
{
    // Classe base abstrata para transações
}

public class DepositTransaction : Transaction { }

public class WithdrawTransaction : Transaction { }

public class BillPaymentTransaction : Transaction { }
```
**Classes Filhas**:
1. `DepositTransaction` (`src/CashFlow.Transactions.Domain/Entities/DepositTransaction.cs`)
2. `WithdrawTransaction` (`src/CashFlow.Transactions.Domain/Entities/WithdrawTransaction.cs`)
3. `BillPaymentTransaction` (`src/CashFlow.Transactions.Domain/Entities/BillPaymentTransaction.cs`)

**Cálculo do NOC**:
- Classes que herdam diretamente de `Transaction`: `DepositTransaction`, `WithdrawTransaction`, `BillPaymentTransaction`

**NOC = 3**

**Análise**: **NOC = 3** está dentro do ideal (1-5), mas é maior que o exemplo bom. Indica que a classe `Transaction` está sendo usada para especialização de tipos de transação. Este valor ainda é aceitável, mas se crescer acima de 5, indicaria necessidade de revisão do design para evitar excessiva especialização.

---

## 4. CBO (Coupling Between Objects) - Acoplamento entre Objetos

**Definição**: Mede o número de classes às quais uma classe está acoplada (usa ou é usada por).

### ✅ Exemplo BOM: Classe `Customer` (CBO = 0)

**Arquivo**: `src/CashFlow.Customers.Domain/Entities/Customer.cs`

```csharp
public class Customer
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; }

    public Customer(string fullName) { ... }
    public void WithFullName(string fullName) => FullName = fullName;
}
```

**Cálculo do CBO**:
- A classe `Customer` não referencia outras classes do domínio
- Usa apenas tipos primitivos (`Guid`, `string`)

**CBO = 0**

**Análise**: **CBO = 0** indica **zero acoplamento** com outras classes. Isso resulta em alta reutilização e facilidade de teste, características ideais para entidades de domínio.

### ❌ Exemplo RUIM: Classe `PayBill` (CBO = 9)

**Arquivo**: `src/CashFlow.Transactions.Application/PayBill.cs`

```csharp
using CashFlow.Lib.EventBus;
using CashFlow.Transactions.Domain.Repositories;
using CashFlow.Transactions.Domain.Exceptions;
using CashFlow.Transactions.Application.Requests;
using CashFlow.Transactions.Application.Responses;
using Microsoft.Extensions.Logging;

public class PayBill(ILogger<PayBill> logger, IRepository accountRepository, IEventBus eventBus)
    : IPayBill
{
    public async Task<AccountResponse> ExecuteAsync(PayBillRequest request, CancellationToken token)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId);
        
        if (account == null)
            throw new NotFoundException($"Account with id {request.AccountId} was not found.");

        account.PayBill(request.Value);
        await accountRepository.UpsertAsync(account, token);
        
        var lastTransaction = account.Transactions.LastOrDefault();
        if (lastTransaction == null)
        {
            logger.LogWarning("No transaction found after PayBill operation. AccountId: {AccountId}", account.Id);
            return new AccountResponse(account.Id);
        }
        
        await eventBus.PublishAsync(lastTransaction, "accounts.update");
        logger.LogInformation("Bill payment processed. AccountId {AccountId}.", account.Id);

        return new AccountResponse(account.Id);
    }
}
```

**Cálculo do CBO**:
Classes às quais `PayBill` está acoplada:
1. `ILogger<PayBill>` (injeção de dependência)
2. `IRepository` (injeção de dependência)
3. `IEventBus` (injeção de dependência)
4. `NotFoundException` (exceção de domínio)
5. `PayBillRequest` (request DTO)
6. `AccountResponse` (response DTO)
7. `Account` (entidade de domínio)
8. `Transaction` (entidade de domínio, através de `account.Transactions`)
9. `IPayBill` (interface implementada)

**CBO = 9**

**Análise**: **CBO = 9** está **muito acima do ideal (< 5)**. Isso indica:
- Alta dependência de outras classes
- Dificuldade de reutilização
- Complexidade elevada para testes (requer muitos mocks)
- Alto risco de propagação de mudanças

Este alto acoplamento é comum em classes de aplicação que orquestram múltiplas dependências, mas deve ser minimizado através de padrões como Dependency Injection e interfaces. O uso de interfaces (`ILogger`, `IRepository`, `IEventBus`, `IPayBill`) ajuda a reduzir o acoplamento concreto, mas ainda resulta em um CBO alto.

---

## 5. RFC (Response For a Class) - Resposta para uma Classe

**Definição**: Soma do número de métodos na classe mais o número de métodos chamados por esses métodos (conjunto de resposta).

### ✅ Exemplo BOM: Classe `Customer` (RFC Baixo)

**Arquivo**: `src/CashFlow.Customers.Domain/Entities/Customer.cs`

```csharp
public class Customer
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; }

    public Customer(string fullName)
    {
        Id = Guid.CreateVersion7();
        FullName = fullName;
    }

    public void WithFullName(string fullName) => FullName = fullName;
}
```

**Cálculo do RFC**:
- Métodos próprios (M): Construtor (1) + `WithFullName()` (1) = 2
- Métodos chamados (R): `Guid.CreateVersion7()` = 1

**RFC = M + R = 2 + 1 = 3**

**Análise**: **RFC = 3** é um valor muito baixo, indicando que a classe tem poucos métodos e chama poucos métodos externos. Isso facilita compreensão, teste e manutenção.

### ❌ Exemplo RUIM: Classe `EventBus` (RFC Alto)

**Arquivo**: `src/CashFlow.Lib.EventBus/EventBus.cs`

```csharp
public class EventBus(ILogger<EventBus> logger, ConnectionFactory factory) : IEventBus
{
    public async Task PublishAsync<T>(T @event, string queueName) where T : class { ... }

    public async Task SubscribeAsync<T>(string queueName, Func<T, Task> handler) where T : class { ... }
}

public async Task PublishAsync<T>(T @event, string queueName) where T : class
{
    await using var connection = await factory.CreateConnectionAsync();
    await using var channel = await connection.CreateChannelAsync();
    
    var message = JsonSerializer.Serialize(@event);
    var body = Encoding.UTF8.GetBytes(message);
    
    await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, body: body);
        
    logger.LogInformation($"Publishing domain event: {queueName} - {message}");
}

public async Task SubscribeAsync<T>(string queueName, Func<T, Task> handler) where T : class
{
    var connection = await factory.CreateConnectionAsync();
    var channel = await connection.CreateChannelAsync();

    await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false,arguments: null);

    var consumer = new AsyncEventingBasicConsumer(channel);
    consumer.ReceivedAsync += async (model, ea) =>
    {
        try
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var _jsonOptions = new JsonSerializerOptions{ PropertyNameCaseInsensitive = true };
            var message = JsonSerializer.Deserialize<T>(json, _jsonOptions);

            if (message != null)
            {
                await handler(message); 
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing message from queue {QueueName}", queueName);
        }
    };

    await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);
        
    logger.LogInformation("Started consuming from queue: {QueueName}", queueName);
}
```

**Cálculo do RFC**:
- Métodos próprios (M): Construtor (1) + `PublishAsync()` (1) + `SubscribeAsync()` (1) = 3
- Métodos chamados (R):
  - **Construtor**: nenhum método externo chamado = 0
  - **PublishAsync()**: 
    - `CreateConnectionAsync()` (1)
    - `CreateChannelAsync()` (1)
    - `Serialize()` (1)
    - `GetBytes()` (1)
    - `QueueDeclareAsync()` (1)
    - `BasicPublishAsync()` (1)
    - `LogInformation()` (1)
    - **Subtotal = 7**
  - **SubscribeAsync()**: 
    - `CreateConnectionAsync()` (1)
    - `CreateChannelAsync()` (1)
    - `QueueDeclareAsync()` (1)
    - `new AsyncEventingBasicConsumer()` (1)
    - `BasicConsumeAsync()` (1)
    - `LogInformation()` (1)
    - `LogError()` (1)
    - `ToArray()` (1)
    - `GetString()` (1)
    - `Deserialize()` (1)
    - `handler()` (1)
    - **Subtotal = 11**

**RFC = M + R = 3 + (0 + 7 + 11) = 21**

**Análise**: **RFC = 21** está dentro do aceitável (< 50), mas é **7x maior** que o exemplo bom. O valor reflete o grande número de métodos externos chamados (RabbitMQ, JSON serialization, logging). Para classes de infraestrutura que integram com sistemas externos, valores mais altos são esperados, mas devem ser monitorados. Valores acima de 50 indicariam necessidade de refatoração para reduzir a complexidade.

---

## 6. LCOM (Lack of Cohesion of Methods) - Falta de Coesão dos Métodos

**Definição**: Mede a falta de coesão entre métodos de uma classe. Classes com baixa coesão têm métodos que não compartilham variáveis de instância.

### ⚠️ Exemplo com Falta de Coesão: Classe `Account` (LCOM = 1 - Coesão Moderada)

**Arquivo**: `src/CashFlow.Transactions.Domain/Entities/Account.cs`

```csharp
public class Account : Entity
{
    public Guid Id { get; private set; }
    public int Version { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal Balance { get; private set; }
    public ICollection<Transaction>? Transactions { get; set; }

    public Account(Guid customerId)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        Balance = decimal.Zero;
    }
    
    public void AddDebit(decimal amount) // Usa: Id, Transactions, Balance (através de ProcessDebit)
    {
        ProcessDebit(new WithdrawTransaction(Id, amount)); 
    }
    
    public void AddCredit(decimal amount)  // Usa: Id, Transactions, Balance (através de ProcessCredit)
    {
        ProcessCredit(new DepositTransaction(Id, amount));
    }
    
    public void PayBill(decimal amount) // Usa: Id, Transactions, Balance (através de ProcessDebit)
    {
        ProcessDebit(new BillPaymentTransaction(Id, amount));
    }

    public void AddTransaction(string direction, decimal amount) // Usa: Id (através de AddCredit/AddDebit)
    {
        if (string.Equals(direction, "Credit", StringComparison.OrdinalIgnoreCase))
        {
            AddCredit(amount);
        }
        else if (string.Equals(direction, "Debit", StringComparison.OrdinalIgnoreCase))
        {
            AddDebit(amount);
        }
        else
        {
            throw new ArgumentException($"Invalid direction: {direction}. Must be 'Credit' or 'Debit'.", nameof(direction));
        }
    }

    private void ProcessCredit(Transaction transaction) // Usa: Transactions, Balance, Id
    {
        Transactions ??= new List<Transaction>();
        Transactions.Add(transaction);
        Balance += transaction.Value;
        // ... eventos ...
    }

    private void ProcessDebit(Transaction transaction) // Usa: Balance, Transactions, Id
    {
        if (Balance < transaction.Value) 
            throw new InvalidOperationException("Debit amount can't be less than current balance");
        
        Transactions ??= new List<Transaction>();
        Transactions.Add(transaction);
        Balance -= transaction.Value;
        // ... eventos ...
    }
}
```

**Cálculo do LCOM**:
- Variáveis de instância: `Id`, `Version`, `CustomerId`, `Balance`, `Transactions`
- Análise de uso:
  - Construtor: usa `Id`, `CustomerId`, `Balance` (não usa `Version`)
  - `ProcessDebit()`: usa `Balance`, `Transactions`, `Id`
  - `ProcessCredit()`: usa `Transactions`, `Balance`, `Id`
  - `AddDebit()`: usa `Id`, `Transactions`, `Balance` (através de `ProcessDebit()`)
  - `AddCredit()`: usa `Id`, `Transactions`, `Balance` (através de `ProcessCredit()`)
  - `PayBill()`: usa `Id`, `Transactions`, `Balance` (através de `ProcessDebit()`)
  - `AddTransaction()`: usa `Id` (através de `AddCredit()`/`AddDebit()`)
  - **`Version`**: não é usada por nenhum método

**Problema identificado**: A propriedade `Version` não é utilizada por nenhum método da classe, nem mesmo no construtor. Isso cria um grupo de variáveis (`Version`) que não compartilha uso com os outros métodos, resultando em falta de coesão.

**LCOM = 1** (coesão moderada - há uma variável não compartilhada)

**Análise**: **LCOM = 1** indica **coesão moderada**. Embora a maioria dos métodos compartilhe variáveis (`Id`, `Balance`, `Transactions`), a propriedade `Version` não é utilizada, indicando que pode ser:
- Uma propriedade herdada ou necessária para persistência/versionamento que será gerenciada externamente
- Uma propriedade que deveria ser usada mas não está sendo implementada
- Uma propriedade que pode ser removida se não for necessária

Para melhorar a coesão, considere:
- Remover `Version` se não for necessária
- Implementar lógica que utilize `Version` (ex: incrementar em cada transação para controle de concorrência)
- Mover `Version` para uma classe base se for apenas para persistência

### ❌ Exemplo RUIM: Classe `Usuario` (LCOM Alto - Baixa Coesão)

**Exemplo hipotético demonstrando baixa coesão**

```csharp
public class Usuario
{
    public string Nome;
    public string Email;

    public void ExibirNome()
    {
        Console.WriteLine(Nome);
    }

    public void EnviarEmail(string mensagem)
    {
        Console.WriteLine($"Enviando para {Email}: {mensagem}");
    }

    public void CadastrarProduto()
    {
        Console.WriteLine("Produto cadastrado!");
    }
}
```

**Cálculo do LCOM**:
- Variáveis de instância: `Nome`, `Email`
- Análise de uso:
  - `ExibirNome()`: usa apenas `Nome` (não usa `Email`)
  - `EnviarEmail()`: usa apenas `Email` (não usa `Nome`)
  - `CadastrarProduto()`: **não usa nenhuma variável de instância**

**Problemas identificados**:
1. **Métodos não compartilham variáveis**: Cada método usa variáveis diferentes ou nenhuma
2. **Responsabilidades não relacionadas**: `CadastrarProduto()` não tem relação com `Usuario`
3. **Baixa coesão**: Os métodos não trabalham juntos com as mesmas propriedades

**Grupos de métodos identificados**:
- Grupo 1: `ExibirNome()` → usa `Nome`
- Grupo 2: `EnviarEmail()` → usa `Email`
- Grupo 3: `CadastrarProduto()` → não usa nenhuma variável

**LCOM = 2** (baixa coesão - múltiplos grupos de métodos não relacionados)

**Análise**: **LCOM = 2** indica **baixa coesão**. A classe possui métodos que não compartilham variáveis de instância, indicando que:
- ❌ A classe tem responsabilidades não relacionadas (`CadastrarProduto` não pertence a `Usuario`)
- ❌ Os métodos não trabalham juntos de forma coesa
- ❌ A classe viola o **Princípio da Responsabilidade Única (SRP)**

**Recomendações para melhorar a coesão**:
1. **Remover método não relacionado**: `CadastrarProduto()` deve estar em uma classe `Produto` ou `ProdutoService`
2. **Agrupar métodos relacionados**: Se `ExibirNome()` e `EnviarEmail()` fazem sentido juntos, manter; caso contrário, considerar separação
3. **Refatorar para classes mais coesas**:
   ```csharp
   public class Usuario
   {
        public string Nome { get; set; }
        public string Email { get; set; }
       
        public void ExibirNome() => Console.WriteLine(Nome);
        public void EnviarEmail(string mensagem) => Console.WriteLine($"Enviando para {Email}: {mensagem}");
        public void CadastrarProduto() => Console.WriteLine("Produto cadastrado!");
   }
   ```


   ```csharp
   public class Usuario
   {
        public string Nome { get; set; }
        public string Email { get; set; }
       
        public void ExibirNome() => Console.WriteLine(Nome);
        public void EnviarEmail(string mensagem) => Console.WriteLine($"Enviando para {Nome}-{Email}: {mensagem}");
   }

   public class Usuario
   {
        public string Nome { get; set; }
        public string Email { get; set; }
       
        public void ExibirNome() => Console.WriteLine(Nome);
        public void AlterarNome(string nome) => Nome = nome;
   }
   ```

   **Cálculo do LCOM para esta classe**:
   - Variáveis de instância: `Nome`, `Email`
   - Análise de uso:
     - `ExibirNome()`: usa apenas `Nome` (não usa `Email`)
     - `AlterarNome()`: usa apenas `Nome` (não usa `Email`)
   - **Ambos os métodos compartilham a variável `Nome`**
   - **A variável `Email` não é utilizada por nenhum método**
   
   **LCOM = 1** (coesão moderada - há uma variável não compartilhada)
   
   **Análise**: **LCOM = 1** indica **coesão moderada**. Os métodos são coesos em relação à variável `Nome`, mas a propriedade `Email` não é utilizada, indicando que:
   - ✅ Os métodos trabalham juntos com a mesma propriedade (`Nome`)
   - ⚠️ A propriedade `Email` não é utilizada, sugerindo que pode ser removida ou que falta implementar métodos que a utilizem

   public class Usuario
   {
        public string Nome { get; set; }
        public string Email { get; set; }
       
        public void ExibirNome() => Console.WriteLine(Nome);
        public void EnviarEmail(string mensagem) => Console.WriteLine($"Enviando para {Email}: {mensagem}");
   }
   ```

   **Cálculo do LCOM para esta classe**:
   - Variáveis de instância: `Nome`, `Email`
   - Análise de uso:
     - `ExibirNome()`: usa apenas `Nome` (não usa `Email`)
     - `EnviarEmail()`: usa apenas `Email` (não usa `Nome`)
   - **Os métodos não compartilham variáveis** - cada método usa uma variável diferente
   
   **Grupos de métodos identificados**:
   - Grupo 1: `ExibirNome()` → usa `Nome`
   - Grupo 2: `EnviarEmail()` → usa `Email`
   
   **LCOM = 1** (coesão moderada - métodos não compartilham variáveis entre si)
   
   **Análise**: **LCOM = 1** indica **coesão moderada**. Embora ambos os métodos utilizem variáveis da classe, eles não compartilham as mesmas variáveis, indicando que:
   - ⚠️ Os métodos trabalham com propriedades diferentes (`Nome` vs `Email`)
   - ⚠️ Não há coesão completa, pois os métodos não interagem com as mesmas propriedades
   - ✅ Ainda é aceitável, pois ambos os métodos são relacionados ao conceito de `Usuario`
   
   **Comparação**: Este exemplo tem melhor coesão que o exemplo anterior (linhas 654-673) que tinha LCOM = 2, pois aqui não há métodos que não utilizam nenhuma variável da classe.

   ```csharp
   public class ProdutoService
   {
       public void CadastrarProduto() => Console.WriteLine("Produto cadastrado!");
   }
   ```
**Valores de referência para LCOM**:
- **LCOM = 0**: Alta coesão (ideal) - todos os métodos compartilham variáveis
- **LCOM = 1**: Coesão moderada (aceitável) - poucas variáveis não compartilhadas
- **LCOM ≥ 2**: Baixa coesão (problema) - múltiplos grupos de métodos não relacionados

---

## 7. Complexidade Ciclomática (Cyclomatic Complexity)

**Definição**: Mede o número de caminhos independentes através do código de um método. Cada decisão (if, else, while, for, switch, catch, operadores lógicos &&, ||) adiciona 1 à complexidade. O valor mínimo é sempre 1 (um caminho linear).

**Fórmula**: Complexidade Ciclomática = Número de decisões + 1

### ✅ Exemplo BOM: Método `AddCredit` (Complexidade Ciclomática = 1)

**Arquivo**: `src/CashFlow.Transactions.Domain/Entities/Account.cs`

```csharp
public void AddCredit(decimal amount)
{
    ProcessCredit(new DepositTransaction(Id, amount));
}
```

**Cálculo da Complexidade Ciclomática**:
- Decisões no código: 0 (nenhum if, else, while, switch, etc.)
- Complexidade Ciclomática = 0 + 1 = **1**

**Análise**: **Complexidade Ciclomática = 1** é o valor ideal. O método é linear, sem condicionais, facilitando:
- **Testabilidade**: Apenas um caminho de execução a ser testado
- **Manutenibilidade**: Fácil de entender e modificar
- **Legibilidade**: Código direto e claro

### ❌ Exemplo RUIM: Método `AddTransaction` (Complexidade Ciclomática = 3)

**Arquivo**: `src/CashFlow.Transactions.Domain/Entities/Account.cs`

```csharp
public void AddTransaction(string direction, decimal amount)
{
    if (string.Equals(direction, "Credit", StringComparison.OrdinalIgnoreCase))
    {
        AddCredit(amount);
    }
    else if (string.Equals(direction, "Debit", StringComparison.OrdinalIgnoreCase))
    {
        AddDebit(amount);
    }
    else
    {
        throw new ArgumentException($"Invalid direction: {direction}. Must be 'Credit' or 'Debit'.", nameof(direction));
    }
}
```

**Cálculo da Complexidade Ciclomática**:
- Decisões no código:
  - 1º `if` (verifica se direction é "Credit") = 1
  - 1º `else if` (verifica se direction é "Debit") = 1
  - `else` não conta como decisão (é o caso padrão)
- Total de decisões = 2
- Complexidade Ciclomática = 2 + 1 = **3**

**Análise**: **Complexidade Ciclomática = 3** está dentro do aceitável (< 10), mas é **3x maior** que o exemplo bom. O método possui múltiplos caminhos de execução:
1. Caminho 1: `direction == "Credit"` → chama `AddCredit()`
2. Caminho 2: `direction == "Debit"` → chama `AddDebit()`
3. Caminho 3: `direction` é outro valor → lança exceção

**Impactos**:
- **Testabilidade**: Requer 3 casos de teste (um para cada caminho)
- **Manutenibilidade**: Adicionar novos tipos de direção aumenta a complexidade
- **Legibilidade**: Estrutura condicional aninhada pode ser confusa

**Recomendações para reduzir complexidade**:
- Usar um enum `Direction` em vez de string
- Aplicar o padrão Strategy para diferentes tipos de transação
- Usar um dicionário ou switch expression (C# 8+)

### 🔄 Comparação: Mesma Lógica, Diferentes Estruturas de Controle

A forma como escrevemos o código pode afetar significativamente a complexidade ciclomática, mesmo mantendo a mesma lógica de negócio. Veja como o mesmo método pode ser implementado de formas diferentes:

#### Versão 1: Condicionais Aninhadas com Múltiplas Validações (CC = 5)

```csharp
public void ValidateAndProcessTransaction(string direction, decimal amount, Guid accountId)
{
    if (accountId != Guid.Empty)
    {
        if (amount > 0)
        {
            if (string.Equals(direction, "Credit", StringComparison.OrdinalIgnoreCase))
            {
                AddCredit(amount);
            }
            else
            {
                if (string.Equals(direction, "Debit", StringComparison.OrdinalIgnoreCase))
                {
                    AddDebit(amount);
                }
                else
                {
                    throw new ArgumentException("Invalid direction");
                }
            }
        }
        else
        {
            throw new ArgumentException("Amount must be greater than zero");
        }
    }
    else
    {
        throw new ArgumentException("Account ID cannot be empty");
    }
}
```

**Cálculo da Complexidade Ciclomática**:
- Decisões: `if (accountId != Guid.Empty)` = 1
- Decisões: `if (amount > 0)` = 1
- Decisões: `if (direction == "Credit")` = 1
- Decisões: `if (direction == "Debit")` = 1
- Total de decisões = 4
- **Complexidade Ciclomática = 4 + 1 = 5**

**Problemas desta abordagem**:
- ❌ Estrutura de controle frágil com múltiplos níveis de aninhamento
- ❌ Difícil de ler e manter
- ❌ Cada validação adiciona um nível de indentação
- ❌ Alta complexidade ciclomática

#### Versão 2: Guard Clauses (Early Returns) (CC = 5)

```csharp
public void ValidateAndProcessTransaction(string direction, decimal amount, Guid accountId)
{
    if (accountId == Guid.Empty)
        throw new ArgumentException("Account ID cannot be empty");
    
    if (amount <= 0)
        throw new ArgumentException("Amount must be greater than zero");
    
    if (string.Equals(direction, "Credit", StringComparison.OrdinalIgnoreCase))
    {
        AddCredit(amount);
        return;
    }
    
    if (string.Equals(direction, "Debit", StringComparison.OrdinalIgnoreCase))
    {
        AddDebit(amount);
        return;
    }
    
    throw new ArgumentException("Invalid direction");
}
```

**Cálculo da Complexidade Ciclomática**:
- Decisões: `if (accountId == Guid.Empty)` = 1
- Decisões: `if (amount <= 0)` = 1
- Decisões: `if (direction == "Credit")` = 1
- Decisões: `if (direction == "Debit")` = 1
- Total de decisões = 4
- **Complexidade Ciclomática = 4 + 1 = 5**

**Nota**: Apesar de ter a mesma CC, esta versão é **mais legível** porque:
- ✅ Elimina aninhamento desnecessário
- ✅ Cada validação é independente
- ✅ Fluxo de leitura mais linear (top-to-bottom)
- ✅ Mais fácil de testar e manter

#### Versão 3: Switch Expression (C# 8+) (CC = 3)

```csharp
public void ValidateAndProcessTransaction(string direction, decimal amount, Guid accountId)
{
    if (accountId == Guid.Empty)
        throw new ArgumentException("Account ID cannot be empty");
    
    if (amount <= 0)
        throw new ArgumentException("Amount must be greater than zero");
    
    _ = direction.ToLowerInvariant() switch
    {
        "credit" => AddCredit(amount),
        "debit" => AddDebit(amount),
        _ => throw new ArgumentException("Invalid direction")
    };
}
```

**Cálculo da Complexidade Ciclomática**:
- Decisões: `if (accountId == Guid.Empty)` = 1
- Decisões: `if (amount <= 0)` = 1
- Decisões: `switch` (com 2 cases + default) = 1 (switch conta como uma única decisão)
- Total de decisões = 3
- **Complexidade Ciclomática = 3 + 1 = 4**

**Vantagens desta abordagem**:
- ✅ Menor complexidade ciclomática
- ✅ Código mais conciso e expressivo
- ✅ Fácil de adicionar novos casos
- ✅ Compilador garante exaustividade

#### Versão 4: Validação Separada + Switch Expression (CC = 2)

```csharp
public void ValidateAndProcessTransaction(string direction, decimal amount, Guid accountId)
{
    ValidateInputs(accountId, amount);
    ProcessTransaction(direction, amount);
}

private void ValidateInputs(Guid accountId, decimal amount)
{
    if (accountId == Guid.Empty)
        throw new ArgumentException("Account ID cannot be empty");
    
    if (amount <= 0)
        throw new ArgumentException("Amount must be greater than zero");
}

private void ProcessTransaction(string direction, decimal amount)
{
    _ = direction.ToLowerInvariant() switch
    {
        "credit" => AddCredit(amount),
        "debit" => AddDebit(amount),
        _ => throw new ArgumentException("Invalid direction")
    };
}
```

**Cálculo da Complexidade Ciclomática**:
- **Método `ValidateAndProcessTransaction`**: 0 decisões = **CC = 1**
- **Método `ValidateInputs`**: 2 decisões = **CC = 3**
- **Método `ProcessTransaction`**: 1 decisão (switch) = **CC = 2**

**Vantagens desta abordagem**:
- ✅ **Separação de responsabilidades**: validação separada da lógica de negócio
- ✅ Cada método tem baixa complexidade ciclomática
- ✅ Fácil de testar cada parte independentemente
- ✅ Reutilização: `ValidateInputs` pode ser usado em outros métodos
- ✅ **Princípio da Responsabilidade Única (SRP)** aplicado

#### 📊 Resumo Comparativo

| Versão | Estrutura | CC Total | Legibilidade | Manutenibilidade | Testabilidade |
|--------|-----------|----------|--------------|------------------|----------------|
| **1. Aninhada** | Múltiplos `if` aninhados | **5** | ❌ Baixa | ❌ Difícil | ❌ Complexa |
| **2. Guard Clauses** | Early returns | **5** | ✅ Boa | ✅ Fácil | ✅ Simples |
| **3. Switch Expression** | Switch moderno | **4** | ✅ Muito boa | ✅ Muito fácil | ✅ Simples |
| **4. Separada** | Métodos separados | **1-3** | ✅ Excelente | ✅ Excelente | ✅ Muito simples |

**Conclusão**: A estrutura de controle frágil (aninhamento excessivo) aumenta a complexidade ciclomática e dificulta a manutenção. Refatorar para usar guard clauses, switch expressions ou separar responsabilidades em métodos menores pode reduzir a complexidade e melhorar significativamente a qualidade do código.

**Valores de referência**:
- **1-5**: Baixa complexidade (ideal)
- **6-10**: Complexidade moderada (aceitável, mas monitorar)
- **11-20**: Alta complexidade (considerar refatoração)
- **> 20**: Complexidade muito alta (refatoração necessária)

---

## Resumo Comparativo das Métricas

| Métrica | Valor Ideal | Exemplo BOM | Valor BOM | Exemplo RUIM | Valor RUIM | Diferença |
|---------|------------|-------------|-----------|--------------|------------|-----------|
| **WMC** (Complexidade Ciclomática) | < 20 | `Customer` | **2** | `Account` | **10** | 5x maior |
| **WMC** (Contagem Simples) | < 20 | `Customer` | **2** | `Account` | **7** | 3.5x maior |
| **DIT** | 1-4 | `Customer` | **0** | `DepositTransaction` | **2** | Dentro do ideal |
| **NOC** | 1-5 | `Entity` | **2** | `Transaction` | **3** | Dentro do ideal |
| **CBO** | < 5 | `Customer` | **0** | `PayBill` | **9** | Muito acima |
| **RFC** | < 50 | `Customer` | **3** | `EventBus` | **21** | 7x maior |
| **LCOM** | 0 (baixo) | `Account` | **0** | `Usuario` | **2** | Baixa coesão |
| **Complexidade Ciclomática** (método) | 1-5 | `AddCredit()` | **1** | `AddTransaction()` | **3** | 3x maior |

---

## Conclusão

As métricas CK fornecem uma visão quantitativa da qualidade do design orientado a objetos. Os exemplos apresentados mostram que:

- **Valores baixos** (dentro do ideal) facilitam manutenção, teste e compreensão
- **Valores altos** (acima do ideal) indicam necessidade de atenção e possivelmente refatoração
- A comparação entre exemplos bons e ruins ajuda a entender o impacto das decisões de design

É importante notar que valores "ruins" não necessariamente indicam código incorreto, mas sim áreas que podem se beneficiar de melhorias para aumentar a qualidade e manutenibilidade do código. Por exemplo:

- Classes de **aplicação** (como `PayBill`) naturalmente têm CBO alto devido à orquestração de múltiplas dependências
- Classes de **infraestrutura** (como `EventBus`) naturalmente têm RFC alto devido à integração com sistemas externos
- Classes de **domínio** (como `Customer`, `Account`) devem manter valores baixos para facilitar manutenção e testes

A chave está em entender o contexto e aplicar as métricas de forma apropriada para cada camada da arquitetura.

---

## Referências

- Chidamber, S. R., & Kemerer, C. F. (1994). A metrics suite for object oriented design. *IEEE Transactions on software engineering*, 20(6), 476-493.
