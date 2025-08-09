using QuickAcid.Proceedings;
using QuickAcid.Proceedings.ClerksOffice;
using QuickAcid.Refinery;
using QuickPulse.Explains.Text;

namespace QuickAcid.Tests.Proceedings;

public class VerdictTests
{
    [Fact]
    public void Base_Case()
    {
        var dossier =
            new Dossier(
                    FailingSpec: "Some Invariant:account.Balance >= 0",
                    AssayerSpec: null,
                    Exception: null,
                    OriginalRunExecutionCount: 10,
                    ExecutionNumbers: [1, 2, 3, 4],
                    ShrinkCount: 1,
                    Seed: 12345678
                );
        var caseFile = CaseFile.WithVerdict(Verdict.FromDossier(dossier)
            .AddExecutionDeposition(new ExecutionDeposition(1)
                .AddTrackedDeposition(new TrackedDeposition("account", "{ something irrelevant}"))
                .AddInputDeposition(new InputDeposition("withdraw", 42))
                .AddActionDeposition(new ActionDeposition("account.Withdraw"))));
        var result = Fabricator.Forge(caseFile);
        var reader = LinesReader.FromText(result);
        Assert.Equal("namespace Refined.By.QuickAcid;", reader.NextLine());
        Assert.Equal("", reader.NextLine());
        Assert.Equal("public class UnitTests", reader.NextLine());
        Assert.Equal("{", reader.NextLine());
        Assert.Equal("    [Fact]", reader.NextLine());
        Assert.Equal("    public void Some_Invariant()", reader.NextLine());
        Assert.Equal("    {", reader.NextLine());
        Assert.Equal("        var account = new Account();", reader.NextLine());
        Assert.Equal("        account.Withdraw(42);", reader.NextLine());
        Assert.Equal("        Assert.True(account.Balance >= 0);", reader.NextLine());
        Assert.Equal("    }", reader.NextLine());
        Assert.Equal("}", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void Multiple_Inputs_Method()
    {
        var dossier =
            new Dossier(
                    FailingSpec: "Some Invariant:account.Balance >= 0",
                    AssayerSpec: null,
                    Exception: null,
                    OriginalRunExecutionCount: 10,
                    ExecutionNumbers: [1, 2, 3, 4],
                    ShrinkCount: 1,
                    Seed: 12345678
                );
        var caseFile = CaseFile.WithVerdict(Verdict.FromDossier(dossier)
            .AddExecutionDeposition(new ExecutionDeposition(1)
                .AddTrackedDeposition(new TrackedDeposition("account", "{ something irrelevant}"))
                .AddInputDeposition(new InputDeposition("withdraw:1", 42))
                .AddInputDeposition(new InputDeposition("withdraw:2", 666))
                .AddActionDeposition(new ActionDeposition("account.Withdraw"))));
        var result = Fabricator.Forge(caseFile);
        var reader = LinesReader.FromText(result);
        Assert.Equal("namespace Refined.By.QuickAcid;", reader.NextLine());
        Assert.Equal("", reader.NextLine());
        Assert.Equal("public class UnitTests", reader.NextLine());
        Assert.Equal("{", reader.NextLine());
        Assert.Equal("    [Fact]", reader.NextLine());
        Assert.Equal("    public void Some_Invariant()", reader.NextLine());
        Assert.Equal("    {", reader.NextLine());
        Assert.Equal("        var account = new Account();", reader.NextLine());
        Assert.Equal("        account.Withdraw(42, 666);", reader.NextLine());
        Assert.Equal("        Assert.True(account.Balance >= 0);", reader.NextLine());
        Assert.Equal("    }", reader.NextLine());
        Assert.Equal("}", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void More()
    {
        var dossier =
            new Dossier(
                    FailingSpec: "Some Invariant:account.Balance >= 0",
                    AssayerSpec: null,
                    Exception: null,
                    OriginalRunExecutionCount: 10,
                    ExecutionNumbers: [1, 2, 3, 4],
                    ShrinkCount: 1,
                    Seed: 12345678
                );
        var caseFile = CaseFile.WithVerdict(Verdict.FromDossier(dossier)
            .AddExecutionDeposition(new ExecutionDeposition(1)
                .AddTrackedDeposition(new TrackedDeposition("account", "{ 1 }"))
                .AddInputDeposition(new InputDeposition("deposit", 100))
                .AddActionDeposition(new ActionDeposition("account.Deposit"))
                .AddInputDeposition(new InputDeposition("withdraw:1", 42))
                .AddInputDeposition(new InputDeposition("withdraw:2", 666))
                .AddActionDeposition(new ActionDeposition("account.Withdraw"))));
        var result = Fabricator.Forge(caseFile);
        var reader = LinesReader.FromText(result);
        Assert.Equal("namespace Refined.By.QuickAcid;", reader.NextLine());
        Assert.Equal("", reader.NextLine());
        Assert.Equal("public class UnitTests", reader.NextLine());
        Assert.Equal("{", reader.NextLine());
        Assert.Equal("    [Fact]", reader.NextLine());
        Assert.Equal("    public void Some_Invariant()", reader.NextLine());
        Assert.Equal("    {", reader.NextLine());
        Assert.Equal("        var account = new Account();", reader.NextLine());
        Assert.Equal("        account.Deposit(100);", reader.NextLine());
        Assert.Equal("        account.Withdraw(42, 666);", reader.NextLine());
        Assert.Equal("        Assert.True(account.Balance >= 0);", reader.NextLine());
        Assert.Equal("    }", reader.NextLine());
        Assert.Equal("}", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void Realistic()
    {
        var dossier =
            new Dossier(
                    FailingSpec: "Some Invariant:account.Balance >= 0",
                    AssayerSpec: null,
                    Exception: null,
                    OriginalRunExecutionCount: 10,
                    ExecutionNumbers: [1, 2, 3, 4],
                    ShrinkCount: 1,
                    Seed: 12345678
                );
        var caseFile = CaseFile.WithVerdict(Verdict.FromDossier(dossier)
            .AddExecutionDeposition(new ExecutionDeposition(1)
                .AddTrackedDeposition(new TrackedDeposition("account", "{ 1 }"))
                .AddInputDeposition(new InputDeposition("deposit", 100))
                .AddActionDeposition(new ActionDeposition("account.Deposit"))
                .AddActionDeposition(new ActionDeposition("account.Deposit")))
            .AddExecutionDeposition(new ExecutionDeposition(2)
                .AddTrackedDeposition(new TrackedDeposition("account", "{ 2 }"))
                .AddInputDeposition(new InputDeposition("withdraw:1", 42))
                .AddInputDeposition(new InputDeposition("withdraw:2", 666))
                .AddActionDeposition(new ActionDeposition("account.Withdraw"))));
        var result = Fabricator.Forge(caseFile);
        var reader = LinesReader.FromText(result);
        Assert.Equal("namespace Refined.By.QuickAcid;", reader.NextLine());
        Assert.Equal("", reader.NextLine());
        Assert.Equal("public class UnitTests", reader.NextLine());
        Assert.Equal("{", reader.NextLine());
        Assert.Equal("    [Fact]", reader.NextLine());
        Assert.Equal("    public void Some_Invariant()", reader.NextLine());
        Assert.Equal("    {", reader.NextLine());
        Assert.Equal("        var account = new Account();", reader.NextLine());
        Assert.Equal("        account.Deposit(100);", reader.NextLine());
        Assert.Equal("        account.Deposit(100);", reader.NextLine());
        Assert.Equal("        account.Withdraw(42, 666);", reader.NextLine());
        Assert.Equal("        Assert.True(account.Balance >= 0);", reader.NextLine());
        Assert.Equal("    }", reader.NextLine());
        Assert.Equal("}", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }
}