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
        public static RootObject sortOnlineAnswers(RootObject rObj, string errorMsg)
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

        /// <summary>
        /// Rank the suggestions on similarity with the input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="suggestions"></param>
        /// <returns></returns>
        public static responseInfo sortDBResults(bugData input, suggestAnswer[] suggestions)
        {
            double[] rank = new double[suggestions.Length];
            int sugCount = 0;
            //int answerPosition = -1;
            //double lowestRank = -1;
            foreach (suggestAnswer ans in suggestions)
            {
                rank[sugCount] = 0;
                rank[sugCount] += cosineSimilarity.CosineIDFSimilarity(input.ErrorMessage, ans.ErrorMessage);
                if ((input.StackTrace != null) && (ans.Stacktrace != null))
                    rank[sugCount] += cosineSimilarity.CosineIDFSimilarity(input.StackTrace, ans.Stacktrace);
                if ((input.Tags != null) && (ans.Tags != null))
                    rank[sugCount] += cosineSimilarity.CosineSimilarity(input.Tags, ans.Tags);
                if ((input.SoftwareInfo != null) && (ans.Info != null))
                    rank[sugCount] += cosineSimilarity.CosineSimilarity(input.SoftwareInfo,ans.Info);

                if ((input.Filename != null) && (ans.Filename != null))
                    if(input.Filename.Equals(ans.Filename))
                        rank[sugCount] += 1;
                if ((input.NameSpace != null) && (ans.NameSpace != null))
                    if(input.NameSpace.Equals(ans.NameSpace))
                        rank[sugCount] += 1;
                if ((input.SoftwareName != null) && (ans.SoftwareName != null))
                    if(input.SoftwareName.Equals(ans.SoftwareName))
                        rank[sugCount] += 1;
                if ((input.Version != null) && (ans.Version != null))
                    if(input.Version.Equals(ans.Version))
                        rank[sugCount] += 1;
                if ((input.Vendor != null) && (ans.Vendor != null))
                    if(input.Vendor.Equals(ans.Vendor))
                        rank[sugCount] += 1;
                if ((input.OperatingSystem != null) && (ans.Os != null))
                    if(input.OperatingSystem.Equals(ans.Os))
                        rank[sugCount] += 1;
                if ((input.Guid != null) && (ans.Guid != null))
                    if(input.Guid.Equals(ans.Guid))
                        rank[sugCount] += 1;
                //if ((lowestRank == -1) || (rank[sugCount] > lowestRank))
                //{
                //    lowestRank = rank[sugCount];
                //    answerPosition = sugCount;
                //}
                sugCount++;
            }
            sortSuggestions(rank,suggestions);
            responseInfo res = new responseInfo();
            //make res from suggestions[answerPosition]
            res.Error = suggestions[0].ErrorMessage;
            res.Question = suggestions[0].ErrorMessage;
            res.UserId = suggestions[0].UserId;
            int totalAns = (suggestions.Length > 3) ? 3 : suggestions.Length;
            res.Solution = new string[totalAns];
            res.Vote = new int[totalAns];
            for (int u = 0; u < totalAns; u++)
            {
                res.Solution[u] = suggestions[u].Answer;
                res.Vote[u] = suggestions[u].Vote;
            }
            return res;
        }

        /// <summary>
        /// Sorting based on rank
        /// </summary>
        /// <param name="rank"></param>
        /// <param name="qObj"></param>
        public static void sortSuggestions(double[] rank, suggestAnswer[] sAns)
        {
            double element;
            int ic;
            suggestAnswer tmp = new suggestAnswer();

            for (int loopvar = 1; loopvar < rank.Length; loopvar++)
            {
                element = rank[loopvar];
                tmp = sAns[loopvar];
                ic = loopvar - 1;
                while (ic >= 0 && rank[ic] < element)
                {
                    rank[ic + 1] = rank[ic];    //move all elements
                    sAns[ic + 1] = sAns[ic];
                    ic--;
                }
                rank[ic + 1] = element;    //Insert element            
                sAns[ic + 1] = tmp;
            }
        }
    }
}
