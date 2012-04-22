using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Principal;

namespace Logger
{
    //The FileLogger class logs the data into a log file
    public class FileLoggerExtension : IloggerExtension
    {
        //each handler is responsible for a seprate test RunId
        private static Dictionary<string, FileWriter> handler = new Dictionary<string, FileWriter>();

        //This is used to lock the handler creation process, so that multiple handlers are not created for a single runId
        private object createLock = new object();

        //For internal logging
        private static StreamWriter file;

        /// <summary>
        /// 
        /// </summary>
        public FileLoggerExtension()
        {
            file = new StreamWriter(@"E:\log.txt");
        }

        /// <summary>
        /// This is used to cloes all the unclosed logFiles.
        /// </summary>
        ~FileLoggerExtension()
        {
            //close all the open log files
            foreach (KeyValuePair<string, FileWriter> kvp in handler)
            {
                try
                {
                    kvp.Value.close();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
            handler.Clear();
        }

        /// <summary>
        /// It consumes the logData by writing the data in the logFile.
        /// The logFile is named as "runId today.day-today.month.txt".
        /// All the logData associated with this runId is stored in it's file.
        /// If the file doesnot exist then it creates that file and writes data into it.
        /// </summary>
        /// <param name="RunId"></param>
        /// <param name="data"></param>
        public void consume(string name, string data)
        {
            file.WriteLine("File consum called with data:"+data+ " and name :"+name);
            try
            {
                handler[name].write(data);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public void open(string name)
        {
            file.WriteLine("File open called with  name :" + name);
            if (!handler.ContainsKey(name))
            {
                //Creates a new logFile for that RunId
                lock(createLock){
                    if (!handler.ContainsKey(name))
                    {
                        try
                        {
                            handler.Add(name, new FileWriter());
                            handler[name].open(name);
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This function closes the logFile of that particular handler after it send the finish message.
        /// </summary>
        /// <param name="RunId"></param>
        public void actionOnCompletion(string name)
        {
            if (handler.ContainsKey(name))
            {
                try
                {
                    handler[name].close();
                    handler.Remove(name);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
        }
    }
}
