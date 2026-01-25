<script setup lang="ts">
import { useStudentStore } from '@/stores/studentStore'

const store = useStudentStore()

// Props definieren (v-model Bindung)
defineProps<{
  modelValue: boolean
}>()

// Events definieren
const emit = defineEmits(['update:modelValue'])

// Schließen Funktion
function close() {
  emit('update:modelValue', false)
}
</script>

<template>
  <!-- Dialog für Statistiken -->
  <v-dialog :model-value="modelValue" @update:model-value="close" max-width="400px">
    <v-card>
      <v-card-title class="text-h5">Statistik Übersicht</v-card-title>
      
      <v-card-text v-if="store.stats">
        <v-list lines="two">
          <v-list-item title="Gesamtanzahl" :subtitle="String(store.stats.totalCount ?? store.stats.TotalCount)"></v-list-item>
          <v-list-item title="Thema" :subtitle="store.stats.topic ?? store.stats.Topic"></v-list-item>
          <v-list-item title="Status" :subtitle="store.stats.status ?? store.stats.Status"></v-list-item>
          <v-list-item title="Serverzeit" :subtitle="store.stats.serverTime ?? store.stats.ServerTime"></v-list-item>
        </v-list>
      </v-card-text>
      
      <v-card-text v-else>
        Lade Daten...
      </v-card-text>

      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="primary" variant="text" @click="close">Schließen</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
