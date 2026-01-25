import { defineStore } from 'pinia'
import Student from '@/types/Student'
import axios from 'axios'



// Load Balancer implementieren
const SERVICE_URLS = [
  "http://localhost:5187",
  "http://localhost:5188",
  "http://localhost:5189",
];
// Round-Robin Zähler
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
        // Nachweis (optional): zeigt in der Konsole welche Instanz getroffen wurde
        const info = await axios.get('/info', { baseURL });
        console.log('ROUND-ROBIN HIT:', info.data);

        const responseData = response.data;
        let studentsList: Student[] = [];
        let universityName = '';
        
        if (Array.isArray(responseData)) {
            studentsList = responseData;
        } else if (responseData && Array.isArray(responseData.data)) {
             // Kleinbuchstaben 'data' (camelCase JSON Standard)
            studentsList = responseData.data;
            universityName = responseData.university || '';
        } else if (responseData && Array.isArray(responseData.Data)) {
            // PascalCase 'Data' (falls der JSON-Serializer die Großschreibung beibehält)
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
    // Student anhand der ID abrufen
    async getStudentById(id: number) {
      this.loading = true;
      this.error = null;
      try {
        const baseURL = pickServiceUrl();
        const { data } = await axios.get<Student>(`/api/Students/${id}`, { baseURL });
        this.currentStudent = data;
      } catch (err: any) {
        this.error = err.message || 'Laden des Studenten fehlgeschlagen';
        console.error(err);
      } finally {
        this.loading = false;
      }
    },

    async createStudent(student: Student) {
        this.loading = true;
        try {
            const baseURL = pickServiceUrl();
            // Zugriff auf den API-Schlüssel aus den Umgebungsvariablen (.env Datei)
            // Vite stellt Env-Variablen über import.meta.env bereit
            const apiKey = import.meta.env.VITE_API_KEY;

            // Header-Konfiguration erstellen
            const config = {
                baseURL,
                headers: {
                    'X-API-KEY': apiKey
                }
            };
            
            const { data } = await axios.post<Student>('/api/Students', student, config);
            this.students.push(data);
        } catch(error: any) {
            console.error('Erstellen fehlgeschlagen!', error);
            this.error = error.message; // Fehler im State speichern
            throw error; // WICHTIG: Fehler weiterwerfen, damit die UI ihn bemerkt!
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

    // CSV Download
    async downloadStudentsCsv() {
      try {
        const baseURL = pickServiceUrl();
        // Anfrage an den Export-Endpunkt
        const response = await axios.get('/api/Students/export', {
          baseURL,
          responseType: 'blob', // Damit wir die Datei als Blob erhalten
          headers: {
              'Accept': 'text/csv'
          }
        });

        // Simuliere einen Klick auf einen unsichtbaren Link
        const url = window.URL.createObjectURL(new Blob([response.data]));
        const link = document.createElement('a');
        link.href = url;
        link.setAttribute('download', 'students.csv');
        document.body.appendChild(link);
        link.click();
        
        // Aufräumen
        link.remove();
      } catch (error) {
        console.error('CSV Download fehlgeschlagen', error);
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