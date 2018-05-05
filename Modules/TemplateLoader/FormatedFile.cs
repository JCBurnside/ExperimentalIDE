using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TemplateLoader
{
    public class FormatedFile : IEquatable<FormatedFile>
    {
        public string Name { get; internal set; }
        public string Contents { get; internal set; }

        public FormatedFile(string name = "file.txt")
        {
            Name = name;
            Contents = "";
        }

        public static explicit operator FormatedFile(FileInfo info)
        {
            return new FormatedFile
            {
                Name = info.Name.Substring(0, info.Name.IndexOf('.')),
                Contents = String.Join(Environment.NewLine, File.ReadAllLines(info.FullName))
            };
        }

        public static bool operator ==(FormatedFile lhs, FormatedFile rhs)
            => lhs.Name == rhs.Name && lhs.Contents == rhs.Contents;

        public static bool operator !=(FormatedFile lhs, FormatedFile rhs)
            => lhs.Name != rhs.Name || lhs.Contents != rhs.Contents;

        public override bool Equals(object obj)
        {
            if (obj is FormatedFile file)
            {
                return file != null &&
                       Name == file.Name &&
                       Contents == file.Contents;
            }
            return false;
        }

        public override int GetHashCode()
        {

            return new { Name, Contents }.GetHashCode();
        }

        public bool Equals(FormatedFile other)
        {
            return this == other;
        }
    }
}
