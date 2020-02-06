#pragma once
#include <easyhook.h>
extern "C" _declspec(dllimport) int Install();
extern "C" _declspec(dllimport) int UnInstall();
extern "C" _declspec(dllimport) void __cdecl SetCallBack(int (*pfCallBack)(int a, LPCWSTR s));
extern "C" _declspec(dllimport) void __stdcall NativeInjectionEntryPoint(REMOTE_ENTRY_INFO* pInfo);
