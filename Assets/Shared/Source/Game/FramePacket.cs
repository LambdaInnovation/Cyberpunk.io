using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EntitySyncData {
	public ushort entityID;
	public ComponentSyncData[] components;
}

public class ComponentSyncData {
	public byte componentID;
	public byte[] data;
}

public class FramePacket {

	public EntitySyncData[] entities;

	public void Read(BinaryReader reader) {
		ushort entityCount = reader.ReadUInt16();
		entities = new EntitySyncData[entityCount];
		for (int i = 0; i < entityCount; ++i) {
			var edata = new EntitySyncData();
			entities[i] = edata;

			edata.entityID = reader.ReadUInt16();

			var componentCount = reader.ReadByte();
			edata.components = new ComponentSyncData[componentCount];
			for (int j = 0; j < componentCount; ++j) {
				var cdata = new ComponentSyncData();
				edata.components[i] = cdata;

				cdata.componentID = reader.ReadByte();
				var dataSize = reader.ReadUInt16();
				cdata.data = reader.ReadBytes(dataSize);
			}
		}
	}

	public void Write(BinaryWriter writer) {
		writer.Write((ushort) entities.Length);
		foreach (var ent in entities) {
			writer.Write((ushort) ent.entityID);
			writer.Write((byte) ent.components.Length);

			foreach (var component in ent.components) {
				writer.Write((byte) component.componentID);
				writer.Write((ushort) component.data.Length);
				writer.Write(component.data);
			}
		}
	}

	public override string ToString() {
		return string.Format("FramePacket(entityCount={0})", entities.Length);
	}

}