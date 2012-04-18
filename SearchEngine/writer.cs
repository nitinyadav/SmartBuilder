using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SearchEngine
{
    class writer
    {
        System.IO.StreamWriter file;

        public writer(string path)
        {
            file =  new System.IO.StreamWriter(path);
        }

        public void write(string cat)
        {
            file.WriteLine(cat);
        }

        public void close()
        {
            file.Close();
        }
    }
}
