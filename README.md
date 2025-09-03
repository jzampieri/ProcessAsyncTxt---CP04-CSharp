# Processador Assíncrono de Arquivos TXT (.NET 8, C#)

## Descrição

Este projeto implementa uma aplicação de console em **C# (.NET 8)** para processamento assíncrono de arquivos de texto (`.txt`).

A aplicação percorre todos os arquivos `.txt` de um diretório informado pelo usuário, processa cada um e gera um relatório consolidado com o número de linhas e palavras de cada arquivo.

## Funcionalidades

* Solicita ao usuário o caminho de um diretório.
* Localiza todos os arquivos `.txt` na pasta.
* Processa cada arquivo de forma assíncrona e paralela, mantendo o console responsivo.
* Conta:

  * Quantidade de **linhas**.
  * Quantidade de **palavras** (com suporte a acentuação e caracteres especiais).
* Gera um relatório em `./export/relatorio.txt`, no formato:

  ```
  arquivo1.txt - 100 linhas - 560 palavras
  arquivo2.txt - 42 linhas - 180 palavras
  ```

## Requisitos

* **.NET 8 SDK**
* Visual Studio 2022 (ou superior) ou Visual Studio Code

## Como executar

### Visual Studio

1. Clone ou baixe o repositório.
2. Abra a solução no Visual Studio.
3. Compile o projeto (`Ctrl + Shift + B`).
4. Execute (`Ctrl + F5`).
5. Digite o caminho de um diretório que contenha arquivos `.txt`.
6. Ao final, verifique o relatório gerado em `./export/relatorio.txt`.

### CLI (opcional)

```bash
dotnet build
dotnet run
```

## Estrutura do projeto

```
ProcessAsyncTxt/
 ├── ProcessAsyncTxt.csproj
 ├── Program.cs
 └── export/
     └── relatorio.txt   (gerado após execução)
```

