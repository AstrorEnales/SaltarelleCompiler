///////////////////////////////////////////////////////////////////////////////
// IntPtr

var ss_IntPtr = function#? DEBUG IntPtr$##(value) {
	this.value = value || 0;
};

ss_IntPtr.__typeName = 'ss.IntPtr';
ss.IntPtr = ss_IntPtr;
ss_IntPtr.__class = false;
ss.initClass(ss_IntPtr, ss, {
  toInt32: function() { return this.value; },
  toInt64: function() { return this.value; },
  toString: function() { return value.toString(); },
  getHashCode: function() { return value; },
  equals: function(obj) { return obj instanceof ss_IntPtr && obj.value === this.value; }
}, null, []);

ss_IntPtr.getDefaultValue = ss_IntPtr.createInstance = function#? DEBUG IntPtr$getDefaultValue##() {
	return new ss_IntPtr(0);
};

ss_IntPtr.zero = new ss_IntPtr(0);
ss_IntPtr.size = 4;
