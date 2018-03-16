using System;
using System.Collections.Generic;
using System.Text;

namespace TemplateLoader.Tests
{
    internal static class Extenstions
    {
        internal static void Add<T1, T2>(this Dictionary<T1, T2> self, Dictionary<T1, T2> other)
        {
            foreach ((T1 key, T2 value) in other)
            {
                self.Add(key,value);
            }
        }
    }
}
