using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ErrorLoggingTest
{
    [Serializable]
    [DataContract]
    public class bugData
    {
        string softwareName;
        string vendor;
        string version;
        string softwareInfo;
        string filename;
        string stackTrace;
        string errorMessage;
        string outputText;
        int moreOption;
        string bugId;
        string userId;
        string guid;
        string nameSpace;
        string token;
        string tags;
        string operatingSystem;

        [DataMember]
        public string SoftwareName
        {
            get { return softwareName; }
            set { softwareName = value; }
        }

        [DataMember]
        public string Vendor
        {
            get { return vendor; }
            set { vendor = value; }
        }

        [DataMember]
        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        [DataMember]
        public string SoftwareInfo
        {
            get { return softwareInfo; }
            set { softwareInfo = value; }
        }

        [DataMember]
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        [DataMember]
        public string StackTrace
        {
            get { return stackTrace; }
            set { stackTrace = value; }
        }

        [DataMember]
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }

        [DataMember]
        public string OutputText
        {
            get { return outputText; }
            set { outputText = value; }
        }

        [DataMember]
        public int MoreOptions
        {
            get { return moreOption; }
            set { moreOption = value; }
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
        public string Token
        {
            get { return token; }
            set { token = value; }
        }

        [DataMember]
        public string Tags
        {
            get { return tags; }
            set { tags = value; }
        }

        [DataMember]
        public string Guid
        {
            get { return guid; }
            set { guid = value; }
        }

        [DataMember]
        public string NameSpace
        {
            get { return nameSpace; }
            set { nameSpace = value; }
        }

        [DataMember]
        public string OperatingSystem
        {
            get { return operatingSystem; }
            set { operatingSystem = value; }
        }
    }
}
