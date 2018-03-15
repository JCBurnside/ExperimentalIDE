using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace TemplateLoader
{
    public class TemplateParserDirectory : TemplateParserBase
    {
        public static Task<string> FromFile(FileInfo info, CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult("");
        }
    }
}
