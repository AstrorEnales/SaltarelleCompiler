// MapZoomRange.cs
// Script#/Libraries/Microsoft/BingMaps
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Maps {

    [Imported]
    [ScriptName("Object")]
    [IgnoreNamespace]
    public sealed class MapZoomRange {

        [IntrinsicProperty]
        public int Min {
            get;
            set;
        }

        [IntrinsicProperty]
        public int Max {
            get;
            set;
        }
    }
}
