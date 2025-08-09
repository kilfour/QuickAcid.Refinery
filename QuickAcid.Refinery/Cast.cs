using QuickPulse;

namespace QuickAcid.Refinery;

public record Cast
{
    public bool Flat { get; init; } = false;
    public bool NeedsIndent { get; init; } = false;
    public Cast EnableIndent() => this with { NeedsIndent = true };
    public Cast DisableIndent() => this with { NeedsIndent = false };

    public int Level { get; init; } = 0;
    public Cast IncreaseLevel() => this with { Level = Level + 1 };

    public Valve Intersperse { get; init; } = Valve.Closed();
    public Cast SetIntersperse() => this with { Intersperse = Valve.Install() };

    public Valve Tracked { get; init; } = Valve.Closed();
    public Cast SetTracked() => this with { Tracked = Valve.Install() };

}
