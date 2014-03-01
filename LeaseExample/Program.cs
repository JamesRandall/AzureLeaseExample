using System;
using System.Threading.Tasks;

namespace LeaseExample
{
    class Program
    {
        static void Main(string[] args)
        {
            SingleUpdate();
            ForceCollision();
        }

        static void SingleUpdate()
        {
            SimpleExample example = new SimpleExample();
            Guid key = example.CreateEntity();
            example.UpdateEntity(key);
        }

        static void ForceCollision()
        {
            SimpleExample example = new SimpleExample();
            Guid key = example.CreateEntity();
            Task.WaitAll(new[]
            {
                Task.Run(() => example.UpdateEntityWithDelay(key)),
                Task.Run(() => example.UpdateEntityWithDelay(key))
            });
        }
    }
}
