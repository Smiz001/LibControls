using LibControls.Controls.Events;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace LibControls.Controls
{
  [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
  [TemplatePart(Name = "PART_ButtonUp", Type = typeof(ButtonBase))]
  [TemplatePart(Name = "PART_ButtonDown", Type = typeof(ButtonBase))]
  public class DoubleUpDownWithNull : Control
  {

    private TextBox PART_TextBox = new TextBox();

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      TextBox textBox = GetTemplateChild("PART_TextBox") as TextBox;
      if (textBox != null)
      {
        PART_TextBox = textBox;
        PART_TextBox.PreviewKeyDown += textBox_PreviewKeyDown;
        PART_TextBox.TextChanged += textBox_TextChanged;
        if (Value == null)
          PART_TextBox.Text = string.Empty;
        else
          PART_TextBox.Text = Value.ToString();
        PART_TextBox.MouseWheel += textBox_MouseWheel;
        PART_TextBox.LostFocus += textBox_LostFocus;
      }
      ButtonBase PART_ButtonUp = GetTemplateChild("PART_ButtonUp") as ButtonBase;
      if (PART_ButtonUp != null)
      {
        PART_ButtonUp.Click += buttonUp_Click;
      }
      ButtonBase PART_ButtonDown = GetTemplateChild("PART_ButtonDown") as ButtonBase;
      if (PART_ButtonDown != null)
      {
        PART_ButtonDown.Click += buttonDown_Click;
      }
    }

    static DoubleUpDownWithNull()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DoubleUpDownWithNull), new FrameworkPropertyMetadata(typeof(DoubleUpDownWithNull)));
    }

    public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
        "ValueChanged", RoutingStrategy.Direct,
        typeof(ValueChangedEventHandler), typeof(DoubleUpDownWithNull));
    public event ValueChangedEventHandler ValueChanged
    {
      add
      {
        base.AddHandler(DoubleUpDownWithNull.ValueChangedEvent, value);
      }
      remove
      {
        base.RemoveHandler(DoubleUpDownWithNull.ValueChangedEvent, value);
      }
    }

    public double MaxValue
    {
      get { return (double)GetValue(MaxValueProperty); }
      set { SetValue(MaxValueProperty, value); }
    }

    public static readonly DependencyProperty MaxValueProperty =
        DependencyProperty.Register("MaxValue", typeof(double), typeof(DoubleUpDownWithNull), new FrameworkPropertyMetadata(100D, maxValueChangedCallback, coerceMaxValueCallback));
    private static object coerceMaxValueCallback(DependencyObject d, object value)
    {
      double minValue = ((DoubleUpDownWithNull)d).MinValue;
      if ((double)value < minValue)
        return minValue;

      return value;
    }
    private static void maxValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      DoubleUpDownWithNull numericUpDown = ((DoubleUpDownWithNull)d);
      numericUpDown.CoerceValue(MinValueProperty);
      numericUpDown.CoerceValue(ValueProperty);
    }

    public double MinValue
    {
      get { return (double)GetValue(MinValueProperty); }
      set { SetValue(MinValueProperty, value); }
    }
    public static readonly DependencyProperty MinValueProperty =
        DependencyProperty.Register("MinValue", typeof(double), typeof(DoubleUpDownWithNull), new FrameworkPropertyMetadata(0D, minValueChangedCallback, coerceMinValueCallback));
    private static object coerceMinValueCallback(DependencyObject d, object value)
    {
      double maxValue = ((DoubleUpDownWithNull)d).MaxValue;
      if ((double)value > maxValue)
        return maxValue;

      return value;
    }
    private static void minValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      DoubleUpDownWithNull numericUpDown = ((DoubleUpDownWithNull)d);
      numericUpDown.CoerceValue(DoubleUpDownWithNull.MaxValueProperty);
      numericUpDown.CoerceValue(DoubleUpDownWithNull.ValueProperty);
    }

    public double Increment
    {
      get { return (double)GetValue(IncrementProperty); }
      set { SetValue(IncrementProperty, value); }
    }
    public static readonly DependencyProperty IncrementProperty =
        DependencyProperty.Register("Increment", typeof(double), typeof(DoubleUpDownWithNull), new FrameworkPropertyMetadata(1D, null, coerceIncrementCallback));
    private static object coerceIncrementCallback(DependencyObject d, object value)
    {
      DoubleUpDownWithNull numericUpDown = ((DoubleUpDownWithNull)d);
      double i = numericUpDown.MaxValue - numericUpDown.MinValue;
      if ((double)value > i)
        return i;

      return value;
    }

    public double? Value
    {
      get { return (double?)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(double?), typeof(DoubleUpDownWithNull), new FrameworkPropertyMetadata(0D, valueChangedCallback, coerceValueCallback), validateValueCallback);
    private static void valueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      DoubleUpDownWithNull numericUpDown = (DoubleUpDownWithNull)d;
      ValueChangedWithNullEventArgs ea =
          new ValueChangedWithNullEventArgs(DoubleUpDownWithNull.ValueChangedEvent, d, (double?)e.OldValue, (double?)e.NewValue);
      numericUpDown.RaiseEvent(ea);
      //if (ea.Handled) numericUpDown.Value = (double)e.OldValue;
      //else 
      numericUpDown.PART_TextBox.Text = e.NewValue == null ? string.Empty : e.NewValue.ToString();
    }
    private static bool validateValueCallback(object value)
    {
      if (value == null) return true;
      double val = (double)value;
      if (val > double.MinValue && val < double.MaxValue)
        return true;
      else
        return false;
    }
    private static object coerceValueCallback(DependencyObject d, object value)
    {
      double? val = (double?)value;
      double minValue = ((DoubleUpDownWithNull)d).MinValue;
      double maxValue = ((DoubleUpDownWithNull)d).MaxValue;
      double? result;
      if (val < minValue)
        result = minValue;
      else if (val > maxValue)
        result = maxValue;
      else
        result = (double?)value;

      return result;
    }

    private void buttonUp_Click(object sender, RoutedEventArgs e)
    {
      if (Value == null)
        Value = Increment;
      else
        Value += Increment;
    }
    private void buttonDown_Click(object sender, RoutedEventArgs e)
    {
      if (Value == null)
        Value = 0 - Increment;
      else
        Value -= Increment;
    }

    private void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Space)
        e.Handled = true;
    }
    private void textBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      int index = PART_TextBox.CaretIndex;
      double result;
      if (!double.TryParse(PART_TextBox.Text.Replace('.', ','), out result))
      {
        if (PART_TextBox.Text.Length == 1 && PART_TextBox.Text == "-")
          return;
        var changes = e.Changes.FirstOrDefault();
        PART_TextBox.Text = PART_TextBox.Text.Remove(changes.Offset, changes.AddedLength);
        PART_TextBox.CaretIndex = index > 0 ? index - changes.AddedLength : 0;
      }
      else if (result <= MaxValue && result >= MinValue)
      {
        Value = result;
        PART_TextBox.CaretIndex = index;
      }
      else
      {
        PART_TextBox.Text = Value.ToString();
        PART_TextBox.CaretIndex = index > 0 ? index - 1 : 0;
      }
      if (string.IsNullOrEmpty(PART_TextBox.Text))
      {
        //Value = 0;
        //PART_TextBox.CaretIndex = 1;
      }
    }

    private void textBox_MouseWheel(object sender, MouseWheelEventArgs e)
    {
      if (e.Delta > 0)
      {
        if (Value == null)
          Value = Increment;
        else
        {
          if (Value + Increment <= MaxValue)
            Value += Increment;
        }
      }
      else
      {
        if (Value == null)
          Value = 0 - Increment;
        else
        {
          if (Value - Increment >= MinValue)
            Value -= Increment;
        }
      }
      PART_TextBox.CaretIndex = PART_TextBox.Text.Length;
    }
    private void textBox_LostFocus(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrEmpty(PART_TextBox.Text))
      {
        Value = null;
      }
    }
  }
}
