﻿namespace ScriptedEvents.Commands.MainCommand
{
    using System;
    using System.Linq;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using RemoteAdmin;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Action : ICommand
    {
        /// <inheritdoc/>
        public string Command => "action";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "act" };

        /// <inheritdoc/>
        public string Description => "Runs a specific action with specific arguments.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("script.action"))
            {
                response = "Missing permission: script.action";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Missing the name of the action to execute.";
                return false;
            }

            string actionName = arguments.ElementAt(0);
            if (string.IsNullOrWhiteSpace(actionName))
            {
                response = "Missing the name of the action to execute.";
                return false;
            }

            if (!MainPlugin.ScriptModule.TryGetActionType(actionName.ToUpper(), out Type argType))
            {
                response = "Invalid argument name provided.";
                return false;
            }

            IAction action = Activator.CreateInstance(argType) as IAction;

            if (action is not IScriptAction scriptAction)
            {
                response = "This action cannot be executed.";
                return false;
            }

            scriptAction.RawArguments = arguments.Skip(1).ToArray();
            scriptAction.Arguments = arguments.Skip(1).ToArray();

            // Fill out mock script info
            Script mockScript = new()
            {
                Context = sender is ServerConsoleSender ? ExecuteContext.ServerConsole : ExecuteContext.RemoteAdmin,
                Sender = sender,
                RawText = string.Join(" ", arguments),
                ScriptName = "ACTION COMMAND EXECUTION",
                Actions = new[] { scriptAction },
            };

            mockScript.OriginalActionArgs[action] = scriptAction.RawArguments;
            if (sender is PlayerCommandSender playerSender && Player.TryGet(playerSender, out Player plr))
            {
                mockScript.AddPlayerVariable("{SENDER}", "The player who executed the script.", new[] { plr });
            }

            if (MainPlugin.Configs.Debug)
                mockScript.AddFlag("DEBUG");

            mockScript.AddFlag("ACTIONCOMMANDEXECUTION");

            try
            {
                MainPlugin.ScriptModule.RunScript(mockScript);
            }
            catch (Exception ex)
            {
                response = $"Error while running action: {ex.Message}";
                mockScript.Dispose();
                return false;
            }

            response = "Done";
            return true;
        }
    }
}
