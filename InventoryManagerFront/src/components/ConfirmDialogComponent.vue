<template>
  <q-dialog v-model="isOpen" persistent>
    <q-card>
      <q-card-section class="row items-center">
        <q-icon :name="icon" :color="iconColor" size="2em" />
        <span class="q-ml-sm">{{ message }}</span>
      </q-card-section>

      <q-card-actions align="right">
        <q-btn flat label="Cancelar" color="primary" @click="cancelar" />
        <q-btn flat label="Confirmar" :color="confirmColor" @click="confirmar" />
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>

<script setup lang="ts">
import { ref, defineProps, defineEmits, watch } from 'vue';

const props = defineProps({
  modelValue: Boolean,
  message: {
    type: String,
    default: 'Tem certeza que deseja continuar?'
  },
  icon: {
    type: String,
    default: 'warning'
  },
  iconColor: {
    type: String,
    default: 'warning'
  },
  confirmColor: {
    type: String,
    default: 'negative'
  }
});

const emit = defineEmits(['update:modelValue', 'confirm']);

const isOpen = ref(props.modelValue);

watch(() => props.modelValue, (newVal) => {
  isOpen.value = newVal;
});

const cancelar = () => {
  isOpen.value = false;
  emit('update:modelValue', false);
  emit('confirm', false);
};

const confirmar = () => {
  isOpen.value = false;
  emit('update:modelValue', false);
  emit('confirm', true);
};
</script>