using System;
using Shared.Enums;
using Shared.Handlers;
using Shared.Server;

namespace PacketDebugger
{
    public static class PacketHandler
    {
        [Handler(HandlerTypes.PacketHandler)]
        public static void PacketData(Req req, Res res)
        {
            if (!req.Headers.ContainsKey("osu-token")) return;


            while (true)
            {
                if (req.Reader.BaseStream.Length - req.Reader.BaseStream.Position < 7)
                    break; // Dont read any invalid packets! (less then bytelength of 7)

                short packetId = req.Reader.ReadInt16();
                req.Reader.ReadBoolean();
                byte[] packetData = req.Reader.ReadBytes();

                Console.WriteLine(
                    $"Packet: {packetId} Length: {packetData.Length} Data: {BitConverter.ToString(packetData).Replace("-", "")}");
            }
        }
    }
}