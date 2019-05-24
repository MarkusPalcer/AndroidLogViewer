using System.Windows;
using System.Windows.Input;

namespace AndroidLogViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            LogItems.SelectionChanged += (sender, args) => LogItems.ScrollIntoView(LogItems.SelectedItem);
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ExecuteCopyLogItems(object sender, ExecutedRoutedEventArgs e)
        {
            (DataContext as MainWindowViewModel)?.CopySelectionCommand?.Execute(LogItems.SelectedItems);
        }
    }
}
