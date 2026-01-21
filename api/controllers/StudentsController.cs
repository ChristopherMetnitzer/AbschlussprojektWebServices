using Microsoft.AspNetCore.Mvc;
using Abschlussprojekt.Services;
using Abschlussprojekt.Models;


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
        [HttpGet]
        public IActionResult GetAll()
        {
            // Aufgabe 9: Config auslesen
            string uniName = _config["UniversitySettings:Name"];            var students = _service.GetAll();
            
            // Wir geben die Liste und den Uni-Namen zurück
            return Ok(new { University = uniName, Data = students });
        }

        // --- READ (Einen lesen) ---
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var student = _service.GetById(id);
            if (student == null) return NotFound(); // 404
            return Ok(student); // 200
        }

        // --- CREATE (Erstellen) ---
        [HttpPost]
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