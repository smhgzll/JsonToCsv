using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using CsvHelper;
using Newtonsoft.Json;

namespace JsonToCsv
{
    class Program
    {
        static void Main(string[] args)
        {
            var checkFolderPath = "C:/temp/NewImpl";
            string[] files = System.IO.Directory.GetFiles(checkFolderPath, "*.json", SearchOption.AllDirectories);

            Stopwatch watch = new Stopwatch();
            foreach (var file in files)
            {
                watch.Reset();
                watch.Start();
                var fileInfo = new FileInfo(file);
                var outputFileName = fileInfo.FullName.Replace(fileInfo.Extension, ".csv");
                var jsonContent = File.OpenText(file).ReadToEnd();
                var csv = jsonToCSV(jsonContent, ",");
                File.WriteAllText(outputFileName, csv);
                watch.Stop();
                Console.WriteLine($"File ({fileInfo.Name}) converted in {watch.Elapsed.Minutes} minutes.");
            }
            Console.WriteLine($"All done...");
        }

        public static string jsonToCSV(string jsonContent, string delimiter)
        {
            StringWriter csvString = new StringWriter();
            using (var csv = new CsvWriter(csvString))
            {
                //csv.Configuration.SkipEmptyRecords = true;
                //csv.Configuration.WillThrowOnMissingField = false;
                csv.Configuration.Delimiter = delimiter;

                using (var dt = jsonStringToTable(jsonContent))
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        csv.WriteField(column.ColumnName);
                    }
                    csv.NextRecord();

                    foreach (DataRow row in dt.Rows)
                    {
                        for (var i = 0; i < dt.Columns.Count; i++)
                        {
                            csv.WriteField(row[i]);
                        }
                        csv.NextRecord();
                    }
                }
            }
            return csvString.ToString();
        }

        public static DataTable jsonStringToTable(string jsonContent)
        {
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonContent);
            return dt;
        }
    }
}
