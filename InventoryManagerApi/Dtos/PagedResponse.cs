﻿namespace InventoryManagerApi.Dtos
{
    public class PagedResponse<T>
    {
        public List<T> Data { get; set; } = new();
        public int TotalRecords { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
