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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Threading;

namespace ArduinoCommunicator
{
    public class ArduinoCommunicator
    {
        private readonly char _objectDelimiter;
        private readonly char _propertiesDelimiter;
        private Task _engine;
        private bool _engineRunning;
        private SerialPort _serialPort;
        private string _incoming;
        private object _lockObject;
        public int _badFormatCount;
        private Thread _engineThread;
        private Queue<string> _sendQueue;

        public delegate void MessageFromArduinoEventHandler(List<Dictionary<string, object>> messages);

        public event MessageFromArduinoEventHandler messageFromArduinoEventHandlerEvent;

        public ArduinoCommunicator(char objectDelimiter = ',', char propertiesDelimiter = ':')
        {
            _objectDelimiter = objectDelimiter;
            _propertiesDelimiter = propertiesDelimiter;
            _lockObject = new object();
            _badFormatCount = 0;
            _sendQueue = new Queue<string>();
        }

        public string CreateToArduinoMessage(Dictionary<string, object> dictionary)
        {
            return string.Join(_objectDelimiter,
                dictionary.Select(kv => kv.Key + "," + kv.Value.ToString()).ToArray());
        }

        public void SendToArduino(Dictionary<string, object> dictionary)
        {
            //var message = CreateToArduinoMessage(dictionary);
            //_serialPort.WriteLine(message);
            _sendQueue.Enqueue(CreateToArduinoMessage(dictionary));          
        }

        public new List<Dictionary<string, object>> GetItems(string message)
        {
            var lines = message.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            var results = new List<Dictionary<string, object>>();
            for (var j = 0; j < lines.Length; j++)
            {
                var result = new Dictionary<string, object>();

                var objects = lines[j].Split(_objectDelimiter);

                try
                {
                    for (var i = 0; i < objects.Length; i++)
                    {
                        var properties = objects[i].Split(_propertiesDelimiter);

                        if (properties.Length < 3)
                            continue;

                        var type = properties[0];
                        var name = properties[1];
                        object value = null;

                        switch (type)
                        {
                            case "i":
                                value = Convert.ToInt16(properties[2]);
                                break;
                            case "u":
                                value = Convert.ToUInt16(properties[2]);
                                break;
                            case "l":
                                value = Convert.ToInt32(properties[2]);
                                break;
                            case "ul":
                                value = Convert.ToUInt32(properties[2]);
                                break;
                            case "s":
                                value = properties[2];
                                break;
                        }

                        result.Add(name, value);
                    }
                }
                catch (Exception ex)
                {
                    _badFormatCount++;
                }
                results.Add(result);
            }

            if (results == null)
            {
                int g;
                g = 1;
            }
            return results;
        }

        public void Start(string comPort, int baudRate)
        {
            _engineRunning = true;
            _serialPort = new SerialPort(comPort, baudRate);
            // _serialPort.DataReceived += _serialPort_DataReceived;
            _serialPort.Open();
            _engineThread = new Thread(Engine);
            _engineThread.Start();
        }

        public void Engine()
        {
            while (_engineRunning)
            {
                var line = _serialPort.ReadLine();
                var items = GetItems(line);

                if (items != null && messageFromArduinoEventHandlerEvent != null)
                    messageFromArduinoEventHandlerEvent(items);

                if (_sendQueue.Count > 0)
                    for (var i = 0; i < _sendQueue.Count; i++)
                    {
                        var message = _sendQueue.Dequeue();
                        _serialPort.WriteLine(message+Environment.NewLine);
                    }
            }
        }

        public void Stop()
        {
            _engineRunning = false;
            _engineThread.Join();
            _serialPort.Close();
        }

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lock (_lockObject)
            {
                var newData = _serialPort.ReadExisting();

                _incoming += newData;
                var lastNewLine = _incoming.LastIndexOf(Environment.NewLine);
                if (lastNewLine < 0)
                    return;
                var lines = _incoming.Substring(0, lastNewLine + 2);
                _incoming = _incoming.Substring(lastNewLine + 2);

                // Console.Write(lines);

                var items = GetItems(lines);

                if (items == null)
                {
                    int g;
                    g = 1;
                }

                if (messageFromArduinoEventHandlerEvent != null)
                    messageFromArduinoEventHandlerEvent(items);
            }
        }
    }
}
