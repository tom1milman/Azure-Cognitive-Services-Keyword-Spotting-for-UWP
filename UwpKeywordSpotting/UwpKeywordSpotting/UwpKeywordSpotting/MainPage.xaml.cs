using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Xamarin.Forms;

namespace UwpKeywordSpotting
{
    public partial class MainPage : ContentPage
    {
        public static MainPage mainPage;
        
        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            mainPage = this;
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
    }
}
