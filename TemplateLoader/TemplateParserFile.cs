using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TemplateLoader.Exceptions;

namespace TemplateLoader
{
    public class TemplateParserFile : TemplateParserBase
    {
        private static Regex insertion = new Regex("{\\S{1,}}");

        public static async Task FromFile(FileInfo file, CancellationToken token = default(CancellationToken))
        {
            string fileString = "";
            if (!Values.ContainsKey("extension")) Values["extension"] = "txt";
            if (!Values.ContainsKey("outputPath")) Values["outputPath"] = "~/";
            foreach (string line in await File.ReadAllLinesAsync(file.FullName, token))
            {
                if (line.StartsWith(';'))
                {
                    string[] args = line.Substring(1).Split(' ');
                    if (args.Length == 0) continue;
                    switch (args[0])
                    {
                        case "fileType":
                            if (args.Length != 2) continue;
                            Values["extension"] = args[1];
                            continue;
                        case "outputPath":
                        case "outputLocation":
                            if (args.Length != 2) Values["location"] = "~/";
                            else Values["location"] = args[1];
                            continue;
                        default:
                            continue;
                    }
                }
                insertion.Replace(line, (Match m) =>
                {
                    if (m.Value.Contains(":"))
                    {
                        string[] splits = m.Value.Split(":");
                        if (splits.Length <= 1) throw new IllegalPipeException();
                        string output = splits[0];
                        foreach (string pipeId in splits.Skip(1))
                        {
                            string pipeIdFiltered = pipeId.Remove(pipeId.LastIndexOf('}'));
                            if (!Pipes.ContainsKey(pipeIdFiltered)) throw new IllegalPipeException(pipeIdFiltered);
                            output = Pipes[pipeIdFiltered](output);
                        }
                        return output;
                    }
                    else
                    {
                        if (Values.ContainsKey(m.Value))
                        {
                            return Values[m.Value].ToString();
                        }
                        return m.Value;
                    }
                });
                fileString += insertion + Environment.NewLine;
            }

        }


    }


}
