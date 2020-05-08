<template>
  <div id="crosshair">
    <img
      v-if="visible"
      src="../img/crosshair.svg"
    >
  </div>
</template>

<script>
export default {
  name: 'Crosshair',
  data() {
    return {
      visible: true,
    };
  },
};
</script>

<style scoped>
#crosshair {
  position: relative;
}

img {
    position: fixed;
    top: 50%;
    left: 50%;
    height: 200px;
    width: auto;
    transform: translate(-50%, -50%);
}
</style>
