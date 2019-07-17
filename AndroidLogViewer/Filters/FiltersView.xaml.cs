namespace AndroidLogViewer.Filters
{
    /// <summary>
    /// Interaction logic for FiltersView.xaml
    /// </summary>
    public partial class FiltersView
    {
        public FiltersView(FiltersViewModel viewModel, FilterEditorView newFilterView)
        {
            InitializeComponent();
         
            DataContext = viewModel;
            NewFilter.Content = newFilterView;
        }
    }
}
