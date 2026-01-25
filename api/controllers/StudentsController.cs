using Microsoft.AspNetCore.Mvc;
using Abschlussprojekt.Services;
using Abschlussprojekt.Models;
using Abschlussprojekt.Attributes;
using System.Linq;

namespace Abschlussprojekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _service;
        private readonly IConfiguration _config;
        private readonly IStudentEventQueue _eventQueue;
        private readonly ILogger<StudentsController> _logger;

        // Aufgabe 5: Dependency Injection im Konstruktor
        public StudentsController(IStudentService service, IConfiguration config, IStudentEventQueue eventQueue,
            ILogger<StudentsController> logger)
        {
            _service = service;
            _config = config;
            _eventQueue = eventQueue;
            _logger = logger;
        }

        // --- READ (Alle lesen) ---
        [HttpGet] // Standard Route
        [Produces("application/json", "application/xml")]  // XML-Formatierung unterstützen
        public IActionResult GetAll()
        {
            // Erweiterte Aufgabe 2: Response Caching-Header setzen
            Response.Headers["X-Debug"] = "StudentsController-GetAll";
            Response.Headers["Cache-Control"] = "public,max-age=60";

            // Aufgabe 9: Config auslesen
            string uniName = _config["UniversitySettings:Name"];            
            var students = _service.GetAll();
            
            // Wir geben die Liste und den Uni-Namen zurück
            return Ok(new { University = uniName, Data = students });
        }

        // --- READ (Export) ---
        [HttpGet("export")]
        [Produces("text/csv")]
        public IActionResult Export()
        {
            var students = _service.GetAll();
            // Gibt direkt die Liste zurück, damit der CsvOutputFormatter greift
            return Ok(students);
        }

        [HttpGet("paged")]
        public IActionResult GetPaged([FromQuery]int pageNumber = 1, [FromQuery]int pageSize = 10)
        {
            pageNumber = Math.Max(pageNumber, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            
            string uniName = _config["UniversitySettings:Name"];
            var allStudents = _service.GetAll();

            int totalCount = allStudents.Count();

            var items = allStudents
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

                return Ok(new 
                { 
                    University = uniName, 
                    PageNumber = pageNumber, 
                    PageSize = pageSize, 
                    TotalCount = totalCount, 
                    Items = items 
                });



        }


        // --- READ (Einen lesen) ---
        [HttpGet("{id:int}")]
        [Produces("application/json", "application/xml")]   // XML-Formatierung unterstützen

        public IActionResult GetById(int id)
        {
            Response.Headers["Cache-Control"] = "public,max-age=60"; // Demo/Nachweis
            var student = _service.GetById(id);
            if (student == null) return NotFound(); // 404
            return Ok(student); // 200
        }

        // --- CREATE (Erstellen) ---
        [HttpPost]
        [ApiKey] // Nur diese Methode benötigt den API-Key
        public async Task<IActionResult> Create([FromBody] Student student)
        {
            _service.Create(student);
            var created = _service.GetById(student.Id);

            _logger.LogInformation("Student CREATED: {Id}", created.Id);

            await _eventQueue.EnqueueAsync(new StudentEvent("StudentCreated",created.Id,DateTime.UtcNow));

            // 201 Created ist der korrekte Status für Erstellung
            return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
        }

        // --- UPDATE (Aktualisieren) ---
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Student student)
        {
            if (id != student.Id) return BadRequest("ID in URL und Body stimmen nicht überein");

            var success = _service.Update(id, student);
            
            if (!success) return NotFound(); // 404 wenn ID nicht existiert
            
            return NoContent(); // 204 No Content (Erfolg, aber keine Daten zurücksenden)
        }

        // --- DELETE (Löschen) ---
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = _service.Delete(id);
            if (!success)
            {
                _logger.LogWarning("DELETE failed: Student {Id} not found", id);
                return NotFound();// 404 wenn ID nicht existiert
            }

            _logger.LogInformation("Student DELETED: {Id}", id);
            await _eventQueue.EnqueueAsync(new StudentEvent("StudentDeleted", id, DateTime.UtcNow));
            return NoContent();
        }
    }
}