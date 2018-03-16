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

        public static async Task<string> FromFile(FileInfo file, CancellationToken token = default(CancellationToken))
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
                string output = insertion.Replace(line, (Match m) =>
                {
                    if (m.Value.Contains(":"))
                    {
                        string[] splits = m.Value.Split(":");
                        if (splits.Length <= 1) throw new IllegalPipeException();
                        string result = splits[0];
                        foreach (string pipeId in splits.Skip(1))
                        {
                            string[] pipeAndArg = pipeId.Remove(pipeId.LastIndexOf('}')).Split(',');
                            if (!Pipes.ContainsKey(pipeAndArg[0])) throw new IllegalPipeException(pipeAndArg[0]);
                            result = Pipes[pipeAndArg[0]](result, pipeAndArg.Skip(1).ToArray());
                        }
                        return result;
                    }
                    else
                    {
                        if (Values.ContainsKey(m.Value))
                        {
                            if (Values[m.Value] is DateTime dt)
                            {
                                if(Values.ContainsKey("dateTimeFormat")){
                                    return dt.ToString((string)Values["dateTiemFormat"]);
                                }
                                return dt.ToShortDateString();
                            }
                            return Values[m.Value].ToString();
                        }
                        return m.Value;
                    }
                });
                fileString += output + Environment.NewLine;
            }
            return fileString;
        }


    }


}
