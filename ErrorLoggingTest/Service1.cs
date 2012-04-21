using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using StackApi;
using System.IO;
using MySql.Data.MySqlClient;
using SearchEngine;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Specialized;

namespace ErrorLoggingTest
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        logger log;
        public static Catalog my_catalog= new Catalog();
        public static int MAX_RESULTS = 10;

        public clientInfo GetUserDetail(string userId)
        {
            if (userId == null || userId == "")
            {
                throw new ArgumentNullException("Request Object not initialized !!");
            }
            clientInfo var = new clientInfo(userId);
            return var;
        }

        public responseInfo GetAnswer(bugData input)
        {
            if (input == null)
            {
                //throw new ArgumentNullException("Request Object not initialized !!");
                return null;
            }
            log = new logger();
            apiFunctions func = new apiFunctions();

            if (input.BugId == "")
            {
                input.BugId = log.logBugData(input);
                log.log("Log called");
                responseInfo var = new responseInfo(input);

                //SEARCH: search local DB
                string searchterm = input.ErrorMessage;

                string [] searchTermA = null;
    
                Regex r = new Regex(@"\s+");            //remove all whitespace
                searchterm = r.Replace(searchterm, " ");// to a single space
                searchTermA = searchterm.Split(' ');// then split
                for (int i = 0; i < searchTermA.Length; i++) {
                    searchTermA[i] = searchTermA[i].Trim(' ', '?','\"', ',', '\'', ';', ':', '.', '(', ')').ToLower();
                }
    
                // Array of arrays of results that match ONE of the search criteria
				Hashtable[] searchResultsArrayArray = new Hashtable[searchTermA.Length];
				// finalResultsArray is populated with pages that *match* ALL the search criteria
				Hashtable finalResultsArray = new Hashtable();
				    
                bool botherToFindMatches = true;
				int indexOfShortestResultSet = -1, lengthOfShortestResultSet = -1;
    
                for (int i = 0; i < searchTermA.Length; i++) {
                    searchResultsArrayArray[i] = my_catalog.Search (searchTermA[i].ToString());
                    if (null == searchResultsArrayArray[i]) {
                        //botherToFindMatches = false;
                    } else {
                        int resultsInThisSet = searchResultsArrayArray[i].Count;
                        if ( (lengthOfShortestResultSet == -1) || (lengthOfShortestResultSet > resultsInThisSet) ) {
                            indexOfShortestResultSet  = i;
                            lengthOfShortestResultSet = resultsInThisSet;
                        }
                    }
                }
                
                // Find the common files from the array of arrays of documents
                // matching ONE of the criteria
                if (botherToFindMatches) {
                    int c = indexOfShortestResultSet;                               // loop through the *shortest* resultset
                    Hashtable searchResultsArray = searchResultsArrayArray[c];
    
                    if (null != searchResultsArray)
                    foreach (object foundInFile in searchResultsArray) {            // for each file in the *shortest* result set
                        DictionaryEntry fo = (DictionaryEntry)foundInFile;          // find matching files in the other resultsets
    
                        int matchcount=0, totalcount=0, weight=0;
    
                        for (int cx = 0; cx < searchResultsArrayArray.Length; cx++) {
                            totalcount+=(cx+1);                                // keep track, so we can compare at the end (if term is in ALL resultsets)
                            if (cx == c) {                                     // current resultset
                                matchcount += (cx+1);                          // implicitly matches in the current resultset
                                weight += (int)fo.Value;                       // sum the weighting
                            } else {
                                Hashtable searchResultsArrayx = searchResultsArrayArray[cx];
                                if (null != searchResultsArrayx)
                                foreach (object foundInFilex in searchResultsArrayx) {        // for each file in the result set
                                    DictionaryEntry fox = (DictionaryEntry)foundInFilex;
                                    if (fo.Key.ToString() == fox.Key.ToString()) {                                  // see if it matches
                                        matchcount += (cx+1);                  // and if it matches, track the matchcount
                                        weight += (int)fox.Value;               // and weighting; then break out of loop, since
                                        break;                                 // no need to keep looking through this resultset
                                    }
                                } // foreach
                            } // if
                        } // for
                        if ( (matchcount>0) && (matchcount == totalcount) ) { // was matched in each Array
                            // we build the finalResults here, to pass to the formatting code below
                            // - we could do the formatting here, but it would mix up the 'result generation'
                            // and display code too much
                            fo.Value = weight; // set the 'weight' in the combined results to the sum of individual document matches
                            if ( !finalResultsArray.Contains (fo.Key) ) finalResultsArray.Add ( fo.Key, fo.Value);
                        } // if
                    } // foreach
                } // if

                // Format the results
                if (finalResultsArray.Count > 0)
                {
                    SortedList output = new SortedList(finalResultsArray.Count);
                    DictionaryEntry fo;
                    string result = "";
                    string bugId = "";

                    foreach (object foundInFile in finalResultsArray)
                    {
                        fo = (DictionaryEntry)foundInFile;
                        bugId = (String)fo.Key;
                        int rank = (int)fo.Value;
                        result = bugId;
                        //extract info from bugId
                        int sortrank = (rank * -1);
                        if (output.Contains(sortrank))
                        {
                            output[sortrank] = ((string)output[sortrank]) + ":" + result;
                        }
                        else
                        {
                            output.Add(sortrank, result);
                        }
                        result = "";
                    }
                //}

                //searchterm = searchterm.Trim(' ', '?', '\"', ',', '\'', ';', ':', '.', '(', ')').ToLower();
                //Hashtable searchResultsArray = my_catalog.Search(searchterm);

                //SortedList output;
                //if (null != searchResultsArray)
                //{
                    //output = new SortedList(searchResultsArray.Count);
                    //DictionaryEntry fo;
                    //string result = "";
                    //string bugId = "";
                    //foreach (object foundInFile in searchResultsArray)
                    //{
                    //    fo = (DictionaryEntry)foundInFile;
                    //    bugId = (String)fo.Key;
                    //    int rank = (int)fo.Value;
                    //    result = bugId;
                    //    //extract info from bugId
                    //    int sortrank = (rank * -1);
                    //    if (output.Contains(sortrank))
                    //    {
                    //        output[sortrank] = ((string)output[sortrank]) +":"+ result;
                    //    }
                    //    else
                    //    {
                    //        output.Add(sortrank, result);
                    //    }
                    //    result = "";
                    //}

                    //saves 10 result instances and applies the similarity metrics on these
                    int resCount = 0;
                    string[] bId = new string[100];
                    foreach (object rows in output.Keys)
                    {
                        bId[resCount] = (string)output[rows];
                        if (bId[resCount].Contains(':'))
                        {
                            string[] temp = bId[resCount].Split(':');
                            foreach (string s in temp)
                            {
                                bId[resCount] = s;
                                resCount++;
                                if (resCount >= MAX_RESULTS)
                                    break;
                            }
                        }
                        else
                        {
                            resCount++;
                        }
                        if (resCount >= MAX_RESULTS)
                            break;
                    }
                    suggestAnswer[] suggestions = new suggestAnswer[resCount];
                    for (int f = 0; f < resCount; f++)
                    {
                        suggestions[f] = this.extractRowSQL(bId[f]);
                    }
                    
                    //rank results
                    responseInfo response = SolutionRank.sortDBResults(input,suggestions);
                    return response;

                    //TESTING
                    //extract first object from sql with highest rank
                    /*string bugID="";
                    foreach (object o in output.Keys)
                    {
                        bugID = (string)output[o];
                        if (bugID.Contains(':'))
                        {
                            int pos = bugID.IndexOf(':');
                            bugID = bugID.Substring(0,pos);
                        }
                        return this.extractRowSQL(bugID);
                    }
                    return null;*/
                }
                else
                {
                    //No result in local DB so search developer network of stakoverflow

                    //call the stackapi to get the response
                    func.similar(input.ErrorMessage, input.Tags);
                    func.rootObject.bugId = input.BugId;
                    
                    //sort the answers based on the question similarity
                    func.rootObject = SolutionRank.sortOnlineAnswers(func.rootObject, input.ErrorMessage);

                    log.logAnswer(func.rootObject);
                    //no of solutions proposed
                    int count = 0;

                    //take the solution with haighest rank ie 0th element
                    Question question = func.rootObject.questions[0];
                    log.log("Question: " + func.cleanText(question.body));
                    
                    var.Question = func.cleanText(question.body);
                    var.Solution = new string[question.answer_count];
                    var.Vote = new int[question.answer_count];
                    try
                    {
                        foreach (Answer answer in question.answers)
                        {
                            var.Solution[count] = func.cleanText(answer.body);
                            var.Vote[count] = answer.score;
                            log.log("Answer :" + var.Solution[count]);
                            count++;
                        }
                    }
                    catch (Exception e) { System.Console.WriteLine(e.Message); }
                    var.BugId = input.BugId;
                    return var;
                }
            }
            else
            {
                StackApi.RootObject solution = log.retrievStored(input.BugId);
                responseInfo var = new responseInfo();
                if (input.MoreOptions < solution.questions.Length)
                {
                    StackApi.Question question = solution.questions[input.MoreOptions];
                    log.log("Question in mem: " + func.cleanText(question.body));
                    var.Question = func.cleanText(question.body);
                    var.Solution = new string[question.answer_count];
                    var.Vote = new int[question.answer_count];
                    int count = 0;
                    try
                    {
                        foreach (Answer answer in question.answers)
                        {
                            var.Solution[count] = func.cleanText(answer.body);
                            var.Vote[count] = 0;
                            log.log("Answer in mem:" + var.Solution[count]);
                            count++;
                        }
                    }
                    catch (Exception e) { System.Console.WriteLine(e.Message); }
                    var.BugId = input.BugId;
                    return var;
                }
                else
                {
                    return null;
                }
            }
        }

        public void updateAnswer(suggestAnswer ans)
        {
            //add to the databse from the user
            string bId = writeAnswertoSQL(ans);
            string[] bugId = bId.Split(':');
            
            //bugId can be added to the search engine
            Regex r = new Regex(@"\s+");           // remove all whitespace
            string wordsOnly = r.Replace(ans.ErrorMessage, " "); // compress all whitespace to one space
            string[] wordsOnlyA = wordsOnly.Split(' ');

            // Loop through words in string
            string key = "";
            foreach (string word in wordsOnlyA)
            {
                foreach (string s in bugId)
                {
                    if (s.Length > 0)
                    {
                        key = word.Trim(' ', '?', '\"', ',', '\'', ';', ':', '.', '(', ')').ToLower();
                        my_catalog.Add(key, s);
                    }
                }
            }
        }

        string writeAnswertoSQL(suggestAnswer ans)
        {
            string MyConString = "SERVER=localhost;DATABASE=test;UID=root;PASSWORD=nitin;";
            MySqlConnection connection = new MySqlConnection(MyConString);
            MySqlCommand command = connection.CreateCommand();

            MySqlCommand mycommand = connection.CreateCommand();
            string errorMsg = ans.ErrorMessage;
            string answer = ans.Answer;
            string userId = ans.UserId;
            int votes = ans.Vote;
            string filename = ans.Filename;
            string stacktrace = ans.Stacktrace;
            string nameSpace = ans.NameSpace;
            string os = ans.Os;
            string softwareName = ans.SoftwareName;
            string tags = ans.Tags;
            string guid = ans.Guid;
            string version = ans.Version;
            string info = ans.Info;
            string vendor = ans.Vendor;

            string sqlQuery = "INSERT INTO bugdata (question";
            string sqlData = ") "+ "Values ('" + errorMsg ;
            string sqlEnd = "')";

            if (answer.Length>1)
            {
                sqlQuery += ",answer";
                sqlData += "','"+answer;
            }
            if (userId.Length > 1)
            {
                sqlQuery += ",userId";
                sqlData += "','" + userId;
            }
            if (filename.Length > 1)
            {
                sqlQuery += ",filename";
                sqlData += "','" + filename;
            }
            if (stacktrace.Length > 1)
            {
                sqlQuery += ",stacktrace";
                sqlData += "','" + stacktrace;
            }
            if (votes >= 0 )
            {
                sqlQuery += ",votes";
                sqlData += "','" + votes;
            }
            if (nameSpace.Length > 1)
            {
                sqlQuery += ",namespace";
                sqlData += "','" + nameSpace;
            }
            if (os.Length > 1)
            {
                sqlQuery += ",os";
                sqlData += "','" + os;
            }
            if (softwareName.Length > 1)
            {
                sqlQuery += ",softwareName";
                sqlData += "','" + softwareName;
            }
            if (tags.Length > 1)
            {
                sqlQuery += ",tags";
                sqlData += "','" + tags;
            }
            if (guid.Length > 1)
            {
                sqlQuery += ",guid";
                sqlData += "','" + guid;
            }
            if (version.Length > 1)
            {
                sqlQuery += ",version";
                sqlData += "','" + version;
            }
            if (info.Length > 1)
            {
                sqlQuery += ",info";
                sqlData += "','" + info;
            }
            if (vendor.Length > 1)
            {
                sqlQuery += ",vendor";
                sqlData += "','" + vendor;
            }
            
            mycommand.CommandText = sqlQuery + sqlData + sqlEnd;
            connection.Open();
            mycommand.ExecuteNonQuery();

            MySqlDataReader Reader;
            command.CommandText = "select BugId from bugdata where question ='" + errorMsg + "'";
            Reader = command.ExecuteReader();
            string bugId = "";
            while (Reader.Read())
            {
                bugId += Reader.GetValue(0).ToString()+":";
            }
            connection.Close();
            return bugId;
        }

        suggestAnswer extractRowSQL(string bugId)
        {
            if (bugId == null)
                return null;
            suggestAnswer ans = new suggestAnswer();

            string MyConString = "SERVER=localhost;DATABASE=test;UID=root;PASSWORD=nitin;";
            MySqlConnection connection = new MySqlConnection(MyConString);
            MySqlCommand command = connection.CreateCommand();

            MySqlCommand mycommand = connection.CreateCommand();
            connection.Open();
            
            MySqlDataReader Reader;
            command.CommandText = "Select Question, Answer, BugId, UserId, Votes, stacktrace, Namespace, os, tags, software, guid, version, info, vendor from bugdata where BugId =" + bugId + "";
            Reader = command.ExecuteReader();
            if(Reader.Read())
            {
                ans.ErrorMessage = Reader[0].ToString();
                ans.Answer = Reader[1].ToString();
                ans.BugId = Reader.GetInt32(2);
                ans.UserId = Reader[3].ToString();
                ans.Vote = Reader.GetInt32(4);
                ans.Stacktrace = Reader[5].ToString();
                ans.NameSpace = Reader[6].ToString();
                ans.Os = Reader[7].ToString();
                ans.Tags = Reader[8].ToString();
                ans.SoftwareName = Reader[9].ToString();
                ans.Guid = Reader[10].ToString();
                ans.Version = Reader[11].ToString();
                ans.Info = Reader[12].ToString();
                ans.Vendor = Reader[13].ToString();
            }
            connection.Close();
            
            return ans;
        }
    }
}
