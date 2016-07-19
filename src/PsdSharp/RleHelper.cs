using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace PsdSharp
{
    class RleHelper
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

                int len = (int) byteValue;
                if (len < 128)
                {
                    len++;
                    while (len != 0 && (startIdx + count) < imgData.Length)
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

                    while (len != 0 && (startIdx + count) < imgData.Length)
                    {
                        imgData[startIdx + count] = byteValue;
                        count++;
                        len--;
                    }
                }
                //else if (128 == len)
                //{
                //    // Do nothing
                //}
            }
        }

        private class RlePacketStateMachine
        {
            private bool m_rlePacket = false;

            private byte[] m_packetValues = new byte[128];

            private int packetLength;

            private Stream m_stream;

            internal RlePacketStateMachine(Stream stream)
            {
                m_stream = stream;
            }

            internal void Flush()
            {
                byte header;
                if (m_rlePacket)
                {
                    header = (byte) (-(packetLength - 1));
                }
                else
                {
                    header = (byte) (packetLength - 1);
                }

                m_stream.WriteByte(header);

                int length = (m_rlePacket ? 1 : packetLength);

                m_stream.Write(m_packetValues, 0, length);

                packetLength = 0;
            }

            internal void Push(byte color)
            {
                if (packetLength == 0)
                {
                    // Starting a fresh packet.
                    m_rlePacket = false;
                    m_packetValues[0] = color;
                    packetLength = 1;
                }
                else if (packetLength == 1)
                {
                    // 2nd byte of this packet... decide RLE or non-RLE.
                    m_rlePacket = (color == m_packetValues[0]);
                    m_packetValues[1] = color;
                    packetLength = 2;
                }
                else if (packetLength == m_packetValues.Length)
                {
                    // Packet is full. Start a new one.
                    Flush();
                    Push(color);
                }
                else if (packetLength >= 2 && m_rlePacket && color != m_packetValues[packetLength - 1])
                {
                    // We were filling in an RLE packet, and we got a non-repeated color.
                    // Emit the current packet and start a new one.
                    Flush();
                    Push(color);
                }
                else if (packetLength >= 2 && m_rlePacket && color == m_packetValues[packetLength - 1])
                {
                    // We are filling in an RLE packet, and we got another repeated color.
                    // Add the new color to the current packet.
                    ++packetLength;
                    m_packetValues[packetLength - 1] = color;
                }
                else if (packetLength >= 2 && !m_rlePacket && color != m_packetValues[packetLength - 1])
                {
                    // We are filling in a raw packet, and we got another random color.
                    // Add the new color to the current packet.
                    ++packetLength;
                    m_packetValues[packetLength - 1] = color;
                }
                else if (packetLength >= 2 && !m_rlePacket && color == m_packetValues[packetLength - 1])
                {
                    // We were filling in a raw packet, but we got a repeated color.
                    // Emit the current packet without its last color, and start a
                    // new RLE packet that starts with a length of 2.
                    --packetLength;
                    Flush();
                    Push(color);
                    Push(color);
                }
            }
        }
    }
}
