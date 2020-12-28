using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using OpenMcdf;

namespace LocRW
{
    /// <summary>
    /// Contains all the methods used to read/write loc files.
    /// </summary>
    public class LocFile
    {
        #region public methods
        /// <summary>
        /// Read data from .loc file.
        /// </summary>
        /// <param name="path">Path of .loc file.</param>
        /// <returns></returns>
        public static List<TriggerClass> ReadLocFile(string path)
        {
            CompoundFile cf = new CompoundFile(path);

            CFStream triggersStream = cf.RootStorage.GetStream("Messages");
            CFStream messagesStream = cf.RootStorage.GetStream("ls00000409");
            var values = GetValues(triggersStream.GetData());
            var messages = GetMessages(messagesStream.GetData());

            cf.Close();

            //return PackData(values, messages);
            List<TriggerClass> list = new List<TriggerClass>();
            for (int x = 0; x < values.Count(); x++)
            {
                TriggerClass item = new TriggerClass()
                {
                    Value = values[x],
                    Message = messages[x]
                };
                list.Add(item);
            }
            return list;
        }

        /// <summary>
        /// Creates a .loc file using the data provided.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="list"></param>
        public static void WriteLocFile(string path, List<TriggerClass> list)
        {
            CompoundFile cf = new CompoundFile();
            CFStream ls409 = cf.RootStorage.AddStream("ls00000409");
            CFStream messages = cf.RootStorage.AddStream("Messages");

            messages.SetData(SetMessagesForCF(list));
            ls409.SetData(SetLs409forCF(list));

            cf.Save(path);
            cf.Close();
        }
        #endregion


        /// <summary>
        /// Get triggers values from the Messages stream.
        /// </summary>
        /// <param name="data">Byte array from Messages stream.</param>
        /// <returns></returns>
        private static string[] GetValues(byte[] data)
        {
            int n = (data[3] << 8) | data[2];
            string[] values = new string[n];
            int startAddress = 8;
            for (int x = 0; x < n; x++)
            {
                int m = startAddress + 8 * x;
                int index = (data[m + 7] << 24) | (data[m + 6] << 16) | (data[m + 5] << 8) | data[m + 4];
                int value = (data[m + 3] << 24) | (data[m + 2] << 16) | (data[m + 1] << 8) | data[m];
                values[index - 1] = value.ToString();
            }
            return values;
        }

        /// <summary>
        /// Get messages from the LS0000409 stream.
        /// </summary>
        /// <param name="data">Byte array from ls00000409 stream.</param>
        /// <returns></returns>
        private static string[] GetMessages(byte[] data)
        {
            int n = (data[3] << 8) | data[2];
            var pointers = GetPointers(data);
            string[] values = new string[n];
            int offset = n * 12 + 10;
            for (int x = 0; x < n; x++)
            {
                int start = offset + (int)pointers[x] * 2;
                int length;
                if (x < n - 1)
                {
                    length = ((int)pointers[x + 1] - (int)pointers[x]) * 2;
                }
                else
                {
                    length = data.Length - offset - (int)pointers[x] * 2;
                }
                string text = Encoding.Unicode.GetString(data, start, length);
                values[x] = text;
            }
            return values;
        }


        /// <summary>
        /// Get the start address of each message.
        /// </summary>
        /// <param name="data">Byte array from ls00000409 stream.</param>
        /// <returns></returns>
        private static long[] GetPointers(byte[] data)
        {
            int n = (data[3] << 8) | data[2];
            long[] values = new long[n];
            int startAddress = 10;
            for (int x = 0; x < n; x++)
            {
                int m = startAddress + 12 * x;
                double index = (data[m + 3] << 24) | (data[m + 2] << 16) | (data[m + 1] << 8) | data[m];
                long value = (long)((data[m + 11] << 56)| 
                    (data[m + 10] << 48) | 
                    (data[m + 9] << 40) | 
                    (data[m + 8] << 32) | 
                    (data[m + 7] << 24) | 
                    (data[m + 6] << 16) | 
                    (data[m + 5] << 8) | 
                    data[m + 4]
                );
                values[(int)index - 1] = value;
            }
            return values;
        }



        /// <summary>
        /// Set data for Messages stream.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static byte[] SetMessagesForCF(List<TriggerClass> list)
        {
            // Size of stream
            byte[] result = new byte[list.Count * 8 + 8];

            // Set format version
            result[0] = 5;
            byte[] triggersNumber = BitConverter.GetBytes(list.Count);
            // Set total number of triggers
            result[2] = triggersNumber[0];
            result[3] = triggersNumber[1];
            // Set initial version control
            result[4] = 3;

            int offset = 8;
            for (int x = 0; x < list.Count; x++)
            {
                // Set trigger value
                byte[] value = BitConverter.GetBytes(Convert.ToInt32(list[x].Value));
                result[offset + x * 8] = value[0];
                result[offset + x * 8 + 1] = value[1];
                result[offset + x * 8 + 2] = value[2];
                result[offset + x * 8 + 3] = value[3];
                // Set index
                byte[] value2 = BitConverter.GetBytes(x + 1);
                result[offset + x * 8 + 4] = value2[0];
                result[offset + x * 8 + 5] = value2[1];
                result[offset + x * 8 + 6] = value2[2];
                result[offset + x * 8 + 7] = value2[3];
            }
            return result;
        }

        /// <summary>
        /// Set data for ls00000409 stream.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static byte[] SetLs409forCF(List<TriggerClass> list)
        {

            long[] pointers = new long[list.Count];

            pointers[0] = list.Count * 12 + 10;
            for (int x = 1; x < list.Count; x++)
            {
                pointers[x] = (pointers[x - 1] + list[x-1].Message.Trim().Count() * 2); 
            }
            byte[] result = new byte[pointers.Last() + list.Last().Message.Trim().Count() * 2];


            // Set format version
            result[0] = 1;
            // Set total number of triggers
            byte[] triggersNumber = BitConverter.GetBytes(list.Count);
            result[2] = triggersNumber[0];
            result[3] = triggersNumber[1];

            int startAddress = 10;
            for (int x = 0; x < list.Count; x++)
            {
                byte[] value2 = BitConverter.GetBytes(x + 1);
                result[startAddress + x * 12] = value2[0];
                result[startAddress + x * 12 + 1] = value2[1];
                result[startAddress + x * 12 + 2] = value2[2];
                result[startAddress + x * 12 + 3] = value2[3];

                byte[] value = BitConverter.GetBytes((pointers[x] - pointers[0]) / 2);
                result[startAddress + x * 12 + 4] = value[0];
                result[startAddress + x * 12 + 5] = value[1];
                result[startAddress + x * 12 + 7] = value[2];
                result[startAddress + x * 12 + 8] = value[3];
                result[startAddress + x * 12 + 9] = value[4];
                result[startAddress + x * 12 + 10] = value[5];
                result[startAddress + x * 12 + 11] = value[6];
                result[startAddress + x * 12 + 12] = value[7];
            }

            for (int x = 0; x < list.Count; x++)
            {
                long addr = pointers[x];

                UnicodeEncoding Unicode = new UnicodeEncoding();
                int byteCount = Unicode.GetByteCount(list[x].Message.Trim());
                var tempBytes = new byte[byteCount];
                byte[] bytesEncodedCount = Unicode.GetBytes(list[x].Message.Trim());

                foreach (var data in bytesEncodedCount)
                {
                    result[addr++] = data;
                }
            }

            return result;
        }


    }
}
