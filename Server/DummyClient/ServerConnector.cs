﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{ 
	class ServerConnector
	{

		//Func<Sesson> _sessionFactory; 
		public void Connect(IPEndPoint endPoint, Action<Socket> onAcceptHandler)
		{
			//_onAcceptHandler += onAcceptHandler;

			Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			 
			SocketAsyncEventArgs args = new SocketAsyncEventArgs();
			args.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnectCompleted);
			args.RemoteEndPoint = endPoint;
			args.UserToken = socket;

			RegisterConnect(args);
		}
		 
		void RegisterConnect(SocketAsyncEventArgs args)
		{
			Socket socket = args.UserToken as Socket;
			if (socket == null)
				return;

			bool pending = socket.ConnectAsync(args);
			if (pending == false)
				OnConnectCompleted(null, args);
			 
		}

		void OnConnectCompleted(Object sender, SocketAsyncEventArgs args)
		{
			if (args.SocketError == SocketError.Success)
			{
				//_onAcceptHandler.Invoke(args.ConnectSocket);
			}
			else
			{
				Console.WriteLine($"OnConnectedCompleted Fail: {args.SocketError.ToString()}");
			} 
		}

	}
}
