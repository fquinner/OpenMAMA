set PATH=%JAVA_HOME_OVERRIDE%\bin;%PATH%
mkdir build || exit /b %ERRORLEVEL%
cd build  || exit /b %ERRORLEVEL%
cmake -DCMAKE_BUILD_TYPE=RelWithDebInfo -DWITH_CSHARP=ON -DWITH_UNITTEST=ON -DWITH_JAVA=ON -DPROTON_ROOT=C:/tools/vcpkg/installed/%PLATFORM%-windows/ -DGTEST_ROOT=C:/tools/vcpkg/installed/%PLATFORM%-windows/ -DINSTALL_RUNTIME_DEPENDENCIES=ON -DCMAKE_INSTALL_PREFIX="%OPENMAMA_INSTALL_DIR%" -G "%GENERATOR%" -DCMAKE_TOOLCHAIN_FILE=c:/tools/vcpkg/scripts/buildsystems/vcpkg.cmake -DAPR_ROOT=C:/tools/vcpkg/installed/%PLATFORM%-windows/ ..  || exit /b %ERRORLEVEL%
cmake --build . --config RelWithDebInfo --target install  || exit /b %ERRORLEVEL%
cd ..  || exit /b %ERRORLEVEL%
python release_scripts\ci-run.py  || exit /b %ERRORLEVEL%
7z a openmama-%VERSION%.win.%PLATFORM%.zip "%OPENMAMA_INSTALL_DIR%"  || exit /b %ERRORLEVEL%