<template>
  <q-card padding style="width: 700px; max-width: 80vw;">
    <q-card-section>
      <q-toolbar>
        <q-toolbar-title>{{ props.idUnidade == null ? "Cadastrar" : "Editar" }} Unidade</q-toolbar-title>
        <q-space />
        <q-btn dense flat icon="close" v-close-popup>
          <q-tooltip>Fechar</q-tooltip>
        </q-btn>
      </q-toolbar>

      <q-form @submit="salvarUnidade" class="row q-col-gutter-md">
        <div class="col-12">
          <q-input
            v-model="unidade.nome"
            label="Nome"
            required
            :rules="[
              val => val.length >= 2 || 'O número mínimo de caracteres é 2',
              val => val.length <= 25 || 'O número máximo de caracteres é 25'
            ]"
          />
        </div>
        <div class="col-12">
          <q-input
            v-model="unidade.sigla"
            label="Sigla"
            required
            :rules="[
              val => val.length >= 1 || 'O número mínimo de caracteres é 1',
              val => val.length <= 10 || 'O número máximo de caracteres é 10'
            ]"
          />
        </div>
        
        <div class="col-12 flex justify-between">
          <q-toggle
            v-model="unidade.status"
            :label="unidade.status ? 'Status: Ativo' : 'Status: Inativo'"
            :disable="props.idUnidade == null"
          />

          <q-btn type="submit" label="Salvar" color="primary" />
        </div>
      </q-form>
    </q-card-section>
  </q-card>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { api } from '../boot/axios';
import { Notify } from 'quasar';
import { UnidadeMedida } from './models';

const props = defineProps<{ idUnidade: number | null }>();
const emit = defineEmits(['atualizarLista', 'fecharDialog']);

const unidade = ref<UnidadeMedida>({
  id: null,
  nome: '',
  sigla: '',
  status: true
});

const carregarUnidade = async (id: number) => {
  try {
    const { data } = await api.get<UnidadeMedida>(`/unidades/${id}`);
    unidade.value = data;
  } catch (error) {
    Notify.create({
      message: 'Erro ao carregar unidade!',
      color: 'negative'
    });
  }
};

const salvarUnidade = async () => {
  try {
    if (unidade.value.id) {
      await api.put(`/unidades/${unidade.value.id}`, unidade.value).then((response) => {
        if (response.status === 204)
          Notify.create({message: 'Cadastro salvo com sucesso', color: 'positive' });
      });
    } else {
      await api.post('/unidades', unidade.value).then((response) => {
        if (response.status === 201)
          Notify.create({message: 'Cadastro salvo com sucesso', color: 'positive' });
      });
    }
    emit('atualizarLista');
    emit('fecharDialog');
  } catch (error) {
    Notify.create({ 
      message: 'Erro ao salvar cadastro',
      color: 'negative'
    });
  }
};

onMounted(() => {
  if (props.idUnidade) {
    carregarUnidade(props.idUnidade);
  }
});
</script>
