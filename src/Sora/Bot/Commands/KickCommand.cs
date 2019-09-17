using System;
using System.Collections.Generic;
using Sora.Framework;
using Sora.Framework.Objects;

namespace Sora.Bot.Commands
{
    public class KickCommand : ISoraCommand
    {
        public string Command => "kick";
        public string Description => "Kick somebody from your Server";
        public List<Argument> Args => new List<Argument> {new Argument {ArgName = "Username"}};
        public int ExpectedArgs => 1;
        public Permission RequiredPermission => Permission.From(Permission.ADMIN_KICK);

        public bool Execute(Presence executer, string[] args) => throw new NotImplementedException();
    }
}
