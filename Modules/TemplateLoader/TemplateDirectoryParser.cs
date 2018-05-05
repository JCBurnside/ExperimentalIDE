using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Xml;
using System.Reflection;
using System.Linq;
using System.Xml.Serialization;

namespace TemplateLoader
{
    public class TemplateDirectoryParser : TemplateParserBase
    {
        protected override Regex Insertion { get; } = new Regex("^:\\S+:$");

        public TemplateDirectoryParser(Dictionary<string, object> values = null, Dictionary<string, Pipe> pipes = null) : base(values, pipes) { }


        public async Task<FormatedDirectory> FromFile(FileInfo info, CancellationToken token = default)
        {
            using (XmlTextReader nodeReader = new XmlTextReader(info.OpenRead()))
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Async = true;
                settings.ValidationType = ValidationType.Schema;
                settings.Schemas.Add(null, new FileInfo("DirectoryTemplateSchema.xsd").FullName);
                settings.ValidationEventHandler += validationHandler;
                settings.IgnoreWhitespace = true;
                using (XmlReader reader = XmlReader.Create(nodeReader, settings))
                {
                    return await xmlReadRecurisive(reader, token);
                }
            }
        }

        private async Task<FormatedDirectory> xmlReadRecurisive(XmlReader initial, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            FormatedDirectory output = new FormatedDirectory();
            string outputName = initial.GetAttribute("name");
            if (Insertion.IsMatch(outputName??String.Empty))
            {
                outputName = outputName.Substring(1, outputName.Length - 2);
                if (Values.ContainsKey(outputName))
                {
                    outputName = Values[outputName].ToString();
                }
            }
            output.Name = outputName;
            List<FormatedDirectory> subFolders = new List<FormatedDirectory>();
            List<FormatedFile> subFiles = new List<FormatedFile>();
            while (initial.Read())
            {
                if (initial.Name == "Directory")
                {
                    using (XmlReader inner = initial.ReadSubtree())
                    {

                        subFolders.Add(await xmlReadRecurisive(inner, token));
                    }
                }
                if (initial.Name == "File")
                {
                    string name = initial.GetAttribute("name");
                    if (Insertion.IsMatch(name))
                    {
                        name = name.Substring(1, outputName.Length - 2);
                        if (Values.ContainsKey(name))
                        {
                            name = Values[name].ToString();
                        }
                    }
                    string template = initial.GetAttribute("template");
                    if (!String.IsNullOrWhiteSpace(template) &&
                        Templates.ContainsKey(template))
                    {
                        subFiles.Add(await new TemplateFileParser(new Dictionary<string, object> { { "fileName", name } }).FromFile(Templates[template]));
                    }
                    else
                    {
                        subFiles.Add(new FormatedFile(name));
                    }
                }
            }
            return output;
        }

        private void validationHandler(object sender, System.Xml.Schema.ValidationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Validation Error: {0}", e.Message);
        }

        private async Task<FormatedDirectory> ReplaceNamesAsync(FormatedDirectory root, CancellationToken token = default)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"sub dir count:{root.Folders.Count()}");
#endif
            if (Insertion.IsMatch(root.Name))
            {
                string[] splits = root.Name.Substring(1).Split(':');
                if (Values.ContainsKey(splits[0]))
                {
                    string output = splits[0];
                    foreach (string pipeName in splits.Skip(1))
                    {
                        token.ThrowIfCancellationRequested();
                        string[] pipeSplit = pipeName.Split(',');

                        if (Pipes.ContainsKey(pipeSplit[0]))
                        {
                            if (pipeSplit.Length == 1)
                            {
                                output = Pipes[pipeSplit[0]](output);
                            }
                            else
                            {
                                output = Pipes[pipeSplit[0]](output, pipeSplit.Skip(1));
                            }
                        }
                    }
                }
                else
                {
                    root.Name = splits[0];
                }
            }
            for (int dirPos = 0; dirPos < root.Folders.Length; dirPos++)
            {
                token.ThrowIfCancellationRequested();
                root.Folders[dirPos] = await ReplaceNamesAsync(root.Folders[dirPos], token);
            }
            List<FormatedFile> files = await root.GetFiles(token);
            for (int filePos = 0; filePos < files.Count; filePos++)
            {
                if (Insertion.IsMatch(files[filePos].Name))
                {
                    string name = files[filePos].Name;
                    if (name.Count(c => c == ',') == 0)
                    {
                        if (Values.ContainsKey(name))
                        {
                            files[filePos].Name = Values[name].ToString();
                        }
                    }
                    else
                    {
                        string[] splits = name.Substring(1, name.Length - 2).Split(',');
                        string output = files[filePos].Name;
                        string pipeName = splits[0];
                        if (Pipes.ContainsKey(pipeName))
                        {
                            output = Pipes[pipeName](output, splits.Skip(1));
                        }
                        files[filePos].Name = output;
                    }
                }
            }
            return root;
        }

    }
}
