using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ErrorLoggingTest
{
    [ServiceContract]
    public interface IService1
    {
        
        [OperationContract]
        [WebGet(UriTemplate="/GetUserDetail", RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
        clientInfo GetUserDetail(string userId);

        [OperationContract]
        [WebGet(UriTemplate = "/GetAnswer", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        responseInfo GetAnswer(bugData input);

        [OperationContract]
        [WebGet(UriTemplate = "/UpdateAnswer", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void updateAnswer(correctAnswer response);

    }
}
