
using ServerCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text; 
using System.Threading.Tasks;

public enum PacketId
{
		PlayerInfoReq = 1,	Test = 2,
 
}
 
class PlayerInfoReq 
{   
	public long playerId;
	public string name;
	 
	public class Skill
	{  
		public int id;
		public short  level;
		public float duration;
		 
		public class Attribute
		{  
			public int att;
		 
			public void Read(ReadOnlySpan<byte> s, ref ushort count)
			{ 
				 				
				att = BitConverter.ToInt32(s.Slice(count, s.Length - count));
				count += sizeof(int);
				
			}  
		
			public bool Write(Span<byte> s, ref ushort count)
			{
				bool success = true;
				
				success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.att);
				count += sizeof(int);   
				
						   
				return success;
			}
					 
		
		}    
		public List<Attribute> attributes = new List<Attribute>(); 
		
	 
		public void Read(ReadOnlySpan<byte> s, ref ushort count)
		{ 
			 				
			id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
			count += sizeof(int);
			
			 				
			 level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
			count += sizeof(short);
			
			 				
			duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
			
			
			ushort attributeLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
			count += sizeof(ushort);
			
			attributes.Clear(); 
			for (int i = 0; i < attributeLen; i++)
			{
				Attribute attribute = new Attribute();
				attribute.Read(s, ref count);
				attributes.Add(attribute); 
			}
			
		}  
	
		public bool Write(Span<byte> s, ref ushort count)
		{
			bool success = true;
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.id);
			count += sizeof(int);   
			
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this. level);
			count += sizeof(short);   
			
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.duration);
			count += sizeof(float);   
			
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)attributes.Count);
			count += sizeof(ushort); 
			foreach (Attribute attribute in this.attributes)
			{ 
				success &= attribute.Write(s, ref count); 
			}
			
					   
			return success;
		}
				 
	
	}    
	public List<Skill> skills = new List<Skill>(); 
	 
	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		
		 				
		playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
		
		
		ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort); 
		  
		this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
		count += nameLen; 
		
		
		ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		
		skills.Clear(); 
		for (int i = 0; i < skillLen; i++)
		{
			Skill skill = new Skill();
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
		
		
		ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
		count += sizeof(ushort); 
		count += nameLen; 
		
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)skills.Count);
		count += sizeof(ushort); 
		foreach (Skill skill in this.skills)
		{ 
			success &= skill.Write(s, ref count); 
		}
		
 
		success &= BitConverter.TryWriteBytes(s, count);


		if (success == false)
			return null;
			 
		return SendBufferHelper.close(count);
	}
}
 
class Test 
{   
	public int testInt;
	public byte testByte; 
	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		
		 				
		testInt = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		
		this.testByte = (byte)segment.Array[segment.Offset + count];
		count += sizeof(byte);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
			 
		ushort count = 0; 
		bool success = true;

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketId.Test);
		count += sizeof(ushort);

		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.testInt);
		count += sizeof(int);   
		
		 segment.Array[segment.Offset + count] = (byte)this.testByte;
		count += sizeof(byte);
 
		success &= BitConverter.TryWriteBytes(s, count);


		if (success == false)
			return null;
			 
		return SendBufferHelper.close(count);
	}
}

