using System.Collections.Generic;
using System.Linq;

namespace csharp_sample.Job
{
    /// <summary>
    /// 재활용을 위한 최소한의 작업 단위를 지정합니다.
    /// </summary>
    /// 

    internal static class WordSplitJob
    {
        internal static string[] Do(string input)
        {
            return input.Split();
        }
    }

    internal static class WordGroupingJob
    {
        // Linq Sample
        internal static IEnumerable<KeyValuePair<string, int>> Do(string[] input)
        {
            return input
                .GroupBy(x => x)
                .Where(x => string.IsNullOrEmpty(x.Key) == false)
                .Select(x => new KeyValuePair<string, int>(x.Key, x.Count()));
        }
    }
}
