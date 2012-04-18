using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ErrorLoggingTest
{
    [DataContract]
    public class clientInfo
    {
        string userName;
        string userId;
        string token;
        DateTime dob;
        string sex;
        string contactNo;
        string address;

        public clientInfo(string userId)
        {
            userName = "user1";
            token = "abcd";
            dob = DateTime.Today;
            sex = "male";
            contactNo = "+919936441656";
            address = "abcd colony, india";
        }
        [DataMember]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
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
        public DateTime Dob
        {
            get { return dob; }
            set { dob = value; }
        }

        [DataMember]
        public string Sex
        {
            get { return sex; }
            set { sex = value; }
        }

        [DataMember]
        public string ContactNo
        {
            get { return contactNo; }
            set { contactNo = value; }
        }

        [DataMember]
        public string Address
        {
            get { return address; }
            set { address = value; }
        }

    }
}
