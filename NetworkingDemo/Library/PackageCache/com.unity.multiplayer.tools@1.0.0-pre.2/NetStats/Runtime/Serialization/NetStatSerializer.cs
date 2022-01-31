using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Collections;

namespace Unity.Multiplayer.Tools.NetStats
{
    internal class NetStatSerializer : INetStatSerializer
    {
        readonly BinaryFormatter m_BinaryFormatter;
        readonly MemoryStream m_MemoryStream;

        public NetStatSerializer()
        {
            m_MemoryStream = new MemoryStream();
            m_BinaryFormatter = new BinaryFormatter();
        }
        
        public NativeArray<byte> Serialize(MetricCollection metricCollection)
        {
            m_MemoryStream.SetLength(0);
            
            m_BinaryFormatter.Serialize(m_MemoryStream, metricCollection);
            long streamLength = m_MemoryStream.Length;
        
            var nativeArray = new NativeArray<byte>((int)streamLength, Allocator.Temp);
            var memoryStreamBuffer = m_MemoryStream.GetBuffer();
            
            for (int i = 0; i < streamLength; ++i)
            {
                nativeArray[i] = memoryStreamBuffer[i];
            }

            return nativeArray;
        }

        public MetricCollection Deserialize(NativeArray<byte> bytes)
        {
            m_MemoryStream.SetLength(0);
            
            int bytesLength = bytes.Length;
            for (int i = 0; i < bytesLength; ++i)
            {
                m_MemoryStream.WriteByte(bytes[i]);
            }

            m_MemoryStream.Seek(0, SeekOrigin.Begin);
            return (MetricCollection)m_BinaryFormatter.Deserialize(m_MemoryStream);
        }
    }
}
