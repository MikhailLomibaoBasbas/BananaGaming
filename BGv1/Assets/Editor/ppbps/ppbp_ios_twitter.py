#!/usr/bin/env python
import sys, os.path, glob, shutil
from mod_pbxproj import XcodeProject

def addFrameworks(install_path):
	#modify XCode pbxproj and add framework dependencies
	pbxproj_path = os.path.join(install_path, 'Unity-iPhone.xcodeproj/project.pbxproj')
	project = XcodeProject.Load(pbxproj_path)
	project.add_file_if_doesnt_exist('System/Library/Frameworks/Twitter.framework', tree='SDKROOT', weak=True)
	if project.modified:
	  project.backup()
	  project.saveFormat3_2() #IMPORTANT, DONT USE THE OLD VERSION!


def main(install_path):
	print '==================================='
	print 'Starting to load Twitter plugin'
	addFrameworks(install_path)
	print 'Successfully loaded Twitter plugin'
	print '==================================='

install_path = sys.argv[1]
main(install_path)