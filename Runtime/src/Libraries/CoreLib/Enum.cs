// Enum.cs
// Script#/Libraries/CoreLib
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System.Runtime.CompilerServices;

namespace System {

	[Imported(IsRealType = true)]
    [ScriptNamespace("ss")]
    public abstract class Enum : ValueType, IHashable<Enum> {
		public static Enum Parse(Type enumType, string value) {
			return null;
		}

		public static string ToString(Type enumType, Enum value) {
			return null;
		}

	    public bool Equals(Enum other) {
		    return false;
	    }

		public new int GetHashCode() {
			return 0;
		}
    }
}
