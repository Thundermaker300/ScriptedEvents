namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;
    using MEC;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Handlers;
    using UnityEngine;

    public class WaitForSecondsAction : ITimingAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "WAITSEC";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Yielding;

        /// <inheritdoc/>
        public string Description => "Yields execution of the script for the given number of seconds.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("seconds", typeof(float), "The amount of seconds. Variables & Math are supported.", true),
        };

        /// <inheritdoc/>
        public float? Execute(Script scr, out ActionResponse message)
        {
            if (Arguments.Length < 1)
            {
                message = new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);
                return null;
            }

            string formula = ConditionVariables.ReplaceVariables(string.Join(" ", Arguments));

            if (!ConditionHelper.TryMath(formula, out MathResult result))
            {
                message = new(MessageType.NotANumberOrCondition, this, "duration", formula, result);
                return null;
            }

            if (result.Result < 0)
            {
                message = new(MessageType.LessThanZeroNumber, this, "duration", result.Result);
                message = new(false, "A negative number cannot be used as the duration argument of the WAITSEC action.");
                return null;
            }

            message = new(true);
            return Timing.WaitForSeconds(result.Result);
        }
    }
}
