using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;

namespace PKGBot.CLI
{
    [Verb("extract", HelpText = "Extract data for packages from data sources into a SQLite database.")]
    class ExtractOptions
    {
        [Option('f', "input-file", Required = true, HelpText = "Input data file.")]
        public string InputFile { get; set; }

        [Option('o', "output-file", Required = true, HelpText = "Output SQLite database file.")]
        public string OutputFile { get; set; }

        [Option('n', "--row-limit", Required = false, HelpText = "Limit the number of records to extract from the file.", Default = 0)]
        public int RecordLimit { get; set; }

        [Option('A', "authentication", Required = false, HelpText = "Authentication string (if necessary) that will be used by the selected extractor.")]
        public string Authentication { get; set; }

        [Value(0, Required = true, HelpText = "The extractor to use.")]
        public string Extractor { get; set; }

        [Value(1, Required = false, HelpText = "Any additional parameters for the selected extractor.")]
        public IEnumerable<string> ExtractParameters { get; set; }
    }

    [Verb("transform", HelpText = "Transform data for packages in SQLite database.")]
    class TransformOptions
    {
        [Option('f', "input-file", Required = true, HelpText = "Input SQLite database file.")]
        public string InputFile { get; set; }

    }
}
