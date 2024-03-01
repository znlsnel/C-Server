﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{  
	public abstract class PacketSession : Session
	{
		public static readonly int HeaderSize = 2;
		// sealed =override 
		public sealed override int OnRecv(ArraySegment<byte> buffer)
		{
			int processLen = 0;
			  
			while (true)
			{
				// 최소한 헤더는 파싱할 수 있는지 체크!
				if (buffer.Count < HeaderSize)
					break;
				 
				// 패킷이 완전체로 도착했는지
				// ToUint16 -> 16비트 2바이트를 꺼내서 읽음 ( 즉 HeaderSize를 읽는거 ) 
				ushort dataSize = 	BitConverter.ToUInt16(buffer.Array, buffer.Offset);
				if (buffer.Count < dataSize)
					break;
				 
				// 여기까지 왔으면 패킷 조립 가능
				OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
				 
				processLen += dataSize;
				buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
			}
			return 0; 
		}

		public abstract void OnRecvPacket(ArraySegment<byte> buffer);

	}

	public abstract class Session
	{
		Socket _socket;
		int _disconnected = 0;
		 
		RecvBuffer _recvBuffer = new RecvBuffer(1024);
		
		
		object _lock = new object();
		Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
		List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
		SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
		SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
		 
		public abstract void OnConnected(EndPoint endPoint);
		public abstract int OnRecv(ArraySegment<byte> buffer)	;
		public abstract void OnSend(int numOfBytes); 
		public abstract void OnDisconnected(EndPoint endPoint);
		 
		public void Start (Socket socket)
		{ 
			_socket = socket;

			_recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
			_recvArgs.SetBuffer(new byte[1024], 0, 1024);

			_sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
			RegisterRecv();
		}

		public void Start()
		{


			_recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
			_recvArgs.SetBuffer(new byte[1024], 0, 1024);

			_sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
			RegisterRecv();
		}

		public void Send(ArraySegment<byte> data)
		{
			lock (_lock)
			{
				_sendQueue.Enqueue(data);
					RegisterSend();
				
                
			}
		}
		
		public void Disconnect()
		{
			if (Interlocked.Exchange(ref _disconnected, 1) == 1)
				return;

			OnDisconnected(_socket.RemoteEndPoint);
			_socket.Shutdown(SocketShutdown.Both);
			_socket.Close();
		}

		#region 네트워크 통신
		void RegisterRecv()
		{
			_recvBuffer.Clean();
			ArraySegment<byte> segment = _recvBuffer.WriteSegment;
			_recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);


			bool pending = _socket.ReceiveAsync(_recvArgs);
			if (pending == false)
			{
				OnRecvCompleted(null, _recvArgs);
			}
		}

		void RegisterSend()
		{


			while (_sendQueue.Count > 0)
			{
				ArraySegment<byte> buff = _sendQueue.Dequeue();
				_pendingList.Add(buff);
			} 
			_sendArgs.BufferList = _pendingList; 

			bool pending = _socket.SendAsync(_sendArgs);
			if (pending == false)
			{
				OnSendCompleted(null, _sendArgs);
			} 
		}  
		void OnSendCompleted(object sender, SocketAsyncEventArgs args)
		{
			lock ( _lock)
			{
				if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
				{
					try
					{ 
						_sendArgs.BufferList = null;
						_pendingList.Clear();

						OnSend(_sendArgs.BytesTransferred);

						if (_sendQueue.Count > 0)
							RegisterSend();


					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.ToString());
					}
				}
			}                 
		}


		void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
		{
			if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
			{
				try
				{
					if (_recvBuffer.OnWrite(args.BytesTransferred) == false)
					{
						Disconnect();
						return;
					}

					// 컨텐츠 쪽으로 데이터를 넘겨주고 얼마나 처리했는지 받는다.

					int processLen = OnRecv(_recvBuffer.ReadSegment);
					if (processLen < 0 || _recvBuffer.DataSize > processLen)
					{
						Disconnect();
						return;
					}

					if (_recvBuffer.OnRead(processLen) == false)
					{
						Disconnect();
						return;
					}

					RegisterRecv(); 
				}
				catch (Exception e)
				{ 
					Console.Write("o ");
					//Console.WriteLine(e.ToString());
				}

			}
			else
			{
				// TODO Disconnect
			}

		}
		#endregion
	}
}
