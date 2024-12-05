using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Xml;
using System.Xml.Xsl;

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
                // Check for null or empty arrays
                if (InputFiles == null || OutputFiles == null)
                {
                    Log.LogError("InputFiles and OutputFiles cannot be null.");
                    return false;
                }

                if (InputFiles.Length != OutputFiles.Length)
                {
                    Log.LogError("InputFiles and OutputFiles must have the same number of items.");
                    return false;
                }

                if (!System.IO.File.Exists(XsltFile))
                {
                    Log.LogError($"XSLT file '{XsltFile}' does not exist.");
                    return false;
                }

                var xslt = new XslCompiledTransform();
                xslt.Load(XsltFile);

                for (int i = 0; i < InputFiles.Length; i++)
                {
                    var inputFile = InputFiles[i].ItemSpec;
                    var outputFile = OutputFiles[i].ItemSpec;

                    if (!System.IO.File.Exists(inputFile))
                    {
                        Log.LogError($"Input file '{inputFile}' does not exist.");
                        continue;
                    }

                    var xsltArgs = new XsltArgumentList();
                    if (!string.IsNullOrEmpty(Parameters))
                    {
                        foreach (var param in Parameters.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var kvp = param.Split('=');
                            if (kvp.Length == 2)
                            {
                                xsltArgs.AddParam(kvp[0], "", kvp[1]);
                            }
                            else
                            {
                                Log.LogWarning($"Invalid parameter format: '{param}'. Expected 'name=value'.");
                            }
                        }
                    }

                    // Ensure the directory for the output file exists
                    var outputDir = System.IO.Path.GetDirectoryName(outputFile);
                    if (!System.IO.Directory.Exists(outputDir))
                    {
                        System.IO.Directory.CreateDirectory(outputDir);
                    }

                    Log.LogMessage($"Transforming '{inputFile}' to '{outputFile}' using XSLT '{XsltFile}'.");

                    using (var writer = XmlWriter.Create(outputFile))
                    {
                        xslt.Transform(inputFile, xsltArgs, writer);
                    }
                }

                return !Log.HasLoggedErrors;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex, true);
                return false;
            }
        }
    }
}
