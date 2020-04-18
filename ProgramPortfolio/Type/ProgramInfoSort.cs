using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgramPortfolio
{
	/// <summary>
	/// EventCodeStruct 정렬
	/// </summary>
	public class ProgramInfoSort : IComparer<ProgramInfoClass>
	{
		/// <summary>
		/// 비교함수
		/// </summary>
		/// <param name="o1">첫번째</param>
		/// <param name="o2">두번째</param>
		/// <returns>정순서</returns>
		public int Compare(ProgramInfoClass o1, ProgramInfoClass o2)
		{
			return o1.M_strProjectName.CompareTo(o2.M_strProjectName);
		}
	}
}
