using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace AndroidLogViewer.WPF
{
    [ContentProperty(nameof(Templates))]
    public class TypeBasedTemplateSelector : DataTemplateSelector
    {
        public IList Templates { get; set; } = new List<TypeBasedTemplate>();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;

            return Templates.OfType<TypeBasedTemplate>().FirstOrDefault(x => x.Type.IsInstanceOfType(item))?.Template;
        }


    }

    [ContentProperty(nameof(Template))]
    public class TypeBasedTemplate
    {
        public DataTemplate Template { get; set; }
        public System.Type Type { get; set; }
    }
}