using QuickAcid.Proceedings;
using QuickPulse;
using QuickPulse.Arteries;

namespace QuickAcid.Refinery;

public static class Fabricator
{
    public static string Forge(CaseFile caseFile)
    {
        return
            Signal.From(The.Anvil)
                .SetArtery(TheString.Catcher())
                .Pulse(caseFile)
                .GetArtery<Holden>()
                .Whispers();
    }
}
