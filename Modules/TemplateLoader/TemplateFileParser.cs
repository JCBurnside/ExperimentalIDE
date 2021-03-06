﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TemplateLoader.Exceptions;

namespace TemplateLoader
{
    public class TemplateFileParser : TemplateParserBase
    {
        public TemplateFileParser(Dictionary<string, object> values=null, Dictionary<string, Pipe> pipes=null) : base(values, pipes)
        {
        }
        

        protected override Regex Insertion { get; } = new Regex("{\\S{1,}}");

        public async Task<FormatedFile> FromFile(FileInfo file, CancellationToken token = default(CancellationToken))
        => FromArray(await File.ReadAllLinesAsync(file.FullName, token));


        public FormatedFile FromArray(string[] lines, CancellationToken token = default(CancellationToken))
        {
            try
            {
                List<string> outputList = new List<string>();
                foreach (string line in lines)
                {
                    token.ThrowIfCancellationRequested();
                    if (!String.IsNullOrWhiteSpace(line))
                    {
                        string output = fromString(line);
                        if (output == string.Empty) continue;
                        outputList.Add(output);
                    }
                    else
                    {
                        outputList.Add(line);
                    }
                }
                return new FormatedFile
                {
                    Name = (string)Values["fileName"],
                    Contents = String.Join(Environment.NewLine, outputList),
                };
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        private string fromString(string line)
        {
            if (checkPreproccess(line)) return string.Empty;
            return Insertion.Replace(line, (Match m) =>
            {
                string value = m.Value.Substring(1, m.Value.Length - 2);
                if (value.Contains(":"))
                {
                    string[] splits = value.Split(":");
                    if (splits.Length <= 1) throw new IllegalPipeException();
                    string result = Values.ContainsKey(splits[0]) ? (string)Values[splits[0]] : splits[0];
                    System.Diagnostics.Debug.WriteLineIf(Values.ContainsKey(result), Values.Where(kv => kv.Key == result).Select(kv => kv.Value).FirstOrDefault());
                    foreach (string pipeId in splits.Skip(1))
                    {
                        string[] pipeAndArg = pipeId.Split(',');
                        if (!Pipes.ContainsKey(pipeAndArg[0])) throw new IllegalPipeException(pipeAndArg[0]);
                        result = Pipes[pipeAndArg[0]](result, pipeAndArg.Skip(1).ToArray());
                    }
                    System.Diagnostics.Debug.WriteLine(result);
                    return result;
                }
                else
                {
                    if (Values.ContainsKey(value))
                    {
                        if (Values[value] is DateTime dt)
                        {
                            if (Values.ContainsKey("dateTimeFormat"))
                            {
                                return dt.ToString((string)Values["dateTimeFormat"]);
                            }
                            return dt.ToShortDateString();
                        }
                        return Values[value].ToString();
                    }
                    return value;
                }
            });
        }

    }


}
