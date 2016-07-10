using Common.Constants;
using Common.Network.Packets;
using WorldServer.Network;
using System;
using WorldServer.Game;
using System.Net;
using WorldServer.Game.Objects;
using WorldServer.Storage;

namespace WorldServer.Packets.Handlers
{
    public class AuthHandler
    {
        public static void HandleAuthSession(ref PacketReader packet, ref WorldManager manager)
        {
            PacketWriter writer = new PacketWriter(Opcodes.SMSG_AUTH_RESPONSE);

            uint version = packet.ReadUInt32();
            packet.ReadUInt32();
            string[] namepass = packet.ReadString().Split(new string[] {"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (namepass.Length != 2)
                writer.WriteUInt8((byte)AuthCodes.AUTH_UNKNOWN_ACCOUNT);
            else
            {
                string name = namepass[0];
                string pass = namepass[1];

                Account account = Database.Accounts.GetByName(name);
                if(account == null && Globals.AUTO_CREATE_ACCOUNT)
                {
                    if(!string.IsNullOrWhiteSpace(pass) && !string.IsNullOrWhiteSpace(name))
                    {
                        account = new Account();
                        account.Name = name;
                        account.GMLevel = 3;
                        account.SetPassword(pass);
                        account.IP = ((IPEndPoint)manager.Socket.RemoteEndPoint).Address.ToString();

                        if (Database.Accounts.TryAdd(account))
                            Database.Accounts.Save(account);
                        else
                            account = null;
                    }
                }


                if (version != Globals.CLIENT_VERSION)
                    writer.WriteUInt8((byte)AuthCodes.AUTH_VERSION_MISMATCH);
                else if (account == null)
                    writer.WriteUInt8((byte)AuthCodes.AUTH_UNKNOWN_ACCOUNT);
                else if (!account.ComparePassword(pass))
                    writer.WriteUInt8((byte)AuthCodes.AUTH_INCORRECT_PASSWORD);
                else
                {
                    account.IP = ((IPEndPoint)manager.Socket.RemoteEndPoint).Address.ToString();
                    manager.Account = account;
                    writer.WriteUInt8((byte)AuthCodes.AUTH_OK);
                }

            }
            

            manager.Send(writer);
        }
    }
}
