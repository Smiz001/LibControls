using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Shapes;

namespace LibControls.Windows
{
  public class Window1:Window
  {
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);
    private HwndSource _hwndSource;

    string title;
    //public Window1(string title) : base()
    //{
    //  PreviewMouseMove += OnPreviewMouseMove;
    //  WindowStartupLocation = WindowStartupLocation.CenterScreen;
    //  this.title = title;
    //}

    static Window1()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(Window1),
          new FrameworkPropertyMetadata(typeof(Window1)));
    }

    #region Click events

    protected void MinimizeClick(object sender, RoutedEventArgs e)
    {
      WindowState = WindowState.Minimized;
    }

    protected void RestoreClick(object sender, RoutedEventArgs e)
    {
      WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
    }

    protected void CloseClick(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void MouseLeftButtonDownOntopGrid(object sender, MouseButtonEventArgs e)
    {
      //// base.OnMouseLeftButtonDown(e);
      if (Mouse.LeftButton == MouseButtonState.Pressed)
        this.DragMove();
    }
    #endregion

    public override void OnApplyTemplate()
    {
      Button minimizeButton = GetTemplateChild("minimizeButton") as Button;
      if (minimizeButton != null)
      {
        minimizeButton.Click += MinimizeClick;
        if (ResizeMode == ResizeMode.NoResize)
          minimizeButton.Visibility = Visibility.Hidden;
      }

      Button restoreButton = GetTemplateChild("restoreButton") as Button;
      if (restoreButton != null)
      {
        restoreButton.Click += RestoreClick;
        if (ResizeMode == ResizeMode.NoResize)
          restoreButton.Visibility = Visibility.Hidden;
      }

      Button closeButton = GetTemplateChild("closeButton") as Button;
      if (closeButton != null)
        closeButton.Click += CloseClick;

      TextBlock tblTitle = GetTemplateChild("tblTitle") as TextBlock;
      if (tblTitle != null)
        tblTitle.Text += title;


      Grid gridBtn = GetTemplateChild("gridForBtn") as Grid;
      if (gridBtn != null)
        gridBtn.MouseLeftButtonDown += MouseLeftButtonDownOntopGrid;

      if (ResizeMode != ResizeMode.NoResize)
      {

        Grid resizeGrid = GetTemplateChild("resizeGrid") as Grid;
        if (resizeGrid != null)
        {
          foreach (UIElement element in resizeGrid.Children)
          {
            Rectangle resizeRectangle = element as Rectangle;
            if (resizeRectangle != null)
            {
              resizeRectangle.PreviewMouseDown += ResizeRectangle_PreviewMouseDown;
              resizeRectangle.MouseMove += ResizeRectangle_MouseMove;
            }
          }
        }
      }

      base.OnApplyTemplate();
    }

    protected void ResizeRectangle_MouseMove(Object sender, MouseEventArgs e)
    {
      Rectangle rectangle = sender as Rectangle;
      switch (rectangle.Name)
      {
        case "top":
          Cursor = Cursors.SizeNS;
          break;
        case "bottom":
          Cursor = Cursors.SizeNS;
          break;
        case "left":
          Cursor = Cursors.SizeWE;
          break;
        case "right":
          Cursor = Cursors.SizeWE;
          break;
        case "topLeft":
          Cursor = Cursors.SizeNWSE;
          break;
        case "topRight":
          Cursor = Cursors.SizeNESW;
          break;
        case "bottomLeft":
          Cursor = Cursors.SizeNESW;
          break;
        case "bottomRight":
          Cursor = Cursors.SizeNWSE;
          break;
        default:
          break;
      }
    }

    //private void moveRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    //{
    //  if (Mouse.LeftButton == MouseButtonState.Pressed)
    //    DragMove();
    //}

    protected void OnPreviewMouseMove(object sender, MouseEventArgs e)
    {
      if (Mouse.LeftButton != MouseButtonState.Pressed)
        Cursor = Cursors.Arrow;
    }


    protected void ResizeRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      Rectangle rectangle = sender as Rectangle;
      switch (rectangle.Name)
      {
        case "top":
          Cursor = Cursors.SizeNS;
          ResizeWindow(ResizeDirection.Top);
          break;
        case "bottom":
          Cursor = Cursors.SizeNS;
          ResizeWindow(ResizeDirection.Bottom);
          break;
        case "left":
          Cursor = Cursors.SizeWE;
          ResizeWindow(ResizeDirection.Left);
          break;
        case "right":
          Cursor = Cursors.SizeWE;
          ResizeWindow(ResizeDirection.Right);
          break;
        case "topLeft":
          Cursor = Cursors.SizeNWSE;
          ResizeWindow(ResizeDirection.TopLeft);
          break;
        case "topRight":
          Cursor = Cursors.SizeNESW;
          ResizeWindow(ResizeDirection.TopRight);
          break;
        case "bottomLeft":
          Cursor = Cursors.SizeNESW;
          ResizeWindow(ResizeDirection.BottomLeft);
          break;
        case "bottomRight":
          Cursor = Cursors.SizeNWSE;
          ResizeWindow(ResizeDirection.BottomRight);
          break;
        default:
          break;
      }
    }

    private void ResizeWindow(ResizeDirection direction)
    {
      SendMessage(_hwndSource.Handle, 0x112, (IntPtr)(61440 + direction), IntPtr.Zero);
    }

    private enum ResizeDirection
    {
      Left = 1,
      Right = 2,
      Top = 3,
      TopLeft = 4,
      TopRight = 5,
      Bottom = 6,
      BottomLeft = 7,
      BottomRight = 8,
    }

    protected override void OnInitialized(EventArgs e)
    {
      SourceInitialized += OnSourceInitialized;
      base.OnInitialized(e);
    }

    private void OnSourceInitialized(object sender, EventArgs e)
    {
      _hwndSource = (HwndSource)PresentationSource.FromVisual(this);
    }
  }
}
