// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
#include "dllmain.h"
#include <easyhook.h>

#ifdef _WIN64
#pragma comment(lib, "EasyHook64.lib")
#else
#pragma comment(lib, "EasyHook32.lib")
#endif

typedef int (*pfCallBack)(int a, LPCWSTR s);
pfCallBack CallBackfunc = NULL;

extern "C" _declspec(dllexport) void __cdecl SetCallBack(int (*pfCallBack)(int a, LPCWSTR s))
{
	CallBackfunc = pfCallBack;
}

extern "C" _declspec(dllexport) int Inject(int pid)
{
#ifdef _WIN64
	WCHAR* dll64 = L"HookToolDll64.dll";
#else
	WCHAR* dll32 = L"HookToolDll32.dll";
#endif

	NTSTATUS nt = RhInjectLibrary(
		pid,   // The process to inject into
		0,           // ThreadId to wake up upon injection
		EASYHOOK_INJECT_STEALTH,
#ifdef _WIN64
		NULL, // 32-bit
		dll64, // 64-bit
#else
		dll32, // 32-bit
		NULL, // 64-bit
#endif
		CallBackfunc, // data to send to injected DLL entry point
		sizeof(pfCallBack)// size of data to send
	);

	if (nt != 0)
	{
		CallBackfunc(1, L"RhInjectLibrary failed");
		PWCHAR err = RtlGetLastErrorString();
		CallBackfunc(2, err);
	}
	else
	{
		CallBackfunc(1, L"Library injected successfully");
	}

	return nt;
}


BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

