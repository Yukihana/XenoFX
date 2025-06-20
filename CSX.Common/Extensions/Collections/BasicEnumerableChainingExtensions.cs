using System;

namespace CSX.Common.Extensions.Collections;

public static class BasicEnumerableChainingExtensions
{
    /// <summary>
    /// Allows you to chain a function call with an input value,
    /// similar to the `let` keyword in some languages.
    /// Usage: `input.Let(SomeMethod)`
    /// </summary>
    public static TResult Let<T, TResult>(this T input, Func<T, TResult> func)
        => func(input);

    /// <summary>
    /// Allows you to chain a function call with an input value,
    /// similar to the `let` keyword in some languages.
    /// This overload takes an additional state parameter.
    /// Usage: `input.Let(SomeMethod, state)`
    /// </summary>
    public static TResult Let<T, TState, TResult>(this T input, Func<T, TState, TResult> func, TState state)
        => func(input, state);
}