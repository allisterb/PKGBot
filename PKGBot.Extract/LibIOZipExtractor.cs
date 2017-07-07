using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Diagnostics;
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
            using (FileStream file = InputFile.OpenRead())
            using (ZipReader zipReader = ZipReader.Open(file, new ReaderOptions() { LeaveStreamOpen = false }))
            {
                while (zipReader.MoveToNextEntry())
                {
                    L.Debug("Moving to file {0} in zip archive.", zipReader.Entry.Key);
                    using (Stream f = zipReader.OpenEntryStream())
                    using (StreamReader sr = new StreamReader(f))
                    {
                        CsvReader csv = new CsvReader(sr, true);
                        if (zipReader.Entry.Key.ToLower().StartsWith("dependencies"))
                        {
                            SQLiteConnection conn = new SQLiteConnection("Data Source = {0}; Version = 3; Journal Mode = Off; Synchronous=Off".F(OutputFile.FullName));
                            string column_names = csv.GetFieldHeaders().Aggregate((s1, s2) => "{0},\"{1}\"".F(s1, s2));
                            string ddl = "create table Dependencies" + "(" + column_names + ")";
                            conn.Open();
                            L.Information("Creating Dependencies table in database {db}", ddl, conn.FileName);
                            L.Debug("Executing SQL {sql} in database {db}", ddl, conn.FileName);
                            SQLiteCommand create_cmd = new SQLiteCommand(ddl, conn);
                            create_cmd.ExecuteNonQuery();
                            conn.Close();
                            this.ExtractDependenciesTable(L, csv, OutputFile.FullName);
                        }
          
                    }

                }
            }
        }

        protected Action<ILogger, CsvReader, string> ExtractDependenciesTable { get; } = (l, csv, f) =>
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source = {0}; Version = 3; Journal Mode = Memory; Page Size=1024; Synchronous=Off".F(f)))
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                conn.Open();
                long insert_count = 0;
                int field_count = csv.FieldCount;
                StringBuilder p = new StringBuilder("(");
                for (int i = 0; i < field_count; i++)
                {
                    p.Append("?,");
                }
                p.Remove(p.Length - 1, 1);
                p.Append(")");
                SQLiteTransaction trans = conn.BeginTransaction(IsolationLevel.Serializable);
                SQLiteCommand insert_cmd = null;
                insert_cmd = new SQLiteCommand("INSERT INTO Dependencies VALUES " + p.ToString(), conn, trans);
                insert_cmd.Parameters.Add(new SQLiteParameter(DbType.Int64));
                for (int i = 1; i < field_count; i++)
                {
                    insert_cmd.Parameters.Add(new SQLiteParameter(DbType.String));
                }
                StringBuilder values = new StringBuilder(field_count * 100);
                while (csv.ReadNextRecord())
                {
                    insert_cmd.Reset();
                    insert_cmd.Parameters[0].Value = csv[0];
                    string v;
                    for (int c = 1; c < field_count; c++)
                    {
                        v = csv[c];
                        v.Replace("\"", string.Empty);
                        v.Replace("\'", string.Empty);
                        insert_cmd.Parameters[c].Value = "\"" + v + "\"";
                     }
                    insert_cmd.ExecuteNonQuery();
                    if (++insert_count % 100000 == 0)
                    {
                        trans.Commit();
                        sw.Stop();
                        l.Information("Inserted {count} records into Dependencies table in {time}s.", insert_count, sw.ElapsedMilliseconds / 1000);
                        sw.Start();
                        trans = conn.BeginTransaction(IsolationLevel.Serializable);
                    }
                  }
            }
        };
    }
}
