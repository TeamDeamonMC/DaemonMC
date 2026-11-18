namespace DaemonMC.Network.Enumerations
{
    public enum PersonaPieceTypes
    {
        persona_unknown,
        persona_skeleton,
        persona_body,
        persona_skin,
        persona_bottom,
        persona_feet,
        persona_dress,
        persona_top,
        persona_high_pants,
        persona_hand,
        persona_outerwear,
        persona_facial_hair,
        persona_mouth,
        persona_eyes,
        persona_hair,
        persona_hood,
        persona_back,
        persona_face_accessory,
        persona_head,
        persona_legs,
        persona_left_leg,
        persona_right_leg,
        persona_arms,
        persona_left_arm,
        persona_right_arm,
        persona_capes,
        persona_classic_skin,
        persona_emote,
        persona_coco,
        unsupported,
    };

    public static class PersonaPieceTypesExtensions
    {
        public static PersonaPieceTypes Parse(string? pieceType)
        {
            if (string.IsNullOrEmpty(pieceType))
            {
                return PersonaPieceTypes.persona_unknown;
            }

            if (Enum.TryParse(pieceType, true, out PersonaPieceTypes type))
            {
                return type;
            }

            if (Enum.TryParse("persona_" + pieceType, true, out type))
            {
                return type;
            }

            return PersonaPieceTypes.unsupported;
        }

        public static PersonaPieceTypes FromId(int id)
        {
            return Enum.IsDefined(typeof(PersonaPieceTypes), id) ? (PersonaPieceTypes)id : PersonaPieceTypes.unsupported;
        }
    }
}
