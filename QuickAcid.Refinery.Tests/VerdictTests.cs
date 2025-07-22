using QuickAcid.Proceedings;

namespace QuickAcid.Tests.Proceedings;

public class VerdictTests : DepositionTest
{

    [Fact]
    public void One()
    {
        var caseFile = CaseFile.WithVerdict(Verdict.FromDossier(Dossier)
            .AddExecutionDeposition(new ExecutionDeposition(1)
                .AddTrackedDeposition(new TrackedDeposition("account", "{ something irrelevant}"))));
        var reader = Forge(caseFile);
        Assert.Equal("namespace Refined.By.QuickAcid;", reader.NextLine());
        Assert.Equal("", reader.NextLine());
        Assert.Equal("public class UnitTests", reader.NextLine());
        Assert.Equal("{", reader.NextLine());
        Assert.Equal("    [Fact]", reader.NextLine());
        Assert.Equal("    public void Some_Invariant()", reader.NextLine());
        Assert.Equal("    {", reader.NextLine());
        Assert.Equal("        var account = new Account();", reader.NextLine());
        // Assert.Equal("        account.Withdraw(42);", reader.NextLine());
        // Assert.Equal("        Assert.True(account.Balance >= 0);", reader.NextLine());
        Assert.Equal("    }", reader.NextLine());
        Assert.Equal("}", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }
}