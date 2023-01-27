﻿using Exiled.API.Features;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.API.Helpers;
using System;

namespace ScriptedEvents.Actions
{
    public class TurnOffLightsAction : IAction
    {
        public string Name => "LIGHTSOFF";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            if (Arguments.Length < 1) return new(false, "Missing argument: duration");
            if (!ScriptHelper.TryConvertNumber(Arguments[0], out int duration))
                return new(false, "First argument must be an int or range of ints!");

            foreach (var room in Room.List)
                room.TurnOffLights(duration);

            return new(true, string.Empty);
        }
    }
}