﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
	public class SendBufferHelper
	{
		// 현재 쓰레드에서만 고유하게 사용할 수 있는 전역 버퍼
		public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() =>{return null; });

		public static int ChunkSize { get; set; } = 65536 * 100;
		 
		public static ArraySegment<byte> Open(int reserveSize)
		{
			if (CurrentBuffer.Value == null)
				CurrentBuffer.Value = new SendBuffer(ChunkSize);
			if (CurrentBuffer.Value.FreeSize < reserveSize)
				CurrentBuffer.Value = new SendBuffer(ChunkSize);

			return CurrentBuffer.Value.Open(reserveSize);
		} 
		public static ArraySegment<byte> close(int usedSize)
		{
			return CurrentBuffer.Value.Close(usedSize);
		}
	} 

	public class SendBuffer
	{
		// [] []  [] []  [] []  [] []  [] []
		byte[] _buffer;
		int _usedSize = 0;
		 
		public int FreeSize { get { return _buffer.Length - _usedSize; } }

		public SendBuffer (int chunkSize)
		{
			_buffer = new byte[chunkSize];
		}

		public ArraySegment<byte> Open(int reserveSize)
		{
			if (reserveSize > FreeSize)
				return null;

			return new ArraySegment<byte>(_buffer, _usedSize, reserveSize);
		}
		public ArraySegment<byte> Close(int usedSize) 
		{
			ArraySegment<byte> buffer = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
			_usedSize += usedSize;
			return buffer;
		} 

	}
}
