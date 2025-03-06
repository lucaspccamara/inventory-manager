export interface UnidadesConversoes {
  id: number;
  origem: Unidades;
  destino: Unidades;
  fator: number;
}

export interface Unidades {
  id: number;
  nome: string;
  sigla: string;
}

export interface Produtos {
  id: number | null;
  nome: string;
  unidadeCompra: Unidades | null;
  unidadesVenda: UnidadesConversoes[];
  menorUnidade: Unidades | null;
}
