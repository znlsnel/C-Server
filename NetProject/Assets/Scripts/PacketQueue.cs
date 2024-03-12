using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class PacketQueue
{
	public static PacketQueue Instance { get; } = new PacketQueue();

	Queue<IPacket> _packetQueue = new Queue<IPacket>();
	object _lock = new object();

	public void Push (IPacket pakcet)
	{
		lock (_lock)
		{
			_packetQueue.Enqueue(pakcet);
		}
	}

	public IPacket Pop()
	{
		lock (_lock)
		{
			if (_packetQueue.Count == 0)
				return null;

			return _packetQueue.Dequeue();
		}
	}

	public List<IPacket> PopAll()
	{
		List<IPacket> list = new List<IPacket>();

		lock (_lock)
		{
			while (_packetQueue.Count > 0)
			{
				list.Add(_packetQueue.Dequeue());	
			}
		}
		return list;
	}
}

