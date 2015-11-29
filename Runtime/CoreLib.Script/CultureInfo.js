///////////////////////////////////////////////////////////////////////////////
// CultureInfo

var ss_CultureInfo = function#? DEBUG CultureInfo$##(name) {
	this.name = name;
	this.numberFormat = ss_NumberFormatInfo.invariantInfo;
	this.dateTimeFormat = ss_DateTimeFormatInfo.invariantInfo;
};

ss_CultureInfo.__typeName = 'ss.CultureInfo';
ss.CultureInfo = ss_CultureInfo;
ss.initClass(ss_CultureInfo, ss, {
	getFormat:  function#? DEBUG CultureInfo$getFormat##(type) {
		switch (type) {
			case ss_NumberFormatInfo: return this.numberFormat;
			case ss_DateTimeFormatInfo: return this.dateTimeFormat;
			default: return null;
		}
	}
}, null, [ss_IFormatProvider]);

ss_CultureInfo.invariantCulture = new ss_CultureInfo('en-US');
ss_CultureInfo.currentCulture = ss_CultureInfo.invariantCulture;
