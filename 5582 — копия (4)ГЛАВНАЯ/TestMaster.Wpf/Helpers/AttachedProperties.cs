using System.Windows;

namespace TestMaster.Wpf.Helpers;

public static class AttachedProperties
{
    public static readonly DependencyProperty HasItemsProperty =
        DependencyProperty.RegisterAttached("HasItems", typeof(bool), typeof(AttachedProperties), new PropertyMetadata(false));

    public static void SetHasItems(UIElement element, bool value)
    {
        element.SetValue(HasItemsProperty, value);
    }

    public static bool GetHasItems(UIElement element)
    {
        return (bool)element.GetValue(HasItemsProperty);
    }
}