using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpTestApp
{
    class Program
    {
        static int blink = 0;
        static void Main(string[] args)
        {
            Console.WriteLine("ArduinoCommunicator Test");

            var ac = new ArduinoCommunicator.ArduinoCommunicator();

            ac.messageFromArduinoEventHandlerEvent += Ac_messageFromArduinoEventHandlerEvent;

            ac.Start("COM5", 115200);

            Console.WriteLine("Waiting for messages while sending a message every 3 seconds...");

            Task.Run(() =>
            {
                while (true)
                {
                    if (blink == 0)
                        blink = 1;
                    else
                        blink = 0;
                    var msg = new Dictionary<string, object>
                    {
                        {"SetServo", "22" },
                        {"blink", blink.ToString() }
                    };

                    ac.SendToArduino(msg);

                    Thread.Sleep(300);
                }
            });
            Console.ReadKey();

        }

        private static void Ac_messageFromArduinoEventHandlerEvent(List<Dictionary<string, object>> messages)
        {
            if (messages == null)
                return;

            for (var i = 0; i < messages.Count; i++)
            {
                var message = messages[i];
                var msgs = message.ToList();

                if (msgs.Count == 0)
                    continue;

                for (var j = 0; j < msgs.Count; j++)
                {
                    var name = msgs[j].Key;
                    var value = msgs[j].Value;

                    Console.Write($"{{{name} = {value?.ToString()}}} ");
                }
                Console.WriteLine();
            }
        }
    }
}
