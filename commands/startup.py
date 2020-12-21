import os
import subprocess

appData = os.getenv('APPDATA')
startup = os.path.join(appData, 'Microsoft\Windows\Start Menu\Programs\Startup')

subprocess.Popen(
    ['explorer.exe', startup], cwd=startup)
