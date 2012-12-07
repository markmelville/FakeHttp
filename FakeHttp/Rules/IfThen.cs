using System;

namespace FakeHttp.Rules
{
    public class IfThen<T, TOut>
    {
        public IfThen(Func<T, bool> predicate, Func<T, TOut> selector)
        {
            Predicate = predicate;
            Selector = selector;
        }

        public Func<T, bool> Predicate { get; private set; }
        public Func<T, TOut> Selector { get; private set; }
    }

    public class IfThen<T, T2, TOut>
    {
        public IfThen(Func<T, T2, bool> predicate, Func<T, T2, TOut> selector)
        {
            Predicate = predicate;
            Selector = selector;
        }

        public Func<T, T2, bool> Predicate { get; private set; }
        public Func<T, T2, TOut> Selector { get; private set; }
    }
}
