using System;
using System.Collections.Generic;
using System.Text;

namespace TemplateLoader
{
    /// <summary>
    /// Used to indicate something is wrong the loading template
    /// </summary>
    public class IllegalTemplateException : Exception
    {
        /// <summary>
        /// true if the illeage template is for creation of Files
        /// </summary>
        public bool IsFile { get; }

        /// <summary>
        /// full path of the template file
        /// </summary>
        public string TemplateLocation { get; }

        /// <summary>
        /// true if the file truely exists
        /// </summary>
        public bool Exists { get; }

        /// <summary>
        /// creates new IllegalTemplateException with the default message
        /// </summary>
        /// <param name="isFile">if it a file template</param>
        /// <param name="templateLocation">location of template</param>
        /// <param name="exists">if the file exists</param>
        public IllegalTemplateException(bool isFile = false, string templateLocation = null, bool exists = false) : this($"Illegal Template Found at {templateLocation ?? "null"}", isFile, templateLocation, exists) { }


        /// <summary>
        /// creates new IllegalTemplateException with the default message
        /// </summary>
        /// <param name="isFile">if it a file template</param>
        /// <param name="templateLocation">location of template</param>
        /// <param name="exists">if the file exists</param>
        /// <param name="message">message to send</param>
        public IllegalTemplateException(string message, bool isFile = false, string templateLocation = null, bool exists = false) : base(message)
        {
            IsFile = isFile;
            TemplateLocation = templateLocation;
            Exists = exists;
        }
    }
}
