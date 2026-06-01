using System.Text.RegularExpressions;

namespace PoE1ToonAudit.Parsing;

public record ParsedMod(IReadOnlyList<ResistanceType> Resistances, int Value);

public static class ItemModParser
{
    // check for single mods with multiple (hybrid) res first
    private static readonly (string Pattern, ResistanceType[] Types)[] ResPatterns =
    [
        // all res
        ("to all Elemental Resistances",            [ResistanceType.Fire, ResistanceType.Cold, ResistanceType.Lightning]),

        // crafted / veiled elemental hybrids
        ("to Fire and Cold Resistances",            [ResistanceType.Fire, ResistanceType.Cold]),
        ("to Fire and Lightning Resistances",       [ResistanceType.Fire, ResistanceType.Lightning]),
        ("to Cold and Lightning Resistances",       [ResistanceType.Cold, ResistanceType.Lightning]),

        // crafted / veiled chaos hybrids
        ("to Fire and Chaos Resistances",           [ResistanceType.Fire,      ResistanceType.Chaos]),
        ("to Cold and Chaos Resistances",           [ResistanceType.Cold,      ResistanceType.Chaos]),
        ("to Lightning and Chaos Resistances",      [ResistanceType.Lightning, ResistanceType.Chaos]),

        // pure
        ("to Fire Resistance",                      [ResistanceType.Fire]),
        ("to Cold Resistance",                      [ResistanceType.Cold]),
        ("to Lightning Resistance",                 [ResistanceType.Lightning]),
        ("to Chaos Resistance",                     [ResistanceType.Chaos]),
    ];
    
    // checks for numeric values optionally preceded by a `+` or `-`. Can be precompiled too if needed.
    private static readonly Regex NumericValueRegex = new(@"[+-]?\d+", RegexOptions.Compiled);

    private static ParsedMod? TryParseResistance(string mod)
    {
        foreach (var (pattern, types) in ResPatterns)
        {
            if (!mod.Contains(pattern, StringComparison.OrdinalIgnoreCase) 
                || mod.Contains("maximum", StringComparison.OrdinalIgnoreCase))
                continue;

            var match = NumericValueRegex.Match(mod);
            
            // ternary expression doing an unneeded extra check
            // return !match.Success ? null : new ParsedMod(types, int.Parse(match.Value));
            
            return new ParsedMod(types, int.Parse(NumericValueRegex.Match(mod).Value));
        }

        return null;
    }
    
    public static ResistanceTotals ParseResistanceTotals(IEnumerable<string> mods)
    {
        ResistanceTotals totals = new();

        foreach (var mod in mods)
        {
            var parsed = TryParseResistance(mod);
            if (parsed is null) continue;

            foreach (var type in parsed.Resistances)
                totals.Add(type, parsed.Value);
        }

        return totals;
    }
}