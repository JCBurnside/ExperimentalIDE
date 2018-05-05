using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

namespace TemplateLoader
{

    public abstract class TemplateParserBase
    {
        public delegate string Pipe(string input, IEnumerable<string> format = null);

        protected abstract Regex Insertion { get; }

        protected static Dictionary<string, object> _Values = new Dictionary<string, object>();

        public static Dictionary<string, FileInfo> Templates { get; } = new Dictionary<string, FileInfo>();

        public static Dictionary<string, object> Values
        {
            get
            {
                if (!_Values.ContainsKey("location"))
                {
                    _Values["location"] = "~/";
                }
                if (!_Values.ContainsKey("workingDir"))
                {
                    _Values["workingDir"] = Assembly.GetCallingAssembly().Location;
                }
                return _Values;
            }
            set => _Values = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static Dictionary<string, Pipe> Pipes { get; private set; } = new Dictionary<string, Pipe>{
            { "pathToNamespace", DefaultPipes.PathToNamespace },
            { "dateFormat", DefaultPipes.DateToString }
        };

        public TemplateParserBase(Dictionary<string, object> values, Dictionary<string, Pipe> pipes)
        {
            if (values != null)
            {
                Values.AddOverride(values);
            }
            if (pipes != null)
            {
                Pipes.AddOverride(pipes);
            }
        }

        public static string ParseDirectoryPath(string path)
        {
            if (path.StartsWith("~/"))
                return path.Replace("~/", (string)Values["workingDir"]);
            return path;
        }

        protected static bool checkPreproccess(string line)
        {
            if (line.StartsWith(';'))
            {
                string[] args = line.Substring(1).Split(' ');
                if (args.Length == 0) return true;
                switch (args[0])
                {
                    case "fileType":
                        if (args.Length != 2) break;
                        Values["extension"] = args[1];
                        break;
                    case "outputPath":
                    case "outputLocation":
                        if (args.Length != 2) Values["location"] = "~/";
                        else Values["location"] = args[1];
                        break;
                }
                return true;
            }
            return false;
        }
    }
}
