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

namespace ErrorLoggingTest
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        logger log;
        Catalog my_catalog;

        public Service1()
        {
            my_catalog = new Catalog();
        }

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
                searchterm = searchterm.Trim(' ', '?', '\"', ',', '\'', ';', ':', '.', '(', ')').ToLower();
                Hashtable searchResultsArray = my_catalog.Search(searchterm);

                SortedList output;
                if (null != searchResultsArray)
                {
                    output = new SortedList(searchResultsArray.Count);
                    DictionaryEntry fo;
                    string result = "";
                    string bugId = "";
                    foreach (object foundInFile in searchResultsArray)
                    {
                        fo = (DictionaryEntry)foundInFile;
                        bugId = (String)fo.Key;
                        int rank = (int)fo.Value;
                        result = bugId;
                        //extract info from bugId
                        int sortrank = (rank * -1);
                        if (output.Contains(sortrank))
                        {
                            output[sortrank] = ((string)output[sortrank]) +":"+ result;
                        }
                        else
                        {
                            output.Add(sortrank, result);
                        }
                        result = "";
                    }

                    /*foreach (object rows in output)
                    {
                        string bId = (string)((DictionaryEntry)rows).Value;
                    }*/
                    // total results = searchResultsArray.Count

                    //extract object from sql with bugId as bid
                    string bugID = (string)((DictionaryEntry)output[0]).Value;
                    return this.extractRowSQL(bugID);
                }
                else
                {
                    //No result in local DB so search developer network of stakoverflow

                    //call the stackapi to get the response
                    func.similar(input.ErrorMessage, input.Tags);
                    func.rootObject.bugId = input.BugId;
                    
                    //sort the answers based on the question similarity
                    func.rootObject = SolutionRank.sortAnswers(func.rootObject, input.ErrorMessage);

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

        public void updateAnswer(correctAnswer ans)
        {
            //add to the databse from the user
            string bugId = writeAnswertoSQL(ans);
            
            //bugId can be added to the search engine
            Regex r = new Regex(@"\s+");           // remove all whitespace
            string wordsOnly = r.Replace(ans.Question, " "); // compress all whitespace to one space
            string[] wordsOnlyA = wordsOnly.Split(' ');

            // Loop through words in string
            int i = 0;
            string key = "";
            foreach (string word in wordsOnlyA)
            {
                key = word.Trim(' ', '?', '\"', ',', '\'', ';', ':', '.', '(', ')').ToLower();
                my_catalog.Add(key, bugId);
                i++;
            }
        }

        string writeAnswertoSQL(correctAnswer ans)
        {
            string MyConString = "SERVER=localhost;DATABASE=test;UID=root;PASSWORD=nitin;";
            MySqlConnection connection = new MySqlConnection(MyConString);
            MySqlCommand command = connection.CreateCommand();

            MySqlCommand mycommand = connection.CreateCommand();
            string question = ans.Question;
            string errorMsg = ans.ErrorMessage;
            string answer = ans.Answer;
            string userId = ans.UserId;
            int vote = ans.Vote;
            string filename = ans.Filename;
            string info = ans.Info;
            string stacktrace = ans.Stacktrace;

            string sqlQuery = "INSERT INTO bugdata (question";
            string sqlData = ") "+ "Values ('" + errorMsg ;
            string sqlEnd = "')";

            if (answer.Length>1)
            {
                sqlQuery += "answer";
                sqlData += "','"+answer;
            }
            if (userId.Length > 1)
            {
                sqlQuery += "userId";
                sqlData += "','" + userId;
            }
            if (filename.Length > 1)
            {
                sqlQuery += "filename";
                sqlData += "','" + filename;
            }
            if (stacktrace.Length > 1)
            {
                sqlQuery += "stacktrace";
                sqlData += "','" + stacktrace;
            }
            if (vote !=null)
            {
                sqlQuery += "votes";
                sqlData += "','" + vote;
            }
            //mycommand.CommandText = @"INSERT INTO bugdata (question, answer,filename,info) "
            //                    + "Values ('" + question + "','" + answer + "','" + filename + "','" + info + "')";
            mycommand.CommandText = sqlQuery + sqlData + sqlEnd;
            connection.Open();
            mycommand.ExecuteNonQuery();

            MySqlDataReader Reader;
            command.CommandText = @"select BugId from bugdata where question ='" + question + "'";
            Reader = command.ExecuteReader();
            string bugId = "";
            while (Reader.Read())
            {
                bugId += Reader.GetValue(0).ToString();
            }
            connection.Close();
            return bugId;
        }

        responseInfo extractRowSQL(string bugId)
        {
            responseInfo ans = new responseInfo();

            string MyConString = "SERVER=localhost;DATABASE=test;UID=root;PASSWORD=nitin;";
            MySqlConnection connection = new MySqlConnection(MyConString);
            MySqlCommand command = connection.CreateCommand();

            MySqlCommand mycommand = connection.CreateCommand();
            connection.Open();
            
            MySqlDataReader Reader;
            command.CommandText = @"select * from bugdata where BugId ='" + bugId + "'";
            Reader = command.ExecuteReader();
            if(Reader.Read())
            {
                ans.Question = Reader["Question"].ToString();
                ans.Solution[0] = Reader["Answer"].ToString();
                ans.BugId = Reader["BugId"].ToString();
                ans.UserId = Reader["UserId"].ToString();
                ans.Vote[0] = int.Parse(Reader["Votes"].ToString());
            }
            connection.Close();
            
            return ans;
        }
    }
}
