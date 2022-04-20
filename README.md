# Reaction :trophy:
Simple and easy-to-use interactivity framework for Avalonia

## Quick start :horse_racing:
Add `xmlns:act="clr-namespace:Reaction;assembly=Reaction"` to your view and use attached properties to bind view model properties

| Property  | Type | Description |
| ------------- |:---------------|:---------------|
| React.ObserveOn | string | Names of the events to observe. Multiple values should be delimited by `\|`. In case of targeting the `PropertyChanged` event it is possible to use the property names.
| React.Command | ICommand | Command to execute when one of the events specified in `React.ObserveOn` is raised |
| React.CommandParameter | object? | (optional) Parameter passed to `React.Command` upon its execution |
| React.CommandParameterProvider | IReactParameterProvider? | (optional) Provider that converts specific `EventArgs` to user defined types. It is used to retrieve the command parameter only when `React.CommandParameter` is null |

## Example :mountain_bicyclist:
Call `MyFunction` with specific argument when `PointerWheelChanged` or `KeyDown` event is raised. `MyReactionParameterProvider` converts the `EventArgs` to `double`, so the function will always receive valid parameter type.

MainWindow.xaml
```
<Window ...
    xmlns:act="clr-namespace:Reaction;assembly=Reaction"
    act:React.Command="{Binding MyCommand}"
    act:React.CommandParameterProvider="{Binding ParameterProvider}"
    act:React.ObserveOn="PointerWheelChanged|KeyDown">
  ...
</Window>
```
MainWindowViewModel.cs
```
...
public ICommand MyCommand => ReactiveCommand.Create<double>(MyFunction);
public static IReactParameterProvider ParameterProvider => new MyReactionParameterProvider();
...
```
MyReactionParameterProvider.cs
```
public class MyReactionParameterProvider : IReactParameterProvider
{
    public object? GetDefaultCommandParameter(object sender, EventArgs e)
    {
        return e switch
        {
            PointerWheelEventArgs pw => pw.Delta.Y,
            KeyEventArgs k => k.Key == Key.Up ? 1d : 0d,
            ...
            _ => null,
        };
    }
}
```

## That's it! :bicyclist:
Feel free to check how it looks in action by running the `Sample` project!
