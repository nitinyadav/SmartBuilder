using System;
using System.IO;
using System.Security.Principal;
using System.Collections.Generic;

namespace Logger
{

    public class XMLLoggerExtension : IloggerExtension
    {
        public const string XmlHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
        public const string XmlStyle = "<?xml-stylesheet href=\"Logger2.xslt\" type= \"text/xsl\"?>";

        //each handler is responsible for a seprate test RunId
        private static Dictionary<string, XMLDatatype> handler = new Dictionary<string, XMLDatatype>();

        //This is used to lock the handler creation process, so that multiple handlers are not created for a single runId
        private object createLock = new object();

        public XMLLoggerExtension()
        {
        }

        ~XMLLoggerExtension()
        {
            //string currentKey;
            foreach (KeyValuePair<string, XMLDatatype> kvp in handler)
            {
                if (kvp.Value.filename != null && !kvp.Value.endTagWritten)
                {
                    kvp.Value._writer.write("</Root>");
                    try { kvp.Value._writer.close(); }
                    catch (Exception)
                    { System.Diagnostics.Debug.WriteLine("Error in closing XML file"); }
                }
                //currentKey = kvp.Key;
                //handler.Remove(currentKey);
            }
            handler.Clear();
        }

        public void open(string name)
        {
            if (!handler.ContainsKey(name))
            {
                //Creates a new logFile for that RunId
                lock (createLock)
                {
                    if (!handler.ContainsKey(name))
                    {
                        try
                        {
                            handler.Add(name, new XMLDatatype());
                            handler[name]._writer = new FileWriter();
                            handler[name]._writer.open(name);
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                        }
                    }
                }
            }
        }

        public void consume(string name, string msgobj)
        {
            if (!handler.ContainsKey(name))
            {
                open(name);
            }
            string xmlBlob = msgobj;
            if (!handler[name].headerWritten)
            {
                handler[name]._writer.write(XmlHeader);
                handler[name]._writer.write(XmlStyle);
                //base.Append("<Root>");
                handler[name]._writer.write("<Root xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
                handler[name].headerWritten = true;
            }
            handler[name]._writer.write(xmlBlob);
        }

        public void actionOnCompletion(string name)
        {
            handler[name]._writer.write("</Root>");
            handler[name].endTagWritten = true;
            handler[name]._writer.close();
            handler.Remove(name);
        }
    }
}