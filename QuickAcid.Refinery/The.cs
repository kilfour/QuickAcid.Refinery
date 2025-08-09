using QuickAcid.Proceedings;
using QuickPulse;
using QuickPulse.Bolts;

namespace QuickAcid.Refinery;

public static class The
{
    private static string CapitalizeFirstLetter(string input) => char.ToUpper(input[0]) + input[1..];
    private static string DecapitalizeFirstLetter(string input) => char.ToLower(input[0]) + input[1..];

    private readonly static Flow<Unit> space = Pulse.Trace(" ");
    private readonly static Flow<Unit> newLine = Pulse.Trace(Environment.NewLine);

    private readonly static Flow<Unit> indent =
        from cast in Pulse.Gather<Cast>()
        let indentString = new string(' ', cast.Value.Level * 4)
        from trace in Pulse.TraceIf<Cast>(a => a.NeedsIndent, () => indentString)
        select Unit.Instance;

    private static Flow<Unit> Indented(string str) => indent.Then(Pulse.Trace(str));

    private readonly static Flow<Unit> Separator = Pulse.Trace(",");
    private readonly static Flow<Unit> Colon = Pulse.Trace(":");
    private readonly static Flow<Unit> semiColon = Pulse.Trace(";");
    private readonly static Flow<Unit> Null = Indented("null");

    private static Flow<object> Interspersed(Flow<object> flow) =>
        from input in Pulse.Start<object>()
        from ministers in Pulse.Gather<Cast>()
        from seperator in Pulse.When<Cast>(a => a.Intersperse.Restricted(), Separator)
        let element = Pulse.ToFlow(flow, input)
        select input;

    // private readonly static Flow<object> InterspersedPrimed =
    //     from input in Pulse.Start<object>()
    //     from ministers in Pulse.Gather<Ministers>()
    //     from seperator in Pulse.When<Ministers>(a => a.StartOfCollection.Restricted(), Separator)
    //     let element = Pulse.ToFlow(Anastasia!, input)
    //     from _ in Spacing.Then(element)
    //     select input;

    // private readonly static Flow<IEnumerable> Interspersed =
    //     from input in Pulse.Start<IEnumerable>()
    //     from ministers in Pulse.Gather<Ministers>()
    //     let inner = Pulse.ToFlow(InterspersedPrimed, input.Cast<object>())
    //     from _ in Pulse.Scoped<Ministers>(a => a.PrimeStartOfCollection(), inner)
    //     select input;

    private static Flow<Unit> Enclosed(string left, string right, Flow<Unit> innerFlow) =>
        from leftBracket in Indented(left)
        from nl in Pulse.TraceIf<Cast>(a => !a.Flat, () => Environment.NewLine)
        from _ in Pulse.Scoped<Cast>(a => a.IncreaseLevel().EnableIndent(), innerFlow)
        from __ in Pulse.Scoped<Cast>(a => a.EnableIndent(), Indented(right))
        select Unit.Instance;

    private static Flow<Unit> Braced(Flow<Unit> innerFlow) => Enclosed("{", "}", innerFlow);
    private static Flow<Unit> Bracketed(Flow<Unit> innerFlow) => Enclosed("(", ")", innerFlow);
    private static Flow<Unit> SquareBracketed(Flow<Unit> innerFlow) => Enclosed("[", "]", innerFlow);


    private static string FormatMethodName(string specName) => specName.Replace(" ", "_");

    private readonly static Flow<TrackedDeposition> trackedDeposition =
        from input in Pulse.Start<TrackedDeposition>()
        let split = input.Label.Split(":")
        let val = split.Length == 2 ? split[1] : "true"
        from _ in indent
        from __ in split.Length == 2
            ? Pulse.Trace($"var {split[0]} = {split[1]};")
            : Pulse.Trace($"var {input.Label} = new {CapitalizeFirstLetter(input.Label)}();")
        from ___ in newLine
        select input;

    public record ActionWithInputs(ActionDeposition Action, IEnumerable<InputDeposition> Inputs);

    // var groupedInputs = inputs
    // .Where(i => i.Label.StartsWith(methodName + ":"))
    // .OrderBy(i => ParseInt(i.Label after colon))
    // .Select(i => i.Value);
    private readonly static Flow<object> inputValue =
        from input in Pulse.Start<object>()
        from _ in Pulse.TraceIf<Cast>(a => a.Intersperse.Restricted(), () => ", ")
        from __ in Pulse.Trace(input)
        select input;

    private readonly static Flow<ActionWithInputs> actionDeposition =
        from start in Pulse.Start<ActionWithInputs>()
        let action = start.Action
        let inputs = start.Inputs
        let split = action.Label.Split(".")
        let inputNames = split.Length == 2 ? split[1].Split(":").Select(DecapitalizeFirstLetter) : []

        let inputValues = inputs.Where(a => inputNames.Contains(a.Label.Split(":").First())).Select(a => a.Value)
        from call in indent.Then(Pulse.Trace($"{action.Label}"))
        from lb in Pulse.Trace("(")
        from _ in Pulse.Scoped<Cast>(a => a.SetIntersperse(), Pulse.ToFlow(inputValue, inputValues))
        from rb in Pulse.Trace(")")
        from end in semiColon.Then(newLine)
        select start;

    private readonly static Flow<ExecutionDeposition> executionDeposition =
        from input in Pulse.Start<ExecutionDeposition>()
        from _ in Pulse.ToFlowIf<TrackedDeposition, Cast>(
                a => a.Tracked.Passable(), trackedDeposition, () => input.TrackedDepositions)
        from __ in Pulse.ToFlow(actionDeposition, input.ActionDepositions.Select(a => new ActionWithInputs(a, input.InputDepositions)))
        select input;

    private readonly static Flow<ExecutionDeposition> maybeExecutionDeposition =
        from input in Pulse.Start<ExecutionDeposition>()
        from _ in Pulse.ToFlowIf(input.HasContent(), executionDeposition, () => input)
        select input;

    private readonly static Flow<FailedSpecDeposition> assertSpec =
        from input in Pulse.Start<FailedSpecDeposition>()
        let split = input.FailedSpec.Split(":")
        let val = split.Length == 2 ? split[1] : "true"
        from _2 in Pulse.Trace($"Assert.True({val});").Then(newLine)
        select input;

    private readonly static Flow<ExceptionDeposition> assertException =
        from input in Pulse.Start<ExceptionDeposition>()
        select input;

    private readonly static Flow<FailureDeposition> assertion =
        from input in Pulse.Start<FailureDeposition>()
        from _1 in Pulse.ToFlowIf(input is FailedSpecDeposition, assertSpec, () => (FailedSpecDeposition)input)
        from _2 in Pulse.ToFlowIf(input is ExceptionDeposition, assertException, () => (ExceptionDeposition)input)
        select input;

    private readonly static Flow<FailedSpecDeposition> testMethodNameSpec =
        from input in Pulse.Start<FailedSpecDeposition>()
        from _2 in Pulse.Trace($"public void {FormatMethodName(input.FailedSpec.Split(":")[0])}()").Then(newLine)
        select input;

    private readonly static Flow<ExceptionDeposition> testMethodNameException =
        from input in Pulse.Start<ExceptionDeposition>()
        select input;

    private readonly static Flow<FailureDeposition> testMethodName =
        from input in Pulse.Start<FailureDeposition>()
        from _1 in Pulse.ToFlowIf(input is FailedSpecDeposition, testMethodNameSpec, () => (FailedSpecDeposition)input)
        from _2 in Pulse.ToFlowIf(input is ExceptionDeposition, testMethodNameException, () => (ExceptionDeposition)input)
        select input;

    public record Body(IEnumerable<ExecutionDeposition> Executions, FailureDeposition Failure);
    private readonly static Flow<Body> body =
        from input in Pulse.Start<Body>()
        from _ in Pulse.ToFlow(maybeExecutionDeposition, input.Executions)
        from __ in indent.Then(Pulse.ToFlow(assertion, input.Failure))
        select input;

    private readonly static Flow<Verdict?> verdict =
        from verdict in Pulse.Start<Verdict>()
        from _ in indent.Then(Pulse.Trace("[Fact]")).Then(newLine)
        from __ in indent.Then(Pulse.ToFlow(testMethodName, verdict.FailureDeposition))
        from ___ in Pulse.Scoped<Cast>(a => a.SetTracked(),
             Braced(Pulse.ToFlow(body, new Body(verdict.ExecutionDepositions, verdict.FailureDeposition))))
        from ____ in newLine
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
