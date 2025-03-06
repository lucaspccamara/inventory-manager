<template>
  <q-page padding>
    <q-card>
      <q-card-section>
        <q-tabs v-model="tab" dense class="text-primary">
          <q-tab name="geral" label="Geral" />
          <q-tab name="unidades" label="Unidades" />
          <q-tab name="conversoes" label="Conversões" />
        </q-tabs>
      </q-card-section>
      
      <q-card-section>
        <q-tab-panels v-model="tab" animated>
          <q-tab-panel name="geral">
            <p>Configurações gerais do sistema.</p>
          </q-tab-panel>
          
          <q-tab-panel name="unidades">
            <Unidades />
          </q-tab-panel>
          
          <q-tab-panel name="conversoes">
            <Conversoes />
          </q-tab-panel>
        </q-tab-panels>
      </q-card-section>
    </q-card>
  </q-page>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import Unidades from 'components/UnidadesComponent.vue';
import Conversoes from 'components/ConversoesComponent.vue';

const tab = ref('geral');
</script>

<style lang="scss" scoped>
.q-tab-panels {
  .q-page {
    min-height: 100% !important;
  }
}
</style>