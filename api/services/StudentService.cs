using Abschlussprojekt.Models;

namespace Abschlussprojekt.Services
{
    public class StudentService : IStudentService
    {
        // Unsere In-Memory Datenbank
        private static readonly List<Student> _students = new List<Student>
        {
            new Student { Id = 1, Name = "Max Mustermann", Matrikelnummer = "123456", Semester = 3 },
            new Student { Id = 2, Name = "Lisa Müller", Matrikelnummer = "654321", Semester = 1 }
        };

        public List<Student> GetAll() => _students;

        public Student? GetById(int id) => _students.FirstOrDefault(s => s.Id == id);

        public void Create(Student student)
        {
            // Simuliert Auto-Increment ID
            int newId = _students.Any() ? _students.Max(s => s.Id) + 1 : 1;
            student.Id = newId;
            _students.Add(student);
        }

        public bool Update(int id, Student student)
        {
            var existing = GetById(id);
            if (existing == null) return false;

            existing.Name = student.Name;
            existing.Matrikelnummer = student.Matrikelnummer;
            existing.Semester = student.Semester;
            return true;
        }

        public bool Delete(int id)
        {
            var student = GetById(id);
            if (student == null) return false;
            
            _students.Remove(student);
            return true;
        }
    }
}