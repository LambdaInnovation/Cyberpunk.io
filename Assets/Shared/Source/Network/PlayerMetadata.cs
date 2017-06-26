using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[Serializable]
public sealed class PlayerMetadata {
	
	public string playerName = "Unnamed";

	public void WriteBytes(BinaryWriter writer) {
		writer.Write(playerName);
	}

	public void ReadBytes(BinaryReader reader) {
		playerName = reader.ReadString();
	}

}
