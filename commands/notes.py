import os
import sys
import subprocess

import os.path
from os import path

def write_all_text(path, contents):
    file1 = open(path, 'w')
    file1.write(contents)
    file1.close()

def getName(args):
    if(len(args) > 1):
        return args[1]
    else:
        return "Notes"

name = getName(sys.argv)

currentDir = os.getcwd()
filename = 'notes.md'
if not path.exists(filename):
    write_all_text(filename, f'# {name}\n\n\n')

subprocess.Popen(
    ['C:\Program Files\Microsoft VS Code\Code.exe', currentDir, "--goto", f'{filename}:3'], cwd=currentDir)
