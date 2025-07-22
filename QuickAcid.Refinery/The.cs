using QuickAcid.Proceedings;
using QuickPulse;
using QuickPulse.Bolts;

namespace QuickAcid.Refinery;

public static class The
{
    private static string Capitalize(string input) => char.ToUpper(input[0]) + input[1..];

    private readonly static Flow<Unit> space = Pulse.Trace(" ");
    private readonly static Flow<Unit> newLine = Pulse.Trace(Environment.NewLine);

    private readonly static Flow<Unit> indent =
        from cast in Pulse.Gather<Cast>()
        let indentString = new string(' ', cast.Value.Level * 4)
        from trace in Pulse.TraceIf<Cast>(a => a.NeedsIndent, () => indentString)
        select Unit.Instance;

    private static Flow<Unit> Indented(string str) => indent.Then(Pulse.Trace(str));

    private readonly static Flow<Unit> Separator = Pulse.Trace(",");
    private readonly static Flow<Unit> Colon = Pulse.Trace(": ");
    private readonly static Flow<Unit> Null = Indented("null");

    private static Flow<Unit> Enclosed(string left, string right, Flow<Unit> innerFlow) =>
        from leftBracket in Indented(left).Then(newLine)
        from _ in Pulse.Scoped<Cast>(a => a.IncreaseLevel().EnableIndent(), innerFlow)
        from __ in Pulse.Scoped<Cast>(a => a.EnableIndent(), Indented(right))
        select Unit.Instance;

    private static Flow<Unit> Braced(Flow<Unit> innerFlow) => Enclosed("{", "}", innerFlow);
    private static Flow<Unit> Bracketed(Flow<Unit> innerFlow) => Enclosed("(", ")", innerFlow);
    private static Flow<Unit> SquareBracketed(Flow<Unit> innerFlow) => Enclosed("[", "]", innerFlow);


    private static string FormatMethodName(string specName) => specName.Replace(" ", "_");

    private readonly static Flow<TrackedDeposition> trackedDeposition =
        from input in Pulse.Start<TrackedDeposition>()
        from _ in indent.Then(Pulse.Trace($"var {input.Label} = new {Capitalize(input.Label)}();")).Then(newLine)
        select input;

    private readonly static Flow<ExecutionDeposition> executionDeposition =
        from input in Pulse.Start<ExecutionDeposition>()
        from _ in Pulse.ToFlow(trackedDeposition, input.TrackedDepositions)
            // from __ in line
            // from ___ in Pulse.Trace($"  Executed ({input.ExecutionId}):")
            // from ____ in Pulse.Scoped<Decorum>(
            //     a => a with { Intersperse = Valve.Install() },
            //     Pulse.ToFlow(actionDeposition, input.ActionDepositions))
            // from _____ in Pulse.ToFlow(inputDeposition, input.InputDepositions)
        select input;

    private readonly static Flow<ExecutionDeposition> maybeExecutionDeposition =
        from input in Pulse.Start<ExecutionDeposition>()
            //from _ in Pulse.Trace("var account = new Account();")
        from _ in Pulse.ToFlowIf(input.HasContent(), executionDeposition, () => input)
        select input;

    private readonly static Flow<FailedSpecDeposition> failedSpecDeposition =
        from input in Pulse.Start<FailedSpecDeposition>()
        from _2 in Pulse.Trace($"public void {FormatMethodName(input.FailedSpec)}()").Then(newLine)
        select input;

    private readonly static Flow<ExceptionDeposition> exceptionDeposition =
        from input in Pulse.Start<ExceptionDeposition>()
        select input;

    private readonly static Flow<FailureDeposition> failureDeposition =
        from input in Pulse.Start<FailureDeposition>()
        from _1 in Pulse.ToFlowIf(input is FailedSpecDeposition, failedSpecDeposition, () => (FailedSpecDeposition)input)
        from _2 in Pulse.ToFlowIf(input is ExceptionDeposition, exceptionDeposition, () => (ExceptionDeposition)input)
        select input;
    private readonly static Flow<Verdict?> verdict =
        from verdict in Pulse.Start<Verdict>()
        from _1 in indent.Then(Pulse.Trace("[Fact]")).Then(newLine)
        from _ in indent.Then(Pulse.ToFlow(failureDeposition, verdict.FailureDeposition))
        from _3 in Braced(Pulse.ToFlow(maybeExecutionDeposition, verdict.ExecutionDepositions)).Then(newLine)
        select verdict;

    public readonly static Flow<CaseFile> Anvil =
        from input in Pulse.Start<CaseFile>()
        from _ in Pulse.Gather(new Cast())
        from _1 in Pulse.Trace("namespace Refined.By.QuickAcid;").Then(newLine)
        from _2 in newLine
        from _3 in Pulse.Trace("public class UnitTests").Then(newLine)
        from _4 in Braced(Pulse.ToFlow(verdict, input.Verdict))
        select input;
}
