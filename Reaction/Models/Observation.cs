using Reaction.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Reaction.Models
{
    public class Observation
    {
        public Observation(ICommand command, string observeOn, object commandParameter)
        {
            Command = command;
            CommandParameter = commandParameter;
            CommandParameterProvider = new DefaultReactParamProvider();
            ObserveOn = observeOn;
        }

        public Observation(ICommand command, string observeOn, IReactParameterProvider commandParameterProvider)
        {
            Command = command;
            CommandParameterProvider = commandParameterProvider ?? new DefaultReactParamProvider();
            ObserveOn = observeOn;
        }

        public Observation(ICommand command, string observeOn, object? commandParameter = null, IReactParameterProvider? commandParameterProvider = null)
        {
            Command = command;
            CommandParameter = commandParameter;
            CommandParameterProvider = commandParameterProvider ?? new DefaultReactParamProvider();
            ObserveOn = observeOn;
        }

        public ICommand Command { get; init; }
        public object? CommandParameter { get; init; }
        public IReactParameterProvider CommandParameterProvider { get; init; }
        public string ObserveOn { get; init; }

    }
}
