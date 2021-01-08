#!/bin/bash

# All errors fatal
set -x

. /etc/profile

set -e

export OPENMAMA_INSTALL_DIR=/opt/openmama
unset PREFIX
unset VERSION_FILE

# Build the project
test -d build && rm -rf build || true
mkdir -p build
cd build
cmake -DCMAKE_BUILD_TYPE=RelWithDebInfo \
     -DBIN_GRADLE=$SDKMAN_DIR/candidates/gradle/current/bin/gradle \
     -DCMAKE_BUILD_TYPE=RelWithDebInfo \
     -DCMAKE_INSTALL_PREFIX=$OPENMAMA_INSTALL_DIR \
     -DWITH_JAVA=ON \
     -DWITH_CSHARP=OFF \
     -DWITH_UNITTEST=ON \
     -DCMAKE_CXX_FLAGS="-Werror -Wno-error=strict-prototypes -Wno-deprecated-declarations" \
     -DCMAKE_C_FLAGS="-Werror -Wno-error=strict-prototypes -Wno-deprecated-declarations" \
     ..
make -j install
cd - > /dev/null

# Perform unit tests
python3 release_scripts/ci-run.py

# Clean up Unit Test files - we don't need them any more
rm -f $OPENMAMA_INSTALL_DIR/*.xml $OPENMAMA_INSTALL_DIR/bin/UnitTest*

# Include the test data and grab profile configuration from there for packaging
test -d $OPENMAMA_INSTALL_DIR/data && rm -rf $OPENMAMA_INSTALL_DIR/data || true
mkdir -p $OPENMAMA_INSTALL_DIR/data
cd $OPENMAMA_INSTALL_DIR/data
curl -sL https://github.com/OpenMAMA/OpenMAMA-testdata/archive/master.tar.gz | tar xz --strip 1
cp profiles/profile.openmama .
rm -rf profiles
cd - > /dev/null

# Generate the package (deb / rpm / tarball).
./release_scripts/build-package.sh