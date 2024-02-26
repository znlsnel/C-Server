﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace DummyClient
{
	class Program
	{
		static void Main(string[] args)
		{
			 Console.WriteLine("===========Im Client===============");
			string host = Dns.GetHostName();
			IPHostEntry ipHost = Dns.GetHostEntry(host);
			IPAddress ipAddr = ipHost.AddressList[0];
			IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

			Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

			try
			{
				socket.Connect(endPoint);
				Console.WriteLine($"Connected To {socket.RemoteEndPoint.ToString()}");

				// 서버로 보내기
				byte[] sendBuffer = Encoding.UTF8.GetBytes("Hello Server! this is Client!");
				int sendBytes = socket.Send(sendBuffer);

				// 서버에게 받기
				byte[] recvBuff = new byte[1024];
				int recvBuffers = socket.Receive(recvBuff);
				string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBuffers);
				Console.WriteLine($"[From Server] {recvData}");

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}

        }
	}
}
