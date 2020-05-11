<template>
  <div id="app">
    <Crosshair />
  </div>
</template>

<script>
import Crosshair from './Crosshair.vue';

export default {
  name: 'App',
  components: {
    Crosshair,
  },
  mounted() {
    this.listener = (event) => {
      const item = event.data;
      if (this[item.type]) {
        this[item.type](item); // execute the method, sending event.data with it
      }
    };

    window.addEventListener('message', this.listener);
  },
  methods: {
    ON_HUD_TOGGLE({ toggle }) {
      console.log(`Received: ${toggle}`);
    }
  }
};
</script>
