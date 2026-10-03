using System.Windows.Controls;

namespace LoLMatchup.Views.Controls;

/// <summary>A nested portrait owns activation; no Click event can reach the role card.</summary>
public sealed class EnemyPortraitButton : Button
{
    protected override void OnClick()
    {
        if (Command?.CanExecute(CommandParameter) == true) Command.Execute(CommandParameter);
    }
}
