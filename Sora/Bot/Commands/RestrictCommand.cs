using System.Collections.Generic;
using Sora.Enums;
using Sora.Services;

namespace Sora.Bot.Commands
{
    public class RestrictCommand : ISoraCommand
    {
        public string Command => "restrict";
        public string Description => "Restrict a user.";
        public List<Argument> Args => new List<Argument>
        {
            new Argument
            {
                ArgName = "User"
            },
            new Argument
            {
                ArgName  = "Duration (10d)"
            },
            new Argument
            {
                ArgName  = "Reason"
            }
        };

        public int ExpectedArgs => 3;

        public Privileges RequiredPrivileges => Privileges.ARestrict;
        public bool Execute(string[] args)
        {
            throw new System.NotImplementedException();
        }
    }
}