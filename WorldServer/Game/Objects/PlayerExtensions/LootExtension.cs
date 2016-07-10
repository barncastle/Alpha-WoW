using Common.Constants;
using Common.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Game.Structs;
using WorldServer.Game.Managers;
using WorldServer.Storage;
using Common.Helpers;

namespace WorldServer.Game.Objects.PlayerExtensions.Loot
{
    public static class LootExtension
    {
        public static void SendLoot(this Player p, ulong Guid)
        {
            WorldObject obj = Database.Creatures.TryGet<WorldObject>(Guid) ??
                              Database.GameObjects.TryGet<WorldObject>(Guid) ??
                              Database.Items.TryGet<WorldObject>(Guid) ??
                              null;

            if (obj == null) return;

            LootTypes lootType = LootTypes.LOOT_TYPE_NOTALLOWED;
            uint money = 0;
            List<LootObject> loot = new List<LootObject>();
            sbyte slot = 1;

            if (obj.IsTypeOf(ObjectTypes.TYPE_UNIT)) //Creature Loot
            {
                p.CurrentLootTarget = Guid;
                Creature mob = (Creature)obj;

                if (mob.Loot.Count > 0 || mob.Money > 0)
                {
                    foreach (LootObject lo in mob.Loot.ToArray())
                    {
                        if (lo.Lootable(p))
                            loot.Add(lo);
                        else
                            mob.Loot.Remove(lo);
                    }

                    lootType = LootTypes.LOOT_TYPE_CORPSE;
                }

                if (mob.Loot.Count == 0 && mob.Money == 0)
                    LootRelease(mob, p);

                money = mob.Money;
            }
            else if (obj.IsTypeOf(ObjectTypes.TYPE_GAMEOBJECT))
            {
                GameObject go = (GameObject)obj;
                if (go.LootRecipient != p)
                    return;

                p.CurrentLootTarget = obj.Guid;

                go.GenerateLoot();
                foreach (LootObject lo in go.Loot.ToArray())
                {
                    if (lo.Lootable(p))
                        loot.Add(lo);
                    else
                        go.Loot.Remove(lo);

                    lootType = LootTypes.LOOT_TYPE_SKINNING;
                }
            }
            else if (obj.IsTypeOf(ObjectTypes.TYPE_ITEM))
            {

            }

            PacketWriter pw = new PacketWriter(Opcodes.SMSG_LOOT_RESPONSE);
            pw.WriteUInt64(obj.Guid);
            pw.WriteUInt8((byte)lootType);
            pw.WriteUInt32(money);
            pw.WriteUInt8((byte)loot.Count);
            foreach (LootObject li in loot)
            {
                p.QueryItemCheck(li.Item.Entry);

                pw.WriteUInt8((byte)slot);
                pw.WriteUInt32(li.Item.Template.Entry);
                pw.WriteUInt32((uint)li.Count);
                pw.WriteUInt32(li.Item.Template.DisplayID);
                pw.WriteUInt32(0);
                pw.WriteUInt32(0);
                slot++;
            }
            GridManager.Instance.SendSurrounding(pw, p);
        }

        public static void LootItems(this Player p, byte slot)
        {
            WorldObject obj = null;
            List<LootObject> objloot = new List<LootObject>();
            uint objmoney = 0;

            if (Database.Creatures.ContainsKey(p.CurrentLootTarget))
            {
                if (p.CurrentSelection != p.CurrentLootTarget)
                    return;
                Creature mob = Database.Creatures.TryGet(p.CurrentLootTarget);
                if (mob == null)
                    return;

                obj = mob;
                objloot = mob.Loot;
                objmoney = mob.Money;
            }
            else if(Database.GameObjects.ContainsKey(p.CurrentLootTarget))
            {
                GameObject go = Database.GameObjects.TryGet(p.CurrentLootTarget);
                if (go == null)
                    return;

                obj = go;
                objloot = go.Loot;
            }
            else
            {
                LootRelease(p, p);
            }
            
            List<LootObject> loot = new List<LootObject>();
            if (objloot.Count > 0 || objmoney > 0)
            {
                foreach (LootObject lo in objloot)
                    if (lo.Lootable(p))
                        p.AddItem(lo.Item.Entry, lo.Count);

                slot--;
                objloot.RemoveAt(slot); //Remove from loot store : 0 based index

                //Send item removed to everyone
                PacketWriter response = new PacketWriter(Opcodes.SMSG_LOOT_REMOVED);
                response.WriteUInt8(slot);
                GridManager.Instance.SendSurrounding(response, p);

                if (objloot.Count == 0 && objmoney == 0)
                {
                    LootRelease(obj, p);
                    return;
                }
                else
                {
                    p.SendLoot(p.CurrentLootTarget);
                    p.Dirty = true;
                }
            }
        }

        public static void LootMoney(this Player p)
        {
            if (p.CurrentSelection != p.CurrentLootTarget)
                return;

            Creature mob = Database.Creatures.TryGet(p.CurrentLootTarget);
            if (mob == null || mob?.Money <= 0)
                return;

            uint money = mob.Money;
            mob.Money = 0;
            p.Money += money;

            PacketWriter pw = new PacketWriter(Opcodes.SMSG_LOOT_MONEY_NOTIFY);
            pw.WriteUInt32(money);
            p.Client.Send(pw);

            if (mob.Loot.Count == 0 && mob.Money == 0)
            {
                //No more items
                LootRelease(mob, p);
                return;
            }
            else
                p.SendLoot(mob.Guid);
        }

        public static void LootRelease(WorldObject obj, Player p)
        {
            if(obj.IsTypeOf(ObjectTypes.TYPE_UNIT) && obj != p)
            {
                Creature mob = (Creature)obj;
                mob.Loot.Clear();
                Flag.RemoveFlag(ref mob.DynamicFlags, (uint)UnitDynamicTypes.UNIT_DYNAMIC_LOOTABLE); //Remove lootable flags
                mob.SendLootRelease(p);
                GridManager.Instance.SendSurrounding(mob.BuildUpdate(), mob);
            }
            
            Flag.RemoveFlag(ref p.UnitFlags, (uint)UnitFlags.UNIT_FLAG_LOOTING);
            GridManager.Instance.SendSurrounding(p.BuildUpdate(), p);

            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_LOOT_RELEASE_RESPONSE);
            pkt.WriteUInt64(p.CurrentLootTarget);
            pkt.WriteUInt8(1);
            p.Client.Send(pkt);

            p.CurrentLootTarget = 0;
        }
    }
}
