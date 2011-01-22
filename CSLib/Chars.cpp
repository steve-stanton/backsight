#include "Chars.h"

using namespace System;

	// Constructor for the managed to native conversion.
	// [Private]
	Chars::Chars(String^ managedString)
	{
		this->unmanagedStringPointer = System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(managedString);
	}

	// Converts the marshalled pointer to a native string.
	// [Private]
	char* Chars::ToNativeString()
	{
		return static_cast<char*>(this->unmanagedStringPointer.ToPointer());
	}

	// Converts a managed string to a native string.
	// [Public] [Static]
	char* Chars::Convert(String^ managedString)
	{
		return (gcnew Chars(managedString))->ToNativeString();
	}

	// Converts a native string to a managed string.
	// [Public] [Static]
	String^ Chars::Convert(char* nativeString)
	{
		return System::Runtime::InteropServices::Marshal::PtrToStringAnsi(static_cast<IntPtr>(nativeString));
	}

	// Destructor (implicitly implements IDisposable)
	// [Public]
	Chars::~Chars()
	{
		if(this->unmanagedStringPointer != IntPtr::Zero)
		{
			System::Runtime::InteropServices::Marshal::FreeHGlobal(this->unmanagedStringPointer);
		}
	}
