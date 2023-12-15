namespace PVAConverter.Data
{
    internal struct Vertex
    {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public Vector2 UV { get; set; }

        public byte[] ToByteArray() // Written by ChatGPT
        {
            // Convert each component to byte arrays
            byte[] positionBytes = BitConverter.GetBytes(Position.x)
                .Concat(BitConverter.GetBytes(Position.y))
                .Concat(BitConverter.GetBytes(Position.z))
                .ToArray();

            byte[] normalBytes = BitConverter.GetBytes(Normal.x)
                .Concat(BitConverter.GetBytes(Normal.y))
                .Concat(BitConverter.GetBytes(Normal.z))
                .ToArray();

            byte[] uvBytes = BitConverter.GetBytes(UV.x)
                .Concat(BitConverter.GetBytes(UV.y))
                .ToArray();

            // Concatenate all the byte arrays to create the final result
            byte[] result = positionBytes
                .Concat(normalBytes)
                .Concat(uvBytes)
                .ToArray();

            return result;
        }
    }
}
