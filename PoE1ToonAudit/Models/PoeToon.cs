namespace PoE1ToonAudit.Models;

public record PoeToon
(
    List<PoeItem> Items,
    PoeCharacter Character
);

public record PoeCharacter
(
    string Name,
    string Class,
    int Level,
    bool IsDead,
    string League
);