﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketGenerator
{
	class PacketFormat
	{
		// {0} 패킷 이름
		// {1} 멤버 변수들
		// {2} 멤버 변수 read
		// {3} 멤버 변수 write
		public static string packetFormat =
@" 
class {0} 
{{   
	{1} 
	public void Read(ArraySegment<byte> segment)
	{{
		ushort count = 0;
		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		count += sizeof(ushort);
		count += sizeof(ushort);
		
		{2}
	}}

	public ArraySegment<byte> Write()
	{{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
			 
		ushort count = 0; 
		bool success = true;

		count += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketId.{0});
		count += sizeof(ushort);

		{3}
 
		success &= BitConverter.TryWriteBytes(s, count);


		if (success == false)
			return null;
			 
		return SendBufferHelper.close(count);
	}}
}}
";

		// {0} 변수 형식
		// {1} 변수 이름 
		public static string memberFormat =
@"public {0} {1};";

		// {0} 리스트 이름 [대문자]
		// {1} 리스트 이름 [소문자]
		// {2} 맴버 변수들
		// {3} 맴버 변수 Read
		// {4} 맴버 변수 Write

		public static string memberListFormat =
@" 
public struct {0}
{{ 
	{2}
 
	public void Read(ReadOnlySpan<byte> s, ref ushort count)
	{{ 
		{3}
	}}  

	public bool Write(Span<byte> s, ref ushort count)
	{{
		bool success = true;
		{4}
				  
		return success;
	}}
			 

}}   
public List<{0}> {1}s = new List<{0}>(); 
";

		// {0} 변수 이름
		// {1} TO~ 변수 형식
		// {2} 변수 형식
		public static string readFormat =
@" 				
{0} = BitConverter.To{1}(s.Slice(count, s.Length - count));
count += sizeof({2});
"; 

		public static string readStringFormat =
@"
ushort {0}Len = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort); 
  
this.{0} = Encoding.Unicode.GetString(s.Slice(count, {0}Len));
count += {0}Len; 
";

		// {0} = 변수 이름
		// {1} = 변수 형식
		public static string writeFormat =
@"
success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.{0});
count += sizeof({1});   
";

		//{0} 변수 이름
		public static string writeStringFormat =
@"
ushort {0}Len = (ushort)Encoding.Unicode.GetBytes(this.{0}, 0, this.{0}.Length, segment.Array, segment.Offset + count + sizeof(ushort));
success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), {0}Len);
count += sizeof(ushort); 
count += {0}Len; 
"; 
	}
}