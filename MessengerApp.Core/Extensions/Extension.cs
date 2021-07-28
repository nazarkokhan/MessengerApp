using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessengerApp.Core.DTO.Authorization;

namespace MessengerApp.Core.Extensions
{
    public static class Extension
    {
        public static bool IsExpired(this DateTime expTime)
        {
            return DateTime.UtcNow >= expTime;
        }
        
        public static async Task ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> func)
        {
            foreach (var e in enumerable) await func(e);
        }

        public static IQueryable<T> TakePage<T>(this IQueryable<T> queryable, int page, int items)
        {
            return queryable
                .Skip((page - 1) * items)
                .Take(items);
        }
    }
}