using System;
using System.Collections.Generic;
using Sora.Objects;
using Sora.Services;

namespace Sora.Bot.Commands
{
    public class RestrictCommand : ISoraCommand
    {
        public string Command => "restrict";
        public string Description => "Restrict a user.";

        public List<Argument> Args
            => new List<Argument>
            {
                new Argument {ArgName = "User"},
                new Argument {ArgName = "Duration (10d)"},
                new Argument {ArgName = "Reason"}
            };

        public int ExpectedArgs => 3;
        public Permission RequiredPermission => Permission.From(Permission.ADMIN_RESTRICT);

        public bool Execute(Presence executor, string[] args) => throw new NotImplementedException();
    }
}
