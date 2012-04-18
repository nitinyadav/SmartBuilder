using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace SearchEngine
{
    /// <summary>Catalog of words and pages<summary>
    public class Catalog
    {
        /// <summary>Internal datastore of Words referencing Files</summary>
        private System.Collections.Hashtable index;	//TODO: implement collection with faster searching

        public int Length
        {
            get { return index.Count; }
        }
        /// <summary>Constructor</summary>
        public Catalog()
        {
            index = new System.Collections.Hashtable();
            string path = "orm.txt";
            // This text is added only once to the file.
            if (File.Exists(path))
            {
                System.IO.StreamReader file = new System.IO.StreamReader("orm.txt");
                string line = "";
                string[] temp = new string[2] { "", "" };
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Length > 1)
                    {
                        temp = line.Split(':');
                        Add(temp[0], temp[1]);
                    }
                }
                file.Close();
            }
            
        }

        ~Catalog()
        {
            writer Writer = new writer("orm.txt");
            Writer.write(this.ToString());
            Writer.close();
        }

        /// <summary>
        /// Add a new Word/BugId pair to the Catalog
        /// </summary>
        public bool Add(string word, string BugId)
        {
            // ### Make sure the Word object is in the index ONCE only
            if (index.ContainsKey(word))
            {
                Word theword = (Word)index[word];	// add this file reference to the Word
                theword.Add(BugId);
            }
            else
            {
                Word theword = new Word(word, BugId);	// create a new Word object
                index.Add(word, theword);
            }
            return true;
        }

        /// <summary>Returns all the Files which contain the searchWord</summary>
        /// <returns>Hashtable </returns>
        public Hashtable Search(string searchWord)
        {
            // apply the same 'trim' as when we're building the catalog
            searchWord = searchWord.Trim('?', '\"', ',', '\'', ';', ':', '.', '(', ')').ToLower();
            Hashtable retval = null;
            if (index.ContainsKey(searchWord))
            {
                Word thematch = (Word)index[searchWord];
                retval = thematch.InBugId(); // return the collection of File objects
            }
            return retval;
        }

        /// <summary>Debug string</summary>
        public override string ToString()
        {
            string temp= "";
            foreach (object w in index.Keys) temp += ((Word)w).ToString();	// output ALL words, will take a long time
            return temp;
        }
    }

    /// <summary>Instance of a word<summary>
    public class Word
    {
        /// <summary>The cataloged word</summary>
        public string Text;
        /// <summary>Collection of bugs the word appears in</summary>
        private System.Collections.Hashtable BugCollection = new System.Collections.Hashtable();
        /// <summary>Constructor with first file reference</summary>
        public Word(string text, string BugId)
        {
            Text = text;
            BugCollection.Add(BugId, 1);
        }
        /// <summary>Add a file referencing this word</summary>
        public void Add(string BugId)
        {
            if (BugCollection.ContainsKey(BugId))
            {
                int wordcount = (int)BugCollection[BugId];
                BugCollection[BugId] = wordcount + 1;
            }
            else
            {
                BugCollection.Add(BugId, 1);
            }
        }
        /// <summary>Collection of files containing this Word (Value=WordCount)</summary>
        public Hashtable InBugId()
        {
            return BugCollection;
        }
        /// <summary>Debug string</summary>
        public override string ToString()
        {
            string temp = "";
            foreach (object tempFile in BugCollection.Values) temp += Text+":"+((Bug)tempFile).ToString()+"\n";
            return temp;
        }
    }


    /// <summary>File attributes</summary>
    /// <remarks>Beware ambiguity with System.IO.File - always fully qualify File object references</remarks>
    public class Bug
    {
        public string BugId;
        
        /// <summary>Constructor requires all File attributes</summary>
        public Bug(string bugId)
        {
            BugId = bugId;
        }
        /// <summary>Debug string</summary>
        public override string ToString()
        {
            return BugId;
        }
    }
}
