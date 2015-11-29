///////////////////////////////////////////////////////////////////////////////
// Thread

var ss_Thread = function#? DEBUG Thread$##(start) {
	this.start = start;
	this.managedThreadId = 0;
    this.name = '';
};

ss_Thread.__typeName = 'ss.Thread';
ss.Thread = ss_Thread;
ss.initClass(ss_Thread, ss, {}, null, []);

ss_Thread.currentThread = new ss_Thread();
