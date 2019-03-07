using JetBrains.Annotations;
using Shared.Services;

namespace Shared.Interfaces
{
    public interface IOsuStringable
    {
        string ToOsuString([CanBeNull] Database db);
    }
}