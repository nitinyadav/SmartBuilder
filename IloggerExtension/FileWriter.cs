using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.IO;

namespace Microsoft.UCG.Test.LWA
{
    class FileWriter
    {
        // It stores the logData in a string
        private StringBuilder sb;
        
        //If is used to write the data in the file
        private StreamWriter file;

        //It counts the number of instances of the data in StringBuilder
        private int logInstance;

        /// <summary>
        /// It ctreates the new instance of the stringBuilder
        /// </summary>
        public FileWriter()
        {
            sb = new StringBuilder();
            logInstance = 0;
        }

        /// <summary>
        /// It opens the file with the name supplied as the parameter
        /// The location of log file is the user's Desktop\Report\
        /// </summary>
        /// <param name="name"></param>
        public void open(string name)
        {
            string path;        //to store location of log file.

            if (WindowsIdentity.GetCurrent().Name.Contains(@"\"))
            {
                //to split the domain name from user name
                path = @"C:\Users\" + WindowsIdentity.GetCurrent().Name.Split('\\')[1] + @"\Desktop\Report\";
            }
            else
            {
                path = @"C:\Users\" + WindowsIdentity.GetCurrent().Name + @"\Desktop\Report\";
            }

            //If the directory does not exist at that location it creates the directory.
            System.IO.Directory.CreateDirectory(path);

            path = path + name + " " + System.DateTime.Today.Day + "-" + System.DateTime.Today.Month + ".txt";

            file = new StreamWriter(path);

            file.AutoFlush = true;
        }

        /// <summary>
        /// It is used to write the data in the file. It writes the data in the StringBuilder and
        /// writes data in file once in every 10 calls to the write.
        /// </summary>
        /// <param name="data"></param>
        public void write(string data)
        {
            sb.Append(data);
            logInstance++;
            if (logInstance >= 1 && file != null)
            {
                file.Write(sb);
                logInstance = 0;
            }
        }

        /// <summary>
        /// It closes the file and write if any unsaved data is left on the StringBuilder.
        /// </summary>
        public void close()
        {
            if (logInstance > 0)
            {
                file.Write(sb);
                logInstance = 0; 
            }
            file.Close();
        }
    }
}
