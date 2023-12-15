using PVAConverter.Data;
using PVAConverter.Model;
using System.Globalization;

namespace PVAConverter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            foreach (string arg in args)
            {
                string fileName = Path.GetFileNameWithoutExtension(arg);
                IModelLoader modelLoader = new ModelFileLoader(arg);
                ModelData? modelData = modelLoader.Load();
                if (modelData != null)
                {
                    byte[] pvaContent = PVABuilder.BuildPVA(modelData);
                    File.WriteAllBytes(fileName + ".pva", pvaContent);
                }
            }
        }
    }
}