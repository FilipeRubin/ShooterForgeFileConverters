using PVAConverter.Data;
using System;

namespace PVAConverter.Model
{
    internal class ModelFileLoader : IModelLoader
    {
        private readonly string _fileName;
        private int _numPositions;
        private int _numNormals;
        private int _numUVs;
        private int _numFaces;
        private int _numFaceVertexes;
        private int _numIndexes;
        private Vector3[] _positions;
        private Vector3[] _normals;
        private Vector2[] _uvs;
        private int[] _vertexesPerFace;
        private OBJVertexRef[] _refs;

        public ModelFileLoader(string fileName)
        {
            _fileName = fileName;
            _numPositions = 0;
            _numNormals = 0;
            _numUVs = 0;
            _numFaces = 0;
            _numFaceVertexes = 0;
            _numIndexes = 0;
            _positions = null!;
            _normals = null!;
            _uvs = null!;
            _vertexesPerFace = null!;
            _refs = null!;
        }

        public ModelData? Load()
        {
            Console.WriteLine("Loading model metadata...");
            LoadMetadata();

            if ((_numPositions | _numNormals | _numUVs | _numFaces) == 0)
            {
                return null;
            }
            Console.WriteLine("Model metadata read!");

            Console.WriteLine("Filling arrays...");
            FillArrays();
            Console.WriteLine("Arrays filled!");
            Console.WriteLine("Loading references...");
            LoadReferences();
            Console.WriteLine("References loaded!");

            Console.WriteLine("Filling vertexes...");
            Vertex[] vertexes = new Vertex[_numFaceVertexes];

            for (int i = 0; i < _numFaceVertexes; i++)
            {
                vertexes[i].Position = _positions[_refs[i].posId];
                vertexes[i].Normal = _normals[_refs[i].norId];
                vertexes[i].UV = _uvs[_refs[i].uvId];
            }
            Console.WriteLine("Vertexes filled!");

            int[] indexes = new int[_numIndexes];

            int indexIndex = 0; // This is the index iterating the index array

            int firstIndex = 0; // First index of the current face

            Console.WriteLine("Filling indexes...");

            for (int i = 0; i < _numFaces; i++) // For every vertex contained in every face declared in "f" lines in the .obj file
            {
                for (int j = 2; j < _vertexesPerFace[i]; j++)
                {
                    if ((indexIndex + 2U) < _numIndexes) // Just so the compiler doesn't C6386 unnecessarily
                    {
                        indexes[indexIndex + 0U] = firstIndex;
                        indexes[indexIndex + 1U] = firstIndex + j - 1;
                        indexes[indexIndex + 2U] = firstIndex + j;
                    }
                    indexIndex += 3;
                }
                firstIndex += _vertexesPerFace[i];
            }
            Console.WriteLine("Indexes filled!");

            Console.WriteLine("Optimizing... (final step)");
            RemoveDuplicates(ref vertexes, ref indexes);
            Console.WriteLine("Model optimized!");

            return new ModelData(vertexes, indexes);
        }

        private void LoadMetadata()
        {
            foreach (string line in File.ReadAllLines(_fileName))
            {
                if (line.StartsWith("v "))
                {
                    _numPositions++;
                }
                else if (line.StartsWith("vn "))
                {
                    _numNormals++;
                }
                else if (line.StartsWith("vt "))
                {
                    _numUVs++;
                }
                else if (line.StartsWith("f "))
                {
                    int indexesThisLine = 0;
                    for (int i = 1; i < line.Length; i++)
                    {
                        if (line[i] == ' ')
                            indexesThisLine++;
                    }
                    _numFaceVertexes += indexesThisLine;
                    if (indexesThisLine > 3)
                    {
                        indexesThisLine = (indexesThisLine - 2) * 3;
                    }
                    _numIndexes += indexesThisLine;
                    _numFaces++;
                }
            }
            _positions = new Vector3[_numPositions];
            _normals = new Vector3[_numNormals];
            _uvs = new Vector2[_numUVs];
            _vertexesPerFace = new int[_numFaces];
        }

        private void FillArrays()
        {
            int positionIndex = 0;
            int normalIndex = 0;
            int uvIndex = 0;
            int faceIndex = 0;

            foreach (string line in File.ReadAllLines(_fileName))
            {
                if (line.StartsWith("v ")) // Positions
                {
                    char[,] posBuffer = new char[3, 10];

                    int posBufferIdx     = 0; // Index of posBuffer col
                    int posBufferElemIdx = 0; // Index of posBuffer row

                    for (int i = 2; i < line.Length; i++)
                    {
                        if (line[i] == ' ')
                        {
                            posBufferElemIdx = 0;
                            posBufferIdx++;
                            continue;
                        }
                        posBuffer[posBufferIdx, posBufferElemIdx] = line[i];
                        posBufferElemIdx++;
                    }

                    char[] posX = new char[10];
                    char[] posY = new char[10];
                    char[] posZ = new char[10];
                    Buffer.BlockCopy(posBuffer, posBuffer.GetLength(1) * sizeof(char) * 0, posX, 0, posBuffer.GetLength(1) * sizeof(char));
                    Buffer.BlockCopy(posBuffer, posBuffer.GetLength(1) * sizeof(char) * 1, posY, 0, posBuffer.GetLength(1) * sizeof(char));
                    Buffer.BlockCopy(posBuffer, posBuffer.GetLength(1) * sizeof(char) * 2, posZ, 0, posBuffer.GetLength(1) * sizeof(char));

                    if (float.TryParse(posX, out float valueX))
                        _positions[positionIndex].x = valueX;
                    if (float.TryParse(posY, out float valueY))
                        _positions[positionIndex].y = valueY;
                    if (float.TryParse(posZ, out float valueZ))
                        _positions[positionIndex].z = valueZ;
                    positionIndex++;
                }
                else if (line.StartsWith("vn "))
                {
                    char[,] norBuffer = new char[3, 10];

                    int norBufferIdx     = 0; // Index of posBuffer col
                    int norBufferElemIdx = 0; // Index of posBuffer row

                    for (int i = 3; i < line.Length; i++)
                    {
                        if (line[i] == ' ')
                        {
                            norBufferElemIdx = 0;
                            norBufferIdx++;
                            continue;
                        }
                        norBuffer[norBufferIdx, norBufferElemIdx] = line[i];
                        norBufferElemIdx++;
                    }

                    char[] norX = new char[10];
                    char[] norY = new char[10];
                    char[] norZ = new char[10];
                    Buffer.BlockCopy(norBuffer, norBuffer.GetLength(1) * sizeof(char) * 0, norX, 0, norBuffer.GetLength(1) * sizeof(char));
                    Buffer.BlockCopy(norBuffer, norBuffer.GetLength(1) * sizeof(char) * 1, norY, 0, norBuffer.GetLength(1) * sizeof(char));
                    Buffer.BlockCopy(norBuffer, norBuffer.GetLength(1) * sizeof(char) * 2, norZ, 0, norBuffer.GetLength(1) * sizeof(char));

                    if (float.TryParse(norX, out float valueX))
                        _normals[normalIndex].x = valueX;
                    if (float.TryParse(norY, out float valueY))
                        _normals[normalIndex].y = valueY;
                    if (float.TryParse(norZ, out float valueZ))
                        _normals[normalIndex].z = valueZ;
                    normalIndex++;
                }
                else if (line.StartsWith("vt "))
                {
                    char[,] uvBuffer = new char[2, 10];

                    int uvBufferIdx = 0; // Index of posBuffer col
                    int uvBufferElemIdx = 0; // Index of posBuffer row

                    for (int i = 3; i < line.Length; i++)
                    {
                        if (line[i] == ' ')
                        {
                            uvBufferElemIdx = 0;
                            uvBufferIdx++;
                            continue;
                        }
                        uvBuffer[uvBufferIdx, uvBufferElemIdx] = line[i];
                        uvBufferElemIdx++;
                    }

                    char[] uvX = new char[10];
                    char[] uvY = new char[10];
                    Buffer.BlockCopy(uvBuffer, uvBuffer.GetLength(1) * sizeof(char) * 0, uvX, 0, uvBuffer.GetLength(1) * sizeof(char));
                    Buffer.BlockCopy(uvBuffer, uvBuffer.GetLength(1) * sizeof(char) * 1, uvY, 0, uvBuffer.GetLength(1) * sizeof(char));
                    
                    if (float.TryParse(uvX, out float valueX))
                        _uvs[uvIndex].x = valueX;
                    if (float.TryParse(uvY, out float valueY))
                        _uvs[uvIndex].y = valueY;
                    uvIndex++;
                }
                else if (line.StartsWith("f "))
                {
                    for (int i = 1; i < line.Length; i++)
                    {
                        if (line[i] == ' ')
                        {
                            _vertexesPerFace[faceIndex]++;
                        }
                    }
                    faceIndex++;
                }
            }

            _refs = new OBJVertexRef[_numFaceVertexes];
        }

        private void LoadReferences()
        {
            int refIdx = 0;

            foreach (string line in File.ReadAllLines(_fileName))
            {
                if (!line.StartsWith("f "))
                    continue;

                char[,] vertexReferenceBuffer = new char[3,10]; // 3 elements, position, normal and UV, respectivelly. Integers can have up to 10 digits (as for int.MaxValue)
                int bufferIdx = 0;
                int elemIdx = 0;
                for (int i = 2; i <= line.Length; i++)
		        {
			        if (i == line.Length || line[i] == ' ')
			        {
                        // Digits of an integer represented as characters (to be conmverted)
                        char[] posRefBuffer = new char[10];
                        char[] norRefBuffer = new char[10];
                        char[] uvRefBuffer  = new char[10];
                        Buffer.BlockCopy(vertexReferenceBuffer, vertexReferenceBuffer.GetLength(1) * sizeof(char) * 0, posRefBuffer, 0, vertexReferenceBuffer.GetLength(1) * sizeof(char));
                        Buffer.BlockCopy(vertexReferenceBuffer, vertexReferenceBuffer.GetLength(1) * sizeof(char) * 2, norRefBuffer, 0, vertexReferenceBuffer.GetLength(1) * sizeof(char));
                        Buffer.BlockCopy(vertexReferenceBuffer, vertexReferenceBuffer.GetLength(1) * sizeof(char) * 1,  uvRefBuffer, 0, vertexReferenceBuffer.GetLength(1) * sizeof(char));

                        if (int.TryParse(posRefBuffer, out int posRef))
				            _refs[refIdx].posId = posRef - 1;
                        if (int.TryParse(norRefBuffer, out int norRef))
                            _refs[refIdx].norId = norRef - 1;
                        if (int.TryParse(uvRefBuffer, out int uvRef))
                            _refs[refIdx].uvId = uvRef - 1;

                        vertexReferenceBuffer = new char[3, 10];

                        refIdx++;
				        bufferIdx = 0;
				        elemIdx = 0;
			        }
                    else if (line[i] == '/')
                    {
                        bufferIdx++;
                        elemIdx = 0;
                    }
                    else if (char.IsDigit(line[i]))
			        {
				        vertexReferenceBuffer[bufferIdx, elemIdx] = line[i];
				        elemIdx++;
			        }
		        }
            }
        }

        private void RemoveDuplicates(ref Vertex[] vertexes, ref int[] indexes)
        {
            int numDuplicates = 0;

            // Count how many duplicates
            for (int i = 0; i < vertexes.Length; i++)
            {
                for (int j = i + 1; j < vertexes.Length; j++)
                {
                    if (vertexes[i].Equals(vertexes[j]))
                    {
                        numDuplicates++;
                        break;
                    }
                }
            }

            int numUniques = vertexes.Length - numDuplicates;

            Vertex[] uniques = new Vertex[numUniques];
            int oIndex = 0; // Original array index

            for (int i = 0; i < numUniques; i++) // For every unique vertex, fill the unique array
            {
                bool repeatedElement = false;

                for (int j = 0; j < i; j++)
                {
                    if (uniques[j].Equals(vertexes[oIndex]))
                    {
                        repeatedElement = true;
                        break;
                    }
                }

                if (!repeatedElement)
                {
                    uniques[i] = vertexes[oIndex];
                }
                else
                {
                    i--; // Uses the same index as before
                }
                oIndex++;
            }

            for (int i = 0; i < indexes.Length; i++) // For every index, check and readjust its value if needed
            {
                ref Vertex vertexIndexPointsTo = ref vertexes[indexes[i]]; // We will need to find where this vertex is located in the new array

                for (int j = 0; j < numUniques; j++)
                {
                    if (vertexIndexPointsTo.Equals(uniques[j]))
                    {
                        indexes[i] = j;
                        break;
                    }
                }
            }

            vertexes = uniques;
        }
    }
}
