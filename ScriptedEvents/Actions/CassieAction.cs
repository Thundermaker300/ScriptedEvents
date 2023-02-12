﻿namespace ScriptedEvents.Actions
{
    using System;
    using Exiled.API.Features;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class CassieAction : IScriptAction, IHelpInfo
    {
        public string Name => "CASSIE";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Makes a loud cassie announcement.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("message", typeof(string), "The message. Separate message with a | to specify a caption. Variables are supported.", true),
        };

        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, ExpectedArguments);

            string text = string.Join(" ", Arguments);

            if (string.IsNullOrWhiteSpace(text))
                return new(MessageType.InvalidUsage, this, null, ExpectedArguments);

            string[] cassieArgs = text.Split('|');

            for (int i = 0; i < cassieArgs.Length; i++)
            {
                cassieArgs[i] = ConditionVariables.ReplaceVariables(cassieArgs[i]);
            }

            if (cassieArgs.Length == 1)
            {
                text = ConditionVariables.ReplaceVariables(text);
                Cassie.MessageTranslated(text, text);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(cassieArgs[0]))
                    return new(MessageType.CassieCaptionNoAnnouncement, this, "message");

                if (string.IsNullOrWhiteSpace(cassieArgs[1]))
                    Cassie.Message(cassieArgs[0]);
                else
                    Cassie.MessageTranslated(cassieArgs[0], cassieArgs[1]);
            }

            return new(true, string.Empty);
        }
    }
}
