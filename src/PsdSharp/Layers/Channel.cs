using PsdSharp.IO;

namespace PsdSharp.Layers
{
    public class Channel
    {
        public byte[] ImageData { get; set; }

        public short Id { get; set; }

        internal static Channel Load(BigEndianBinaryReader reader)
        {
            Channel channel = new Channel();

            channel.Id = reader.ReadInt16();

            // TODO: Add support for PSB (PSB channel image data length is 8 bytes)
            channel.ImageData = new byte[reader.ReadInt32()];

            return channel;
        }
    }
}