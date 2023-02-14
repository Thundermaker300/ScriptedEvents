﻿namespace ScriptedEvents.Variables.Condition.Escapes
{
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    public class EscapesVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public VariableGroupType GroupType => VariableGroupType.Condition;

        /// <inheritdoc/>
        public IVariable[] Variables => new IVariable[]
        {
            new Escapes(),
            new ClassDEscapes(),
            new ScientistEscapes(),
        };
    }

    public class Escapes : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{ESCAPES}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => Round.EscapedDClasses + Round.EscapedScientists;
    }

    public class ClassDEscapes : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{CLASSDESCAPES}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => Round.EscapedDClasses;
    }

    public class ScientistEscapes : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{SCIENTISTESCAPES}";

        /// <inheritdoc/>
        public string Description => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public float Value => Round.EscapedScientists;
    }
}
