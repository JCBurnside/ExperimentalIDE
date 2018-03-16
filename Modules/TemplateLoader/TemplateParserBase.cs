using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace TemplateLoader
{

    public class TemplateParserBase
    {
        public delegate string Pipe(string input, string[] format=null);
        private static Dictionary<string, object> _Values = new Dictionary<string, object>();
        public static Dictionary<string, object> Values
        {
            get {
                if (!_Values.ContainsKey("outputPath"))
                {
                    _Values["outputPath"] = "~/";
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

        public static string ParseDirectoryPath(string path)
        {
            if(path.StartsWith("~/"))
                return path.Replace("~/", (string)Values["workingDir"]);
            if (path.StartsWith('/'))
                return path.Replace("/", (string)Values["workingDir"]);
            return path;
        }
    }
}
