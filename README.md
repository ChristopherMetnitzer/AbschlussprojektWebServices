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

Der "StudentsController" ist die zentrale REST-Schnittstelle. Es sind CRUD-Operationen (Create, Read, Update, Delete) implementiert und nutzt das HTTP-Protokoll korrekt aus.

Implementierte Endpunkte und HTTP-Verben:

GET (Lesen)

- /api/students Funktion liefert alle Studenten in einer Liste. 
Status: 200 OK.

- /api/students/{id} liefert einen Studenten, bei dem die id die übergebene ist. 
Status: 200 OK (Der Student wurde gefunden) oder 404 Not Found (Student wurde nicht gefunden).

POST (Erstellen)

- Route: /api/students erstellt eine neue Ressource. 
Status: 201 Created (Der Student wurde erfolgreich erstellt)

PUT (Überschreiben)

- /api/students/{id} überschreibt den Datensatz mit der gewählten id komplett. 
Status: 204 No Content (Erfolg ohne Body) oder 400 Bad Request (Validierungsfehler).

DELETE (Löschen)

- Route: /api/students/{id} löscht den gesamten Datensatz vom Student mit dieser id.
Status: 204 No Content (Erfolg) oder 404 Not Found (wenn es die id nicht gibt).


---

## AUFGABE 3: ASYNCHRONES MESSAGING & LOGGING
---
(Bearbeitet von: [PLATZHALTER NAME])

[HIER PLATZHALTER FÜR DIE DOKUMENTATION DES KOLLEGEN]


## AUFGABE 4: OPENAPI SPEZIFIKATION
---
(Bearbeitet von: [PLATZHALTER NAME])

[HIER PLATZHALTER FÜR DIE DOKUMENTATION DES KOLLEGEN]


## AUFGABE 5: SERVICE-KLASSE & DEPENDENCY INJECTION
---
(Bearbeitet von: Christopher Metnitzer)

Um den Code und die Softwarearchitektur sauber zu machen ("Separation of Concerns"), wurde Fokus darauf gelegt, dass keine Logik im Controller implementiert wird. Es wurde anstatt dessen das Service Pattern verwendet.

Komponenten:
1. IStudentService (Interface): Legt fest, wie der StudentService aufgebaut ist und erlaubt das Testen.
2. StudentService (Implementierung): Logik und Datenhaltung sind hier definiert.

Dependency Injection:
Im Program.cs wird mittels "builder.Services.AddSingleton<IStudentService, StudentService>();" registriert.

Warum wurde Singleton gewählt:
Weil wir in diesem Projekt keine persistente Datenbank, sondern eine Liste verwenden, ist es unbedingt nötig sicher zu sein, dass es nur eine Version dieser Liste zu jedem Zeitpunkt geben kann.
- Würde ich "AddScoped" oder "AddTransient" verwenden, würde bei jedem HTTP-Request eine neue Liste erstellt werden. Damit würde ich automatisch alle Daten verlierden.
- Mit "AddSingleton" ist sicher, dass dieselbe Instanz (und damit die Daten) verfügbar sind und bleiben solange die Applikation läuft.

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

REST (Representational State Transfer) hat insgesamt sechs Einschränkungen (Constraints), die vom System erfüllt werden müssen:

1. Client-Server-Architektur
- Klare Trennung: Client macht UI, Server macht Daten und Logik
2. Statelessness (Zustandslosigkeit)
- Server speichert keine Daten über User-Sessions, Anfragen sind unabhängig.
3. Cacheability (Pufferspeicherbarkeit)
- Server sagt Client, was und wie lange es gecached werden soll
4. Layered System (Mehrschichtiges System)
- Der Client weiß nicht ob er mit dem Server, Proxy oder LoadBalancer spricht
5. Code on Demand (optional)
- Server kann Code an Client schicken, den dieser ausführt
6. Uniform Interface (Einheitliche Schnittstelle)
- Standardisierte Kommunikation durch eindeutige Adressen und feste Methoden

In meiner Implementierung des Student Administration Microservice waren folgende Prinzipien davon besonders leitend:

##### A) Statelessness (Zustandslosigkeit)

Theorie:
Vom Server aus werden keine Daten vom Client zwischen einzelnen HTTP Anfragen gespeichert.
Demnach muss jede Anfrage ohne Extrainformationen alle Infos haben, die zur Verarbeitung der Daten notwendig sind!

Es wurde folgendermaßen umgesetzt:
Ich habe im "StudentsController" KEINE Form von Session-Speicher implementiert. Jede Methode die aufgerufen wird ist vollständig alleine und ohne extra Daten funktionsfähig.
- Der Beweis hierfür ist, dass der Controller bei einem Update zwingend die ID in der URL und das Objekt im Body benötigt, weil er sich nicht auf vorherige Eingaben verlassen kann oder sollte.

##### B) Uniform Interface (Einheitliche Schnittstelle)

Theorie:
Wenn ein Client mit einem Server kommuniziert, muss diese Kommunikation/Interaktion über eine standardisierte Schnittstelle passieren. Dadurch wird sichergestellt, dass die Komponenten voneinander unabhängig/entkoppelt sind.

Es wurde folgendermaßen umgesetzt:
Es werden ressourcen-basierte URIs und Standard-HTTP-Methoden verwendet.
- Ressourcen: Die URI "/api/students" repräsentiert die Liste an Studenten.
- Verben: Anstatt Aktionen in der URL zu kodieren (kein "/createStudent"), nutze ich HTTP-Verben, so wie sie eigentlich gedacht sind:
    * GET zum Lesen
    * POST zum Erstellen
    * DELETE zum Löschen
- Status Codes: Der Server antwortet mit standardisierten Codes (HTTP Codes wie: 200, 201, 204) damit jede Form von Client die Antwort sofort versteht.

---

## AUFGABE 9: NUTZUNG DER APPSETTINGS.JSON
(Bearbeitet von: Christopher Metnitzer)

Um Hardcoding im Quellcode zu vermeiden ("Configuration over Code"), 
wurden variable Parameter in die Konfigurationsdatei ausgelagert.

##### Struktur in appsettings.json:

"UniversitySettings": {
  "Name": "FH Campus02 Business Analytics & AI",
  "Semester": "Wintersemester 2025/2026",
  "MaxStudentsPerCourse": 20
}

Implementierung:
Die Werte werden über das Interface "IConfiguration" über DI in den Controller geladen. 
Dadurch wird es möglich, Umgebungsvariablen (z.B. Semester) zu ändern, ohne den Code neu zu kompilieren und deployen.

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


Todo
