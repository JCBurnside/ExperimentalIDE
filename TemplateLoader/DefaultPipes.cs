using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TemplateLoader.Exceptions;

namespace TemplateLoader
{
    internal static class DefaultPipes
    {

        internal static string PathToNamespace(string path, string format=""){
            if (path.Any(Path.GetInvalidPathChars().Contains)) throw new IllegalPipeArgumentException("PathToNamespace", nameof(path));
            return path.Replace(' ', '_').Replace('/', '.');
        }

        internal static string DateToString(string input, string format = "YYYY-MM-DD HHmm"){
            if (!DateTime.TryParse(input, out var time)) throw new IllegalPipeArgumentException(nameof(DateToString), nameof(input));
            return time.ToString(format);
        }

    }
}
