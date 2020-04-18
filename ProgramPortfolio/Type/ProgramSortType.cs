using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgramPortfolio
{
	/// <summary>
	/// Program 정렬 관련 Class
	/// </summary>
	public static class ProgramSort
	{
		/// <summary>
		/// Program 정렬 Type
		/// </summary>
		public enum ProgramSortType : uint
		{
			/// <summary>
			/// 시간순
			/// </summary>
			Time = 0,
			/// <summary>
			/// 언어순
			/// </summary>
			Language,
			/// <summary>
			/// 알파벳순
			/// </summary>
			Alphabet,
		}

		/// <summary>
		/// Language Name
		/// </summary>
		public static readonly Dictionary<ProgramSortType, string> SortName = new Dictionary<ProgramSortType, string>()
		{
			{ProgramSortType.Time, "시간순(역순)"},
			{ProgramSortType.Language, "언어별"},
			{ProgramSortType.Alphabet, "알파벳순"},
		};
	}
}
