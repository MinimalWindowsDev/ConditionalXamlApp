using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Xml.Xsl;
using System.Xml;

namespace CustomTasks
{
    public class XsltTransformTask : Task
    {
        [Required]
        public ITaskItem[] InputFiles { get; set; }

        [Required]
        public ITaskItem[] OutputFiles { get; set; }

        [Required]
        public string XsltFile { get; set; }

        public string Parameters { get; set; }

        public override bool Execute()
        {
            try
            {
                var xslt = new XslCompiledTransform();
                xslt.Load(XsltFile);

                for (int i = 0; i < InputFiles.Length; i++)
                {
                    var inputFile = InputFiles[i].ItemSpec;
                    var outputFile = OutputFiles[i].ItemSpec;

                    var xsltArgs = new XsltArgumentList();
                    if (!string.IsNullOrEmpty(Parameters))
                    {
                        foreach (var param in Parameters.Split(';'))
                        {
                            var kvp = param.Split('=');
                            xsltArgs.AddParam(kvp[0], "", kvp[1]);
                        }
                    }

                    using (var writer = XmlWriter.Create(outputFile))
                    {
                        xslt.Transform(inputFile, xsltArgs, writer);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }
    }
}
