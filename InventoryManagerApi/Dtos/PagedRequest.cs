namespace InventoryManagerApi.Dtos
{
    public class PagedRequest<T> where T : class
    {
        public T Filter { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public bool SortDesc { get; set; } = false;

        public bool IsAll => PageSize == 0;
    }
}
