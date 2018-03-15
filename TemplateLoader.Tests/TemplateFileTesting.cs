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
            FileInfo file = new FileInfo("Resources/templates/CSFileTemplate.txt");
            await TemplateParserFile.FromFile(file);
            string[] templateOutput = { };
            for (int ctr = 0; ctr < templateOutput.Length; ctr++)
            {
            }
        }
    }
}
