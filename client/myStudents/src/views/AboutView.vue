<script setup lang="ts">
  import ActionButton from '@/components/ActionButton.vue';
</script>

<template>
  <div class="about">
    <ActionButton></ActionButton>
  </div>
</template>

<style>
@media (min-width: 1024px) {
  .about {
    min-height: 100vh;
    display: flex;
    align-items: center;
  }
}
</style>
