using System;

namespace AndroidLogViewer.Extensions
{
    public static class ActionExtensions
    {
        public static IDisposable ToDisposable(this Action action)
        {
            return new ActionDisposable(action);
        }

        private class ActionDisposable : IDisposable
        {
            private readonly Action action;

            public ActionDisposable(Action action)
            {
                this.action = action;
            }

            public void Dispose()
            {
                action();
            }
        }
    }
}