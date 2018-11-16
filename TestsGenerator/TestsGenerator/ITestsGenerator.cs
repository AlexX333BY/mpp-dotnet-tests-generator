using System.Threading.Tasks;

namespace TestsGenerator
{
    public interface ITestsGenerator
    {
        void Generate();
        Task GetGenerateTask();
    }
}
