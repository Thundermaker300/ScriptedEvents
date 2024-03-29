﻿namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class TpRoomAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "TPROOM";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Teleports players to the specified room center.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to teleport", true),
            new Argument("room", typeof(RoomType), "The room to teleport to. Alternatively, a zone can be provided to teleport players to a random room in the zone (random for each player). Do NOT use Scp173 room!!!", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out PlayerCollection players, script))
                return new(false, players.Message);

            if (VariableSystem.TryParse(Arguments[1], out ZoneType zt, script))
            {
                List<Room> validRooms = Room.List.Where(r => r.Zone.HasFlag(zt) && r.Type is not RoomType.Lcz173 && r.Type is not RoomType.Hcz079).ToList();
                foreach (Player ply in players)
                {
                    validRooms.ShuffleList();
                    if (ply.Role is not FpcRole || !ply.IsConnected) continue;
                    ply.Teleport(validRooms[UnityEngine.Random.Range(0, validRooms.Count)]);
                }

                return new(true);
            }

            if (!VariableSystem.TryParse(Arguments[1], out RoomType rt, script))
                return new(false, $"Invalid room: {Arguments[1]}");

            foreach (Player ply in players)
            {
                if (ply.Role is not FpcRole || !ply.IsConnected) continue;
                ply.Teleport(rt);
            }

            return new(true);
        }
    }
}
