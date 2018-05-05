using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace TemplateLoader
{
    internal static class Extenstions
    {
        internal static void Add<T1, T2>(this Dictionary<T1, T2> self, Dictionary<T1, T2> other)
        {
            foreach ((T1 key, T2 value) in other)
            {
                self.Add(key, value);
            }
        }

        internal static void AddOverride<T1, T2>(this Dictionary<T1, T2> self, Dictionary<T1, T2> other)
        {
            foreach ((T1 key, T2 value) in other)
            {
                self[key] = value;
            }
        }

        internal static async Task<T> ReadContentAsAsync<T>(this XmlReader self, IXmlNamespaceResolver resolver, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            object returned = await self.ReadContentAsAsync(typeof(T), resolver);
            if (returned is T t)
            {
                return t;
            }
            return default;
        }
    }
}
