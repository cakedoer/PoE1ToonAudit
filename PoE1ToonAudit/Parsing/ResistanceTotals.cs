namespace PoE1ToonAudit.Parsing;

public class ResistanceTotals
{
    private readonly Dictionary<ResistanceType, int> _totals = new()
    {
        [ResistanceType.Fire]      = 0,
        [ResistanceType.Cold]      = 0,
        [ResistanceType.Lightning] = 0,
        [ResistanceType.Chaos]     = 0,
    };

    public void Add(ResistanceType type, int value) => _totals[type] += value;
    public int Get(ResistanceType type) => _totals[type];
    public bool AllElementalMeet(int threshold) => ResistanceTypes.Elemental.All(t => _totals[t] >= threshold);
}