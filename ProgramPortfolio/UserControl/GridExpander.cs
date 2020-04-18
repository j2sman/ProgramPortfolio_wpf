using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace ProgramPortfolio
{
	/// 
	/// <summary>
	/// Expander 확장 class(자동 Size 줄임/늘림)
	/// </summary>
	/// 
	/// <remarks>
	/// remark
	/// </remarks>
	/// 
	/// <date>2012-04-20</date>
	/// <author>정대성</author>
	/// 
	public class GridExpander : Expander, IDisposable
	{
		/// <summary>
		/// 펼쳐졌을때 넓이 또는 높이값
		/// </summary>
		public GridLength ExpandedValue
		{
			get;
			set;
		}

		
		///
		/// <summary>
		/// 생성자
		/// </summary>
		/// 
		/// <remarks>
		/// remark
		/// </remarks>
		/// 
		/// <date>2012-04-20</date>
		/// <author>정대성</author>
		/// 
		/// <returns>
		/// Data type : void 
		/// </returns>
		/// 
		public GridExpander() : base()
		{
			ExpandedValue = GridLength.Auto;

			Expanded += new RoutedEventHandler(GridExpander_Expanded);
			Collapsed += new RoutedEventHandler(GridExpander_Collapsed);
			SizeChanged += new SizeChangedEventHandler(GridExpander_SizeChanged);
		}

		///
		/// <summary>
		/// 소멸자
		/// </summary>
		/// 
		/// <remarks>
		/// remark
		/// </remarks>
		/// 
		/// <date>2012-04-20</date>
		/// <author>정대성</author>
		/// 
		/// <returns>
		/// Data type : void 
		/// </returns>
		/// 
		~GridExpander()
		{
			Dispose();
		}

		
		///
		/// <summary>
		/// Dispose
		/// </summary>
		/// 
		/// <remarks>
		/// remark
		/// </remarks>
		/// 
		/// <date>2012-04-20</date>
		/// <author>정대성</author>
		/// 
		/// <returns>
		/// Data type : void  
		/// </returns>
		/// 
		public void Dispose()
		{
			Expanded -= GridExpander_Expanded;
			Collapsed -= GridExpander_Collapsed;
			SizeChanged -= GridExpander_SizeChanged;
		}

		
		///
		/// <summary>
		/// Expanded 처리
		/// </summary>
		/// 
		/// <remarks>
		/// remark
		/// </remarks>
		/// 
		/// <date>2012-04-20</date>
		/// <author>정대성</author>
		/// 
		/// <returns>
		/// Data type : void  
		/// </returns>
		/// 
		private void ProcessExpand()
		{
			Expander expander = this;

			Grid grid = expander.Parent as Grid;

			if (grid == null)
			{
				return;
			}

			// ExpandedValue설정
			if (ExpandedValue == GridLength.Auto)
			{
				if (IsExpanded)
				{
					if (ExpandDirection == ExpandDirection.Left || ExpandDirection == ExpandDirection.Right)
					{
						// Width
						int columnProperty = Convert.ToInt32(expander.GetValue(Grid.ColumnProperty));

						if (columnProperty < grid.ColumnDefinitions.Count)
						{
							ExpandedValue = grid.ColumnDefinitions[columnProperty].Width;
						}
					}
					else
					{
						// Height
						int rowProperty = Convert.ToInt32(expander.GetValue(Grid.RowProperty));

						if (rowProperty < grid.RowDefinitions.Count)
						{
							ExpandedValue = grid.RowDefinitions[rowProperty].Height;
						}
					}
				}

				if (ExpandedValue == GridLength.Auto)
				{
					ExpandedValue = new GridLength(1, GridUnitType.Star);
				}
			}

			// 넓이 또는 높이 설정
			if (ExpandDirection == ExpandDirection.Left || ExpandDirection == ExpandDirection.Right)
			{
				// Width
				int columnProperty = Convert.ToInt32(expander.GetValue(Grid.ColumnProperty));

				if (expander.IsExpanded)
				{
					grid.ColumnDefinitions[columnProperty].Width = ExpandedValue;
				}
				else
				{
					grid.ColumnDefinitions[columnProperty].Width = new GridLength(23, GridUnitType.Pixel);
				}
			}
			else
			{
				// Height
				int rowProperty = Convert.ToInt32(expander.GetValue(Grid.RowProperty));

				if (expander.IsExpanded)
				{
					grid.RowDefinitions[rowProperty].Height = ExpandedValue;
				}
				else
				{
					grid.RowDefinitions[rowProperty].Height = new GridLength(23, GridUnitType.Pixel);
				}
			}
		}

		
		///
		/// <summary>
		/// Expanded Event Handler
		/// </summary>
		/// 
		/// <remarks>
		/// remark
		/// </remarks>
		/// 
		/// <date>2012-04-20</date>
		/// <author>정대성</author>
		/// 
		/// <returns>
		/// Data type : void 
		/// </returns>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// 
		void GridExpander_Expanded(object sender, RoutedEventArgs e)
		{
			ProcessExpand();
		}

		
		///
		/// <summary>
		/// Collapsed Event Handler
		/// </summary>
		/// 
		/// <remarks>
		/// remark
		/// </remarks>
		/// 
		/// <date>2012-04-20</date>
		/// <author>정대성</author>
		/// 
		/// <returns>
		/// Data type : void 
		/// </returns>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// 
		void GridExpander_Collapsed(object sender, RoutedEventArgs e)
		{
			ProcessExpand();
		}

		
		///
		/// <summary>
		/// Size Changed Event Handler
		/// </summary>
		/// 
		/// <remarks>
		/// remark
		/// </remarks>
		/// 
		/// <date>2012-04-20</date>
		/// <author>정대성</author>
		/// 
		/// <returns>
		/// Data type : void 
		/// </returns>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// 
		void GridExpander_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			ProcessExpand();
		}
	}
}
