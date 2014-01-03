#!/bin/bash -e
# This is meant to be used on OS X only.
export SSHTARGET="ranier@10.3.10.33"
export LOCALDIR="../"
export REMOTEDIR="~/Desktop/UnityBuilds/unity_game/"
export BUILDDIR="Builds/iOS/"

cd "$(dirname "$0")"
shell/common_build.sh