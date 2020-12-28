using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocRW
{
    class CsvTest
    {
        public struct ResultStruct
        {
            string ErrorFlag;
            string Message;
        }

        public static ResultStruct Test(string path)
        {
            ResultStruct result = new ResultStruct();
            // Test if file exists

            // Test if header exists

            // Test if comma separator is used

            return result;
        }
    }
}
