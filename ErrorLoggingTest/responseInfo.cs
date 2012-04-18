using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackApi;

namespace ErrorLoggingTest
{
    [DataContract]
    public class responseInfo
    {
        string bugId;
        string userId;
        string error;
        string question;
        string[] solution;
        int[] vote;
        
        public responseInfo()
        {
            bugId = "bug1";
            userId = "user1";
            error = "file not found";
        }

        public responseInfo(bugData input)
        {
            userId = input.UserId;
            bugId = input.BugId;
            error = input.ErrorMessage;

        }

        private static HttpWebRequest GetWebRequest(string formattedUri)
        {
            // Create the request’s URI.
            Uri serviceUri = new Uri(formattedUri, UriKind.Absolute);

            // Return the HttpWebRequest.
            return (HttpWebRequest)System.Net.WebRequest.Create(serviceUri);
        }


        [DataMember]
        public string BugId
        {
            get { return bugId; }
            set { bugId = value; }
        }

        [DataMember]
        public string UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        [DataMember]
        public string Error
        {
            get { return error; }
            set { error = value; }
        }

        [DataMember]
        public string[] Solution
        {
            get { return solution; }
            set { solution = value; }
        }

        [DataMember]
        public int[] Vote
        {
            get { return vote; }
            set { vote = value; }
        }

        [DataMember]
        public string Question
        {
            get { return question; }
            set { question = value; }
        }
    }
}
