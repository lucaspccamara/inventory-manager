namespace InventoryManagerApi.Utils
{
    public class SearchUtils
    {
        public static bool ApplySearchType(string field, string value, string? searchType)
        {
            switch (searchType?.ToLower())
            {
                case "exact":
                    return field.Equals(value, StringComparison.OrdinalIgnoreCase);
                case "startswith":
                    return field.StartsWith(value, StringComparison.OrdinalIgnoreCase);
                case "endswith":
                    return field.EndsWith(value, StringComparison.OrdinalIgnoreCase);
                case "contains":
                default:
                    return field.Contains(value, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
