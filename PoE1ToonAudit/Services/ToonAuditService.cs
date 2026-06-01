using PoE1ToonAudit.Models;
using PoE1ToonAudit.Parsing;

namespace PoE1ToonAudit.Services;

public class ToonAuditService
{
    private const int KitavaPenalty     = 60;
    private const int ResistanceCap     = 75;
    private const int RequiredRawEleRes = ResistanceCap + KitavaPenalty; // 135
    // ignore uniques for evaluating life rolls
    private const int UniqueFrame       = 3;
    
    // ignore weapons, trinkets, alternate weapon set
    private static readonly HashSet<string> NonLifeSlots =
        [ "Weapon", "Weapon2", "Offhand2", "Trinket" ];
    
    // for offhand in primary weapon set, only the following types for life
    private static readonly HashSet<string> DefensiveOffhands =
        [ "Shield", "Buckler", "Quiver" ];

    private static readonly ResistanceType[] EleResTypes =
        [ ResistanceType.Fire, ResistanceType.Cold, ResistanceType.Lightning ];

    public PoeToonAuditResult Audit(PoeToon data)
    {
        List<PoeItem> equippedGear = data.Items
            .Where(i => i.InventoryId != "MainInventory" && i.InventoryId != "Flask")
                .ToList();

        var allMods = equippedGear
            .SelectMany(i => i.AllMods);

        var resTotals = ItemModParser.ParseResistanceTotals(allMods);

        var fire      = resTotals.Get(ResistanceType.Fire);
        var cold      = resTotals.Get(ResistanceType.Cold);
        var lightning = resTotals.Get(ResistanceType.Lightning);
        var chaos     = resTotals.Get(ResistanceType.Chaos);

        // check if item has a life roll
        var itemsMissingLife = equippedGear
            .Where(i => !NonLifeSlots.Contains(i.InventoryId))
            .Where(i => i.FrameType != UniqueFrame)
            .Where(i => i.InventoryId != "Offhand" || DefensiveOffhands.Any(s => i.TypeLine.Contains(s)))
            .Where(i => i.AllMods.All(m => !m.Contains("maximum Life")))
            .Select(i => new MissingLifeItem(i.Name, i.TypeLine, i.InventoryId))
                .ToList();

        return new PoeToonAuditResult
        (
            // todo calculate megablood resists from flasks
            HasMageblood:      equippedGear.Any(i => i.InventoryId == "Belt" && i.Name.Contains("Mageblood")),
            IsCapReadyForMaps: EleResTypes.All(t => resTotals.Get(t) >= RequiredRawEleRes),
            Fire:              EvalRes("Fire",      fire),
            Cold:              EvalRes("Cold",      cold),
            Lightning:         EvalRes("Lightning", lightning),
            Chaos:             EvalChaos(chaos),
            ItemsMissingLife:  itemsMissingLife
        );
    }

    private static EleResAudit EvalRes(string element, int total) => new
    (
        Element:  element,
        Total:    total,
        Net:      total - KitavaPenalty,
        IsCapped: total >= RequiredRawEleRes,
        ShortBy:  total >= RequiredRawEleRes ? 0 : RequiredRawEleRes - total,
        Overcap:  total > RequiredRawEleRes  ? total - RequiredRawEleRes : 0
    );

    private static ChaosResAudit EvalChaos(int total) => new
    (
        Total:          total,
        Net:            total - KitavaPenalty,
        AtLeastZeroNet: total >= KitavaPenalty,
        Overcap:        total > KitavaPenalty ? total - KitavaPenalty : 0
    );
}