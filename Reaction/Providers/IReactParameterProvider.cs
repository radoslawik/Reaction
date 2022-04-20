using System;

namespace Reaction.Providers
{
    public interface IReactParameterProvider
    {
        object? GetDefaultCommandParameter(object sender, EventArgs e);
    }
}
