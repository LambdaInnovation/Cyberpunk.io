using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SerializeTest {

	[MenuItem("Tools/Serialize Test")]
	public static void TestSerialize() {
		var a1 = new CA { a = 2, b = 3 };
		var b1 = new CB { c = 2.4f, d = 25.5f };
		var formatter = new BinaryFormatter();

		byte[] s1bytes;
		using (var stream = new MemoryStream()) {
			formatter.Serialize(stream, a1);
			formatter.Serialize(stream, b1);
			
			s1bytes = stream.ToArray();
			Debug.Log(s1bytes.Length + " => " + string.Join(",", Array.ConvertAll(s1bytes, Convert.ToString)));
		}

		using (var stream = new MemoryStream(s1bytes)) {
			var a2 = (CA) formatter.Deserialize(stream);
			//var b2 = (CB) formatter.Deserialize(stream);

			Debug.Log(a2);
			//Debug.Log(b2);
		}
	}

	[Serializable]
	public class CA {
		public int a, b;

		public override string ToString() {
			return string.Format("CA(a={0},b={1})", a, b);
		}
	}

	[Serializable]
	public class CB {
		public float c, d;

		public override string ToString() {
			return string.Format("CB(c={0},d={1})", c, d);
		}
	}

}
