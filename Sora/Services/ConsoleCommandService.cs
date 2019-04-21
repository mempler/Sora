using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Shared.Enums;
using Shared.Helpers;
using Shared.Models;
using Shared.Services;
using Console = Colorful.Console;

namespace Sora.Services
{
    public struct Argument
    {
        public string ArgName;
        public bool Optional;
    }
    
    public struct ConsoleCommand
    {
        public delegate bool ConsoleCommandExecution(string[] args);
        
        public string Command;
        public string Description;
        public List<Argument> Args;
        public bool ConsoleOnly;
        public ConsoleCommandExecution Callback;
    }
    
    public class ConsoleCommandService
    {
        private List<ConsoleCommand> _commands;
        private Thread _consoleThread;
        private readonly object _mut;
        private bool _shouldStop;

        public List<ConsoleCommand> Commands => _commands;

        public ConsoleCommandService(Database db)
        {
            _commands = new List<ConsoleCommand>();
            _mut = new object();
            
            #region DefaultCommands
            RegisterCommand("help", 
                            "Get a list of ALL Commands!", 
                            new List<Argument>
                            {
                                new Argument
                                {
                                    ArgName = "command",
                                    Optional = true
                                }
                            }, 
            args =>
            {
                string l = "\n====== Command List ======\n";
                
                foreach (ConsoleCommand cmd in _commands)
                {
                    string aList = "\t<";
                    
                    foreach (Argument a in cmd.Args)
                    {
                        aList += a.ArgName;
                        if (a.Optional)
                            aList += "?";
                        aList += ", ";
                    }

                    aList = aList.TrimEnd(cmd.Args.Count < 1 ? '<' : ' ').TrimEnd(',');
                    if (cmd.Args.Count > 0)
                        aList += ">";

                    l += "\n%#1c9624%/*\n";
                    l += cmd.Description;
                    l += "\n*/%#FFFFFF%\n";
                    l += "\n" +  cmd.Command + aList;
                    l += "\n";
                }

                l += "\n==========================";
                
                Logger.Info(l);

                return true;
            });

            RegisterCommand("clear",
                "Clear Console",
                new List<Argument>(),
                args =>
                {
                    lock (Logger.Locker)
                    {
                        System.Console.Clear();
                    }
                    return true;
                });
            
            RegisterCommand("adduser",
                "Add a User to our Database",
                new List<Argument>
                {
                    new Argument
                    {
                        ArgName = "Username",
                        Optional = false
                    },
                    new Argument
                    {
                        ArgName  = "Password",
                        Optional = false
                    },
                    new Argument
                    {
                        ArgName  = "E-Mail",
                        Optional = false
                    },
                    new Argument
                    {
                        ArgName  = "Privileges",
                        Optional = true
                    }
                },
                args =>
                {
                    Users u = Users.NewUser(db, args[0], args[1], args[2], (Privileges) (args.Length > 3 ? Convert.ToInt32(args[3]) : 0));
                    Logger.Info("Created User",
                                "%#F94848%" + u.Username,
                                "%#B342F4%(", Users.GetUserId(db, u.Username), "%#B342F4%)");
                    return true;
                });
            #endregion
        }      
        
        public void RegisterCommand(string Command, string Description, List<Argument> args, ConsoleCommand.ConsoleCommandExecution cb)
        {
            lock(_mut)
            _commands.Add(new ConsoleCommand
            {
                Command = Command,
                Description = Description,
                Args = args,
                Callback = cb
            });
        }

        public IEnumerable<ConsoleCommand> GetCommands(string Command)
        {
            lock (_mut)
            return _commands.Where(z => z.Command == Command.Split(" ")[0]);
        }

        public void Start()
        {
            if (_consoleThread?.IsAlive ?? false)
                return;
            _shouldStop = false;
            _consoleThread = new Thread(() =>
            {
                string x = string.Empty;
                
                while (!_shouldStop)
                {
                    ConsoleKeyInfo k = Console.ReadKey();
                    lock (Logger.Locker)
                    {
                        Console.SetCursorPosition(0, Console.CursorTop);
                        System.Console.Write(x);
                        
                        switch (k.Key)
                        {
                            case ConsoleKey.Enter:
                                IEnumerable<ConsoleCommand> cmds = GetCommands(x);
                                string x1 = x;
                                new Thread(() =>
                                {
                                    IEnumerable<ConsoleCommand> consoleCommands = cmds as ConsoleCommand[] ?? cmds.ToArray();
                                    if (!consoleCommands.ToList().Any())
                                        Logger.Err("Command not found! type %#f1fc5a%help %#FFFFFF%for a Command List!");
                                    foreach (ConsoleCommand m in consoleCommands)
                                    {
                                        List<string> l = x1.Split(" ").ToList();
                                        l.RemoveAt(0);
                                        if (l.Count < m.Args.Count - m.Args.Sum(op => Convert.ToInt32(op.Optional)))
                                        {
                                            string aList = "\t<";

                                            foreach (Argument a in m.Args)
                                            {
                                                aList += a.ArgName;
                                                if (a.Optional)
                                                    aList += "?";
                                                aList += ", ";
                                            }

                                            aList = aList.TrimEnd(m.Args.Count < 1 ? '<' : ' ').TrimEnd(',');
                                            if (m.Args.Count > 0)
                                                aList += ">";
                                            
                                            Logger.Err("Insufficient amount of Arguments!\nUsage: " +m.Command + aList);
                                            break;
                                        }
                                        if (m.Callback(l.ToArray()))
                                            break;
                                    }
                                }).Start();
                                
                                x = "";
                                Console.Write(new string(' ', Console.WindowWidth));
                                Console.SetCursorPosition(0, Console.CursorTop - 1);
                                break;
                            case ConsoleKey.Backspace:
                                if (x.Length == 0)
                                    break;
                                x = x.Remove(x.Length - 1);
                                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                                System.Console.Write(" ");
                                break;
                            default:
                                x += k.KeyChar;
                                System.Console.Write(k.KeyChar);
                                break;
                        }

                        Console.Write(new string(' ', Console.WindowWidth));
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                    }
                }
            });

            _consoleThread.Start();
        }

        public void Stop()
        {
            _shouldStop = true;
            _consoleThread?.Join();
        }
    }
}