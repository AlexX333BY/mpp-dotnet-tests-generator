using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TestsGenerator
{
    public class ParallelFileReader : IParallelReader
    {
        protected Dictionary<string, string> successfullyReadText;
        protected int threadsCount;

        public IDictionary<string, string> SuccessfullyReadText => new Dictionary<string, string>(successfullyReadText);

        public int ThreadsCount
        {
            get => threadsCount;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("There should be at least 1 thread");
                }
                threadsCount = value;
            }
        }

        public IDictionary<string, string> ReadText(IEnumerable<string> paths)
        {
            var result = new ConcurrentDictionary<string, string>();
            var exceptions = new ConcurrentBag<Exception>();
            ParallelOptions options = new ParallelOptions
            {
                MaxDegreeOfParallelism = ThreadsCount
            };

            if (paths == null)
            {
                throw new ArgumentException("Paths shouldn't be null");
            }

            Parallel.ForEach(paths, options, path =>
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
            return SuccessfullyReadText;
        }

        public ParallelFileReader() 
        {
            threadsCount = 1;
            successfullyReadText = new Dictionary<string, string>();
        }
    }
}
