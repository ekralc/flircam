<template>
  <div id="app">
    <crosshair :visible="hudVisible" />
  </div>
</template>

<script>
import Crosshair from './Crosshair.vue';

export default {
  name: 'App',
  components: {
    Crosshair,
  },
  data() {
    return {
      hudVisible: false,
    };
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
      this.hudVisible = toggle;
    },
  },
};
</script>
