using ReportingWeb.ExportFiles;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace ReportingWeb.ExportFiles
{
    //use TableStorageClient to get data and export it as csv
    public static class ExportAsCSV
    {
        //return csv file according to Event id
        //1. creae file.
        //2. query data using client class
        //3. store data and reutrn FileDetail

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