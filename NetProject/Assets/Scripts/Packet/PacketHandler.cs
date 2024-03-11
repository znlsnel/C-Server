﻿using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
 

class PacketHandler
{
	public static void S_ChatHandler(PacketSession session, IPacket packet)
	{ 
		S_Chat chatPacket = (S_Chat)packet;
		ServerSession serverSession = session as ServerSession;
		//if (chatPacket.playerID == 1)
		{ 
			UnityEngine.Debug.Log(chatPacket.chat);

			GameObject go = GameObject.Find("Player");
			if (go == null)
				UnityEngine.Debug.Log("Player not Found");
			else
				UnityEngine.Debug.Log("Player Found"); 
			
		} 

	}
}
