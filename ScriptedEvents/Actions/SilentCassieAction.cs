﻿namespace ScriptedEvents.Actions
{
    using System;
    using Exiled.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class SilentCassieAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "SILENTCASSIE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Cassie;

        /// <inheritdoc/>
        public string Description => "Makes a silent cassie announcement.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("message", typeof(string), "The message. Separate message with a | to specify a caption. Variables are supported.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            string text = string.Join(" ", Arguments);

            if (string.IsNullOrWhiteSpace(text))
                return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            string[] cassieArgs = text.Split('|');

            for (int i = 0; i < cassieArgs.Length; i++)
            {
                cassieArgs[i] = VariableSystem.ReplaceVariables(cassieArgs[i], script);
            }

            if (cassieArgs.Length == 1)
            {
                text = VariableSystem.ReplaceVariables(text, script);
                Cassie.MessageTranslated(text, text, isNoisy: false);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(cassieArgs[0]))
                    return new(MessageType.CassieCaptionNoAnnouncement, this, "message");

                if (string.IsNullOrWhiteSpace(cassieArgs[1]))
                    Cassie.Message(cassieArgs[0], isNoisy: false);
                else
                    Cassie.MessageTranslated(cassieArgs[0], cassieArgs[1], isNoisy: false);
            }

            return new(true, string.Empty);
        }
    }
}
