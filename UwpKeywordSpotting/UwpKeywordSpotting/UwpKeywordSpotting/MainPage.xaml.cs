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
        public static MainPage mainPage;
        public bool isKwsOn;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            mainPage = this;
            isKwsOn = false;

            Thread thread = new Thread(() => AppToAppConnectorManager.AppConnector.OpenListenerApp(true));
            thread.Start();
        }

        public async Task SetSpeechResultText(string text)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SpeechResultText.Text = $"Recognized: {text}";
            });
        }

        public async Task SetSpeechListeningViewVisibility(bool setVisibility)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ListeningView.IsVisible = setVisibility;
            });
        }


        private async void ListenToMic_Clicked(object sender, EventArgs e)
        {
            await AppToAppConnectorManager.AppConnector.SendRequest(CommunicationEnums.Speech.ToString(), true);
            await SetSpeechListeningViewVisibility(true);
        }


        private async void ToggleKws_Clicked(object sender, EventArgs e)
        {
            await ToggleKws(!isKwsOn);
        }

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
    }
}
