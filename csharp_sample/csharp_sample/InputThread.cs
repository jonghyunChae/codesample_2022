using csharp_sample.Job;
using System;
using System.Threading;

namespace csharp_sample
{
    internal static class InputThread
    {
        internal static void Run()
        {
            Console.WriteLine("!exit -> 프로그램 종료");
            Console.WriteLine("!show -> 현재 워드 카운팅 상황 확인");
            Console.WriteLine("나머지 문장 혹은 단어 입력 -> 워드 카운팅 수행");

            Console.WriteLine("Input Thread = " + Thread.CurrentThread.ManagedThreadId);

            while (true)
            {
                string input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    continue;
                }

                IJobExecuter executer = null;
                if (input[0] == '!')
                {
                    if (input.Equals("!exit", StringComparison.OrdinalIgnoreCase))
                    {
                        OutputThread.Exit();
                        break;
                    }

                    if (input.Equals("!show", StringComparison.OrdinalIgnoreCase))
                    {
                        executer = new ShowCommandProcessor();
                    }
                    else
                    {
                        Console.WriteLine("존재하지 않는 명령어입니다.");
                        continue;
                    }
                }
                else
                {
                    executer = new WordCountProcessor();
                }    

                executer?.Run(input, OutputThread.OnResult);
            }

            Console.WriteLine("Input Thread Exit");
        }
    }
}
