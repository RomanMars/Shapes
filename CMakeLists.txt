cmake_minimum_required(VERSION 3.0.0)
project(ShapesOgl VERSION 0.1.0)

include(CTest)
enable_testing()

add_executable(ShapesOgl ${CMAKE_CURRENT_SOURCE_DIR}/main.cpp)


add_subdirectory(lib/glad)
target_link_libraries(ShapesOgl GLAD)


target_include_directories(ShapesOgl PUBLIC "${CMAKE_CURRENT_SOURCE_DIR}/Include/")

target_link_libraries(ShapesOgl "${CMAKE_CURRENT_SOURCE_DIR}/lib/libglfw3.a")

target_include_directories(ShapesOgl PUBLIC "${CMAKE_CURRENT_SOURCE_DIR}/logl/")

set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PROJECT_VERSION ${PROJECT_VERSION})
include(CPack)
