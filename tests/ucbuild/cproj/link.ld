MEMORY
{
	BEG (rx) : ORIGIN = 0x8000, LENGTH = 0x1000
	RO  (r)  : ORIGIN = 0x9000, LENGTH = 0x1000
	END (rx) : ORIGIN = 0x27FFF, LENGTH = 1
}

SECTIONS
{	
	.text : {
		*(.text*)
	} > BEG
	
	.read_only : {
		*(.ARM.exidx)
	} > RO
	
	.magics : {
		KEEP(*(.magic))
	} > END
}