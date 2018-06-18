using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO.Ports;
using System.Threading.Tasks;

namespace ArduinoCommunicator
{
    public class ArduinoCommunicator
    {
        private readonly char _objectDelimiter;
        private readonly char _propertiesDelimiter;
        private Task _engine;
        private bool _engineRunning;
        private SerialPort _serialPort;

        public delegate void MessageFromArduinoEventHandler(Dictionary<string, object> message);

        public event MessageFromArduinoEventHandler messageFromArduinoEventHandlerEvent;

        public ArduinoCommunicator(char objectDelimiter = ',', char propertiesDelimiter = ':')
        {
            _objectDelimiter = objectDelimiter;
            _propertiesDelimiter = propertiesDelimiter;
        }

        public string CreateToArduinoMessage(Dictionary<string, object> dictionary)
        {
            return string.Join(_objectDelimiter,
                dictionary.Select(kv => kv.Key + "," + kv.Value.ToString()).ToArray());
        }

        public void SendToArduino(Dictionary<string, object> dictionary)
        {
            var message = CreateToArduinoMessage(dictionary);
            _serialPort.Write(message);
        }

        public Dictionary<string, object> GetItems(string message)
        {
            var result = new Dictionary<string, object>();

            var objects = message.Split(_objectDelimiter);

            for (var i = 0; i < objects.Length; i++)
            {
                var properties = objects[i].Split(_propertiesDelimiter);

                if (properties.Length < 3)
                    return null;

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
            return result;
        }

        public void StartEngine(string comPort, int baudRate)
        {
            if (_engine == null || _engine.Status == TaskStatus.Canceled)
            {
                _engineRunning = true;
                _engine = Task.Factory.StartNew(() => ListenerEngine(comPort, baudRate));
            }
        }

        public void StopEngine()
        {
            _engineRunning = false;
        }

        public void ListenerEngine(string comPort, int baudRate)
        {
            _serialPort = new SerialPort(comPort, baudRate);
            _serialPort.Open();
            while (_engineRunning)
            {
                var line = _serialPort.ReadLine();
                var items = GetItems(line);

                if (messageFromArduinoEventHandlerEvent != null)
                    messageFromArduinoEventHandlerEvent(items);

            }
            _serialPort.Close();
        }
    }
}
