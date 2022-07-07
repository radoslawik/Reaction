using Reaction.Models;
using Reaction.Providers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Sample.Helpers;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Sample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel() => this.WhenAnyValue(x => x.CurrentLength).Subscribe(l => this.RaisePropertyChanged(nameof(ReactiveGreeting)));
        private void UpdateGreeting(double length) => CurrentLength = Math.Min(Greeting.Length, Math.Max(0, length > 0 ? CurrentLength + 1 : CurrentLength - 1));
        [Reactive]
        public int CurrentLength { get; set; } = AvaloniaGreeting.Length;
        public static string AvaloniaGreeting => "Welcome to Avalonia!";
        public static string Greeting => $"{AvaloniaGreeting} Welcome to Reaction world!";
        public string ReactiveGreeting => Greeting[..CurrentLength];
        public static string HelpMessage => "Try scrolling or pressing arrows";
        [Reactive]
        public bool IsHelpVisible { get; set; } = true;
        #region Single reaction
        public ICommand UpdateGreetingCommand => ReactiveCommand.Create<double>(UpdateGreeting);
        public static IReactParameterProvider PositionParameterProvider => new PositionParameterProvider();
        public static string GreetingTriggers => "PointerWheelChanged|KeyDown";
        #endregion
        #region Multiple reactions
        public IReadOnlyCollection<Observation> Observations => new[] { new Observation(ReactiveCommand.Create<bool>(vis => IsHelpVisible = vis), "IsPointerOver", new VisibilityParameterProvider()) };
        #endregion
    }
}
