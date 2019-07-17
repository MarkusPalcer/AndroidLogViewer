namespace AndroidLogViewer.Filters
{
    /// <summary>
    /// Interaction logic for FilterEditorView.xaml
    /// </summary>
    public partial class FilterEditorView
    {
        public FilterEditorView(FilterEditorViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
