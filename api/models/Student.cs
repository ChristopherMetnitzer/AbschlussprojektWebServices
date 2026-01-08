namespace Abschlussprojekt.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Matrikelnummer { get; set; } = string.Empty;
        public int Semester { get; set; }
        public string? University { get; set; }
    }
}