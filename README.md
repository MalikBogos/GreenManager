```markdown
# GreenManager - .NET 10 Applicatie Suite

Welkom bij het GreenManager project! Deze applicatie-suite bestaat uit een ASP.NET Core backend (Web/API), een .NET MAUI mobiele applicatie en een WPF desktop applicatie. Alle projecten zijn gebouwd met **.NET 10**.

## 1. Voorbereiding & Installatie

Zorg ervoor dat je de .NET 10 SDK en Visual Studio geïnstalleerd hebt.

* Open de solution (`GreenManager.sln`) in Visual Studio.
* Herstel alle benodigde NuGet-pakketten door het volgende commando uit te voeren in de terminal, of door simpelweg de solution te 'builden':
  ```bash
  dotnet restore

```

## 2. Database Configuratie & Testaccounts

Het project maakt gebruik van Entity Framework Core. Je moet de database genereren voordat je de applicaties kunt gebruiken.

* Stel het **GreenManager_Wpf** project in als *Startup Project* (Rechtermuisklik -> Set as Startup Project).
* Open de **Package Manager Console** (Tools -> NuGet Package Manager -> Package Manager Console).
* Zorg dat de Models Class-Library geselecteerd is als *Default project* in de console.
* Voer het volgende commando uit om de database aan te maken:
```powershell
update-database

```



**Standaard Testaccounts (Seeded Users):**
Na het updaten van de database zijn de volgende accounts direct beschikbaar om mee in te loggen. Ze staan standaard ingesteld op geactiveerd (`EmailConfirmed = true`):

* `admin@greenmanager.be` : wachtwoord = 123
* `employee@greenmanager.be` : wachtwoord = 123
* `guest@greenmanager.be` : wachtwoord = 123

> **Let op:** Gebruikers die later via het administratiesysteem worden aangemaakt, krijgen automatisch het standaardwachtwoord `Welcome123!`.

## 3. Configuratie (User Secrets)

Om veiligheidsredenen staan databaseconnecties, wachtwoorden en API-sleutels niet in de broncode. Je moet deze zelf toevoegen via 'Manage User Secrets'. Klik met de rechtermuisknop op het desbetreffende project in Visual Studio en kies **Manage User Secrets**.

### 3A. Secrets voor GreenManager_Web (ASP.NET Core)

Voeg de volgende JSON-structuur toe en vul je eigen Mailtrap inloggegevens en een veilige JWT-sleutel in:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=GreenManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Jwt": {
    "Issuer": "GreenManagerWeb",
    "Audience": "GreenManagerWebClients",
    "Key": "VUL_HIER_EEN_GEHEIME_SLEUTEL_IN_VAN_MINIMAAL_32_KARAKTERS"
  },
  "EmailSettings": {
    "MailServer": "sandbox.smtp.mailtrap.io",
    "MailPort": 2525,
    "SenderName": "GreenManager Systeem",
    "SenderEmail": "no-reply@greenmanager.be",
    "SenderUsername": "VUL_HIER_JE_MAILTRAP_USERNAME_IN",
    "SenderPassword": "VUL_HIER_JE_MAILTRAP_WACHTWOORD_IN"
  }
}

```

### 3B. Secrets voor WPF Project

Voeg de volgende JSON-structuur toe aan de secrets van het WPF project voor de directe databaseconnectie:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Database=GreenManagerDb;Integrated Security=True;"
  }
}

```

## 4. Project Starten

Om de volledige applicatie te testen, moet de backend draaien om de API te hosten.

* Start eerst het **GreenManager_Web** project op zonder debug-mode (als je de MAUI-app wil testen).
* Stel vervolgens het **GreenManager_App** (.NET MAUI) of het **WPF** project in als *Startup Project* en start deze op om in te loggen met een van de testaccounts.

## 5. Gebruikte Dependencies & Pakketten

Hieronder vind je een overzicht van de externe pakketten en bibliotheken die binnen deze solution worden gebruikt.

### GreenManager_Web & Models (ASP.NET Core / API)

* `Microsoft.EntityFrameworkCore` (v10.0.11) - ORM framework voor database communicatie.
* `Microsoft.EntityFrameworkCore.SqlServer` (v10.0.11) - SQL Server database provider.
* `Microsoft.EntityFrameworkCore.Tools` / `.Design` (v10.0.11) - Voor database migraties en design-time tools.
* `Microsoft.AspNetCore.Identity.EntityFrameworkCore` / `.UI` (v10.0.11) - Identiteitsbeheer, rollen en UI-componenten.
* `Microsoft.AspNetCore.Authentication.JwtBearer` (v10.0.11) - Validatie van inkomende JWT-tokens.
* `System.IdentityModel.Tokens.Jwt` (v8.22.0) - Aanmaken en verwerken van JWT's.
* `Serilog.AspNetCore` (v10.0.0) / `Serilog.Sinks.File` (v7.0.0) - Gestructureerde logging naar bestanden.
* `AutoMapper` (v16.2.0) - Voor het automatisch mappen van Entities naar DTO's.
* `Swashbuckle.AspNetCore` (v10.2.3) / `Microsoft.AspNetCore.OpenApi` (v10.0.11) - Swagger UI voor API-documentatie en testen.
* `Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation` (v10.0.11) - Dynamisch compileren van Razor views tijdens runtime.
* `AspNetCore.Unobtrusive.Ajax` (v3.2.0) - Voor asynchrone server-requests zonder paginaverversing.
* `BootstrapIcons.AspNetCore` (v1.13.0) - Integratie van Bootstrap iconen.
* `Microsoft.Extensions.Configuration.Json` / `.UserSecrets` (v10.0.11) - Configuratiebeheer.
* `Microsoft.Extensions.DependencyInjection` (v10.0.11) - Beheer van services (DI).
* `Microsoft.VisualStudio.Web.CodeGeneration.Design` (v10.0.2) - Scaffolding engine.
* `Azure.Core` (v1.61.0) & `NuGet.Protocol` (v7.9.0) - Ondersteunende core-bibliotheken.

### GreenManager_App (.NET MAUI)

* `CommunityToolkit.Mvvm` - Biedt basisattributen (ObservableObject, RelayCommand) voor het MVVM-patroon.
* `sqlite-net-pcl` - Verzorgt de lokale SQLite-database verbinding voor de offline functionaliteit.
* `SQLitePCLRaw.bundle_green` - Helper-pakket voor soepele SQLite integratie op alle mobiele platformen.

### GreenManager_WPF (Desktop)

* `CommunityToolkit.Mvvm` - Houdt WPF ViewModels gestructureerd zonder 'code-behind' complexiteit.
* `Microsoft.EntityFrameworkCore.SqlServer` - Voor de directe databaseconnectie.

### Web Frontend Bibliotheken (wwwroot/lib)

* `Bootstrap (v5)` - CSS-framework voor een responsieve weergave.
* `jQuery` - DOM-manipulatie aan de client-zijde.
* `jQuery Validation` / `jQuery Validation Unobtrusive` - Client-side formuliervalidatie gekoppeld aan C# data-annotaties.

## 6. Bronnen, Inspiratie & Documentatie

Dit project en de achterliggende code-architectuur zijn deels geïnspireerd op de agenda-applicatie van de docent en de beschikbaar gestelde modules op Canvas. Daarnaast is veelvuldig gebruikgemaakt van de onderstaande documentatie met nog andere alemene documentatie en informatie-videos, blogs, stackoverflow, AI-hulpmiddelen en artikels voor algemene leerdoeleinden.

**Algemene Microsoft Documentatie**

* [ASP.NET documentation | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/)
* [.NET Multi-platform App UI documentation - .NET MAUI | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/maui/)
* [Windows Presentation Foundation for .NET documentation | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)

**Specifieke (WPF/MVVM) Tutorials & Naslagwerk**

* [WPF MVVM Tutorial: Build An App with Data Binding and Commands](https://www.youtube.com/watch?v=4v8PobcZpqM) (Tactic Devs)
* [WPF Tutorial - Fundamentals](https://wpf-tutorial.com/)
* [Create a WPF app with Visual Studio tutorial](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/get-started/create-app-visual-studio)
* [Community Toolkits for .NET Documentatie](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
* [Owned Entities - EF Core](https://learn.microsoft.com/en-us/ef/core/modeling/owned-entities)
* [Data Seeding - EF Core](https://learn.microsoft.com/en-us/ef/core/modeling/data-seeding)
* [Scaffold Identity in ASP.NET Core projects](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/scaffold-identity)

**AI Conversaties & Ondersteuning**
Tijdens de ontwikkeling van dit project is AI (Perplexity AI en Google Gemini) ingezet als algemeen hulpmiddel, voor uiteenlopende doeleinden zoals: probleemoplossing, het opstellen en structureren van het project, debugging, het begrijpen van bestaande of gegenereerde code, inspiratie bij ontwerpkeuzes, ondersteuning bij implementatie, het opstellen van de documentatie, hulp met de README en andere algemene doeleinden.

Alle code die op deze manier tot stand kwam, is door mij persoonlijk grondig doorgenomen, begrepen, en waar nodig aangepast aan de specifieke vereisten en architectuur van dit project, zoals vereist. Onderstaande gesprekken worden gedeeld zoals gevraagd:

* **Perplexity AI:**
* [Conversatie 1](https://www.perplexity.ai/search/17118226-cd6d-4a3d-ad1d-f24a638eda78)
* [Conversatie 2](https://www.perplexity.ai/search/9b0dadfc-40f7-4ab2-b73b-8f337f6aae33)
* [Conversatie 3](https://www.perplexity.ai/search/c48e0cfa-c7dc-4b33-ab16-e61eb258d0f8)
* [Conversatie 4](https://www.perplexity.ai/search/bc8f55c8-01de-4f87-a632-965ac5ac1b5b)
* [Conversatie 5](https://www.perplexity.ai/search/d2966cc7-910f-460e-920b-e695a934d003)
* [Conversatie 6](https://www.perplexity.ai/search/b82a9621-80c2-45a5-9a5c-a9fbb3a1d8cf)


* **Google Gemini:**
* [Conversatie 1](https://share.gemini.google/iQ4W9NqAczTS)
* [Conversatie 2](https://share.gemini.google/fFKF2IqTsmcJ)
* [Conversatie 3](https://share.gemini.google/YQIprHCfltvO)

