import { defineStore } from 'pinia'
import Student from '@/types/Student'
import axios from 'axios'



//Load balancer implemenierren 
const SERVICE_URLS = [
  "http://localhost:5187",
  "http://localhost:5188",
  "http://localhost:5189",
];
// Round-Robin Counter
let rrCounter = 0;

function pickServiceUrl() {
  const url = SERVICE_URLS[rrCounter % SERVICE_URLS.length];
  rrCounter++;
  return url;
}



// Pinia Store für Studenten
export const useStudentStore = defineStore('student', {
    state: () => ({
        students: [] as Student[],
        currentStudent: null as Student| null,
        loading: false,
        error: null as string | null
    }),

// Aktionen für API-Aufrufe

actions: {
    async getStudents() {
      this.loading = true;
      this.error = null;
      try {
        const baseURL = pickServiceUrl();
        const response = await axios.get('/api/Students',{baseURL});
        // Nachweis (optional): zeigt in der Console welche Instanz getroffen wurde
        const info = await axios.get('/info', { baseURL });
        console.log('ROUND-ROBIN HIT:', info.data);

        const responseData = response.data;
        let studentsList: Student[] = [];
        let universityName = '';
        
        if (Array.isArray(responseData)) {
            studentsList = responseData;
        } else if (responseData && Array.isArray(responseData.data)) {
             // Lowercase 'data' (camelCase JSON default)
            studentsList = responseData.data;
            universityName = responseData.university || '';
        } else if (responseData && Array.isArray(responseData.Data)) {
            // PascalCase 'Data' (if JSON serializer preserves casing)
            studentsList = responseData.Data;
            universityName = responseData.University || '';
        } else {
            console.error('Unexpected API response format:', responseData);
        }

        // Anzeigen von Universität falls vorhanden
        if (universityName) {
            this.students = studentsList.map(s => ({
                ...s, 
                university: s.university || universityName
            }));
        } else {
            this.students = studentsList;
        }
      } catch (err: any) {
        this.error = err.message || 'Failed to load students';
        console.error('API Error:', err);
      } finally {
        this.loading = false;
      }
    },
// Get student by ID
    async getStudentById(id: number) {
      this.loading = true;
      this.error = null;
      try {
        const baseURL = pickServiceUrl();
        const { data } = await axios.get<Student>(`/api/Students/${id}`);
        this.currentStudent = data;
      } catch (err: any) {
        this.error = err.message || 'Failed to load student';
        console.error(err);
      } finally {
        this.loading = false;
      }
    },

    async createStudent(student: Student) {
        this.loading = true;
        try {
            const baseURL = pickServiceUrl();
            const { data } = await axios.post<Student>('/api/Students', student, {baseURL});
            this.students.push(data);
        } catch(error) {
            console.error('Erstellen fehlgeschlagen!')
        } finally {
            this.loading = false;
        }
    },

    async deleteStudent(id: number) {
        this.loading = true;
        try {
            const baseURL = pickServiceUrl();
            await axios.delete(`/api/Students/${id}`, {baseURL});
            this.students = this.students.filter(s => s.id !== id);
            
            if (this.currentStudent?.id === id) {
                this.currentStudent = null;
            }
        } catch (error) {
            console.log('Löschen fehlgeschlagen!');
            throw error;
        } finally {
            this.loading = false;
        }
    },

    async updateStudent(student: Student) {
        this.loading = true;
        try {
            const baseURL = pickServiceUrl();
            await axios.put(`/api/Students/${student.id}`, student, {baseURL});
            const index = this.students.findIndex(s => s.id === student.id);
            if (index !== -1) {
                this.students[index] = student;
            }
        } catch (error) {
            console.error('Update fehlgeschlagen!');
            throw error;
        } finally {
            this.loading = false;
        }
    },
    // Test-Methode für Round-Robin Load Balancing
    async testRoundRobin(times = 9) {
    for (let i = 0; i < times; i++) {
        const baseURL = pickServiceUrl();
        try {
        const info = await axios.get('/info', { baseURL, timeout: 1000 });
        console.log("ROUND-ROBIN HIT:", info.data);
        } catch (e) {
        console.warn("INSTANCE DOWN:", baseURL);
        i--; 
        }
    }
    }


  }
})