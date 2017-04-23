using System;
using System.Linq;
using System.Net;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UdpChatUWP
{
    public sealed partial class Scenario1_Connect : Page
    {
        public Scenario1_Connect()
        {
            InitializeComponent();
            refreshButton_Click(null, null);
        }

        private async void connectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                void error(string message)
                {
                    throw new Exception(message);
                }

                int localPort;
                int remotePort;
                IPAddress multicastIP;

                #region Validation

                if (string.IsNullOrEmpty(nameTextBox.Text) || string.IsNullOrEmpty(multicastTextBox.Text)
                    || string.IsNullOrEmpty(localPortTextBox.Text) || string.IsNullOrEmpty(remotePortTextBox.Text))
                {
                    error("All fields are required!");
                }

                if (nameTextBox.Text.Length > 32 || nameTextBox.Text.Length < 3)
                {
                    error("The \"Name\" length should be between 3 and 32 characters!");
                }

                if (!int.TryParse(remotePortTextBox.Text, out remotePort) || !int.TryParse(localPortTextBox.Text, out localPort))
                {
                    error("\"Local port\" and \"Remote port\" should be numbers!");
                }

                if (remotePort < 1 || remotePort > 65535 || localPort < 1 || localPort > 65535)
                {
                    error("The \"Local port\" and \"Remote port\" should be between 1 and 65535!");
                }
                
                if (!IPAddress.TryParse(multicastTextBox.Text, out multicastIP))
                {
                    error("The \"Multicast address\" does not meet the prescribed format!");
                }

                #endregion

                var hostInfo = new HostInfo
                {
                    UserName = nameTextBox.Text,
                    LocalIP = new HostName(GetLocalData().ip),
                    MulticastIP = new HostName(multicastTextBox.Text),
                    LocalPort = localPort,
                    RemotePort = remotePort
                };

                Frame.Navigate(typeof(Scenario2_Chat), hostInfo);
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
            }
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            var hostInfo = GetLocalData();
            nameTextBox.Text = hostInfo.hostName;

            if (!string.IsNullOrEmpty(hostInfo.ip))
            {
                localIpTextBlock.Text = hostInfo.ip;
                connectButton.IsEnabled = true;
            }
        }

        private (string ip, string hostName) GetLocalData()
        {
            var ip = NetworkInformation.GetHostNames().Where(h => h.Type == HostNameType.Ipv4).First()?.ToString();
            var hostName = Dns.GetHostName();

            return (ip, hostName);
        }
    }
}
