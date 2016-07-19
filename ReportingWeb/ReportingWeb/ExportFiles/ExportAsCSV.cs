using System;
using System.Collections.Generic;

namespace ReportingWeb.ExportFiles
{
    //use TableStorageClient to get data and export it as csv
    public static class ExportAsCSV
    {


        /// <summary>
        /// 1. create file.
        /// 2. query data using client class
        /// 3. store data and reutrn FileDetail
        /// </summary>
        /// <param name="input">List of records to write to file</param>
        /// <returns>return csv file according to Event id</returns>
        public static string Export<T>(List<T> input) where T : class
        {
            string path = @"C:\WINDOWS\TEMP\{0}_{1}.csv";
            path = string.Format(path, DateTime.Now.Ticks, typeof(T).Name);
            CsvExport<T> csv = new CsvExport<T>(input);
            csv.ExportToFile(path);
            return path;
        }
    }
}