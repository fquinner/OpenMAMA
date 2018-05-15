FIND_PATH(PROTON_INCLUDE_DIR proton/version.h
	PATHS
		${QPID_ROOT}/include
		${PROTON_ROOT}/include
		/usr/local/include
		/usr/include
)

FIND_LIBRARY(PROTON_LIBRARY qpid-proton
	PATHS
		${QPID_ROOT}/lib64
		${PROTON_ROOT}/lib64
		/usr/lib64
		/usr/local/lib64
		${QPID_ROOT}/lib
		${PROTON_ROOT}/lib
		/usr/lib
		/usr/local/lib
	NO_DEFAULT_PATH
)

IF (PROTON_LIBRARY AND PROTON_INCLUDE_DIR)
    SET(PROTON_LIBRARIES ${PROTON_LIBRARY})
    SET(PROTON_FOUND "YES")
ELSE ()
  SET(APR_FOUND "NO")
ENDIF ()


IF (PROTON_FOUND)
   IF (NOT PROTON_FIND_QUIETLY)
      MESSAGE(STATUS "Found Proton: ${PROTON_LIBRARIES}")
   ENDIF ()
ELSE ()
   IF (PROTON_FIND_REQUIRED)
      MESSAGE(FATAL_ERROR "Could not find Proton library")
   ENDIF ()
ENDIF ()
