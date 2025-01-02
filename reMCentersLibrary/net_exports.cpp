#include "net_pch.h"
#include "net_exports.h"
#include "native.h"
#include <msclr/marshal_cppstd.h> // Include this header for string conversion

using namespace msclr::interop; // Namespace for string conversion
namespace reMCentersLibrary {
	public  ref class DllMethod
	{
	
	public: static bool Patchx64Dll() {
		return	reMCentersNative::DllMethod::Patchx64Dll();
	}

	public: static bool Patchx86Dll() {
		return	reMCentersNative::DllMethod::Patchx86Dll();
	}
    
	/// <summary>
	/// Determines Platform Type for a specified dll
	/// </summary>
	/// <param name="dllFilePath">path of dll file</param>
	/// <returns>
	/// 0 means unknown platform or error
	/// <para>1 means AMD x64</para>
	/// <para>2 means x86</para></returns>
	public: static int GetPlatform(System::String^ dllFilePath) {
		return reMCentersNative::DllMethod::GetPlatform(marshal_as<std::string>(dllFilePath));
    }

	public: static bool IsPresent() {
		return reMCentersNative::DllMethod::IsPresent();
	}
	};

}