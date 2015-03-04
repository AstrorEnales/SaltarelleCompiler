using System;
using System.Runtime.CompilerServices;
using System.Globalization;
using QUnit;

namespace CoreLib.TestScript {
	[TestFixture]
	public class ConvertTests {
		private byte[] GetTestArr() {
			var result = new byte[64*3];
			for (int i = 0; i < 64; i++) {
				result[i * 3] = (byte)(i << 2);
				result[i * 3 + 1] = 0;
				result[i * 3 + 2] = 0;
			}
			return result;
		}
		
		private void AssertAlmostEqual(double d1, double d2) {
			var diff = d2 - d1;
			if (diff < 0)
				diff = -diff;
			Assert.IsTrue(diff < 1e-6);
		}

		[Test]
		public void ToBase64StringWithOnlyArrayWorks() {
			var testArr = GetTestArr();
			Assert.AreEqual(Convert.ToBase64String(testArr), "AAAABAAACAAADAAAEAAAFAAAGAAAHAAAIAAAJAAAKAAALAAAMAAANAAAOAAAPAAAQAAARAAASAAATAAAUAAAVAAAWAAAXAAAYAAAZAAAaAAAbAAAcAAAdAAAeAAAfAAAgAAAhAAAiAAAjAAAkAAAlAAAmAAAnAAAoAAApAAAqAAArAAAsAAAtAAAuAAAvAAAwAAAxAAAyAAAzAAA0AAA1AAA2AAA3AAA4AAA5AAA6AAA7AAA8AAA9AAA+AAA/AAA");
			Assert.AreEqual(Convert.ToBase64String(new byte[] { 1, 2, 3 }), "AQID");
			Assert.AreEqual(Convert.ToBase64String(new byte[] { 1, 2, 3, 4 }), "AQIDBA==");
			Assert.AreEqual(Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5 }), "AQIDBAU=");
			Assert.AreEqual(Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5, 6 }), "AQIDBAUG");
			Assert.AreEqual(Convert.ToBase64String(new byte[0]), "");
		}
		
		[Test]
		public void ToSingleWorks() {
			Assert.AreEqual(Convert.ToSingle(true), 1f);
			Assert.AreEqual(Convert.ToSingle(false), 0f);
			AssertAlmostEqual(Convert.ToSingle(1.45f), 1.45);
			Assert.AreEqual(Convert.ToSingle((byte)244), 244f);
			Assert.AreEqual(Convert.ToSingle((sbyte)-14), -14f);
			Assert.AreEqual(Convert.ToSingle(14.5m), 14.5f);
			Assert.IsTrue(Math.Abs(Convert.ToSingle(0.45) - 0.45f) < 0.1f);
			Assert.AreEqual(Convert.ToSingle((int)-14), -14f);
			Assert.AreEqual(Convert.ToSingle((uint)165), 165f);
			Assert.AreEqual(Convert.ToSingle((short)-13), -13f);
			Assert.AreEqual(Convert.ToSingle((ushort)13), 13f);
			Assert.AreEqual(Convert.ToSingle((long)-2345), -2345f);
			Assert.AreEqual(Convert.ToSingle((ulong)2345), 2345f);
			Assert.AreEqual(Convert.ToSingle((object)-1456), -1456f);
			Assert.IsTrue(
				Math.Abs(Convert.ToSingle("-45.245", NumberFormatInfo.InvariantInfo) + 45.245f) < 0.1f);
			Assert.IsTrue(Math.Abs(
				Convert.ToSingle((object)"-45.245", NumberFormatInfo.InvariantInfo) + 45.245f) < 0.1f);
		}


		[Test]
		public void ToBase64StringWithArrayAndFormattingOptionsWorks() {
			var testArr = GetTestArr();
			Assert.AreEqual(Convert.ToBase64String(testArr, Base64FormattingOptions.None), "AAAABAAACAAADAAAEAAAFAAAGAAAHAAAIAAAJAAAKAAALAAAMAAANAAAOAAAPAAAQAAARAAASAAATAAAUAAAVAAAWAAAXAAAYAAAZAAAaAAAbAAAcAAAdAAAeAAAfAAAgAAAhAAAiAAAjAAAkAAAlAAAmAAAnAAAoAAApAAAqAAArAAAsAAAtAAAuAAAvAAAwAAAxAAAyAAAzAAA0AAA1AAA2AAA3AAA4AAA5AAA6AAA7AAA8AAA9AAA+AAA/AAA");
			Assert.AreEqual(Convert.ToBase64String(testArr, Base64FormattingOptions.InsertLineBreaks), "AAAABAAACAAADAAAEAAAFAAAGAAAHAAAIAAAJAAAKAAALAAAMAAANAAAOAAAPAAAQAAARAAASAAA\n" + 
			                                                                                           "TAAAUAAAVAAAWAAAXAAAYAAAZAAAaAAAbAAAcAAAdAAAeAAAfAAAgAAAhAAAiAAAjAAAkAAAlAAA\n" +
			                                                                                           "mAAAnAAAoAAApAAAqAAArAAAsAAAtAAAuAAAvAAAwAAAxAAAyAAAzAAA0AAA1AAA2AAA3AAA4AAA\n" +
			                                                                                           "5AAA6AAA7AAA8AAA9AAA+AAA/AAA");
		}
		
		[Test]
		public void ToInt32Works() {
			Assert.AreEqual(Convert.ToInt32("3590"), 3590);
			Assert.AreEqual(Convert.ToInt32("-3590"), -3590);
		}

		[Test]
		public void ToBase64StringWithArrayAndOffsetAndLengthWorks() {
			var arr = GetTestArr();
			Assert.AreEqual(Convert.ToBase64String(arr, 100, 90), "AACIAACMAACQAACUAACYAACcAACgAACkAACoAACsAACwAAC0AAC4AAC8AADAAADEAADIAADMAADQAADUAADYAADcAADgAADkAADoAADsAADwAAD0AAD4AAD8");
		}
		
		[Test]
		public void ToBooleanWorks() {
			Assert.AreEqual(Convert.ToBoolean("true"), true);
			Assert.AreEqual(Convert.ToBoolean("True"), true);
		}

		[Test]
		public void ToBase64StringWithArrayAndOffsetAndLengthAndFormattingOptionsWorks() {
			var arr = GetTestArr();
			Assert.AreEqual(Convert.ToBase64String(arr, 100, 90, Base64FormattingOptions.None), "AACIAACMAACQAACUAACYAACcAACgAACkAACoAACsAACwAAC0AAC4AAC8AADAAADEAADIAADMAADQAADUAADYAADcAADgAADkAADoAADsAADwAAD0AAD4AAD8");
			Assert.AreEqual(Convert.ToBase64String(arr, 100, 90, Base64FormattingOptions.InsertLineBreaks), "AACIAACMAACQAACUAACYAACcAACgAACkAACoAACsAACwAAC0AAC4AAC8AADAAADEAADIAADMAADQ\n" +
			                                                                                                "AADUAADYAADcAADgAADkAADoAADsAADwAAD0AAD4AAD8");
			Assert.AreEqual(Convert.ToBase64String(arr, 70, 114, Base64FormattingOptions.InsertLineBreaks), "AABgAABkAABoAABsAABwAAB0AAB4AAB8AACAAACEAACIAACMAACQAACUAACYAACcAACgAACkAACo\n" +
			                                                                                                "AACsAACwAAC0AAC4AAC8AADAAADEAADIAADMAADQAADUAADYAADcAADgAADkAADoAADsAADwAAD0");
		}
		
		[Test]
		public void ToDoubleWorks() {
			Assert.AreEqual(Convert.ToDouble("34.6405904", CultureInfo.InvariantCulture), 34.6405904);
			Assert.AreEqual(Convert.ToDouble("-34.6405904", CultureInfo.InvariantCulture), -34.6405904);
		}

		[Test]
		public void FromBase64StringWorks() {
			Assert.AreEqual(Convert.FromBase64String("AAAABAAACAAADAAAEAAAFAAAGAAAHAAAIAAAJAAAKAAALAAAMAAANAAAOAAAPAAAQAAARAAASAAATAAAUAAAVAAAWAAAXAAAYAAAZAAAaAAAbAAAcAAAdAAAeAAAfAAAgAAAhAAAiAAAjAAAkAAAlAAAmAAAnAAAoAAApAAAqAAArAAAsAAAtAAAuAAAvAAAwAAAxAAAyAAAzAAA0AAA1AAA2AAA3AAA4AAA5AAA6AAA7AAA8AAA9AAA+AAA/AAA"), GetTestArr());
			Assert.AreEqual(Convert.FromBase64String("AQID"), new byte[] { 1, 2, 3 });
			Assert.AreEqual(Convert.FromBase64String("AQIDBA=="), new byte[] { 1, 2, 3, 4 });
			Assert.AreEqual(Convert.FromBase64String("AQIDBAU="), new byte[] { 1, 2, 3, 4, 5 });
			Assert.AreEqual(Convert.FromBase64String("AQIDBAUG"), new byte[] { 1, 2, 3, 4, 5, 6 });
			Assert.AreEqual(Convert.FromBase64String("AQIDBAU="), new byte[] { 1, 2, 3, 4, 5 });
			Assert.AreEqual(Convert.FromBase64String("A Q\nI\tD"), new byte[] { 1, 2, 3 });
			Assert.AreEqual(Convert.FromBase64String(""), new byte[0]);
		}
		
		[Test]
		public void ToCharWorks() {
			Assert.AreEqual((int)Convert.ToChar("a"), 'a'.ToString());
		}
	}
}
