using System;

namespace Charlotte.Examples
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Running examples");

            var broker = "127.0.0.1:8883";

            new SimplePublish(broker);
            new SimpleSubscribe(broker);
            new WildcardSubscribe(broker);
        }
    }
}
