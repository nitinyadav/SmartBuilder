using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using StackApi;

namespace ErrorLoggingTest
{
    /// <summary>
    /// To compute the ranking of the questions obtained
    /// </summary>
    class SolutionRank
    {
        /// <summary>
        /// Sorts the questions
        /// </summary>
        /// <param name="rObj">The root object</param>
        /// <param name="errorMsg">error message string</param>
        /// <returns></returns>
        public static RootObject sortAnswers(RootObject rObj, string errorMsg)
        {
            Hashtable ht = new Hashtable();
            char[] separator = new char[] { ' ', ',', '.', ':', ';','-'};
            string[] tokens = errorMsg.Split(separator);
            //List<String> token = errorMsg.Split(separator);
            List<String> error = new List<String>();

            foreach (string st in tokens)
            {
                ht.Add(st, true);
                error.Add(st);
            }
            error = removeExtra(error);

            int[] rank = new int[rObj.questions.Length];
            int count = 0;
            apiFunctions text= new apiFunctions();
            foreach (Question q in rObj.questions)
            {
                q.body = text.cleanText(q.body);
                // compare q.body and error
                if (q.body.Contains(errorMsg))
                {
                    rank[count] = 1000;
                    continue;
                }
                string[] qtoken = q.body.Split(separator);
                foreach(string str in qtoken)
                {
                    if (error.Contains(str))
                    {
                        rank[count]++;
                        error.Remove(str);
                    }
                }
                count++;
            }
            //sort on the basis of rank
            sortQuestions(rank, rObj.questions);

            return rObj;
        }

        /// <summary>
        /// Sorting based on rank
        /// </summary>
        /// <param name="rank"></param>
        /// <param name="qObj"></param>
        public static void sortQuestions(int[] rank,Question[] qObj)
        {
            int element;
            int ic;
            Question tmp = new Question();
            
            for (int loopvar = 1; loopvar < rank.Length; loopvar++)
            {
                element = rank[loopvar];
                tmp = qObj[loopvar];
                ic = loopvar - 1;
                while (ic >= 0 && rank[ic] < element)
                {
                    rank[ic + 1] = rank[ic];    //move all elements
                    qObj[ic + 1] = qObj[ic];
                    ic--;
                }
                rank[ic + 1] = element;    //Insert element            
                qObj[ic + 1] = tmp;
            }
        }

        /// <summary>
        /// Removes is, an , the from the error string
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static List<String> removeExtra(List<String> error)
        {
            string[] extra = new string[] { "","a","an","the","is","are","was","has","here","\n","\r","\t"};
            foreach (string current in error.ToList())
            {
                foreach (string rem in extra)
                    if (rem.Equals(current))
                        error.Remove(current);
            }
            return error;
        }
    }
}
