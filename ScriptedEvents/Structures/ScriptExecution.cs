namespace ScriptedEvents.Structures
{
    using MEC;

    public readonly struct ScriptExecution
    {
        public ScriptExecution(Script script, CoroutineHandle handle, int runId)
        {
            Script = script;
            Handle = handle;
            RunId = runId;
        }

        public Script Script { get; }

        public CoroutineHandle Handle { get; }

        public int RunId { get; }
    }
}
