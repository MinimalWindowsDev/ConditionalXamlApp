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

        private void LogDebugInfo(string message)
        {
            Log.LogMessage(MessageImportance.High, $"[DEBUG] {message}");
        }

        public override bool Execute()
        {
            try
            {
                LogDebugInfo($"Starting XSLT transformation");
                LogDebugInfo($"XSLT file: {XsltFile}");
                LogDebugInfo($"Raw parameters: {Parameters}");

                var settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Parse,
                    IgnoreWhitespace = false
                };

                var xslt = new XslCompiledTransform(true);
                using (var reader = XmlReader.Create(XsltFile, settings))
                {
                    xslt.Load(reader);
                }

                for (int i = 0; i < InputFiles.Length; i++)
                {
                    var inputFile = InputFiles[i].ItemSpec;
                    var outputFile = OutputFiles[i].ItemSpec;
                    LogDebugInfo($"Processing {inputFile} -> {outputFile}");

                    var xsltArgs = new XsltArgumentList();
                    if (!string.IsNullOrEmpty(Parameters))
                    {
                        foreach (var param in Parameters.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var kvp = param.Split(new[] { '=' }, 2);
                            if (kvp.Length == 2)
                            {
                                string paramName = kvp[0].Trim();
                                string paramValue = kvp[1].Trim();
                                LogDebugInfo($"Parameter: {paramName} = {paramValue}");
                                xsltArgs.AddParam(paramName, "", paramValue);
                            }
                        }
                    }

                    var outputDir = System.IO.Path.GetDirectoryName(outputFile);
                    if (!string.IsNullOrEmpty(outputDir) && !System.IO.Directory.Exists(outputDir))
                    {
                        System.IO.Directory.CreateDirectory(outputDir);
                    }

                    using (var reader = XmlReader.Create(inputFile, settings))
                    {
                        var writerSettings = new XmlWriterSettings
                        {
                            Indent = true,
                            IndentChars = "    ",
                            NewLineChars = "\n",
                            NewLineHandling = NewLineHandling.Replace
                        };

                        using (var writer = XmlWriter.Create(outputFile, writerSettings))
                        {
                            xslt.Transform(reader, xsltArgs, writer);
                        }
                    }

                    LogDebugInfo($"Transformation completed for {inputFile}");
                    if (System.IO.File.Exists(outputFile))
                    {
                        string content = System.IO.File.ReadAllText(outputFile);
                        LogDebugInfo($"Transformed content:\n{content}");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex, true);
                return false;
            }
        }
    }
}