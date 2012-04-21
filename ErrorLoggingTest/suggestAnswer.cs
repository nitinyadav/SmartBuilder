using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;


namespace ErrorLoggingTest
{
    [DataContract]
    public class suggestAnswer
    {
        string errorMessage;
        string answer;
        string filename;
        string stacktrace;
        string tags;
        string userId;
        string info;
        int vote;
        string nameSpace;
        string softwareName;
        string version;
        string vendor;
        string os;
        string guid;
        int bugId;

        public suggestAnswer()
        {
            errorMessage = "";
            answer = "";
            filename = "";
            stacktrace = "";
            tags = "";
            userId = "";
            info = "";
            vote = 0;
            nameSpace = "";
            softwareName = "";
            version = "";
            vendor = "";
            os = "";
            guid = "";
            bugId = -1;
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

        [DataMember]
        public int BugId
        {
            get { return bugId; }
            set { bugId = value; }
        }

        [DataMember]
        public string NameSpace
        {
            get { return nameSpace; }
            set { nameSpace = value; }
        }
        [DataMember]
        public string SoftwareName
        {
            get { return softwareName; }
            set { softwareName = value; }
        }
        [DataMember]
        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        [DataMember]
        public string Os
        {
            get { return os; }
            set { os = value; }
        }
        [DataMember]
        public string Guid
        {
            get { return guid; }
            set { guid = value; }
        }
        [DataMember]
        public string Vendor
        {
            get { return vendor; }
            set { vendor = value; }
        }
    }
}
