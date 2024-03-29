﻿using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static S_PlayerList;

 
class PacketHandler
{
	public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
	{ 
		S_BroadcastEnterGame pkt = packet as S_BroadcastEnterGame;
		ServerSession serverSession = session as ServerSession;
	}

	public static void S_BroadcastAttackHandler(PacketSession session, IPacket packet)
	{

	}
	public static void S_BroadcastFireObjIdxHandler(PacketSession session, IPacket packet)
	{

	}

	public static void S_BroadcastHandler(PacketSession session, IPacket packet)
	{

	}

	public static void S_BroadcastDamageHandler(PacketSession session, IPacket packet)
	{

	}
	public static void S_BroadcastUpdateScoreHandler(PacketSession session, IPacket packet)
	{

	}
	
	public static void S_BroadcastChatHandler(PacketSession session, IPacket packet)
	{
		//S_BroadcastEnterGame pkt = packet as S_BroadcastEnterGame;
		//ServerSession serverSession = session as ServerSession;
	}

	public static void S_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
	{
		S_BroadcastLeaveGame pkt = packet as S_BroadcastLeaveGame;
		ServerSession serverSession = session as ServerSession;
	}

	public static void S_PlayerListHandler(PacketSession session, IPacket packet)
	{
		S_PlayerList pkt = packet as S_PlayerList;
		ServerSession serverSession = session as ServerSession;
	}
	public static void S_BroadcastMoveHandler(PacketSession session, IPacket packet)
	{
		S_BroadcastMove pkt = packet as S_BroadcastMove;
		ServerSession serverSession = session as ServerSession;
	}  
}

