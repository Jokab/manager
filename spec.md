Nedan följer en sammanhållen specifikation för att implementera en första prototyp av ditt fantasy manager-spel med draft, baserat på de krav och önskemål som framkommit i diskussionen.

---

## Översikt

**Syfte**
Att skapa en prototyp av ett webbaserat fantasy-fotbollsspel där användare bildar ligor, genomför en draft med unika spelare och sätter ihop sina trupper (22 spelare). Under turneringens gång väljer varje deltagare en startelva per omgång för att samla poäng baserat på matchhändelser.

**Huvudfunktioner**
1. **Skapa & Hantera Ligor**
   - Användare ska kunna registrera sig (enkel inloggning med användarnamn/lösenord).
   - Inloggade användare kan skapa en ny liga och bjuda in andra (exempelvis via en länk eller ligakod).
2. **Draft i Realtid (SignalR)**
   - När alla deltagare i ligan är på plats kan en “Start Draft”-funktion anropas.
   - Draften genomförs i turordning (ex. 1→2→3→4→4→3→2→1...).
   - När en spelare väljs av en deltagare försvinner denne från listan av tillgängliga spelare för alla andra.
   - Deltagarna väljer tills alla har 22 spelare (2 målvakter, 8 försvarare, 8 mittfältare, 4 anfallare) och max 4 spelare från samma land.
3. **Poäng och Statistik**
   - Ligan definierar ett poängsystem innan draften (t.ex. mål = 3 poäng, vinst = 1 poäng).
   - Efter varje match uppdateras poäng för spelarna i lagens startelvor.
   - Totalsumma visas i en ligatabell.
4. **Startelva & Trupp**
   - Varje användare ska alltid ha en trupp på 22 spelare.
   - Inför varje omgång (gruppspel eller slutspel) väljer man 11 av dessa 22 att “starta” och få poäng.
   - Byten i startelvan är tillåtna tills strax innan första matchstart för omgången.
5. **Slutspelsdraft**
   - När gruppspelet är slut får varje deltagare byta ut upp till 3 spelare i en ny mini-draft, med samma turordningslogik.
   - Spelare som redan är tagna i tidigare draftar är fortsatt otillgängliga.
6. **Avslutning**
   - När finalen spelats stängs ligan automatiskt och en slutlig topplista visas.

---

## Tekniska Val

1. **Backend/Server**
   - **ASP.NET Core 9** (C#).
   - **SignalR** för realtidsfunktioner (t.ex. vid draftval).
   - In-memory datalagring (för prototyp): T.ex. `InMemoryDatabase` eller en enkel lista i minnet, med en längre sikt på att byta till SQL/NoSQL.

2. **Frontend**
   - **Blazor Server** eller **Blazor WebAssembly** (valfritt, men Blazor Server + SignalR brukar vara smidigt).
   - Realtidsuppdatering via SignalR-hubbar.
   - Enkel, funktionell UI utan avancerad styling till att börja med.

3. **Mockad/Enkel “Match-Engine”**
   - Simulerar eller manuellt triggar resultat för matcher.
   - Kan vara hårdkodade data eller import av existerande VM/EM-spelare + påhittade eller historiska resultat.
   - Uppdaterar poängen i systemet när matchen är “klar”.

---

## Datamodell (Föreslagen struktur)

För en första prototyp kan vi använda oss av enkla C#-klasser i serverprojektet. Nedan är ett förslag på hur modellerna kan se ut i kodexempel:

```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }  // Unikt användarnamn
    public string PasswordHash { get; set; } // Beroende på hur vi vill hantera lösenord
    public List<Team> Teams { get; set; } = new();
}

public class League
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string InviteCode { get; set; }   // Valfritt sätt att bjuda in
    public LeagueSettings Settings { get; set; }
    public List<User> Participants { get; set; } = new();
    public bool DraftStarted { get; set; }
    public bool DraftCompleted { get; set; }
    public bool IsKnockoutDraftCompleted { get; set; } // För mini-draften
    public List<MatchResult> MatchResults { get; set; } = new();
    public List<DraftPick> DraftPicks { get; set; } = new();
}

public class LeagueSettings
{
    public int MaxPlayersFromSameCountry { get; set; } = 4;
    // Poängsystem
    public int PointsPerGoal { get; set; } = 3;
    public int PointsPerWin { get; set; } = 1;
    // ...fler inställningar kan läggas till vid behov
}

public class Team
{
    public int Id { get; set; }
    public User Owner { get; set; }
    public League League { get; set; }
    public string TeamName { get; set; }
    public List<Player> DraftedPlayers { get; set; } = new();
    public List<Player> StartingEleven { get; set; } = new();
    // Poäng per omgång, totala poäng
    public int TotalPoints { get; set; } = 0;
}

public class Player
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Country { get; set; }
    public Position Position { get; set; }
    public bool Eliminated { get; set; } // True om landet åkt ut
}

public enum Position
{
    Goalkeeper,
    Defender,
    Midfielder,
    Forward
}

public class DraftPick
{
    public int Id { get; set; }
    public League League { get; set; }
    public User User { get; set; }
    public Player PickedPlayer { get; set; }
    public int Order { get; set; } // I vilken turordning detta gjordes
}

public class MatchResult
{
    public int Id { get; set; }
    public DateTime MatchDate { get; set; }
    public string HomeTeamCountry { get; set; }
    public int HomeGoals { get; set; }
    public string AwayTeamCountry { get; set; }
    public int AwayGoals { get; set; }
    // Här kan man även lägga in info om vilka spelare som gjort mål, etc.
}
```

> **Obs:** Detta är en väldigt förenklad modell; man kan behöva ytterligare entiteter (t.ex. tabell för “Events” i en match om man vill spara mål, gula kort m.m.).

---

## Flöden och Funktionalitet

### 1. Registrering och Inloggning

1. **Registrera användare**
   - En enkel sida där man anger användarnamn och lösenord.
   - Inga avancerade krav på säkerhet i prototypen.
2. **Logga in**
   - Användarnamn + lösenord valideras mot in-memory-databasen.
   - Skapa en session eller Blazor-state.

### 2. Skapa och Gå med i Liga

1. **Ligaskapande**
   - En inloggad användare klickar “Skapa ny liga”.
   - Ange **liganamn** och generera en **invitekod** (eller liknande) som andra kan använda för att ansluta.
   - Ange (valfritt) poänginställningar (mål = x poäng, vinst = y poäng, etc.).
   - Spara i “LeagueSettings” i databasen.
2. **Gå med i en liga**
   - Användare anger invite-kod eller väljer en publik liga (om det finns).
   - Systemet skapar automatiskt ett nytt “Team” kopplat till användaren och ligan.
   - Användaren får ange “TeamName”.

### 3. Initiera Draft (Gruppspels-draft)

1. **Starta**
   - Ligaskaparen klickar på “Start Draft” när alla deltagare är redo.
   - Turordningen lottas (1→2→...→N och sedan tvärtom i “snake”-stil, eller enligt överenskommen ordning 1→2→3→4→4→3→2→1 etc.).
2. **Draft-läget (SignalR)**
   - Visa en realtidsvy för alla anslutna.
   - **Komponenter**:
     - Lista över **Tillgängliga spelare** (ej draftade, matchande positions/lands-regler).
     - Aktiv användare markeras.
     - När en användare väljer en spelare (trycker på “Drafta”), skickas det via SignalR till servern.
       - Servern kontrollerar:
         - Att det är rätt användares tur.
         - Att spelarens land/position inte bryter regler (t.ex. max 4 från samma land).
         - Att användarens trupp inte redan är full (22 spelare).
       - Godkänns valet, sparas det i en `DraftPick`, och tas bort från listan av tillgängliga spelare.
       - SignalR-pushar uppdateringen till alla klienter (ny tur).
3. **Övergång till “DraftCompleted”**
   - När alla deltagare har 22 spelare vardera sätts `DraftCompleted` till `true` i ligan.

### 4. Hantera Startelvor och Omgångar

1. **Välja startelva**
   - Inför varje omgång (gruppspelets omgång 1,2,3, sedan åttondel, kvartsfinal etc.) får varje användare välja 11 spelare av sina 22.
   - UI:et kan vara en enkel “checklist” eller drag-and-drop.
   - Kontrollera att man har exakt 1 målvakt, 4 försvarare, 4 mittfältare och 2 anfallare om man kör formationen 4-4-2.
   - Kontrollera också att spelare inte är “Eliminated” (har landet åkt ut, ska det visas varning eller block).
   - När klockslaget för omgångens start (definieras av matchschemat) passeras, låses startelvan för den omgången.

2. **Poängberäkning**
   - Efter att en match är “klar” (manuellt eller automatiskt uppdaterat), ska systemet kolla:
     - Vilket lag vann (för vinstpoäng).
     - Vilka spelare som gjorde mål (om vi spårar det i prototypen).
   - För alla anmälda startelvor i ligan summeras poängen enligt ligans inställningar.
   - Läggs till teamets “TotalPoints”.

### 5. Slutspels-draft (Byta 3 Spelare)

1. **När gruppspelet är slut**
   - En flagga `IsKnockoutDraftCompleted` i `League`.
   - När matchresultaten för sista gruppspelsomgången registrerats, triggas en ny draft för att byta 3 spelare.
2. **Mini-draft**
   - Samma logik som i huvud-draften:
     - Lottad turordning, 1→2→3... men utan krav att välja 22 spelare totalt – man byter upp till 3 valfria.
     - Utslagna spelare (spelare vars land är ute) är spärrade i listan om de inte redan är i någons trupp. De kan inte väljas.
     - Spelare som redan draftats av någon annan är fortsatt otillgängliga.
3. **Byta ut befintliga**
   - När man plockar in en ny spelare (draftar), tas en av de gamla bort (från ens 22) för att bibehålla konstant antal.
   - Det behöver inte vara en utslagen spelare; man får fritt välja vilka man byter ut.

### 6. Avslut av Liga

1. **Final & Stängning**
   - När finalens resultat registrerats, markeras ligan som “avslutad”.
   - Visa slutlig tabell över totala poäng.
   - Ingen möjlighet att ändra i startelvor, göra ytterligare draftar etc.

---

## SignalR-kommunikation

### Huvudpunkter

- **En Hub** kan räcka, t.ex. `DraftHub`, där klienterna ansluter.
- Metoder på servern:
  - `JoinDraft(int leagueId)` – användare ansluter till rummets signal.
  - `PickPlayer(int playerId)` – den aktiva användaren gör sitt val.
- När servern godkänner ett val skickar den ut en “BroadcastPick(playerId, userId, nextUserId, updatedAvailablePlayersList, etc.)” till alla i rummets context.
- Samma Hub kan användas för mini-draften, så länge vi håller koll på om det är första draften eller slutspelsdraft.

---

## Förslag på Flöde för Prototypens Testscenarium

1. **Spelardata**
   - Använd (om möjligt) en lista över verkliga landslagsspelare från t.ex. VM/EM (cirka 300–400 spelare). Position, land, namn.
   - Man kan hårdkoda: “Land A, Land B, Land C...” om man inte vill använda riktiga landsnamn i prototyp.

2. **Matcher**
   - Skapa ett minischema med t.ex. 6–8 matcher i gruppspelet och 3–4 slutspelsmatcher. Alternativt ta ett riktigt schema för en liten turnering.
   - När en match “spelas” (klicka på “Registrera resultat”), mata in t.ex. 2–1 så att systemet vet vilket land som vann.
   - Sätt flags för vilka länder som åkt ut.

3. **Flödestest**
   - Skapa 1 liga, 3–4 användare.
   - Kör full draft tills alla har 22 spelare.
   - Fyll i startelva för omgång 1, lägg in matchresultat, se poäng.
   - Fyll i omgång 2, omgång 3... se hur poängen ökar.
   - Avsluta gruppspel → aktivera mini-draft (3 byten).
   - Spela slutspel, mata in resultat.
   - När finalen är klar → systemet stänger ligan, visar slutresultat.

---

## Säkerhets- och Robusthetsaspekter (Prototypnivå)

- **Inloggning**: Enkel hashing av lösenord är tillräcklig i prototypen.
- **Autentisering i SignalR**: Använd sessions eller `HttpContext.User` för att verifiera vem som gör draftval.
- **Felsituationer**: Om någon tappar nätet i en turordningsbaserad draft kan hela ligan blockeras. I prototypen förutsätter vi att alla är närvarande.
- **Ingen tidsgräns** i draften: Minskar komplexitet, men kan vara bra att visa en räkning på vems tur det är.

---

## Summering och Nästa Steg

1. **Arkitektur**:
   - ASP.NET Core + Blazor (Server) + SignalR.
   - Alla moduler körs i en och samma applikation för prototypen.
2. **Funktion**:
   - Registrering/inloggning.
   - Skapa/Gå med i liga.
   - Draft (22 spelare, 4-4-2, max 4 från samma land).
   - Omgångshantering, välja startelva.
   - Matchresultat + poängberäkning.
   - Slutspelsdraft (3 byten).
   - Avslut när finalen är slut.
3. **Gränssnitt**:
   - Enkla sidor/komponenter i Blazor för:
     - (1) Ligaskapande och inställningar.
     - (2) Draft-vy.
     - (3) Översikt över sin trupp & startelva.
     - (4) Översikt (Tabell) över totalpoäng för ligan.
4. **Test & Demo**:
   - Hårdkoda eller läs in en lista över spelare.
   - Skapa exempelmatcher och gå igenom hela turneringen steg för steg.

Med denna specifikation kan en utvecklare eller AI ta projektet och börja implementera stegvis. Fokus bör ligga på en fungerande draftprocess, enkel datahantering i minnet, samt en prototyp av matchresultat/poänglogik – därefter kan man iterera och förfina funktioner och UI.

---

**Lycka till med implementationen!**