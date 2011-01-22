#pragma once

using namespace System;

// See http://blogs.msdn.com/jeremykuhne/archive/2005/06/11/428363.aspx
// ...another approach (which may or may not be better) is described
// at http://msdn.microsoft.com/en-us/library/805c56f8(VS.80).aspx
// also http://support.microsoft.com/kb/311259

public ref class Chars
{
	private:

		// Data
		IntPtr unmanagedStringPointer;

		// Methods
		Chars(String^ managedString);
		char* ToNativeString();

	public:

		static char* Convert(String^ managedString);
		static String^ Convert(char* nativeString);
		~Chars();
};
