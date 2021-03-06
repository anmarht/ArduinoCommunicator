// TestApp.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "stdio.h"
#include "ArduinoCommunicator.h"

int main()
{
	char pcToArduinoFormat[] = "name,anmar,counter,28732";

	ArduinoCommunicator ac;

	SendToPC(ac, 
		ac.IntValue("counter", 4, true), 
		ac.StrValue("name", "anmar"), 
		ac.LongValue("longnum", 342432));

	char * counter = ac.GetField(pcToArduinoFormat, "counter");

	printf("\r\n%s", counter);

	return 0;
}

