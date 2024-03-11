
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DummyClient;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager(); 
	public static PacketManager Instance { get { return _instance; } }
	#endregion
 
	PacketManager()
	{
		Register(); 
	}

	Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc =
	new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
	Dictionary<ushort, Action<PacketSession, IPacket>> _handler = 
		new Dictionary<ushort, Action<PacketSession, IPacket>>();

		 
	public void Register()
	{
		
			_makeFunc.Add((ushort)PacketId.S_Chat, MakePacket<S_Chat>); 
			_handler.Add((ushort)PacketId.S_Chat, PacketHandler.S_ChatHandler); 


	}


	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallBack = null)
	{
		ushort count = 0;
		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
		if (_makeFunc.TryGetValue(id, out func))
		{

			IPacket packet = func.Invoke(session, buffer);
			if (onRecvCallBack != null)
				onRecvCallBack.Invoke(session, packet);
			else
				HandlePacket(session, packet);
		} 
	}



	T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
	{
		T pkt = new T();
		pkt.Read(buffer);

		return pkt;
	}

	public void HandlePacket(PacketSession session, IPacket packet)
	{
		Action<PacketSession, IPacket> action = null;
		if (_handler.TryGetValue(packet.Protocol, out action))
			action.Invoke(session, packet);
	}

}

