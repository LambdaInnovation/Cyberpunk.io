using System.Collections;
using System.Collections.Generic;
using Entitas;
using System.IO;

// Interface for components that are to be serialized over network via frame .
public interface IFrameSyncComponent : IComponent {

    void WriteBytes(Stream stream);

    void ReadBytes(Stream stream);

}
