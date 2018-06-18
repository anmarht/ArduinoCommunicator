using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ArduinoCommunicator Test");

            var ac = new ArduinoCommunicator.ArduinoCommunicator();

            ac.messageFromArduinoEventHandlerEvent += Ac_messageFromArduinoEventHandlerEvent;

            ac.StartEngine("COM5", 115200);

            Console.WriteLine("Waiting for messages...");

            Console.ReadKey();

        }

        private static void Ac_messageFromArduinoEventHandlerEvent(Dictionary<string, object> message)
        {
            Console.WriteLine("Received from Arduion:");

            var msgs = message.ToList();
                        
            for (var i = 0; i < message.Count; i++)
            {
                var name = msgs[i].Key;
                var value = msgs[i].Value;

                Console.WriteLine($"Key = {name}, Value = {value.ToString()}");
            }            
        }
    }
}
