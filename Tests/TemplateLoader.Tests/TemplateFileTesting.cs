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
            FileInfo templateFile = new FileInfo("Resources/Templates/CSFileTemplate.txt");
            FileInfo expectedFile = new FileInfo("Resources/Expected/CSFileTemplate.cs");
            TemplateParserBase.Values.AddOverride(new Dictionary<string, object> { 
               { "ProjectName", "Testing" },
               { "fileName", "CSFileTemplate" }
            });
            string templateOutput = await TemplateParserFile.FromFile(templateFile);
            string expected = string.Join(Environment.NewLine, await File.ReadAllLinesAsync(expectedFile.FullName));
            Assert.Equal(expected, templateOutput);
        }
    }
}
