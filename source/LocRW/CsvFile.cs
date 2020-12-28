using CsvHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace LocRW
{
    public class CsvFile
    {
        public static void WriteCSV(string path, List<TriggerClass> triggers)
        {
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(triggers);
            }
        }

        public static List<TriggerClass> ReadCSV(string path)
        {
            List<TriggerClass> result = new List<TriggerClass>();

            try
            {
                using (var reader = new StreamReader(path))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<TriggerClass>();
                    result = records.ToList();
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
            return result;
        }
    }
}
