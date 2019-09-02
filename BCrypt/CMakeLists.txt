cmake_minimum_required(VERSION 3.10.2)

project(BCrypt C CXX)

add_library(BCrypt SHARED bcrypt.c crypt_blowfish.c crypt_gensalt.c wrapper.c dllmain.cpp)
