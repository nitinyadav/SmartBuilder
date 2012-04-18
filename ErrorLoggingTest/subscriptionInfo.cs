using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ErrorLoggingTest
{
    [DataContract]
    public class subscriptionInfo
    {
        DateTime dop;    //date of purchase
        DateTime doe;    //date of expiry
        int subscriptionDays;
        string paymentMode;

        [DataMember]
        public DateTime Dop
        {
            get { return dop; }
            set { dop = value; }
        }

        [DataMember]
        public DateTime Doe
        {
            get { return doe; }
            set { doe = value; }
        }

        [DataMember]
        public int SubscriptionDays
        {
            get { return subscriptionDays; }
            set { subscriptionDays = value; }
        }

        [DataMember]
        public string PaymentMode
        {
            get { return paymentMode; }
            set { paymentMode = value; }
        }

    }
}
