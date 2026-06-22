using System.Collections.Generic;
using System.Linq;
using SmartMedPharmacy.Models;

namespace SmartMedPharmacy.Data
{
    public static class MedicineSearch
    {
        public static List<Medicine> LinearSearchByName(List<Medicine> source, string name)
        {
            List<Medicine> result = new List<Medicine>();
            if (string.IsNullOrWhiteSpace(name))
                return new List<Medicine>(source);

            string term = name.Trim().ToLowerInvariant();
            foreach (Medicine m in source)
            {
                if (m.Name != null && m.Name.ToLowerInvariant().Contains(term))
                    result.Add(m);
            }
            return result;
        }

        public static List<Medicine> FilterByCategory(List<Medicine> source, string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return new List<Medicine>(source);

            string term = category.Trim().ToLowerInvariant();
            return source.Where(m => m.Category != null && m.Category.ToLowerInvariant().Contains(term)).ToList();
        }

        public static List<Medicine> FilterByPriceRange(List<Medicine> source, decimal min, decimal max)
        {
            return source.Where(m => m.Price >= min && m.Price <= max).ToList();
        }

        public static Medicine BinarySearchByName(List<Medicine> source, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            List<Medicine> sorted = source.OrderBy(m => m.Name, System.StringComparer.OrdinalIgnoreCase).ToList();
            string target = name.Trim();

            int low = 0;
            int high = sorted.Count - 1;
            while (low <= high)
            {
                int mid = (low + high) / 2;
                int cmp = string.Compare(sorted[mid].Name, target, System.StringComparison.OrdinalIgnoreCase);
                if (cmp == 0)
                    return sorted[mid];
                if (cmp < 0)
                    low = mid + 1;
                else
                    high = mid - 1;
            }
            return null;
        }
    }
}
