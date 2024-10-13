from pathlib import Path

# Get the current directory
dir = Path(__file__).parent

# Get all of the files within the directory
all   = dir.glob('*')
files = []

# Get all non python files
for file in all:
	if file.suffix != '.py':
		files.append(file)
		
# Delete all of the files
for file in files:
	file.unlink()