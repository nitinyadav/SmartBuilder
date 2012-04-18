using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Microsoft.UCG.Test.LWA
{
    class XMLDatatype
    {
        private bool myHeaderWritten = false;
        private bool myEndTagWritten = false;
        private string myFilename;
        private FileWriter myWriter = null;

        public bool headerWritten
        {
            get { return myHeaderWritten; }
            set { myHeaderWritten = value; }
        }

        public bool endTagWritten
        {
            get { return myEndTagWritten; }
            set { myEndTagWritten = value; }
        }

        public string filename
        {
            get { return myFilename; }
            set { myFilename = value; }
        }

        public FileWriter _writer
        {
            get { return myWriter; }
            set { myWriter = value; }
        }
    }
}
