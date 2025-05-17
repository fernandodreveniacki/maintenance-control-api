# Sistema de Controle de Manuten��o Industrial (API)

## ?? Descri��o

Esta � uma API para gerenciamento de manuten��o industrial, desenvolvida em .NET 8.0, que permite gerenciar m�quinas, alertas, manuten��es, planos de manuten��o e usu�rios. A API oferece autentica��o JWT e possui testes automatizados com Postman.

## ?? Funcionalidades

* ? Login e Autentica��o com JWT.
* ? Gerenciamento de M�quinas (CRUD).
* ? Gerenciamento de Alertas (CRUD).
* ? Gerenciamento de Manuten��es (CRUD).
* ? Gerenciamento de Planos de Manuten��o (CRUD).
* ? Controle de Atribui��es de Planos (CRUD).
* ? Gerenciamento de Usu�rios (CRUD).

---

## ? Como Usar a API

### ? Requisitos:

* .NET SDK (8.0 ou superior).

### ? Como Rodar a API:

1. Clone o reposit�rio:

   ```bash
   git clone 
   ```

2. Navegue at� a pasta da API:

   ```bash
   cd MaintenanceControlSystem.API
   ```

3. Rode o comando:

   ```bash
   dotnet run
   ```

4. A API estar� rodando em:

   ```plaintext
   http://localhost:5065
   ```

---

## ? Testes com Postman

* V� na pasta `Postman/`.
* Importe a cole��o `SistemaControleManutencao.postman_collection.json` no Postman.
* Garanta que o **Login** � a primeira requisi��o.
* Use o **Collection Runner** para rodar todos os testes automaticamente.

---

## ? Solucionando Erros Comuns

* ?? **"404 Not Found"** ? O ID especificado n�o existe.
* ?? **"400 Bad Request"** ? Os dados enviados est�o incorretos ou incompletos.
* ?? **"401 Unauthorized"** ? O Token JWT n�o foi gerado corretamente ou expirou.
* ?? **"409 Conflict"** ? Tentativa de criar um recurso (m�quina, usu�rio) com dados j� existentes.

