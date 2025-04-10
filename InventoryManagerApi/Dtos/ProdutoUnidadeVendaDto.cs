﻿using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagerApi.Dtos
{
    public class ProdutoUnidadeVendaDto
    {
        public int? Id { get; set; }
        public int ProdutoId { get; set; }
        public UnidadeMedidaDto? Origem { get; set; }
        public UnidadeMedidaDto? Destino { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Fator { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoPadrao { get; set; }
        public bool Status { get; set; } = true;
        public bool Editing { get; set; } = false;
    }
}
