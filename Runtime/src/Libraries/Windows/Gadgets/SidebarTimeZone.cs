// SidebarTimeZone.cs
// Script#/Libraries/Windows
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System;
using System.Runtime.CompilerServices;

namespace System.Gadgets {

    /// <summary>
    /// Contains information about a particular time zone.
    /// </summary>
    [IgnoreNamespace]
    [Imported]
    public sealed class SidebarTimeZone {

        private SidebarTimeZone() {
        }

        [IntrinsicProperty]
        public long Bias {
            get {
                return 0;
            }
        }

        [IntrinsicProperty]
        public string DisplayName {
            get {
                return null;
            }
        }

        [IntrinsicProperty]
        [PreserveCase]
        public long DSTBias {
            get {
                return 0;
            }
        }

        [IntrinsicProperty]
        [PreserveCase]
        public DateTime DSTDate {
            get {
                return null;
            }
        }

        [IntrinsicProperty]
        [PreserveCase]
        public string DSTDisplayName {
            get {
                return null;
            }
        }

        [IntrinsicProperty]
        public string Name {
            get {
                return null;
            }
        }

        [IntrinsicProperty]
        public DateTime StandardDate {
            get {
                return null;
            }
        }

        [IntrinsicProperty]
        public string StandardDisplayName {
            get {
                return null;
            }
        }
    }
}
