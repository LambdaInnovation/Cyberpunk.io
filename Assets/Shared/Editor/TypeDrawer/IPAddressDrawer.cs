using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using Entitas.VisualDebugging.Unity.Editor;
using UnityEditor;
using System.Net;
using System;

public class IPAddressDrawer : ITypeDrawer {
    object ITypeDrawer.DrawAndGetNewValue(Type memberType, string memberName, object value, object target) {
		EditorGUILayout.LabelField("Address", value.ToString());
        return value;
    }

    bool ITypeDrawer.HandlesType(Type type) {
        return type == typeof(IPAddress);
    }
}
