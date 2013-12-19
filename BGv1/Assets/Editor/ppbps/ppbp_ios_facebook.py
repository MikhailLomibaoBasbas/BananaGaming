#!/usr/bin/env python
import sys, os.path, glob, shutil
from mod_pbxproj import XcodeProject

def configureInfoPlist(install_path, elements_to_add):
	# install Facebook plist settings
	info_plist_path = os.path.join(install_path, 'Info.plist')
	file = open(info_plist_path, 'r')
	plist = file.read()
	file.close()
	plist = plist.replace('<key>', elements_to_add + '<key>', 1)
	file = open(info_plist_path, 'w')
	file.write(plist)
	file.close()

def addFrameworks(install_path):
	#modify XCode pbxproj and add framework dependencies
	pbxproj_path = os.path.join(install_path, 'Unity-iPhone.xcodeproj/project.pbxproj')
	project = XcodeProject.Load(pbxproj_path)
	project.add_file_if_doesnt_exist('System/Library/Frameworks/Accounts.framework', tree='SDKROOT', weak=True)
	project.add_file_if_doesnt_exist('System/Library/Frameworks/AdSupport.framework', tree='SDKROOT', weak=True)
	project.add_file_if_doesnt_exist('System/Library/Frameworks/Security.framework', tree='SDKROOT', weak=True)
	project.add_file_if_doesnt_exist('System/Library/Frameworks/Social.framework', tree='SDKROOT', weak=True)
	project.add_file_if_doesnt_exist('usr/lib/libsqlite3.0.dylib', tree='SDKROOT', weak=True)
	if project.modified:
	  project.backup()
	  project.saveFormat3_2() #IMPORTANT, DONT USE THE OLD VERSION!


def main(install_path, plist_elements_to_add):
	print '==================================='
	print 'Starting to load Facebook plugin'
	configureInfoPlist(install_path, plist_elements_to_add)
	addFrameworks(install_path)
	print 'Successfully loaded Facebook plugin'
	print '==================================='

install_path = sys.argv[1]
fb_app_id = '136228806535961'
plist_elements_to_add = '''
	<key>FacebookAppId</key>
	<string>%s</string>
	<key>CFBundleURLTypes</key>
	<array>
	 <dict>
	  <key>CFBundleURLSchemes</key>
	  <array>
	   <string>fb%s</string>
	  </array>
	 </dict>
	</array>
	<key>CFBundleLocalizations</key>
	<array>
	 <string>en</string>
	 <string>ja</string>
	</array>
	''' % (fb_app_id, fb_app_id)
main(install_path, plist_elements_to_add)