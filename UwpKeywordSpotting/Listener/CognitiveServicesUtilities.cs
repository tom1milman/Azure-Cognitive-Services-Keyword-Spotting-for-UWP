using Listener.Resources;
using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UwpKeywordSpotting.AppToAppCommunication;
using Windows.Storage;

namespace Listener
{
    public class CognitiveServicesUtilities
    {
        public SpeechRecognizer KwsRecognizer, SpeechRecognizer;
        public bool isKwnOn;
        private string AzureKey, Region;
        private KeywordRecognitionModel KwsModel;

        public CognitiveServicesUtilities()
        {
            SetKeys();
            SetReognitionModel();
            SetSpeechRecognizer();
            SetKwsRecognizer();
        }

        private void SetKeys()
        {
            ResourceManager rm = CognitiveServicesKeys.ResourceManager;
            AzureKey = rm.GetString("AzureSubscriptionKey");
            Region = rm.GetString("Region");
        }

        private async Task SetReognitionModel()
        {
            string fileName = "kws.table";
            StorageFile sFile = null;
            try
            {
                sFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Listener\Resources\" + fileName);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message} \n{e.StackTrace}");
                throw;
            }

            string path = sFile.Path;
            KwsModel = KeywordRecognitionModel.FromFile(path);
        }

        private void SetSpeechRecognizer()
        {
            var config = SpeechConfig.FromSubscription(AzureKey, Region);
            SpeechRecognizer = new SpeechRecognizer(config);
        }

        private void SetKwsRecognizer()
        {
            var config = SpeechConfig.FromSubscription(AzureKey, Region);

            var stopRecognition = new TaskCompletionSource<int>();
            KwsRecognizer = new SpeechRecognizer(config);

            // Subscribes to events.
            KwsRecognizer.Recognizing += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizingKeyword)
                {
                    Console.WriteLine($"RECOGNIZING KEYWORD: Text={e.Result.Text}");
                    Console.WriteLine(">>> Sending Request>>>");
                    Thread speechRecognitionThread = new Thread(() => RecognizeSpeechAsync());
                    speechRecognitionThread.Start();
                    Program.connectionUtils.SendRequest(CommunicationEnums.GuiOn, bool.TrueString);
                    Console.WriteLine("<<< Request sent <<<");
                }
                else if (e.Result.Reason == ResultReason.RecognizingSpeech)
                {
                    Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
                }
            };

            KwsRecognizer.Recognized += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedKeyword)
                {
                    Console.WriteLine($"RECOGNIZED KEYWORD: Text={e.Result.Text}");
                }
                else if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                }
                else if (e.Result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine("NOMATCH: Speech could not be recognized.");
                }
            };

            KwsRecognizer.Canceled += (s, e) =>
            {
                Console.WriteLine($"CANCELED: Reason={e.Reason}");

                if (e.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                    Console.WriteLine($"CANCELED: Did you update the subscription info?");
                }
                stopRecognition.TrySetResult(0);
            };

            KwsRecognizer.SessionStarted += (s, e) =>
            {
                Console.WriteLine("\n    Session started event.");
            };

            KwsRecognizer.SessionStopped += (s, e) =>
            {
                Console.WriteLine("\n    Session stopped event.");
                Console.WriteLine("\nStop recognition.");
                stopRecognition.TrySetResult(0);
            };
        }

        public async void ContinuousRecognitionWithKeywordSpottingAsync()
        {
            // Creates an instance of a keyword recognition model. Update this to
            // point to the location of your keyword recognition model.
            var model = KwsModel;

            // The phrase your keyword recognition model triggers on.
            var keyword = "Okay ASA";

            var stopRecognition = new TaskCompletionSource<int>();

            // Starts recognizing.
            Console.WriteLine($"Say something starting with the keyword '{keyword}' followed by whatever you want...");

            // Starts continuous recognition using the keyword model. Use
            // StopKeywordRecognitionAsync() to stop recognition.
            Console.WriteLine("<<<");
            await KwsRecognizer.StartKeywordRecognitionAsync(model).ConfigureAwait(true);
            Console.WriteLine(">>>");


            // Waits for a single successful keyword-triggered speech recognition (or error).
            // Use Task.WaitAny to keep the task rooted.
            Task.WaitAny(new[] { stopRecognition.Task });

            // Stops recognition.
            await KwsRecognizer.StopKeywordRecognitionAsync().ConfigureAwait(false);

        }

        public async Task<String> RecognizeSpeechAsync()
        {
            Console.WriteLine("Say something...");

            // Starts speech recognition, and returns after a single utterance is recognized. The end of a
            // single utterance is determined by listening for silence at the end or until a maximum of 15
            // seconds of audio is processed.  The task returns the recognition text as result. 
            // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
            // shot recognition like command or query. 
            // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
            var result = await SpeechRecognizer.RecognizeOnceAsync();

            // Checks result.
            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                Console.WriteLine($"---Recognized: {result.Text}");
                Program.connectionUtils.SendRequest(CommunicationEnums.Speech, result.Text);
                return result.Text;
            }
            else if (result.Reason == ResultReason.NoMatch)
            {
                Console.WriteLine($"NOMATCH: Speech could not be recognized.");
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(result);
                Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    Console.WriteLine($"CANCELED: Did you update the subscription info?");
                }
            }

            return null;

        }

    }
}
