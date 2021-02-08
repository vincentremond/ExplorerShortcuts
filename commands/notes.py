import os
import subprocess

import os.path
from os import path

def write_all_text(path, contents):
    file1 = open(path, 'w')
    file1.write(contents)
    file1.close()

currentDir = os.getcwd()
filename = 'notes.md'
if not path.exists(filename):
    write_all_text(filename, "# Notes\n\n\n")

subprocess.Popen(
    ['C:\Program Files\Microsoft VS Code\Code.exe', currentDir, "--goto", f'{filename}:3'], cwd=currentDir)
