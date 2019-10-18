using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MCAuthWeb.Data
{
    public static class NavigationManagerExtensions
    {
        public static bool TryGetGetQueryPrameter(this NavigationManager manager, string key, out string value)
        {
            value = null;

            if (manager.Uri != null)
            {
                var uri = new Uri(manager.Uri, UriKind.Absolute);

                if (!string.IsNullOrWhiteSpace(uri.Query))
                {
                    value = QueryHelpers.ParseNullableQuery(uri.Query).TryGetValue(key, out var values) ? values.First() : null;
                }
            }

            return value != null;
        }
    }
}
