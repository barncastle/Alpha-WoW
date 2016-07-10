using Common.Constants;

namespace WorldServer.Game.Structs
{
    public class UnitStructs
    {
        public class CalcDamageInfo
        {
            public ulong attacker;             // Attacker
            public ulong target;               // Target for damage
            public uint damageSchoolMask;
            public uint damage;
            public uint absorb;
            public uint resist;
            public uint blocked_amount;
            public HitInfo HitInfo;
            public VictimStates TargetState;
            // Helper
            public AttackTypes attackType; //
            public ProcFlags procAttacker;
            public ProcFlags procVictim;
            public ProcFlagsExLegacy procEx;
            public uint cleanDamage;          // Used only for rage calculation
                                              //MeleeHitOutcome hitOutCome;  // TODO: remove this field (need use TargetState)
        }

        public struct VirtualItemInfo
        {
            byte m_classID;
            byte m_subclassID;
            byte m_material;
            byte m_inventoryType;
            byte m_sheatheType;
            byte m_padding0;
            byte m_padding1;
            byte m_padding2;
        }
    }
}
