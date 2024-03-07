﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
	public class Listener 
	{ 
		Socket _listenSocket;
		Func<Session> _sessionFactory; 

		public void Init(IPEndPoint endPoint, Func<Session> seesionFactory, int register =  10, int backLog = 50) 
		{
			_sessionFactory += seesionFactory;  

			_listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			_listenSocket.Bind(endPoint);
			_listenSocket.Listen(backLog); 

			for (int i = 0; i < register; i++)
			{
				SocketAsyncEventArgs args = new SocketAsyncEventArgs();
				args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
				RegisterAccept(args);
			}
		}   
		
		public Session GetSession() { return _sessionFactory.Invoke(); }

		void RegisterAccept(SocketAsyncEventArgs args)
		{
			args.AcceptSocket = null;

			bool pending = _listenSocket.AcceptAsync(args);
			if (pending == false) // 비동긴데 동기마냥 연결된 경우
				OnAcceptCompleted(null, args); 
		}
                   
        
		void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
		{
			if (args.SocketError == SocketError.Success)
			{
				Session session = _sessionFactory.Invoke();
				session.Start(args.AcceptSocket);
				session.OnConnected(args.AcceptSocket.RemoteEndPoint);
			}  
			else
			{
				Console.WriteLine(args.SocketError.ToString());
			} 
			RegisterAccept(args);
		}

		public Socket Accept()
		{

			return _listenSocket.Accept();
		}
	}
}
