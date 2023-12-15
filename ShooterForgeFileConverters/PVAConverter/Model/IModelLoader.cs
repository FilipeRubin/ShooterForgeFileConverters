using PVAConverter.Data;

namespace PVAConverter.Model
{
    internal interface IModelLoader
    {
        ModelData? Load();
    }
}
