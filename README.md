# Správa Knih

Tento projekt je kompletní systém pro správu knih, který umožňuje uživatelům prohlížet, vyhledávat, přidávat nové knihy a spravovat jejich výpůjčky. Skládá se z ASP.NET Core Web API backendu a Blazor WebAssembly klientské aplikace.

## Funkce

- **Prohlížení knih**: Zobrazení seznamu všech dostupných knih
- **Vyhledávání**: Hledání knih podle názvu, autora nebo ISBN
- **Přidávání knih**: Vytváření nových záznamů knih s validací
- **Výpůjčky**: Možnost vypůjčit a vrátit knihy (sledování dostupného množství)
- **REST API**: Kompletní RESTful API pro integraci s dalšími systémy
- **Swagger dokumentace**: Interaktivní API dokumentace

## Technologie

- **Backend**: ASP.NET Core 8 Web API
- **Frontend**: Blazor WebAssembly
- **Databáze**: SQL Server
- **ORM**: Entity Framework Core
- **UI Framework**: Bootstrap 5

## Architektura

Projekt je rozdělen do několika vrstev:

- **BookManagement.API**: ASP.NET Core Web API s kontrolery
- **BookManagement.Client**: Blazor WebAssembly aplikace
- **BookManagement.Common**: Sdílené entity, požadavky, odpovědi a výjimky
- **BookManagement.DbService**: Služba pro přístup k databázi
- **BookManagement.Services**: Business logika

## Požadavky

- .NET 8.0 nebo vyšší
- SQL Server (lokální nebo vzdálený)
- Visual Studio 2022 nebo Visual Studio Code

## Instalace a spuštění

### 1. Klonování repozitáře

```bash
git clone <repository-url>
cd scio-knihy
```

### 2. Nastavení databáze

Aktualizujte připojovací řetězec v `Book-managmenet/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=your-database;User Id=your-username;Password=your-password;TrustServerCertificate=true;Encrypt=false;"
  }
}
```

### 3. Spuštění backendu

```bash
cd Book-managmenet
dotnet restore
dotnet build
dotnet run
```

### 4. Spuštění klienta

```bash
cd BookManagement.Client
dotnet restore
dotnet build
dotnet run
```

## API Endpoints

### Knihy

- `GET /books` - Získání všech knih
- `GET /books/name/{name}` - Hledání podle názvu
- `GET /books/author/{author}` - Hledání podle autora
- `GET /books/isbn/{isbn}` - Získání knihy podle ISBN
- `POST /books/create` - Vytvoření nové knihy
- `POST /books/lend/{isbn}` - Výpůjčka knihy
- `POST /books/return/{isbn}` - Vrácení knihy

### Externí API

Projekt také obsahuje `ExternalController` pro integraci s externími službami.

## Datový model

### Book

```csharp
public class Book
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? ISBN { get; set; }
    public short PublicationYear { get; set; }
    public short AmountAvailable { get; set; }
}
```

## Validace

- **ISBN**: 14-17 znaků
- **Rok vydání**: Maximálně aktuální rok + 1 (pomocí `MaxYearValidationAttribute`)
- **Název a Autor**: Povinné, minimálně 2 znaky
- Všechny zprávy na backendu jsou v angličtině, jelikož je to univerzální jazyk. Na frontendu se vše zobrazuje v češtině.

## Middleware

- **ExceptionMiddleware**: Globální handling výjimek s přívětivými chybovými zprávami

## Testování

### Jednotkové testy (Unit Tests)

Projekt obsahuje komprehenzivní testovací sadu s **43 testy** pomocí **xUnit** a **Moq**:

```bash
cd BookManagement.Tests
dotnet test
```

#### Pokryté testy:

- **BookServiceTests** (26 testů): Business logika pro práci s knihami

  - `GetAll`, `GetByISBN`, `GetByName`, `GetAllByAuthor`
  - `CreateBook`, `DecreaseAvailableBookAmount`, `IncreaseAvailableBookAmount`
  - Testy chybových stavů a výjimek

- **BooksControllerTests** (11 testů): HTTP API endpointy

  - GET endpoints (seznam, vyhledávání)
  - POST endpoints (vytváření, výpůjčky, vrácení)
  - Validace odpovědí a chybových stavů

- **MaxYearValidationAttributeTests** (5 testů): Vlastní validace

  - Kontrola validity roku vydání
  - Ověření budoucích let

- **ExceptionTests** (6 testů): Vlastní výjimky

  - `FailedToCreateException`
  - `NotFoundException`
  - `FailedToLendExpection`

- **RequestResponseTests** (2 testy): Validace požadavků a odpovědí
  - Validace ISBN formátu
  - Kontrola povinných polí

### Manuální testování API

Pro manuální testování API můžete použít:

- **Swagger UI**: `https://localhost:5001/swagger` (v Development módu)
- **Postman** nebo podobné nástroje
- **Integrované HTTP soubory**: `Book-managmenet.http` v root adresáři

### Spuštění testů s pokrytím kódu

```bash
cd BookManagement.Tests
dotnet test /p:CollectCoverageMetrics=true
```

## Budoucí zlepšení a Roadmap

Zde jsou návrhy na vylepšení projektu pro budoucí vývoj:

1. Možnost aplikace Rate Limitingu
2. Uživatelské účty, autorizace a autentikaci
3. Audit
4. Vylepšit Logování - zvážit možnost logování do souboru nebo do db
5. Vytvořit BookManagement.Repository v případě více entit. Aktuálně stačí generická DbService.

## Přispívání

1. Forkněte projekt
2. Vytvořte feature branch (`git checkout -b feature/AmazingFeature`)
3. Commitněte změny (`git commit -m 'Add some AmazingFeature'`)
4. Pushněte do branch (`git push origin feature/AmazingFeature`)
5. Otevřete Pull Request

## Licence

Tento projekt je licencován pod MIT License - viz soubor LICENSE pro detaily.

## Kontakt

Pro otázky nebo návrhy mě kontaktujte - karelgeyer@gmail.com.
