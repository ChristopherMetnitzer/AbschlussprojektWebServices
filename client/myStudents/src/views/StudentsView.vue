<script setup lang="ts">
    import { useStudentStore } from "@/stores/studentStore";
    const store = useStudentStore();
    import Students from '@/components/Students.vue';

</script>

<template>

    <v-container>
        <Students></Students>
    </v-container>
    <button @click="store.testRoundRobin(9)">Test Round-Robin</button>


    
</template>