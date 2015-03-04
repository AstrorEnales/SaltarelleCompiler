using System.Runtime.CompilerServices;

namespace System {
	[Imported(ObeysTypeSystem = true)]
	[ScriptNamespace("ss")]
	public static class Convert {
		[InlineCode("{$System.Script}.enc64({inArray})")]
		public static string ToBase64String(byte[] inArray) {
			return null;
		}
		
		public static float ToSingle(bool value) {
			return 0;
		}

		[InlineCode("{$System.Script}.enc64({inArray}, {options})")]
		public static string ToBase64String(byte[] inArray, Base64FormattingOptions options) {
			return null;
		}
		
		// TODO: always throws InvalidCastException
		//public static float ToSingle(char value) {
		//	return 0;
		//}

		// TODO: always throws InvalidCastException
		//public static float ToSingle(DateTime value) {
		//	return 0;
		//}

		public static float ToSingle(byte value) {
			return 0;
		}

		[InlineCode("{$System.Script}.enc64({inArray}.slice({offset}, {offset} + {length}))")]
		public static string ToBase64String(byte[] inArray, int offset, int length) {
			return null;
		}
		
		public static float ToSingle(decimal value) {
			return 0;
		}

		[InlineCode("{$System.Script}.enc64({inArray}.slice({offset}, {offset} + {length}), {options})")]
		public static string ToBase64String(byte[] inArray, int offset, int length, Base64FormattingOptions options) {
			return null;
		}
		
		public static float ToSingle(double value) {
			return 0;
		}

		[InlineCode("{$System.Script}.dec64({s})")]
		public static byte[] FromBase64String(string s) {
			return null;
		}
		
		public static float ToSingle(short value) {
			return 0;
		}

		public static float ToSingle(int value) {
			return 0;
		}

		public static float ToSingle(long value) {
			return 0;
		}

		public static float ToSingle(object value) {
			return 0;
		}

		public static float ToSingle(sbyte value) {
			return 0;
		}

		public static float ToSingle(float value) {
			return 0;
		}

		public static float ToSingle(string value) {
			return 0;
		}

		public static float ToSingle(ushort value) {
			return 0;
		}

		public static float ToSingle(uint value) {
			return 0;
		}

		public static float ToSingle(ulong value) {
			return 0;
		}

		public static float ToSingle(object value, IFormatProvider provider) {
			return 0;
		}

		public static float ToSingle(string value, IFormatProvider provider) {
			return 0;
		}

		public static string ToString(string value) {
			return "";
		}

		public static int ToInt32(string value) {
			return 0;
		}

		public static double ToDouble(string value, IFormatProvider provider) {
			return 0;
		}

		public static bool ToBoolean(string value) {
			return false;
		}

		public static char ToChar(string value) {
			return default(char);
		}
	}
}
