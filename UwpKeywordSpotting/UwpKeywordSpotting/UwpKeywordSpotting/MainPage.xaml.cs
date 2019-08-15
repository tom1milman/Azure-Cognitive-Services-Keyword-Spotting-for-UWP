using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using UwpKeywordSpotting.AppToAppCommunication;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Xamarin.Forms;

namespace UwpKeywordSpotting
{
    public partial class MainPage : ContentPage
    {
        #region Properties

        public static MainPage mainPage;
        public bool isKwsOn;

        #endregion

        #region Constructor

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            mainPage = this;
            isKwsOn = false;

            Thread thread = new Thread(() => AppToAppConnectorManager.AppConnector.OpenListenerApp(true));
            thread.Start();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets Speech Result Text (UI) to the given string
        /// </summary>
        /// <param name="text">string</param>
        /// <returns>Task</returns>
        public async Task SetSpeechResultText(string text)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SpeechResultText.Text = $"Recognized: {text}";
            });
        }

        /// <summary>
        /// Sets the visibility of the View (true/false)
        /// </summary>
        /// <param name="setVisibility">bool - set view visible?</param>
        /// <returns>Task</returns>
        public async Task SetSpeechListeningViewVisibility(bool setVisibility)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ListeningView.IsVisible = setVisibility;
            });
        }

        /// <summary>
        /// Toggles the keyword spotting functionality 
        /// </summary>
        /// <param name="toggle">bool - turn kws on?</param>
        /// <returns>Task</returns>
        public async Task ToggleKws(bool toggle)
        {
            CommunicationEnums requestEnum;

            if (toggle)
                requestEnum = CommunicationEnums.TurnKwsOn;
            else
                requestEnum = CommunicationEnums.TurnKwsOff;

            await AppToAppConnectorManager.AppConnector.SendRequest(requestEnum.ToString(), toggle);

            isKwsOn = !isKwsOn;

            if (toggle)
                ToggleKwsButton.BackgroundColor = Color.ForestGreen;
            else
                ToggleKwsButton.BackgroundColor = Color.DarkRed;
        }

        #endregion

        #region ButtonCommands

        private async void ListenToMic_Clicked(object sender, EventArgs e)
        {
            await AppToAppConnectorManager.AppConnector.SendRequest(CommunicationEnums.Speech.ToString(), true);
            await SetSpeechListeningViewVisibility(true);
        }

        private async void ToggleKws_Clicked(object sender, EventArgs e)
        {
            await ToggleKws(!isKwsOn);
        }

        #endregion
    }
}
