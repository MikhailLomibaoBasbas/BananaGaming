#!/usr/bin/env python
import sys, os.path, glob, shutil
from mod_pbxproj import XcodeProject

def copySourceFilesToInstallPath(source_path, install_path):
	pbxproj_path = os.path.join(install_path, 'Unity-iPhone.xcodeproj/project.pbxproj')
	project = XcodeProject.Load(pbxproj_path)
	library_group = project.get_groups_by_name('Libraries')[0]

	nodes = glob.glob(os.path.join(source_path, '*'))
	for node in nodes:
		if os.path.isdir(node):
			basepath, directory = os.path.split(node)
			group = project.get_or_create_group(directory, parent=library_group)
			source = os.path.join(source_path, directory)
			library = os.path.join('Libraries', directory)
			install = os.path.join(install_path, library)
			if not os.path.exists(install):
				os.makedirs(install)
			recursivelyCopyFiles(source, install, library, project, group)

	if project.modified:
	  project.backup()
	  project.saveFormat3_2() #IMPORTANT, DONT USE THE OLD VERSION!

def recursivelyCopyFiles(source_path, install_path, library_path, project, group):
	filenames = glob.glob(os.path.join(source_path, '*'))
	for f in filenames:
		print "copying file: " + f
		if os.path.isfile(f):
			if f[-4:] != 'meta':
				directory, filename = os.path.split(f)
				shutil.copy(f, os.path.join(install_path, filename))
				project.add_file_if_doesnt_exist(os.path.join(library_path, filename), parent=group)
		else:
			directory, foldername = os.path.split(f)
			newinstall_path = os.path.join(install_path, foldername)
			newlibrary_path = os.path.join(library_path, foldername)
			if not os.path.exists(newinstall_path):
				os.makedirs(newinstall_path)
			newgroup = project.get_or_create_group(foldername, parent=group)
			recursivelyCopyFiles(f, newinstall_path, newlibrary_path, project, newgroup)


def main(install_path):
	print '==================================='
	print 'Starting iOS post process build'
	source_path = os.path.abspath(os.path.join(os.path.dirname( __file__ ), '..', '..', 'Plugins', 'iOS'))
	copySourceFilesToInstallPath(source_path, install_path)
	print 'Successfully copied all sources in Plugins/iOS directory'
	print '==================================='

install_path = sys.argv[1]
main(install_path)