namespace PoE1ToonAudit;

public record PoeToonRoot( List<PoeItem> Items );
public record PoeItem
(
    string Name,
    string TypeLine,
    string InventoryId,
    int FrameType
);