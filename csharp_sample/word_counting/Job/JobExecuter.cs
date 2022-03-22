using System;
using System.Linq;
using System.Threading.Tasks;

namespace csharp_sample.Job
{
    /// <summary>
    /// 작업 수행 로직을 정의합니다. 비동기 코드를 보여주기 위해 일부러 async / await를 사용합니다.
    /// Job 인스턴스들의 조합으로 수행하게끔 할까 하다가 코드양이 많아질 것 같아서 패스합니다.
    /// </summary>
    /// 

    internal interface IJobExecuter
    {
        internal async Task Run(string input, Action<IJobResult> onResult)
        {
            var result = await RunInternal(input);
            if (result != null)
            {
                onResult?.Invoke(result);
            }
        }

        protected abstract Task<IJobResult> RunInternal(string input);
    }

    internal class WordCountProcessor : IJobExecuter
    {
        async Task<IJobResult> IJobExecuter.RunInternal(string input)
        {
            // await를 이용하여 비동기 예시
            var splitted = await Task.Run(() => WordSplitJob.Do(input));
            var grouping = WordGroupingJob.Do(splitted);
            return new WordCountResult(grouping.ToDictionary(x => x.Key, x => x.Value));
        }
    }

    internal class ShowCommandProcessor : IJobExecuter
    {
        // await 없이 동기 예시
        async Task<IJobResult> IJobExecuter.RunInternal(string input)
        {
            return new ShowConsoleResult(ShowConsoleResult.ShowType.ShowCount);
        }
    }
}
