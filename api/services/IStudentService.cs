using Abschlussprojekt.Models;

namespace Abschlussprojekt.Services
{
    public interface IStudentService
    {
        List<Student> GetAll();
        Student? GetById(int id);
        void Create(Student student);
        bool Update(int id, Student student);
        bool Delete(int id);
    }
}