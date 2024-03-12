﻿using ServerCore;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
 

class PacketHandler 
{ 
	public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
	{  
		ClientSession clientSession = session as ClientSession;
			 
		if (clientSession.Room == null)
			return; 
			   
		GameRoom room = clientSession.Room; 
		room.Push(() => room.Leave(clientSession));
	}
	   
	public static void C_MoveHandler(PacketSession session, IPacket packet)
	{
		C_Move movePacket = packet as C_Move;
		ClientSession clientSession = session as ClientSession;
		 
		Console.WriteLine($"Pos : {movePacket.posX}, {movePacket.posY}, {movePacket.posZ}");
		if (clientSession.Room == null)
			return;   
		   
		GameRoom room = clientSession.Room;
		room.Push(() => room.Move(clientSession, movePacket));
	}  
}

