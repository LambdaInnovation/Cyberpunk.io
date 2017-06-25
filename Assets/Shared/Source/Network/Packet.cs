using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using UnityEngine;

public enum PacketType : byte {
	Sync,
	SyncAck,
	Fin,
	FinAck,
	Frame,
	Input,
	Ping // Debug-usage only
}

public class Packet {

	public static Packet FromBytes(byte[] bytes) {
		PacketType packetType = (PacketType) bytes[0];
		byte[] dataBytes = new byte[bytes.Length - 1];
		Array.Copy(bytes, 1, dataBytes, 0, bytes.Length - 1);

		return new Packet(packetType, dataBytes);
	}

	public PacketType packetType;
	public byte[] data;

	public Packet(PacketType type, byte[] data) {
		this.packetType = type;
		this.data = data;
	}

	public byte[] ToBytes() {
		byte[] ret = new byte[data.Length + 1];
		ret[0] = (byte) packetType;
		Array.Copy(data, 0, ret, 1, data.Length);
		return ret;
	}

	public override string ToString() {
		return string.Format("Packet(type={0},data={1})", packetType, data);
	}

}

