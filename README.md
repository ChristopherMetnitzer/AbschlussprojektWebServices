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

Client durch SPA realisiert.
API Verwendung ist in studentStore.ts implementiert.
In einer Tabellen Komponente werden die Daten der API dargestellt, bzw. gibt es die Möglichkeit einen Studenten zu ändern, zu löschen oder neu anzulegen.
In App.vue ist die Komponente importiert.
main.ts führt die ganze App aus. Außerdem ist hier die Adresse der API definiert (da es nur eine Adresse gibt)



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

            

MITGLIED 3: [PLATZHALTER NAME]
------------------------------
Aufgabenbereich: [PLATZHALTER]
Aufwand: [PLATZHALTER]

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



## ERWEITERTE AUFGABE 1: CONTENT NEGOTIATION
---
(Bearbeitet von: Thomas Proksch)

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
---------------------------
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




---------------------------
## Retry-Pattern (3 Punkte)

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
