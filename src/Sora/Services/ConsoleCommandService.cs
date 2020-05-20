using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore.Internal;
using Sora.Database;
using Sora.Database.Models;
using Sora.Framework;
using Sora.Framework.Utilities;

namespace Sora.Services
{
    public struct Argument
    {
        public string ArgName;
    }

    public struct ConsoleCommand
    {
        public delegate bool ConsoleCommandExecution(string[] args);

        public string Command;
        public string Description;
        public List<Argument> Args;
        public bool ConsoleOnly;
        public int ExpectedArgs;
        public ConsoleCommandExecution Callback;
    }

    public class ConsoleCommandService
    {
        private readonly object _mut;
        private Thread _consoleThread;
        private bool _shouldStop;

        public ConsoleCommandService(SoraDbContext ctx)
        {
            Commands = new List<ConsoleCommand>();
            _mut = new object();

            #region DefaultCommands

            RegisterCommand(
                "help",
                "Get a list of ALL Commands!",
                new List<Argument> {new Argument {ArgName = "command"}},
                0,
                args =>
                {
                    var l = "\n====== Command List ======\n";

                    foreach (var cmd in Commands)
                    {
                        var aList = "\t<";

                        var i = 0;
                        foreach (var a in cmd.Args)
                        {
                            aList += a.ArgName;
                            if (i >= cmd.ExpectedArgs)
                                aList += "?";
                            aList += ", ";
                            i++;
                        }

                        aList = aList.TrimEnd(cmd.Args.Count < 1 ? '<' : ' ').TrimEnd(',');
                        if (cmd.Args.Count > 0)
                            aList += ">";

                        l += "\n%#1c9624%/*\n";
                        l += cmd.Description;
                        l += $"\n*/{LCol.WHITE}\n";
                        l += "\n" + cmd.Command + aList;
                        l += "\n";
                    }

                    l += "\n==========================";

                    Logger.Info(l);

                    return true;
                }
            );

            RegisterCommand(
                "clear",
                "Clear Console",
                new List<Argument>(),
                0,
                args =>
                {
                    Console.Clear();
                    return true;
                }
            );

            RegisterCommand(
                "adduser",
                "Add a User to our Database",
                new List<Argument>
                {
                    new Argument {ArgName = "Username"},
                    new Argument {ArgName = "Password"},
                    new Argument {ArgName = "E-Mail"},
                    new Argument {ArgName = "Permissions"}
                },
                3,
                args =>
                {
                    var u = DbUser.RegisterUser(ctx, Permission.From(args[3..].Join()),
                        args[0], args[2], args[1], false).Result;
                    
                    if (u == null) {
                        Logger.Err("Failed! User has already been registered!");
                        return true;
                    }
                    
                    
                    Logger.Info(args[1]);
                    Logger.Info(
                        "Created User",
                        "%#F94848%" + u.UserName,
                        "%#B342F4%(", u.Id, "%#B342F4%)"
                    );
                    return true;
                }
            );

            #endregion
        }

        public List<ConsoleCommand> Commands { get; }

        public void RegisterCommand(string command, string description, List<Argument> args, int expectedArgs,
            ConsoleCommand.ConsoleCommandExecution cb)
        {
            lock (_mut)
            {
                Commands.Add(
                    new ConsoleCommand
                    {
                        Command = command,
                        Description = description,
                        Args = args,
                        ExpectedArgs = expectedArgs,
                        Callback = cb
                    }
                );
            }
        }

        public IEnumerable<ConsoleCommand> GetCommands(string command)
        {
            lock (_mut)
            {
                return Commands.Where(z => z.Command == command.Split(" ")[0]);
            }
        }

        public void Start()
        {
            if (_consoleThread?.IsAlive ?? false)
                return;
            _shouldStop = false;
            _consoleThread = new Thread(
                () =>
                {
                    var x = string.Empty;

                    while (!_shouldStop)
                    {
                        var k = Colorful.Console.ReadKey();
                        Colorful.Console.SetCursorPosition(0, Colorful.Console.CursorTop);
                        Console.Write(x);

                        switch (k.Key)
                        {
                            case ConsoleKey.Enter:
                                var cmds = GetCommands(x);
                                var x1 = x;
                                new Thread(
                                    () =>
                                    {
                                        IEnumerable<ConsoleCommand> consoleCommands =
                                            cmds as ConsoleCommand[] ?? cmds.ToArray();
                                        if (!consoleCommands.ToList().Any())
                                            Logger.Err(
                                                "Command not found! type %#f1fc5a%help %#FFFFFF%for a Command List!"
                                            );
                                        foreach (var m in consoleCommands)
                                        {
                                            var l = x1.Split(" ").ToList();
                                            l.RemoveAt(0);
                                            if (l.Count < m.ExpectedArgs)
                                            {
                                                var aList = "\t<";

                                                var i = 0;
                                                foreach (var a in m.Args)
                                                {
                                                    aList += a.ArgName;
                                                    if (i >= m.ExpectedArgs)
                                                        aList += "?";
                                                    aList += ", ";
                                                    i++;
                                                }

                                                aList = aList.TrimEnd(m.Args.Count < 1 ? '<' : ' ').TrimEnd(',');
                                                if (m.Args.Count > 0)
                                                    aList += ">";

                                                Logger.Err(
                                                    "Insufficient amount of Arguments!\nUsage: " + m.Command + aList
                                                );
                                                break;
                                            }

                                            if (m.Callback(l.ToArray()))
                                                break;
                                        }
                                    }
                                ).Start();

                                x = "";
                                Colorful.Console.Write(new string(' ', Colorful.Console.WindowWidth));
                                Colorful.Console.SetCursorPosition(0, Colorful.Console.CursorTop - 1);
                                break;
                            case ConsoleKey.Backspace:
                                if (x.Length == 0)
                                    break;
                                x = x.Remove(x.Length - 1);
                                Colorful.Console.SetCursorPosition(
                                    Colorful.Console.CursorLeft - 1, Colorful.Console.CursorTop
                                );
                                Console.Write(" ");
                                break;
                            default:
                                x += k.KeyChar;
                                Console.Write(k.KeyChar);
                                break;
                        }

                        Colorful.Console.Write(new string(' ', Colorful.Console.WindowWidth));
                        Colorful.Console.SetCursorPosition(0, Colorful.Console.CursorTop - 1);
                    }
                }
            );

            _consoleThread.Start();
        }

        public void Stop()
        {
            _shouldStop = true;
            _consoleThread?.Join();
        }
    }
}
