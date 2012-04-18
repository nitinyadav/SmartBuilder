using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Microsoft.UCG.Test.LWA
{
    /// <summary>
    /// The IloggerInterface presents an interface to process the logData.
    /// </summary>
    public interface IloggerExtension
    {
        /// <summary>
        /// This function consumes the data received. It processes the data and the runId of the received content.
        /// </summary>
        /// <param name="RunId"></param>
        /// <param name="data"></param>
        void consume(string RunId, string data);


        /// <summary>
        /// It defines the interface to perform the action after the test case is complete. Its like a closing 
        /// signal to the server from the client.
        /// </summary>
        /// <param name="RunId"></param>
        void actionOnCompletion(string RunId);

        /// <summary>
        /// Its used to open a new service and create a new instance in the LoggerExtensions.
        /// </summary>
        /// <param name="name"></param>
        void open(string name);
    }
}
