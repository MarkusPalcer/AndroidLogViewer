using System.Windows;
using PantherDI.ContainerCreation;

namespace AndroidLogViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var container = new ContainerBuilder()
                .WithSupportForUnregisteredTypes()
                .WithAssembly(GetType().Assembly)
                .WithGenericResolvers()
                .Build();

            MainWindow = container.Resolve<MainWindow>();
            MainWindow.Show();
        }
    }
}
