using Common.Constants;
using Common.Database;
using Common.Helpers;
using Common.Network;
using Common.Network.Packets;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Game.Managers;
using WorldServer.Game.Objects.PlayerExtensions;
using WorldServer.Game.Objects.PlayerExtensions.Loot;
using WorldServer.Game.Structs;
using WorldServer.Storage;

namespace WorldServer.Game.Objects
{
    [Table("spawns_gameobjects")]
    public class GameObject : WorldObject
    {
        public uint Entry;
        public GameObjectTemplate Template;
        public float[] Rotation = new float[4];
        public int SpawnTime;
        public int AnimProgress;
        public byte State;
        public object Data;

        public Player LootRecipient = null;
        public uint LootId
        {
            get
            {
                switch (this.Template.Type)
                {
                    case (uint)GameObjectTypes.TYPE_FISHINGNODE:
                    case (uint)GameObjectTypes.TYPE_CHEST:
                        return Template.RawData[1];
                    default:
                        return 0;
                }
            }
        }
        public List<LootObject> Loot = new List<LootObject>();

        public GameObject()
        {
            this.Guid = Globals.GO_GUID + 1;
            Globals.GO_GUID = this.Guid;
            this.ObjectType |= ObjectTypes.TYPE_GAMEOBJECT;
        }

        public GameObject(ulong guid)
        {
            this.Guid = guid | (ulong)HIGH_GUID.HIGHGUID_GAMEOBJECT;
            this.ObjectType |= ObjectTypes.TYPE_GAMEOBJECT;
        }

        public GameObject(ref MySqlDataReader dr)
        {
            this.ObjectType |= ObjectTypes.TYPE_GAMEOBJECT;
            this.Guid = Convert.ToUInt64(dr["spawn_id"]) | (ulong)HIGH_GUID.HIGHGUID_GAMEOBJECT;
            this.Entry = Convert.ToUInt32(dr["spawn_entry"]);
            this.Map = Convert.ToUInt32(dr["spawn_map"]);
            this.Location = new Vector(Convert.ToSingle(dr["spawn_positionX"]),
                                       Convert.ToSingle(dr["spawn_positionY"]),
                                       Convert.ToSingle(dr["spawn_positionZ"]));
            this.Orientation = Convert.ToSingle(dr["spawn_orientation"]);
            this.Rotation = new[] {Convert.ToSingle(dr["spawn_rotation0"]),
                                   Convert.ToSingle(dr["spawn_rotation1"]),
                                   Convert.ToSingle(dr["spawn_rotation2"]),
                                   Convert.ToSingle(dr["spawn_rotation3"])};
            this.SpawnTime = Convert.ToInt32(dr["spawn_spawntime"]);
            this.AnimProgress = Convert.ToInt32(dr["spawn_animprogress"]);
            this.State = Convert.ToByte(dr["spawn_state"]);
        }

        public void OnDbLoad()
        {
            this.Template = Database.GameObjectTemplates.TryGet(this.Entry);
            this.Scale = this.Template.Size;

            if (Rotation[2] == 0 && Rotation[3] == 0)
            {
                Rotation[2] = (float)Math.Sin(this.Orientation / 2);
                Rotation[3] = (float)Math.Cos(this.Orientation / 2);
            }

            GridManager.Instance.AddOrGet(this, true);
        }

        public override PacketWriter QueryDetails()
        {
            PacketWriter pw = new PacketWriter(Opcodes.SMSG_GAMEOBJECT_QUERY_RESPONSE);
            pw.WriteUInt32(this.Entry);
            pw.WriteUInt32(this.Template.Type);
            pw.WriteUInt32(this.Template.DisplayId);
            pw.WriteString(this.Template.Name);
            pw.WriteString(string.Empty);
            pw.WriteString(string.Empty);
            pw.WriteString(string.Empty);

            for (int i = 0; i < this.Template.RawData.Length; i++)
                pw.WriteUInt32(this.Template.RawData[i]);

            return pw;
        }

        public override PacketWriter BuildUpdate(UpdateTypes type = UpdateTypes.UPDATE_PARTIAL, bool self = false)
        {
            PacketWriter packet = CreateObject(false);
            UpdateClass uc = new UpdateClass();
            uc.UpdateValue<ulong>(ObjectFields.OBJECT_FIELD_GUID, this.Guid);
            uc.UpdateValue<uint>(ObjectFields.OBJECT_FIELD_TYPE, (uint)this.ObjectType);
            uc.UpdateValue<uint>(ObjectFields.OBJECT_FIELD_ENTRY, this.Entry);
            uc.UpdateValue<float>(ObjectFields.OBJECT_FIELD_SCALE_X, this.Scale);
            uc.UpdateValue<uint>(ObjectFields.OBJECT_FIELD_PADDING, 0);
            uc.UpdateValue<uint>(GameObjectFields.GAMEOBJECT_DISPLAYID, Template.DisplayId);
            uc.UpdateValue<uint>(GameObjectFields.GAMEOBJECT_FLAGS, Template.Flags);
            uc.UpdateValue<uint>(GameObjectFields.GAMEOBJECT_FACTION, Template.Faction);
            uc.UpdateValue<uint>(GameObjectFields.GAMEOBJECT_STATE, (uint)State);

            for (int i = 0; i < 4; i++)
                uc.UpdateValue<float>(GameObjectFields.GAMEOBJECT_ROTATION, Rotation[i], i);

            uc.UpdateValue<float>(GameObjectFields.GAMEOBJECT_POS_X, this.Location.X);
            uc.UpdateValue<float>(GameObjectFields.GAMEOBJECT_POS_Y, this.Location.Y);
            uc.UpdateValue<float>(GameObjectFields.GAMEOBJECT_POS_Z, this.Location.Z);
            uc.UpdateValue<float>(GameObjectFields.GAMEOBJECT_FACING, this.Orientation);
            uc.BuildPacket(ref packet, true);
            return packet;
        }

        public void Use(Player p)
        {
            //Triggers spell
            switch (this.Template.Type)
            {
                case (uint)GameObjectTypes.TYPE_CHEST:
                    if (this.State == (byte)GameObjectStates.GO_STATE_READY && LootRecipient == null)
                    {
                        this.State = (byte)GameObjectStates.GO_STATE_ACTIVE;
                        LootRecipient = p;

                        p.SendLoot(this.Guid);

                        GridManager.Instance.SendSurrounding(this.BuildUpdate(), this);
                    }
                    break;
            }
        }

        public void GenerateLoot()
        {
            this.Loot.Clear();
            HashSet<LootItem> loot = new HashSet<LootItem>();
            Dictionary<int, List<LootItem>> lootgroups = Database.GameObjectLoot.TryGet(this.LootId)
                                                         .GroupBy(x => x.GroupId).ToDictionary(gr => gr.Key, gr => gr.ToList());
            if (lootgroups.Count == 0)
                return;

            int maxKey = lootgroups.Max(x => x.Key);

            for (int i = 0; i <= maxKey; i++)
            {
                if (!lootgroups.ContainsKey(i))
                    continue;

                float rollchance = (float)new Random().NextDouble() * 100f;
                for (int x = 0; x < lootgroups[i].Count; x++)
                {
                    if (lootgroups[i][x].Chance >= 100 && Database.ItemTemplates.ContainsKey(lootgroups[i][x].Item))
                    {
                        loot.Add(lootgroups[i][x]);
                        break;
                    }

                    rollchance -= lootgroups[i][x].Chance;
                    if (rollchance <= 0 && Database.ItemTemplates.ContainsKey(lootgroups[i][x].Item))
                    {
                        loot.Add(lootgroups[i][x]);
                        break;
                    }
                }
            }

            foreach (LootItem li in loot.ToArray()) //Generate loot based on item chance
            {
                if (!Database.ItemTemplates.ContainsKey(li.Item))
                    continue;

                Item item = Database.ItemTemplates.CreateItemOrContainer(li.Item);
                item.CurrentSlot = (uint)item.EquipSlot;
                item.Owner = this.Guid;
                item.Contained = this.Guid;
                item.Type = (InventoryTypes)item.Template.InvType;
                item.DisplayID = item.Template.DisplayID;
                Database.Items.TryAdd(item);

                this.Loot.Add(new LootObject()
                {
                    Item = item,
                    Count = (uint)(new Random().Next(li.MinCount, li.MaxCount)),
                    IsQuestItem = li.QuestItem
                });
            }
        }
    }
}
