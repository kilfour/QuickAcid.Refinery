using QuickAcid.Proceedings;
using QuickAcid.Proceedings.ClerksOffice;
using QuickAcid.Refinery;
using QuickPulse.Explains.Text;

namespace QuickAcid.Tests.Proceedings;

public abstract class DepositionTest
{
    protected Dossier Dossier { get; } =
        new Dossier(
                FailingSpec: "Some Invariant",
                Exception: null,
                OriginalRunExecutionCount: 10,
                ExecutionNumbers: [1, 2, 3, 4],
                ShrinkCount: 1,
                Seed: 12345678
            );

    protected LinesReader Forge(CaseFile caseFile)
    {
        var result = Fabricator.Forge(caseFile);
        var reader = LinesReader.FromText(result);
        return reader;
    }
}