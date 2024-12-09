import sys

# Get the generation options
hdr_cnt = int(sys.argv[1])
src_cnt = int(sys.argv[2])

# Generate the header files
for idx in range(hdr_cnt):
	open(f'generated_header_{idx}.h', 'w').close()
	
# Generate the source files
for idx in range(src_cnt):
	
	# Create the file
	file = open(f'generated_source_{idx}.c', 'w')
	
	# Add includes
	for hidx in range(hdr_cnt):
		file.write(f'#include "generated_header_{hidx}.h"\n')
	
	# Write the file
	file.close()