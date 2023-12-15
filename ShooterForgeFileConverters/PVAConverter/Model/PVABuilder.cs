using PVAConverter.Data;
using System.Text;

namespace PVAConverter.Model
{
    internal static class PVABuilder
    {
        public static byte[] BuildPVA(ModelData modelData)
        {
            // 5 bytes = header "rubin"
            // 4 bytes = vertex buffer length
            // 4 bytes = index buffer length
            // 
            // vertex buffer occupies 32 bytes per element
            // index buffer occupies 4 bytes per element
            int fileLength = 5 + 4 + 4 + modelData.Vertices.Length * 32 + modelData.Indices.Length * 4;

            using MemoryStream stream = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Encoding.ASCII.GetBytes("rubin"));
            writer.Write(BitConverter.GetBytes((uint)modelData.Vertices.Length));
            writer.Write(BitConverter.GetBytes((uint)modelData.Indices.Length));

            foreach (Vertex vertex in modelData.Vertices)
            {
                writer.Write(vertex.ToByteArray());
            }
            foreach (int index in modelData.Indices)
            {
                writer.Write(index);
            }

            byte[] pvaContent = stream.ToArray();

            return pvaContent;
        }
    }
}
