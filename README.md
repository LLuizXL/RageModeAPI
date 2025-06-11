# RageModeAPI

![Visitors](https://api.visitorbadge.io/api/visitors?path=https%3A%2F%2Fgithub.com%2FLLuizXL%2FRageModeAPI&countColor=%23263759)
![GitHub Stars](https://img.shields.io/github/stars/LLuizXL/RageModeAPI)
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)

---

## 🔗 Links do Projeto

- **Frontend (React)**: https://github.com/Joaquimtxt/RageModeReact  
- **Site Hospedado**: https://rage-mode-react.vercel.app/  
- **Backend (esta API)**: https://github.com/LLuizXL/RageModeAPI  
- **API online (no SOMEe)**: http://apiragemode.somee.com/

---

## ⚙️ Sobre a RageModeAPI

Essa API, desenvolvida em C# com ASP.NET Core e Entity Framework, é o backend do **RageMode React Forum**, consumido pelo frontend hospedado na Vercel.

Ela gerencia operações CRUD completas para entidades principais:

- **Usuários**  
  - Cadastro, autenticação, edição de perfil e upload de foto
- **Jogos**  
  - Lista de jogos, criação, edição e exclusão
- **Personagens**  
  - Relacionados a um jogo; inclui movesets, combos, imagens
- **Posts**  
  - Criar e editar conteúdo post, associar post ao autor,
  - Relacionado ao personagem/jogo, mandar imagem para post
- **Interações**  
  - Likes, dislikes, comentários, respostas a comentários.
- **Seguidores**  
  - Seguir usuários.

### 🎯 Principais Endpoints


# 💻 Rodando Localmente

## ⚙️ Requisitos

- Windows com **Visual Studio Community 2022**
- SDK .NET 8.0+
- Banco de dados SQL Server (LocalDB ou SQL Server)
- Git instalado

## 📦 Pacotes NuGet Necessários

Antes de rodar, verifique se os seguintes pacotes estão instalados (conforme a imagem):

| Pacote | Função |
|--------|--------|
| `Microsoft.EntityFrameworkCore` | ORM principal |
| `Microsoft.EntityFrameworkCore.SqlServer` | Suporte ao SQL Server |
| `Microsoft.EntityFrameworkCore.Design` | Ferramentas de design para EF |
| `Microsoft.EntityFrameworkCore.Tools` | Suporte a comandos EF no terminal |
| `Microsoft.EntityFrameworkCore.InMemory` | Banco em memória para testes |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | Autenticação com Identity |
| `Microsoft.VisualStudio.Web.CodeGeneration.Design` | Geração de código e scaffolding |
| `Swashbuckle.AspNetCore` | Swagger UI para documentação da API |

Você pode instalar todos no **Gerenciador de Pacotes NuGet** ou via terminal:

```bash
Install-Package Microsoft.EntityFrameworkCore
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Design
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.EntityFrameworkCore.InMemory
Install-Package Microsoft.AspNetCore.Identity.EntityFrameworkCore
Install-Package Microsoft.VisualStudio.Web.CodeGeneration.Design
Install-Package Swashbuckle.AspNetCore
```

##🚀 Como executar a API localmente
1-Clone o repositório:

```bash
git clone https://github.com/LLuizXL/RageModeAPI.git
cd RageModeAPI
```
2-Abra a solução no Visual Studio 2022

Configure a string de conexão
Edite o arquivo appsettings.json com sua conexão SQL Server local:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DbRageModeApi;Trusted_Connection=True;"
}
```
3-Aplique as migrações do Entity Framework
No Console do Gerenciador de Pacotes:

```powershell
Add-Migration Initial
Update-Database
```
4-Execute a API
Aperte F5 ou clique em Executar.
A API estará disponível em:

```arduino
https://localhost:5001
http://localhost:5000
```
