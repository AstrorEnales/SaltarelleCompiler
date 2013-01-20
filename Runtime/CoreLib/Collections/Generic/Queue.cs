// Queue.cs
// Script#/Libraries/CoreLib
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System.Runtime.CompilerServices;

namespace System.Collections.Generic {
    /// <summary>
    /// The Queue data type which is mapped to the Array type in Javascript.
    /// </summary>
    [IgnoreNamespace]
    [Imported(ObeysTypeSystem = true)]
    [ScriptName("Array")]
	[IgnoreGenericArguments]
    public sealed class Queue<T> {

        [IntrinsicProperty]
        [ScriptName("length")]
        public int Count {
            get {
                return 0;
            }
        }

        public void Clear() {
        }

        public bool Contains(T item) {
            return false;
        }

        [ScriptName("shift")]
		public T Dequeue() {
            return default(T);
        }

        [ScriptName("push")]
		public void Enqueue(T item) {
        }

        [ScriptName("peekFront")]
		public T Peek() {
            return default(T);
        }
    }
}
