import os
import subprocess

currentDir = os.getcwd()
appData = os.getenv('USERPROFILE')
fork = os.path.join(appData, 'AppData\Local\Fork\Fork.exe')
subprocess.Popen([fork, currentDir], cwd=currentDir)
