namespace PCSConverter.Model
{
    internal class FileModelLoader : IModelLoader
    {
        private readonly string _fileName;

        public FileModelLoader(string fileName)
        {
            _fileName = fileName;
        }

        public float[] Load()
        {
            int verticeCount = LoadVerticeCount();
            float[] result = new float[verticeCount * 3];
            LoadVertices(ref result);
            return result;
        }

        private int LoadVerticeCount()
        {
            int verticeCount = 0;
            foreach (string line in File.ReadAllLines(_fileName))
            {
                if (line.StartsWith("v "))
                {
                    verticeCount++;
                }
            }
            return verticeCount;
        }

        private void LoadVertices(ref float[] vertices)
        {
            int currentIndex = 0;

            foreach (string line in File.ReadAllLines(_fileName))
            {
                if (line.StartsWith("v "))
                {
                    char[,] floatBuffer = new char[3, 10];
                    int floatBufferIndex = 0;
                    int floatBufferElementIndex = 0;
                    for (int i = 2; i < line.Length; i++)
                    {
                        char currentCharacter = line[i];
                        if (currentCharacter == ' ')
                        {
                            floatBufferIndex++;
                            floatBufferElementIndex = 0;
                            continue;
                        }
                        floatBuffer[floatBufferIndex, floatBufferElementIndex] = line[i];
                        floatBufferElementIndex++;
                    }

                    char[] posX = new char[10];
                    char[] posY = new char[10];
                    char[] posZ = new char[10];
                    Buffer.BlockCopy(floatBuffer, floatBuffer.GetLength(1) * sizeof(char) * 0, posX, 0, floatBuffer.GetLength(1) * sizeof(char));
                    Buffer.BlockCopy(floatBuffer, floatBuffer.GetLength(1) * sizeof(char) * 1, posY, 0, floatBuffer.GetLength(1) * sizeof(char));
                    Buffer.BlockCopy(floatBuffer, floatBuffer.GetLength(1) * sizeof(char) * 2, posZ, 0, floatBuffer.GetLength(1) * sizeof(char));

                    if (float.TryParse(posX, out float valueX))
                        vertices[currentIndex + 0] = valueX;
                    if (float.TryParse(posY, out float valueY))
                        vertices[currentIndex + 1] = valueY;
                    if (float.TryParse(posZ, out float valueZ))
                        vertices[currentIndex + 2] = valueZ;

                    currentIndex += 3;
                }
            }
        }
    }
}
