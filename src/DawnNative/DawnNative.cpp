// DawnNative.cpp : Defines the functions for the static library.
//

#include "pch.h"
#include "framework.h"

#include <dawn/webgpu.h>
#include <dawn/dawn_wsi.h>
#include <dawn_native/DawnNative.h>
#include <dawn/dawn_proc.h>
#include <GLFWUtils.h>
#include <BackendBinding.h>


extern "C" utils::BackendBinding* dawnCreateBackendBinding(wgpu::BackendType backendType, GLFWwindow* window, WGPUDevice device)
{	
	auto binding = utils::CreateBinding(backendType, window, device);
	return binding;
}

extern "C" int dawnGetPreferredSwapChainTextureFormat(utils::BackendBinding * binding)
{
	return (int)binding->GetPreferredSwapChainTextureFormat();
}

extern "C" uint64_t dawnGetSwapChainImplementation(utils::BackendBinding * binding)
{
	return binding->GetSwapChainImplementation();
}

extern "C" void dawnDestroyBackendBinding(utils::BackendBinding * ptr)
{
	if (!ptr)return;
	delete ptr;
}


extern "C" dawn_native::Instance* dawnNewInstance()
{
	auto inst = new dawn_native::Instance();

	
	return inst;

}

extern "C" void dawnDestroyInstance(dawn_native::Instance* instance)
{
	if (!instance) return;
	delete instance;
}

extern "C" void dawnEnableBackendValidation(dawn_native::Instance * instance, int validate)
{
	instance->EnableBackendValidation(validate);
}

extern "C" void dawnEnableBeginCaptureOnStartup(dawn_native::Instance * instance, int beginCaptureOnStartup)
{
	instance->EnableBeginCaptureOnStartup(beginCaptureOnStartup);
}

extern "C" void dawnEnableGPUBasedBackendValidation(dawn_native::Instance * instance, int enableGPUBasedBackendValidation)
{
	instance->EnableGPUBasedBackendValidation(enableGPUBasedBackendValidation);
}

extern "C" const dawn_native::ToggleInfo* dawnGetToggleInfo(dawn_native::Instance * instance, const char* toggleName)
{
	return instance->GetToggleInfo(toggleName);
}

extern "C" int dawnGetSupportedExtensions(dawn_native::Adapter * adapter, int bufSize, const char** extNames)
{
	auto exts = adapter->GetSupportedExtensions();
	if (!extNames) return (int)exts.size();

	auto cnt = std::min(bufSize, (int)exts.size());
	for (int i = 0; i < cnt; i++)
	{
		extNames[i] = exts[i];
	}

	return cnt;
}


extern "C" int dawnDiscoverDefaultAdapters(dawn_native::Instance* instance, int bufSize, dawn_native::Adapter** adapters)
{
	instance->DiscoverDefaultAdapters();
	auto a = instance->GetAdapters();
	
	if (!adapters) {
		auto cnt = (int)a.size();
		return cnt;
	}



	auto cnt = std::min(bufSize, (int)a.size());
	for (int i = 0; i < cnt; i++)
	{
		adapters[i] = new dawn_native::Adapter(a[i]);
	}
	return cnt;
}

extern "C" void dawnFreeAdapter(dawn_native::Adapter * adapter)
{
	delete adapter;
}


typedef struct {
	int pipelineStatisticsQuery;
	int shaderFloat16;
	int textureCompressionBC;
	int timestampQuery;
	dawn_native::BackendType backendType;
	dawn_native::DeviceType deviceType;
	uint32_t deviceId;
	uint32_t vendorId;
	const char* name;
} DawnAdapterInfo;

extern "C" DawnAdapterInfo dawnGetAdapterInfo(dawn_native::Adapter * adapter)
{
	auto props = adapter->GetAdapterProperties();
	auto backend = adapter->GetBackendType();
	auto devType = adapter->GetDeviceType();
	auto pci = adapter->GetPCIInfo();

	DawnAdapterInfo info;
	info.pipelineStatisticsQuery = props.pipelineStatisticsQuery;
	info.shaderFloat16 = props.shaderFloat16;
	info.textureCompressionBC = props.textureCompressionBC;
	info.timestampQuery = props.timestampQuery;
	info.backendType = backend;
	info.deviceType = devType;
	info.deviceId = pci.deviceId;
	info.vendorId = pci.vendorId;
	auto len = pci.name.length() + 1;
	auto res = new char[len];
	strcpy_s(res, len, pci.name.c_str());
	info.name = res;
	return info;
}

extern "C" void dawnFreeAdapterInfo(DawnAdapterInfo info)
{
	delete info.name;
}

extern "C" WGPUDevice dawnCreateDevice(dawn_native::Adapter * adapter, int extCount, const char** exts, int enabledCount, const char** enabled, int disabledCount, const char** disabled)
{
	if (extCount > 0 || enabledCount > 0 || disabledCount > 0)
	{
		dawn_native::DeviceDescriptor desc;
		desc.requiredExtensions = std::vector<const char*>();
		desc.forceEnabledToggles = std::vector<const char*>();
		desc.forceDisabledToggles = std::vector<const char*>();

		for (int i = 0; i < extCount; i++)
		{
			desc.requiredExtensions.push_back(exts[i]);
		}

		for (int i = 0; i < enabledCount; i++)
		{
			desc.forceEnabledToggles.push_back(enabled[i]);
		}

		for (int i = 0; i < disabledCount; i++)
		{
			desc.forceDisabledToggles.push_back(disabled[i]);
		}
		auto dev = adapter->CreateDevice(&desc);
		DawnProcTable backendProcs = dawn_native::GetProcs();
		dawnProcSetProcs(&backendProcs);
		return dev;
	}
	else {
		auto dev = adapter->CreateDevice();
		DawnProcTable backendProcs = dawn_native::GetProcs();
		dawnProcSetProcs(&backendProcs);
		return dev;
	}
}


// TODO: This is an example of a library function
void fnDawnNative()
{
}
