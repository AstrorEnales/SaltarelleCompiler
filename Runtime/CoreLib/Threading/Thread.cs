using System;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System.Threading
{
	[Imported(ObeysTypeSystem = true)]
	[ScriptNamespace("ss")]
	public class Thread {
		[IntrinsicProperty]
		public int ManagedThreadId { get { return 0; } }

		[IntrinsicProperty]
		public string Name { get; set; }

		[IntrinsicProperty]
		public static Thread CurrentThread { get { return null; } }

		public Thread() {
			
		}

		public Thread(ThreadStart start) {
		}

		public Thread(ParameterizedThreadStart start) {
		}
	}

	public delegate void ThreadStart();

	public delegate void ParameterizedThreadStart(object obj);
}