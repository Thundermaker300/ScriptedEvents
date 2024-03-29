﻿namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class PrintAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "PRINT";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

        /// <inheritdoc/>
        public string Description => "Creates a log message in the console the script was executed from. Variables are supported.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("message", typeof(string), "The message.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string message = VariableSystem.ReplaceVariables(string.Join(" ", Arguments), script);

            if (script.Sender is null || script.Context is ExecuteContext.Automatic)
            {
                Log.Info(message);
                return new(true);
            }

            if (script.Context is ExecuteContext.PlayerConsole)
            {
                Player.Get(script.Sender)?.SendConsoleMessage(message, "green");
                return new(true);
            }

            script.Sender.Respond(message);

            return new(true);
        }
    }
}