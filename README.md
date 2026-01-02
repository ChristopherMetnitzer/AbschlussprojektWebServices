========================================================================
PROJEKTDOKUMENTATION: Student Administration Microservice
========================================================================

1. PROJEKTTEAM & AUFWAND (Aufgabe 10)
========================================================================

| Name                  | Aufgabenbereich                                      | Aufwand (ca.) |
|-----------------------|------------------------------------------------------|---------------|
| Christopher Metnitzer | Backend Core (Controller, Service, DI, REST, Config) | 3 Stunden     |
| [PLATZHALTER NAME]    | [PLATZHALTER AUFGABENBEREICH]                        | [STUNDEN]     |
| [PLATZHALTER NAME]    | [PLATZHALTER AUFGABENBEREICH]                        | [STUNDEN]     |
| [PLATZHALTER NAME]    | [PLATZHALTER AUFGABENBEREICH]                        | [STUNDEN]     |


2. SERVICE BESCHREIBUNG & DDD (Aufgabe 1)
   (Bearbeitet von: Christopher Metnitzer)
========================================================================
Bounded Context: StudentAdministrationContext
Der Service ist verantwortlich für die Verwaltung von Stammdaten der 
Studierenden.

Ubiquitous Language (Allgegenwärtige Sprache):
- Student: Repräsentiert eine eingeschriebene Person an der Hochschule.
- Matrikelnummer: Eindeutiges Identifikationsmerkmal.
- Semester: Fortschritt des Studiums.

Datenmodell:
- Entity: Student (Id, Name, Matrikelnummer, Semester).
- Validierung: ID-Prüfung bei Updates, Existenzprüfung bei Löschung.


3. ASP.NET CORE WEB API CONTROLLER (Aufgabe 2)
   (Bearbeitet von: Christopher Metnitzer)
========================================================================
Implementierung des "StudentsController" mit voller CRUD-Funktionalität.

Eingesetzte HTTP-Verben:
- GET:    Lesen von Daten (Alle Studenten oder Einzelabruf per ID).
          Status: 200 OK / 404 Not Found.
- POST:   Erstellen eines neuen Studenten.
          Status: 201 Created (inkl. Location-Header).
- PUT:    Vollständiges Update eines Studenten.
          Status: 204 No Content / 400 Bad Request.
- DELETE: Löschen eines Studenten.
          Status: 204 No Content.


4. ASYNCHRONES MESSAGING & LOGGING (Aufgabe 3)
   (Bearbeitet von: [PLATZHALTER])
========================================================================
[HIER TEXT VOM GRUPPENMITGLIED EINFÜGEN]


5. OPENAPI SPEZIFIKATION (Aufgabe 4)
   (Bearbeitet von: [PLATZHALTER])
========================================================================
[HIER TEXT VOM GRUPPENMITGLIED EINFÜGEN]


6. DEPENDENCY INJECTION & SERVICE ARCHITEKTUR (Aufgabe 5)
   (Bearbeitet von: Christopher Metnitzer)
========================================================================
Umsetzung der "Separation of Concerns":
- Interface: IStudentService (Definiert Methoden).
- Klasse:    StudentService (Implementiert Logik & In-Memory Datenhaltung).
- Injection: Constructor Injection im StudentsController.

Registrierung in Program.cs:
builder.Services.AddSingleton<IStudentService, StudentService>();

Begründung für Singleton:
Da keine Datenbank verwendet wird, liegen die Daten in einer Liste im 
Arbeitsspeicher. "Singleton" garantiert, dass diese Liste über die 
gesamte Laufzeit der Applikation erhalten bleibt und nicht bei jedem 
Request gelöscht wird.


7. CLIENT APPLIKATION (Aufgabe 6)
   (Bearbeitet von: [PLATZHALTER])
========================================================================
[HIER TEXT VOM GRUPPENMITGLIED EINFÜGEN]


8. ROUTING (Aufgabe 7)
   (Bearbeitet von: [PLATZHALTER])
========================================================================
[HIER TEXT VOM GRUPPENMITGLIED EINFÜGEN]
(Hinweis: Standard-Routing "api/[controller]" wurde von Christopher 
implementiert, Custom-Route fehlt noch von Kollegen).


9. REST PRINZIPIEN (Aufgabe 8)
   (Bearbeitet von: Christopher Metnitzer)
========================================================================
Der Service folgt strikt dem REST-Architekturstil:

A) Statelessness (Zustandslosigkeit):
Der Server speichert keinen Session-Status (Client-State). Jeder Request 
enthält alle nötigen Informationen zur Verarbeitung. Dies ermöglicht 
einfache Skalierbarkeit.

B) Uniform Interface (Einheitliche Schnittstelle):
Verwendung von ressourcen-basierten URIs (/api/students) und Standard-
HTTP-Methoden (GET, POST, PUT, DELETE) zur Manipulation der Ressourcen.


10. APPSETTINGS.JSON (Aufgabe 9)
    (Bearbeitet von: Christopher Metnitzer)
========================================================================
Konfigurationswerte wurden ausgelagert, um Hardcoding zu vermeiden.

Verwendete Keys:
- "UniversitySettings:Name"
- "UniversitySettings:Semester"

Diese Werte werden per IConfiguration in den Controller injiziert und 
in der GET-Response ausgegeben.


11. FUNKTIONIERENDE GESAMTLÖSUNG (Aufgabe 11)
    (Status Gesamtprojekt)
========================================================================
[PLATZHALTER FÜR STATUS]


12. AUTHENTIFIZIERUNG & SECURITY (Aufgabe 12)
    (Bearbeitet von: [PLATZHALTER])
========================================================================
[HIER TEXT VOM GRUPPENMITGLIED EINFÜGEN]