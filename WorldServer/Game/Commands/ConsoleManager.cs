using System;
using System.Collections.Generic;
using System.Threading;
using Common.Logging;
using WorldServer.Game.Objects;

namespace WorldServer.Game.Commands
{
    public class ConsoleManager
    {
        public static Dictionary<string, HandleCommand> CommandHandlers = new Dictionary<string, HandleCommand>();
        public delegate void HandleCommand(Player player, string[] args);

        public static void InitCommands()
        {
            LoadCommandDefinitions();

            while (true)
            {
                Thread.Sleep(1);
                Log.Message(LogType.CMD, "Alpha WoW >> ");

                InvokeHandler(Console.ReadLine(), null);
            }
        }

        public static void DefineCommand(string command, HandleCommand handler)
        {
            CommandHandlers[command.ToLower()] = handler;
        }

        public static bool InvokeHandler(string command, Player player)
        {
            string[] lines = command.Split(' ');
            string[] args = new string[lines.Length - 1];
            Array.Copy(lines, 1, args, 0, lines.Length - 1);
            return InvokeHandler(lines[0].ToLower(), player, args);         
        }

        public static bool InvokeHandler(string command, Player player, params string[] args)
        {
            command = command.TrimStart('.'); //In game commands forced dot
            if (CommandHandlers.ContainsKey(command))
            {
                CommandHandlers[command].Invoke(player, args);
                return true;
            }
            else
                return false;
        }

        public static void LoadCommandDefinitions()
        {
            DefineCommand("additem", GameMasterCommands.AddItem);
            DefineCommand("addskill", GameMasterCommands.AddSkill);
            DefineCommand("setskill", GameMasterCommands.SetSkill);
            DefineCommand("kill", GameMasterCommands.Kill);
            DefineCommand("level", GameMasterCommands.SetLevel);
            DefineCommand("kick", GameMasterCommands.Kick);
            DefineCommand("money", GameMasterCommands.Money);
            DefineCommand("setpower", GameMasterCommands.SetPower);
        }
    }
}
