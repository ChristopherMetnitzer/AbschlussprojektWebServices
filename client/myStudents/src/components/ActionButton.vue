<script setup>
  const items = [
    { title: 'Click Me' },
    { title: 'Click Me' },
    { title: 'Click Me' },
    { title: 'Click Me 2' },
  ]
</script>

<template>
  <div class="d-flex justify-space-around">
    <v-menu transition="scale-transition">
      <template v-slot:activator="{ props }">
        <v-btn color="green" v-bind="props">
            <slot>Do something with me!</slot>
        </v-btn>
      </template>

      <v-list>
        <v-list-item v-for="(item, i) in items" :key="i" :value="i">
          <v-list-item-title>{{ item.title }}</v-list-item-title>
        </v-list-item>
      </v-list>
    </v-menu>
  </div>
</template>
