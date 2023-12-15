using System.Globalization;
using PCSConverter.Model;

namespace PCSConverter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            foreach (string arg in args)
            {
                string fileName = Path.GetFileNameWithoutExtension(arg);
                IModelLoader modelLoader = new FileModelLoader(arg);
                float[] vertices = modelLoader.Load();
                IConvexShapeExporter convexShapeExporter = new FileConvexShapeExporter(fileName + ".pcs", vertices);
                convexShapeExporter.Export();
            }
        }
    }
}