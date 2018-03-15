using System;
using System.Collections.Generic;
using System.Text;

namespace TemplateLoader
{

    public delegate string Pipe(string input, string format = null);
    public class TemplateParserBase
    {
        private static Dictionary<string, object> _Values = new Dictionary<string, object>();
        public static Dictionary<string, object> Values
        {
            get => _Values;
            set
            {
                _Values = value ?? throw new ArgumentNullException(nameof(Values));
                if (!_Values.ContainsKey("outputPath"))
                {
                    _Values["outputPath"] = "~/";
                }
            }
        }
        public static Dictionary<string, Pipe> Pipes { get; private set; } = new Dictionary<string, Pipe>{
            { "pathToNamespace", DefaultPipes.PathToNamespace },
            { "dateFormat", DefaultPipes.DateToString }
        };
    }
}
