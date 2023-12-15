using System.Text;

namespace PCSConverter.Model
{
    internal class FileConvexShapeExporter : IConvexShapeExporter
    {
        private readonly string _fileName;
        private readonly byte[] _data;

        public FileConvexShapeExporter(string fileName, float[] data)
        {
            _fileName = fileName;
            int dataLength = 5 + sizeof(uint) + Buffer.ByteLength(data);
            _data = new byte[dataLength];
            byte[] lengthBytes = BitConverter.GetBytes((uint)(data.Length / 3));
            int dataOffset = 0;
            Buffer.BlockCopy(Encoding.ASCII.GetBytes("rubin"), 0, _data, dataOffset, 5);
            dataOffset += 5;
            Buffer.BlockCopy(lengthBytes, 0, _data, dataOffset, sizeof(uint));
            dataOffset += sizeof(uint);
            Buffer.BlockCopy(data, 0, _data, dataOffset, Buffer.ByteLength(data));
        }

        public void Export()
        {
            File.WriteAllBytes(_fileName, _data);
        }
    }
}
