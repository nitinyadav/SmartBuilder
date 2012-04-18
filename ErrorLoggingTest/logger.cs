using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.OleDb;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Soap;

namespace ErrorLoggingTest
{
    class logger
    {
        StreamWriter fs;
        StreamWriter count;
        Stream db;
        public static List<bugData> bugs = new List<bugData>();
        public static List<StackApi.RootObject> solutions = new List<StackApi.RootObject>();
        //public XmlSerializer x;
        SoapFormatter sf;
        
        public logger()
        {
            //bugs = new List<bugData>();
            //x = new XmlSerializer(bugs.GetType());
            sf = new SoapFormatter();
            initialize();
        }

        ~logger()
        {
            //x.Serialize(db,bugs);
        }

        public void initialize()
        {
            loadBugs();
            loadSolutions();
        }

        private void loadBugs()
        {
            if (File.Exists("E:\\Data\\count.txt") && (bugs.Count == 0))
            {
                StreamReader sr = new StreamReader("E:\\Data\\count.txt");
                string line = sr.ReadLine();
                int c = int.Parse(line);
                sr.Close();
                for (int i = 1; i <= c; i++)
                {
                    try
                    {
                        db = File.Open("E:\\Data\\data" + i + ".bin", FileMode.Open);
                        bugs.Add((bugData)sf.Deserialize(db));
                        db.Close();
                    }
                    catch (Exception e) { System.Console.WriteLine(e.Message); }
                }
            }
        }

        private void loadSolutions()
        {
            if (File.Exists("E:\\Data\\Solcount.txt") && (bugs.Count == 0))
            {
                StreamReader sr = new StreamReader("E:\\Data\\Solcount.txt");
                string line = sr.ReadLine();
                int c = int.Parse(line);
                sr.Close();
                for (int i = 1; i <= c; i++)
                {
                    try
                    {
                        db = File.Open("E:\\Data\\Solution" + i + ".bin", FileMode.Open);
                        solutions.Add((StackApi.RootObject)sf.Deserialize(db));
                        db.Close();
                    }
                    catch (Exception e) { System.Console.WriteLine(e.Message); }
                }
            }
        }

        public void log(string data)
        {
            fs = new StreamWriter("E:\\Data\\log.txt", true);
            fs.AutoFlush = true;
            fs.WriteLine(System.DateTime.Now + " : " + data);
            fs.Close();
        }

        public StackApi.RootObject retrievStored(string bId)
        {
            foreach (StackApi.RootObject obj in solutions)
            {
                if (obj.bugId.Equals(bId))
                    return obj;
            }
            return null;
        }

        public void logAnswer(StackApi.RootObject var)
        {
            solutions.Add(var);
            try
            {
                db = File.Open("E:\\Data\\Solution" + solutions.Count + ".bin", FileMode.Create);
                sf.Serialize(db, var);
                db.Close();
                count = new StreamWriter("E:\\Data\\Solcount.txt", false);
                count.Write(solutions.Count);
                count.Flush();
                count.Close();
            }
            catch (Exception e) { System.Console.WriteLine(e.Message); }
        }

        public string logBugData(bugData input)
        {
            bugs.Add(input);
            input.BugId = bugs.Count.ToString();
            try
            {
                db = File.Open("E:\\Data\\data" + bugs.Count + ".bin", FileMode.Create);
                sf.Serialize(db, input);
                db.Close();
                count = new StreamWriter("E:\\Data\\count.txt", false);
                count.Write(bugs.Count);
                count.Flush();
                count.Close();
            }
            catch (Exception e) { System.Console.WriteLine(e.Message); }
            return bugs.Count.ToString();
            //x.Serialize(System.Console.Out, bugs);
            
            //db.WriteLine("<Data>");
            //db.WriteLine("<softwareName>"+input.SoftwareName+"</softwareName>");
            //db.WriteLine("<vendor>" + input.Vendor + "</vendor>");
            //db.WriteLine("<version>" + input.Version + "</version>");
            //db.WriteLine("<softwareInfo>" + input.SoftwareInfo + "</softwareInfo>");
            //db.WriteLine("<filename>" + input.Filename + "</filename>");
            //db.WriteLine("<stackTrace>" + input.StackTrace + "</stackTrace>");
            //db.WriteLine("<errorMessage>" + input.ErrorMessage + "</errorMessage>");
            //db.WriteLine("<outputText>" + input.OutputText + "</outputText>");
            //db.WriteLine("<moreOptions>" + input.MoreOptions + "</moreOptions>");
            //db.WriteLine("<bugId>" + input.BugId + "</bugId>");
            //db.WriteLine("<userId>" + input.UserId + "</userId>");
            //db.WriteLine("<guid>" + input.Guid + "</guid>");
            //db.WriteLine("<nameSpace>" + input.NameSpace + "</nameSpace>");
            //db.WriteLine("<token>" + input.Token + "</token>");
            //db.WriteLine("<operatingSystem>" + input.OperatingSystem + "</operatingSystem>");
            //db.WriteLine("<tags>");
            //bool flag = false;
            //foreach (string tag in input.Tags)
            //{
            //    if (flag == false)
            //    {
            //        db.WriteLine(tag);
            //        flag = true;
            //    }
            //    else
            //    {
            //        db.WriteLine(","+tag);
            //    }
            //}
            //db.WriteLine("</tags>");
            //db.WriteLine("</Data>");
        }
    }
}
