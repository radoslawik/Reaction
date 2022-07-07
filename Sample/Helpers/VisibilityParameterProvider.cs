using Avalonia;
using Reaction.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Helpers
{
    internal class VisibilityParameterProvider : IReactParameterProvider
    {
        public object? GetDefaultCommandParameter(object sender, EventArgs e)
        {
            return e switch
            {
                AvaloniaPropertyChangedEventArgs pc => pc.Property.Name switch
                {
                    "IsPointerOver" => pc.NewValue is bool val && val,
                    _ => null
                },
                _ => null,
            };
        }
    }
}
