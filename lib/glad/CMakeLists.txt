add_library( 
    GLAD #STATIC SHARED MODULE
    include/glad/glad.h
    include/KHR/khrplatform.h
    src/glad.c
)

target_include_directories(GLAD PUBLIC "${CMAKE_CURRENT_SOURCE_DIR}/src"
GLAD PUBLIC "${CMAKE_CURRENT_SOURCE_DIR}/include"
)

#target_compile_definitions(TestLib PUBLIC TestLibVersion=1)
#target_compile_definitions(TestLib PUBLIC TestLibFolder="${CMAKE_CURRENT_SOURCE_DIR}/src")
