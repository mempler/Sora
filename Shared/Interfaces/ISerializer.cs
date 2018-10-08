using Shared.Helpers;

namespace Shared.Interfaces
{
    public interface ISerializer
    {
        void ReadFromStream(MStreamReader sr);
        void WriteToStream(MStreamWriter sw);
    }
}