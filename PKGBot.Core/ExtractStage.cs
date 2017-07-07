using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Serilog;

namespace PKGBot
{
    public class ExtractStage
    {
        #region Constructors
        public ExtractStage(ILogger logger, string extractor, Dictionary<string, object> extract_options)
        {
            this.L = logger;
            if (extractor == "libio")
            {
                if (extract_options.Count < 1 || !extract_options.ContainsKey("InputFile"))
                {
                    L.Error("At least 1 option is needed for the LibIO extractor: the input file.");
                    return;
                }
                else if (!extract_options.ContainsKey("OutputFile"))
                {
                    L.Error("The output file option must be specified.");
                    return;
                }
                else
                {
                    FileInfo inputFile = new FileInfo((string) extract_options["InputFile"]);
                    if (!inputFile.Exists)
                    {
                        L.Error("The input file {file} does not exist.", inputFile.FullName);
                        return;
                    }
                    else
                    {
                        bool overwrite = extract_options.ContainsKey("Overwrite") ? true : false;
                        FileInfo outputFile = new FileInfo((string) extract_options["OutputFile"]);
                        if (outputFile.Exists && !overwrite)
                        {
                            L.Information("The output file {file} exists and the --overwrite option was not specified.", outputFile.FullName);
                            return;
                        }
                        else if (outputFile.Exists && overwrite)
                        {
                            L.Information("Overwriting output file {file}.", outputFile.FullName);
                            outputFile.Delete();
                        }
                        Extractor = new LibIOZipExtractor(logger, inputFile, extract_options);
                    }
                    
                }
            }
            else
            {
                throw new ArgumentException("Unknown extractor: " + extractor);
            }
        }
        #endregion

        #region Properties
        ILogger L;
        public Extractor Extractor { get; protected set; }
        #endregion

        #region Methods
        public bool Run(int recordsLimit, Dictionary<string, string> options)
        {
            Contract.Requires(Extractor != null);
            Contract.Requires(this.Extractor.Initialised == true);
            return true;
        }
        #endregion
    }
}
