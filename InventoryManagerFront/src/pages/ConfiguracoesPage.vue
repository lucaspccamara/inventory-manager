<template>
  <q-page padding>
    <q-card>
      <q-card-section>
        <q-tabs v-model="tab" dense align="left" class="text-primary">
          <q-tab name="geral" label="Geral" />
          <q-tab name="unidades" label="Unidades" />
        </q-tabs>
      </q-card-section>
      
      <q-card-section>
        <q-tab-panels v-model="tab" animated>
          <q-tab-panel name="geral">
            <q-form @submit.prevent="salvarConfiguracoes" class="row q-col-gutter-md">
              <div class="col-12 text-h6">
                <p>Configurações gerais do sistema</p>
              </div>
              <div class="col-6">
                <q-input
                  v-model="configuracoes.nomeEmpresa"
                  label="Nome da empresa"
                  bottom-slots
                  counter
                  maxlength="255"
                />
              </div>
              <div class="col-6">
                <q-input
                  v-model="configuracoes.email"
                  label="E-mail de contato"
                  bottom-slots
                  counter
                  maxlength="255"
                />
              </div>

              <div class="col">
                <q-input
                  v-model="configuracoes.cnpj"
                  label="CNPJ"
                  mask="##.###.###/####-##"
                />
              </div>
              
              <div class="col">
                <q-input
                  v-model="configuracoes.telefone"
                  label="Telefone"
                  mask="(##) ####-####"
                />
              </div>
              <div class="col">
                <q-input
                  v-model="configuracoes.celular"
                  label="Celular"
                  mask="(##) #####-####"
                />
              </div>
              <div class="col-12">
                <div class="flex">
                  <div
                    class="logo-avatar-wrapper"
                    @mouseenter="showEdit = true"
                    @mouseleave="showEdit = false"
                  >
                    <div class="text-caption text-grey q-mt-xs q-mb-sm">Logo da Empresa</div>
                    <div class="logo-avatar-custom avatar-bordered">
                      <template v-if="configuracoes.logoUrl">
                        <img
                          :src="configuracoes.logoUrl"
                          alt="Logo da empresa"
                          :class="{ 'avatar-darken': showEdit }"
                          class="logo-img"
                        />
                      </template>
                      <template v-else>
                        <div class="logo-placeholder" :class="{ 'avatar-darken': showEdit }">
                          <q-icon name="business" size="100px" color="grey-5" />
                        </div>
                      </template>
                      <q-btn
                        v-if="showEdit"
                        class="edit-btn"
                        icon="edit"
                        color="primary"
                        round
                        size="md"
                        @click="triggerLogoInput"
                      />
                      <q-btn
                        v-if="configuracoes.logoUrl && showEdit"
                        class="edit-btn"
                        icon="delete"
                        color="negative"
                        round
                        size="md"
                        @click="triggerLogoInput"
                      />
                    </div>
                    <input
                      ref="logoInput"
                      type="file"
                      accept="image/*"
                      style="display: none"
                      @change="onLogoFileChange"
                    />
                  </div>
                  <q-space />
                  <div>
                    <q-btn label="Salvar" type="submit" color="primary" />
                  </div>
                </div>
              </div>
              <div class="col-12 flex justify-center">
                <span class="text-grey">Versão do sistema: {{ versao }}</span>
              </div>
            </q-form>
          </q-tab-panel>
          
          <q-tab-panel name="unidades">
            <Unidades />
          </q-tab-panel>
        </q-tab-panels>
      </q-card-section>
    </q-card>
  </q-page>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import Unidades from '../components/UnidadesComponent.vue';

const tab = ref('geral');
const showEdit = ref(false);
const logoInput = ref<HTMLInputElement | null>(null);

const configuracoes = ref({
  nomeEmpresa: '',
  cnpj:'',
  email: '',
  telefone: '',
  celular: '',
  logoUrl: ''
});

const versao = import.meta.env.VITE_PACKAGE_VERSION;

function triggerLogoInput() {
  logoInput.value?.click();
}

function onLogoFileChange(event: Event) {
  const files = (event.target as HTMLInputElement).files;
  if (files && files.length > 0) {
    const reader = new FileReader();
    reader.onload = e => {
      configuracoes.value.logoUrl = e.target?.result as string;
      // Aqui você pode salvar a imagem na API ou localStorage, se desejar
    };
    reader.readAsDataURL(files[0] as Blob);
  }
}

function salvarConfiguracoes() {
  // Salvar configurações na API
  // Exibir um Notify de sucesso
}
</script>

<style lang="scss" scoped>
.q-tab-panels {
  .q-page {
    min-height: 100% !important;
  }
}

.logo-avatar-wrapper {
  position: relative;
  display: inline-block;
}
.logo-avatar-custom {
  height: 200px;
  max-height: 200px;
  border-radius: 16px;
  overflow: hidden;
  position: relative;
  background: #fff;
  display: flex;
  align-items: center;
  justify-content: center;
}
.logo-img {
  width: 100%;
  height: 100%;
  object-fit: scale-down;
  display: block;
  background: #fff;
  transition: filter 0.3s;
}
.logo-placeholder {
  width: 200px;
  height: 200px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f5f5f5;
  transition: filter 0.3s;
}
.avatar-bordered {
  outline: 3px solid #020b14;
  box-shadow: 0 4px 16px rgba(0,0,0,0.18);
  transition: box-shadow 0.2s, border 0.2s;
}
.avatar-darken {
  filter: brightness(0.5);
  transition: filter 0.3s;
}
.edit-btn {
  position: absolute;
  z-index: 2;
  background: white;
}
</style>