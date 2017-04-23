using System;
using System.Collections.ObjectModel;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace UdpChatUWP
{
    public sealed partial class Scenario2_Chat : Page
    {
        private HostInfo _hostInfo;

        private DatagramSocket _listener;
        private IOutputStream _outputStream;
        private DataWriter _writer;

        private ObservableCollection<Message> _messages = new ObservableCollection<Message>();

        public Scenario2_Chat()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var hostInfo = e.Parameter as HostInfo;
            _hostInfo = hostInfo;
            nameTextBlock.Text = hostInfo.UserName;

            StartListener();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _listener?.Dispose();
        }

        public async void StartListener()
        {
            try
            {
                _listener = new DatagramSocket();

                _listener.Control.MulticastOnly = true;
                await _listener.BindServiceNameAsync(_hostInfo.LocalPort.ToString());
                _listener.JoinMulticastGroup(_hostInfo.MulticastIP);

                _listener.MessageReceived += _listener_MessageReceived;

                _outputStream = await _listener.GetOutputStreamAsync(
                    new EndpointPair(_hostInfo.LocalIP, _hostInfo.LocalPort.ToString(), _hostInfo.MulticastIP, _hostInfo.RemotePort.ToString()));
                _writer = new DataWriter(_outputStream);
                await _writer.StoreAsync();
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
            }
        }

        private async void _listener_MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            try
            {
                var length = args.GetDataReader().UnconsumedBufferLength;
                var data = new byte[length];

                args.GetDataReader().ReadBytes(data);

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    _messages.Insert(0, Message.Deserialize(data));
                });
            }
            catch (Exception ex)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await new MessageDialog(ex.Message).ShowAsync();
                });
            }
        }

        private async void SendMessage(string messageText)
        {
            try
            {
                var message = new Message
                {
                    UserName = _hostInfo.UserName,
                    UserIP = _hostInfo.LocalIP.ToString(),
                    Text = messageText
                };
                _writer.WriteBytes(message.Serialize());

                await _writer.StoreAsync();
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
            }
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(messageTextBox.Text))
            {
                SendMessage(messageTextBox.Text);
                messageTextBox.Text = string.Empty;
            }
        }

        private void messageTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                sendButton_Click(messageTextBox, null);
            }
        }
    }
}
