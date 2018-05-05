using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static TemplateLoader.TemplateParserBase;

namespace TemplateLoader
{
    public class FormatedDirectory : IEquatable<FormatedDirectory>
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlIgnore]
        private List<FormatedFile> formatedFiles;

        public async Task<List<FormatedFile>> GetFiles(CancellationToken token = default)
        {
            if (formatedFiles != null && !filesChanged) return formatedFiles;
            TemplateFileParser fileParser = new TemplateFileParser();
            List<FormatedFile> output = new List<FormatedFile>();
            if (files != null)
            {
                foreach (File format in files)
                {
                    token.ThrowIfCancellationRequested();
                    Values["fileName"] = format.Name;
                    if (Templates.ContainsKey(format.Template))
                    {
                        output.Add(await fileParser.FromFile(Templates[format.Template], token));
                    }
                    else
                    {
                        FileInfo info = new FileInfo(format.Template);
                        if (info.Exists)
                        {
                            output.Add(await fileParser.FromFile(info, token));
                        }
                        else
                        {
                            throw new IllegalTemplateException("Invalid format specified");
                        }
                    }
                }
            }
            formatedFiles = output;
            filesChanged = false;
            return formatedFiles;
        }

        public void SetFiles(IEnumerable<FormatedFile> files)
        {
            formatedFiles = files.ToList();
        }

        public void SetFiles(IEnumerable<File> files)
        {
            files = files.ToArray();
        }

        [XmlIgnore]
        private File[] _files;
        [XmlIgnore]
        private bool filesChanged = true;

        [XmlElement("file")]
        private File[] files
        {
            get => _files;
            set
            {
                filesChanged = true;
                _files = value;
            }
        }

        [XmlIgnore]
        public FormatedDirectory[] Folders { get; set; }


        public static bool operator ==(FormatedDirectory lhs, FormatedDirectory rhs) => Equals(lhs, rhs);

        public static bool operator !=(FormatedDirectory lhs, FormatedDirectory rhs) => !Equals(lhs, rhs);


        public bool Equals(FormatedDirectory other)
        {
            return new HashSet<File>(files).SetEquals(other.files) && Name == other.Name && new HashSet<FormatedDirectory>(Folders).SetEquals(other.Folders);
        }

        public override int GetHashCode()
        {
            var hashCode = 903005820;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<File[]>.Default.GetHashCode(files);
            hashCode = hashCode * -1521134295 + EqualityComparer<FormatedDirectory[]>.Default.GetHashCode(Folders);
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public class File : IEquatable<File>
        {
            [XmlAttribute("name")]
            public string Name { get; set; }
            [XmlAttribute("template")]
            public string Template { get; set; }

            public static bool operator ==(File lhs, File rhs) => Equals(lhs, rhs);

            public static bool operator !=(File lhs, File rhs) => !Equals(lhs, rhs);

            public bool Equals(File other)
            {
                return Name == other.Name && Template == other.Template;
            }

            public override int GetHashCode()
            {
                var hashCode = 2130014689;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Template);
                return hashCode;
            }

            public override bool Equals(object obj)
            {
                if (obj is File file)
                {
                    return Equals(file);
                }
                return false;
            }
        }

    }



}
