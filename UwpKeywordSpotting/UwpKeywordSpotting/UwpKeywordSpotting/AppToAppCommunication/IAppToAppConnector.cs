using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;

namespace UwpKeywordSpotting.AppToAppCommunication
{
    public interface IAppToAppConnector
    {
        AppServiceConnection Connection { get; set; }
        void OpenListenerApp(bool isFirst);
        Task<string> SendRequest(CommunicationEnums currentEnum, Object obj);
    }
}
