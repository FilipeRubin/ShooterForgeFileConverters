namespace PVAConverter.Data
{
    internal class ModelData
    {
        public Vertex[] Vertices { get; init; }
        public int[] Indices { get; init; }

        public ModelData(Vertex[] vertices, int[] indices)
        {
            Vertices = vertices;
            Indices = indices;
        }
    }
}
