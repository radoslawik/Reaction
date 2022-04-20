using System;

namespace Reaction.Providers
{
    internal class DefaultReactParamProvider : IReactParameterProvider
    {
        public object? GetDefaultCommandParameter(object sender, EventArgs e) => null;
    }
}
