using System;
using Common.Constants;
using Common.Helpers;
using Common.Network.Packets;
using Common.Database.DBC;
using System.Linq;
using Common.Database.DBC.Structures;
using System.Collections.Generic;
using WorldServer.Network;
using WorldServer.Game.Structs;
using Common.Helpers.Extensions;
using WorldServer.Game.Managers;
using WorldServer.Packets.Handlers;
using System.Threading.Tasks;
using Common.Network;
using WorldServer.Storage;
using System.Collections.Concurrent;
using WorldServer.Game.Objects.PlayerExtensions.Trade;
using WorldServer.Game.Objects.PlayerExtensions.Quests;
using WorldServer.Game.Objects.PlayerExtensions.Skill;
using WorldServer.Game.Objects.PlayerExtensions.Loot;
using MySql.Data.MySqlClient;
using Common.Database;
using Newtonsoft.Json;
using System.Text;
using WorldServer.Game.Objects.UnitExtensions;

namespace WorldServer.Game.Objects
{
    [Table("characters")]
    public class Player : Unit
    {
        public uint AccountId;
        [JsonIgnore]
        public WorldManager Client;
        public string Name;
        public byte Race;
        public byte RaceMask;
        public byte ClassMask;
        public byte Gender;
        public byte Skin;
        public byte Face;
        public byte HairStyle;
        public byte HairColour;
        public byte FacialHair;
        public uint GuildGuid;
        public Creature NonCombatPet;
        public uint PetDisplayInfo;
        public uint PetLevel;
        public uint PetFamily;
        public bool IsOnline = false;
        public TStat XP = new TStat();
        public uint TalentPoints;
        public uint SkillPoints;
        public ulong CurrentTarget;
        public ulong CurrentSelection;
        public ulong CurrentLootTarget;
        public byte SheathState = 0;
        public bool LoggedIn = false;
        public ChatFlags ChatFlag = ChatFlags.CHAT_TAG_NONE;
        public string ChatAutoResponse = "";
        public byte PlayerFlags;
        public byte BankSlots;
        public byte ComboPoints;
        private int TaxiNodes;

        //Systems
        private StatManager StatSystem;
        public InventoryManager Inventory;

        //Misc
        public byte SwingError = 0;
        private long LastRegen = 0;

        public HashSet<int> Talents = new HashSet<int>();
        public Dictionary<ushort, MirrorSkillInfo> Skills = new Dictionary<ushort, MirrorSkillInfo>();
        public Dictionary<uint, Quest> Quests = new Dictionary<uint, Quest>();
        public Dictionary<uint, PlayerSpell> Spells = new Dictionary<uint, PlayerSpell>();
        public Dictionary<byte, ActionButton> ActionButtons = new Dictionary<byte, ActionButton>();
        
        //Social
        public List<ulong> FriendList = new List<ulong>();
        public List<ulong> IgnoreList = new List<ulong>();        

        //Teleport
        public bool TeleportSemaphore { get; private set; } = false;
        public bool TeleportShortSemaphore { get; private set; } = false;
        private Quaternion TeleportLocation = null;
        private uint TeleportMap = 0;
        private long TeleportTime = 0;

        //Visual
        public VirtualItemInfo[] VirtualItems = new VirtualItemInfo[3];

        //Trade
        public TradeData TradeInfo = null;
        public bool IsTrading = false;

        //Group
        public Group Group = null;
        public bool GroupInvited = false;
        public WhoPartyStatuses GroupStatus = WhoPartyStatuses.WHO_PARTY_STATUS_NOT_IN_PARTY;

        //Object query collections
        public ConcurrentDictionary<ulong, WorldObject> ObjectsInRange = new ConcurrentDictionary<ulong, WorldObject>();
        public ConcurrentBag<ulong> QueriedObjects = new ConcurrentBag<ulong>();
        public HashSet<uint> QueriedItems = new HashSet<uint>();

        public Player() { }

        public Player(ulong guid)
        {
            this.ObjectType |= ObjectTypes.TYPE_PLAYER;
            this.Guid = guid;
        }

        public Player(ref MySqlDataReader dr)
        {
            this.ObjectType |= ObjectTypes.TYPE_PLAYER;
            this.AccountId = Convert.ToUInt32(dr["account"]);
            this.Guid = Convert.ToUInt64(dr["guid"]);
            this.Name = dr["name"].ToString();
            this.Race = Convert.ToByte(dr["race"]);
            this.Class = Convert.ToByte(dr["class"]);
            this.Gender = Convert.ToByte(dr["gender"]);
            this.Level = Convert.ToByte(dr["level"]);
            this.XP.Current = Convert.ToUInt32(dr["xp"]);
            this.Money = Convert.ToUInt32(dr["money"]);
            this.Skin = Convert.ToByte(dr["skin"]);
            this.Face = Convert.ToByte(dr["face"]);
            this.HairStyle = Convert.ToByte(dr["hairstyle"]);
            this.HairColour = Convert.ToByte(dr["haircolour"]);
            this.FacialHair = Convert.ToByte(dr["facialhair"]);
            this.BankSlots = Convert.ToByte(dr["bankslots"]);
            this.Location.X = Convert.ToSingle(dr["position_x"]);
            this.Location.Y = Convert.ToSingle(dr["position_y"]);
            this.Location.Z = Convert.ToSingle(dr["position_z"]);
            this.Orientation = Convert.ToSingle(dr["orientation"]);
            this.Map = Convert.ToUInt32(dr["map"]);
            this.SkillPoints = Convert.ToUInt32(dr["skillpoints"]);
            this.TalentPoints = Convert.ToUInt32(dr["talentpoints"]);
            //this.TaxiNodes = Convert.ToInt32(dr["taximask"]);
        }

        #region Creation Functions

        public void Create()
        {
            CreateActionButton cActions = Database.CreateActionButtons.Values.FirstOrDefault(x => x.Race == this.Race && x.Class == this.Class);

            //Misc
            XP.Current = 0;
            TalentPoints = 0;
            SkillPoints = 0;
            MovementFlags = 0;
            DynamicFlags = 0;

            //Settings
            SetStartLocation();
            SetBaseStats();
            SetLevelStats();
            SetPowerType();
            SetRaceVariables();
            SetStartItems();
            SetStartProfiencies();

            this.UnitFlags = (uint)Common.Constants.UnitFlags.UNIT_FLAG_SHEATHE;
            this.MovementFlags = 0;

            if (Race == (byte)Races.RACE_TAUREN)
                this.Scale = (this.Gender == (byte)Genders.GENDER_MALE ? 1.3f : 1.25f); //Taurens have different scale

            //Calculate remaining items
            this.StatSystem = new StatManager(this);
            this.StatSystem.UpdateAll();
            this.GetXPForNextLevel();

            this.Save();
        }

        public void PreLoad()
        {
            Inventory = new InventoryManager(this);
            StatSystem = new StatManager(this);
            SetLevelStats();
            SetPowerType();
            SetRaceVariables();
            SetBaseStats();
            StatSystem.UpdateAll();
            this.GetXPForNextLevel();

            SetStartProfiencies();

            if (!this.LoggedIn)
            {
                Health.Current = Health.Maximum;
                Mana.Current = Mana.Maximum;
            }

            this.UnitFlags = 0;
            this.PlayerFlags = 0;
            this.DynamicFlags = 0;
            this.StandState = 0;
            this.IsAttacking = false;
            this.InCombat = false;
            this.MovementFlags = 0;
            Inventory.SetBaseAttackTime();
            this.ChatFlag = ChatFlags.CHAT_TAG_NONE;
            this.LastRegen = 0;

            this.TransportID = 0;
            this.TransportOrientation = 0;
            this.Transport = new Vector();

            this.IsTrading = false;
            this.TradeInfo = null;
            this.Group = null;

            ObjectsInRange = new ConcurrentDictionary<ulong, WorldObject>();
            QueriedObjects = new ConcurrentBag<ulong>();
            QueriedItems = new HashSet<uint>();

            GridManager.Instance.AddOrGet(this, true);

            foreach (Item item in Database.Items.Where(x => x.Value.Player == this.Guid).Select(x => x.Value))
                this.Inventory.GetBag(item.Bag).AddItem(item, item.CurrentSlot);

        }

        public void Login()
        {
            this.LoggedIn = true;

            LoadSocial();

            this.StatSystem.UpdateAll();
            Health.ResetCurrent();
            Rage.Current = 0;
            Energy.ResetCurrent();
            Energy.ResetCurrent();
            Focus.ResetCurrent();
            Mana.ResetCurrent();
            this.Group = null;

            GridManager.Instance.UpdateObject(this);
            this.FindObjectsInRange(true);
            this.UpdateSurroundingQuestStatus();
            this.LeaveCombat();
            this.Client.Send(QueryDetails(), true);
        }

        public void SendMOTD()
        {
            if (this.Client != null)
                foreach (string msg in Globals.WELCOME_MESSAGE.Split(new string[] { "\r\n" }, StringSplitOptions.None))
                    ChatManager.Instance.SendSystemMessage(this, msg.Trim());
        }

        public void Kick()
        {
            this.Client.CloseSocket();
        }

        public void Logout()
        {
            this.LoggedIn = false;
            this.CurrentSelection = 0;
            this.CurrentTarget = 0;
            this.ObjectsInRange.Clear();
            this.QueriedObjects = new ConcurrentBag<ulong>();
            this.QueriedItems.Clear();
            this.IsOnline = false;
            this.IsAttacking = false;
            this.InCombat = false;
            this.TransportID = 0;
            this.TransportOrientation = 0;
            this.Transport = new Vector();
            this.IsTrading = false;
            this.GroupInvited = false;

            if (this.Group != null)
                this.Group.TryRemoveMember(this);

            if (GridManager.Instance.Grids.ContainsKey(this.Grid.Value))
            {
                if (GridManager.Instance.Grids[this.Grid.Value].TryRemove(this))
                {
                    GridManager.Instance.SendSurrounding(BuildDestroy(), this);
                    Parallel.ForEach(GridManager.Instance.GetSurroundingObjects(this, true).Cast<Player>(), p =>
                    {
                        WorldObject dump;
                        if (p.ObjectsInRange.ContainsKey(this.Guid))
                            p.ObjectsInRange.TryRemove(this.Guid, out dump);
                    });
                }
            }

            this.Save();
        }

        private void SetStartLocation()
        {
            var startLoc = Database.CreatePlayerInfo.Values.First(x => x.Race == this.Race && x.Class == this.Class);

            //Location
            this.Location = new Vector(startLoc.X, startLoc.Y, startLoc.Z);
            this.Orientation = startLoc.O;
            this.Map = startLoc.Map;
            this.Zone = startLoc.Zone;
        }

        private void SetLevelStats()
        {
            LevelStatsInfo levelStats = Database.LevelStatsInfo.Values
                                        .First(x => x.Race == this.Race && x.Class == this.Class && x.Level == this.Level);

            //Attributes
            Strength.SetAll(levelStats.Str);
            Agility.SetAll(levelStats.Agi);
            Stamina.SetAll(levelStats.Stam);
            Intellect.SetAll(levelStats.Inte);
            Spirit.SetAll(levelStats.Spi);
        }

        private void SetBaseStats()
        {
            var baseStats = Database.ClassLevelStats.Values.First(x => x.Class == this.Class && x.Level == this.Level);

            //Base stats
            Health.SetAll((uint)baseStats.BaseHP);
            Mana.SetAll((uint)baseStats.BaseMana);
            Focus.SetAll(100);
            Energy.SetAll(100);
            Rage.Current = 0;
            Rage.Maximum = 100;
            this.BaseAttackTime = 1;
        }

        private void SetPowerType()
        {
            switch (Class)
            {
                case (byte)Classes.CLASS_WARRIOR:
                    PowerType = (byte)PowerTypes.TYPE_RAGE;
                    break;
                case (byte)Classes.CLASS_HUNTER:
                    PowerType = (byte)PowerTypes.TYPE_FOCUS;
                    break;
                case (byte)Classes.CLASS_ROGUE:
                    PowerType = (byte)PowerTypes.TYPE_ENERGY;
                    break;
                default:
                    PowerType = (byte)PowerTypes.TYPE_MANA;
                    break;
            }
        }

        private void SetRaceVariables()
        {
            ChrRaces race = null;
            if (!DBC.ChrRaces.TryGetValue(this.Race, out race))
                return;

            this.DisplayID = (uint)(this.Gender == (byte)Genders.GENDER_MALE ? race.m_MaleDisplayId : race.m_FemaleDisplayId);
            this.Faction = (uint)race.m_factionID;
            this.TaxiNodes = race.m_startingTaxiNodes;

            switch (Race)
            {
                case (byte)Races.RACE_HUMAN:
                    BoundingRadius = (this.Gender == (byte)Genders.GENDER_MALE ? 0.306f : 0.208f);
                    break;
                case (byte)Races.RACE_ORC:
                    BoundingRadius = (this.Gender == (byte)Genders.GENDER_MALE ? 0.372f : 0.236f);
                    break;
                case (byte)Races.RACE_DWARF:
                    BoundingRadius = 0.347f;
                    break;
                case (byte)Races.RACE_NIGHT_ELF:
                    BoundingRadius = (this.Gender == (byte)Genders.GENDER_MALE ? 0.389f : 0.306f);
                    break;
                case (byte)Races.RACE_UNDEAD:
                    BoundingRadius = 0.383f;
                    break;
                case (byte)Races.RACE_TAUREN:
                    BoundingRadius = (this.Gender == (byte)Genders.GENDER_MALE ? 0.9747f : 0.8725f);
                    break;
                case (byte)Races.RACE_GNOME:
                    BoundingRadius = 0.3519f;
                    break;
                case (byte)Races.RACE_TROLL:
                    BoundingRadius = 0.306f;
                    break;
                default:
                    break;
            }
        }

        private void SetStartItems()
        {
            CharStartOutfit startItems = DBC.CharStartOutfit.Values.FirstOrDefault(x => x.Match(this.Race, this.Class, this.Gender));
            if (startItems == null)
                return;

            for (int i = 0; i < startItems.m_InventoryType.Count(); i++)
            {
                uint entry = 0;

                if (startItems.m_InventoryType[i] < 1 ||
                    !uint.TryParse(startItems.m_ItemID[i].ToString(), out entry) ||
                    !Database.ItemTemplates.ContainsKey(entry))
                    continue;

                Item item = Database.ItemTemplates.CreateItemOrContainer(entry);
                item.Type = (InventoryTypes)startItems.m_InventoryType[i];
                item.DisplayID = (uint)startItems.m_DisplayItemID[i];
                item.CreateItem();

                if (Database.Items.TryAdd(item) || Database.Items.ContainsKey(item.Guid))
                {
                    uint slot = (uint)item.EquipSlot;
                    if (slot == (uint)InventorySlots.SLOT_MAINHAND)
                        this.BaseAttackTime = item.Template.WeaponSpeed;

                    this.Inventory.AddItem(item, slot, InventorySlots.SLOT_INBACKPACK);
                }
            }
        }

        private void SetStartProfiencies()
        {
            this.Spells.Clear();

            //Base spells
            this.Spells.Add(6603, new PlayerSpell(6603)); //Attack

            foreach (SkillLineAbility sla in DBC.SkillLineAbility.Values)
            {
                Spell spell = DBC.Spell[(uint)sla.m_spell];
                SkillLine sl = DBC.SkillLine[sla.m_skillLine];
                if (sl.m_skillType != 0)
                    continue;

                if (spell.baseLevel > this.Level)
                    continue;

                if ((sl.m_raceMask > 0 && !sl.m_raceMask.HasFlag(RaceMask)) ||
                    (sl.m_classMask > 0 && !sl.m_classMask.HasFlag(ClassMask)) ||
                    (sl.m_excludeRace > 0 && !sl.m_excludeRace.HasFlag(RaceMask)) ||
                    (sl.m_excludeClass > 0 && !sl.m_excludeClass.HasFlag(ClassMask)))
                    continue;

                if ((sla.m_raceMask > 0 && !sla.m_raceMask.HasFlag(RaceMask)) ||
                    (sla.m_classMask > 0 && !sla.m_classMask.HasFlag(ClassMask)) ||
                    (sla.m_excludeRace > 0 && !sla.m_excludeRace.HasFlag(RaceMask)) ||
                    (sla.m_excludeClass > 0 && !sla.m_excludeClass.HasFlag(ClassMask)))
                    continue;

                this.AddSkill((ushort)sl.m_ID);

                if (spell.baseLevel != 0 && !this.Spells.ContainsKey(spell.Id))
                    this.Spells.Add(spell.Id, new PlayerSpell(spell));
            }

            //Base Language and Attack etc
            var skillinfo = Database.CreateSkillInfo.Values
                            .Where(x => x.Race == this.Race && x.Class == this.Class)
                            .Select(x => x.SkillID);

            foreach (ushort skill in skillinfo)
                this.AddSkill(skill);
        }

        public void SendInitialSpells()
        {
            ushort slot = 1;
            PacketWriter pkt = new PacketWriter(Opcodes.SMSG_INITIAL_SPELLS);
            pkt.WriteUInt8(0);
            pkt.WriteUInt16((ushort)this.Spells.Count);
            foreach (uint spellid in this.Spells.Keys)
            {
                pkt.WriteUInt16((ushort)spellid);
                pkt.WriteUInt16(slot++);
            }
            pkt.WriteUInt16(0);

            this.Client.Send(pkt);
        }

        #endregion

        #region Visualisation Functions
        public void SetVirtualItem(int slot, ItemTemplate it)
        {
            if (slot > 2)
                return;

            if (it == null)
            {
                VirtualItems[slot].m_classID = 0;
                VirtualItems[slot].m_inventoryType = 0;
                VirtualItems[slot].m_material = 0;
                VirtualItems[slot].m_sheatheType = 0;
                VirtualItems[slot].m_subclassID = 0;
                VirtualItems[slot].m_display = 0;
            }
            else
            {
                VirtualItems[slot].m_classID = (byte)it.Type;
                VirtualItems[slot].m_inventoryType = (byte)it.InvType;
                VirtualItems[slot].m_material = (byte)it.Material;
                VirtualItems[slot].m_sheatheType = (byte)it.SheatheType;
                VirtualItems[slot].m_subclassID = (byte)it.Subtype;
                VirtualItems[slot].m_display = it.DisplayID;
            }
        }

        public void SetSheath(byte sheath)
        {
            SetVirtualItem(0, null);
            SetVirtualItem(1, null);
            SetVirtualItem(2, null);

            switch (sheath)
            {
                case 0:
                    Flag.SetFlag(ref this.UnitFlags, (uint)Common.Constants.UnitFlags.UNIT_FLAG_SHEATHE);
                    IsArmed = false;
                    break;
                case 1:
                    Flag.RemoveFlag(ref this.UnitFlags, (uint)Common.Constants.UnitFlags.UNIT_FLAG_SHEATHE);
                    SetVirtualItem(0, this.Inventory.Backpack.GetItem((byte)InventorySlots.SLOT_MAINHAND)?.Template);
                    SetVirtualItem(1, this.Inventory.Backpack.GetItem((byte)InventorySlots.SLOT_OFFHAND)?.Template);
                    IsArmed = true;
                    break;
                case 2:
                    Flag.RemoveFlag(ref this.UnitFlags, (uint)Common.Constants.UnitFlags.UNIT_FLAG_SHEATHE);
                    SetVirtualItem(2, this.Inventory.Backpack.GetItem((byte)InventorySlots.SLOT_RANGED)?.Template);
                    IsArmed = true;
                    break;
            }

            this.SheathState = sheath;
        }

        public void ToggleChatFlag(ChatFlags flag)
        {
            if (this.ChatFlag == flag)
            {
                this.ChatFlag = ChatFlags.CHAT_TAG_NONE;

                if (flag == ChatFlags.CHAT_TAG_AFK)
                    Flag.RemoveFlag(ref PlayerFlags, (byte)Common.Constants.PlayerFlags.PLAYER_FLAGS_AFK);
                else if (flag == ChatFlags.CHAT_TAG_DND)
                    Flag.RemoveFlag(ref PlayerFlags, (byte)Common.Constants.PlayerFlags.PLAYER_FLAGS_DND);
            }
            else
            {
                this.ChatFlag = flag;

                if (flag == ChatFlags.CHAT_TAG_AFK)
                    Flag.SetFlag(ref PlayerFlags, (byte)Common.Constants.PlayerFlags.PLAYER_FLAGS_AFK);
                else if (flag == ChatFlags.CHAT_TAG_DND)
                    Flag.SetFlag(ref PlayerFlags, (byte)Common.Constants.PlayerFlags.PLAYER_FLAGS_DND);
            }

            this.Dirty = true;
        }

        public void AddActionButton(byte button, ushort action, byte type, byte misc)
        {
            ActionButton ab = new ActionButton(action, misc, type);
            this.ActionButtons.Add(button, ab);
        }

        public void RemoveActionButton(byte button)
        {
            if (this.ActionButtons.ContainsKey(button))
                this.ActionButtons.Remove(button);
        }
        #endregion

        #region Quest Functions
        public void GiveXp(uint xp, ulong enemyguid = 0)
        {
            if (this.Level >= MaxLevel || this.IsDead)
                return;

            uint xpval = this.XP.Current + xp;

            PacketWriter pw = new PacketWriter(Opcodes.SMSG_LOG_XPGAIN);
            pw.WriteUInt64((enemyguid == 0 ? this.Guid : enemyguid)); //Victim or Self
            pw.WriteUInt32(xp); //XP Amount
            pw.WriteUInt8((byte)(enemyguid == 0 ? 0 : 1)); //0 = Kill, 1 = Non Kill

            if (enemyguid > 0)
            {
                pw.WriteUInt32(xp); //Not sure if these are needed
                pw.WriteFloat(1);
            }

            this.Client.Send(pw);

            if (xpval >= this.XP.Maximum) //Level up!
            {
                this.XP.Current = (xpval - this.XP.Maximum); //Set the overload as current
                this.GetXPForNextLevel(); //Set the new max
                GiveLevel((byte)(this.Level + 1));
            }
            else
                this.XP.Current = xpval;

            this.Dirty = true;
        }

        public void GiveLevel(byte level)
        {
            if (this.Level == level)
                return;

            this.Level = level;
            SetLevelStats();
            SetBaseStats();

            var basestats = Database.ClassLevelStats.Values.First(x => x.Class == this.Class && x.Level == 1);
            var baseattrs = Database.LevelStatsInfo.Values.First(x => x.Race == this.Race && x.Class == this.Class && x.Level == 1);

            PacketWriter pk = new PacketWriter(Opcodes.SMSG_LEVELUP_INFO);
            pk.WriteUInt32(level);
            pk.WriteUInt32((uint)(Health.BaseAmount - basestats.BaseHP));
            pk.WriteUInt32((uint)(Mana.BaseAmount - basestats.BaseMana));
            pk.WriteUInt32(0);
            pk.WriteUInt32(0);
            pk.WriteUInt32(0);
            pk.WriteUInt32(0);
            pk.WriteUInt32(Strength.BaseAmount - baseattrs.Str);
            pk.WriteUInt32(Agility.BaseAmount - baseattrs.Agi);
            pk.WriteUInt32(Stamina.BaseAmount - baseattrs.Stam);
            pk.WriteUInt32(Intellect.BaseAmount - baseattrs.Inte);
            pk.WriteUInt32(Spirit.BaseAmount - baseattrs.Spi);
            this.Client.Send(pk);

            StatSystem.UpdateAll();

            if (!this.IsDead)
                Health.ResetCurrent();

            Rage.Current = 0;
            Energy.ResetCurrent();
            Energy.ResetCurrent();
            Focus.ResetCurrent();
            Mana.ResetCurrent();

            this.SkillPoints = 0;
            this.TalentPoints = 0;
            for (int i = 2; i <= this.Level; i++) //First points start from level 2
            {
                this.SkillPoints += (uint)(i % 5 == 0 ? 2 : 1); //Skillpoints are 1 per level, 2 per mod of 5
                this.TalentPoints += 10 + (uint)(Math.Floor(this.Level / 10d) * 5); //Talent points are base 10, +5 every 10 levels
            }

            this.UpdateSurroundingQuestStatus();
        }
        #endregion

        #region Item Functions

        public bool AddItem(uint entry, uint amount)
        {
            if (!Database.ItemTemplates.ContainsKey(entry))
                return false;

            if (!this.Inventory.CanStoreItem(entry, amount))
            {
                SendEquipError(InventoryError.EQUIP_ERR_CANT_CARRY_MORE_OF_THIS, null, null);
                return false;
            }

            ItemTemplate template = Database.ItemTemplates.TryGet(entry);
            if (amount <= template.MaxStackCount)
            {
                Item item = Database.ItemTemplates.CreateItemOrContainer(entry);
                item.StackCount = amount;
                AddItem(item);
                return true;
            }
            else
            {
                uint stackcount = amount;
                for (int i = 0; i < Math.Ceiling((float)amount / template.MaxStackCount); i++)
                {
                    uint amt = (stackcount >= template.MaxStackCount ? template.MaxStackCount : stackcount);

                    Item item = Database.ItemTemplates.CreateItemOrContainer(entry);
                    item.StackCount = amt;
                    AddItem(item);
                    stackcount -= amt;
                }

                return true;
            }
        }

        public bool AddItemInSlot(uint entry, uint slot = 0)
        {
            if (!Database.ItemTemplates.ContainsKey(entry))
                return false;

            if (this.Inventory.GetEntryCount(entry) > Database.ItemTemplates.TryGet(entry).MaxCount)
                return false;

            for (uint i = (uint)InventorySlots.SLOT_INBACKPACK; i > (uint)InventorySlots.SLOT_BAG1; i--)
            {
                Container container = this.Inventory.GetBag(i);
                if (container != null && !container.IsFull)
                {
                    Item item = Database.ItemTemplates.CreateItemOrContainer(entry);
                    item.Owner = this.Guid;
                    item.Contained = this.Guid;
                    item.Type = (InventoryTypes)item.Template.InvType;
                    item.DisplayID = item.Template.DisplayID;

                    if (container.AddItem(item))
                    {
                        switch (slot)
                        {
                            case (uint)InventorySlots.SLOT_MAINHAND:
                                SetVirtualItem(0, item.Template);
                                this.BaseAttackTime = item.Template.WeaponSpeed;
                                break;
                            case (uint)InventorySlots.SLOT_OFFHAND:
                                SetVirtualItem(1, item.Template);
                                break;
                            case (uint)InventorySlots.SLOT_RANGED:
                                SetVirtualItem(2, item.Template);
                                break;
                        }

                        this.Dirty = true;
                        return true;
                    }
                }
            }

            SendBuyError(BuyResults.BUY_ERR_CANT_CARRY_MORE, null, entry);
            return false;
        }

        public bool AddItem(Item item)
        {
            if (item == null)
                return false;

            if (!this.Inventory.CanStoreItem(item.Entry, 1))
            {
                SendEquipError(InventoryError.EQUIP_ERR_CANT_CARRY_MORE_OF_THIS, item, null);
                return false;
            }

            if (this.Inventory.AddItem(item))
            {
                this.Dirty = true;
                return true;
            }

            return false;
        }

        public bool RemoveItem(uint entry, uint count)
        {
            if (this.Inventory.GetEntryCount(entry) < count)
                return false;

            uint remainingcount = count;

            for (uint i = (uint)InventorySlots.SLOT_BAG1; i < (uint)InventorySlots.SLOT_INBACKPACK; i++)
            {
                Container container = this.Inventory.GetBag(i);
                if (container == null)
                    continue;

                if (container.HasItem(entry))
                {
                    foreach (Item item in container.Items.Values)
                    {
                        if (item.Entry == entry && item.StackCount <= remainingcount)
                        {
                            container.RemoveItem(item);
                            remainingcount -= item.StackCount;
                        }
                        else if (item.Entry == entry && item.StackCount > remainingcount)
                        {
                            item.StackCount -= remainingcount;
                            remainingcount -= item.StackCount;
                        }

                        if (remainingcount <= 0)
                        {
                            this.Dirty = true;
                            return true;
                        }

                    }
                }
            }

            return false;
        }

        public bool RemoveItem(Item item)
        {
            if (item == null)
                return false;

            for (uint i = (uint)InventorySlots.SLOT_BAG1; i < (uint)InventorySlots.SLOT_INBACKPACK; i++)
            {
                Container container = this.Inventory.GetBag(i);
                if (container == null)
                    continue;

                if (container.HasItem(item))
                {
                    if (container.RemoveItem(item))
                    {
                        this.Dirty = true;
                        return true;
                    }
                }
            }

            return false;
        }

        public Item GetItem(byte bag, byte slot)
        {
            uint bagslot = (uint)(bag == 255 ? 23 : bag);
            return this.Inventory.GetBag(bagslot)?.GetItem(slot);
        }

        public void SwapItem(byte srcbag, byte srcslot, byte dstbag, byte dstslot)
        {
            uint srcbagslot = (uint)(srcbag == 255 ? 23 : srcbag);
            uint dstbagslot = (uint)(dstbag == 255 ? 23 : dstbag);
            bool srcIsBackpack = srcbagslot == (uint)InventorySlots.SLOT_INBACKPACK;
            bool dstIsBackpack = dstbagslot == (uint)InventorySlots.SLOT_INBACKPACK;

            Item srcItem = this.Inventory.GetBag(srcbagslot)?.GetItem(srcslot);
            Item dstItem = this.Inventory.GetBag(dstbagslot)?.GetItem(dstslot);
            if (srcItem == null)
                return;

            if (this.IsDead)
            {
                SendEquipError(InventoryError.EQUIP_ERR_YOU_ARE_DEAD, srcItem, dstItem); //Dead
                return;
            }

            //Check backpack / paperdoll placement
            if (srcIsBackpack)
            {
                if (srcItem.Template.LevelReq > this.Level && InventoryManager.IsEquipmentPos(dstbagslot, dstslot))
                {
                    SendEquipError(InventoryError.EQUIP_ERR_CANT_EQUIP_LEVEL_I, srcItem, dstItem); //Not high enough level!
                    return;
                }
            }

            if (dstIsBackpack) //Destination slot checks
            {
                if (InventoryManager.IsEquipmentPos(dstbagslot, dstslot) && dstslot != srcItem.EquipSlot && srcItem.EquipSlot != (uint)InventorySlots.SLOT_INBACKPACK)
                {
                    SendEquipError(InventoryError.EQUIP_ERR_ITEM_DOESNT_GO_TO_SLOT, srcItem, dstItem); //Wrong slot for destination
                    return;
                }

                if (InventoryManager.IsBagPos(dstbagslot, dstslot) && srcItem.Type != InventoryTypes.BAG) //Destination bag slot check
                {
                    SendEquipError(InventoryError.EQUIP_ERR_ITEM_DOESNT_GO_TO_SLOT, srcItem, dstItem); //Wrong slot for destination
                    return;
                }
            }

            //If we have an original item that is being swapped to backpack
            if (dstItem != null && srcIsBackpack)
            {
                if (InventoryManager.IsEquipmentPos(srcbagslot, srcslot) || InventoryManager.IsBagPos(srcbagslot, srcslot)) //Switching to inventory to bag
                {
                    if (srcslot != dstItem.EquipSlot && dstItem.EquipSlot != (uint)InventorySlots.SLOT_INBACKPACK) //Check we can equip it there
                    {
                        SendEquipError(InventoryError.EQUIP_ERR_ITEM_DOESNT_GO_TO_SLOT, srcItem, dstItem); //Wrong slot for destination
                        return;
                    }
                    if (dstItem.Template.LevelReq > this.Level) //Check we can use it
                    {
                        SendEquipError(InventoryError.EQUIP_ERR_CANT_EQUIP_LEVEL_I, srcItem, dstItem); //Not high enough level!
                        return;
                    }
                    if (dstItem.Type == InventoryTypes.BAG && InventoryManager.IsEquipmentPos(srcbagslot, srcslot))
                    {
                        SendEquipError(InventoryError.EQUIP_ERR_ITEM_DOESNT_GO_TO_SLOT, srcItem, dstItem); //Wrong slot for destination
                        return;
                    }
                }
            }

            //prevent bag in bag
            if (srcIsBackpack && InventoryManager.IsBagPos(srcbagslot, srcslot) && srcslot == dstbag)
            {
                SendEquipError(InventoryError.EQUIP_ERR_NONEMPTY_BAG_OVER_OTHER_BAG, srcItem, dstItem);
                return;
            }
            else if (dstIsBackpack && InventoryManager.IsBagPos(dstbagslot, dstslot) && dstslot == srcbag)
            {
                SendEquipError(InventoryError.EQUIP_ERR_NONEMPTY_BAG_OVER_OTHER_BAG, dstItem, srcItem);
                return;
            }

            //check for requirements
            if (srcItem.Template.LevelReq > this.Level)
            {
                SendEquipError(InventoryError.EQUIP_ERR_CANT_EQUIP_LEVEL_I, srcItem, dstItem);
                return;
            }

            //Stack Items : return
            if (srcItem.Entry == dstItem?.Entry && dstItem.Template.MaxStackCount > dstItem.StackCount)
            {
                uint diff = dstItem.Template.MaxStackCount - dstItem.StackCount;
                if (diff >= srcItem.StackCount)
                {
                    //Destroy src stack
                    dstItem.StackCount += srcItem.StackCount;
                    this.Inventory.GetBag(srcbagslot)?.RemoveItemInSlot(srcslot);
                }
                else
                {
                    //Update stack values
                    srcItem.StackCount -= diff;
                    dstItem.StackCount = dstItem.Template.MaxStackCount;
                }

                this.Dirty = true;
                return;
            }

            //Do the actual transfer here

            //Remove items
            this.Inventory.GetBag(srcbagslot)?.RemoveItemInSlot(srcslot);
            this.Inventory.GetBag(dstbagslot)?.RemoveItemInSlot(dstslot);

            if (srcItem.IsContainer && InventoryManager.IsBagPos(srcbagslot, srcslot) && srcIsBackpack)
            {
                if (((Container)srcItem).IsEmpty)
                    this.Inventory.RemoveBag(srcslot);
                else
                {
                    SendEquipError(InventoryError.EQUIP_ERR_CAN_ONLY_DO_WITH_EMPTY_BAGS, srcItem, dstItem);
                    return;
                }
            }

            if (dstItem != null && dstItem.IsContainer && InventoryManager.IsBagPos(dstbagslot, dstslot) && dstIsBackpack)
            {
                if (((Container)dstItem).IsEmpty)
                    this.Inventory.RemoveBag(dstslot);
                else
                {
                    SendEquipError(InventoryError.EQUIP_ERR_CAN_ONLY_DO_WITH_EMPTY_BAGS, srcItem, dstItem);
                    return;
                }
            }

            //Bag transfers
            if (InventoryManager.IsBagPos(dstbagslot, dstslot) && dstIsBackpack && srcItem.IsContainer)
                this.Inventory.AddBag(dstslot, (Container)srcItem);

            if (dstItem != null && InventoryManager.IsBagPos(srcbagslot, srcslot) && srcIsBackpack && dstItem.IsContainer)
                this.Inventory.AddBag(srcslot, (Container)dstItem);

            //Add items
            this.Inventory.GetBag(dstbagslot)?.AddItem(srcItem, dstslot);
            this.Inventory.GetBag(srcbagslot)?.AddItem(dstItem, srcslot);

            if ((srcslot == (byte)InventorySlots.SLOT_MAINHAND && srcIsBackpack) ||
                (dstslot == (byte)InventorySlots.SLOT_MAINHAND && dstIsBackpack))
                Inventory.SetBaseAttackTime();

            this.Dirty = true;
        }

        public void SplitItem(byte srcbag, byte srcslot, byte dstbag, byte dstslot, byte count)
        {
            uint srcbagslot = (uint)(srcbag == 255 ? 23 : srcbag);
            uint dstbagslot = (uint)(dstbag == 255 ? 23 : dstbag);

            Item srcitem = this.Inventory.GetBag(srcbagslot)?.GetItem(srcslot);
            Item dstitem = this.Inventory.GetBag(dstbagslot)?.GetItem(dstslot);
            if (srcitem == null)
            {
                SendEquipError(InventoryError.EQUIP_ERR_ITEM_NOT_FOUND, srcitem, null);
                return;
            }

            if (srcitem.StackCount == count)
            {
                SendEquipError(InventoryError.EQUIP_ERR_COULDNT_SPLIT_ITEMS, srcitem, null);
                return;
            }

            if (srcitem.StackCount < count)
            {
                SendEquipError(InventoryError.EQUIP_ERR_TRIED_TO_SPLIT_MORE_THAN_COUNT, srcitem, null);
                return;
            }

            //Clone to a new item with count of 
            Item clone = srcitem.Clone(count);

            if (InventoryManager.IsEquipmentPos(dstbagslot, dstslot) || InventoryManager.IsBankSlot(dstbagslot, dstbag) ||
                InventoryManager.IsBagPos(dstbagslot, dstslot) || InventoryManager.IsInventoryPos(dstbagslot, dstslot))
            {
                if (dstitem != null)
                {
                    if (dstitem.Entry != srcitem.Entry)
                    {
                        SendEquipError(InventoryError.EQUIP_ERR_ITEM_DOESNT_GO_TO_SLOT, srcitem, null);
                        return;
                    }
                    else
                    {
                        if (dstitem.StackCount + count > dstitem.Template.MaxStackCount)
                        {
                            SendEquipError(InventoryError.EQUIP_ERR_ITEM_DOESNT_GO_TO_SLOT, srcitem, null);
                            return;
                        }
                    }

                    dstitem.StackCount += count;
                    srcitem.StackCount -= count;
                }
                else
                {
                    if (this.Inventory.GetBag(dstbagslot)?.AddItem(clone) == true)
                        srcitem.StackCount -= count;
                }

                if (InventoryManager.IsBagPos(dstbagslot, dstslot))
                {
                    //auto unequip two hand
                }
            }

            this.Dirty = true;
        }

        public void SendEquipError(InventoryError error, Item item1, Item item2)
        {
            PacketWriter pw = new PacketWriter(Opcodes.SMSG_INVENTORY_CHANGE_FAILURE);
            pw.WriteUInt8((byte)error);
            if (error != InventoryError.EQUIP_ERR_OK)
            {
                if (error == InventoryError.EQUIP_ERR_CANT_EQUIP_LEVEL_I)
                    pw.WriteUInt32(item1?.Template.LevelReq ?? 0);

                pw.WriteUInt64(item1?.Guid ?? this.Guid);
                pw.WriteUInt64(item2?.Guid ?? this.Guid);
                pw.WriteUInt8(0);
            }

            this.Client.Send(pw);
        }

        public void SendSellError(SellResults error, Creature vendor, ulong item)
        {
            PacketWriter pw = new PacketWriter(Opcodes.SMSG_SELL_ITEM);
            pw.WriteUInt64(vendor?.Guid ?? this.Guid);
            pw.WriteUInt64(item);
            pw.WriteUInt8((byte)error);
            this.Client.Send(pw);
        }

        public void SendBuyError(BuyResults error, Creature vendor, uint entry)
        {
            PacketWriter pw = new PacketWriter(Opcodes.SMSG_BUY_FAILED);
            pw.WriteUInt64(vendor?.Guid ?? this.Guid);
            pw.WriteUInt32(entry);
            pw.WriteUInt8((byte)error);
            this.Client.Send(pw);
        }

        #endregion

        #region Combat Functions
        public void SendAttackSwingNotInRange()
        {
            PacketWriter packet = new PacketWriter(Opcodes.SMSG_ATTACKSWING_NOTINRANGE);
            this.Client.Send(packet);
        }

        public void SendAttackSwingFacingWrongWay()
        {
            PacketWriter packet = new PacketWriter(Opcodes.SMSG_ATTACKSWING_BADFACING);
            this.Client.Send(packet);
        }

        private bool CanUseAttackType(AttackTypes attacktype)
        {
            switch (attacktype)
            {
                case AttackTypes.BASE_ATTACK:
                    return this.IsDisarmedMainHand;
                case AttackTypes.OFFHAND_ATTACK:
                    return this.IsDisarmedOffHand;
            }
            return true;
        }

        public void CalculateMinMaxDamage(AttackTypes attType, bool normalized, bool addTotalPct, out float min_damage, out float max_damage)
        {
            Item weapon;
            min_damage = 0;
            max_damage = 0;

            float weapon_mindamage = 0;
            float weapon_maxdamage = 0;
            float weapon_speed = 1.4f;

            float attack_power = 1; //Calulating this via vanilla formula as AP wasn't in Alpha
            switch (this.Class)
            {
                case (byte)Classes.CLASS_WARRIOR:
                case (byte)Classes.CLASS_PALADIN:
                    attack_power = (this.Strength.Current * 2) + (this.Level * 3) - 20;
                    break;
                case (byte)Classes.CLASS_DRUID:
                    attack_power = this.Strength.Current * 2 - 20;
                    break;
                case (byte)Classes.CLASS_HUNTER:
                    attack_power = this.Strength.Current + this.Agility.Current + (this.Level * 2) - 20;
                    break;
                case (byte)Classes.CLASS_MAGE:
                case (byte)Classes.CLASS_PRIEST:
                case (byte)Classes.CLASS_WARLOCK:
                    attack_power = this.Strength.Current - 10;
                    break;
                case (byte)Classes.CLASS_ROGUE:
                    attack_power = this.Strength.Current + ((this.Agility.Current * 2) - 20) + (this.Level * 2) - 20;
                    break;
                case (byte)Classes.CLASS_SHAMAN:
                    attack_power = (this.Strength.Current - 10) + ((this.Agility.Current * 2) - 20) + (this.Level * 2);
                    break;
            }


            if (attType == AttackTypes.BASE_ATTACK)
                weapon = this.Inventory.Backpack.GetItem((byte)InventorySlots.SLOT_MAINHAND);
            else
                weapon = this.Inventory.Backpack.GetItem((byte)InventorySlots.SLOT_OFFHAND);

            if (weapon != null)
            {
                weapon_mindamage = weapon.Template.DamageStats[0].Min;
                weapon_maxdamage = weapon.Template.DamageStats[0].Max;
                weapon_speed = weapon.Template.WeaponSpeed / 1000;
            }


            float base_value = (((this.Damage.Maximum + this.Damage.Current) / 2) + (attack_power / 14)) * weapon_speed;
            //float base_pct = GetModifierValue(unitMod, UnitModifierType.BASE_PCT);
            //float total_value = GetModifierValue(unitMod, UnitModifierType.TOTAL_VALUE);
            //float total_pct = addTotalPct ? GetModifierValue(unitMod, UnitModifierType.TOTAL_PCT) : 1.0f;

            /*
            if (IsInFeralForm())                                    //check if player is druid and in cat or bear forms
            {
                float weaponSpeed = UnitConst.BASE_ATTACK_TIME / 1000.0f;
                if (Item * weapon = GetWeaponForAttack(BASE_ATTACK, true))
                    weaponSpeed = weapon->GetTemplate()->Delay / 1000;

                if (GetShapeshiftForm() == FORM_CAT)
                {
                    weapon_mindamage = weapon_mindamage / weaponSpeed;
                    weapon_maxdamage = weapon_maxdamage / weaponSpeed;
                }
                else if (GetShapeshiftForm() == FORM_BEAR)
                {
                    weapon_mindamage = weapon_mindamage / weaponSpeed + weapon_mindamage / 2.5;
                    weapon_maxdamage = weapon_mindamage / weaponSpeed + weapon_maxdamage / 2.5;
                }
            }
            else*/
            if (!CanUseAttackType(attType))      //check if player not in form but still can't use (disarm case)
            {
                //off attack, set values to 0
                if (attType != AttackTypes.BASE_ATTACK)
                {
                    min_damage = 0;
                    max_damage = 0;
                    return;
                }
                weapon_mindamage = this.Damage.BaseAmount;
                weapon_maxdamage = this.Damage.Maximum;
            }

            min_damage = ((base_value + weapon_mindamage)); // * base_pct + total_value) * total_pct;
            max_damage = ((base_value + weapon_maxdamage)); // * base_pct + total_value) * total_pct;
        }

        public void LeaveCombat()
        {
            if (!this.InCombat || this.Attackers.Count > 0)
                return;

            CharHandler.HandleAttackStop(ref this.Client);

            this.SetAttackTimer(AttackTypes.BASE_ATTACK, 0);
            if (Inventory.HasOffhandWeapon())
                this.SetAttackTimer(AttackTypes.OFFHAND_ATTACK, 0);
        }

        public void GetXPForNextLevel()
        {
            this.XP.Maximum = (uint)((8 * this.Level) + XPDifficultyModifier()) * MobXPBaseModifier();
        }

        private uint XPDifficultyModifier()
        {
            uint lvl = this.Level;
            if (lvl <= 28)
                return 0;
            else if (lvl == 29)
                return 1;
            else if (lvl == 30)
                return 3;
            else if (lvl == 31)
                return 6;
            else
                return (5 * (lvl - 30));
        }

        private uint MobXPBaseModifier()
        {
            return (uint)(45 + (5 * this.Level));
        }

        public void ForceCastSpell(uint spellid)
        {
            if (!DBC.Spell.ContainsKey(spellid))
                return;

            SpellTargets targets = new SpellTargets();
            targets.Target = this;

            SpellCast cast = new SpellCast(this);
            cast.Targets = targets;
            cast.Spell = DBC.Spell[spellid];
            cast.Triggered = false;
            this.PrepareSpell(cast);
        }
        #endregion

        #region Update Functions
        public override void Update(long time)
        {
            if (!LoggedIn)
                return;

            base.Update(time);

            if (this.CurrentLootTarget != 0)
            {
                WorldObject loottarget = Database.Creatures.TryGet<WorldObject>(this.CurrentLootTarget) ??
                                         Database.GameObjects.TryGet<WorldObject>(this.CurrentLootTarget) ?? null;

                if (loottarget == null || loottarget.Location.Distance(this.Location) > Globals.INTERACT_DISTANCE)
                    LootExtension.LootRelease(this, this);
            }

            this.AttackUpdate();
            this.UpdateTeleport(time);

            if (this.Dirty) //Combine all updates to one call per update
            {
                if (this.Group != null)
                    this.Group.SendPartyMemberStats(this);

                this.Client.Send(BuildUpdate(UpdateTypes.UPDATE_FULL, true));
                this.Dirty = false;
            }
        }

        public void UpdateSurroundingPlayers()
        {
            Grid current = GridManager.Instance.Grids[this.Grid.Value];
            GridManager.Instance.UpdateObject(this);

            //Update us with the old
            foreach (Player p in current.Players.Values)
            {
                if (p.Guid == this.Guid)
                    continue;
                this.Client.Send(p.BuildDestroy(), true);
            }

            //Update us with the new and the new with us
            foreach (Player p in GridManager.Instance.GetSurroundingObjects(this, true))
            {
                if (!p.ObjectsInRange.ContainsKey(this.Guid) && p != this)
                    p.Client.Send(this.BuildUpdate(UpdateTypes.UPDATE_FULL), true);
                else if (p.ObjectsInRange.ContainsKey(this.Guid) && p != this)
                    p.Client.Send(this.BuildUpdate(), true);

                p.FindObjectsInRange(true);
            }

            //Update the old about us
            foreach (Player p in current.Players.Values)
            {
                if (p.Guid == this.Guid)
                    continue;
                p.FindObjectsInRange(true);
            }
        }

        public void Respawn()
        {
            this.Health.Current = this.Health.Maximum;
            this.Rage.Current = 0;
            this.Mana.ResetCurrent();
            this.Energy.ResetCurrent();
            this.Focus.ResetCurrent();

            this.IsDead = false;
            this.IsAttacking = false;
            this.Attackers.Clear();
            this.InCombat = false;
            this.UnitFlags = 0;
            this.DynamicFlags = 0;
        }

        public void Regenerate()
        {
            if (this.InCombat || this.IsAttacking)
                return;

            bool dirty = false;

            if (this.Health.Current < this.Health.Maximum)
            {
                dirty = true;
                this.Health.Current += 5;
            }
            if (this.Mana.Current < this.Mana.Maximum)
            {
                dirty = true;
                this.Mana.Current += 5;
            }
            if (this.Energy.Current < this.Energy.Maximum)
            {
                dirty = true;
                this.Energy.Current += 5;
            }
            if (this.Focus.Current < this.Focus.Maximum)
            {
                dirty = true;
                this.Focus.Current += 5;
            }
            if (this.Rage.Current > 0)
            {
                dirty = true;
                this.Rage.Current -= 5;
            }

            if (dirty && LastRegen <= Globals.Time) //Cap to every second
            {
                LastRegen = Globals.GetFutureTime(1);
                this.Dirty = true;
            }

        }

        public void AttackUpdate()
        {
            if (!this.IsAttacking)
            {
                if (this.InCombat)
                    this.LeaveCombat();

                //this.Regenerate();
                return;
            }

            if (Database.Creatures.TryGet(this.CombatTarget)?.IsDead == true)
            {
                LeaveCombat();
                return;
            }

            this.UpdateMeleeAttackingState();
        }

        public void Teleport(uint map, Quaternion location)
        {
            //Same map and not in an instance
            if (this.Map == map && this.Map <= 1)
            {
                PacketWriter movementStatus = new PacketWriter(Opcodes.SMSG_MOVE_WORLDPORT_ACK);
                movementStatus.WriteUInt64(0); //Transport ID
                movementStatus.WriteFloat(0); //Transport
                movementStatus.WriteFloat(0);
                movementStatus.WriteFloat(0);
                movementStatus.WriteFloat(0);
                movementStatus.WriteFloat(location.X); //Player
                movementStatus.WriteFloat(location.Y);
                movementStatus.WriteFloat(location.Z);
                movementStatus.WriteFloat(location.W);
                movementStatus.WriteFloat(0); //?
                movementStatus.WriteUInt32(0x08000000);
                this.Client.Send(movementStatus);

                this.TeleportShortSemaphore = true;
            }
            else
            {
                //Loading screen
                PacketWriter transferPending = new PacketWriter(Opcodes.SMSG_TRANSFER_PENDING);
                transferPending.WriteUInt32(map);
                this.Client.Send(transferPending);

                //Destroy self
                GridManager.Instance.SendSurrounding(BuildDestroy(), this);

                //New world transfer
                PacketWriter newWorld = new PacketWriter(Opcodes.SMSG_NEW_WORLD);
                newWorld.WriteUInt8((byte)map);
                newWorld.WriteFloat(location.X);
                newWorld.WriteFloat(location.Y);
                newWorld.WriteFloat(location.Z);
                newWorld.WriteFloat(location.W);
                this.Client.Send(newWorld);

                this.TeleportSemaphore = true;
            }

            this.TeleportLocation = location;
            this.TeleportMap = map;
            this.TeleportTime = Globals.GetFutureTime(0.1f); //Minimum wait
        }

        public void UpdateTeleport(long time)
        {
            if ((this.TeleportSemaphore || this.TeleportShortSemaphore) && this.TeleportTime <= time)
            {
                if (this.TeleportSemaphore)
                {
                    //Send login packets again
                    MiscHandler.HandleLoginSetTimespeed(ref this.Client);
                    this.PreLoad();
                    this.CurrentTarget = 0;
                    this.CurrentSelection = 0;
                }

                this.Inventory.SendInventoryUpdate();  //Create Inventory
                this.Map = TeleportMap;
                this.Orientation = TeleportLocation.W;
                this.Location = new Vector(TeleportLocation.X, TeleportLocation.Y, TeleportLocation.Z);
                this.TeleportSemaphore = false;
                this.TeleportShortSemaphore = false;
                this.TeleportTime = 0;

                if (this.Group != null)
                    this.Group.SendAllPartyStatus();

                UpdateSurroundingPlayers();
            }
        }
        #endregion

        #region Object Management Functions
        public void QueryItemCheck(uint entry)
        {
            if (entry == 0 || !this.LoggedIn)
                return;

            if (!this.QueriedItems.Contains(entry) && Database.ItemTemplates.ContainsKey(entry))
            {
                this.Client.Send(Database.ItemTemplates.TryGet(entry).QueryDetails());
                this.QueriedItems.Add(entry);
            }
        }

        public void FindObjectsInRange(bool clear = false)
        {
            Task.Factory.StartNew(() =>
            {
                if (clear)
                    this.ObjectsInRange.Clear();

                Parallel.ForEach(GridManager.Instance.GetSurroundingObjects(this), x =>
                {
                    if (x == this)
                        return;

                    if (this.Location.DistanceSqrd(x.Location) <= Math.Pow(Globals.UPDATE_DISTANCE, 2))
                    {
                        if (!this.ObjectsInRange.ContainsKey(x.Guid))
                        {
                            this.ObjectsInRange.TryAdd(x.Guid, x);

                            this.Client.Send(x.BuildUpdate(UpdateTypes.UPDATE_FULL), true);

                            //This forces the client to know the object's details
                            if (!this.QueriedObjects.Contains(x.Guid))
                            {
                                this.Client.Send(x.QueryDetails(), true); //Send query details
                                this.QueriedObjects.Add(x.Guid);
                            }
                        }
                    }
                    else if (this.ObjectsInRange.ContainsKey(x.Guid))
                    {
                        WorldObject dump;
                        if (this.ObjectsInRange.TryRemove(x.Guid, out dump))
                        {
                            this.Client.Send(x.BuildDestroy(), true);

                            if (this.CurrentSelection == x.Guid)
                            {
                                this.CurrentSelection = 0;
                                this.Dirty = true;
                            }

                            if (this.CurrentTarget == x.Guid)
                            {
                                this.CurrentTarget = 0;
                                this.Dirty = true;
                            }
                        }
                    }
                });
            });
        }
        #endregion

        #region Packet Functions

        public override PacketWriter QueryDetails()
        {
            PacketWriter nameCache = new PacketWriter(Opcodes.SMSG_NAME_QUERY_RESPONSE);
            nameCache.WriteUInt64(this.Guid);
            nameCache.WriteString(this.Name);
            nameCache.WriteUInt32(this.Race);
            nameCache.WriteUInt32(this.Gender);
            nameCache.WriteUInt32(this.Class);
            return nameCache;
        }

        public override PacketWriter BuildUpdate(UpdateTypes type = UpdateTypes.UPDATE_PARTIAL, bool self = false)
        {
            this.Dirty = !self;
            this.StatSystem.UpdateAll();
            this.Inventory.SendInventoryUpdate(self);

            //Send update packet
            PacketWriter packet = null;
            switch (type)
            {
                case UpdateTypes.UPDATE_FULL:
                    packet = CreateObject(self);
                    break;
                default:
                    packet = new PacketWriter(Opcodes.SMSG_UPDATE_OBJECT);
                    break;
            }

            UpdateClass uc = new UpdateClass();

            uc.Touch(ObjectTypes.TYPE_OBJECT);
            uc.Touch(ObjectTypes.TYPE_UNIT);
            uc.Touch(ObjectTypes.TYPE_PLAYER);

            //Object Fields
            uc.UpdateValue<ulong>(ObjectFields.OBJECT_FIELD_GUID, this.Guid);
            uc.UpdateValue<uint>(ObjectFields.OBJECT_FIELD_TYPE, (uint)this.ObjectType); //0x19 Player + Unit + Object
            uc.UpdateValue<uint>(ObjectFields.OBJECT_FIELD_ENTRY, 0);
            uc.UpdateValue<float>(ObjectFields.OBJECT_FIELD_SCALE_X, this.Scale);

            //Unit Fields
            uc.UpdateValue<uint>(UnitFields.UNIT_CHANNEL_SPELL, this.ChannelSpell);
            uc.UpdateValue<ulong>(UnitFields.UNIT_FIELD_CHANNEL_OBJECT, this.ChannelSpell);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_HEALTH, this.Health.Current);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_POWER1, this.Mana.Current);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_POWER2, this.Rage.Current);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_POWER3, this.Focus.Current);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_POWER4, this.Energy.Current);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_MAXHEALTH, this.Health.Maximum);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_MAXPOWER1, this.Mana.Maximum);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_MAXPOWER2, this.Rage.Maximum);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_MAXPOWER3, this.Focus.Maximum);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_MAXPOWER4, this.Energy.Maximum);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_LEVEL, this.Level);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_FACTIONTEMPLATE, this.Faction);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_BYTES_0, ByteConverter.ConvertToUInt32(this.Race, this.Class, this.Gender, this.PowerType));
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_STAT0, this.Strength.Current);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_STAT1, this.Agility.Current);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_STAT2, this.Stamina.Current);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_STAT3, this.Intellect.Current);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_STAT4, this.Spirit.Current);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_BASESTAT0, this.Strength.BaseAmount);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_BASESTAT1, this.Agility.BaseAmount);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_BASESTAT2, this.Stamina.BaseAmount);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_BASESTAT3, this.Intellect.BaseAmount);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_BASESTAT4, this.Spirit.BaseAmount);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_FLAGS, this.UnitFlags);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_COINAGE, this.Money);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_BASEATTACKTIME, this.BaseAttackTime); //Main hand
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_BASEATTACKTIME, 0, 1); //Offhand
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_RESISTANCES, Armor.BaseAmount + (Agility.Current * 2));
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_RESISTANCES, Holy.BaseAmount, 1);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_RESISTANCES, Fire.BaseAmount, 2);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_RESISTANCES, Nature.BaseAmount, 3);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_RESISTANCES, Frost.BaseAmount, 4);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_RESISTANCES, Shadow.BaseAmount, 5);
            uc.UpdateValue<float>(UnitFields.UNIT_FIELD_BOUNDINGRADIUS, this.BoundingRadius);
            uc.UpdateValue<float>(UnitFields.UNIT_FIELD_COMBATREACH, 1.5f);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_DISPLAYID, this.DisplayID);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE, Armor.PositiveAmount);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE, Holy.PositiveAmount, 1);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE, Fire.PositiveAmount, 2);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE, Nature.PositiveAmount, 3);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE, Frost.PositiveAmount, 4);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE, Shadow.PositiveAmount, 5);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE, Armor.NegativeAmount);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE, Holy.NegativeAmount, 1);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE, Fire.NegativeAmount, 2);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE, Nature.NegativeAmount, 3);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE, Frost.NegativeAmount, 4);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE, Shadow.NegativeAmount, 5);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_BYTES_1, ByteConverter.ConvertToUInt32(this.StandState, 0, this.ShapeShiftForm, this.SheathState));
            uc.UpdateValue<float>(UnitFields.UNIT_MOD_CAST_SPEED, 1f);
            uc.UpdateValue<uint>(UnitFields.UNIT_DYNAMIC_FLAGS, this.DynamicFlags);
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_DAMAGE, ByteConverter.ConvertToUInt32((ushort)this.Damage.Current, (ushort)this.Damage.Maximum));
            uc.UpdateValue<uint>(UnitFields.UNIT_FIELD_BYTES_2, ByteConverter.ConvertToUInt32(this.ComboPoints, 0, 0, 0));

            uc.UpdateValue<uint>(PlayerFields.PLAYER_FIELD_NUM_INV_SLOTS, 0x89); //Paperdoll + Bag Slots + Bag space
            uc.UpdateValue<uint>(PlayerFields.PLAYER_BYTES, ByteConverter.ConvertToUInt32(this.Skin, this.Face, this.HairStyle, this.HairColour));
            uc.UpdateValue<uint>(PlayerFields.PLAYER_XP, this.XP.Current);
            uc.UpdateValue<uint>(PlayerFields.PLAYER_NEXT_LEVEL_XP, this.XP.Maximum);
            uc.UpdateValue<uint>(PlayerFields.PLAYER_BYTES_2, ByteConverter.ConvertToUInt32(this.PlayerFlags, this.FacialHair, this.BankSlots, 0));

            uc.UpdateValue<uint>(PlayerFields.PLAYER_CHARACTER_POINTS1, this.TalentPoints);
            uc.UpdateValue<uint>(PlayerFields.PLAYER_CHARACTER_POINTS2, this.SkillPoints);
            uc.UpdateValue<float>(PlayerFields.PLAYER_BLOCK_PERCENTAGE, this.BlockPercentage);
            uc.UpdateValue<float>(PlayerFields.PLAYER_DODGE_PERCENTAGE, this.DodgePercentage);
            uc.UpdateValue<float>(PlayerFields.PLAYER_PARRY_PERCENTAGE, this.ParryPercentage);
            uc.UpdateValue<uint>(PlayerFields.PLAYER_BASE_MANA, this.Mana.Maximum);

            this.BuildSkillUpdate(ref uc);
            this.BuildQuestUpdate(ref uc);
            this.Inventory.BuildPacket(ref uc);

            switch (type)
            {
                case UpdateTypes.UPDATE_PARTIAL:
                    return UpdateObject(ref uc);
                case UpdateTypes.UPDATE_FULL:
                    uc.BuildPacket(ref packet, true);
                    return packet;
                default:
                    return null;
            }
        }
        #endregion

        #region Database Functions
        public void Save()
        {
            this.SaveBase();
            this.Inventory.Save();
            this.SaveSocial();
        }

        private void SaveBase()
        {
            List<string> columns = new List<string>() {
                "account", "guid", "name", "race", "class", "gender", "level","xp", "money", "skin", "face", "hairstyle",
                "haircolour","facialhair","bankslots","position_x","position_y","position_z","orientation","map","talentpoints",
                "skillpoints"
            };

            List<MySqlParameter> parameters = new List<MySqlParameter>()
            {
                new MySqlParameter("@account", this.AccountId),
                new MySqlParameter("@guid", this.Guid),
                new MySqlParameter("@name", this.Name),
                new MySqlParameter("@race", this.Race),
                new MySqlParameter("@class", this.Class),
                new MySqlParameter("@gender", this.Gender),
                new MySqlParameter("@level", this.Level),
                new MySqlParameter("@xp", this.XP.Current),
                new MySqlParameter("@money", this.Money),
                new MySqlParameter("@skin", this.Skin),
                new MySqlParameter("@face", this.Face),
                new MySqlParameter("@hairstyle", this.HairStyle),
                new MySqlParameter("@haircolour", this.HairColour),
                new MySqlParameter("@facialhair", this.FacialHair),
                new MySqlParameter("@bankslots", this.BankSlots),
                new MySqlParameter("@position_x", this.Location.X),
                new MySqlParameter("@position_y", this.Location.Y),
                new MySqlParameter("@position_z", this.Location.Z),
                new MySqlParameter("@orientation", this.Orientation),
                new MySqlParameter("@map", this.Map),
                new MySqlParameter("@talentpoints", this.TalentPoints),
                new MySqlParameter("@skillpoints", this.SkillPoints),
            };

            BaseContext.SaveEntity("characters", columns, parameters, Globals.CONNECTION_STRING);
        }


        private void SaveSocial()
        {
            BaseContext.ExecuteCommand(Globals.CONNECTION_STRING, "DELETE FROM character_social WHERE guid = @guid",
                                        new List<MySqlParameter>() { new MySqlParameter("@guid", this.Guid) });

            StringBuilder sb = new StringBuilder();
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            sb.AppendLine("INSERT INTO character_social (`guid`,`friend`,`ignore`) VALUES ");

            if(FriendList.Count > 0)
            {
                for (int i = 0; i < FriendList.Count; i++)
                {
                    parameters.Add(new MySqlParameter("@guid" + i, this.Guid));
                    parameters.Add(new MySqlParameter("@friend" + i, FriendList[i]));
                    parameters.Add(new MySqlParameter("@ignore" + i, false));

                    if (i > 0) sb.Append(",");
                    sb.AppendLine("(@guid" + i + ",@friend" + i + ",@ignore" + i + ")");
                }
                BaseContext.ExecuteCommand(Globals.CONNECTION_STRING, sb.ToString(), parameters);
            }

            if(IgnoreList.Count > 0)
            {
                parameters.Clear();

                for (int i = 0; i < IgnoreList.Count; i++)
                {
                    parameters.Add(new MySqlParameter("@guid" + i, this.Guid));
                    parameters.Add(new MySqlParameter("@friend" + i, IgnoreList[i]));
                    parameters.Add(new MySqlParameter("@ignore" + i, true));

                    if (i > 0) sb.Append(",");
                    sb.AppendLine("(@guid" + i + ",@friend" + i + ",@ignore" + i + ")");
                }
                BaseContext.ExecuteCommand(Globals.CONNECTION_STRING, sb.ToString(), parameters);
            }
        }

        private void LoadSocial()
        {
            if(Database.SocialList.ContainsKey(this.Guid))
            {
                this.IgnoreList.Clear();
                this.FriendList.Clear();

                List<SocialList> social = Database.SocialList.TryGet(this.Guid);
                foreach (var friend in social)
                {
                    if (!Database.Players.ContainsKey(friend.Friend))
                        continue;

                    if (friend.Ignore)
                        this.IgnoreList.Add(friend.Friend);
                    else
                        this.FriendList.Add(friend.Friend);
                }
            }

        }

        #endregion
    }
}
