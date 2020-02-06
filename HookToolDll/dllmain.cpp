// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
#include "dllmain.h"
#include <easyhook.h>
#include <tchar.h>
#include <stdlib.h>
#include <string>
//#include <ntifs.h>

#ifdef _WIN64
#pragma comment(lib, "EasyHook64.lib")
#else
#pragma comment(lib, "EasyHook32.lib")
#endif

// NtCreateFile 的函数指针
typedef NTSTATUS(NTAPI* pfnNTCREATEFILE) (
	OUT PHANDLE             FileHandle,
	IN ACCESS_MASK          DesiredAccess,
	IN POBJECT_ATTRIBUTES   ObjectAttributes,
	OUT PVOID               IoStatusBlock,
	IN PLARGE_INTEGER       AllocationSize OPTIONAL,
	IN ULONG                FileAttributes,
	IN ULONG                ShareAccess,
	IN ULONG                CreateDisposition,
	IN ULONG                CreateOptions,
	IN PVOID                EaBuffer OPTIONAL,
	IN ULONG                EaLength
	);

// 全局变量
pfnNTCREATEFILE         pfnNtCreateFile = (pfnNTCREATEFILE)GetProcAddress(GetModuleHandle(_T("ntdll.dll")), "NtCreateFile");
TRACED_HOOK_HANDLE      hHookNtCreateFile = new HOOK_TRACE_INFO();
ULONG                   HookNtCreateFile_ACLEntries[1] = { 0 };

bool CheckFolderExist(PCWSTR strPath)
{
	WIN32_FIND_DATA  wfd;
	bool rValue = false;
	HANDLE hFind = FindFirstFile(strPath, &wfd);
	if ((hFind != INVALID_HANDLE_VALUE) && (wfd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY))
	{
		rValue = true;
	}
	FindClose(hFind);
	return rValue;
}

typedef int (*pfCallBack)(int a, LPCWSTR s);
pfCallBack CallBackfunc = NULL;

extern "C" _declspec(dllexport) void __cdecl SetCallBack(int (*pfCallBack)(int a, LPCWSTR s))
{
	CallBackfunc = pfCallBack;
}

NTSTATUS NTAPI NtCreateFileHook(OUT PHANDLE FileHandle,
	IN ACCESS_MASK DesiredAccess,
	IN POBJECT_ATTRIBUTES ObjectAttributes,
	OUT PIO_STATUS_BLOCK IoStatusBlock,
	IN PLARGE_INTEGER AllocationSize OPTIONAL,
	IN ULONG FileAttributes,
	IN ULONG ShareAccess,
	IN ULONG CreateDisposition,
	IN ULONG CreateOptions,
	IN PVOID EaBuffer OPTIONAL,
	IN ULONG EaLength)
{
	ULONG MyCreateDisposition = CreateDisposition;
	PWSTR file = ObjectAttributes->ObjectName->Buffer;
	CallBackfunc(1, file);
	std::wstring strFile(file);

	if (strFile.substr(strFile.length()-4, 4) == L".exe")
	{
		std::wstring strPath = strFile.substr(4, strFile.length() - 8);
		//CallBackfunc(3, strPath.c_str());
		if (CheckFolderExist(strPath.c_str()))
		{
			CallBackfunc(0, strPath.c_str());
			switch (CreateDisposition)
			{
			case FILE_OVERWRITE_IF:
				MyCreateDisposition = FILE_OVERWRITE;
				break;
			case FILE_OPEN_IF:
				MyCreateDisposition = FILE_OPEN;
				break;
			default:
				return 0;
			}
		}
	}
	
	// 调用系统原有的 NtCreateFile
	return pfnNtCreateFile(FileHandle, DesiredAccess, ObjectAttributes,
		IoStatusBlock, AllocationSize, FileAttributes, ShareAccess,
		MyCreateDisposition, CreateOptions, EaBuffer, EaLength);
}

BOOL InstallHook()
{
	pfnNtCreateFile = (pfnNTCREATEFILE)GetProcAddress(GetModuleHandle(_T("ntdll.dll")), "NtCreateFile");
	hHookNtCreateFile = new HOOK_TRACE_INFO();
	HookNtCreateFile_ACLEntries[1] = { 0 };

	NTSTATUS    status;

	// 开始 Hook NtCreateFile 函数，使其跳转到自己的 NtCreateFileHook 函数中
	status = LhInstallHook(pfnNtCreateFile, NtCreateFileHook, NULL, hHookNtCreateFile);
	if (!SUCCEEDED(status))
	{
		CallBackfunc(2, L"LhInstallHook failed..");
		return FALSE;
	}

	// 开始 Hook，如果不调用这句，Hook 是不生效的
	status = LhSetExclusiveACL(HookNtCreateFile_ACLEntries, 1, hHookNtCreateFile);
	if (!SUCCEEDED(status))
	{
		CallBackfunc(2, L"LhSetExclusiveACL failed..");
		return FALSE;
	}

	CallBackfunc(1, L"InstallHook success...");

	return TRUE;
}

BOOL UnInstallHook()
{
	LhUninstallAllHooks();

	if (NULL != hHookNtCreateFile)
	{
		LhUninstallHook(hHookNtCreateFile);
		delete hHookNtCreateFile;
		hHookNtCreateFile = NULL;
	}

	LhWaitForPendingRemovals();

	CallBackfunc(1, L"UninstallHook success...");

	return TRUE;
}

DWORD WINAPI HookThreadProc(LPVOID lpParamter)
{
	InstallHook();
	return 0;
}

void StartHookThread()
{
	DWORD dwThreadID = 0;
	HANDLE hThread = CreateThread(NULL, 0, HookThreadProc, NULL, 0, &dwThreadID);
	CloseHandle(hThread);
}

extern "C" _declspec(dllexport) BOOL Install() {
	// 调用 InstallHook 的线程
	StartHookThread();
	CallBackfunc(1, L"DLL_PROCESS_ATTACH");
	return TRUE;
}

extern "C" _declspec(dllexport) BOOL UnInstall() {
	// 调用 UnInstallHook 的线程
	BOOL result = UnInstallHook();
	CallBackfunc(1, L"DLL_PROCESS_DETACH");
	return result;
}

extern "C" _declspec(dllexport) void __stdcall NativeInjectionEntryPoint(REMOTE_ENTRY_INFO* pInfo)
{
	SetCallBack(*reinterpret_cast<pfCallBack>(pInfo->UserData));
	Install();
	RhWakeUpProcess();
}

BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		break;
	case DLL_THREAD_ATTACH:
		break;
	case DLL_THREAD_DETACH:
		break;
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}
