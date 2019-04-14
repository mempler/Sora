using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Shared.Helpers;
using Console = Colorful.Console;

namespace Sora.Services
{
    public struct ConsoleCommand
    {
        public delegate bool ConsoleCommandExecution(string[] args);
        
        public string Command;
        public ConsoleCommandExecution Callback;
    }
    
    public class ConsoleCommandService
    {
        private List<ConsoleCommand> _commands;
        private Thread _consoleThread;
        private readonly object _mut;
        private bool _shouldStop;

        public ConsoleCommandService()
        {
            _commands = new List<ConsoleCommand>();
            _mut = new object();
        }      
        
        public void RegisterCommand(string Command, ConsoleCommand.ConsoleCommandExecution cb)
        {
            lock(_mut)
            _commands.Add(new ConsoleCommand
            {
                Command = Command,
                Callback = cb
            });
        }

        public void Start()
        {
            if (_consoleThread?.IsAlive ?? false)
                return;
            _shouldStop = false;
            _consoleThread = new Thread(() =>
            {
                string x = string.Empty;
                int cur;
                
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
                                string x1 = x;
                                IEnumerable<ConsoleCommand> cmds = _commands.Where(z => z.Command == x1.Split(" ")[0]);

                                new Thread(() =>
                                {
                                    IEnumerable<ConsoleCommand> consoleCommands = cmds as ConsoleCommand[] ?? cmds.ToArray();
                                    if (!consoleCommands.ToList().Any())
                                        Logger.Err("Command not found! type %#f1fc5a%help %#FFFFFF%for a Command List!");
                                    foreach (ConsoleCommand m in consoleCommands)
                                    {
                                        List<string> l = x1.Split(" ").ToList();
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