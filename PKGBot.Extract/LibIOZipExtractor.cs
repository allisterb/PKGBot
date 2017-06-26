using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Serilog;

namespace PKGBot
{
    public class LibIOZipExtractor : Extractor
    {
        public LibIOZipExtractor(ILogger logger, FileInfo inputFile, Dictionary<string, object> options) : base(logger,inputFile,  options) 
        {
            using (FileStream file = InputFile.OpenRead())
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    using (Stream stream = entry.Open())
                    {
                        // do whatever we want with stream
                        // ...
                    }
                }
            }
        }
    }
}
