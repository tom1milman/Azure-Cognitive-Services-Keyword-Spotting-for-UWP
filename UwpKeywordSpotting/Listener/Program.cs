using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Listener
{
    public class Program
    {
        public static CognitiveServicesUtilities cognitiveServicesUtils;
        public static ConnectionUtilities connectionUtils;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting: ");
            cognitiveServicesUtils = new CognitiveServicesUtilities();
            connectionUtils = new ConnectionUtilities();

            bool isConnected;

            try
            {
                isConnected = connectionUtils.ConnectToUWP().Result;
            }
            catch (Exception)
            {

                throw;
            }

            while (isConnected) { }
        }
    }
}
