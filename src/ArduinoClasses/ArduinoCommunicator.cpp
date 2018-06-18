/**************************************************************************
*  Copyright 2008 Anmar Al-Tamimi <anmarhasan@gmail.com>
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

#include "ArduinoCommunicator.h"
#include <stdlib.h>
#include <string.h>
#include "stdio.h"
#include "stdarg.h"
#include <Arduino.h>

ArduinoCommunicator::ArduinoCommunicator(char delimiter)
{
  _buffer = (char *)malloc(1024);
  _tmpbuf = (char *)malloc(128);
  _delimiter = delimiter;
}


void * ArduinoCommunicator::IntValue(const char *name, int value, bool final)
{
  sprintf(_tmpbuf, "i:%s:%d%c", name, value, !final ? _delimiter : 0);
  strcat(_buffer, _tmpbuf);
  return NULL;
}

void * ArduinoCommunicator::UIntValue(const char *name, unsigned int value, bool final)
{
  sprintf(_tmpbuf, "u:%s:%u%c", name, value, !final ? _delimiter : 0);
  strcat(_buffer, _tmpbuf);
  return NULL;
}

void * ArduinoCommunicator::LongValue(const char *name, long value, bool final)
{
  sprintf(_tmpbuf, "l:%s:%ld%c", name, value, !final ? _delimiter : 0);
  strcat(_buffer, _tmpbuf);
  return NULL;
}

void * ArduinoCommunicator::ULongValue(const char *name, unsigned long value, bool final)
{
  sprintf(_tmpbuf, "ul:%s:%lu%c", name, value, !final ? _delimiter : 0);
  strcat(_buffer, _tmpbuf);
  return NULL;
}

void * ArduinoCommunicator::StrValue(const char *name, const char *value, bool final)
{
  sprintf(_tmpbuf, "s:%s:%s%c", name, value, !final ? _delimiter : 0);
  strcat(_buffer, _tmpbuf);
  return NULL;
}

void ArduinoCommunicator::CleanBuffer()
{
  memset((void *)_buffer, 0, sizeof(_buffer));
}

void ArduinoCommunicator::Send(...)
{
  Serial.println(_buffer);
}

char* ArduinoCommunicator::GetField(char* line, const char *fieldName)
{
  char *token = NULL;
  char delimiterString[] = "\0\0";
  bool getNext = false;
  delimiterString[0] = _delimiter;
  token = strtok(line, delimiterString);

  if (strcmp(token, fieldName) == 0)
    getNext = true;

  while (token != NULL)
  {
    token = strtok(NULL, delimiterString);

    if (getNext)
      return token;

    if (strcmp(token, fieldName) == 0)
      getNext = true;
  }
}
