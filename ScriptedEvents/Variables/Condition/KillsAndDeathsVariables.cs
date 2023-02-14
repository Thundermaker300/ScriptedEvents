﻿namespace ScriptedEvents.Variables.Condition.KillsAndDeaths
{
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    public class KillsAndDeathsVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public VariableGroupType GroupType => VariableGroupType.Condition;

        /// <inheritdoc/>
        public IVariable[] Variables => new IVariable[]
        {

        };
    }

    public class Kills : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{KILLS}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => Round.Kills;
    }

    public class ScpKills : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{SCPKILLS}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => Round.KillsByScp;
    }
}
