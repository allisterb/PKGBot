using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Serilog;
using CommandLine;

namespace PKGBot.CLI
{
    class Program
    {
        public enum ExitResult
        {
            SUCCESS = 0,
            INVALID_OPTIONS = 1,
            INPUT_FILE_ERROR = 2,
            ERROR_TRANSFORMING_DATA = 3
        }

        static Dictionary<string, string> AppConfig { get; set; }
        static ILogger L;
        static List<string> Extractors { get; set; } = new List<string> { "libio"};
        static ExtractOptions ExtractOptions { get; set; }
        static Extractor Extractor { get; set; }
        static FileInfo InputFile { get; set; }
        
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.LiterateConsole()
               .CreateLogger();
            L = Log.ForContext<Program>();

            var result = Parser.Default.ParseArguments<ExtractOptions, TransformOptions>(args)
            .WithNotParsed((IEnumerable<Error> errors) =>
            {
                Exit(ExitResult.INVALID_OPTIONS);

            })
            .WithParsed((ExtractOptions o) =>
            {
                if (Extractors.Contains(o.Extractor))
                {
                    ExtractOptions = o;
                    Extract();
                }
                else
                {
                    L.Error("The current extractors are: {extractors}.", Extractors);
                    Exit(ExitResult.INVALID_OPTIONS);
                }

            });
        }

        static bool Extract()
        {
            Dictionary<string, object> extract_options = new Dictionary<string, object>();
            foreach (PropertyInfo prop in ExtractOptions.GetType().GetProperties())
            {
                extract_options.Add(prop.Name, prop.GetValue(ExtractOptions));
            }
            ExtractStage ES = new ExtractStage(Log.Logger, ExtractOptions.Extractor, extract_options);
            return ES.Extractor != null && ES.Extractor.Initialised;
        }

        static void Exit(ExitResult result)
        {
            Log.CloseAndFlush();
            Environment.Exit((int)result);
        }

        static int ExitWithCode(ExitResult result)
        {
            Log.CloseAndFlush();
            return (int)result;
        }

    }
}
