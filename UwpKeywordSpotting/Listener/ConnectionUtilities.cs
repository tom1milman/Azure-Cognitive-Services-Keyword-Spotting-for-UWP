using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UwpKeywordSpotting.AppToAppCommunication;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace Listener
{
    public class ConnectionUtilities
    {
        AppServiceConnection Connection = null;

        public async Task<bool> ConnectToUWP()
        {
            Connection = new AppServiceConnection();
            Connection.AppServiceName = "VoiceRecognition";
            Connection.PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName;
            Connection.RequestReceived += Connection_RequestReceived;
            Connection.ServiceClosed += Connection_ServiceClosed;

            string str;

            AppServiceConnectionStatus status = await Connection.OpenAsync();
            switch (status)
            {
                case AppServiceConnectionStatus.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    str = "Connection established - waiting for requests";
                    Console.WriteLine(str);
                    Console.WriteLine();
                    break;
                case AppServiceConnectionStatus.AppNotInstalled:
                    Console.ForegroundColor = ConsoleColor.Red;
                    str = "The app AppServicesProvider is not installed.";
                    Console.WriteLine(str);
                    return false;
                case AppServiceConnectionStatus.AppUnavailable:
                    Console.ForegroundColor = ConsoleColor.Red;
                    str = "The app AppServicesProvider is not available.";
                    Console.WriteLine("The app AppServicesProvider is not available.");
                    return false;
                case AppServiceConnectionStatus.AppServiceUnavailable:
                    Console.ForegroundColor = ConsoleColor.Red;
                    str = string.Format("The app AppServicesProvider is installed but it does not provide the app service {0}.", Connection.AppServiceName);
                    Console.WriteLine(string.Format("The app AppServicesProvider is installed but it does not provide the app service {0}.", Connection.AppServiceName));
                    return false;
                case AppServiceConnectionStatus.Unknown:
                    Console.ForegroundColor = ConsoleColor.Red;
                    str = string.Format("An unkown error occurred while we were trying to open an AppServiceConnection.");
                    Console.WriteLine(string.Format("An unkown error occurred while we were trying to open an AppServiceConnection."));
                    return false;
            }

            return true;
        }

        public async void SendRequest(CommunicationEnums currentEnum, string result)
        {
            ValueSet request = new ValueSet();
            request.Add(currentEnum.ToString(), result);

            await Connection.SendMessageAsync(request);
        }

        private async void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            ValueSet set = args.Request.Message;
            Object temp = null;

            CommunicationEnums currentEnum = CommunicationEnumsExtention.GetEnumFromValueSet(set.Keys);

            switch (currentEnum)
            {
                case CommunicationEnums.TurnKwsOn:
                    Program.cognitiveServicesUtils.isKwnOn = true;
                    Program.cognitiveServicesUtils.ContinuousRecognitionWithKeywordSpottingAsync();
                    break;
                case CommunicationEnums.TurnKwsOff:
                    Program.cognitiveServicesUtils.isKwnOn = false;
                    await Program.cognitiveServicesUtils.KwsRecognizer.StopContinuousRecognitionAsync();
                    break;
                case CommunicationEnums.Speech:
                    await Program.cognitiveServicesUtils.KwsRecognizer.StopContinuousRecognitionAsync();
                    await Program.cognitiveServicesUtils.RecognizeSpeechAsync();
                    break;
                default:
                    Console.WriteLine("Request Received not reccognized");
                    break;
            }

        }

        private void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            Environment.Exit(0);
        }
    }
}
