#include "pch.h"
#include "Utility.h"


namespace MCentersNative {
	
	bool Utility::isLittleEndian;
	bool Utility::IsLittleEndian() {
		return Utility::isLittleEndian;
	}
	void Utility::Initialize() {
		int num = 1;
		if (*(char*)&num != 1) Utility::isLittleEndian = false;
		else
			Utility::isLittleEndian = true;
	}
	
	 RecordPEx86* Utility::GetRecordPEx86(std::fstream& fileStream) {
		if (!fileStream) return nullptr;
		fileStream.seekg(0, std::ios::beg);

		auto record = new RecordPEx86();
		auto dosHeader = &record->DosHeader;
		auto ntHeader = &record->NtHeader;
		auto sectionHeaders = &record->sectionHeaders;

		fileStream.read(reinterpret_cast<char*>(dosHeader), sizeof(*dosHeader));
		if (dosHeader->e_magic != IMAGE_DOS_SIGNATURE) return nullptr;

		fileStream.seekg(dosHeader->e_lfanew, std::ios::beg);

		fileStream.read(reinterpret_cast<char*>(ntHeader), sizeof(*ntHeader));
		if (ntHeader->Signature != IMAGE_NT_SIGNATURE) return nullptr;

		auto sectionsOffset=dosHeader->e_lfanew +
			sizeof(ntHeader->Signature) + // Signature
			sizeof(IMAGE_FILE_HEADER) +
			ntHeader->FileHeader.SizeOfOptionalHeader;
		fileStream.seekg(sectionsOffset, std::ios::beg);

		auto numberOfSections = ntHeader->FileHeader.NumberOfSections;
		sectionHeaders->resize(numberOfSections);
		fileStream.read(reinterpret_cast<char*>(sectionHeaders->data()), numberOfSections * sizeof(IMAGE_SECTION_HEADER));

		auto found = false;
		for (int i = 0; i < numberOfSections; ++i) {
			IMAGE_SECTION_HEADER section = (*sectionHeaders)[i];
			if (std::strcmp(reinterpret_cast<char*>(section.Name), ".text") == 0) {
				record->textSectionHeader = section;
				found = true;
				break;
			}
		}
		if (!found) return nullptr;
		return record;
	}
	
	
	RecordPEx64* Utility::GetRecordPEx64(std::fstream& fileStream) {
		if (!fileStream) return nullptr;
		fileStream.seekg(0, std::ios::beg);

		auto record = new RecordPEx64();
		auto dosHeader = &record->DosHeader;
		auto ntHeader = &record->NtHeader;
		
		auto sectionHeaders = &record->sectionHeaders;

		fileStream.read(reinterpret_cast<char*>(dosHeader), sizeof(*dosHeader));
		if (dosHeader->e_magic != IMAGE_DOS_SIGNATURE) return nullptr;

		fileStream.seekg(dosHeader->e_lfanew, std::ios::beg);

		fileStream.read(reinterpret_cast<char*>(ntHeader), sizeof(*ntHeader));
		if (ntHeader->Signature != IMAGE_NT_SIGNATURE) return nullptr;

		auto sectionsOffset = dosHeader->e_lfanew +
			sizeof(ntHeader->Signature) +
			sizeof(IMAGE_FILE_HEADER) +
			ntHeader->FileHeader.SizeOfOptionalHeader;
		fileStream.seekg(sectionsOffset, std::ios::beg);

		auto numberOfSections = ntHeader->FileHeader.NumberOfSections;
		sectionHeaders->resize(numberOfSections);
		fileStream.read(reinterpret_cast<char*>(sectionHeaders->data()), numberOfSections * sizeof(IMAGE_SECTION_HEADER));

		auto found = false;
		for (int i = 0; i < numberOfSections; ++i) {
			IMAGE_SECTION_HEADER section = (*sectionHeaders)[i];
			if (std::strcmp(reinterpret_cast<char*>(section.Name), ".text") == 0) {
				found = true;
				record->textSectionHeader = section;
				break;
			}
		}
		if (!found) return nullptr;
		found = false;
		for (int i = 0; i < numberOfSections; ++i) {
			IMAGE_SECTION_HEADER section = (*sectionHeaders)[i];
			if (std::strcmp(reinterpret_cast<char*>(section.Name), ".rdata") == 0) {
				found = true;
				record->rDataSectionHeader = section;
				break;
			}
		}
		if (!found) return nullptr;
		return record;
	}
	
	size_t Utility::SearchBytePattern(const std::vector<char>& data, char* pattern, size_t patternLength,size_t startIndex) {
		if (patternLength == 0 || data.size() < patternLength+startIndex) {
			return std::string::npos; // Use std::string::npos to indicate "not found"
		}
		auto bu = data.data();
		for (size_t i = startIndex; i <= data.size() - patternLength; ++i) {
			bool found = true;
			for (size_t j = 0; j < patternLength; ++j) {
				if (bu[i + j] != pattern[j]) {
					found = false;
					break;
				}
			}
			if (found) {
				return i; // Pattern found, return starting index
			}
		}

		return std::string::npos; // Pattern not found
	}

	bool Utility::areStringsEqual(const char* str1, const char* str2) {
		while (*str1 && *str2) {
			if (*str1 != *str2) {
				return false;
			}
			++str1;
			++str2;
		}
		// Both strings must end at the same time to be equal
		return *str1 == '\0' && *str2 == '\0';
	}
	bool Utility::stringStartsWith(const char* str, const char* prefix) {
			  while (*prefix) {
				  if (*str != *prefix) {
					  return false;
				  }
				  ++str;
				  ++prefix;
			  }
			  return true;
		  }


	bool Utility::writeBytesAtAddress(std::fstream& file, uint64_t address, const char* data, std::size_t dataSize) {
			  // Open the file for both reading and writing in binary mode
			  if (!file) {

				  return false;
			  }

			  // Seek to the specified address
			  file.seekp(static_cast<std::streampos>(address));
			  if (!file) {

				  return false;
			  }

			  // Write the data to the file
			  file.write(data, dataSize);
			  if (!file) {

				  return false;
			  }
			  return true;
		  }
	size_t Utility::find_pattern_in_array(const char* source, const char* pattern, size_t sourceSize, size_t patternSize, size_t offset) {
			  // Iterate through the source array
			  for (size_t i = offset; i <= sourceSize - patternSize; ++i) {
				  // Check if pattern matches at current position
				  if (std::memcmp(source + i, pattern, patternSize) == 0) {
					  return i;
				  }
			  }
			  // Pattern not found
			  return std::string::npos;
		  }

	


}

