using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ProgramPortfolio
{
	/// <summary>
	/// 프로그램 정보 Class
	/// </summary>
	public class ProgramInfoClass : ICloneable
	{
		/// <summary>
		/// 프로그램 명칭
		/// </summary>
		public string M_strName
		{
			get;
			set;
		}

		/// <summary>
		/// 프로젝트 명칭
		/// </summary>
		public string M_strProjectName
		{
			get;
			set;
		}

		/// <summary>
		/// Program Language Type
		/// </summary>
		public ProgramLanguage.LanguageType M_objLanguageType
		{
			get;
			set;
		}

		/// <summary>
		/// 상세정보
		/// </summary>
		public string M_strDetailInfo
		{
			get;
			set;
		}

		/// <summary>
		/// Image List
		/// </summary>
		public List<BitmapImage> M_objImageList
		{
			get;
			set;
		}

		/// <summary>
		/// Clone
		/// </summary>
		/// <returns>복사한 객체</returns>
		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
