# Desafio Técnico - Sistema de Gestão Financeira

## Objetivo
Desenvolver uma aplicação financeira capaz de gerenciar clientes, transações financeiras (entradas, saídas e pagamento de boleto), também seja capaz de gerar relatórios de fechamento diário.

## Requisitos

### Requisitos Funcionais

#### 1. Gerenciamento de Clientes

A aplicação precisa identificar clientes unicamente, bem como conhecer **nome** e **data de nascimento**. Para cada cliente registrado deve ser criado uma **conta bancária** que será utilizada posteriormente para transacionar.

**Requisitos específicos:**
- Identificação única de cada cliente (ID)
- Cadastro de nome completo do cliente
- Cadastro de data de nascimento
- Criação automática de conta bancária ao cadastrar um novo cliente
- Consulta de clientes cadastrados
- Atualização de dados cadastrais

#### 2. Gestão de Contas Financeiras

- Cada cliente possui uma conta bancária única criada automaticamente no cadastro
- Controle de saldo das contas em tempo real
- Histórico completo de transações por conta
- Rastreabilidade de todas as operações realizadas

#### 3. Transações Financeiras

A aplicação deve possibilitar que clientes usem suas contas para:

- **Depositar**: Operação de crédito que adiciona valor ao saldo da conta
- **Sacar**: Operação de débito que retira valor do saldo da conta
- **Pagar Boletos**: Operação específica de débito para pagamento de boletos

**Regra fundamental:** Uma conta **nunca deve ficar com saldo negativo**. Todas as operações de débito (saque e pagamento de boleto) devem validar se há saldo suficiente antes de serem processadas.

**Requisitos adicionais:**
- Validação de saldo suficiente antes de qualquer débito
- Registro de data e hora de cada transação
- Rastreabilidade completa das operações
- Validação de valores (transações devem ter valores maiores que zero)

#### 4. Relatórios e Extratos

Clientes precisam emitir **extratos consolidados** para verificar o quanto foi transacionado em um período ou determinado dia.

**Funcionalidades:**
- Extrato consolidado por período (data inicial e data final)
- Extrato consolidado por dia específico
- Consolidação de todas as transações (depósitos, saques e pagamentos de boletos)
- Agrupamento por cliente e por data
- Geração de relatórios de fechamento diário
- Histórico completo de movimentações financeiras

### Requisitos Técnicos

#### Arquitetura
- Arquitetura baseada em **Domain-Driven Design (DDD)**
- Separação em camadas: Domain, Application, Data, API
- Comunicação assíncrona entre serviços via **Event Bus** (RabbitMQ)
- Persistência de dados em **MongoDB**

#### Padrões e Princípios
- Aplicação de conceitos de **Orientação a Objetos**:
  - Encapsulamento de dados e comportamentos
  - Herança para compartilhamento de funcionalidades comuns
  - Polimorfismo através de interfaces
  - Abstração de contratos e implementações
- Seguimento dos **Princípios SOLID**
- Implementação de **Domain Events** para comunicação entre contextos
- Uso de **Rich Domain Model** com lógica de negócio nas entidades

#### Tecnologias e Ferramentas
- **.NET** (C#) como linguagem principal
- **MongoDB** para persistência de dados
- **RabbitMQ** para mensageria e eventos
- **Docker** para containerização dos serviços
- APIs RESTful para exposição de funcionalidades

### Regras de Negócio

1. **Identificação Única de Clientes**: Cada cliente deve possuir um identificador único no sistema
2. **Dados Obrigatórios do Cliente**: Nome e data de nascimento são obrigatórios no cadastro
3. **Criação Automática de Conta**: Ao cadastrar um cliente, uma conta bancária deve ser criada automaticamente
4. **Saldo Não Negativo**: **Regra crítica** - Uma conta nunca deve ficar com saldo negativo. Todas as operações de débito (saque e pagamento de boleto) devem validar saldo suficiente antes de serem processadas
5. **Transações de Depósito**: Sempre permitidas, incrementam o saldo da conta
6. **Transações de Saque**: Devem validar saldo suficiente antes de processar
7. **Pagamento de Boletos**: Devem validar saldo suficiente antes de processar
8. **Validação de Valores**: Todas as transações devem ter valores maiores que zero
9. **Rastreabilidade**: Todas as transações devem registrar data/hora de criação
10. **Extratos Consolidados**: O sistema deve permitir consulta de extratos por período ou dia específico
11. **Consolidação Diária**: O sistema deve processar eventos de transações para gerar fechamentos diários automaticamente

### Entregáveis Esperados

1. **Código Fonte**: Implementação completa seguindo os padrões arquiteturais definidos
2. **Documentação**: 
   - Documentação da arquitetura (diagramas C4)
   - Documentação de APIs
   - Estudo de caso demonstrando aplicação de conceitos de OO
3. **Testes**: Testes unitários para as principais funcionalidades
4. **Configuração**: Docker Compose para execução local do ambiente completo

### Critérios de Avaliação

- **Qualidade do Código**: Clareza, organização, manutenibilidade
- **Aplicação de Conceitos OO**: Uso adequado de encapsulamento, herança, polimorfismo e abstração
- **Arquitetura**: Separação de responsabilidades, baixo acoplamento, alta coesão
- **Regras de Negócio**: Implementação correta das validações e regras
- **Documentação**: Qualidade e completude da documentação técnica
- **Testes**: Cobertura e qualidade dos testes implementados

## Benefícios do Desafio

Este desafio permite demonstrar:

- **Capacidade de modelagem**: Criação de um modelo de domínio rico e expressivo
- **Conhecimento de OO**: Aplicação prática de conceitos fundamentais de orientação a objetos
- **Arquitetura de Software**: Projeto de uma arquitetura escalável e manutenível
- **Integração de Sistemas**: Comunicação entre serviços através de eventos
- **Boas Práticas**: Aplicação de princípios SOLID, DDD e padrões de design

---

**Boa sorte no desenvolvimento!**

