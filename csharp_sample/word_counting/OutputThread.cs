using csharp_sample.Job;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace csharp_sample
{
    class OutputFuncAttribute : Attribute
    {
        internal Type TargetType { get; }
        internal OutputFuncAttribute(Type type)
        {
            TargetType = type;
        }
    }

    internal static class OutputThread
    {
        static Dictionary<Type, Action<IJobResult>> JobBuilder { get; } = new Dictionary<Type, Action<IJobResult>>();

        static bool Running = false;
        static ConcurrentQueue<IJobResult> OutputQueue { get; } = new ConcurrentQueue<IJobResult>();
        static Dictionary<string, int> WordCount { get; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        static AutoResetEvent Signal { get; } = new AutoResetEvent(false);

        static OutputThread()
        {
            Build();
        }

        /// <summary>
        /// Expression Tree를 이용하여 IJobResult 타입 input 받은 것을 as cast로 함수 인자타입에 맞게끔 변환하여 호출해주는 delegate를 생성합니다.
        /// </summary>
        private static void Build()
        {
            foreach (var method in typeof(OutputThread).GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var attribute = method.GetCustomAttribute(typeof(OutputFuncAttribute)) as OutputFuncAttribute;
                if (attribute != null)
                {
                    var paramExpr = Expression.Parameter(typeof(IJobResult));
                    
                    Expression callExpr = null;
                    if (method.GetParameters().Length == 0)
                    {
                        callExpr = Expression.Call(method);
                    }
                    else if (method.GetParameters().Length == 1)
                    {
                        if (method.GetParameters()[0].ParameterType == attribute.TargetType)
                        {
                            callExpr = Expression.Call(method, Expression.TypeAs(paramExpr, attribute.TargetType));
                        }
                    }

                    if (callExpr != null)
                    {
                        var func = Expression.Lambda<Action<IJobResult>>(
                            callExpr,
                            paramExpr
                            ).Compile();
                        JobBuilder[attribute.TargetType] = func;
                    }
                    else
                    {
                        throw new Exception("Invalid Method Parameters Type");
                    }
                }
            }
        }

        internal static void OnResult(IJobResult result)
        {
            OutputQueue.Enqueue(result);
        }

        internal static void Exit()
        {
            Running = false;
            Signal.Set();
        }

        internal static void Run()
        {
            Console.WriteLine("Output Thread = " + Thread.CurrentThread.ManagedThreadId);
            Running = true;

            int delay = 10;
            while (Running)
            {
                if (OutputQueue.TryDequeue(out var result) == false)
                {
                    delay = Math.Max(delay + 10, 100);
                    Signal.WaitOne(delay);
                    continue;
                }

                delay = 10;
                if (JobBuilder.TryGetValue(result.GetType(), out var func))
                {
                    func(result);
                }
                else
                {
                    Console.WriteLine("Cannot find Job : " + result.GetType().Name);
                }

                Console.WriteLine("Job Done : " + result.GetType().Name);
                Console.WriteLine();
            }

            Console.WriteLine("Exit Output Thread");
        }


        [OutputFunc(typeof(WordCountResult))]
        internal static void WordCountApply(WordCountResult result)
        {
            foreach (var pair in result.ResultDic)
            {
                if (WordCount.ContainsKey(pair.Key) == false)
                {
                    WordCount.Add(pair.Key, 0);
                }
                WordCount[pair.Key] += pair.Value;
            }
        }

        [OutputFunc(typeof(ShowConsoleResult))]
        internal static void ShowConsole()
        {
            foreach (var pair in WordCount)
            {
                Console.WriteLine($"{pair.Key} / {pair.Value}");
            }
        }
    }
}
