import { defineStore } from 'pinia'
import { ref } from 'vue'
import Student from '@/types/Student'
import axios from 'axios'

export const useStudentStore = defineStore ('student', () => {
    const students = ref<Student[]>([])
    const loading = ref(false)
    const error = ref<string | null>(null)

    const API_URL = ''

    async function fetchStudents() {
        loading.value = true
        try {
            const response = await fetch(API_URL)
            if (!response.ok) throw new Error('Failed to fetch')
            students.value = await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : 'Unknown error'
            console.error(err)
        } finally {
            loading.value = false
        }
    }

    async function addStudent(student: Student) {
        // POST Student
    }

    async function deleteStudent(id: number) {
        // DELETE Student
    }

    async function updateStudent(matrikelnummer: string, id?: number, name?: string, semester?: string) {
        // UPDATE Student
    }
})