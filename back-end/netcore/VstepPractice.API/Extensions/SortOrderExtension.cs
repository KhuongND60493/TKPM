using VstepPractice.API.Common.Enums;

namespace VstepPractice.API.Extensions;

public static class SortOrderExtension
{
    public static SortOrder ConvertStringToSortOrder(string? sortOrder)
        => !string.IsNullOrEmpty(sortOrder)
            ? sortOrder.Trim().ToLower().Equals("asc")
            ? SortOrder.Ascending : SortOrder.Descending : SortOrder.Descending;

    // Format: Column1-ASC,Column2-DESC...
    public static IDictionary<string, SortOrder> ConvertStringToSortOrderV2(string? sortOrder, Func<string, string> getSortProductPropertyFunc)
    {
        var result = new Dictionary<string, SortOrder>();

        if (!string.IsNullOrEmpty(sortOrder))
        {
            foreach (var item in sortOrder.Split(","))
            {
                if (!item.Contains('-'))
                    throw new FormatException("Sort condition should be follow by format: Column1-ASC,Column2-DESC...");
                var property = item.Trim().Split("-");
                var key = getSortProductPropertyFunc(property[0]);
                var value = ConvertStringToSortOrder(property[1]);
                result.TryAdd(key, value);
            }
        }

        return result;
    }
}