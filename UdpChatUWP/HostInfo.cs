using Windows.Networking;

namespace UdpChatUWP
{
    class HostInfo
    {
        public string UserName { get; set; }
        public HostName LocalIP { get; set; }
        public HostName MulticastIP { get; set; }
        public int LocalPort { get; set; }
        public int RemotePort { get; set; }
    }
}
