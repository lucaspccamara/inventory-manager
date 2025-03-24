<template>
  <q-input
    class="currency-input"
    ref="currencyInputRef"
    v-model="formattedValue"
    dense
    rounded
    outlined
    :label="label"
    :disable="disable"
    :error="error"
    :error-message="errorMessage"
    @blur="onBlur"
  />
</template>

<script setup lang="ts">
import { watchEffect } from 'vue';
import { useCurrencyInput } from 'vue-currency-input';

// Definição das props
const props = defineProps({
  modelValue: {
    type: Number,
    required: true
  },
  label: {
    type: String,
    default: ''
  },
  currency: {
    type: String,
    default: 'BRL'
  },
  locale: {
    type: String,
    default: 'pt-BR'
  },
  precision: {
    type: Number,
    default: 2
  },
  disable: {
    type: Boolean,
    default: false
  },
  error: {
    type: Boolean,
    default: false
  },
  errorMessage: {
    type: String,
    default: ''
  }
});

// Emissão de eventos
const emit = defineEmits(['update:modelValue', 'blur']);

// Configuração do `useCurrencyInput`
const { inputRef: currencyInputRef, numberValue, formattedValue } = useCurrencyInput({
  currency: props.currency,
  locale: props.locale,
  precision: props.precision,
  autoDecimalDigits: true, // Permite digitação fluida sem precisar de ponto/vírgula manualmente
  hideGroupingSeparatorOnFocus: false, // Oculta os separadores enquanto o usuário digita
  hideCurrencySymbolOnFocus: false, // Oculta o símbolo da moeda sempre visível
  valueRange: { min: 0 } // Evitar valores negativos
});

// Sincroniza o valor inicial com o formato correto
watchEffect(() => {
  if (numberValue.value !== props.modelValue) {
    numberValue.value = props.modelValue;
  }
});

// Emite a atualização do modelValue enquanto o usuário digita
watchEffect(() => {
  emit('update:modelValue', numberValue.value || 0);
});

// Emite o evento de blur para o componente pai
const onBlur = () => {
  emit('blur', numberValue.value || 0);
};
</script>

<style lang="scss" scoped>
.currency-input {
  padding: 0;
}	
</style>
