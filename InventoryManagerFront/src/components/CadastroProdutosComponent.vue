<template>
  <q-card padding style="width: 900px; max-width: 80vw;">
    <q-card-section>
      <q-toolbar>
        <q-toolbar-title>Cadastro de Produtos</q-toolbar-title>
        <q-space />
        <q-btn dense flat icon="close" v-close-popup>
          <q-tooltip>Fechar</q-tooltip>
        </q-btn>
      </q-toolbar>

      <q-form @submit.prevent="salvarProduto" class="row q-col-gutter-md">
        <div class="col-12">
          <q-input v-model="produto.nome" label="Nome do Produto" required />
        </div>

        <div class="col-12">
          <q-input v-model="produto.descricao" label="Descrição" type="textarea" autogrow />
        </div>

        <div class="col-6">
          <q-select
            v-model="produto.unidadeCompra"
            :options="opcoesUnidades"
            option-value="id"
            option-label="nome"
            label="Unidade de Compra"
            required
            @scroll="handleScroll"
            @update:model-value="verificarAlteracaoUnidadeCompra"
          />
        </div>

        <div class="col-6">
          <q-select
            v-model="produto.menorUnidade"
            :options="opcoesUnidades"
            option-value="id"
            option-label="nome"
            label="Menor Unidade de Venda"
            required
            @scroll="handleScroll"
            @update:model-value="verificarAlteracaoMenorUnidade"
          />
        </div>

        <div class="col-12">
          <q-list bordered separator v-if="produto.unidadeCompra && produto.menorUnidade">
            <q-item-label header class="row justify-between items-center">
              Unidades de Venda
              <q-btn
                label="Adicionar Unidade"
                @click="adicionarUnidade(null)"
                color="primary"
                :disable="!produto.nome.trim() || !produto.unidadeCompra || !produto.menorUnidade"
              />
            </q-item-label>
            <q-item v-for="(unidade, index) in produto.unidadesVenda" :key="index">
              <div class="row full-width items-center">
                <span>Preço Padrão:&nbsp;</span>
  
                <div v-if="unidade.editing">
                  <CurrencyInput
                    class="input-size-content"
                    v-model="unidade.precoPadrao"
                  />
                </div>
                <strong v-else>R$ {{ formatarPreco(unidade.precoPadrao) }}</strong>
  
                <span>&nbsp;| 1&nbsp;</span>
  
                <div v-if="unidade.editing && unidade.origem.id !== produto.menorUnidade.id">
                  <q-select
                    class="input-size-content"
                    v-model="unidade.origem"
                    :options="opcoesUnidades"
                    option-value="id"
                    option-label="nome"
                    dense
                    rounded
                    outlined
                  />
                </div>
                <strong v-else>{{ unidade.origem.nome }} ({{ unidade.origem.sigla }})</strong>
  
                <div class="row items-center" v-if="unidade.origem.id !== produto.menorUnidade.id">
                  <span>&nbsp;contém&nbsp;</span>
    
                  <div v-if="unidade.editing">
                    <q-input
                      class="input-size-content"
                      v-model.number="unidade.fator"
                      type="number"
                      dense
                      rounded
                      outlined
                    />
                  </div>
                  <span v-else>{{ unidade.fator }}</span>
                  
                  <strong>&nbsp;{{ produto.menorUnidade.nome }} ({{ produto.menorUnidade.sigla }})</strong>
                </div>
  
                <div class="col" align="right">
                  <q-btn
                    :icon="unidade.editing ? 'save' : 'edit'"
                    :color="unidade.editing ? 'green' : 'primary'"
                    dense
                    flat
                    @click="salvarEditarUnidade(unidade)"
                  />
                  <q-btn
                    v-if="unidade.editing"
                    icon="close"
                    color="red"
                    dense
                    flat
                    @click="removerUnidade(index)"
                  />
                  <q-btn
                    v-if="!unidade.editing"
                    icon="delete"
                    color="red"
                    dense
                    flat
                    @click="removerUnidade(index)"
                    :disable="(produto.unidadesVenda[index]?.origem?.id === produto.unidadeCompra?.id && produto.unidadesVenda[index]?.destino?.id === produto.menorUnidade?.id) || (produto.unidadesVenda[index]?.origem?.id === produto.menorUnidade?.id)"
                  />
                </div>
              </div>
            </q-item>
          </q-list>
        </div>

        <div class="col-3">
          <q-toggle v-model="produto.status" :label="produto.status ? 'Status: Ativo' : 'Status: Inativo'" :disable="props.idProduto == null" />
        </div>

        <div class="col flex justify-end">
          <q-btn
            type="submit"
            label="Salvar Produto"
            color="primary"
            :disable="!produto.nome.trim() || !produto.unidadeCompra || !produto.menorUnidade"
          />
        </div>
      </q-form>
    </q-card-section>
  </q-card>

  <ConfirmDialogComponent
    v-if="confirmDialogVisible"
    :mensagem="'Tem certeza que deseja alterar a ' + (unidadeAlterada === 'unidadeCompra' ? 'Unidade de Compra' : 'Menor Unidade de Venda') + '?'"
    @confirmar="confirmarAlteracaoUnidade"
    @cancelar="() => (confirmDialogVisible = false)"
  />
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue';
import { api } from 'src/boot/axios';
import { Notify } from 'quasar';
import type { Produto, ProdutoCreateDto, UnidadeMedida, UnidadeConversao, ApiRequest, ApiResponse } from './models';
import CurrencyInput from './CurrencyInput.vue';

const props = defineProps<{ idProduto: number | null }>();
const emit = defineEmits(['atualizarLista', 'fecharDialog']);

const requestUnidades = ref<ApiRequest<{ nome?: string; sigla?: string; status?: boolean, searchType: string }>>({
  filter: { nome: '', sigla: '', status: true, searchType: 'contains' },
  page: 1,
  pageSize: 10,
  sortBy: 'id',
  sortDesc: false
});

const responseUnidades = ref<ApiResponse<UnidadeMedida>>({
  data: [],
  totalRecords: 0,
  page: 1,
  pageSize: 10,
  totalPages: 1
});

const paginationUnidades = ref({
  page: 1,
  rowsPerPage: 10,
  rowsNumber: 0
});

const opcoesUnidades = ref<UnidadeMedida[]>([]);
const confirmDialogVisible = ref(false);
const unidadeAlterada = ref<null | 'unidadeCompra' | 'menorUnidade'>(null);
const editingUnidadeConversao = ref<UnidadeConversao>({ 
  id: null,
  origem: {
    id: 0,
    nome: 'Selecione',
    sigla: '',
    status: true
  },
  destino: {
    id: 0,
    nome: 'Selecione',
    sigla: '',
    status: true
  },
  fator: 1 ,
  precoPadrao: 0
});

const carregarUnidades = async () => {
  try {
    const { data } = await api.post<ApiResponse<UnidadeMedida>>('/unidades/lista', requestUnidades.value);

    responseUnidades.value = data;
    paginationUnidades.value.rowsNumber = data.totalRecords;

    // Adicionar as novas unidades à lista de opções
    data.data.forEach((unidade) => {
      if (!opcoesUnidades.value.some(u => u.id === unidade.id)) {
        opcoesUnidades.value.push(unidade);
      }
    });

    // Incrementar a página para a próxima consulta
    if (paginationUnidades.value.page < data.totalPages) {
      paginationUnidades.value.page++;
    } else {
      // Desabilitar a consulta se não houver mais páginas
      paginationUnidades.value.page = -1;
    }
  } catch (error) {
    Notify.create({
      message: 'Erro ao carregar unidades!',
      color: 'negative'
    });
  }
};

const handleScroll = (event: Event) => {
  const target = event.target as HTMLElement;
  if (target.scrollHeight - target.scrollTop === target.clientHeight) {
    if (paginationUnidades.value.page !== -1) {
      carregarUnidades();
    }
  }
};

const produto = ref<Produto>({
  id: null,
  nome: '',
  descricao: '',
  status: true,
  unidadeCompra: null,
  unidadesVenda: [],
  menorUnidade: null
});

const salvarEditarUnidade = (unidade: UnidadeConversao) => {
  unidade.editing = !unidade.editing;

  // Ordenar as unidades de venda do maior fator para o menor
  produto.value.unidadesVenda.sort((a, b) => b.fator - a.fator);
};

const removerUnidade = (index: number) => {
  produto.value.unidadesVenda.splice(index, 1);
};

const adicionarUnidade = (index: number | null) => {
  const novaUnidade = {
    id: editingUnidadeConversao.value.id,
    origem: { ...editingUnidadeConversao.value.origem },
    destino: {
      id: produto.value.menorUnidade?.id ?? null,
      nome: produto.value.menorUnidade?.nome ?? '',
      sigla: produto.value.menorUnidade?.sigla ?? '',
      status: true
    },
    fator: editingUnidadeConversao.value.fator,
    precoPadrao: editingUnidadeConversao.value.precoPadrao,
    editing: true
  };

  adicionarOuAtualizarUnidade(novaUnidade);

  // Resetar o estado de edição
  editingUnidadeConversao.value = {
    id: null,
    origem: { id: 0, nome: 'Selecione', sigla: '', status: true },
    destino: { id: 0, nome: 'Selecione', sigla: '', status: true },
    fator: 1,
    precoPadrao: 0,
    editing: true
  };
};

const atualizarUnidadesVenda = () => {
  if (produto.value.unidadeCompra && produto.value.menorUnidade) {
    // Remover unidades antigas que não correspondem mais à unidade de compra ou menor unidade
    produto.value.unidadesVenda = produto.value.unidadesVenda.filter(unidade => {
      return (
        (unidade.origem.id === produto.value.unidadeCompra?.id && unidade.destino.id === produto.value.menorUnidade?.id) ||
        (unidade.origem.id === produto.value.menorUnidade?.id && unidade.destino.id === produto.value.menorUnidade?.id)
      );
    });

    // Unidade de Venda 1: Unidade de Compra -> Menor Unidade
    const unidadeVenda1 = {
      id: null,
      origem: {
        id: produto.value.unidadeCompra.id,
        nome: produto.value.unidadeCompra.nome,
        sigla: produto.value.unidadeCompra.sigla,
        status: true
      },
      destino: {
        id: produto.value.menorUnidade.id,
        nome: produto.value.menorUnidade.nome,
        sigla: produto.value.menorUnidade.sigla,
        status: true
      },
      fator: 1,
      precoPadrao: 0,
      editing: true
    };

    // Unidade de Venda 2: Menor Unidade -> Menor Unidade
    const unidadeVenda2 = {
      id: null,
      origem: {
        id: produto.value.menorUnidade.id,
        nome: produto.value.menorUnidade.nome,
        sigla: produto.value.menorUnidade.sigla,
        status: true
      },
      destino: {
        id: produto.value.menorUnidade.id,
        nome: produto.value.menorUnidade.nome,
        sigla: produto.value.menorUnidade.sigla,
        status: true
      },
      fator: 1,
      precoPadrao: 0,
      editing: true
    };

    // Adicionar ou substituir as unidades de venda
    adicionarOuAtualizarUnidade(unidadeVenda1);
    adicionarOuAtualizarUnidade(unidadeVenda2);
  }
};

const adicionarOuAtualizarUnidade = (novaUnidade: UnidadeConversao) => {
  // Verificar se já existe uma unidade de venda igual
  const unidadeExistenteIndex = produto.value.unidadesVenda.findIndex(
    u => u.origem.id === novaUnidade.origem.id && u.destino.id === novaUnidade.destino.id
  );

  if (unidadeExistenteIndex !== -1) {
    // Atualizar a unidade existente
    produto.value.unidadesVenda.splice(unidadeExistenteIndex, 1, {
      ...novaUnidade,
      editing: true // Certifique-se de que a unidade não está em edição ao atualizar
    });
  } else {
    // Adicionar nova unidade
    produto.value.unidadesVenda.push({
      ...novaUnidade,
      editing: true // Inicializar como não editando
    });
  }

  // Ordenar as unidades de venda do maior fator para o menor
  produto.value.unidadesVenda.sort((a, b) => b.fator - a.fator);
};

const verificarAlteracaoUnidadeCompra = (novaUnidadeCompra: UnidadeMedida) => {
  if (produto.value.id !== null) {
    // Exibir diálogo de confirmação durante a edição
    unidadeAlterada.value = 'unidadeCompra';
    confirmDialogVisible.value = true;
    editingUnidadeConversao.value.origem = novaUnidadeCompra;
  } else {
    // Alterar diretamente durante o cadastro
    produto.value.unidadeCompra = novaUnidadeCompra;
    atualizarUnidadesVenda();
  }
};

const verificarAlteracaoMenorUnidade = (novaMenorUnidade: UnidadeMedida) => {
  if (produto.value.id !== null) {
    // Exibir diálogo de confirmação durante a edição
    unidadeAlterada.value = 'menorUnidade';
    confirmDialogVisible.value = true;
    editingUnidadeConversao.value.destino = novaMenorUnidade;
  } else {
    // Alterar diretamente durante o cadastro
    produto.value.menorUnidade = novaMenorUnidade;
    atualizarUnidadesVenda();
  }
};

const confirmarAlteracaoUnidade = () => {
  if (unidadeAlterada.value === 'unidadeCompra') {
    produto.value.unidadeCompra = editingUnidadeConversao.value.origem;
  } else if (unidadeAlterada.value === 'menorUnidade') {
    produto.value.menorUnidade = editingUnidadeConversao.value.destino;
  }
  atualizarUnidadesVenda();
  confirmDialogVisible.value = false;
};

const salvarProduto = () => {
  cadastrarProduto();
};

const cadastrarProduto = async () => {
  const protudoDto = {
    nome: produto.value.nome,
    descricao: produto.value.descricao,
    status: produto.value.status,
    unidadeCompraId: produto.value.unidadeCompra?.id,
    menorUnidadeId: produto.value.menorUnidade?.id,
    unidadesVenda: produto.value.unidadesVenda.map(u => ({
      unidadeMedidaId: u.origem.id,
      menorUnidadeId: u.destino.id,
      fator: u.fator,
      precoPadrao: u.precoPadrao
    }))
  } as ProdutoCreateDto;

  try {
    await api.post('/produtos', protudoDto).then((response) => {
      if (response.status === 201)
        Notify.create({message: 'Cadastro salvo com sucesso', color: 'positive' });
    });

    //emit('atualizarLista');
    //fecharDialog();
  } catch (error) {
    Notify.create({ 
      message: 'Erro ao salvar cadastro',
      color: 'negative'
    });
  }
};

const formatarPreco = (valor: number) => {
  return valor.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
};

watch(() => produto.value.unidadeCompra, atualizarUnidadesVenda);
watch(() => produto.value.menorUnidade, atualizarUnidadesVenda);

carregarUnidades();

onMounted(() => {
  if (props.idProduto) {
    //carregarProduto(props.idProduto);
  }
});
</script>

<style lang="scss" scoped>
.input-size-content{
  width: fit-content;
} 
</style>
