using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Serilog;
using SharpCompress;
using SharpCompress.Common;
using SharpCompress.Common.Zip;
using SharpCompress.Readers;
using SharpCompress.Readers.Zip;
using LumenWorks.Framework.IO.Csv;
using System.Data.SQLite;

namespace PKGBot
{
    public class LibIOZipExtractor : Extractor
    {
        public LibIOZipExtractor(ILogger logger, FileInfo inputFile, Dictionary<string, object> options) : base(logger,inputFile,  options) 
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source = {0}; Version = 3; Journal Mode = Off;".F(OutputFile.FullName)))
            using (FileStream file = InputFile.OpenRead())
            using (ZipReader reader = ZipReader.Open(file, new ReaderOptions() { LeaveStreamOpen = false }))
            {
                conn.Open();
                
                while (reader.MoveToNextEntry())
                {
                    L.Debug("Moving to file {0} in zip archive.", reader.Entry.Key);
                    using (Stream f = reader.OpenEntryStream())
                    using (StreamReader sr = new StreamReader(f))
                    {
                        CsvReader csv = new CsvReader(sr, true);
                        string ddl = "create table Dependencies" + "(" + csv.GetFieldHeaders().Aggregate((s1, s2) => "{0},\"{1}\"".F(s1, s2)) + ")";
                        
                        SQLiteCommand cmd = new SQLiteCommand(ddl, conn);
                        cmd.ExecuteNonQuery(); 
                        while (csv.ReadNextRecord())
                        {
                            foreach (var c in csv)
                            {

                            }
                        }
                     
                    }

                }
            }
        }
    }
}
