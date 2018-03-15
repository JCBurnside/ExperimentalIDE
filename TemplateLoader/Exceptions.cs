using System;
using System.Collections.Generic;
using System.Text;

namespace TemplateLoader.Exceptions
{
    public class IllegalPipeArgumentException : Exception
    {
        public IllegalPipeArgumentException(string pipeName) : base($"Pipe {pipeName} is used on an invalid arg") { }

        public IllegalPipeArgumentException(string pipeName, string arg) : base($"{arg} can't be used on pipe {pipeName}") { }
    }

    public class IllegalPipeException : Exception
    {
        public IllegalPipeException(string pipeName) : base($"Pipe {pipeName} does not exist") { }

        public IllegalPipeException() : base("No Pipe Specified") { }
    }
}
