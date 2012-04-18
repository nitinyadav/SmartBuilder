using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;


namespace ErrorLoggingTest
{
    [DataContract]
    public class correctAnswer
    {
        string question;
        string errorMessage;
        string answer;
        string filename;
        string stacktrace;
        string tags;
        string userId;
        string info;
        int vote;
        
        [DataMember]
        public string Question
        {
            get { return question; }
            set { question = value; }
        }

        [DataMember]
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }

        [DataMember]
        public string Answer
        {
            get { return answer; }
            set { answer = value; }
        }

        [DataMember]
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        [DataMember]
        public string Stacktrace
        {
            get { return stacktrace; }
            set { stacktrace = value; }
        }
        
        [DataMember]
        public string Tags
        {
            get { return tags; }
            set { tags = value; }
        }

        [DataMember]
        public string UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        [DataMember]
        public string Info
        {
            get { return info; }
            set { info = value; }
        }

        [DataMember]
        public int Vote
        {
            get { return vote; }
            set { vote = value; }
        }
    }
}
