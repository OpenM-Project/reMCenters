#include "pch.h"
#include "Zydis.h"

#include <fstream>
#include <vector>
#include <iostream>
#include <algorithm>
#include "Utility.h"

constexpr uint32_t UINT32_t_MAX = (0xffffffff);

namespace MCentersNative {
	namespace DllMethod {
		const  int SizeOfx64LeaOpcode = 7;
		const int SizeOfx64LeaOpcodeWithoutMemoryOffset = 3;

		const char CrackOpcode[] = { '\xb1', '\x00', '\x90' };
		const char* x64LeaReferenceOpcodePrefix = "\x4c\x8d\x05"; //means lea r8,
		const char* Windows_Services_Store_StoreApplicense_getTrial_keyword = "Windows::Services::Store::StoreAppLicense::get_IsTrial";
		const char* Windows_ApplicationModel_Store_LicenseInformationServer_getTrial_keyword = "Windows::ApplicationModel::Store::LicenseInformationServer::get_IsTrial";

		/// <summary>
		/// Determines Platform Type for a specified dll
		/// </summary>
		/// <param name="dllFile">path of dll file</param>
		/// <returns>0 means unknown platform
		/// 1 means Amd x64
		/// 2 neabs x86
		/// </returns>
		int GetPlatform(std::string dllFile)
		{
			std::ifstream file(dllFile, std::ios::binary);
			if (!file) {
				return 0;
			}

			// Read DOS Header
			IMAGE_DOS_HEADER dosHeader;
			file.read(reinterpret_cast<char*>(&dosHeader), sizeof(dosHeader));
			if (dosHeader.e_magic != IMAGE_DOS_SIGNATURE) {
				file.close();
				return 0;
			}

			// Move to the PE Header
			file.seekg(dosHeader.e_lfanew, std::ios::beg);
			DWORD peSignature;
			file.read(reinterpret_cast<char*>(&peSignature), sizeof(peSignature));
			if (peSignature != IMAGE_NT_SIGNATURE) {
				file.close();
				return 0;
			}

			// Read the File Header
			IMAGE_FILE_HEADER fileHeader;
			file.read(reinterpret_cast<char*>(&fileHeader), sizeof(fileHeader));

			if (fileHeader.Machine == IMAGE_FILE_MACHINE_AMD64) {
				file.close();
				return 1;
			}
			if (fileHeader.Machine == IMAGE_FILE_MACHINE_I386) {
				file.close();
				return 2;
			}
			file.close();
			return 0;

		}

		bool Patchx64Dll() {
			bool result = false;
			if (!MCentersNative::Utility::IsLittleEndian()) return false;  // this line ensures code only runs when running in little endian mode, we dont support big endian for now

			std::fstream file("C:\\ProgramData\\MCenters\\Methods\\AutoPatch\\x64\\Windows.ApplicationModel.Store.dll", std::ios::in | std::ios::out | std::ios::binary);
			if (!file) {

				return false;
			}
			std::vector<char> textBuffer;
			std::vector<char> rDataBuffer;
			IMAGE_SECTION_HEADER* textHeader;
			IMAGE_SECTION_HEADER* rDataHeader;
			auto recordPex64= MCentersNative::Utility::GetRecordPEx64(file);
			
			if (recordPex64 == nullptr) goto exit;
			 
			textHeader = &recordPex64->textSectionHeader;
			rDataHeader = &recordPex64->rDataSectionHeader;

			textBuffer.resize(textHeader->SizeOfRawData);
			rDataBuffer.resize(rDataHeader->SizeOfRawData);

			file.seekg(textHeader->PointerToRawData, std::ios::beg);
			if (!file.read(textBuffer.data(), textBuffer.size()))  goto exit;

			file.seekg(rDataHeader->PointerToRawData, std::ios::beg);
			if (!file.read(rDataBuffer.data(), rDataBuffer.size()))  goto exit;
			else {
				auto textBaseAddress = recordPex64->NtHeader.OptionalHeader.ImageBase + textHeader->VirtualAddress;
				auto rDataBaseAddress = recordPex64->NtHeader.OptionalHeader.ImageBase + rDataHeader->VirtualAddress;

				// windows service store part

				auto storeApplicenseGetTrialKeywordPosition = MCentersNative::Utility::find_pattern_in_array(rDataBuffer.data(), Windows_Services_Store_StoreApplicense_getTrial_keyword, rDataBuffer.size(), 55);
				if (storeApplicenseGetTrialKeywordPosition != -1) {
					storeApplicenseGetTrialKeywordPosition += rDataBaseAddress;
					size_t storeApplicenseGetTrialKeywordReferenceOffset = 0;



					int offsetInLea = 0;
					char* leaOffsetter = reinterpret_cast<char*>(&offsetInLea);
					while (true) {
						storeApplicenseGetTrialKeywordReferenceOffset = MCentersNative::Utility::find_pattern_in_array(textBuffer.data(), x64LeaReferenceOpcodePrefix, textBuffer.size(), 3, storeApplicenseGetTrialKeywordReferenceOffset + 1);
						if (storeApplicenseGetTrialKeywordReferenceOffset == -1) goto exit;
						auto startOffset = storeApplicenseGetTrialKeywordReferenceOffset + SizeOfx64LeaOpcodeWithoutMemoryOffset;
						if (startOffset + 4 > textBuffer.size())goto exit;
						leaOffsetter[0] = textBuffer[startOffset];
						leaOffsetter[1] = textBuffer[startOffset+1];
						leaOffsetter[2] = textBuffer[startOffset+2];
						leaOffsetter[3] = textBuffer[startOffset+3];

						
						if (textBaseAddress + storeApplicenseGetTrialKeywordReferenceOffset + offsetInLea + SizeOfx64LeaOpcode == storeApplicenseGetTrialKeywordPosition) {


							auto dataStartPoint = textBuffer.data() + storeApplicenseGetTrialKeywordReferenceOffset;
							size_t size = storeApplicenseGetTrialKeywordReferenceOffset + 0x200 > textBuffer.size() ?
								textBuffer.size() - storeApplicenseGetTrialKeywordReferenceOffset :
								0x200;
							// The runtime address (instruction pointer) was chosen arbitrarily here in order to better 
							// visualize relative addressing. In your actual program, set this to e.g. the memory address 
							// that the code being disassembled was read from. 
							ZyanU64 runtime_address = storeApplicenseGetTrialKeywordReferenceOffset+textHeader->PointerToRawData;

							// Loop over the instructions in our buffer. 
							ZyanUSize offset = 0;
							ZydisDisassembledInstruction instruction;
							while (ZYAN_SUCCESS(ZydisDisassembleIntel(
								/* machine_mode:    */ ZYDIS_MACHINE_MODE_LONG_64,
								/* runtime_address: */ runtime_address,
								/* buffer:          */ dataStartPoint + offset,
								/* length:          */ size - offset,
								/* instruction:     */ &instruction
							))) {

								auto opcode = instruction.text;
								if (MCentersNative::Utility::stringStartsWith(opcode, "mov cl, [rcx")) {
									if (!MCentersNative::Utility::writeBytesAtAddress(file, runtime_address, CrackOpcode, 3)) goto exit;
									break;
								}
								if (MCentersNative::Utility::areStringsEqual(opcode, "mov cl, 0x00"))
									break;

								if (MCentersNative::Utility::stringStartsWith(opcode, "ret"))
									goto exit;

								offset += instruction.info.length;
								runtime_address += instruction.info.length;
							}
							break;
						}
					}
				}


				// windows application model store part
				auto licenseInformationServerGetTrialKeywordPosition = MCentersNative::Utility::find_pattern_in_array(rDataBuffer.data(), Windows_ApplicationModel_Store_LicenseInformationServer_getTrial_keyword, rDataBuffer.size(), 72);
				if (licenseInformationServerGetTrialKeywordPosition == -1) goto exit; // no dll will be missing this value so we have an error in dll and we must exit
				size_t licenseInformationServerGetTrialKeywordReferenceOffset = 0;
				licenseInformationServerGetTrialKeywordPosition += rDataBaseAddress;



				int offsetInLea = 0;
				char* leaOffsetter = reinterpret_cast<char*>(&offsetInLea);
				while (true) {
					licenseInformationServerGetTrialKeywordReferenceOffset = MCentersNative::Utility::find_pattern_in_array(textBuffer.data(), x64LeaReferenceOpcodePrefix, textBuffer.size(), 3, licenseInformationServerGetTrialKeywordReferenceOffset + 1);
					if (licenseInformationServerGetTrialKeywordReferenceOffset == -1) goto exit;

					auto startOffset = licenseInformationServerGetTrialKeywordReferenceOffset + SizeOfx64LeaOpcodeWithoutMemoryOffset;
					if (startOffset + 4 > textBuffer.size())goto exit;
					leaOffsetter[0] = textBuffer[startOffset];
					leaOffsetter[1] = textBuffer[startOffset + 1];
					leaOffsetter[2] = textBuffer[startOffset + 2];
					leaOffsetter[3] = textBuffer[startOffset + 3];

					
					if (textBaseAddress+ licenseInformationServerGetTrialKeywordReferenceOffset + offsetInLea + SizeOfx64LeaOpcode == licenseInformationServerGetTrialKeywordPosition) {


						auto dataStartPoint = textBuffer.data() + licenseInformationServerGetTrialKeywordReferenceOffset;
						size_t size = licenseInformationServerGetTrialKeywordReferenceOffset + 0x200 > textBuffer.size() ?
							textBuffer.size() - licenseInformationServerGetTrialKeywordReferenceOffset :
							0x200;

						// The runtime address (instruction pointer) was chosen arbitrarily here in order to better 
						// visualize relative addressing. In your actual program, set this to e.g. the memory address 
						// that the code being disassembled was read from. 
						ZyanU64 runtime_address = licenseInformationServerGetTrialKeywordReferenceOffset+textHeader->PointerToRawData;

						// Loop over the instructions in our buffer. 
						ZyanUSize offset = 0;
						ZydisDisassembledInstruction instruction;
						while (ZYAN_SUCCESS(ZydisDisassembleIntel(
							/* machine_mode:    */ ZYDIS_MACHINE_MODE_LONG_64,
							/* runtime_address: */ runtime_address,
							/* buffer:          */ dataStartPoint + offset,
							/* length:          */ size - offset,
							/* instruction:     */ &instruction
						))) {

							auto opcode = instruction.text;

							if (MCentersNative::Utility::stringStartsWith(opcode, "setnz cl")) {

								if (!MCentersNative::Utility::writeBytesAtAddress(file, runtime_address, CrackOpcode, 3))goto exit;
								break;
							}
							if (MCentersNative::Utility::areStringsEqual(opcode, "mov cl, 0x00")) 
								break;
							
							if (MCentersNative::Utility::stringStartsWith(opcode, "ret"))
								goto exit;

							offset += instruction.info.length;
							runtime_address += instruction.info.length;
						}
						break;
					}
				}


			}
			result = true;
		exit:;
			file.close();
			return result;
		}

		//
		///
		// x86 dll
		///
		//




		char x86PushOpcode[5] = { '\x68' };//means push offset memory location and used with service store
		uint32_t* x86PushOffset = reinterpret_cast<uint32_t*>(x86PushOpcode + 1);

		char x86EbxOpcode[5] = { '\xbb' };//means mov ebx,memory location and used with application model store
		uint32_t* x86EbxOffset = reinterpret_cast<uint32_t*>(x86EbxOpcode + 1);


		bool Patchx86Dll() {
			bool result = false;
			if (!MCentersNative::Utility::IsLittleEndian()) return false;

			std::fstream file("C:\\ProgramData\\MCenters\\Methods\\AutoPatch\\x86\\Windows.ApplicationModel.Store.dll", std::ios::in | std::ios::out | std::ios::binary);
			if (!file) {

				return false;
			}
			file.seekg(0, std::ios::beg);

			std::vector<char> textSectionBuffer;

			auto recordPEx86 = MCentersNative::Utility::GetRecordPEx86(file);
			if (recordPEx86 == nullptr)goto exit;


			file.seekg(recordPEx86->textSectionHeader.PointerToRawData, std::ios::beg);

			textSectionBuffer.resize(recordPEx86->textSectionHeader.SizeOfRawData);
			if (!file.read(textSectionBuffer.data(), textSectionBuffer.size())) goto exit;

			else {
				auto baseAddress = recordPEx86->NtHeader.OptionalHeader.ImageBase + recordPEx86->textSectionHeader.VirtualAddress;


				size_t storeApplicenseGetTrialKeywordPosition = MCentersNative::Utility::find_pattern_in_array(textSectionBuffer.data(), Windows_Services_Store_StoreApplicense_getTrial_keyword, textSectionBuffer.size(), 55) + baseAddress;



				if (storeApplicenseGetTrialKeywordPosition != std::string::npos) {
					if (storeApplicenseGetTrialKeywordPosition > UINT32_t_MAX) goto exit;
					size_t storeApplicenseGetTrialKeywordReferenceOffset = -1;

					*x86PushOffset = storeApplicenseGetTrialKeywordPosition;

					bool wroteSome = false;
					while (!wroteSome) {
						storeApplicenseGetTrialKeywordReferenceOffset = MCentersNative::Utility::SearchBytePattern(textSectionBuffer, x86PushOpcode, 5, storeApplicenseGetTrialKeywordReferenceOffset + 1);
						if (storeApplicenseGetTrialKeywordReferenceOffset == std::string::npos)
						{
							goto exit;
						}



						auto dataStartPoint = textSectionBuffer.data() + storeApplicenseGetTrialKeywordReferenceOffset;
						size_t size = storeApplicenseGetTrialKeywordReferenceOffset + 0x200 > textSectionBuffer.size() ?
							textSectionBuffer.size() - storeApplicenseGetTrialKeywordReferenceOffset :
							0x200;

						// The runtime address (instruction pointer) was chosen arbitrarily here in order to better 
						// visualize relative addressing. In your actual program, set this to e.g. the memory address 
						// that the code being disassembled was read from. 
						ZyanU64 runtime_address = storeApplicenseGetTrialKeywordReferenceOffset + recordPEx86->textSectionHeader.PointerToRawData;

						// Loop over the instructions in our buffer. 
						ZyanUSize offset = 0;
						ZydisDisassembledInstruction instruction;
						while (ZYAN_SUCCESS(ZydisDisassembleIntel(
							/* machine_mode:    */ ZYDIS_MACHINE_MODE_LONG_COMPAT_32,
							/* runtime_address: */ runtime_address,
							/* buffer:          */ dataStartPoint + offset,
							/* length:          */ size - offset,
							/* instruction:     */ &instruction
						))) {

							auto opcode = instruction.text;
							if (MCentersNative::Utility::stringStartsWith(opcode, "mov cl, [ecx")) {
								if (!MCentersNative::Utility::writeBytesAtAddress(file, runtime_address, CrackOpcode, 3)) goto exit;
								wroteSome = true;
								break;
							}
							if (MCentersNative::Utility::areStringsEqual(opcode, "mov cl, 0x00")) {
								wroteSome = true;
								break;
							}
							if (MCentersNative::Utility::stringStartsWith(opcode, "ret"))
								break;

							offset += instruction.info.length;
							runtime_address += instruction.info.length;
						}


					}
				}


				// windows application model store part
				auto licenseInformationServerGetTrialKeywordPosition = MCentersNative::Utility::find_pattern_in_array(textSectionBuffer.data(), Windows_ApplicationModel_Store_LicenseInformationServer_getTrial_keyword, textSectionBuffer.size(), 72) + baseAddress;
				if (licenseInformationServerGetTrialKeywordPosition == std::string::npos || licenseInformationServerGetTrialKeywordPosition > UINT32_t_MAX) goto exit; // no dll will be missing this value so we have an error in dll and we must exit



				size_t licenseInformationServerGetTrialKeywordReferenceOffset = -1;


				*x86EbxOffset = licenseInformationServerGetTrialKeywordPosition;



				bool wroteSome = false;
				while (!wroteSome) {
					licenseInformationServerGetTrialKeywordReferenceOffset = MCentersNative::Utility::SearchBytePattern(textSectionBuffer, x86EbxOpcode, 5, licenseInformationServerGetTrialKeywordReferenceOffset + 1);
					if (licenseInformationServerGetTrialKeywordReferenceOffset == std::string::npos)
					{
						goto exit;
					}

					auto dataStartPoint = textSectionBuffer.data() + licenseInformationServerGetTrialKeywordReferenceOffset;
					size_t size = licenseInformationServerGetTrialKeywordReferenceOffset + 0x200 > textSectionBuffer.size() ?
						textSectionBuffer.size() - licenseInformationServerGetTrialKeywordReferenceOffset :
						0x200;

					// The runtime address (instruction pointer) was chosen arbitrarily here in order to better 
					// visualize relative addressing. In your actual program, set this to e.g. the memory address 
					// that the code being disassembled was read from. 
					ZyanU64 runtime_address = licenseInformationServerGetTrialKeywordReferenceOffset + recordPEx86->textSectionHeader.PointerToRawData;

					// Loop over the instructions in our buffer. 
					ZyanUSize offset = 0;
					ZydisDisassembledInstruction instruction;
					while (ZYAN_SUCCESS(ZydisDisassembleIntel(
						/* machine_mode:    */ ZYDIS_MACHINE_MODE_LONG_COMPAT_32,
						/* runtime_address: */ runtime_address,
						/* buffer:          */ dataStartPoint + offset,
						/* length:          */ size - offset,
						/* instruction:     */ &instruction
					))) {

						auto opcode = instruction.text;

						if (MCentersNative::Utility::stringStartsWith(opcode, "setnz cl")) {

							if (!MCentersNative::Utility::writeBytesAtAddress(file, runtime_address, CrackOpcode, 3))goto exit;
							wroteSome = true;
							break;
						}
						if (MCentersNative::Utility::areStringsEqual(opcode, "mov cl, 0x00")) {
							wroteSome = true;
							break;
						}
						if (MCentersNative::Utility::stringStartsWith(opcode, "ret"))
							break;

						offset += instruction.info.length;
						runtime_address += instruction.info.length;
					}
					break;

				}


			}
			result = true;
		exit:;
			file.close();
			return result;
		}

		bool IsPresent() {
			return true;
		}
	}


}