using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MagicSoftware.Integration.UserComponents.Interfaces;

namespace MagicSoftware.MQTT.Trigger
{
    /// <summary>
    /// Interaction logic for MQTTTriggerView.xaml
    /// </summary>
    partial class MQTTTriggerView : Window
    {
        private MQTTTriggerViewModel ViewModel { get { return (MQTTTriggerViewModel)DataContext; } }
        
        public MQTTTriggerView()
        {
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(Commands.PromptCommand, (sender, args) =>
            {
                ViewModel.Prompt(args.Parameter);
            }));
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.ValidateLastTopic())
                DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddEmptyRow();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RemoveRow();
        }

    }
}
