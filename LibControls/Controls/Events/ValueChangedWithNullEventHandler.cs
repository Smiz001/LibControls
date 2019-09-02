using System.Windows;

namespace LibControls.Controls.Events
{
  public delegate void ValueChangedWithNullEventHandler(object sender, ValueChangedWithNullEventArgs e);
  public class ValueChangedWithNullEventArgs : RoutedEventArgs
  {
    public ValueChangedWithNullEventArgs(RoutedEvent routedEvent, object source, double? oldValue, double? newValue)
        : base(routedEvent, source)
    {
      OldValue = oldValue;
      NewValue = newValue;
    }
    public double? OldValue { get; private set; }
    public double? NewValue { get; private set; }
  }
}
