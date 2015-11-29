// StreamingContext.cs
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System.Runtime.CompilerServices;

namespace System.Runtime.Serialization
{
	[Imported(ObeysTypeSystem = true)]
	[ScriptNamespace("ss")]
	public struct StreamingContext
	{
		public StreamingContext(StreamingContextStates state)
			: this(state, null) {}

		public StreamingContext(StreamingContextStates state, object additional)
		{
			this.state = state;
			additionalContext = additional;
		}

		private readonly StreamingContextStates state;
		private readonly object additionalContext;

		public object Context
		{
			get { return additionalContext; }
		}

		public StreamingContextStates State
		{
			get { return state; }
		}

		public override int GetHashCode()
		{
			return (int)state;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is StreamingContext))
				return false;
			var other = (StreamingContext)obj;
			return other.additionalContext == additionalContext && other.state == state;
		}
	}
}