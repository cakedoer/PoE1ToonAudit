namespace PoE1ToonAudit.Models;

public record PoeToonAuditResult
(
    bool HasMageblood,
    bool IsCapReadyForMaps,
    EleResAudit Fire,
    EleResAudit Cold,
    EleResAudit Lightning,
    ChaosResAudit Chaos,
    List<MissingLifeItem> ItemsMissingLife
);

public record EleResAudit
(
    string Element,
    int Total,
    int Net,
    bool IsCapped,
    int ShortBy,
    int Overcap
);

public record ChaosResAudit
(
    int Total,
    int Net,
    bool AtLeastZeroNet,
    int Overcap
);

public record MissingLifeItem
(
    string Name,
    string TypeLine,
    string InventoryId
);