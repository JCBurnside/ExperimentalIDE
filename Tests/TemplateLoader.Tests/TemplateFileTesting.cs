using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace TemplateLoader.Tests
{
    public class TemplatFileTesting
    {
        [Fact]
        public async void FileTemplateCSTest()
        {
            TemplateFileParser fileParser = new TemplateFileParser(new Dictionary<string, object>{
               { "ProjectName", "Testing" },
               { "fileName", "CSFileTemplate" }
            });
            FileInfo templateFile = new FileInfo("Resources/Templates/CSFileTemplate.txt");
            FileInfo expectedFile = new FileInfo("Resources/Expected/CSFileTemplate.cs");
            Assert.Equal(await fileParser.FromFile(templateFile),(FormatedFile)expectedFile);
        }
    }
}
