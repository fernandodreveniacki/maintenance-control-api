# Sistema de Controle de Manutenção Industrial (API)

## ?? Descrição

Esta é uma API para gerenciamento de manutenção industrial, desenvolvida em .NET 8.0, que permite gerenciar máquinas, alertas, manutenções, planos de manutenção e usuários. A API oferece autenticação JWT e possui testes automatizados com Postman.

## ?? Funcionalidades

* ? Login e Autenticação com JWT.
* ? Gerenciamento de Máquinas (CRUD).
* ? Gerenciamento de Alertas (CRUD).
* ? Gerenciamento de Manutenções (CRUD).
* ? Gerenciamento de Planos de Manutenção (CRUD).
* ? Controle de Atribuições de Planos (CRUD).
* ? Gerenciamento de Usuários (CRUD).

---

## ? Como Usar a API

### ? Requisitos:

* .NET SDK (8.0 ou superior).

### ? Como Rodar a API:

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

