using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UwpKeywordSpotting
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SpeechListeningView : ContentView
	{
		public SpeechListeningView ()
		{
			InitializeComponent ();
            BindingContext = this;
		}
    }
}