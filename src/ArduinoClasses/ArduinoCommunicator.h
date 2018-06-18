/**************************************************************************
*  Copyright 2018 Anmar Al-Tamimi <anmarhasan@gmail.com>
*
*  Licensed under the Apache License, Version 2.0 (the "License");
*  you may not use this file except in compliance with the License.
*  You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
*  Unless required by applicable law or agreed to in writing, software
*  distributed under the License is distributed on an "AS IS" BASIS,
*  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*  See the License for the specific language governing permissions and
*  limitations under the License.
*
*  ArduinoCommunicator v1.0
*/

#pragma once

class ArduinoCommunicator
{
private:
  char *_buffer;
  char *_tmpbuf;
  char _delimiter;

public:
  ArduinoCommunicator(char delimiter = ',');
  void * IntValue(const char *name, int value, bool final = false);
  void * UIntValue(const char *name, unsigned int value, bool final = false);
  void * LongValue(const char *name, long value, bool final = false);
  void * ULongValue(const char *name, unsigned long value, bool final = false);
  void * StrValue(const char *name, const char *value, bool final = false);
  void CleanBuffer();
  void Send(...);
  char* GetField(char* line, const char *fieldName);
  
};

#define SendToPC(arduino_communicator, ...) \
  arduino_communicator.CleanBuffer(); \
  arduino_communicator.Send(__VA_ARGS__);

