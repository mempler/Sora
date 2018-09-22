using Shared.Database;

// Only used for Migration. so just ignore it ^^
namespace Shared
{
    class Program
    {
        static void Main()
        {
            using (var db = new SoraContext()) { }
        }
    }
}
