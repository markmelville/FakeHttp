using System;

namespace FakeHttp.Rules
{
    public interface IRuleSet<out T, in TOut>
    {
        void AddRule(Func<T, TOut> selector);

        void AddRule(Func<T, bool> predicate, Func<T, TOut> selector);

        void AddRule(Func<T, bool> predicate, TOut output);
    }

    public interface IRuleSet<out T, out T2, in TOut>
    {
        void AddRule(Func<T, T2, TOut> selector);

        void AddRule(Func<T, T2, bool> predicate, Func<T, T2, TOut> selector);

        void AddRule(Func<T, T2, bool> predicate, TOut output);
    }
}
