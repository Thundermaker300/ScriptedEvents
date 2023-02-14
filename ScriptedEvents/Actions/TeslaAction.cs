﻿namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using MEC;
    using PlayerRoles;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
    using UnityEngine;
    using Tesla = Exiled.API.Features.TeslaGate;

    public class TeslaAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "TESLA";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Modifies tesla gates.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The mode to run. Valid options: PLAYERS, ROLETYPE, DISABLE, ENABLE", true),
            new Argument("target", typeof(object), "The targets. Different type based on the mode.\nPLAYERS: A list of players.\nROLETYPE: A valid RoleType (eg. ClassD, Scp173, etc)\nDISABLE & ENABLE: None", true),
            new Argument("duration", typeof(float), "The time before reversing the affect.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2)
            {
                if (Arguments.Length < 1 || Arguments[0] != "DISABLE")
                {
                    return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);
                }
            }

            string mode = Arguments[0];
            string target = mode == "DISABLE" ? null : Arguments[1];
            string duration = Arguments.Length > 2 ? string.Join(string.Empty, Arguments.Skip(2)) : null;

            switch (mode)
            {
                case "PLAYERS":
                    if (!ScriptHelper.TryGetPlayers(target, null, out Player[] players))
                        return new(MessageType.NoPlayersFound, this, "players");

                    foreach (Player player in players)
                    {
                        if (!Tesla.IgnoredPlayers.Contains(player))
                            Tesla.IgnoredPlayers.Add(player);
                    }

                    return Reverse(mode, players, duration);
                case "ROLETYPE":
                    if (!Enum.TryParse(target, out RoleTypeId roleType))
                        return new(MessageType.InvalidRole, this, "target", target);

                    if (!Tesla.IgnoredRoles.Contains(roleType))
                        Tesla.IgnoredRoles.Add(roleType);
                    return Reverse(mode, roleType, duration);
                case "DISABLE":
                    duration = Arguments.Length > 1 ? string.Join(string.Empty, Arguments.Skip(1)) : null;
                    MainPlugin.Handlers.TeslasDisabled = true;
                    return Reverse(mode, null, duration);
                case "ENABLE":
                    duration = Arguments.Length > 1 ? string.Join(string.Empty, Arguments.Skip(1)) : null;
                    MainPlugin.Handlers.TeslasDisabled = false;
                    return Reverse(mode, null, duration);
                default:
                    return new(MessageType.InvalidOption, this, "mode", mode, "PLAYERS/ROLETYPE/DISABLE/ENABLE");
            }
        }

        /// <summary>
        /// Reverses a previous action.
        /// </summary>
        /// <param name="mode">The mode to reverse.</param>
        /// <param name="target">The targeted objects.</param>
        /// <param name="duration">The duration until reversing.</param>
        /// <returns>An <see cref="ActionResponse"/> indicating success of the reverse.</returns>
        public ActionResponse Reverse(string mode, object target, string duration)
        {
            if (duration is null || string.IsNullOrWhiteSpace(duration))
                return new(true);

            float floatDuration;

            if (!ConditionHelper.TryMath(duration, out MathResult result))
            {
                return new(MessageType.NotANumberOrCondition, this, "duration", duration, result);
            }

            if (result.Result < 0)
            {
                return new(MessageType.LessThanZeroNumber, this, "duration", result.Result);
            }

            floatDuration = result.Result;

            Timing.CallDelayed(floatDuration, () =>
            {
                switch (mode)
                {
                    case "PLAYERS":
                        foreach (Player player in (List<Player>)target)
                        {
                            if (Tesla.IgnoredPlayers.Contains(player))
                                Tesla.IgnoredPlayers.Remove(player);
                        }

                        break;
                    case "ROLETYPE":
                        RoleTypeId roleType = (RoleTypeId)target;
                        if (Tesla.IgnoredRoles.Contains(roleType))
                            Tesla.IgnoredRoles.Remove(roleType);
                        break;
                    case "DISABLE":
                        MainPlugin.Handlers.TeslasDisabled = false;
                        break;
                    case "ENABLE":
                        MainPlugin.Handlers.TeslasDisabled = true;
                        break;
                }
            });

            return new(true);
        }
    }
}
