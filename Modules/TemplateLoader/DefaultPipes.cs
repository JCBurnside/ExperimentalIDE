﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TemplateLoader.Exceptions;

namespace TemplateLoader
{
    internal static class DefaultPipes
    {

        internal static string PathToNamespace(string path, string[] args)
        {
            if (path.Any(Path.GetInvalidPathChars().Contains)) throw new IllegalPipeArgumentException(nameof(PathToNamespace), nameof(path));
            if (path.StartsWith('~') &&
                TemplateParserBase.Values.ContainsKey("ProjectName"))
            {
                path = (string)TemplateParserBase.Values["ProjectName"]+path.Remove(0, 1);
            }
            if (path.EndsWith('/')) path = path.Remove(path.Length - 1);
            return path.Replace(' ', '_').Replace('/', '.');
        }

        internal static string DateToString(string input, string[] args)
        {
            if (args.Length == 0) args = new[] { "YYYY-MM-DD HHmm" };
            if (!DateTime.TryParse(input, out var time)) throw new IllegalPipeArgumentException(nameof(DateToString), nameof(input));
            return time.ToString(args[0]);
        }

    }
}
