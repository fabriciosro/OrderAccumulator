📋 Visão Geral
Sistema de trading completo composto por duas aplicações que se comunicam via protocolo FIX 4.4:

OrderGenerator: Frontend React + Backend .NET para geração de ordens

OrderAccumulator: Backend .NET para processamento e controle de risco de ordens

OrderAccumulator - Domain-Driven Design (DDD)

OrderAccumulator/
├── Domain/          ← Aggregates, Entities, Value Objects
├── Application/     ← Commands, Queries, Handlers (CQRS)
├── Infrastructure/  ← Implementação FIX Server + Repositories
└── Presentation/    ← Web API + Configuração

✅ Recebimento de ordens via FIX 4.4

✅ Cálculo de exposição financeira por símbolo

✅ Limite de risco de R$ 100.000.000 por símbolo

✅ Aceitação/rejeição automática de ordens

✅ API REST para consulta de exposições

🛠️ Tecnologias

.NET 8.0

QuickFixN 1.9.0 (FIX Protocol)

MediatR (CQRS)

Swagger/OpenAPI

🧪 Testando o Sistema
1. Inicie o OrderAccumulator

bash

cd OrderAccumulator.Presentation
dotnet run

2. Inicie o OrderGenerator

bash
cd OrderGenerator.Presentation
dotnet run

3. Acesse a aplicação

OrderAccumulator -> https://localhost:5000/swagger/index.html

OrderGenerator -> https://localhost:7001/

Crie ordens e observe os resultados

🐛 Solução de Problemas

Erro de Conexão FIX
Verifique se OrderAccumulator está rodando na porta 9810

Confirme os arquivos de configuração FIX

Verifique as pastas store/ e log/

Build do React Falha
Execute npm install na pasta ClientApp

Verifique se o Node.js está na versão 18+
