using Avalonia;
using Avalonia.Data;
using Avalonia.Interactivity;
using Reaction.Providers;
using System;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace Reaction
{
    public class React : AvaloniaObject
    {
        /// <summary>
        /// Command to invoke when the <seealso cref="ObserveOnProperty"/> property changes.
        public static readonly AttachedProperty<ICommand?> CommandProperty =
            AvaloniaProperty.RegisterAttached<React, Interactive, ICommand?>("Command", null, false, BindingMode.OneTime);

        /// <summary>
        /// Command parameter.
        public static readonly AttachedProperty<object?> CommandParameterProperty =
            AvaloniaProperty.RegisterAttached<React, Interactive, object?>("CommandParameter", null, false, BindingMode.TwoWay);

        /// <summary>
        /// Command parameter.
        public static readonly AttachedProperty<IReactParameterProvider> CommandParameterProviderProperty =
            AvaloniaProperty.RegisterAttached<React, Interactive, IReactParameterProvider>("CommandParameterProvider", 
                new DefaultReactParamProvider(), false, BindingMode.TwoWay);

        /// <summary>
        /// AvaloniaProperty to observe.
        public static readonly AttachedProperty<string> ObserveOnProperty =
            AvaloniaProperty.RegisterAttached<React, Interactive, string>("ObserveOn", "PropertyChanged", false, BindingMode.OneTime);

        static React()
        {
            ObserveOnProperty.Changed.Subscribe(x => HandleCommandChanged(x.Sender));
        }

        private static void HandleCommandChanged(IAvaloniaObject element)
        {
            var type = element.GetType();
            var methodInfo = typeof(React).GetMethod("Handler", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var observeItems = SplitString(GetObserveOn(element));
            foreach(var obs in observeItems)
            {
                var eventInfo = type.GetEvent(obs, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) ??
                    type.GetEvent("PropertyChanged", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (eventInfo is not null && methodInfo is not null && eventInfo.EventHandlerType is Type eventType)
                {
                    var handleDelegate = Delegate.CreateDelegate(eventType, null, methodInfo);
                    eventInfo.AddEventHandler(element, handleDelegate);
                }
            }
        }

        void Handler(object sender, EventArgs e)
        {
            if (sender is IAvaloniaObject obj)
            {
                if (e is AvaloniaPropertyChangedEventArgs arg && !SplitString(GetObserveOn(obj)).Contains(arg.Property.Name))
                {
                    return;
                }
                var param = GetCommandParameter(obj) ?? GetCommandParameterProvider(obj).GetDefaultCommandParameter(sender, e);
                GetCommand(obj)?.Execute(param);
            }
        }

        private static string[] SplitString(string source, char delimiter = '|')
        {
            return source.Split(delimiter);
        }

        /// <summary>
        /// Accessor for Attached property <see cref="CommandProperty"/>.
        /// </summary>
        public static void SetCommand(IAvaloniaObject element, ICommand? commandValue)
        {
            element.SetValue(CommandProperty, commandValue);
        }

        /// <summary>
        /// Accessor for Attached property <see cref="CommandProperty"/>.
        /// </summary>
        public static ICommand? GetCommand(IAvaloniaObject element)
        {
            return element.GetValue(CommandProperty);
        }

        public static void SetCommandParameter(IAvaloniaObject element, object? p)
        {
            element.SetValue(CommandParameterProperty, p);
        }

        public static object? GetCommandParameter(IAvaloniaObject element)
        {
            return element.GetValue(CommandParameterProperty);
        }

        public static void SetObserveOn(IAvaloniaObject element, string propertyName)
        {
            element.SetValue(ObserveOnProperty, propertyName);
        }

        public static string GetObserveOn(IAvaloniaObject element)
        {
            return element.GetValue(ObserveOnProperty);
        }

        public static void SetCommandParameterProvider(IAvaloniaObject element, IReactParameterProvider provider)
        {
            element.SetValue(CommandParameterProviderProperty, provider);
        }

        public static IReactParameterProvider GetCommandParameterProvider(IAvaloniaObject element)
        {
            return element.GetValue(CommandParameterProviderProperty);
        }
    }
}
