import os
import subprocess
import readline
currentDir = os.getcwd()
subprocess.Popen(
    ['C:\Program Files (x86)\Atlassian\Sourcetree\SourceTree.exe', '-f', currentDir], cwd=currentDir)
