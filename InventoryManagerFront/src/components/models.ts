//INTERFACES
export interface ApiRequest<T = any> {
  filter: T;
  page: number;
  pageSize: number;
  sortBy: string;
  sortDesc: boolean;
}

export interface ApiResponse<T = any> {
  data: T[];
  totalRecords: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface Produto {
  id: number | null;
  nome: string;
  descricao: string;
  status: boolean;
  unidadeCompra: UnidadeMedida | null;
  unidadesVenda: UnidadeConversao[];
  menorUnidade: UnidadeMedida | null;
}

export interface ProdutoCreateDto {
  id: number | null;
  nome: string;
  descricao: string;
  status: boolean;
  unidadeCompraId: number;
  unidadesVenda: UnidadeConversaoCreateDto[];
  menorUnidadeId: number;
}

export interface UnidadeConversao {
  id: number | null;
  origem: UnidadeMedida;
  destino: UnidadeMedida;
  fator: number;
  precoPadrao: number;
  editing?: boolean;
}

export interface UnidadeConversaoCreateDto {
  id: number | null;
  unidadeMedidaId: number;
  menorUnidadeId: number;
  fator: number;
  precoPadrao: number;
}

export interface UnidadeMedida {
  id: number | null;
  nome: string;
  sigla: string;
  status: boolean;
}

//ENUMS
export const StatusOpcoesBoolean = [
  { label: 'Inativo', value: false },
  { label: 'Ativo', value: true },
  { label: 'Todos', value: null }
];

export const SearchOptions = [
  { label: 'Exato', value: 'exact' },
  { label: 'Contém', value: 'contains' },
  { label: 'Começa com', value: 'startsWith' },
  { label: 'Termina com', value: 'endsWith' }
];
