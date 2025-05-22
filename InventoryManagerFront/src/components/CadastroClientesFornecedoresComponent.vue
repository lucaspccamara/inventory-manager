<template>
  <q-card padding style="width: 700px; max-width: 80vw;">
    <q-card-section>
      <q-toolbar>
        <q-toolbar-title>{{ props.idClienteFornecedor == null ? "Cadastrar" : "Editar" }} Cliente/Fornecedor</q-toolbar-title>
        <q-space />
        <q-btn dense flat icon="close" v-close-popup>
          <q-tooltip>Fechar</q-tooltip>
        </q-btn>
      </q-toolbar>

      <q-form @submit="salvarCadastro" class="row q-col-gutter-md">
        <div class="col-12">
          <q-input
            v-model="cadastro.nome"
            label="Nome *"
            bottom-slots
            counter
            maxlength="255"
            :rules="[ val => val && val.length > 0 || 'Campo obrigatório' ]"
          />
        </div>

        <div class="col-12 q-pt-none">
          <div class="row q-col-gutter-md">
            <div class="col">
              <q-input
                v-model="cadastro.cpfCnpj"
                label="CPF/CNPJ"
                :mask="cpfCnpjMask"
                :rules="[val => validarCpfCnpj(val)]"
                lazy-rules
                ref="cpfCnpjInput"
                @update:model-value="validarDuranteInput"
              />
            </div>
            <div class="col">
              <q-input
                v-model="cadastro.telefone"
                label="Telefone"
                mask="(##) ####-####"
              />
            </div>
            <div class="col">
              <q-input
                v-model="cadastro.celular"
                label="Celular"
                mask="(##) #####-####"
              />
            </div>
          </div>
        </div>
        
        <div class="col-12 q-pt-none">
          <q-input
            v-model="cadastro.email"
            label="E-mail"
            type="email"
            bottom-slots
            counter
            maxlength="255"
          />
        </div>

        <div class="col-12 q-pt-none">
          <q-input
            v-model="cadastro.endereco"
            label="Endereço"
            bottom-slots
            counter
            maxlength="500"
          />
        </div>

        <div class="col-12 flex justify-between">
          <q-checkbox
            v-model="tipoCliente"
            label="Cliente"
            :disable="(props.tipoMovimentacao === 'saida' || props.tipoMovimentacao === 'orcamento') && !!props.tipoMovimentacao"
          />
          <q-checkbox
            v-model="tipoFornecedor"
            label="Fornecedor"
            :disable="props.tipoMovimentacao === 'entrada' && !!props.tipoMovimentacao"
          />
          <q-toggle
            v-model="cadastro.status"
            :label="cadastro.status ? 'Status: Ativo' : 'Status: Inativo'"
            :disable="props.idClienteFornecedor == null"
          />
          <q-btn type="submit" label="Salvar" color="primary" />
        </div>
      </q-form>
    </q-card-section>
  </q-card>
</template>

<script setup lang="ts">
import { ref, computed, nextTick, onMounted } from 'vue';
import { ClienteFornecedor, TipoClienteFornecedorType, ClienteFornecedorSelectDto } from './models';
import { api } from '../boot/axios';
import { Notify, QInput } from 'quasar';
import { AxiosError } from 'axios';

const props = defineProps<{
  idClienteFornecedor: number | null,
  tipoMovimentacao?: 'entrada' | 'saida' | 'orcamento'
 }>();
const emit = defineEmits(['atualizarLista', 'fecharDialog', 'retornaSelectDto']);

const cpfCnpjInput = ref<QInput | null>(null);
const cpfCnpjMask = computed(() => {
  const valor = cadastro.value.cpfCnpj ?? '';
  return valor.length <= 14 ? '###.###.###-###' : '##.###.###/####-##'; // Mascara de CPF com um caractere a mais para permitir a mudança de máscara
});

const validarCpfCnpj = (valor: string) => {
  if (!valor) return true;
  const tamanho = valor.length;
  if (tamanho === 14 || tamanho === 18) {
    return true; // CPF (11 dígitos) ou CNPJ (14 dígitos) é válido
  }
  return 'CPF deve ter 11 dígitos ou CNPJ deve ter 14 dígitos';
};

const validarDuranteInput = async () => {
  await nextTick(); // Aguarda o Vue atualizar o valor do campo
  if (cpfCnpjInput.value && cpfCnpjInput.value.hasError) {
    cpfCnpjInput.value.validate(); // Revalida o campo somente se houver erro
  }
};

const tipoCliente = ref(false);
const tipoFornecedor = ref(false);
const tipo = computed({
  get: () => {
    if (tipoCliente.value && tipoFornecedor.value) {
      return 3; // Ambos
    } else if (tipoCliente.value) {
      return 1; // Cliente
    } else if (tipoFornecedor.value) {
      return 2; // Fornecedor
    } else {
      return null; // Nenhum
    }
  },
  set: (value: TipoClienteFornecedorType) => {
    tipoCliente.value = value === 1 || value === 3; // Marca Cliente se for 1 ou 3
    tipoFornecedor.value = value === 2 || value === 3; // Marca Fornecedor se for 2 ou 3
  }
});

const cadastro = ref<ClienteFornecedor>({
  id: null,
  nome: '',
  cpfCnpj: '',
  telefone: '',
  celular: '',
  email: '',
  endereco: '',
  tipo: null,
  status: true
});

const salvarCadastro = async () => {
  if (!tipoCliente.value && !tipoFornecedor.value) {
    Notify.create({
      message: "É necessário marcar ao menos 'Cliente' ou 'Fornecedor'.",
      color: 'warning',
      textColor: 'black'
    });
    return;
  }

  cadastro.value.tipo = tipo.value; // Atualiza o tipo com base nos checkboxes
  cadastro.value.nome = cadastro.value.nome.trim(); // Remove espaços em branco do início e do fim

  try {
    if (cadastro.value.id) {
      await api.put(`/clientes/${cadastro.value.id}`, cadastro.value).then((response) => {
        if (response.status === 204)
          Notify.create({ message: 'Cadastro atualizado com sucesso!', color: 'positive' });
      });
    } else {
      await api.post('/clientes', cadastro.value).then((response) => {
        if (response.status === 201)
        {
          Notify.create({ message: 'Cadastro criado com sucesso!', color: 'positive' });

          const novoCliente: ClienteFornecedorSelectDto = {
            id: response.data.id,
            nome: cadastro.value.nome,
          };
          emit('retornaSelectDto', novoCliente);
        }
      });
    }
    emit('atualizarLista');
    emit('fecharDialog');
  } catch (error) {
    Notify.create({
      message: `${(error as AxiosError)?.response?.data}`,
      color: 'negative'
    });
  }
};

const carregarCadastro = async (id: number) => {
  try {
    const { data } = await api.get<ClienteFornecedor>(`/clientes/${id}`);
    cadastro.value = data;
    tipo.value = data.tipo as TipoClienteFornecedorType; // Atualiza os checkboxes com base no tipo
  } catch (error) {
    Notify.create({
      message: 'Erro ao carregar cliente/fornecedor!',
      color: 'negative'
    });
  }
};

onMounted(() => {
  if (props.tipoMovimentacao === 'entrada') {
    tipoFornecedor.value = true;
    tipoCliente.value = false;
  } else if (props.tipoMovimentacao === 'saida' || props.tipoMovimentacao === 'orcamento') {
    tipoCliente.value = true;
    tipoFornecedor.value = false;
  }
  
  if (props.idClienteFornecedor) {
    carregarCadastro(props.idClienteFornecedor);
  }
});
</script>
