#include "BCrypt.hpp"

#if WIN32
#define P_EXPORT __declspec(dllexport)
#else
#define P_EXPORT
#endif

extern "C" P_EXPORT const char *generate_hash(const char *pw, int workload)
{
    std::string ret = BCrypt::generateHash(std::string(pw), workload);

    char* x = (char*) malloc(ret.size() + 1);
    strcpy_s(x, ret.size() + 1, ret.c_str());

    return (const char*) x;
}

extern "C" P_EXPORT bool validate_password(const char *str, const char *ha)
{
    return BCrypt::validatePassword(std::string(str), std::string(ha));
}
