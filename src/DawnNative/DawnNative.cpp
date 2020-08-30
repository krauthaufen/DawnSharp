// DawnNative.cpp : Defines the functions for the static library.
//

#include "pch.h"
#include "framework.h"

#include <dawn/webgpu.h>
#include <dawn_native/DawnNative.h>
#include <dawn/dawn_proc.h>

extern "C" dawn_native::Instance* dawnNewInstance()
{
	auto inst = new dawn_native::Instance();

	DawnProcTable backendProcs = dawn_native::GetProcs();
	dawnProcSetProcs(&backendProcs);
	return inst;

}

extern "C" void dawnDestroyInstance(dawn_native::Instance* instance)
{
	if (!instance) return;
	delete instance;
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

extern "C" WGPUDevice dawnCreateDevice(dawn_native::Adapter * adapter)
{
	return adapter->CreateDevice();
}


// TODO: This is an example of a library function
void fnDawnNative()
{
}
