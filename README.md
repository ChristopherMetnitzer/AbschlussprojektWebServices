# ANWENDUNG

## Start mit Docker (Empfohlen)

Um das gesamte Projekt (Backend API und Client) ohne manuelle Installation von Abhängigkeiten zu starten, verwende Docker.

**1.**  Docker Desktop (oder Docker Engine) ist installiert und läuft.

**2.**  Öffnen Sie ein Terminal im Hauptverzeichnis des Projekts und führen Sie aus:

            docker compose up --build

**3.**  Zugriff:
        Sobald die Container laufen, sind die Dienste unter folgenden Adressen erreichbar:

        Client (App): http://localhost:5173
        Backend (Swagger UI): http://localhost:5187/swagger
        API Endpunkt: http://localhost:5187/api/students


---

# PROJEKTDOKUMENTATION: Student Administration Microservice

---

## AUFGABE 1: SERVICE BESCHREIBUNG & DOMAIN DRIVEN DESIGN (DDD)
---
(Bearbeitet von: [PLATZHALTER NAME])

[HIER PLATZHALTER FÜR DIE DOKUMENTATION DES KOLLEGEN]
- Bounded Context Beschreibung
- Context Map
- Ubiquitous Language
- Datenmodell Beschreibung

---

## AUFGABE 2: ASP.NET CORE WEB API CONTROLLER (CRUD)
---
(Bearbeitet von: Christopher Metnitzer)

Der "StudentsController" stellt die zentrale REST-Schnittstelle dar. 
Er implementiert vollständige CRUD-Operationen (Create, Read, Update, Delete) 
und nutzt die Semantik des HTTP-Protokolls korrekt aus.

Implementierte Endpunkte und HTTP-Verben:

1. GET (Read) - Safe Method
   - Route: /api/students
     Funktion: Liefert eine Liste aller Studenten.
     Status: 200 OK.
   
   - Route: /api/students/{id}
     Funktion: Liefert einen spezifischen Studenten.
     Status: 200 OK (gefunden) oder 404 Not Found (wenn ID ungültig).

2. POST (Create)
   - Route: /api/students
     Funktion: Erstellt eine neue Ressource.
     Status: 201 Created. 
     Besonderheit: Es wird der Standard "CreatedAtAction" verwendet, 
     um im HTTP-Header "Location" die URL zur neu erstellten Ressource 
     zurückzugeben.

3. PUT (Update) - Idempotent
   - Route: /api/students/{id}
     Funktion: Überschreibt einen existierenden Datensatz vollständig.
     Status: 204 No Content (Erfolg ohne Body) oder 400 Bad Request 
     (Validierungsfehler).

4. DELETE (Delete)
   - Route: /api/students/{id}
     Funktion: Entfernt einen Datensatz.
     Status: 204 No Content (Erfolg) oder 404 Not Found.


---

## AUFGABE 3: ASYNCHRONES MESSAGING & LOGGING
---
(Bearbeitet von: Neveen Lazic)

Im Student-Administration-Microservice werden asynchrones Messaging und Logging
zur Verbesserung von Nachvollziehbarkeit, Entkopplung und Erweiterbarkeit eingesetzt.

### Asynchrones Messaging

Nach ausgewählten CRUD-Operationen (`Create`, `Delete`) werden **Domain-Events**
(z. B. `StudentCreated`, `StudentDeleted`) erzeugt und in eine **In-Memory-Queue**
eingereiht.  
Diese Events werden **nicht innerhalb des HTTP-Requests**, sondern asynchron
durch einen `BackgroundService` verarbeitet.

Die Implementierung erfolgt bewusst einfach über eine In-Memory-Queue,
da kein externer Message Broker erforderlich ist und der Fokus auf dem
Architekturprinzip liegt.

**Vorteile:**
- Entkopplung von Request-Verarbeitung und Folgeaktionen
- Schnelle API-Antwortzeiten
- Leicht erweiterbar (z. B. RabbitMQ, Kafka)

---

### Logging

Für das Logging wird das integrierte Logging-Framework von ASP.NET Core verwendet.
Es werden geloggt:
- erfolgreiche CRUD-Operationen im Controller
- asynchrone Event-Verarbeitung im Background Worker
- Fehlerfälle (z. B. ungültige IDs)

Die Log-Ausgaben erfolgen über die Konsole und sind sowohl lokal als auch
in Docker-Containern einsehbar.

---

### Nachweis

Beim Erstellen und Löschen eines Studenten erscheinen folgende Log-Einträge:

                                                                      
-api-1     | info: Abschlussprojekt.Controllers.StudentsController[0]
api-1     |       Student CREATED: 4
api-1     | info: StudentEventWorker[0]
api-1     | info: Abschlussprojekt.Controllers.StudentsController[0]
api-1     |       Student CREATED: 4
api-1     | info: StudentEventWorker[0]
api-1     |       ASYNC EVENT: StudentCreated | StudentId=4 | 01/21/2026 16:36:39
api-1     | info: StudentEventWorker[0]
api-1     |       ASYNC EVENT: StudentCreated | StudentId=4 | 01/21/2026 16:36:39
api-1     |       ASYNC EVENT: StudentCreated | StudentId=4 | 01/21/2026 16:36:39
api-1     | info: Abschlussprojekt.Controllers.StudentsController[0]
api-1     |       Student DELETED: 4
api-1     | info: StudentEventWorker[0]
api-1     |       ASYNC EVENT: StudentDeleted | StudentId=4 | 01/21/2026 16:37:06 

Damit ist nachgewiesen, dass Logging und asynchrones Messaging korrekt
implementiert sind.


## AUFGABE 4: OPENAPI SPEZIFIKATION
---
(Bearbeitet von: [PLATZHALTER NAME])

[HIER PLATZHALTER FÜR DIE DOKUMENTATION DES KOLLEGEN]


## AUFGABE 5: SERVICE-KLASSE & DEPENDENCY INJECTION
---
(Bearbeitet von: Christopher Metnitzer)

Um die Architektur sauber zu halten ("Separation of Concerns"), wurde 
keine Logik im Controller implementiert. Stattdessen kommt das 
Service-Pattern zum Einsatz.

Architektur-Komponenten:
1. IStudentService (Interface): Definiert den Vertrag und ermöglicht 
   Mocking/Testing.
2. StudentService (Implementation): Beinhaltet die Geschäftslogik und 
   die Datenhaltung (In-Memory Liste).

Dependency Injection (DI) Strategie:
Die Registrierung erfolgt in der Program.cs mittels:
"builder.Services.AddSingleton<IStudentService, StudentService>();"

Begründung der Scope-Wahl "Singleton":
Da in diesem Projekt keine persistente Datenbank (wie SQL) verwendet wird, 
sondern die Daten zur Laufzeit in einer List<Student> im Arbeitsspeicher 
liegen, ist "Singleton" zwingend erforderlich.
- Bei "AddScoped" oder "AddTransient" würde bei jedem HTTP-Request eine 
  neue, leere Liste erstellt werden. Daten wären sofort verloren.
- "AddSingleton" garantiert, dass dieselbe Instanz (und damit die Daten) 
  über die gesamte Laufzeit der Applikation verfügbar bleiben.

---

## AUFGABE 6: CLIENT APPLIKATION
(Bearbeitet von: Thomas Proksch)

### Client-Architektur & API-Nutzung

Der Client ist eine Vue 3 Single Page Application (SPA), die Pinia für das State Management und Axios für die HTTP-Kommunikation verwendet.

#### 1. Der Store (studentStore.ts)
Der StudentStore fungiert als Single Source of Truth. Er hält den aktuellen Zustand der Anwendung so, dass alle UI-Komponenten (z. B. Tabellen, Formulare) immer synchron sind.

- **State (Zustand):**
  - students: Eine Liste aller geladenen Studenten-Objekte.
  - loading: Ein Boolean-Flag, um Lade-Spinner in der UI anzuzeigen, während Daten geholt werden.
  - error: Speichert Fehlermeldungen, um sie dem Benutzer anzuzeigen (z. B. "Server nicht erreichbar").

- **Actions (Logik):**

  - Hier findet die eigentliche Arbeit statt. Die Actions (fetchStudents, createStudent, etc.) sind asynchrone Funktionen, die die HTTP-Aufrufe kapseln.

#### 3. Der Datenfluss (Ablauf)
1. **UI-Interaktion**: Der Benutzer klickt auf "Speichern" in der Vue-Komponente.

2. **Store-Aufruf**: Die Komponente ruft studentStore.createStudent(daten) auf.

3. **API-Request**: Der Store nutzt Axios, um einen APi-Request (POST) inkl. Security-Header an den Server zu senden.

4. **State-Update:**
    - Erfolg: Der Server antwortet mit dem neuen Studenten. Der Store fügt diesen direkt zum students-Array hinzu (die UI aktualisiert sich automatisch).
   - Fehler: Der Store fängt den Fehler, setzt den error-State und wirft den Fehler weiter, damit die UI eine Meldung anzeigen kann.

#### 2. HTTP-Kommunikation (Axios)
Axios wird innerhalb der Store-Actions verwendet, um Requests an das .NET Backend zu senden.

  - Konfiguration: Die Basis-URL der API wird aus den Umgebungsvariablen (import.meta.env.VITE_API_BASE_URL) geladen. Das macht den Wechsel zwischen Localhost und Produktion einfach.
  - Sicherheit (API Key):
  In der createStudent-Action wird Axios so konfiguriert, dass der API-Key im Header mitgeschickt wird.

---

## AUFGABE 7: ROUTING
---
(Bearbeitet von: [PLATZHALTER NAME])


[HIER PLATZHALTER FÜR DIE DOKUMENTATION DES KOLLEGEN]
(Hinweis: Das Standard-Routing [Route("api/[controller]")] wurde von 
Christopher im Backend bereits implementiert. Custom Routes fehlen noch.)

---

## AUFGABE 8: REST PRINZIPIEN
---
(Bearbeitet von: Christopher Metnitzer)


Der Service wurde strikt nach den Design-Prinzipien von Roy Fielding (REST) 
entwickelt. Besonders hervorzuheben sind:

A) Statelessness (Zustandslosigkeit)
Der Server speichert keinen Client-Kontext (Session State) zwischen zwei 
Anfragen. Jeder Request (z.B. GET /api/students/1) enthält alle Informationen, 
die zur Verarbeitung notwendig sind.
Vorteil: Der Service ist beliebig horizontal skalierbar, da keine Session-
Affinität benötigt wird.

B) Uniform Interface (Einheitliche Schnittstelle)
Die API ist ressourcen-orientiert aufgebaut. Die URIs enthalten Nomen 
(/api/students), keine Verben (/api/createStudent). Die Manipulation 
der Ressourcen erfolgt ausschließlich über die standardisierten HTTP-Verben 
(GET, POST, PUT, DELETE), was die Schnittstelle für Entwickler intuitiv 
nutzbar macht.

---

## AUFGABE 9: NUTZUNG DER APPSETTINGS.JSON
(Bearbeitet von: Christopher Metnitzer)

Um Hardcoding im Quellcode zu vermeiden ("Configuration over Code"), 
wurden variable Parameter in die Konfigurationsdatei ausgelagert.

Struktur in appsettings.json:
"UniversitySettings": {
  "Name": "FH Campus02 Business Analytics & AI",
  "Semester": "Wintersemester 2025/2026",
  "MaxStudentsPerCourse": 20
}

Implementierung:
Die Werte werden über das Interface "IConfiguration" mittels Dependency 
Injection in den Controller geladen. Dies ermöglicht es, Umgebungsvariablen 
(z.B. Semester) zu ändern, ohne den Code neu kompilieren und deployen 
zu müssen.

---

## AUFGABE 10: PROJEKTTEAM & AUFWAND
---
(Bearbeitet von: Team)


Übersicht der Verantwortlichkeiten und des geschätzten Aufwands:

MITGLIED 1: Christopher Metnitzer
---------------------------------
Aufgabenbereich: Backend Core Implementation
(Aufgaben 2, 5, 8, 9: Controller, Services, DI, REST, Config)
Aufwand: ca. 3 Stunden

MITGLIED 2: Thomas Proksch
------------------------------
Aufgabenbereich: Client
Aufgabe 6
Aufwand:
Docker: ca 0.5 Std
Client: ca 1,5 Std


MITGLIED 3: Neven Lazic
------------------------------
Aufgabenbereich: Aufgabe 3 sowie alle erweiterten Aufgaben (Messaging & Logging, Resilience-Patterns, Load Balancing & Skalierung)
Aufwand: ca. 4–5 Stunden

MITGLIED 4: [PLATZHALTER NAME]
------------------------------
Aufgabenbereich: [PLATZHALTER]
Aufwand: [PLATZHALTER]

---


## AUFGABE 11: FUNKTIONIERENDE GESAMTLÖSUNG
---
(Status des Gesamtprojekts)

Siehe 1. Punkt: Anwendung

---

## AUFGABE 12: AUTHENTIFIZIERUNG & SECURITY
---
(Bearbeitet von: Thomas Proksch)
---

### Arten der Authentifizierung im Web

#### API-Key

##### Definition

Mittels API Keys ist es möglich API-Nutzer zu identifizieren bzw. die API-Anfrage zu verifizieren, ohne den tatsächlichen Nutzer zu kennen. 
Dazu wird der API-Key mit jeder API-Anfrage mitgesendet und die Applikation autorisiert den Zugriff, oder lehnt den Zugriff ab.
Wie der API-Key übermittelt wird unterscheidet sich je nach API.

Möglichkeiten zur Übermittlung sind zB:

- Query Parameter

- API-Header

- API-Body

##### Vorteil von API-Keys

- Einfach zu implementieren


##### Nachteil von API-Keys

- Nutzer des API-Keys unbekannt

- Kann geleakt werden

##### Anwendung

API-Keys funktionieren wenn du eine Applikation authentifizierst, die dein Service nutzen möchte. Außerdem eignen sich API-Keys gut zur Monetarisierung eines Services und du die Häufigkeit der API-Anfragen regeln möchtest.

---

#### HTTP Basic Auth

##### Definition

HTTP Basic Auth ist eine einfache Möglichkeit einen User zu identifizieren. Dabei wird der Username und das Userpasswort kodiert und als Header in **jeder** HTTP-Anfrage mitgesendet.
Die kodierte Useridentifikation wird dabei ähnlich wie ein API-Key genutzt.

##### Vorteil

- Standardisierter Weg seine Berechtigung zu übermitteln

- Der HTTP-Header ist immer gleich

- Leichte Implementierung

##### Nachteil

- Die App muss das Userpasswort kennen

- Intransparente Passwortnutzung seitens der App möglich

- Die App hat Vollzugriff auf das Benutzerkonto

- Keine Zwei-Faktor-Authentifizierung möglich


##### Anwendung

HTTP-Auth ist eine einfache Möglichkeit zur Identifikation von Nutzern. Diese Methode sollte nur dann angewandt werden, wenn Sicherheit kein Risiko darstellt. Auf Grund des Sicherheitsrisikos findet HTTP-Auth heute kaum noch eine Anwendung.

---

#### OAuth 2.0

##### Definition

OAuth 2.0 ist einen auf Token basierte Architektur. Der Token, den das Service und die Applikation zur Authentifizierung verwenden, wird dabei von einem Dritten, einem Authentifizierungsserver, erstellt. Mit diesem Protokoll können Applikationen, im Namen des Nutzers, nach Zugriff auf Services fragen. Zum Beispiel kann deine Healthapplikation auf dem Smartphone, nach deinen Fitnessdaten deiner Sportswatch fragen.

###### Erstellung des Authentifizierungstokens

1. Eine Applikation fragt User nach Zugriff auf ein Service
1. Die Applikation macht eine Anfrage zur Erstellung eines Tokens an den Autorisations Server.
1. Der Autorisations Service erlaubt einer Drittanwendung und der Token wird ausgetauscht.
1. Applikation hat mit dem Token Zugriff auf das Service

##### Vorteil

- Sicherheit, Nutzerdaten bleiben geschützt
- Geregelter Zugriff auf einzelne Services
- Single Responsibility Prinzip
- Zwei-Faktor-Authentifizierung möglich

##### Nachteil

- Komplexität
- Single Point of Failure (Autorisierungsserver)
- Tracking & Datenschutz

##### Anwendung

Wenn du in einer Zero Trust Umgebung (**Internet**) mit persönlichen Informationen arbeitest, solltest du auf eine Token basierte Authentifizierung zurückgreifen, die auch Zwei-Faktor-Authentifizierung ermöglicht und einen externen Autorisierungsserver nutzt.

---

### Implementierung API-Key Authentifizierung

Die Absicherung des „Student Erstellen“-Endpunkts erfolgt durch einen Custom Action Filter in ASP.NET Core.

1. Konfiguration (appsettings.json):
Der gültige API-Schlüssel ist zentral in der Konfigurationsdatei als "ApiKey": "SecretStudentKey123" hinterlegt. Das trennt Secret und Code.

2. Validierungs-Logik (ApiKeyAttribute.cs):
Ein benutzerdefiniertes Attribut ([AttributeUsage(AttributeTargets.Method)]) fängt den Request ab, bevor er den Controller erreicht.

    - Es extrahiert den Wert aus dem HTTP-Header X-API-KEY.
    - Es vergleicht diesen Wert mit dem Eintrag in der appsettings.json.
    - Stimmt der Schlüssel nicht überein, bricht die Pipeline sofort mit 401 Unauthorized oder 403 Forbidden ab.

3. Anwendung (StudentsController.cs):
Das Attribut [ApiKey] wird deklarativ nur über der Create-Methode (POST) platziert. Dadurch bleibt die API für Lesezugriffe (GET) öffentlich zugänglich, während schreibende Operationen geschützt sind.

![API-Ablauf](assets/api.png)

![API Success](assets/success.png)

![API Denied](assets/denied.png)

---

### Security Principles nach Saltzer und Schroeder[^1]

#### Echonomy of Mechanism

*Keep it small and simple*

Ein Fehler in der Sicherheit wird bei normaler Nutzung nicht entdeckt werden. Daher muss der Code Zeile für Zeile überprüft werden. Um das Überprüfen so einfach zu machen, soll der Code klein und simpel sein.

#### Fail-safe Defaults

*Access by permission, not exclusion*

Der Standardzustand sollte immer "kein Zugriff" sein. Berechtigungen werden explizit vergeben. Wenn ein Fehler im System auftritt, führt das dazu, dass das der Zugriff verweigert wird.

#### Complete Mediation

*Check everything, every time*

Jeder Zugriff auf jedes Objekt muss jedes Mal geprüft werden. Es darf keine "Abkürzungen" geben, bei denen das System sich auf eine frühere Prüfung verlässt, da sich Berechtigungen zwischenzeitlich geändert haben könnten.

#### Open Design

*No security by obscurity*

Die Sicherheit des Systems darf nicht davon abhängen, dass der Angreifer nicht weiß, wie es funktioniert. Das Design sollte öffentlich bekannt sein können, ohne die Sicherheit zu gefährden. Nur die Schlüssel (Passwörter/Tokens) müssen geheim bleiben.

#### Separation of Privilege

*Two keys are better than one*

Ein Sicherheitsmechanismus ist stärker, wenn er von mehreren Bedingungen abhängt (Zwei-Faktor-Authentifizierung). So kann ein einzelner Fehler oder ein korrupter Mitarbeiter nicht das gesamte System kompromittieren.

#### Least Privilege

*Only the bare minimum*

Jeder Nutzer und jedes Programm sollte nur die Rechte haben, die für die aktuelle Aufgabe zwingend notwendig sind. Das begrenzt den potenziellen Schaden, falls ein Account gehackt wird oder ein Programm einen Bug hat.

#### Least Common Mechanism

*Don't share more than necessary*

Minimiere Mechanismen, die von vielen Nutzern gleichzeitig verwendet werden (zB geteilte Dateien oder globale Variablen). Geteilte Ressourcen sind potenzielle Wege für Angreifer, um von einem Nutzer zum anderen zu gelangen.

#### Psychological Acceptability

*Easy to use, hard to bypass*

Sicherheit muss für den Nutzer einfach und verständlich sein. Wenn Sicherheitsmaßnahmen zu kompliziert sind oder die Arbeit behindern, werden Nutzer Wege finden, sie zu umgehen und damit das gesamte System schwächen.

[^1]: [The Security Principles of Saltzer and Schroeder](https://shostack.org/blog/the-security-principles-of-saltzer-and-schroeder)


(Bearbeitet von: Thomas Proksch)



## ERWEITERTE AUFGABE 1: CONTENT NEGOTIATION
---
(Bearbeitet von: Neven Lazic)
### Response Formate
Clients sollen über Negotiation unterschiedliche Response-Formate (JSON oder XML) vom gleichen Endpunkt erhalten können
in der 'Program.cs ' wurde XML-Formater tusätzlich zum standart JSON-Formatte aktiviert
cssharp
builder.Services
    .AddControllers()
    .AddXmlSerializerFormatters();

Zwei GET-Endpunkte wurden für mehrere Response-Formater erweitert in StudentsController 
[HttpGet]
[Produces("application/json", "application/xml")]
public IActionResult GetAll()
{
    var students = _service.GetAll();
    return Ok(new StudentsResponseDto
    {
        University = _config["UniversitySettings:Name"],
        Data = students.ToList()
    });
}
[HttpGet("{id:int}")]
[Produces("application/json", "application/xml")]
public IActionResult GetById(int id)
{
    var student = _service.GetById(id);
    if (student == null) return NotFound();
    return Ok(student);
}

JSON anfordern:
in bash:
 curl -H "Accept: application/json" -i http://localhost:5187/api/students/1

TP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Date: Fri, 23 Jan 2026 18:16:32 GMT
Server: Kestrel
Cache-Control: public,max-age=60
Transfer-Encoding: chunked
Vary: Accept-Encoding

{"id":1,"name":"Max Mustermann","matrikelnummer":"123456","semester":3,"university":null}

XML anfordern:
in bash:
curl -H "Accept: application/xml" -i http://localhost:5187/api/students/1
TP/1.1 200 OK
Content-Length: 216
Content-Type: application/xml; charset=utf-8
Date: Fri, 23 Jan 2026 18:17:40 GMT
Server: Kestrel
Cache-Control: public,max-age=60
Vary: Accept-Encoding

<Student xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><Id>1</Id><Name>Max Mustermann</Name><Matrikelnummer>123456</Matrikelnummer><Semester>3</Semester></Student>

Accept-Header liefert die API automatisch das gewünschte  Response-Format (application/json oder application/xml)
### Custom Formatter 

Für den Endpoint /api/students/export wurde ein custon CSV Output Formatter implemetiert
Der Formater wird automatisch verwendet, wenn der Client im Accept- Header text/csv anfordert.

curl -H "Accept: text/csv" -i http://localhost:5187/api/students/export

TP/1.1 200 OK
Content-Type: text/csv; charset=utf-8
Date: Fri, 23 Jan 2026 19:27:57 GMT
Server: Kestrel
Cache-Control: public,max-age=60
Transfer-Encoding: chunked
Vary: Accept-Encoding
X-Debug: StudentsController-GetAll

id,name,matrikelnummer,semester
1,Max Mustermann,123456,3
2,Lisa Müller,654321,1

der CSV-Export ermöglicht eine einfache Weiterverarbeitung der Daten und(z.B in Excel)
---
### Dokumentation der unterstützen Media Types (OpenAPI)
Die unterstützten Media Types werden im Web Service über das [Produces]-Attribut an den Endpunkten definiert und sind dadurch automatisch in der OpenAPI-/Swagger-Dokumentation sichtbar.

[Produces("application/json", "application/xml", "text/csv")]

Unterstütze Media Types:
  application/json (standardformat)
  application/xml
  text/csv(Custom Output Formatter)
  
Voteile  den Content Negotiation sind:
  Ein Endpoint kann mehrere Response-Formate bereitstellen
  Hohe Flexibilität für unterschiedliche Clients
  Keine redundaten Endpunkte notwendig

Nachteile den Content Negotiation sind:
  Höherer Implementierungs- und Testaufwand
  Koplexität bei fFormatierung und Debugging
  Clients müssen korekt  Accept-Header senden

---
## ERWEITERTE AUFGABE 2: RESILIANCE & PERFORMANCE PATTERNS 
---
(Bearbeitet von: Neven Lazic)

In dieser Aufgabe wurden Performance- und Resilience-Patterns implementiert,
um die Robustheit, Skalierbarkeit und Effizienz des Student-Microservices
in einem verteilten System zu verbessern.

### Response Caching
Der GET-Endpunkt '/api/stdents' liefert einen 'Cache-Contol' Header, wo dir Response für Clients/Proxies cachebar ist.
 curl -i http://localhost:5187/api/students

TP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Date: Fri, 23 Jan 2026 13:36:13 GMT
Server: Kestrel
Cache-Control: public,max-age=60
Transfer-Encoding: chunked
Vary: Accept-Encoding
X-Debug: StudentsController-GetAll

### Nachweis: Unterschied in der Response-Zeit
Zur Demonstation wurde ein Demo-Endpunkt verwendet, der eine teure Operation simuliert.
Bei wiederholte Request innerhalb der Cache-Dauer ist die Antwortzeit reduziert

curl -w "\nTime: %{time_total}\n" -o /dev/null -s http://localhost:5187/api/students/cache-demo

curl -w "\nTime: %{time_total}\n" -o /dev/null -s http://localhost:5187/api/students/cache-demo

Time: 0.013725
Time: 0.006718

---
## Pagination 
### Implementierung: Skip/Take mit Query-Parametern

Es wurde ein eigener Endpunkt implementiert:

- **GET** `/api/students/paged?pageNumber=1&pageSize=10`

Dabei werden die Parameter wie folgt verwendet:
- `pageNumber` = aktuelle Seite (startet bei 1)
- `pageSize` = Anzahl der Elemente pro Seite

Die eigentliche Pagination erfolgt über:

- `Skip((pageNumber - 1) * pageSize)`
- `Take(pageSize)`

---

### Metadaten in der Response

Zusätzlich zur paginierten Ergebnisliste werden Metadaten zurückgegeben:

- `TotalCount` (Gesamtanzahl Datensätze)
- `PageNumber`
- `PageSize`

Dadurch kann der Client die Anzahl der Seiten berechnen und korrekt navigieren.

---

### Nachweis (curl)

**Request:**
 bash
curl -i "http://localhost:5187/api/students/paged?pageNumber=1&pageSize=1"

TP/1.1 200 OK
tContent-Type: application/json; charset=utf-8
Date: Fri, 23 Jan 2026 13:53:52 GMT
eServer: Kestrel
Cache-Control: public, max-age=10
Transfer-Encoding: chunked
Vary: Accept-Encoding
{"university":"FH Campus02","pageNumber":1,"pageSize":1,"totalCount":2,"items":[
{"id":1,"name":"Max Mustermann","matrikelnummer":"123456","semester":3,"university":null}]}




---
## Retry-Pattern 

### Ziel
Bei externen Service-Calls treten in verteilten Systemen häufig **transiente Fehler** auf (kurzzeitige Netzwerkprobleme, Timeouts, kurzzeitige Nichtverfügbarkeit).  
Durch ein **Retry-Pattern** werden solche temporären Fehler abgefangen, indem der Request kontrolliert erneut ausgeführt wird.
in ExternalUnbsableService.cs ist den dervice instabil implemetier und hat eine Fehlerquate von 50%

### Retry mit Polly
In Program.cs wird eine Polly Retry-Policy registriert
Es wird maximal 3 Widerholungen durchgeführt
und Backoff ist implemetiert der verhindert Request-Spikes bei AUsfällen

curl -i http://localhost:5187/api/external/data
TP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Date: Fri, 23 Jan 2026 15:34:15 GMT
Server: Kestrel
Cache-Control: public, max-age=10
Transfer-Encoding: chunked
Vary: Accept-Encoding
{"data":"External service data","attempts":3}

api-1 Attempt 1 to call external service.
api-1 info: Abschlussprojekt.Controllers.ExternalController[0]
api-1 Attempt 2 to call external service.
api-1 info: Abschlussprojekt.Controllers.ExternalController[0]
api-1 Attempt 3 to call external service.

In verteilte Systeme sind diese Patterns wichtig weil:
  -Externe Abängigkeit(APIs, Services,Netzwerk) sind nicht zuverlässsig- kurze Ausfälle sind normal.
  -Viele Fehler loösen sich von selbst.
  -Retry erhöht die Robustheit.
  -Begrentze Retries + Backoff verhindern, dass ein Ausfall durch zu viele Wiederholungen noch versärkt wird


## ERWEITERTE AUFGABE 3: LOAD BALANCING & SKALIERUNG
---
(Bearbeitet von: Neven Lazic)

### Mehrere Instanzen (Horizontale Skalierung)
Der Student-Microservice wurde parallel auf mehreren Ports gestartet.
Die gleichzeitige Ausführung der Instanzen ist anhand der laufenden Prozesse
in mehreren Terminal-Fenstern ersichtlich (Ports 5187, 5188, 5189).

### Monitoring / Status – `/info` Endpoint
Der `/info`-Endpoint liefert den Port sowie einen UTC-Timestamp der aktuell
antwortenden Instanz. Dadurch kann eindeutig festgestellt werden, welche
Instanz eine Anfrage verarbeitet hat.

{"instanceId":"aedc5a228c3d494fab49974d0ced0078","port":5187,"timestampUtc":"2026-01-23T11:47:44.581562Z"}
{"instanceId":"9034bd99b50f4b5ca89c08448368f831","port":5189,"timestampUtc":"2026-01-23T11:47:51.3534882Z"}
{"instanceId":"3bb02861a8a042abb9b55ba2458d9289","port":5188,"timestampUtc":"2026-01-23T11:47:57.3334185Z"}

### Client-seitiges Load Balancing (Round-Robin)
Im Client wird ein Round-Robin-Mechanismus verwendet, der Requests zyklisch
auf eine Liste von Service-URLs verteilt. Die Auswahl der Instanz erfolgt
über einen Modulo-Operator.

In der Browser-Konsole ist sichtbar, dass aufeinanderfolgende Requests
abwechselnd an unterschiedliche Ports (5187, 5188, 5189) gesendet werden.

ROUND-ROBIN HIT:
{instanceId: 'd933a48bf99f4ae490da4071391dc49d', port: 5188, timest
ampUtc: '2026-01-21T17:14:57.7721023Z'}
ROUND-ROBIN HIT:
{instanceId: '5e1aa0e6dc3f42e9a943df31f193efbd', port: 5189, timest
ampUtc: '2026-01-21T17:14:57.794721Z'}
ROUND-ROBIN HIT:
{instanceId: 74027a511fdb4069ae207ba942f13edd',
ampUtc: '2026-01-21T17:14:57.8430499Z'}

### Bonus: Simulation eines Instanzausfalls
Der Ausfall einer Instanz wurde simuliert, indem der Prozess einer laufenden
Instanz (Port 5187) manuell beendet wurde. Der Client erkennt den Ausfall
(`INSTANCE DOWN`) und sendet weiterhin Requests an die verbleibenden Instanzen,
die erfolgreich beantwortet werden.

ROUND-ROBIN HIT:
{instanceId: '5e1aa0e6dc3f42e9a943df31f193efbd', port: 5189, timest
ampUtc: '2026-01-21T17:15:23.6401806Z '}

ROUND-ROBIN HIT:
{instanceId: 'd933a48bf99f4ae490da4071391dc49d', port: 5188, timest
ampUtc: '2026-01-21T17:15:24.657129Z'}

INSTANCE DOWN: http://localhost: 5187
### Sticky Sessions vs. Distributed Cache

**Sticky Sessions**
Bei Sticky Sessions wird ein Client dauerhaft an eine bestimmte Service-Instanz
gebunden. Dadurch verbleibt der Session-Zustand im Speicher dieser Instanz.
Dies vereinfacht die Implementierung, schränkt jedoch die Skalierbarkeit ein
und erhöht das Ausfallrisiko.

**Distributed Cache**
Bei einem Distributed Cache (z. B. Redis) wird der Zustand zentral gespeichert.
Alle Service-Instanzen können auf diesen Zustand zugreifen, was horizontale
Skalierung und Ausfallsicherheit ermöglicht, jedoch zusätzliche Infrastruktur
und Komplexität erfordert.

Im vorliegenden Projekt wird bewusst auf Sessions verzichtet und ein stateless
Ansatz verfolgt.
