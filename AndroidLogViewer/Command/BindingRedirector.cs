using System.Windows;

namespace AndroidLogViewer.Command
{
    public class BindingRedirector : DependencyObject
    {
        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(
            "Left", typeof(object), typeof(BindingRedirector), new PropertyMetadata(default(object), LeftChanged));

        private static void LeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BindingRedirector) d).Right = e.NewValue;
        }

        public object Left
        {
            get { return (object) GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        public static readonly DependencyProperty RightProperty = DependencyProperty.Register(
            "Right", typeof(object), typeof(BindingRedirector), new PropertyMetadata(default(object) , RightChanged));

        private static void RightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BindingRedirector) d).Left = e.NewValue;
        }

        public object Right
        {
            get { return (object) GetValue(RightProperty); }
            set { SetValue(RightProperty, value); }
        }

    }
}