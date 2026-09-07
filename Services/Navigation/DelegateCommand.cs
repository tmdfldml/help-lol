using System.Windows.Input;

namespace LoLMatchup.Services.Navigation;

public sealed class DelegateCommand<T>(Action<T> execute, Predicate<T>? canExecute = null) : ICommand
    where T : class
{
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) =>
        parameter is T value && (canExecute?.Invoke(value) ?? true);

    public void Execute(object? parameter)
    {
        if (parameter is T value && CanExecute(value))
        {
            execute(value);
        }
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
