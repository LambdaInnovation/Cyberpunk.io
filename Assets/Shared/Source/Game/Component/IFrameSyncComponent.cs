using System.Collections;
using System.Collections.Generic;
using Entitas;
using System.IO;

// Interface for components that are to be serialized over network via frame .
public interface IFrameSyncComponent : IComponent {

    void WriteBytes(BinaryWriter writer);

    void ReadBytes(BinaryReader reader);

}
