# Sistema de Controle de Manutenção Industrial (API)

## 📌 Descrição

Esta é uma API para gerenciamento de manutenção industrial, que permite criar, listar, atualizar e deletar máquinas, alertas, planos de manutenção e usuários.

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

3. Rode o comando:

   ```bash
   dotnet run
   ```

4. A API estará rodando em:

   ```plaintext
   http://localhost:5065
   ```

---

## ✅ Endpoints da API

### 🟢 Login e Autenticação

* **Login:**

  ```plaintext
  POST /api/user/login
  {
    "username": "seu-usuario",
    "password": "sua-senha"
  }
  ```

### 🟢 Gerenciamento de Máquinas

* **Criar Máquina:** POST /api/machine
* **Listar Máquinas:** GET /api/machine
* **Atualizar Máquina:** PUT /api/machine/{id}
* **Deletar Máquina:** DELETE /api/machine/{id}

### 🟢 Gerenciamento de Alertas

* **Criar Alerta:** POST /api/alert
* **Listar Alertas:** GET /api/alert
* **Atualizar Alerta:** PUT /api/alert/{id}
* **Deletar Alerta:** DELETE /api/alert/{id}

### 🟢 Gerenciamento de Manutenções

* **Criar Manutenção:** POST /api/maintenance
* **Listar Manutenções:** GET /api/maintenance
* **Atualizar Manutenção:** PUT /api/maintenance/{id}
* **Deletar Manutenção:** DELETE /api/maintenance/{id}

### 🟢 Gerenciamento de Planos de Manutenção

* **Criar Plano:** POST /api/maintenanceplan
* **Listar Planos:** GET /api/maintenanceplan
* **Atualizar Plano:** PUT /api/maintenanceplan/{id}
* **Deletar Plano:** DELETE /api/maintenanceplan/{id}

### 🟢 Controle de Atribuições de Planos

* **Criar Atribuição:** POST /api/planassignment
* **Listar Atribuições:** GET /api/planassignment
* **Deletar Atribuição:** DELETE /api/planassignment/{id}

### 🟢 Gerenciamento de Usuários

* **Criar Usuário:** POST /api/user/register
* **Listar Usuários:** GET /api/user
* **Atualizar Usuário:** PUT /api/user/{id}
* **Deletar Usuário:** DELETE /api/user/{id}

---

## ✅ Testes com Postman

* Vá na pasta `Postman/`.
* Importe a coleção `SistemaControleManutencao.json` no Postman.
* Garanta que o **Login** é a primeira requisição.
* Use o **Collection Runner** para rodar todos os testes automaticamente.

---

## ✅ Observações Importantes

* Garanta que o Token JWT é gerado corretamente após o Login e aplicado em todas as requisições.
* Verifique se os IDs dos recursos (máquinas, alertas, manutenções) existem antes de tentar atualizar ou deletar.
* Caso receba um erro "404 Not Found" ou "400 Bad Request", isso significa que o recurso não existe ou os dados enviados estão incorretos.
