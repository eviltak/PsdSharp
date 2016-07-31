using System.IO;

namespace PsdSharp
{
    internal class RleUtility
    {
        public static int EncodedRow(Stream stream, byte[] imgData, int startIdx, int columns)
        {
            long startPosition = stream.Position;

            RlePacketStateMachine machine = new RlePacketStateMachine(stream);

            for (int x = 0; x < columns; ++x)
                machine.Push(imgData[x + startIdx]);

            machine.Flush();

            return (int) (stream.Position - startPosition);
        }

        public static void DecodedRow(Stream stream, byte[] imgData, int startIdx, int columns)
        {
            int count = 0;
            while (count < columns)
            {
                byte byteValue = (byte) stream.ReadByte();

                int len = byteValue;
                if (len < 128)
                {
                    len++;
                    while (len != 0 && startIdx + count < imgData.Length)
                    {
                        byteValue = (byte) stream.ReadByte();

                        imgData[startIdx + count] = byteValue;
                        count++;
                        len--;
                    }
                }
                else if (len > 128)
                {
                    // Next -len+1 bytes in the dest are replicated from next source byte.
                    // (Interpret len as a negative 8-bit int.)
                    len ^= 0x0FF;
                    len += 2;
                    byteValue = (byte) stream.ReadByte();

                    while (len != 0 && startIdx + count < imgData.Length)
                    {
                        imgData[startIdx + count] = byteValue;
                        count++;
                        len--;
                    }
                }
            }
        }

        private class RlePacketStateMachine
        {
            private bool rlePacket;

            private byte[] packetValues = new byte[128];

            private int packetLength;

            private Stream stream;

            internal RlePacketStateMachine(Stream stream)
            {
                this.stream = stream;
            }

            internal void Flush()
            {
                byte header;

                if (rlePacket)
                    header = (byte) -(packetLength - 1);
                else
                    header = (byte) (packetLength - 1);

                stream.WriteByte(header);

                int length = rlePacket ? 1 : packetLength;

                stream.Write(packetValues, 0, length);

                packetLength = 0;
            }

            internal void Push(byte color)
            {
                while (true)
                {
                    if (packetLength == 0)
                    {
                        // Starting a fresh packet.
                        rlePacket = false;
                        packetValues[0] = color;
                        packetLength = 1;
                    }
                    else if (packetLength == 1)
                    {
                        // 2nd byte of this packet... decide RLE or non-RLE.
                        rlePacket = color == packetValues[0];
                        packetValues[1] = color;
                        packetLength = 2;
                    }
                    else if (packetLength == packetValues.Length)
                    {
                        // Packet is full. Start a new one.
                        Flush();
                        continue;
                    }
                    else if (packetLength >= 2 && rlePacket && color != packetValues[packetLength - 1])
                    {
                        // We were filling in an RLE packet, and we got a non-repeated color.
                        // Emit the current packet and start a new one.
                        Flush();
                        continue;
                    }
                    else if (packetLength >= 2 && rlePacket && color == packetValues[packetLength - 1])
                    {
                        // We are filling in an RLE packet, and we got another repeated color.
                        // Add the new color to the current packet.
                        ++packetLength;
                        packetValues[packetLength - 1] = color;
                    }
                    else if (packetLength >= 2 && !rlePacket && color != packetValues[packetLength - 1])
                    {
                        // We are filling in a raw packet, and we got another random color.
                        // Add the new color to the current packet.
                        ++packetLength;
                        packetValues[packetLength - 1] = color;
                    }
                    else if (packetLength >= 2 && !rlePacket && color == packetValues[packetLength - 1])
                    {
                        // We were filling in a raw packet, but we got a repeated color.
                        // Emit the current packet without its last color, and start a
                        // new RLE packet that starts with a length of 2.
                        --packetLength;
                        Flush();
                        Push(color);
                        continue;
                    }
                    break;
                }
            }
        }
    }
}