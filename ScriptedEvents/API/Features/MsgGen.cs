﻿namespace ScriptedEvents.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Pools;

    using PlayerRoles;
    using Respawning;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    /// <summary>
    /// Tool to generate error messages and strings, for consistency between all actions.
    /// </summary>
    public static class MsgGen
    {
        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> of types and a string to show for them.
        /// </summary>
        public static Dictionary<Type, string> TypeToString { get; } = new()
        {
            { typeof(string), "String (Message/Text)" },
            { typeof(char), "Character" },
            { typeof(int), "Int (Whole Number)" },
            { typeof(byte), "Byte (Whole Number, 0-255)" },
            { typeof(float), "Float (Number)" },
            { typeof(bool), "Boolean (TRUE/FALSE)" },
            { typeof(Player[]), "Player List" },
            { typeof(List<Player>), "Player List" },
            { typeof(Door[]), "Door List" },
            { typeof(List<Door>), "Door List" },
            { typeof(RoleTypeId), "RoleTypeId (ID / Number)" },
            { typeof(SpawnableTeamType), "Spawnable Team (ChaosInsurgency OR NineTailedFox)" },
            { typeof(RoomType), "RoomType (ID / Number)" },
            { typeof(IVariable), "Variable" },
            { typeof(IPlayerVariable), "Player Variable" },
            { typeof(IConditionVariable), "Condition Variable" },
            { typeof(IStringVariable), "String (Message/Text) Variable" },
            { typeof(IFloatVariable), "Numerical Variable" },
            { typeof(ILongVariable), "Numerical Variable" },
            { typeof(object), "Any Type" },
        };

        /// <summary>
        /// Generates an error message, based on provided input.
        /// </summary>
        /// <param name="type">The type of message to show.</param>
        /// <param name="action">The action currently executing.</param>
        /// <param name="paramName">The name of the parameter that is causing a skill issue.</param>
        /// <param name="arguments">The arguments of the MessageType. See <see cref="ActionResponse.ActionResponse(MessageType, IAction, string, object[])"/> for documentation on what MessageTypes require what arguments.</param>
        /// <returns>The string to display to the user.</returns>
        public static string Generate(MessageType type, IAction action, string paramName, params object[] arguments)
        {
            switch (type)
            {
                case MessageType.OK:
                    return "OK";

                case MessageType.InvalidUsage when arguments[0] is Argument[] argList:
                    StringBuilder sb = StringBuilderPool.Pool.Get();
                    foreach (Argument arg in argList)
                    {
                        string[] chars = arg.Required ? new[] { "<", ">" } : new[] { "[", "]" };
                        sb.Append($" {chars[0]}{arg.ArgumentName}{chars[1]}");
                    }

                    return ErrorGen.Get(116, action.Name, action.Name + StringBuilderPool.Pool.ToStringReturn(sb));

                case MessageType.InvalidUsage:
                    return ErrorGen.Get(117, action.Name);

                case MessageType.InvalidOption when arguments[0] is string input && arguments[1] is string options:
                    return ErrorGen.Get(118, input, paramName, action.Name, options);

                case MessageType.NotANumber when arguments[0] is not null:
                    return ErrorGen.Get(119, arguments[0], paramName, action.Name);

                case MessageType.NotANumberOrCondition when arguments[0] is not null && arguments[1] is MathResult result:
                    return ErrorGen.Get(120, paramName, action.Name, arguments[0], result.Exception.GetType().Name, result.Message);

                case MessageType.LessThanZeroNumber when arguments[0] is not null:
                    return ErrorGen.Get(121, arguments[0], paramName, action.Name);

                case MessageType.InvalidRole when arguments[0] is not null:
                    return ErrorGen.Get(122, paramName, action.Name, arguments[0]);

                case MessageType.NoPlayersFound:
                    return ErrorGen.Get(123, paramName);

                case MessageType.NoRoomsFound:
                    return ErrorGen.Get(124, arguments[0], paramName);

                case MessageType.CassieCaptionNoAnnouncement:
                    return ErrorGen.Get(125);
            }

            return ErrorGen.Get(126);
        }

        /// <summary>
        /// Returns a string showing the amount of arguments required, given the arguments.
        /// </summary>
        /// <param name="name">The name of the action or variable.</param>
        /// <param name="args">The required arguments.</param>
        /// <returns>Formatted string to show to end-user.</returns>
        public static string VariableArgCount(string name, params string[] args)
        {
            return ErrorGen.Get(130, name, args.Length, string.Join(", ", args));
        }

        /// <summary>
        /// Gets a pretty display for a type.
        /// </summary>
        /// <param name="type">The type to get the display of.</param>
        /// <returns>The display.</returns>
        public static string Display(this Type type)
        {
            if (TypeToString.TryGetValue(type, out string display))
                return display;

            return type.Name;
        }

        /// <summary>
        /// Gets a pretty display for a <see cref="ActionSubgroup"/>.
        /// </summary>
        /// <param name="group">The <see cref="ActionSubgroup"/>.</param>
        /// <returns>The display.</returns>
        public static string Display(this ActionSubgroup group)
        {
            return group switch
            {
                ActionSubgroup.Cassie => "C.A.S.S.I.E",
                ActionSubgroup.Misc => "Miscellaneous",
                ActionSubgroup.RoundRule => "Round Rule",
                _ => group.ToString(),
            };
        }
    }
}
