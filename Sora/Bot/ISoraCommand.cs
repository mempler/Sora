using System.Collections.Generic;
using Sora.Enums;
using Sora.Services;

namespace Sora.Bot
{
    public interface ISoraCommand
    {
        string Command { get; }
        string Description { get; }
        List<Argument> Args { get; }
        int ExpectedArgs { get; }
        Privileges RequiredPrivileges { get; }
        
        bool Execute(string[] args);
    }
}