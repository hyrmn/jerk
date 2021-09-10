using CsvHelper;

using DotNet.Globbing;

using Spectre.Console;
using Spectre.Console.Cli;

using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace src
{
    class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandApp<ReportCombinatorCommand>();

            return app.Run(args);
        }
    }

    public class ReportCombinatorCommand : Command<ReportCombinatorCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [Description("Path to search. Defaults to current working directory.")]
            [CommandArgument(0, "[searchPath]")]
            [DefaultValue(".")]
            public string? SearchPath { get; set; }

            [CommandOption("-p|--pattern <GLOB>")]
            [Description("The file glob to match")]
            [DefaultValue("**/TestOutputResults.xml")]
            public string SearchPattern { get; set; }

            [CommandOption("-o|--output <csv>")]
            [Description("The output file")]
            [DefaultValue("./combined-pain-list.csv")]
            public string OutputFile { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            var processor = new FileProcessor();
            var combinedOutput = new List<TestResult>();
            var files = getFiles(settings);

            AnsiConsole.MarkupLine($"Found [green]{files.Count}[/] files to process");

            foreach (var file in files)
            {
                var resultsFromFile = processor.Parse(file);
                combinedOutput.AddRange(resultsFromFile);
            }

            writeCombinedFile(settings, combinedOutput);

            AnsiConsole.MarkupLine($"Wrote [yellow]{combinedOutput.Count}[/] test result summaries to [green]{settings.OutputFile}[/]");

            return 0;

            static List<FileInfo> getFiles(Settings settings)
            {
                var options = new GlobOptions();
                options.Evaluation.CaseInsensitive = true;

                var glob = Glob.Parse(settings.SearchPattern, options);
                var directory = new DirectoryInfo(settings.SearchPath);
                return directory.GetFiles("*", SearchOption.AllDirectories).Where(f => glob.IsMatch(f.FullName)).ToList();
            }

            static void writeCombinedFile(Settings settings, List<TestResult> combinedOutput)
            {
                var writer = File.CreateText(settings.OutputFile);
                var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                csv.WriteHeader<TestResult>();
                csv.NextRecord();
                csv.WriteRecords(combinedOutput);
                csv.Flush();
            }
        }
    }

    public record TestResult(string TestName, string Outcome, string TestOutput);

    public class FileProcessor
    {
        public IEnumerable<TestResult> Parse(FileInfo file)
        {
            var doc = XDocument.Load(file.FullName);
            var results = doc.Descendants().Where(node => node.Name.LocalName == "UnitTestResult");
            foreach(var node in results)
            {
                var testOutput = getTestOutput(node);

                yield return new TestResult(node.Attribute("testName").Value, node.Attribute("outcome").Value, testOutput);
            }

            static string getTestOutput(XElement node)
            {
                var outputNode = node.Descendants().Where(n => n.Name.LocalName == "Output").FirstOrDefault();
                if (outputNode is null) return string.Empty;

                var testOutputNode = outputNode.Descendants().FirstOrDefault(n => n.Name.LocalName == "StdOut")
                    ?? outputNode.Descendants().FirstOrDefault(n => n.Name.LocalName == "Message");

                return testOutputNode?.Value ?? string.Empty;
            }
        }
    }
}
