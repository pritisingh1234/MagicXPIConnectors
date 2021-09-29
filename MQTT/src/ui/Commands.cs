using System.Windows.Input;

namespace MagicSoftware.MQTT
{
    static class Commands
    {
        public static readonly RoutedCommand PromptCommand = new RoutedCommand("Prompt", typeof(Commands));
        public static readonly RoutedCommand EditExpressionCommand = new RoutedCommand("EditExpression", typeof(Commands));
    }
}
