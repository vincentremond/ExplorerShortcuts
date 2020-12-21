# -*- coding: utf-8 -*-
from distutils.core import setup
import py2exe
import os


# Source : https://stackoverflow.com/questions/19566765/python-py2exe-compile-two-py-files-at-once-to-two-separate-exe-files/19590689#19590689

class Target:
    def __init__(self, **kw):
        self.__dict__.update(kw)


commands = []
commandDir = r'.\\commands\\'
for root, dirs, files in os.walk(commandDir):
    if files:
        for filename in files:
            relativePath = commandDir + filename
            print(relativePath)
            commands.append(Target(script=relativePath))

setup(windows=commands)
