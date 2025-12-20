# Métricas MISRA - Exemplos do Projeto Cash Flow

Este documento apresenta exemplos de código do projeto Cash Flow que ilustram as 5 métricas MISRA, utilizadas para medir a qualidade e complexidade do código orientado a objetos.

Para cada métrica, são apresentados um exemplo **bom** (valor ideal) e um exemplo **ruim** (valor acima do ideal), facilitando a comparação e compreensão.

**Importante**: Este documento utiliza **complexidade cognitiva** (cognitive complexity) conforme proposto por Misra et al., que difere da complexidade ciclomática tradicional. A complexidade cognitiva considera o esforço mental necessário para entender o código, penalizando estruturas aninhadas e refletindo melhor a dificuldade de manutenção e compreensão do código.

---

## 1. MC (Method Complexity) - Complexidade de Método

**Definição**: Mede a complexidade cognitiva de um método individual, baseada nos pesos cognitivos das estruturas de controle utilizadas. Diferente da complexidade ciclomática tradicional, a complexidade cognitiva considera o esforço mental necessário para entender o código, penalizando estruturas aninhadas.

**Fórmula**: MC = Σ (Peso Cognitivo de cada estrutura de controle), onde:
- **Sequência** (código linear): peso = 0
- **Condição** (`if`, `else if`, `else`): peso = 1 (base) + 1 por nível de aninhamento
- **Iteração** (`for`, `while`, `foreach`): peso = 2 (base) + 1 por nível de aninhamento
- **Case** (`switch`, `case`): peso = 1 por case
- **Operadores lógicos** (`&&`, `||`): peso = 0.5 cada (dentro de condições)
- **Try-catch**: peso = 1 (try) + 1 (catch)

**Nota**: O valor mínimo é sempre 0 para métodos vazios ou 1 para métodos com apenas código sequencial.

### ✅ Exemplo BOM: Método `AddCredit` (MC = 1)

**Arquivo**: `src/CashFlow.Transactions.Domain/Entities/Account.cs`

```csharp
public void AddCredit(decimal amount)
{
    ProcessCredit(new DepositTransaction(Id, amount));
}
```

**Cálculo do MC**:
- Estruturas de controle: Apenas sequência (código linear)
- Peso cognitivo: 0 (sequência)
- **MC = 0**

**Nota**: Para métodos não vazios com apenas código sequencial, alguns autores consideram MC = 1 como valor mínimo. Neste exemplo, consideramos MC = 0 para código puramente sequencial sem estruturas de controle.

**Análise**: **MC = 1** é o valor ideal. O método é linear, sem condicionais, facilitando:
- **Testabilidade**: Apenas um caminho de execução a ser testado
- **Manutenibilidade**: Fácil de entender e modificar
- **Legibilidade**: Código direto e claro

### ❌ Exemplo RUIM: Método `AddTransaction` (MC = 3)

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

**Cálculo do MC**:
- Estruturas de controle:
  - 1º `if` (nível 0 de aninhamento): peso = 1 + 0 = 1
  - 1º `else if` (nível 0 de aninhamento): peso = 1 + 0 = 1
  - `else` (nível 0 de aninhamento): peso = 1 + 0 = 1
- **MC = 1 + 1 + 1 = 3**

**Análise**: **MC = 3** está dentro do aceitável (< 10), mas é **3x maior** que o exemplo bom. O método possui múltiplos caminhos de execução:
1. Caminho 1: `direction == "Credit"` → chama `AddCredit()`
2. Caminho 2: `direction == "Debit"` → chama `AddDebit()`
3. Caminho 3: `direction` é outro valor → lança exceção

**Impactos**:
- **Testabilidade**: Requer 3 casos de teste (um para cada caminho)
- **Manutenibilidade**: Adicionar novos tipos de direção aumenta a complexidade
- **Legibilidade**: Estrutura condicional pode ser confusa

**Recomendações para reduzir complexidade**:
- Usar um enum `Direction` em vez de string
- Aplicar o padrão Strategy para diferentes tipos de transação
- Usar um dicionário ou switch expression (C# 8+)

### ❌ Exemplo MUITO RUIM: Método `ProcessMultipleTransactions` (MC = 18)

**Exemplo hipotético demonstrando alta complexidade com loops, condições e aninhamento**

```csharp
public void ProcessMultipleTransactions(List<TransactionRequest> requests, bool validateBalance, bool applyFees)
{
    if (requests == null || requests.Count == 0)
    {
        throw new ArgumentException("Transaction requests cannot be null or empty", nameof(requests));
    }

    decimal totalAmount = 0;
    
    foreach (var request in requests)
    {
        if (request.Amount <= 0)
        {
            throw new ArgumentException($"Invalid amount: {request.Amount}", nameof(request));
        }

        if (validateBalance)
        {
            if (request.Direction == Direction.Debit)
            {
                if (Balance < request.Amount)
                {
                    throw new InvalidOperationException($"Insufficient balance for transaction. Required: {request.Amount}, Available: {Balance}");
                }
            }
        }

        try
        {
            if (request.Direction == Direction.Credit)
            {
                AddCredit(request.Amount);
                totalAmount += request.Amount;
            }
            else if (request.Direction == Direction.Debit)
            {
                if (applyFees && request.Amount > 1000)
                {
                    decimal fee = request.Amount * 0.01m;
                    AddDebit(request.Amount + fee);
                    totalAmount -= (request.Amount + fee);
                }
                else
                {
                    AddDebit(request.Amount);
                    totalAmount -= request.Amount;
                }
            }
            else
            {
                throw new ArgumentException($"Invalid direction: {request.Direction}", nameof(request));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing transaction: {TransactionId}", request.Id);
            throw;
        }
    }

    if (totalAmount != 0)
    {
        logger.LogWarning("Total amount processed: {TotalAmount}", totalAmount);
    }
}
```

**Cálculo do MC**:
- Estruturas de controle (considerando níveis de aninhamento):
  - `if (requests == null || requests.Count == 0)` - nível 0: peso = 1 + 0 = 1
  - `foreach (var request in requests)` - nível 0: peso = 2 + 0 = 2 (iteração)
  - `if (request.Amount <= 0)` - nível 1 (dentro do foreach): peso = 1 + 1 = 2
  - `if (validateBalance)` - nível 1 (dentro do foreach): peso = 1 + 1 = 2
  - `if (request.Direction == Direction.Debit)` - nível 2 (aninhado): peso = 1 + 2 = 3
  - `if (Balance < request.Amount)` - nível 3 (aninhado): peso = 1 + 3 = 4
  - `try` - nível 1 (dentro do foreach): peso = 1 + 1 = 2
  - `if (request.Direction == Direction.Credit)` - nível 2 (dentro do try): peso = 1 + 2 = 3
  - `else if (request.Direction == Direction.Debit)` - nível 2 (dentro do try): peso = 1 + 2 = 3
  - `if (applyFees && request.Amount > 1000)` - nível 3 (aninhado): peso = 1 + 3 = 4
  - `else` - nível 3 (aninhado): peso = 1 + 3 = 4
  - `else` - nível 2 (dentro do try): peso = 1 + 2 = 3
  - `catch` - nível 1 (dentro do foreach): peso = 1 + 1 = 2
  - `if (totalAmount != 0)` - nível 0: peso = 1 + 0 = 1
- **MC = 1 + 2 + 2 + 2 + 3 + 4 + 2 + 3 + 3 + 4 + 4 + 3 + 2 + 1 = 36**

**Análise**: **MC = 36** está **muito acima do ideal** e demonstra múltiplos problemas:
1. **Combinação de estruturas**: Loop (`foreach`) com múltiplas condições aninhadas
2. **Múltiplos níveis de aninhamento**: Até 4 níveis de indentação
3. **Tratamento de exceções**: Try-catch adiciona complexidade
4. **Lógica condicional complexa**: Validações e processamento misturados
5. **Múltiplas responsabilidades**: Validação, processamento, cálculo de taxas, logging

**Caminhos de execução identificados**:
1. Lista vazia/nula → exceção
2. Para cada transação:
   - Valor inválido → exceção
   - Se validar saldo e for débito:
     - Saldo insuficiente → exceção
   - Processamento:
     - Crédito → adiciona crédito
     - Débito com taxa (valor > 1000) → adiciona débito + taxa
     - Débito sem taxa → adiciona débito
     - Direção inválida → exceção
   - Erro no processamento → log e re-lança exceção
3. Total processado diferente de zero → log de aviso

**Impactos**:
- **Testabilidade**: Requer muitos casos de teste (lista vazia, valores inválidos, diferentes direções, com/sem validação, com/sem taxas, erros)
- **Manutenibilidade**: Muito difícil de modificar sem introduzir bugs
- **Legibilidade**: Código difícil de seguir devido ao aninhamento e múltiplas responsabilidades
- **Risco de bugs**: Alta probabilidade de erros lógicos, especialmente na lógica de taxas e validações

**Recomendações para reduzir complexidade**:
- **Extrair métodos**: Separar validação, processamento e cálculo de taxas em métodos privados
- **Guard Clauses**: Validar condições inválidas primeiro e retornar cedo
- **Eliminar aninhamento**: Usar early returns e métodos auxiliares
- **Separar responsabilidades**: Criar classes específicas para validação, processamento e cálculo de taxas
- **Aplicar padrões**: Strategy para diferentes tipos de transação, Chain of Responsibility para validações

**Versão refatorada (MC = 8, muito mais legível)**:

```csharp
public void ProcessMultipleTransactions(List<TransactionRequest> requests, bool validateBalance, bool applyFees)
{
    ValidateRequests(requests);
    
    foreach (var request in requests)
    {
        ValidateTransactionRequest(request);
        
        if (validateBalance)
        {
            ValidateBalanceForDebit(request);
        }
        
        ProcessTransaction(request, applyFees);
    }
    
    LogTotalIfNeeded(CalculateTotal(requests));
}

private void ValidateRequests(List<TransactionRequest> requests)
{
    if (requests == null || requests.Count == 0)
        throw new ArgumentException("Transaction requests cannot be null or empty", nameof(requests));
}

private void ValidateTransactionRequest(TransactionRequest request)
{
    if (request.Amount <= 0)
        throw new ArgumentException($"Invalid amount: {request.Amount}", nameof(request));
}

private void ValidateBalanceForDebit(TransactionRequest request)
{
    if (request.Direction == Direction.Debit && Balance < request.Amount)
        throw new InvalidOperationException($"Insufficient balance. Required: {request.Amount}, Available: {Balance}");
}

private void ProcessTransaction(TransactionRequest request, bool applyFees)
{
    try
    {
        if (request.Direction == Direction.Credit)
        {
            AddCredit(request.Amount);
        }
        else if (request.Direction == Direction.Debit)
        {
            ProcessDebitWithFees(request, applyFees);
        }
        else
        {
            throw new ArgumentException($"Invalid direction: {request.Direction}", nameof(request));
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error processing transaction: {TransactionId}", request.Id);
        throw;
    }
}

private void ProcessDebitWithFees(TransactionRequest request, bool applyFees)
{
    if (applyFees && request.Amount > 1000)
    {
        decimal fee = request.Amount * 0.01m;
        AddDebit(request.Amount + fee);
    }
    else
    {
        AddDebit(request.Amount);
    }
}
```

**Cálculo do MC da versão refatorada**:
- `ProcessMultipleTransactions`: MC = 0 (apenas sequência) + 2 (foreach) + 1 (if validateBalance) = 3
- `ValidateRequests`: MC = 1 (if)
- `ValidateTransactionRequest`: MC = 1 (if)
- `ValidateBalanceForDebit`: MC = 1 (if) + 1 (if aninhado) = 2
- `ProcessTransaction`: MC = 1 (try) + 1 (if) + 1 (else if) + 1 (else) + 1 (catch) = 5
- `ProcessDebitWithFees`: MC = 1 (if) + 1 (else) = 2

**MC total do método principal + métodos auxiliares = 3 + 1 + 1 + 2 + 5 + 2 = 14**

**Redução de complexidade**: De MC = 36 para MC = 14, uma redução de **61%**! Além disso, cada método agora tem responsabilidade única e é muito mais fácil de testar e manter.

### ❌ Exemplo EXTREMAMENTE RUIM: Método com Ifs Aninhados Profundos (MC = 31)

**Exemplo hipotético demonstrando alta complexidade com ifs aninhados**

```csharp
public void ValidateAndProcessTransaction(string direction, decimal amount, Guid accountId, bool requiresApproval)
{
    if (accountId != Guid.Empty)
    {
        if (amount > 0)
        {
            if (string.Equals(direction, "Credit", StringComparison.OrdinalIgnoreCase))
            {
                if (requiresApproval)
                {
                    AddCredit(amount);
                }
                else
                {
                    AddCredit(amount);
                }
            }
            else if (string.Equals(direction, "Debit", StringComparison.OrdinalIgnoreCase))
            {
                if (Balance >= amount)
                {
                    AddDebit(amount);
                }
                else
                {
                    throw new InvalidOperationException("Insufficient balance");
                }
            }
            else
            {
                throw new ArgumentException("Invalid direction");
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

**Cálculo do MC**:
- Estruturas de controle (considerando níveis de aninhamento):
  - `if (accountId != Guid.Empty)` - nível 0: peso = 1 + 0 = 1
  - `if (amount > 0)` - nível 1 (aninhado): peso = 1 + 1 = 2
  - `if (direction == "Credit")` - nível 2 (aninhado): peso = 1 + 2 = 3
  - `if (requiresApproval)` - nível 3 (aninhado): peso = 1 + 3 = 4
  - `else` - nível 3 (aninhado): peso = 1 + 3 = 4
  - `else if (direction == "Debit")` - nível 2 (aninhado): peso = 1 + 2 = 3
  - `if (Balance >= amount)` - nível 3 (aninhado): peso = 1 + 3 = 4
  - `else` - nível 3 (aninhado): peso = 1 + 3 = 4
  - `else` - nível 2 (aninhado): peso = 1 + 2 = 3
  - `else` - nível 1 (aninhado): peso = 1 + 1 = 2
  - `else` - nível 0: peso = 1 + 0 = 1
- **MC = 1 + 2 + 3 + 4 + 4 + 3 + 4 + 4 + 3 + 2 + 1 = 31**

**Nota**: Este cálculo demonstra como o aninhamento aumenta significativamente a complexidade cognitiva, muito mais que a complexidade ciclomática tradicional.

**Análise**: **MC = 31** está **muito acima do ideal** e demonstra dramaticamente os problemas de ifs aninhados:
1. **Múltiplos níveis de aninhamento**: 4 níveis de indentação
2. **Múltiplos caminhos de execução**: 8+ caminhos possíveis
3. **Dificuldade de leitura**: Código difícil de seguir
4. **Alta complexidade de teste**: Requer muitos casos de teste para cobrir todos os caminhos

**Caminhos de execução identificados**:
1. `accountId` vazio → exceção
2. `amount <= 0` → exceção
3. `direction == "Credit"` + `requiresApproval == true` → `AddCredit()`
4. `direction == "Credit"` + `requiresApproval == false` → `AddCredit()`
5. `direction == "Debit"` + `Balance >= amount` → `AddDebit()`
6. `direction == "Debit"` + `Balance < amount` → exceção
7. `direction` inválido → exceção

**Impactos**:
- **Testabilidade**: Requer pelo menos 7 casos de teste para cobertura completa
- **Manutenibilidade**: Adicionar novas validações aumenta exponencialmente a complexidade
- **Legibilidade**: Estrutura de controle frágil e difícil de entender
- **Risco de bugs**: Fácil introduzir erros lógicos com tantos níveis de aninhamento

**Recomendações para reduzir complexidade**:
- **Guard Clauses (Early Returns)**: Validar condições inválidas primeiro e retornar cedo
- **Extrair métodos**: Separar validações em métodos privados menores
- **Eliminar aninhamento desnecessário**: Usar `return` antecipado para reduzir níveis
- **Aplicar padrões de design**: Strategy, Chain of Responsibility, ou Command para diferentes tipos de transação

**Versão refatorada com Guard Clauses (MC = 6, muito mais legível)**:

```csharp
public void ValidateAndProcessTransaction(string direction, decimal amount, Guid accountId, bool requiresApproval)
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
        if (Balance < amount)
            throw new InvalidOperationException("Insufficient balance");
        
        AddDebit(amount);
        return;
    }
    
    throw new ArgumentException("Invalid direction");
}
```

**Cálculo do MC da versão refatorada**:
- Estruturas de controle (todos no nível 0, sem aninhamento):
  - `if (accountId == Guid.Empty)` - nível 0: peso = 1
  - `if (amount <= 0)` - nível 0: peso = 1
  - `if (direction == "Credit")` - nível 0: peso = 1
  - `if (direction == "Debit")` - nível 0: peso = 1
  - `if (Balance < amount)` - nível 0: peso = 1
- **MC = 1 + 1 + 1 + 1 + 1 = 5**

**Redução de complexidade**: De MC = 31 para MC = 5, uma redução de **84%**!

**Vantagens da versão refatorada**:
- ✅ **Redução drástica de complexidade cognitiva** (de 31 para 5)
- ✅ Elimina aninhamento desnecessário
- ✅ Fluxo de leitura linear (top-to-bottom)
- ✅ Mais fácil de testar e manter
- ✅ Cada validação é independente
- ✅ Demonstra como a estrutura do código impacta a complexidade cognitiva

### ❌ Exemplo MUITO RUIM: Método `ProcessDebit` (MC = 2)

**Arquivo**: `src/CashFlow.Transactions.Domain/Entities/Account.cs`

```csharp
private void ProcessDebit(Transaction transaction)
{
    if (Balance < transaction.Value) 
        throw new InvalidOperationException("Debit amount can't be less than current balance");
    
    Transactions ??= new List<Transaction>();

    Transactions.Add(transaction);
    Balance -= transaction.Value;
    
    var transactionEventCreated = new TransactionCreated(
        transaction.Id,
        transaction.AccountId,
        transaction.Direction.ToString(),
        transaction.TransactionType.ToString(),
        transaction.ReferenceDate,
        transaction.Value);
    
    var balanceEvent = new AccountUpdated(Id, transactionEventCreated.ReferenceDate, Balance, transactionEventCreated);
    
    AddEvent(transactionEventCreated);
    AddEvent(balanceEvent);
}
```

**Cálculo do MC**:
- Estruturas de controle:
  - `if (Balance < transaction.Value)` - nível 0: peso = 1
- **MC = 1**

**Análise**: **MC = 1** está dentro do aceitável. O método possui uma única condição simples, facilitando a compreensão. Embora o valor seja baixo, o método realiza múltiplas operações, o que pode ser melhorado através de extração de métodos.

**Valores de referência para MC** (baseado em complexidade cognitiva):
- **0-3**: Baixa complexidade (ideal)
- **4-7**: Complexidade moderada (aceitável, mas monitorar)
- **8-15**: Alta complexidade (considerar refatoração)
- **> 15**: Complexidade muito alta (refatoração necessária)

**Diferença entre Complexidade Ciclomática e Complexidade Cognitiva**:
- **Complexidade Ciclomática**: Conta apenas o número de caminhos de execução (decisões + 1)
- **Complexidade Cognitiva**: Penaliza estruturas aninhadas, refletindo melhor o esforço mental necessário para entender o código
- Métodos com mesmo número de decisões podem ter complexidades cognitivas muito diferentes dependendo do nível de aninhamento

---

## 2. AC (Attribute Complexity) - Complexidade de Atributos

**Definição**: Mede a complexidade dos atributos (propriedades) de uma classe através da contagem simples do número total de atributos. Diferente de outras métricas de Misra que utilizam pesos cognitivos, a AC é uma métrica simples baseada apenas na quantidade de atributos, onde cada atributo contribui igualmente para o valor total.

**Fórmula**: AC = Número total de atributos (propriedades) da classe

**Nota**: Todos os atributos contam como 1, independentemente do tipo (primitivo, referência, coleção, nullable, etc.). A métrica não diferencia entre tipos de atributos.

### ✅ Exemplo BOM: Classe `Customer` (AC = 2)

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

**Cálculo do AC**:
- Número de atributos: 2
  - `Id` (Guid)
  - `FullName` (string)
- **AC = 2**

**Análise**: **AC = 2** é um valor muito baixo, indicando que a classe possui apenas atributos primitivos simples. Isso facilita:
- **Serialização**: Fácil de serializar/deserializar
- **Persistência**: Simples de mapear para banco de dados
- **Testabilidade**: Fácil de criar instâncias para testes
- **Manutenibilidade**: Poucos atributos para gerenciar

### ❌ Exemplo RUIM: Classe `Account` (AC = 7)

**Arquivo**: `src/CashFlow.Transactions.Domain/Entities/Account.cs`

```csharp
public class Account : Entity
{
    public Guid Id { get; private set; }
    public int Version { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal Balance { get; private set; }
    public ICollection<Transaction>? Transactions { get; set; }

    // ... métodos
}
```

**Cálculo do AC**:
- Número de atributos: 5
  - `Id` (Guid)
  - `Version` (int)
  - `CustomerId` (Guid)
  - `Balance` (decimal)
  - `Transactions` (ICollection<Transaction>?)
- **AC = 5**

**Análise**: **AC = 5** está no limite do ideal (≤ 5). A classe possui:
- 4 atributos primitivos
- 1 atributo de coleção

**Impactos**:
- **Complexidade de persistência**: A coleção `Transactions` requer mapeamento especial (relacionamento 1:N)
- **Serialização**: A coleção pode ser grande e impactar performance
- **Memória**: Coleções podem crescer indefinidamente
- **Testabilidade**: Requer setup mais complexo para criar instâncias com transações

**Recomendações para reduzir AC**:
- Considerar lazy loading para `Transactions` (carregar apenas quando necessário)
- Usar paginação para coleções grandes
- Separar relacionamentos em agregados diferentes quando apropriado
- Remover atributos não utilizados ou que possam ser calculados dinamicamente

### ❌ Exemplo MUITO RUIM: Classe `Transaction` (AC = 7)

**Arquivo**: `src/CashFlow.Transactions.Domain/Entities/Transaction.cs`

```csharp
public abstract class Transaction : Entity
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public Account? Account { get; private set; }
    public Direction Direction { get; private set; }
    public TransactionType TransactionType { get; protected set; }
    public DateTime ReferenceDate { get; private set; }
    public decimal Value { get; private set; }
    
    // ... métodos
}
```

**Cálculo do AC**:
- Número de atributos: 7
  - `Id` (Guid)
  - `AccountId` (Guid)
  - `Account` (Account?)
  - `Direction` (enum)
  - `TransactionType` (enum)
  - `ReferenceDate` (DateTime)
  - `Value` (decimal)
- **AC = 7**

**Análise**: **AC = 7** está acima do ideal (≤ 5). A classe possui:
- 6 atributos primitivos/enum
- 1 atributo de referência para outra entidade (`Account?`)

**Impactos**:
- **Acoplamento**: A referência a `Account` cria acoplamento bidirecional
- **Ciclos de referência**: Pode causar problemas em serialização (JSON, XML)
- **Complexidade de mapeamento**: Relacionamento bidirecional requer cuidado no ORM

**Recomendações para reduzir AC**:
- Remover a propriedade de navegação `Account` se não for estritamente necessária
- Usar apenas `AccountId` (foreign key) quando possível
- Considerar DTOs para serialização, evitando referências circulares

**Valores de referência para AC**:
- **1-5**: Baixa complexidade (ideal)
- **6-10**: Complexidade moderada (aceitável, mas monitorar)
- **11-15**: Alta complexidade (considerar refatoração)
- **> 15**: Complexidade muito alta (refatoração necessária)

**Nota importante**: A métrica AC de Misra é uma contagem simples e não diferencia entre tipos de atributos. Todos os atributos (primitivos, referências, coleções, nullable) contam igualmente como 1. Esta simplicidade facilita o cálculo, mas pode não capturar completamente a complexidade real de classes com muitos atributos de referência ou coleções, que podem ser mais complexas de gerenciar do que atributos primitivos.

---

## 3. CLC (Class Complexity) - Complexidade de Classe

**Definição**: Mede a complexidade total de uma classe, calculada como a soma das complexidades ciclomáticas de todos os métodos da classe. É equivalente ao WMC (Weighted Methods per Class) das métricas CK quando calculado usando complexidade ciclomática.

**Fórmula**: CLC = Σ (MC de cada método da classe)

### ✅ Exemplo BOM: Classe `Customer` (CLC = 2)

**Arquivo**: `src/CashFlow.Customers.Domain/Entities/Customer.cs`

```csharp
public class Customer
{
    public Guid Id { get; private set; }
    
    public string FullName { get; private set; }

    public Customer(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("FullName is required");
        
        Id = Guid.CreateVersion7();
        FullName = fullName;
    }

    public void WithFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("FullName is required");
        
        FullName = fullName;
    }
}
```

**Cálculo do CLC**:
- Construtor `Customer()`: 
  - `if (string.IsNullOrWhiteSpace(fullName))` - nível 0: peso = 1
  - MC = 1
- Método `WithFullName()`: 
  - `if (string.IsNullOrWhiteSpace(fullName))` - nível 0: peso = 1
  - MC = 1
- **CLC = 1 + 1 = 2**

**Análise**: **CLC = 2** é um valor muito baixo, indicando que a classe possui métodos simples com apenas validações básicas. Isso facilita:
- **Compreensão**: Fácil de entender o comportamento da classe
- **Testabilidade**: Poucos caminhos de execução para testar
- **Manutenibilidade**: Mudanças são simples e localizadas

### ❌ Exemplo RUIM: Classe `Account` (CLC = 10)

**Arquivo**: `src/CashFlow.Transactions.Domain/Entities/Account.cs`

```csharp
public class Account : Entity
{
    public Guid Id { get; private set; }
    public int Version { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal Balance { get; private set; }
    public ICollection<Transaction>? Transactions { get; set; }

    public Account(Guid customerId) { ... } // MC = 1
    
    public void AddDebit(decimal amount) { ... } // MC = 1
    
    public void AddCredit(decimal amount) { ... } // MC = 1
    
    public void PayBill(decimal amount) { ... } // MC = 1

    public void AddTransaction(string direction, decimal amount) { ... } // MC = 3

    private void ProcessCredit(Transaction transaction) { ... } // MC = 1

    private void ProcessDebit(Transaction transaction) { ... } // MC = 2
}
```

**Cálculo do CLC** (usando complexidade cognitiva):
- Construtor `Account()`: MC = 0 (apenas sequência)
- `AddDebit()`: MC = 0 (apenas sequência)
- `AddCredit()`: MC = 0 (apenas sequência)
- `PayBill()`: MC = 0 (apenas sequência)
- `AddTransaction()`: MC = 3 (if + else if + else, todos nível 0)
- `ProcessCredit()`: MC = 0 (apenas sequência)
- `ProcessDebit()`: MC = 1 (if condicional, nível 0)
- **CLC = 0 + 0 + 0 + 0 + 3 + 0 + 1 = 4**

**Análise**: **CLC = 4** está dentro do aceitável (< 10), mas é maior que o exemplo bom. A classe possui:
- 7 métodos no total
- Métodos com diferentes níveis de complexidade
- O método `AddTransaction` contribui significativamente com MC = 3

**Impactos**:
- **Testabilidade**: Requer mais casos de teste para cobrir todos os caminhos
- **Manutenibilidade**: Mudanças podem afetar múltiplos métodos
- **Compreensão**: Mais difícil de entender o comportamento completo da classe

**Recomendações para reduzir CLC**:
- Extrair lógica complexa para métodos privados menores
- Usar padrões de design (Strategy, Command) para reduzir condicionais
- Considerar quebrar a classe em classes menores se a responsabilidade crescer

### ❌ Exemplo MUITO RUIM: Classe `EventBus` (CLC = 4)

**Arquivo**: `src/CashFlow.Lib.EventBus/EventBus.cs`

```csharp
public class EventBus(ILogger<EventBus> logger, ConnectionFactory factory) : IEventBus
{
    public async Task PublishAsync<T>(T @event, string queueName) where T : class
    {
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        
        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);
        
        await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, body: body);
        
        logger.LogInformation($"Publishing domain event: {queueName} - {message}");
    }

    public async Task SubscribeAsync<T>(string queueName, Func<T, Task> handler) where T : class
    {
        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false,
            arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var _jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
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
}
```

**Cálculo do CLC** (usando complexidade cognitiva):
- Construtor: MC = 0 (apenas sequência)
- `PublishAsync()`: MC = 0 (apenas sequência)
- `SubscribeAsync()`: 
  - `try`: peso = 1
  - `if (message != null)`: peso = 1 (nível 0)
  - `catch`: peso = 1
  - MC = 1 + 1 + 1 = 3
- **CLC = 0 + 0 + 3 = 3**

**Análise**: **CLC = 3** é um valor baixo, indicando que a classe possui métodos relativamente simples em termos de estruturas de controle. No entanto, a classe `EventBus` realiza operações complexas de integração com sistemas externos (RabbitMQ). O valor de CLC não captura completamente a complexidade operacional, apenas a complexidade cognitiva de fluxo de controle.

**Valores de referência para CLC** (baseado em complexidade cognitiva):
- **0-5**: Baixa complexidade (ideal)
- **6-12**: Complexidade moderada (aceitável, mas monitorar)
- **13-20**: Alta complexidade (considerar refatoração)
- **> 20**: Complexidade muito alta (refatoração necessária)

---

## 4. CWC (Coupling Weight For a Class) - Peso de Acoplamento de Classe

**Definição**: Mede o peso de acoplamento de uma classe com outras classes, considerando não apenas o número de classes acopladas, mas também o tipo e intensidade do acoplamento. Diferentes tipos de acoplamento recebem pesos diferentes.

**Fórmula**: CWC = Σ (peso do acoplamento), onde:
- **Acoplamento por herança** (herda de classe): peso = 3
- **Acoplamento por composição** (atributo de classe): peso = 2
- **Acoplamento por dependência** (usa em método): peso = 1
- **Acoplamento por injeção de dependência** (construtor/parâmetro): peso = 1.5
- **Acoplamento por interface** (implementa interface): peso = 1

### ✅ Exemplo BOM: Classe `Customer` (CWC = 0)

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

**Cálculo do CWC**:
- Não herda de nenhuma classe: 0
- Não possui atributos de classe: 0
- Não usa outras classes em métodos (apenas tipos primitivos): 0
- Não recebe dependências: 0
- Não implementa interfaces: 0
- **CWC = 0**

**Análise**: **CWC = 0** indica **zero acoplamento** com outras classes. Isso resulta em:
- **Alta reutilização**: A classe pode ser usada em qualquer contexto
- **Facilidade de teste**: Não requer mocks ou stubs
- **Baixa propagação de mudanças**: Mudanças em outras classes não afetam esta
- **Alta coesão**: A classe é auto-suficiente

### ❌ Exemplo RUIM: Classe `Account` (CWC = 6)

**Arquivo**: `src/CashFlow.Transactions.Domain/Entities/Account.cs`

```csharp
public class Account : Entity
{
    public Guid Id { get; private set; }
    public int Version { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal Balance { get; private set; }
    public ICollection<Transaction>? Transactions { get; set; }

    // ... métodos que usam Transaction, WithdrawTransaction, DepositTransaction, BillPaymentTransaction
}
```

**Cálculo do CWC**:
- Herda de `Entity`: peso = 3
- Atributo `Transactions` (ICollection<Transaction>): peso = 2 (composição)
- Usa `Transaction`, `WithdrawTransaction`, `DepositTransaction`, `BillPaymentTransaction` em métodos: peso = 1 × 4 = 4
- **CWC = 3 + 2 + 4 = 9**

**Correção**: Recalculando apenas acoplamentos únicos:
- Herda de `Entity`: peso = 3
- Atributo `Transactions` (ICollection<Transaction>): peso = 2 (composição)
- Usa `Transaction` (classe base): peso = 1 (dependência)
- **CWC = 3 + 2 + 1 = 6**

**Análise**: **CWC = 6** está acima do ideal (< 5). A classe possui:
- Acoplamento por herança com `Entity`
- Acoplamento por composição com `Transaction` (através da coleção)
- Acoplamento por dependência com classes de transação

**Impactos**:
- **Propagação de mudanças**: Mudanças em `Transaction` ou `Entity` podem afetar `Account`
- **Testabilidade**: Requer criação de objetos `Transaction` para testes
- **Reutilização**: Limitada pelo acoplamento com outras classes

**Recomendações para reduzir CWC**:
- Considerar usar apenas IDs em vez de objetos completos quando possível
- Aplicar o princípio de inversão de dependência (DIP) usando interfaces
- Separar responsabilidades em classes menores

### ❌ Exemplo MUITO RUIM: Classe `PayBill` (CWC = 13.5)

**Arquivo**: `src/CashFlow.Transactions.Application/PayBill.cs`

```csharp
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

**Cálculo do CWC**:
- Implementa interface `IPayBill`: peso = 1
- Injeção de dependência `ILogger<PayBill>`: peso = 1.5
- Injeção de dependência `IRepository`: peso = 1.5
- Injeção de dependência `IEventBus`: peso = 1.5
- Usa `Account` (através de `accountRepository`): peso = 1
- Usa `Transaction` (através de `account.Transactions`): peso = 1
- Usa `PayBillRequest`: peso = 1
- Usa `AccountResponse`: peso = 1
- Usa `NotFoundException`: peso = 1
- **CWC = 1 + 1.5 + 1.5 + 1.5 + 1 + 1 + 1 + 1 + 1 = 11.5**

**Correção**: Considerando apenas tipos únicos de acoplamento:
- Implementa interface `IPayBill`: peso = 1
- Injeção de dependência `ILogger<PayBill>`: peso = 1.5
- Injeção de dependência `IRepository`: peso = 1.5
- Injeção de dependência `IEventBus`: peso = 1.5
- Usa `Account`: peso = 1
- Usa `Transaction`: peso = 1
- Usa `PayBillRequest`: peso = 1
- Usa `AccountResponse`: peso = 1
- Usa `NotFoundException`: peso = 1
- **CWC = 1 + 1.5 + 1.5 + 1.5 + 1 + 1 + 1 + 1 + 1 = 11.5**

**Análise**: **CWC = 11.5** está **muito acima do ideal (< 5)**. A classe possui:
- 3 dependências injetadas (alto acoplamento por injeção)
- Múltiplas dependências de classes de domínio e DTOs
- Alto acoplamento com várias camadas (domínio, aplicação, infraestrutura)

**Impactos**:
- **Testabilidade**: Requer muitos mocks (logger, repository, eventBus)
- **Manutenibilidade**: Mudanças em qualquer dependência podem quebrar a classe
- **Reutilização**: Limitada pelo alto número de dependências
- **Complexidade de setup**: Difícil de instanciar e testar

**Recomendações para reduzir CWC**:
- Aplicar o padrão Mediator para reduzir acoplamento direto
- Usar objetos de valor (Value Objects) em vez de classes complexas
- Considerar o padrão CQRS para separar comandos e consultas
- Extrair lógica de negócio para serviços de domínio

**Valores de referência para CWC**:
- **0-5**: Baixo acoplamento (ideal)
- **6-10**: Acoplamento moderado (aceitável, mas monitorar)
- **11-15**: Alto acoplamento (considerar refatoração)
- **> 15**: Acoplamento muito alto (refatoração necessária)

---

## 5. CC (Code Complexity) - Complexidade de Código

**Definição**: Mede a complexidade ciclomática total do código, calculada como a soma de todas as complexidades ciclomáticas de todos os métodos em um módulo, classe ou sistema. É uma métrica agregada que fornece uma visão geral da complexidade do código.

**Fórmula**: CC = Σ (MC de todos os métodos no escopo analisado)

**Escopo de análise**: Pode ser calculado para:
- **Método**: CC = MC (complexidade do próprio método)
- **Classe**: CC = CLC (soma das complexidades de todos os métodos da classe)
- **Módulo/Sistema**: CC = Σ (CLC de todas as classes no módulo)

### ✅ Exemplo BOM: Classe `Customer` (CC = 2)

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

**Cálculo do CC** (no escopo da classe, usando complexidade cognitiva):
- Construtor `Customer()`: MC = 0
- Método `WithFullName()`: MC = 0
- **CC = 0 + 0 = 0**

**Análise**: **CC = 2** é um valor muito baixo, indicando código simples e linear. Para uma classe de domínio, este é um valor ideal.

### ❌ Exemplo RUIM: Classe `Account` (CC = 10)

**Arquivo**: `src/CashFlow.Transactions.Domain/Entities/Account.cs`

```csharp
public class Account : Entity
{
    // ... atributos

    public Account(Guid customerId) { ... } // MC = 1
    public void AddDebit(decimal amount) { ... } // MC = 1
    public void AddCredit(decimal amount) { ... } // MC = 1
    public void PayBill(decimal amount) { ... } // MC = 1
    public void AddTransaction(string direction, decimal amount) { ... } // MC = 3
    private void ProcessCredit(Transaction transaction) { ... } // MC = 1
    private void ProcessDebit(Transaction transaction) { ... } // MC = 2
}
```

**Cálculo do CC** (no escopo da classe, usando complexidade cognitiva):
- Construtor: MC = 0
- `AddDebit()`: MC = 0
- `AddCredit()`: MC = 0
- `PayBill()`: MC = 0
- `AddTransaction()`: MC = 3
- `ProcessCredit()`: MC = 0
- `ProcessDebit()`: MC = 1
- **CC = 0 + 0 + 0 + 0 + 3 + 0 + 1 = 4**

**Análise**: **CC = 4** está dentro do aceitável para uma classe (< 10). A complexidade é distribuída entre vários métodos, com o método `AddTransaction` sendo o mais complexo (MC = 3).

### ❌ Exemplo MUITO RUIM: Módulo `EventBus` (CC = 4)

**Arquivo**: `src/CashFlow.Lib.EventBus/EventBus.cs`

```csharp
public class EventBus(ILogger<EventBus> logger, ConnectionFactory factory) : IEventBus
{
    public async Task PublishAsync<T>(T @event, string queueName) where T : class
    {
        // ... código sem decisões condicionais
    }

    public async Task SubscribeAsync<T>(string queueName, Func<T, Task> handler) where T : class
    {
        // ... código com try/catch e if
    }
}
```

**Cálculo do CC** (no escopo da classe, usando complexidade cognitiva):
- Construtor: MC = 0
- `PublishAsync()`: MC = 0
- `SubscribeAsync()`: MC = 3 (try + if + catch)
- **CC = 0 + 0 + 3 = 3**

**Análise**: **CC = 3** é um valor baixo, indicando que a classe possui métodos relativamente simples em termos de estruturas de controle. No entanto, a classe `EventBus` realiza operações complexas de integração com sistemas externos (RabbitMQ). A complexidade cognitiva não captura completamente a complexidade operacional, apenas a complexidade de fluxo de controle.

### 📊 Exemplo: CC em Módulo Completo

Para calcular o CC de um módulo completo, somamos as complexidades de todas as classes:

**Módulo**: `CashFlow.Transactions.Domain/Entities`

**Classes**:
- `Account`: CLC = 10
- `Transaction`: CLC = 2 (construtor com 1 if)
- `DepositTransaction`: CLC = 1
- `WithdrawTransaction`: CLC = 1
- `BillPaymentTransaction`: CLC = 1

**Cálculo do CC** (no escopo do módulo, usando complexidade cognitiva):
- `Account`: CLC = 4
- `Transaction`: CLC = 1 (construtor com 1 if)
- `DepositTransaction`: CLC = 0
- `WithdrawTransaction`: CLC = 0
- `BillPaymentTransaction`: CLC = 0
- **CC = 4 + 1 + 0 + 0 + 0 = 5**

**Análise**: **CC = 5** para o módulo de entidades está dentro do aceitável. A maior parte da complexidade está concentrada na classe `Account`, que é a entidade mais complexa do domínio.

**Valores de referência para CC**:
- **Por método**: 1-5 (ideal), 6-10 (aceitável), > 10 (refatorar)
- **Por classe**: 1-10 (ideal), 11-20 (aceitável), > 20 (refatorar)
- **Por módulo**: Depende do tamanho, mas valores acima de 50-100 indicam necessidade de modularização

---

## Resumo Comparativo das Métricas MISRA

| Métrica | Valor Ideal | Exemplo BOM | Valor BOM | Exemplo RUIM | Valor RUIM | Diferença |
|---------|------------|-------------|-----------|--------------|------------|-----------|
| **MC** (Method Complexity) | 0-3 | `AddCredit()` | **0** | `AddTransaction()` | **3** | - |
| **AC** (Attribute Complexity) | 1-5 | `Customer` | **2** | `Transaction` | **7** | 3.5x maior |
| **CLC** (Class Complexity) | 0-5 | `Customer` | **0** | `Account` | **4** | - |
| **CWC** (Coupling Weight) | 0-5 | `Customer` | **0** | `PayBill` | **11.5** | Muito acima |
| **CC** (Code Complexity) | 0-5 (classe) | `Customer` | **0** | `Account` | **4** | - |

---

## Conclusão

As métricas MISRA fornecem uma visão quantitativa da qualidade e complexidade do código orientado a objetos. Os exemplos apresentados mostram que:

- **Valores baixos** (dentro do ideal) facilitam manutenção, teste e compreensão
- **Valores altos** (acima do ideal) indicam necessidade de atenção e possivelmente refatoração
- A comparação entre exemplos bons e ruins ajuda a entender o impacto das decisões de design

É importante notar que valores "ruins" não necessariamente indicam código incorreto, mas sim áreas que podem se beneficiar de melhorias para aumentar a qualidade e manutenibilidade do código. Por exemplo:

- Classes de **aplicação** (como `PayBill`) naturalmente têm CWC alto devido à orquestração de múltiplas dependências
- Classes de **infraestrutura** (como `EventBus`) podem ter CC baixo mas alta complexidade operacional
- Classes de **domínio** (como `Customer`, `Account`) devem manter valores baixos para facilitar manutenção e testes

A chave está em entender o contexto e aplicar as métricas de forma apropriada para cada camada da arquitetura.

---

## Referências

- MISRA (Motor Industry Software Reliability Association) - Guidelines for software development
- Chidamber, S. R., & Kemerer, C. F. (1994). A metrics suite for object oriented design. *IEEE Transactions on software engineering*, 20(6), 476-493.
- McCabe, T. J. (1976). A complexity measure. *IEEE Transactions on software Engineering*, (4), 308-320.

