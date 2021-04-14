import subprocess
import webbrowser

p = subprocess.run(['git', 'config', '--get', 'remote.origin.url'], capture_output=True)
output = p.stdout.strip()
webbrowser.open(output)
