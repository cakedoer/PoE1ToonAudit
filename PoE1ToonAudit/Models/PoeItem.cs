namespace PoE1ToonAudit.Models;

public record PoeItem
(
    string Name,
    string TypeLine,
    string InventoryId,
    int FrameType,
    List<string>? ImplicitMods,
    List<string>? ExplicitMods
)
{
    // handle white items with no implicit (i.e. empty items) to avoid null reference exception
    public IEnumerable<string> AllMods => 
        (ImplicitMods ?? []).Concat(ExplicitMods ?? []);
}