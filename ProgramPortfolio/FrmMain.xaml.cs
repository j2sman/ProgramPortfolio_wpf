////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	FrmMain.xaml.cs
//
// summary:	Implements the form main.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ProgramPortfolio
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	MainWindow.xaml에 대한 상호 작용 논리. </summary>
	///
	/// <remarks>	JDS, 2020-02-27. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////

	public partial class FrmMain : Window, INotifyPropertyChanged
	{
		/// <summary>	속성 값이 변경될 때 발생합니다. </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Executes the property changed action. </summary>
		///
		/// <remarks>	JDS, 2020-02-27. </remarks>
		///
		/// <param name="name">	The name. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private void OnPropertyChanged(String name)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	Program List. </summary>
        ///
        /// <value>	A list of object programs. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private List<ProgramInfoClass> M_objProgramList
        {
            get;
            set;
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	현재 Image Index. </summary>
		///
		/// <value>	The m n current image index. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private int M_nCurImageIndex
		{
			get;
			set;
		}

		/// <summary>	돋보기 On/Off. </summary>
		private bool m_bMagnifierOn = false;

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets a value indicating whether the magnifier on. </summary>
		///
		/// <value>	True if magnifier on, false if not. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public bool M_bMagnifierOn
		{
			get
			{
				return m_bMagnifierOn;
			}
			set
			{
				try
				{
					m_bMagnifierOn = value;
					// 돋보기 상태 갱신
					RefreshMagnifier();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	생성자. </summary>
		///
		/// <remarks>	JDS, 2020-02-27. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public FrmMain()
		{
			InitializeComponent();

			M_bSupportScale = true;
			M_objProgramList = InterpretClass.InitializeProgramList();
			M_bMagnifierOn = false;
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	ctrlGrdProgramList Data 초기화. </summary>
		///
		/// <remarks>	JDS, 2020-02-27. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private void RefreshGrdProgramList()
		{
			if (ctrlCmbSort.SelectedIndex == -1 || ctrlCmbLanguage.SelectedIndex == -1)
			{
				return;
			}

			List<ProgramInfoClass> objCurList = null;
			ctrlGrdProgramList.BeginInit();

			try
			{
				objCurList = new List<ProgramInfoClass>();

				switch ((ProgramSort.ProgramSortType)ctrlCmbSort.SelectedIndex)
				{
					case ProgramSort.ProgramSortType.Time:
						// 최신의 순서로 표시
						objCurList.AddRange(M_objProgramList);
						objCurList.Reverse();
						break;
					case ProgramSort.ProgramSortType.Language:
						// 언어별로 추출
						objCurList.AddRange(M_objProgramList.Where(w => w.M_objLanguageType == (ProgramLanguage.LanguageType)ctrlCmbLanguage.SelectedIndex));
						break;
					case ProgramSort.ProgramSortType.Alphabet:
						// 알파벳 순서
						objCurList.AddRange(M_objProgramList);
						objCurList.Sort(new ProgramInfoSort());
						break;
					default:
						break;
				}

				if (objCurList.Count > 0)
				{
					ctrlGrdProgramList.Items.Clear();
					foreach (var objData in objCurList)
					{
						ctrlGrdProgramList.Items.Add(objData);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("RefreshGrdProgramList." + ex.Message);
			}
			finally
			{
				ctrlGrdProgramList.EndInit();
				objCurList = null;
			}

			RefreshShowProgram();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Program 표시 갱신. </summary>
		///
		/// <remarks>	JDS, 2020-02-27. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private void RefreshShowProgram()
		{
			RefreshDetailInfo();

			if (ctrlGrdProgramList.SelectedIndex != -1)
			{
				ProgramInfoClass objInfo = ctrlGrdProgramList.Items[ctrlGrdProgramList.SelectedIndex] as ProgramInfoClass;
				ctrlTblImageIndex.Text = string.Format("{0}/{1}", M_nCurImageIndex + 1, objInfo.M_objImageList.Count);

				ctrlBtnPrevImage.IsEnabled = M_nCurImageIndex > 0;
				ctrlBtnNextImage.IsEnabled = M_nCurImageIndex < (objInfo.M_objImageList.Count - 1);
			}
			else
			{
				ctrlTblImageIndex.Text = "0/0";

				ctrlBtnPrevImage.IsEnabled = false;
				ctrlBtnNextImage.IsEnabled = false;
			}

			ctrlBtnPrevImage.Background = ctrlBtnPrevImage.IsEnabled ? Brushes.LightGreen : Brushes.Transparent;
			ctrlBtnNextImage.Background = ctrlBtnNextImage.IsEnabled ? Brushes.LightGreen : Brushes.Transparent;

			ctrlBtnPrevImage.Visibility = ctrlBtnPrevImage.IsEnabled ? Visibility.Visible : Visibility.Hidden;
			ctrlBtnNextImage.Visibility = ctrlBtnNextImage.IsEnabled ? Visibility.Visible : Visibility.Hidden;

			RefreshImage();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Image 갱신. </summary>
		///
		/// <remarks>	JDS, 2020-02-27. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private void RefreshImage()
		{
			ctrlImageProgram.Source = null;

			if (ctrlGrdProgramList.SelectedIndex == -1)
			{
				return;
			}

			try
			{
				ProgramInfoClass objInfo = ctrlGrdProgramList.Items[ctrlGrdProgramList.SelectedIndex] as ProgramInfoClass;
				ctrlImageProgram.Source = objInfo.M_objImageList[M_nCurImageIndex];

				if (ctrlImageProgram.Source != null)
				{
					ScaleToFit(true);
				}
			}
			catch (Exception)
			{
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	상세정보 갱신. </summary>
		///
		/// <remarks>	JDS, 2020-02-27. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private void RefreshDetailInfo()
		{
			ctrlTbxDetail.Clear();

			if (ctrlGrdProgramList.SelectedIndex == -1)
			{
				return;
			}

			try
			{
				ProgramInfoClass objInfo = ctrlGrdProgramList.Items[ctrlGrdProgramList.SelectedIndex] as ProgramInfoClass;
				ctrlTbxDetail.AppendText(objInfo.M_strDetailInfo);
				objInfo = null;
			}
			catch (Exception)
			{
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	돋보기 갱신. </summary>
		///
		/// <remarks>	JDS, 2020-02-27. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public void RefreshMagnifier()
		{
			try
			{
				Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
				{
					// 돋보기 On/Off
					ctrlMagnifier.Visibility = M_bMagnifierOn ? Visibility.Visible : Visibility.Hidden;
				}));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Loaded Event Handler. </summary>
		///
		/// <remarks>	JDS, 2020-02-27. </remarks>
		///
		/// <param name="sender">	. </param>
		/// <param name="e">	 	. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// ComboBox 초기화
			ctrlCmbSort.BeginInit();
			ctrlCmbLanguage.BeginInit();

			try
			{
				foreach (ProgramSort.ProgramSortType sort in Enum.GetValues(typeof(ProgramSort.ProgramSortType)))
				{
					ctrlCmbSort.Items.Add(ProgramSort.SortName[sort]);
				}

				ctrlCmbSort.SelectedIndex = 0;

				foreach (ProgramLanguage.LanguageType language in Enum.GetValues(typeof(ProgramLanguage.LanguageType)))
				{
					ctrlCmbLanguage.Items.Add(ProgramLanguage.M_objLanguageName[language]);
				}

				ctrlCmbLanguage.SelectedIndex = 0;

				RefreshGrdProgramList();
			}
			catch (Exception)
			{
			}
			finally
			{
				ctrlCmbSort.EndInit();
				ctrlCmbLanguage.EndInit();
			}

			this.WindowState = System.Windows.WindowState.Maximized;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Closing Event Handler. </summary>
		///
		/// <remarks>	JDS, 2020-02-27. </remarks>
		///
		/// <param name="sender">	. </param>
		/// <param name="e">	 	. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	정렬 ComboBox Selection Changed Event Handler. </summary>
		///
		/// <remarks>	JDS, 2020-02-27. </remarks>
		///
		/// <param name="sender">	. </param>
		/// <param name="e">	 	. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private void ctrlCmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			M_nCurImageIndex = 0;
			// Language일 경우 활성화
			ctrlCmbLanguage.IsEnabled = ctrlCmbSort.SelectedIndex == 1;

			RefreshGrdProgramList();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	언어 ComboBox Selection Changed Event Handler. </summary>
		///
		/// <remarks>	JDS, 2020-02-27. </remarks>
		///
		/// <param name="sender">	. </param>
		/// <param name="e">	 	. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private void ctrlCmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			M_nCurImageIndex = 0;

			RefreshGrdProgramList();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Program List Grid Selection Changed Event Handler. </summary>
		///
		/// <remarks>	JDS, 2020-02-27. </remarks>
		///
		/// <param name="sender">	. </param>
		/// <param name="e">	 	. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private void ctrlGrdProgramList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			M_nCurImageIndex = 0;

			RefreshShowProgram();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Event handler. Called by ctrlBtnPrevImage for click events. </summary>
		///
		/// <remarks>	JDS, 2020-02-27. </remarks>
		///
		/// <param name="sender">	. </param>
		/// <param name="e">	 	Routed event information. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private void ctrlBtnPrevImage_Click(object sender, RoutedEventArgs e)
		{
			if (ctrlGrdProgramList.SelectedIndex == -1)
			{
				return;
			}

			if (M_nCurImageIndex > 0)
			{
				--M_nCurImageIndex;
				RefreshShowProgram();
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Event handler. Called by ctrlBtnNextImage for click events. </summary>
		///
		/// <remarks>	JDS, 2020-02-27. </remarks>
		///
		/// <param name="sender">	. </param>
		/// <param name="e">	 	Routed event information. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private void ctrlBtnNextImage_Click(object sender, RoutedEventArgs e)
		{
			if (ctrlGrdProgramList.SelectedIndex == -1)
			{
				return;
			}

			ProgramInfoClass objInfo = ctrlGrdProgramList.Items[ctrlGrdProgramList.SelectedIndex] as ProgramInfoClass;

			if (M_nCurImageIndex < (objInfo.M_objImageList.Count - 1))
			{
				++M_nCurImageIndex;
				RefreshShowProgram();
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Event handler. Called by ctrlExpDetail for expanded events. </summary>
		///
		/// <remarks>	JDS, 2020-02-27. </remarks>
		///
		/// <param name="sender">	. </param>
		/// <param name="e">	 	Routed event information. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private void ctrlExpDetail_Expanded(object sender, RoutedEventArgs e)
		{
			try
			{
				ScaleToFit(true);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Event handler. Called by ctrlExpDetail for collapsed events. </summary>
		///
		/// <remarks>	JDS, 2020-02-27. </remarks>
		///
		/// <param name="sender">	. </param>
		/// <param name="e">	 	Routed event information. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private void ctrlExpDetail_Collapsed(object sender, RoutedEventArgs e)
		{
			try
			{
				ScaleToFit(true);
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Event handler. Called by Window for preview mouse down events. </summary>
		///
		/// <remarks>	JDS, 2020-02-27. </remarks>
		///
		/// <param name="sender">	. </param>
		/// <param name="e">	 	Mouse button event information. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			try
			{
				M_bMagnifierOn = e.RightButton == MouseButtonState.Pressed;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Event handler. Called by Window for preview mouse up events. </summary>
		///
		/// <remarks>	JDS, 2020-02-27. </remarks>
		///
		/// <param name="sender">	. </param>
		/// <param name="e">	 	Mouse button event information. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private void Window_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			try
			{
				M_bMagnifierOn = e.RightButton == MouseButtonState.Pressed;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}
