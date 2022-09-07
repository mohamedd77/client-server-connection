using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {

        public static void LogException(Exception ex)
        {

            
            string filePath = @"C:\Users\dell\Desktop\Network\HTTPServer\bin\Debug\log.txt";
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Date : " + DateTime.Now.ToString());
               

                while (ex != null)
                {
                    writer.WriteLine(ex.GetType().FullName);
                    writer.WriteLine("Message : " + ex.Message);
                    ex = ex.InnerException;
                }
            }
        }
    }
}
