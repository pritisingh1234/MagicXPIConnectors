using System.Windows;
using System.Windows.Input;

namespace MagicSoftware.MQTT.Step
{
    /// <summary>
    /// Interaction logic for MQTTStepView.xaml
    /// </summary>
    partial class MQTTStepView : Window
    {
        private MQTTStepViewModel ViewModel { get { return (MQTTStepViewModel)DataContext; } }
        
        public MQTTStepView()
        {
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(Commands.PromptCommand, (sender, args) =>
            {
                ViewModel.Prompt(args.Parameter);
            }));

            CommandBindings.Add(new CommandBinding(Commands.EditExpressionCommand, (sender, args) =>
            {
                ViewModel.OpenExpressionEditor(args.Parameter);
            }));
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CheckDetails(this))
            {
                DialogResult = true;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
