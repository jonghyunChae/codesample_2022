using System.Threading;

namespace csharp_sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread input = new Thread(InputThread.Run);
            Thread output = new Thread(OutputThread.Run);
            
            input.Start();
            output.Start();

            input.Join();
            output.Join();
        }
    }
}
