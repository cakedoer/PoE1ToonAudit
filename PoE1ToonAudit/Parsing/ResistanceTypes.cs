namespace PoE1ToonAudit.Parsing;

public enum ResistanceType { Fire, Cold, Lightning, Chaos }

public static class ResistanceTypes
{
    public static readonly ResistanceType[] Elemental =
        [ResistanceType.Fire, ResistanceType.Cold, ResistanceType.Lightning];

    public static readonly ResistanceType[] All =
        [ResistanceType.Fire, ResistanceType.Cold, ResistanceType.Lightning, ResistanceType.Chaos];
}