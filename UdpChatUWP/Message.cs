using System;
using System.IO;
using System.Runtime.Serialization;

namespace UdpChatUWP
{
    [DataContract(Name = "Message", Namespace = "UdpChat.Message")]
    public class Message
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string UserIP { get; set; }

        [DataMember]
        public string Text { get; set; }

        public byte[] Serialize()
        {
            var stream = new MemoryStream();
            var serializer = new DataContractSerializer(typeof(Message));

            serializer.WriteObject(stream, this);
            return stream.ToArray();
        }

        public static Message Deserialize(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            var serializer = new DataContractSerializer(typeof(Message));

            var message = serializer.ReadObject(stream) as Message;

            if (message == null)
            {
                throw new InvalidCastException();
            }
            return message;
        }
    }
}
