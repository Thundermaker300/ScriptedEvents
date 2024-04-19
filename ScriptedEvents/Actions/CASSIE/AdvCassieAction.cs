﻿namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Extensions;
    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class AdvCassieAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "ADVCASSIE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Cassie;

        /// <inheritdoc/>
        public string Description => "Makes a cassie announcement with specified advanced settings.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to play the CASSIE announcement for.", true),
            new Argument("delay", typeof(bool), "If FALSE, CASSIE will make the broadcast as soon as possible. The subtitles may be out of sync as result.", true),
            new Argument("isLoud", typeof(bool), "If FALSE, CASSIE will not make the jingle sound.", true),
            new Argument("hasSubtitles", typeof(bool), "If FALSE, subtitles will not be shown.", true),
            new Argument("message", typeof(string), "The message. Separate message with a | to specify a caption.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[0];

            bool waitToFinish = (bool)Arguments[1];
            bool isLoud = (bool)Arguments[2];
            bool hasSubtitles = (bool)Arguments[3];
            string text = Arguments.JoinMessage(4);

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
                foreach (Player ply in players)
                {
                    ply.MessageTranslated(text, text, waitToFinish, isLoud, hasSubtitles);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(cassieArgs[0]))
                    return new(MessageType.CassieCaptionNoAnnouncement, this, "message");

                if (string.IsNullOrWhiteSpace(cassieArgs[1]))
                {
                    foreach (Player ply in players)
                    {
                        ply.PlayCassieAnnouncement(cassieArgs[0], waitToFinish, isLoud, hasSubtitles);
                    }
                }
                else
                {
                    foreach (Player ply in players)
                    {
                        ply.MessageTranslated(cassieArgs[0], cassieArgs[1], waitToFinish, isLoud, hasSubtitles);
                    }
                }
            }

            return new(true, string.Empty);
        }
    }
}
