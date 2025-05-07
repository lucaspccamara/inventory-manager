<template>
  <q-card padding style="width: 900px; max-width: 80vw;">
    <q-card-section>
      <q-toolbar>
        <q-toolbar-title>{{ props.idPedido == null ? "Cadastrar" : "Editar" }} Pedido</q-toolbar-title>
        <q-space />
        <q-btn dense flat icon="close" v-close-popup>
          <q-tooltip>Fechar</q-tooltip>
        </q-btn>
      </q-toolbar>

      <q-form @submit.prevent="salvarPedido" class="row q-col-gutter-md">
        <div class="col-6">
          <BuscaClienteFornecedor
            v-model="pedido.clienteFornecedor"
            :busca="props.tipoMovimentacao == 'entrada' ? 'fornecedores' : 'clientes'"
            ocultar-tipo-busca
            hint
            :rules="[ (val: ClienteFornecedorSelectDto | null) => val !== null || 'Campo obrigatório' ]"
          />
        </div>

        <div class="col-6">
          <div class="row full-width justify-end">
            <q-input
              v-model="data"
              label="Data do pedido:"
              type="date"
              outlined
              dense
              class="col-4"
              :rules="[
                val => !!val || 'Data obrigatória',
                val => !val || !isNaN(Date.parse(val)) || 'Data inválida'
              ]"
            />
          </div>
        </div>

        <div class="col-12">
          <q-input
            v-model="pedido.observacao"
            label="Observação"
            type="textarea"
            autogrow
            bottom-slots
            counter
            maxlength="500"
          />
        </div>

        <div class="col-12">
          <q-table
            :rows="pedido.itens"
            :columns="colunas"
            row-key="id"
            class="q-mt-md"
            dense
            flat
            bordered
            hide-bottom
          >
            <template v-slot:top>
              <q-btn
                color="primary"
                :disable="loading"
                label="Adicionar Item"
                @click="dialogSelecionarProduto = true"
              />
              <q-space />
              <q-input
                v-model="filter"
                placeholder="Buscar..."
                rounded
                outlined
                dense
                debounce="300"
                color="primary"
                icon="search"
              >
                <template v-slot:append>
                  <q-icon name="search" />
                </template>
              </q-input>
            </template>

            <template v-slot:body-cell-nome="props">
              <q-td :props="props">
                <div style="display: flex;">
                  <div class="ellipsis" style="flex: 1 1 0%; max-width: 200px;" :title="props.row.nome">
                    {{ props.row.nome }}
                  </div>
                </div>
              </q-td>
            </template>

            <template v-slot:body-cell-quantidade="props">
              <q-td :props="props">
                <q-input
                  class="input-size-content justify-self-end"
                  v-model="props.row.quantidade"
                  type="number"
                  min="0"
                  step="1"
                  filled
                  dense
                />
              </q-td>
            </template>
  
            <template v-slot:body-cell-unidadeVendaSelecionada="props">
              <q-td :props="props">
                <q-select
                  class="unidade-select"
                  v-model="props.row.unidadeVendaSelecionada"
                  :options="props.row.unidadesVenda.map((unidade: UnidadeConversao) => ({
                    id: unidade.id,
                    nome: unidade.origem.nome
                  }))"
                  option-value="id"
                  option-label="nome"
                  dense
                  rounded
                  outlined
                  @update:model-value="unidade => onUnidadeSelecionada(props.row, unidade)"
                />
              </q-td>
            </template>

            <template v-slot:body-cell-precoUnitario="props">
              <q-td :props="props">
                <CurrencyInput
                  class="input-size-content justify-self-end"
                  v-model="props.row.precoUnitario"
                  filled
                />
              </q-td>
            </template>

            <template v-slot:body-cell-valorTotal="props">
              <q-td :props="props">
                R$ {{ formatarPreco(props.row.valorTotal) }}
              </q-td>
            </template>

            <template v-slot:body-cell-acoes="props">
              <q-td :props="props">
                <q-btn
                  icon="delete"
                  size="sm"
                  flat
                  round
                  color="negative"
                  @click="removerItem(props.row)"
                />
              </q-td>
            </template>

            <template v-slot:bottom-row v-if="pedido.itens.length > 0">
              <q-tr>
                <q-td /> <!-- Produto -->
                <q-td /> <!-- Quantidade -->
                <q-td /> <!-- Unidade de Venda -->
                <q-td class="text-right text-bold">
                  Total do Pedido:
                </q-td>
                <q-td class="text-bold">
                  R$ {{ formatarPreco(totalPedido) }}
                </q-td>
              </q-tr>
            </template>
          </q-table>
        </div>

        <div class="col flex justify-end">
          <q-btn
            type="submit"
            label="Salvar Pedido"
            color="primary"
          />
        </div>
      </q-form>
    </q-card-section>
  </q-card>

  <q-dialog v-model="dialogSelecionarProduto" persistent class="lg-dialog">
    <q-card class="q-dialog-plugin" style="width: 900px; max-width: 80vw;">
      <q-card-section>
        <q-toolbar>
          <q-toolbar-title>Selecione o Produto</q-toolbar-title>
          <q-space />
          <q-btn dense flat icon="close" v-close-popup>
            <q-tooltip>Fechar</q-tooltip>
          </q-btn>
        </q-toolbar>
      </q-card-section>

      <q-card-section>
        <ProdutosPage
          selecionavel
          @selecionar="onProdutoSelecionado"
        />
      </q-card-section>
    </q-card>
  </q-dialog>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue';
import { api } from 'src/boot/axios';
import { Notify } from 'quasar';
import {
  Pedido,
  PedidoCreateDto,
  PedidoUpdateDto,
  Produto,
  ProdutoPedidoDto,
  UnidadeConversao,
  UnidadeMedida,
  StatusOpcoesEntrada,
  StatusOpcoesSaida,
  ClienteFornecedorSelectDto
} from './models';

import BuscaClienteFornecedor from './BuscaClienteFornecedorComponent.vue';
import ProdutosPage from '../pages/ProdutosPage.vue';
import CurrencyInput from './CurrencyInput.vue';

const props = defineProps<{
  idPedido: number | null,
  tipoMovimentacao: 'entrada' | 'saida' | 'orcamento'
}>();

const emit = defineEmits(['atualizarLista', 'fecharDialog']);

// Setup de datas
const data = ref('');

const setDefaultDate = (dateValue: Date | null) => {
  const today = dateValue ? new Date(dateValue) : new Date();
  data.value = new Date(today.getFullYear(), today.getMonth(), today.getDate()).toISOString().split('T')[0] || '';

  pedido.value.data = new Date(data.value);
};

const formatarPreco = (valor: number) => {
  return valor.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
};

const totalPedido = computed(() =>
  pedido.value.itens.reduce(
    (acc, item) => acc + (item.precoUnitario * item.quantidade),
    0
  )
);

const pedido = ref<Pedido>({
  id: null,
  clienteFornecedor: null,
  observacao: '',
  itens: [],
  data: new Date(data.value),
  total: 0,
  status: 1
});

// Colunas da tabela de itens
const colunas = [
  { name: 'nome', label: 'Produto', field: 'nome', align: 'left' as const, style: 'width: 200px' },
  { name: 'quantidade', label: 'Quantidade', field: 'quantidade', align: 'center' as const, style: 'width: 100px' },
  { name: 'unidadeVendaSelecionada', label: 'Unidade de Venda', field: 'unidadeVendaSelecionada', align: 'center' as const, style: 'width: 140px' },
  { name: 'precoUnitario', label: 'Valor Unitário', field: 'precoUnitario', align: 'center' as const, style: 'width: 150px' },
  { name: 'valorTotal', label: 'Valor Total', field: 'valorTotal', align: 'left' as const, style: 'width: 100px' },
  { name: 'acoes', label: 'Ações', field: 'acoes', align: 'center' as const, style: 'width: 70px' }
];

const filter = ref('');
const loading = ref(false);

const dialogSelecionarProduto = ref(false);

const onProdutoSelecionado = (produto: Produto) => {
  dialogSelecionarProduto.value = false;

  const unidadeVendaSelecionada = produto.unidadesVenda.length > 0 && produto.unidadesVenda[0] ? produto.unidadesVenda[0] : null;
  
  pedido.value.itens.push({
    id: null,
    pedidoId: null,
    produtoId: produto.id ?? 0,
    nome: produto.nome,
    quantidade: 1,
    unidadesVenda: produto.unidadesVenda,
    unidadeVendaSelecionada: {
      id: unidadeVendaSelecionada?.id ?? 0,
      nome: unidadeVendaSelecionada?.origem.nome ?? '',
      sigla: unidadeVendaSelecionada?.origem.sigla ?? '',
      status: unidadeVendaSelecionada?.origem.status ?? true,
    },
    precoUnitario: props.tipoMovimentacao === 'entrada' ? 0 : unidadeVendaSelecionada?.precoPadrao ?? 0,
    valorTotal: props.tipoMovimentacao === 'entrada' ? 0 : unidadeVendaSelecionada?.precoPadrao ?? 0,
  });
}

const onUnidadeSelecionada = (item: ProdutoPedidoDto, unidadeMedida: UnidadeMedida) => {
  // Busca a unidade de conversão correspondente
  const unidade = (item.unidadesVenda || []).find(
    (u: any) => u.id === unidadeMedida.id
  );
  if (unidade && props.tipoMovimentacao !== 'entrada') {
    item.precoUnitario = unidade.precoPadrao ?? 0;
    item.valorTotal = Number(item.quantidade) * Number(item.precoUnitario);
  }
}

const removerItem = (item: any) => {
  pedido.value.itens = pedido.value.itens.filter(i => i.id !== item.id);
}

const itensOriginais = ref<any[]>([]);
// Carregar pedido para edição
const carregarPedido = async(id: number) => {
  loading.value = true;
  try {
    const { data: pedidoData } = await api.get<Pedido>(`/pedidos/${id}`);

    pedido.value = pedidoData;
    setDefaultDate(pedidoData.data);

    itensOriginais.value = JSON.parse(JSON.stringify(pedido.value.itens));
  } catch {
    Notify.create({ message: 'Erro ao carregar pedido.', color: 'negative' });
  } finally {
    loading.value = false;
  }
}

const getItensParaSalvar = () => {
  // Itens adicionados: não têm id
  const adicionados = pedido.value.itens.filter(item => !item.id);

  // Itens modificados: têm id e foram alterados em relação ao original
  const modificados = pedido.value.itens.filter(item => {
    if (!item.id) return false;
    const original = itensOriginais.value.find(orig => orig.id === item.id);
    return original && JSON.stringify(item) !== JSON.stringify(original);
  });

  // Itens removidos: estavam na lista original mas não estão mais na atual
  const removidos = itensOriginais.value
    .filter(orig => !pedido.value.itens.some(item => item.id === orig.id))
    .map(orig => orig.id);

  return { adicionados, modificados, removidos };
}

// Salvar pedido
const salvarPedido = async() => {
  if (!pedido.value.clienteFornecedor?.id || !pedido.value.data || pedido.value.itens.length === 0) {
    Notify.create({ message: 'Preencha todos os campos obrigatórios e adicione ao menos um item.', color: 'warning' });
    return;
  }
  loading.value = true;
  // Preencher Dto de pedido com os dados do formulário
  const pedidoCreateDto: PedidoCreateDto = {
    id: pedido.value.id,
    clienteFornecedorId: pedido.value.clienteFornecedor.id,
    observacao: pedido.value.observacao,
    data: pedido.value.data,
    status: pedido.value.status,
    itens: pedido.value.itens.map(item => ({
      id: item.id,
      pedidoId: item.pedidoId,
      produtoId: item.produtoId,
      produtoUnidadeVendaId: item.unidadeVendaSelecionada?.id ?? 0,
      fatorConversao: item.unidadesVenda?.find(un => un.id == item.unidadeVendaSelecionada?.id)?.fator ?? 1,
      quantidade: item.quantidade,
      precoUnitario: item.precoUnitario
    }))
  };

  if (props.idPedido) {
    // Atualizar pedido existente
    const { adicionados, modificados, removidos } = getItensParaSalvar();
    const pedidoUpdateDto: PedidoUpdateDto = {
      ...pedidoCreateDto,
      itensAdicionados: adicionados.map(item => ({
        id: item.id ?? null,
        pedidoId: item.pedidoId ?? null,
        produtoId: item.produtoId,
        produtoUnidadeVendaId: item.unidadeVendaSelecionada?.id ?? 0,
        fatorConversao: item.unidadesVenda?.find(un => un.id == item.unidadeVendaSelecionada?.id)?.fator ?? 1,
        quantidade: item.quantidade,
        precoUnitario: item.precoUnitario
      })),
      itensModificados: modificados.map(item => ({
        id: item.id,
        pedidoId: item.pedidoId,
        produtoId: item.produtoId,
        produtoUnidadeVendaId: item.unidadeVendaSelecionada?.id ?? 0,
        fatorConversao: item.unidadesVenda?.find(un => un.id == item.unidadeVendaSelecionada?.id)?.fator ?? 1,
        quantidade: item.quantidade,
        precoUnitario: item.precoUnitario
      })),
      itensRemovidos: removidos
    };

    try {
      await api.put(`/pedidos/${props.idPedido}`, pedidoUpdateDto).then((response) => {
        if (response.status === 204) {
          Notify.create({ message: 'Pedido atualizado com sucesso!', color: 'positive' });
          emit('atualizarLista');
          emit('fecharDialog');
        }
      });
    } catch (error) {
      Notify.create({ message: 'Erro ao atualizar pedido', color: 'negative' });
    } finally {
      loading.value = false;
    }
  } else {
    // Salvar novo pedido
    try {
      await api.post('/pedidos', pedidoCreateDto).then((response) => {
        if (response.status === 201) {
          Notify.create({message: 'Pedido salvo com sucesso', color: 'positive' });
          emit('atualizarLista');
          emit('fecharDialog');
        }
      });
    } catch (error) {
      Notify.create({ 
        message: 'Erro ao salvar pedido',
        color: 'negative'
      });
    } finally {
      loading.value = false;
    }
  }
}

watch(data, (newValue) => {
  pedido.value.data = new Date(newValue);
});

watch(
  () => pedido.value.itens,
  (novos) => {
    novos.forEach(item => {
      item.valorTotal = Number(item.quantidade) * Number(item.precoUnitario);
    });
  },
  { deep: true }
);

onMounted(() => {
  if (props.idPedido) {
    carregarPedido(props.idPedido);
  } else {
    setDefaultDate(null);

    if(props.tipoMovimentacao === 'entrada') {
      pedido.value.status = StatusOpcoesEntrada[0].value;
    } else if (props.tipoMovimentacao === 'saida') {
      pedido.value.status = StatusOpcoesSaida[1].value;
    } else {
      pedido.value.status = StatusOpcoesSaida[0].value;
    }
  }
});
</script>

<style lang="scss" scoped>
</style>
