using System.Collections.Generic;

namespace csharp_sample.Job
{
    /// <summary>
    /// JobExecuter에 대한 결과를 정의합니다.
    /// </summary>
    /// 
    internal interface IJobResult
    {
    }

    internal class WordCountResult : IJobResult
    {
        internal Dictionary<string, int> ResultDic { get; }
        internal WordCountResult(Dictionary<string, int> result)
        {
            ResultDic = result;
        }
    }

    internal class ShowConsoleResult : IJobResult
    {
        internal enum ShowType
        {
            Unknown = 0,
            ShowCount,
        }

        internal ShowType Type { get; }
        internal ShowConsoleResult(ShowType type)
        {
            this.Type = type;
        }
    }
}
