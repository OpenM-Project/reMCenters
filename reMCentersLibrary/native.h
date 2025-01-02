#pragma once
#include <string>

namespace reMCentersNative {
	namespace DllMethod {
		bool Patchx64Dll();
		bool Patchx86Dll();
		int GetPlatform(std::string dllFile);
		bool IsPresent();
	}
}