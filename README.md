# Sistema de Ranking de Vendedores

![Capa do Projeto](link_para_uma_imagem_ou_gif_do_sistema)

## Descrição

Sistema para gerenciamento e ranking de vendedores, com cadastro de metas, vendas, indicadores e geração de relatórios de desempenho.

---

## Funcionalidades

- Cadastro, edição e remoção de vendedores, metas e vendas  
- Filtros avançados e pesquisa dinâmica  
- Cálculo automático de desempenho e ranking  
- Interface moderna com Blazor e MudBlazor  
- API RESTful para integração e escalabilidade  
- Organização em camadas e padrão MVVM  
- Tratamento padronizado de respostas com ResultadoOperacao  

---

## Tecnologias Utilizadas

- C# (.NET 9)  
- Blazor WebAssembly  
- MudBlazor para UI  
- Entity Framework Core com SQL Server local  
- Swagger para documentação da API  
- Git para versionamento  

---

## Como Rodar o Projeto

### Pré-requisitos

- .NET 9 SDK  
- SQL Server instalado localmente  
- Visual Studio 2022 ou VS Code  

### Passos

1. Clone o repositório  
2. Configure a string de conexão no `appsettings.json` (exemplo abaixo) ou variável de ambiente  
3. Execute as migrações para criar o banco de dados  
4. Rode a API e o front-end Blazor  
5. Acesse o sistema em [https://localhost:7061](https://localhost:7061) (porta configurada no `launchSettings.json`)  

#### Exemplo de configuração no `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=RankingDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Kestrel": {
    "Endpoints": {
      "Https": {
        "Url": "https://localhost:7061"
      }
    }
  }
}
