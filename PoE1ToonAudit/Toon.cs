namespace PoE1ToonAudit;

public record Toon
(
    string Name, 
    string Class,
    int Level, 
    bool IsDead,
    List<string> EquippedItems
);
