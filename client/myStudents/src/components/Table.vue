<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useStudentStore } from '@/stores/studentStore'
import type Student from '@/types/Student'
import StatisticsDialog from './StatisticsDialog.vue'

const store = useStudentStore()

const search = ref('')
const dialog = ref(false)
const dialogDelete = ref(false)
const statsDialog = ref(false) // Neuer Dialog für Statistik
const snackbar = ref(false)
const snackbarText = ref('')
const snackbarColor = ref('success')

const headers = [
  { title: 'Information', key: 'name', align: 'start' as const },
  { title: 'University', key: 'university' },
  { title: 'Matrikelnummer', key: 'matrikelnummer' },
  { title: 'Semester', key: 'semester' },
  { title: 'Actions', key: 'actions', sortable: false },
]

// Default empty item
const defaultItem = {
  id: 0,
  name: '',
  matrikelnummer: '',
  semester: 1,
  university: '',
}

const editedItem = ref<Student>({ ...defaultItem } as Student)
const editedIndex = ref(-1)

const formTitle = computed(() => {
  return editedIndex.value === -1 ? 'Neuer Student' : 'Student bearbeiten'
})

// Fetch data on mount
onMounted(() => {
  store.getStudents()
})

watch(dialog, (val) => {
  val || close()
})

watch(dialogDelete, (val) => {
  val || closeDelete()
})

function editItem(item: Student) {
  editedIndex.value = store.students.indexOf(item)
  editedItem.value = { ...item }
  dialog.value = true
}

function deleteItem(item: Student) {
  editedIndex.value = store.students.indexOf(item)
  editedItem.value = { ...item }
  dialogDelete.value = true
}

async function deleteItemConfirm() {
  if (editedItem.value.id) {
    try {
      await store.deleteStudent(editedItem.value.id)
      showSnackbar('Student erfolgreich gelöscht', 'success')
      closeDelete()
    } catch (e) {
      showSnackbar('Fehler beim Löschen', 'error')
    }
  }
}

function close() {
  dialog.value = false
  editedItem.value = { ...defaultItem } as Student
  editedIndex.value = -1
}

function closeDelete() {
  dialogDelete.value = false
  editedItem.value = { ...defaultItem } as Student
  editedIndex.value = -1
}

async function save() {
  // Simple validation
  if (!editedItem.value.name || !editedItem.value.matrikelnummer) {
      showSnackbar('Bitte Name und Matrikelnummer ausfüllen', 'warning')
      return;
  }

  try {
    if (editedIndex.value > -1) {
      await store.updateStudent(editedItem.value)
      showSnackbar('Student aktualisiert', 'success')
    } else {
      await store.createStudent(editedItem.value)
      showSnackbar('Student erstellt', 'success')
    }
    close()
  } catch (e) {
    showSnackbar('Fehler beim Speichern', 'error')
  }
}

function showSnackbar(text: string, color: string) {
  snackbarText.value = text
  snackbarColor.value = color
  snackbar.value = true
}

// Statistik öffnen
async function openStats() {
    await store.getStatistics()
    statsDialog.value = true
}

function handleRowClick(_: any, row: { item: Student }) {
    editItem(row.item)
}
</script>

<template>
  <v-container>
    <v-card flat>
      <v-card-title class="d-flex align-center pe-2">
        <v-icon icon="mdi-account-group" class="me-2"></v-icon>
        Studenten Liste
        <v-spacer></v-spacer>
      <v-text-field
        v-model="search"
        prepend-inner-icon="mdi-magnify"
        density="compact"
        label="Suche"
        single-line
        flat
        hide-details
        variant="solo-filled"
      ></v-text-field>
    </v-card-title>

    <v-data-table
      :headers="headers"
      :items="store.students"
      :loading="store.loading"
      :search="search" 
      hover
      @click:row="handleRowClick"
    >
      <template v-slot:top>
        <v-toolbar flat>
          <v-spacer></v-spacer>
          
          <!-- Statistik Button -->
          <v-btn
            class="mb-2 mr-2"
            color="info"
            prepend-icon="mdi-chart-bar"
            @click="openStats"
          >
            Statistik
          </v-btn>

          <!-- Download Button -->
          <v-btn
            class="mb-2 mr-2"
            color="secondary"
            prepend-icon="mdi-download"
            @click="store.downloadStudentsCsv()"
          >
            Download CSV
          </v-btn>

          <!-- Verwenden der neuen Statistik Component -->
          <StatisticsDialog v-model="statsDialog" />

          <v-dialog v-model="dialog" max-width="500px">
            <template v-slot:activator="{ props }">
              <v-btn
                class="mb-2"
                color="primary"
                dark
                v-bind="props"
                prepend-icon="mdi-plus"
              >
                Neuer Student
              </v-btn>
            </template>
            <v-card>
              <v-card-title>
                <span class="text-h5">{{ formTitle }}</span>
              </v-card-title>

              <v-card-text>
                <v-container>
                  <v-row>
                    <v-col cols="12">
                      <v-text-field
                        v-model="editedItem.name"
                        label="Name"
                      ></v-text-field>
                    </v-col>
                    <v-col cols="12">
                      <v-text-field
                        v-model="editedItem.university"
                        label="University (Optional)"
                        placeholder="Leave empty for default"
                      ></v-text-field>
                    </v-col>
                    <v-col cols="12" sm="6">
                      <v-text-field
                        v-model="editedItem.matrikelnummer"
                        label="Matrikelnummer"
                      ></v-text-field>
                    </v-col>
                    <v-col cols="12" sm="6">
                      <v-text-field
                        v-model.number="editedItem.semester"
                        label="Semester"
                        type="number"
                      ></v-text-field>
                    </v-col>
                  </v-row>
                </v-container>
              </v-card-text>

              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="blue-darken-1" variant="text" @click="close">
                  Abbrechen
                </v-btn>
                <v-btn color="blue-darken-1" variant="text" @click="save">
                  Speichern
                </v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>
          <v-dialog v-model="dialogDelete" max-width="500px">
            <v-card>
              <v-card-title class="text-h5">Bist du sicher?</v-card-title>
              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="blue-darken-1" variant="text" @click="closeDelete">Abbrechen</v-btn>
                <v-btn color="blue-darken-1" variant="text" @click="deleteItemConfirm">OK</v-btn>
                <v-spacer></v-spacer>
              </v-card-actions>
            </v-card>
          </v-dialog>
        </v-toolbar>
      </template>

      <!-- Custom Actions Column -->
      <template v-slot:item.actions="{ item }">
        <v-icon
          class="me-2"
          size="small"
          @click.stop="editItem(item)"
        >
          mdi-pencil
        </v-icon>
        <v-icon
          size="small"
          @click.stop="deleteItem(item)"
        >
          mdi-delete
        </v-icon>
      </template>

      <template v-slot:no-data>
        <v-btn
          color="primary"
          @click="store.getStudents()"
        >
          Reload Data
        </v-btn>
      </template>
    </v-data-table>

    <!-- Feedback Snackbar -->
    <v-snackbar
        v-model="snackbar"
        :color="snackbarColor"
        :timeout="3000"
    >
        {{ snackbarText }}
        <template v-slot:actions>
        <v-btn
            color="white"
            variant="text"
            @click="snackbar = false"
        >
            Close
        </v-btn>
        </template>
    </v-snackbar>
  </v-card>
  </v-container>
</template>