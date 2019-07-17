using System.Threading.Tasks;

namespace AndroidLogViewer
{
    public static class TaskExtensions
    {
        public static void FireAndForget(this Task self) { }
        public static void FireAndForget<T>(this Task<T> self) { }

    }
}