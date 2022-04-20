using Avalonia.Input;
using Reaction.Providers;
using System;

namespace Sample.Helpers
{
    public class MyReactionParameterProvider : IReactParameterProvider
    {
        public object? GetDefaultCommandParameter(object sender, EventArgs e)
        {
            return e switch
            {
                PointerWheelEventArgs pw => pw.Delta.Y,
                KeyEventArgs k => k.Key == Key.Up ? 1d : 0d,
                _ => null,
            };
        }
    }
}
