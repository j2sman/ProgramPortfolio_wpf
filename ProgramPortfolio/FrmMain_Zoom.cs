using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ZoomAndPan;

namespace ProgramPortfolio
{
	public partial class FrmMain : Window, INotifyPropertyChanged
	{
		#region Zoom 관련 Property

		/// <summary>
		/// 실제 이동했는지 여부 확인을 위한 변수
		/// </summary>
		private Point M_objPrevMousePoint
		{
			get;
			set;
		}

		/// <summary>
		/// Zoom Timer
		/// </summary>
		private DispatcherTimer M_objZoomTimer
		{
			get;
			set;
		}

		/// <summary>
		/// Defines the current state of the mouse handling logic.
		/// </summary>
		private enum MouseHandlingMode : uint
		{
			/// <summary>
			/// Not in any special mode.
			/// </summary>
			None = 0,

			/// <summary>
			/// The user is left-dragging rectangles with the mouse.
			/// </summary>
			DraggingRectangles,

			/// <summary>
			/// The user is left-mouse-button-dragging to pan the viewport.
			/// </summary>
			Panning,

			/// <summary>
			/// The user is holding down shift and left-clicking or right-clicking to zoom in or out.
			/// </summary>
			Zooming,

			/// <summary>
			/// The user is holding down shift and left-mouse-button-dragging to select a region to zoom to.
			/// </summary>
			DragZooming,
		}

		/// <summary>
		/// Scale 지원여부
		/// </summary>
		private bool m_bSupportScale = false;
		/// <summary>
		/// m_bSupportScale Property
		/// </summary>
		public bool M_bSupportScale
		{
			get { return m_bSupportScale; }
			set
			{
				m_bSupportScale = value;
				// Property Event 발생시킴
				OnPropertyChanged("M_bSupportScale");
			}
		}

		/// <summary>
		/// Specifies the current state of the mouse handling logic.
		/// </summary>
		private MouseHandlingMode M_nMouseHandlingMode
		{
			get;
			set;
		}

		/// <summary>
		/// The point that was clicked relative to the ZoomAndPanControl.
		/// </summary>
		private Point M_objOrigZoomAndPanControlMouseDownPoint
		{
			get;
			set;
		}

		/// <summary>
		/// The point that was clicked relative to the content that is contained within the ZoomAndPanControl.
		/// </summary>
		private Point M_objOrigContentMouseDownPoint
		{
			get;
			set;
		}

		/// <summary>
		/// Records which mouse button clicked during mouse dragging.
		/// </summary>
		private MouseButton M_objMouseButtonDown
		{
			get;
			set;
		}

		/// <summary>
		/// Saves the previous zoom rectangle, pressing the backspace key jumps back to this zoom rectangle.
		/// </summary>
		private Rect M_objPrevZoomRect
		{
			get;
			set;
		}

		/// <summary>
		/// Save the previous content scale, pressing the backspace key jumps back to this scale.
		/// </summary>
		private double M_bPrevZoomScale
		{
			get;
			set;
		}
		#endregion // Zoom 관련 Property

		#region Zoom 관련 함수
		/// <summary>
		/// Zoom the viewport out, centering on the specified point (in content coordinates).
		/// </summary>
		/// <param name="pi_objControl">Zoom Control</param>
		/// <param name="pi_objContentZoomCenter">Center Point</param>
		private static void ZoomOut(
			ZoomAndPanControl pi_objControl,
			Point pi_objContentZoomCenter)
		{
			pi_objControl.ZoomAboutPoint(pi_objControl.ContentScale - 0.5, pi_objContentZoomCenter);
		}

		/// <summary>
		/// Zoom the viewport in, centering on the specified point (in content coordinates).
		/// </summary>
		/// <param name="pi_objControl">Zoom Control</param>
		/// <param name="pi_objContentZoomCenter">Center Point</param>
		private static void ZoomIn(
			ZoomAndPanControl pi_objControl,
			Point pi_objContentZoomCenter)
		{
			pi_objControl.ZoomAboutPoint(pi_objControl.ContentScale + 0.5, pi_objContentZoomCenter);
		}

		/// <summary>
		/// Initialise the rectangle that the use is dragging out.
		/// </summary>
		/// <param name="pi_objDragCanvas">Drag Canvas</param>
		/// <param name="pi_objPoint1">Left-Top</param>
		/// <param name="pi_objPoint2">Right-Bottom</param>
		private static void InitDragZoomRect(
			Canvas pi_objDragCanvas,
			Point pi_objPoint1,
			Point pi_objPoint2)
		{
			try
			{
				Border objDragBorder = pi_objDragCanvas.Children[0] as Border;
				SetDragZoomRect(objDragBorder, pi_objPoint1, pi_objPoint2);

				pi_objDragCanvas.Visibility = Visibility.Visible;
				objDragBorder.Opacity = 0.5;
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// Update the position and size of the rectangle that user is dragging out.
		/// </summary>
		/// <param name="pi_objDragBorder">Drag Border</param>
		/// <param name="pi_objPoint1">Left-Top</param>
		/// <param name="pi_objPoint2">Right-Bottom</param>
		private static void SetDragZoomRect(
			Border pi_objDragBorder,
			Point pi_objPoint1,
			Point pi_objPoint2)
		{
			double dX, dY, dWidth, dHeight;

			//
			// Deterine x,y,width and height of the rect inverting the points if necessary.
			// 

			if (pi_objPoint2.X < pi_objPoint1.X)
			{
				dX = pi_objPoint2.X;
				dWidth = pi_objPoint1.X - pi_objPoint2.X;
			}
			else
			{
				dX = pi_objPoint1.X;
				dWidth = pi_objPoint2.X - pi_objPoint1.X;
			}

			if (pi_objPoint2.Y < pi_objPoint1.Y)
			{
				dY = pi_objPoint2.Y;
				dHeight = pi_objPoint1.Y - pi_objPoint2.Y;
			}
			else
			{
				dY = pi_objPoint1.Y;
				dHeight = pi_objPoint2.Y - pi_objPoint1.Y;
			}

			//
			// Update the coordinates of the rectangle that is being dragged out by the user.
			// The we offset and rescale to convert from content coordinates.
			//
			Canvas.SetLeft(pi_objDragBorder, dX);
			Canvas.SetTop(pi_objDragBorder, dY);
			pi_objDragBorder.Width = dWidth;
			pi_objDragBorder.Height = dHeight;
		}

		/// <summary>
		/// When the user has finished dragging out the rectangle the zoom operation is applied.
		/// </summary>
		/// <param name="pi_objControl">Zoom Control</param>
		/// <param name="pi_objDragCanvas">Drag Canvas</param>
		private static void ApplyDragZoomRect(
			ZoomAndPanControl pi_objControl,
			Canvas pi_objDragCanvas)
		{
			try
			{
				//
				// Retreive the rectangle that the user draggged out and zoom in on it.
				//
				var objDragBorder = pi_objDragCanvas.Children[0] as Border;
				double dContentX = Canvas.GetLeft(objDragBorder);
				double dContentY = Canvas.GetTop(objDragBorder);
				double dContentWidth = objDragBorder.Width;
				double dContentHeight = objDragBorder.Height;

				// FadeOut실행시 Scroll이 잘 안되고 있음. 2014-04-23. JDS
				//ctrlZoom.AnimatedZoomTo(new Rect(dContentX, dContentY, dContentWidth, dContentHeight));
				//FadeOutDragZoomRect();

				pi_objControl.ZoomTo(new Rect(dContentX, dContentY, dContentWidth, dContentHeight));
				pi_objDragCanvas.Visibility = Visibility.Collapsed;
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// Fade out the drag zoom rectangle.
		/// </summary>
		/// <param name="pi_objDragCanvas">Drag Canvas</param>
		private void FadeOutDragZoomRect(
			Canvas pi_objDragCanvas)
		{
			try
			{
				AnimationHelper.StartAnimation(pi_objDragCanvas.Children[0] as Border, Border.OpacityProperty, 0.0, 0.1,
					delegate(object sender, EventArgs e)
					{
						pi_objDragCanvas.Visibility = Visibility.Collapsed;
					});
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// 전체 Zoom 초기화
		/// </summary>
		/// <param name="pi_bAnimate">Animate 여부</param>
		private void ScaleToFit(
			bool pi_bAnimate)
		{
			try
			{
				// 화면 Rendering뒤에 Zoom 계산 되어야 함
				ctrlScroll.UpdateLayout();
				ScaleToFit(ctrlZoom, pi_bAnimate);
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// Zoom 초기화
		/// </summary>
		/// <param name="pi_objControl">Zoom Control</param>
		/// <param name="pi_bAnimate">Animate 여부</param>
		private void ScaleToFit(
			ZoomAndPanControl pi_objControl,
			bool pi_bAnimate)
		{
			try
			{
				// 화면 Rendering뒤에 Zoom 계산 되어야 함
				pi_objControl.UpdateLayout();

				if (pi_bAnimate)
				{
					pi_objControl.AnimatedScaleToFit();
				}
				else
				{
					pi_objControl.ScaleToFit();
				}
			}
			catch (Exception)
			{
			}
		}

		#endregion // Zoom 관련 함수

		/// <summary>
		/// Event raised on mouse down in the ZoomAndPanControl.
		/// </summary>
		private void ctrlScroll_MouseDown(object sender, MouseButtonEventArgs e)
		{
			try
			{
				if (e.ClickCount > 2)
				{
					return;
				}

				var objScroll = sender as ScrollViewer;
				var objZoom = objScroll.Content as ZoomAndPanControl;

				if (M_bSupportScale)
				{
					if (e.ClickCount >= 2)
					{
						ScaleToFit(objZoom, true);

						e.Handled = true;
					}
					else
					{
						Grid objPanel = ctrlGridMain;

						objPanel.Focus();
						Keyboard.Focus(objPanel);

						M_objMouseButtonDown = e.ChangedButton;
						M_objOrigZoomAndPanControlMouseDownPoint = e.GetPosition(objZoom);
						M_objOrigContentMouseDownPoint = e.GetPosition(objPanel);

						if (Keyboard.IsKeyDown(Key.LeftShift) &&
							(e.ChangedButton == MouseButton.Left ||
							 e.ChangedButton == MouseButton.Right))
						{
							// Shift + left- or right-down initiates zooming mode.
							M_nMouseHandlingMode = MouseHandlingMode.Zooming;
						}
						else if (Keyboard.IsKeyDown(Key.LeftCtrl) && M_objMouseButtonDown == MouseButton.Left)
						{
							// Just a plain old left-down initiates panning mode.
							M_nMouseHandlingMode = MouseHandlingMode.Panning;
						}
						else if (M_objMouseButtonDown == MouseButton.Middle)
						{
							// Wheel Click
							// Just a plain old left-down initiates panning mode.
							M_nMouseHandlingMode = MouseHandlingMode.Panning;
						}

						if (M_nMouseHandlingMode != MouseHandlingMode.None)
						{
							// Capture the mouse so that we eventually receive the mouse up event.
							objZoom.CaptureMouse();
							e.Handled = true;
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// Event raised on mouse up in the ZoomAndPanControl.
		/// </summary>
		private void ctrlScroll_MouseUp(object sender, MouseButtonEventArgs e)
		{
			try
			{
				var objScroll = sender as ScrollViewer;
				var objZoom = objScroll.Content as ZoomAndPanControl;

				if (M_bSupportScale)
				{
					if (M_nMouseHandlingMode != MouseHandlingMode.None)
					{
						if (M_nMouseHandlingMode == MouseHandlingMode.Zooming)
						{
							if (M_objMouseButtonDown == MouseButton.Left)
							{
								// Shift + left-click zooms in on the content.
								ZoomIn(objZoom, M_objOrigContentMouseDownPoint);
							}
							else if (M_objMouseButtonDown == MouseButton.Right)
							{
								// Shift + left-click zooms out from the content.
								ZoomOut(objZoom, M_objOrigContentMouseDownPoint);
							}
						}
						else if (M_nMouseHandlingMode == MouseHandlingMode.DragZooming)
						{
							// When drag-zooming has finished we zoom in on the rectangle that was highlighted by the user.
							ApplyDragZoomRect(objZoom, ctrlCanvasDrag);
						}

						objZoom.ReleaseMouseCapture();
						M_nMouseHandlingMode = MouseHandlingMode.None;
						e.Handled = true;
					}
				}
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// Event raised on mouse move in the ZoomAndPanControl.
		/// </summary>
		private void ctrlScroll_MouseMove(object sender, MouseEventArgs e)
		{
			try
			{
				if (e.GetPosition(this) != M_objPrevMousePoint)
				{
					// 실제 이동하였음
					M_objPrevMousePoint = e.GetPosition(this);
				}

				var objScroll = sender as ScrollViewer;
				var objZoom = objScroll.Content as ZoomAndPanControl;

				if (M_bSupportScale)
				{
					Grid objPanel = ctrlGridMain;

					if (M_nMouseHandlingMode == MouseHandlingMode.Panning)
					{
						//
						// The user is left-dragging the mouse.
						// Pan the viewport by the appropriate amount.
						//
						Point objCurContentMousePoint = e.GetPosition(objPanel);
						Vector objDragOffset = objCurContentMousePoint - M_objOrigContentMouseDownPoint;

						objZoom.ContentOffsetX -= objDragOffset.X;
						objZoom.ContentOffsetY -= objDragOffset.Y;

						e.Handled = true;
					}
					else if (M_nMouseHandlingMode == MouseHandlingMode.Zooming)
					{
						Point objCurZoomAndPanControlMousePoint = e.GetPosition(objZoom);
						Vector objDragOffset = objCurZoomAndPanControlMousePoint - M_objOrigZoomAndPanControlMouseDownPoint;
						double dDragThreshold = 10;

						if (M_objMouseButtonDown == MouseButton.Left &&
							(Math.Abs(objDragOffset.X) > dDragThreshold ||
							 Math.Abs(objDragOffset.Y) > dDragThreshold))
						{
							//
							// When Shift + left-down zooming mode and the user drags beyond the drag threshold,
							// initiate drag zooming mode where the user can drag out a rectangle to select the area
							// to zoom in on.
							//
							M_nMouseHandlingMode = MouseHandlingMode.DragZooming;
							Point objCurContentMousePoint = e.GetPosition(objPanel);

							InitDragZoomRect(ctrlCanvasDrag, M_objOrigContentMouseDownPoint, objCurContentMousePoint);
						}

						e.Handled = true;
					}
					else if (M_nMouseHandlingMode == MouseHandlingMode.DragZooming)
					{
						//
						// When in drag zooming mode continously update the position of the rectangle
						// that the user is dragging out.
						//
						Point objCurContentMousePoint = e.GetPosition(objPanel);

						var objCanvas = ctrlCanvasDrag;
						SetDragZoomRect(objCanvas.Children[0] as Border, M_objOrigContentMouseDownPoint, objCurContentMousePoint);

						e.Handled = true;
					}
				}
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// Event raised by rotating the mouse wheel
		/// </summary>
		private void ctrlScroll_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			try
			{
				var objScroll = sender as ScrollViewer;
				var objZoom = objScroll.Content as ZoomAndPanControl;

				if (M_bSupportScale)
				{
					if (Keyboard.IsKeyDown(Key.LeftShift))
					{
						e.Handled = true;

						var objGrid = objZoom.Content as Grid;
						Grid objPanel = ctrlGridMain;

						if (e.Delta > 0)
						{
							Point curContentMousePoint = e.GetPosition(objGrid);
							ZoomIn(objZoom, curContentMousePoint);
						}
						else if (e.Delta < 0)
						{
							Point curContentMousePoint = e.GetPosition(objGrid);
							ZoomOut(objZoom, curContentMousePoint);
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// LoadTimer Tick Event Handler
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void M_objZoomTimer_Tick(object sender, EventArgs e)
		{
			try
			{
				M_objZoomTimer.Stop();
				ScaleToFit(true);
			}
			catch (Exception)
			{
			}
		}
	}
}
