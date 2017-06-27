using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using System;
using System.IO;

[Game]
public class PlayerComponent : IFrameSyncComponent {

	public PlayerMetadata metadata = new PlayerMetadata();

    public void ReadBytes(BinaryReader reader)  {
        metadata.ReadBytes(reader);
    }

    public void WriteBytes(BinaryWriter writer) {
        metadata.WriteBytes(writer);
    }
}
