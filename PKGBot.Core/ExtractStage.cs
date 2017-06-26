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
                else
                {
                    FileInfo inputFile = (FileInfo) extract_options["InputFile"];
                    Extractor = new LibIOZipExtractor(logger, inputFile, extract_options);
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
