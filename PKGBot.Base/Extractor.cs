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
    public abstract class Extractor
    {
        #region Constructors
        protected Extractor(ILogger logger, Dictionary<string, object> options)
        {
            Contract.Requires(logger != null);
            L = logger.ForContext<Extractor>();
            Options = options;
        }


        public Extractor(ILogger logger, FileInfo inputFile, Dictionary<string, object> options) : this(logger, options)
        {
            Contract.Requires(inputFile != null);
            Contract.Requires(options.ContainsKey("OutputFile"));
            if (!inputFile.Exists)
            {
                L.Error("The input file {file} does not exist.", inputFile);
                return;
            }
            else
            {
                InputFile = inputFile;
                OutputFile = new FileInfo((string) options["OutputFile"]);
                this.Initialised = true;
            }
        }
        #endregion

        #region Properties
        public bool Initialised { get; protected set; } = false;
        public ILogger L { get; set; }
        public Dictionary<string, object> Options { get; protected set; }
        public FileInfo InputFile { get; protected set; }
        public FileInfo OutputFile { get; protected set; }
        public string Authentication { get; protected set; } = string.Empty;
        #endregion
    }
}
