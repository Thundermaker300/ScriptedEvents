namespace ScriptedEvents.Actions
{
    using System;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Handlers;

    public class DebugMathAction : IScriptAction, IHiddenAction
    {
        /// <inheritdoc/>
        public string Name => "DEBUGMATH";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Debug;

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string formula = ConditionVariables.ReplaceVariables(string.Join(" ", Arguments));
            if (!ConditionHelper.TryMath(formula, out MathResult result))
            {
                return new(MessageType.NotANumberOrCondition, this, "condition", formula, result);
            }

            return new(true, result.Result.ToString());
        }
    }
}