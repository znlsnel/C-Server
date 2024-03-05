﻿using ServerCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
 
namespace DummyClient
{
	public abstract class Packet
	{
		public ushort _size = 12;
		public ushort _packetId = 0;

		
		public abstract ArraySegment<byte> Write();
		public abstract void Read(ArraySegment<byte> s);
	}
	  
	class PlayerInfoReq 
	{
		public long playerId;
		public string name;

		public struct SkillInfo
		{
			public int id;
			public short level;
			public float duration; 

			public bool Write(Span<byte> s, ref ushort count)
			{
				bool success = true;
				success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), id);
				count += sizeof(int);
				success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), level);
				count += sizeof(short);
				success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), duration);
				count += sizeof(float);
				  
				return success;
			}
			 
			public void Read(ReadOnlySpan<byte> s, ref ushort count)
			{
				this.id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
				count += sizeof(int);
				this.level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
				count += sizeof(short);
				this.duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
				count += sizeof(float); 
			} 
		}

		public List<SkillInfo> skills = new List<SkillInfo>(); 
		
		public void Read(ArraySegment<byte> segment)
		{
			ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
			ushort count = 0;
			//ushort size = BitConverter.ToUInt16(s.Array, s.Offset);
			count += sizeof(ushort);
			//ushort id = BitConverter.ToUInt16(s.Array, s.Offset + count);
			count += sizeof(ushort);
			this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
			count += sizeof(long);

			ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
			count += sizeof(ushort);

			name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
			count += nameLen; 

			ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
			count += sizeof(ushort);

			skills.Clear(); 
			for (int i = 0; i < skillLen; i++)
			{
				SkillInfo skill = new SkillInfo();
				skill.Read(s, ref count);
				skills.Add(skill);
			} 
		}

		public ArraySegment<byte> Write()
		{ 
			ArraySegment<byte> segment = SendBufferHelper.Open(4096);
			Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
			 
			ushort count = 0; 
			bool success = true;

			count += sizeof(ushort);
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketId.PlayerInfoReq);
			count += sizeof(ushort);
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
			count += sizeof(long); 

			// string을 넣고 string의 길이를 반환함 
			ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
			count += sizeof(ushort); 
			count += nameLen; 

			// skill List
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)skills.Count);
			count += sizeof(ushort);
			foreach (SkillInfo skill in skills)
			{
				success &= skill.Write(s, ref count); 
			}

			success &= BitConverter.TryWriteBytes(s, count);


			if (success == false)
				return null;
			 
			return SendBufferHelper.close(count);
		}
	}

	
	public enum PacketId
	{
		PlayerInfoReq = 1,
		PlayerInfoOk = 2,
	}
	 
	public class ServerSession : Session
	{    
		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");

			PlayerInfoReq packet = new PlayerInfoReq() { playerId = 1021, name = "김우디" };
			packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 1, level = 3, duration = 1.24f });
			packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 2, level = 6, duration = 1.64f });
			packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 3, level = 9, duration = 1.34f });
			packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 4, level = 12, duration = 3.4f });
			packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 5, level = 15, duration = 8.14f });
			packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 6, level = 18, duration = 0.2244f });

			 
			//for (int i = 0; i < 5; i++) 
			{ 
				ArraySegment<byte> s =  packet.Write();
				   
				if(s != null)  
					Send(s); 

			}
		}
		 
		public override void OnDisconnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnDisConnected : {endPoint}");

		}

		public override int OnRecv(ArraySegment<byte> buffer)
		{
			string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
			Console.WriteLine($"[From Server] {recvData}");

			return buffer.Count;
		}

		public override void OnSend(int numOfBytes)
		{
			//	throw new NotImplementedException();
		}
	}
}
