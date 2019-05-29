using System.Windows;

namespace AndroidLogViewer.Style
{
    public class Grouping
    {
        public enum GroupingPosition
        {
            /// <summary>
            /// Denotes that the element is not part of any grouping
            /// </summary>
            Single,

            /// <summary>
            /// Denotes that the element is the first inside its group
            /// </summary>
            First,

            /// <summary>
            /// Denotes that the element has predecessors and successors in its group
            /// </summary>
            Middle,

            /// <summary>
            /// Denotes that the element is the last inside its group
            /// </summary>
            Last
        }

        public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached(
            "Position", typeof(GroupingPosition), typeof(Grouping), new PropertyMetadata(default(GroupingPosition)));

        public static void SetPosition(DependencyObject element, GroupingPosition value)
        {
            element.SetValue(PositionProperty, value);
        }

        public static GroupingPosition GetPosition(DependencyObject element)
        {
            return (GroupingPosition) element.GetValue(PositionProperty);
        }
    }
}