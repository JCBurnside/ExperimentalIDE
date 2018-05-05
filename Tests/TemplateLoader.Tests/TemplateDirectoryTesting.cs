using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Xunit;

namespace TemplateLoader.Tests
{
    public class TemplateDirectoryTesting
    {
        [Fact]
        public async void DirectoryTemplateCSTest()
        {
            try
            {

                TemplateDirectoryParser parser = new TemplateDirectoryParser();
                FileInfo inputInfo = new FileInfo("Resources/Templates/CSDirectory.xml");
                FormatedDirectory output = await parser.FromFile(inputInfo);
                FormatedDirectory innerExpected = new FormatedDirectory
                {
                    Name = "testing"
                };
                TemplateFileParser fileParser = new TemplateFileParser();



                innerExpected.SetFiles(new FormatedFile[]{
                    new FormatedFile("test.txt")
                });

                FormatedDirectory expected = new FormatedDirectory
                {
                    Folders = new[]{
                        innerExpected
                    }
                };
                Assert.Equal(expected,output);
            }
            catch (FileNotFoundException)
            {
                //System.Diagnostics.Debug.WriteLine(fnf.FileName);
                Assert.True(false);
            }
        }
    }
}
