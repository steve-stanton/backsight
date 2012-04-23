// CEdit.h : main header file for the CEdit DLL
//

#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"		// main symbols


// CCEditApp
// See CEdit.cpp for the implementation of this class
//

class CCEditApp : public CWinApp
{
public:
	CCEditApp();

// Overrides
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};
