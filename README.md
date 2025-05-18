# Sistema de Controle de Manutenção Industrial (API)

## 📌 Descrição

Esta é uma API para gerenciamento de manutenção industrial, permitindo criar, listar, atualizar e deletar máquinas, alertas, planos de manutenção e usuários.

---

## 🚀 Funcionalidades

* ✅ Login e Autenticação com JWT.
* ✅ Gerenciamento de Máquinas (CRUD).
* ✅ Gerenciamento de Alertas (CRUD).
* ✅ Gerenciamento de Manutenções (CRUD).
* ✅ Gerenciamento de Planos de Manutenção (CRUD).
* ✅ Controle de Atribuições de Planos (CRUD).
* ✅ Gerenciamento de Usuários (CRUD).
* ✅ Testes Automatizados com Postman.

---

## 🚀 Como Usar a API

### ✅ Requisitos:

* .NET SDK (8.0 ou superior).
* SQL Server (para o banco de dados).
* Postman (para testes).

### ✅ Como Rodar a API:

1. Clone o repositório:

   ```bash
   git clone 
   ```

2. Navegue até a pasta da API:

   ```bash
   cd MaintenanceControlSystem.API
   ```

3. Execute o comando:

   ```bash
   dotnet run
   ```

4. A API estará rodando em:

   ```plaintext
   http://localhost:5065
   ```

---

## ✅ Configuração Inicial do Banco de Dados

Para que o sistema funcione corretamente, é necessário criar o primeiro usuário administrador diretamente no banco de dados e popular o banco com alguns dados de teste.

### ✅ Executando o Script de Configuração Inicial

1. Acesse o seu **SQL Server Management Studio (SSMS)** ou outro cliente SQL de sua preferência.

2. Localize o arquivo `InserirDados.sql` dentro da pasta do projeto.

3. Execute o script diretamente no seu banco de dados.

---

## ? Testes com Postman

* Vá na pasta `Postman/`.
* Importe a coleção `SistemaControleManutencao.postman_collection.json` no Postman.
* Garanta que o **Login** é a primeira requisição.
* Use o **Collection Runner** para rodar todos os testes automaticamente.

---

## ? Solucionando Erros Comuns

* ?? **"404 Not Found"** ? O ID especificado não existe.
* ?? **"400 Bad Request"** ? Os dados enviados estão incorretos ou incompletos.
* ?? **"401 Unauthorized"** ? O Token JWT não foi gerado corretamente ou expirou.
* ?? **"409 Conflict"** ? Tentativa de criar um recurso (máquina, usuário) com dados já existentes.

---

## ✅ Observações

* Garanta que o Token JWT é gerado corretamente após o Login.
* Verifique se os IDs dos recursos (máquinas, alertas, manutenções) existem antes de atualizar ou deletar.
* Caso receba um erro "404 Not Found" ou "400 Bad Request", os dados enviados podem estar incorretos.
