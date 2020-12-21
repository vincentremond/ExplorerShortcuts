import os
import subprocess
currentDir = os.getcwd()
subprocess.Popen(
    ['C:\Program Files\Microsoft VS Code\Code.exe', currentDir], cwd=currentDir)
