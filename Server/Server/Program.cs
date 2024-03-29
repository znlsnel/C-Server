﻿using System;
using static System.Collections.Specialized.BitVector32;
using System.Net;
using System.Text;
using System.Threading;

using ServerCore;
  
namespace Server
{
	class GameSession : Session
	{
		int count = 0;
		public override void OnConnected(EndPoint endPoint)
		{
			// client에게 보낸다 
			byte[] sendBuff = Encoding.UTF8.GetBytes($"hello client! this is server{count++}!");
			Send(sendBuff);

			Thread.Sleep(100);
			Disconnect();
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			Console.WriteLine($" OnDisconnected :  {endPoint}");

		}

		public override void OnRecv(ArraySegment<byte> buffer)
		{
			string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
			Console.WriteLine($"[From Client] {recvData}");
		}

		public override void OnSend(int numOfBytes)
		{
			Console.WriteLine($" Transferred bytes :  {numOfBytes}");

		}
	}

	class Program
	{
		static Listener _listener = new Listener();

		static void Main(string[] args)
		{
			Console.WriteLine("===========Im Server===============");

			// DNS  (Domain Name System)
			// 172.1.2.3 이러한 IP 주소를 www.naver.com 과 같은 형태로 바꿔줌
			string host = Dns.GetHostName();
			//string IpAddress = "fe80::e03e:ef3d:d51c:74be%7";
			string IpAddress = "192.168.219.200";

			IPHostEntry ipHost = Dns.GetHostEntry(host);
			//IPAddress ipAddr =  ipHost.AddressList[0];

			IPAddress ipAddr = IPAddress.Parse(IpAddress);
			IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

			_listener.Init(endPoint, () => { return new GameSession(); });

			while (true)
			{
				;
			}

		}

	}
}
