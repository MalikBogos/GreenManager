# GreenManager - .NET 10 Applicatie Suite

Welkom bij het GreenManager project! Deze applicatie-suite bestaat uit een ASP.NET Core backend (Web/API), een .NET MAUI mobiele applicatie en een WPF desktop applicatie. Alle projecten zijn gebouwd met **.NET 10**.

## 1. Voorbereiding & Installatie

Zorg ervoor dat je de .NET 10 SDK en Visual Studio geïnstalleerd hebt.

* Open de solution (`GreenManager.sln`) in Visual Studio.
* Herstel alle benodigde NuGet-pakketten door het volgende commando uit te voeren in de terminal, of door simpelweg de solution te 'builden':
  `dotnet restore`

## 2. Database Configuratie & Testaccounts

Het project maakt gebruik van Entity Framework Core. Je moet de database genereren voordat je de applicaties kunt gebruiken.

* Stel het **GreenManager_Wpf** project in als *Startup Project* (Rechtermuisklik -> Set as Startup Project).
* Open de **Package Manager Console** (Tools -> NuGet Package Manager -> Package Manager Console).
* Zorg dat de Models Class-Library geselecteerd is als *Default project* in de console.
* Voer het volgende commando uit om de database aan te maken:
  `update-database`

**Standaard Testaccounts (Seeded Users):**
Na het updaten van de database zijn de volgende accounts direct beschikbaar om mee in te loggen. Ze staan standaard ingesteld op geactiveerd (`EmailConfirmed = true`):
* `admin@greenmanager.be` : wachtwoord = 123
* `employee@greenmanager.be` : wachtwoord = 123
* `guest@greenmanager.be` : wachtwoord = 123

## 3. Configuratie (User Secrets)

Om veiligheidsredenen staan databaseconnecties, wachtwoorden en API-sleutels niet in de broncode. Je moet deze zelf toevoegen via 'Manage User Secrets'. Klik met de rechtermuisknop op het desbetreffende project in Visual Studio en kies **Manage User Secrets**.

### 3A. Secrets voor GreenManager_Web (ASP.NET Core)
Voeg de volgende JSON-structuur toe en vul je eigen Mailtrap inloggegevens en een veilige JWT-sleutel (aan het Web-Project) in:

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

### 3B. Secrets voor WPF Project
Voeg de volgende JSON-structuur toe aan de secrets van het WPF project voor de directe databaseconnectie:

{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Database=GreenManagerDb;Integrated Security=True;"
  }
}

## 4. Project Starten

Om de volledige applicatie te testen, moet de backend draaien om de API te hosten.

* Start eerst het **GreenManager_Web** project op zonder debug-mode (als je de MAUI-app wil testen).
* Stel vervolgens het **GreenManager_App** (.NET MAUI) of het **WPF** project in als *Startup Project* en start deze op om in te loggen met een van de testaccounts.

> ⚠️ **Let op: Werk in uitvoering (Work in Progress)**
> De documentatie van dit project bevindt zich momenteel in een actieve ontwikkelfase. Deze README, de bronnenlijst en de interne code-documentatie (XML-comments in de C# code) worden ASAP uitgebreid met meer gedetailleerde informatie over de architectuur, DTO-structuren en specifieke functionaliteiten of andere details.