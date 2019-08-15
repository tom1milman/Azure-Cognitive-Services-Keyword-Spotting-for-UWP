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
        #region Properties

        private AppServiceConnection _connection;
        public AppServiceConnection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        #endregion

        #region Constructor

        public AppToAppConnectorUWP() { }

        #endregion

        #region Methods

        /// <summary>
        /// Opens the Listener application
        /// </summary>
        /// <param name="isFirst">bool - is first time openning</param>
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

        /// <summary>
        /// Sends a request from the UWP to the listener with the given enum and obj
        /// </summary>
        /// <param name="currentEnum">Enum associated with the action that is performed</param>
        /// <param name="obj">Object with the needed information</param>
        /// <returns></returns>
        public async Task<string> SendRequest(CommunicationEnums currentEnum, object obj)
        {
            ValueSet request = new ValueSet();
            request.Add(currentEnum.ToString(), obj);

            await Connection.SendMessageAsync(request);
            return null;
        }

        #endregion

        #region Handlers

        /// <summary>
        /// Handler for when Listener gets connected to the UWP
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">AppServiceTriggerDetails</param>
        private async void MainPage_AppServiceConnected(object sender, AppServiceTriggerDetails e)
        {
            Connection.RequestReceived += AppServiceConnection_RequestReceived;
            Debug.WriteLine("Connected");
            await UwpKeywordSpotting.MainPage.mainPage.ToggleKws(true);
        }

        /// <summary>
        /// Handler for when connection is closed/lost with the listener
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MainPage_AppServiceDisconnected(object sender, EventArgs e)
        {
            Debug.WriteLine("Disconnected");
            OpenListenerApp(false);
        }

        /// <summary>
        /// Handler - takes care of incoming requests from the Listener
        /// </summary>
        /// <param name="sender">AppServiceConnection</param>
        /// <param name="args">AppServiceRequestReceivedEventArgs</param>
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
                        await SendRequest(CommunicationEnums.TurnKwsOn, bool.TrueString);
                    await UwpKeywordSpotting.MainPage.mainPage.SetSpeechListeningViewVisibility(false);
                    break;
                case CommunicationEnums.GuiOn:
                    Debug.WriteLine("GUI");
                    await UwpKeywordSpotting.MainPage.mainPage.SetSpeechListeningViewVisibility(true);
                    break;
            }
        }

        #endregion
    }
}

