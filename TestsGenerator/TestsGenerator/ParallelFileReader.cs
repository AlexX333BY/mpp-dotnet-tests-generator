using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TestsGenerator
{
    public class ParallelFileReader : IParallelReader
    {
        protected readonly int maxThreadsCount;
        protected readonly List<string> filePaths;
        protected Dictionary<string, string> successfullyReadText;

        public IDictionary<string, string> SuccessfullyReadText => new Dictionary<string, string>(successfullyReadText);

        public void ReadText()
        {
            var result = new ConcurrentDictionary<string, string>();
            var exceptions = new ConcurrentBag<Exception>();

            ParallelOptions options = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxThreadsCount
            };
            Parallel.ForEach(filePaths, options, path =>
                {
                    try
                    {
                        result[path] = File.ReadAllText(path);
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e);
                    }
                }
            );
            successfullyReadText = new Dictionary<string, string>(result);

            if (!exceptions.IsEmpty)
            {
                throw new AggregateException(exceptions);
            }
        }

        public ParallelFileReader(IEnumerable<string> paths, int maxThreadsCount)
        {
            if (maxThreadsCount < 1)
            {
                throw new ArgumentException("There should be at least 1 thread");
            }
            if (paths == null)
            {
                throw new ArgumentException("Paths shouldn't be null");
            }

            this.maxThreadsCount = maxThreadsCount;
            filePaths = new List<string>(paths);
            successfullyReadText = new Dictionary<string, string>();
        }

        public ParallelFileReader(IEnumerable<string> paths) 
            : this(paths, Environment.ProcessorCount)
        { }
    }
}
