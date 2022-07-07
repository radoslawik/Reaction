using Avalonia;
using Avalonia.Data;
using Avalonia.Interactivity;
using Reaction.Models;
using Reaction.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace Reaction
{
    public class React : AvaloniaObject
    {
        /// <summary>
        /// Command to invoke when the <seealso cref="ObserveOnProperty"/> property changes.
        public static readonly AttachedProperty<IEnumerable<Observation>> ObservationsProperty =
            AvaloniaProperty.RegisterAttached<React, Interactive, IEnumerable<Observation>>("Observations", Array.Empty<Observation>(), false, BindingMode.OneWay);

        /// <summary>
        /// Command to invoke when the <seealso cref="ObserveOnProperty"/> property changes.
        public static readonly AttachedProperty<ICommand?> CommandProperty =
            AvaloniaProperty.RegisterAttached<React, Interactive, ICommand?>("Command", null, false, BindingMode.OneWay);

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
            AvaloniaProperty.RegisterAttached<React, Interactive, string>("ObserveOn", "PropertyChanged", false, BindingMode.OneWay);

        static React()
        {
            ObserveOnProperty.Changed.Subscribe(x => HandleObserveOnChanged(x.Sender));
            ObservationsProperty.Changed.Subscribe(x => HandleObservationsChanged(x.Sender));
        }

        void DefaultHandler(object sender, EventArgs e)
        {
            if (sender is IAvaloniaObject obj)
            {
                foreach (var obs in GetMergedObservations(obj))
                {
                    var observables = SplitString(obs.ObserveOn);
                    if (e is AvaloniaPropertyChangedEventArgs arg)
                    {
                        if(observables.Contains(arg.Property.Name))
                        {
                            Execute(obs, sender, e);
                        }
                    }
                    else
                    {
                        var shouldExecute = observables.Select(s => obj.GetType().GetEvent(s)).OfType<EventInfo>()
                            .SelectMany(ei => ei.EventHandlerType is not null ? ei.EventHandlerType.GenericTypeArguments : Array.Empty<Type>())
                            .Any(x => x == e.GetType());

                        if(shouldExecute)
                        {
                            Execute(obs, sender, e);
                        }
                    }
                }
            }
        }

        private static void Execute(Observation obs, object sender, EventArgs e) => obs.Command.Execute(obs.CommandParameter ?? obs.CommandParameterProvider.GetDefaultCommandParameter(sender, e));

        private static void HandleObservationsChanged(IAvaloniaObject element)
        {
            var (type, methodInfo) = GetClassInfo(element);
            foreach (var obs in GetObservations(element))
            {
                AddObservationHandler(element, SplitString(obs.ObserveOn), type, methodInfo);
            }
        }

        private static void HandleObserveOnChanged(IAvaloniaObject element)
        {
            var (type, methodInfo) = GetClassInfo(element);
            AddObservationHandler(element, SplitString(GetObserveOn(element)), type, methodInfo);
        }

        private static Observation? GetSingleObservation(IAvaloniaObject element) => GetCommand(element) is ICommand command && GetObserveOn(element) is string observeOn ?
            new Observation(command, observeOn, GetCommandParameter(element), GetCommandParameterProvider(element)) : null;

        private static IEnumerable<Observation> GetMergedObservations(IAvaloniaObject element) => GetObservations(element).Concat(new[] { GetSingleObservation(element) }).OfType<Observation>();

        private static (Type, MethodInfo?) GetClassInfo(IAvaloniaObject element) => (element.GetType(),
            typeof(React).GetMethod(nameof(DefaultHandler), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

        private static void AddObservationHandler(IAvaloniaObject element, string[] observeItems, Type type, MethodInfo? methodInfo)
        {
            foreach (var item in observeItems)
            {
                var eventInfo = type.GetEvent(item, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) ??
                    type.GetEvent("PropertyChanged", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (eventInfo is not null && methodInfo is not null && eventInfo.EventHandlerType is Type eventType)
                {
                    var handleDelegate = Delegate.CreateDelegate(eventType, null, methodInfo);
                    eventInfo.AddEventHandler(element, handleDelegate);
                }
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

        public static void SetObservations(IAvaloniaObject element, IEnumerable<Observation> obs)
        {
            element.SetValue(ObservationsProperty, obs);
        }

        public static IEnumerable<Observation> GetObservations(IAvaloniaObject element)
        {
            return element.GetValue(ObservationsProperty);
        }
    }
}
