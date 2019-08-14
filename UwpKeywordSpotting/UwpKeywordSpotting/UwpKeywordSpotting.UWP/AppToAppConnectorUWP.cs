using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UwpKeywordSpotting.AppToAppCommunication;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;

namespace UwpKeywordSpotting.UWP
{
    public class AppToAppConnectorUWP : IAppToAppConnector
    {

        private AppServiceConnection _connection;
        public AppServiceConnection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        public AppToAppConnectorUWP() { }

        public async void OpenListenerApp(bool isFirst)
        {
            if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0))
            {
                try
                {
                    if (isFirst)
                    {
                        App.AppServiceConnected += MainPage_AppServiceConnected;
                        App.AppServiceDisconnected += MainPage_AppServiceDisconnected;
                    }

                    await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        private async void MainPage_AppServiceConnected(object sender, AppServiceTriggerDetails e)
        {
            Connection.RequestReceived += AppServiceConnection_RequestReceived;
            Debug.WriteLine("Connected");
            await UwpKeywordSpotting.MainPage.mainPage.ToggleKws(true);
        }

        private async void MainPage_AppServiceDisconnected(object sender, EventArgs e)
        {
            Debug.WriteLine("Disconnected");
            OpenListenerApp(false);
        }

        public async Task<string> SendRequest(string name, object obj)
        {
            ValueSet request = new ValueSet();
            request.Add(name, obj);

            await Connection.SendMessageAsync(request);
            return null;
        }

        private async void AppServiceConnection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            Object objReceived = null;
            ValueSet set = args.Request.Message;

            CommunicationEnums currentEnum = CommunicationEnumsExtention.GetEnumFromValueSet(set.Keys);

            if (currentEnum == CommunicationEnums.NULL)
                return;

            if (!set.TryGetValue(currentEnum.ToString(), out objReceived) && objReceived == null)
                return;

            switch (currentEnum)
            {
                case CommunicationEnums.Speech:
                    string message = (string)objReceived;
                    Debug.WriteLine(message);
                    await UwpKeywordSpotting.MainPage.mainPage.SetSpeechResultText(message);
                    if (UwpKeywordSpotting.MainPage.mainPage.isKwsOn)
                        await SendRequest(CommunicationEnums.TurnKwsOn.ToString(), bool.TrueString);
                    await UwpKeywordSpotting.MainPage.mainPage.SetSpeechListeningViewVisibility(false);
                    break;
                case CommunicationEnums.GuiOn:
                    Debug.WriteLine("GUI");
                    await UwpKeywordSpotting.MainPage.mainPage.SetSpeechListeningViewVisibility(true);
                    break;
            }
        }
    }
}

