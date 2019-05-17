using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace LibControls.Windows
{
  public class Window2 : Window
  {
    [DllImport("user32.dll")]
    static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
    [DllImport("user32.dll")]
    static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
    [DllImport("user32.dll")]
    static extern int TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

    #region Fields

    private Point startPos;
    private System.Windows.Forms.Screen[] screens = System.Windows.Forms.Screen.AllScreens;
    private Border border;
    private Border main;
    private Rectangle rectMax;
    private Canvas rectMin;

    #endregion

    static Window2()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(Window2),
          new FrameworkPropertyMetadata(typeof(Window2)));
    }

    public override void OnApplyTemplate()
    {
      this.LocationChanged += Window_LocationChanged;
      this.StateChanged += Window_StateChanged;

      Button maximizeButton = GetTemplateChild("maximizeButton") as Button;
      if (maximizeButton != null)
      {
        maximizeButton.Click += Maximize_Click;
        if (ResizeMode == ResizeMode.NoResize)
          maximizeButton.Visibility = Visibility.Hidden;
      }

      Button minimizeButton = GetTemplateChild("minimizeButton") as Button;
      if (minimizeButton != null)
      {
        minimizeButton.Click += Minimize_Click;
        if (ResizeMode == ResizeMode.NoResize)
          minimizeButton.Visibility = Visibility.Hidden;
      }

      Button closeButton = GetTemplateChild("closeButton") as Button;
      if (closeButton != null)
        closeButton.Click += Close_Click;

      Label label = GetTemplateChild("label") as Label;
      if(label!= null)
      {
        label.PreviewMouseDown += System_MouseDown;
        label.PreviewMouseMove += System_MouseMove;
      }

      border = GetTemplateChild("border") as Border;
      main = GetTemplateChild("main") as Border;
      rectMax = GetTemplateChild("rectMax") as Rectangle;
      rectMin = GetTemplateChild("rectMin") as Canvas;
    }

    private void Window_LocationChanged(object sender, EventArgs e)
    {
      int sum = 0;
      foreach (var item in screens)
      {
        sum += item.WorkingArea.Width;
        if (sum >= this.Left + this.Width / 2)
        {
          this.MaxHeight = item.WorkingArea.Height;
          break;
        }
      }
    }

    private void System_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left)
      {
        if (e.ClickCount >= 2)
        {
          this.WindowState = (this.WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }
        else
        {
          startPos = e.GetPosition(null);
        }
      }
      else if (e.ChangedButton == MouseButton.Right)
      {
        var pos = PointToScreen(e.GetPosition(this));
        IntPtr hWnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
        IntPtr hMenu = GetSystemMenu(hWnd, false);
        int cmd = TrackPopupMenu(hMenu, 0x100, (int)pos.X, (int)pos.Y, 0, hWnd, IntPtr.Zero);
        if (cmd > 0) SendMessage(hWnd, 0x112, (IntPtr)cmd, IntPtr.Zero);
      }
    }

    private void System_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        if (this.WindowState == WindowState.Maximized && Math.Abs(startPos.Y - e.GetPosition(null).Y) > 2)
        {
          var point = PointToScreen(e.GetPosition(null));

          this.WindowState = WindowState.Normal;

          this.Left = point.X - this.ActualWidth / 2;
          this.Top = point.Y - border.ActualHeight / 2;
        }
        DragMove();
      }
    }

    private void Maximize_Click(object sender, RoutedEventArgs e)
    {
      this.WindowState = (this.WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
      this.WindowState = WindowState.Minimized;
    }

    private void Window_StateChanged(object sender, EventArgs e)
    {
      if (this.WindowState == WindowState.Maximized)
      {
        main.BorderThickness = new Thickness(0);
        main.Margin = new Thickness(7, 7, 7, 0);
        rectMax.Visibility = Visibility.Hidden;
        rectMin.Visibility = Visibility.Visible;
      }
      else
      {
        main.BorderThickness = new Thickness(1);
        main.Margin = new Thickness(0);
        rectMax.Visibility = Visibility.Visible;
        rectMin.Visibility = Visibility.Hidden;
      }
    }
  }
}
