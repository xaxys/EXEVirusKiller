#pragma once
extern "C" _declspec(dllimport) void __cdecl SetCallBack(int (*pfCallBack)(int a, LPCWSTR s));
extern "C" _declspec(dllimport) int Inject(int pid);