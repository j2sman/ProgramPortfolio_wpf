using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgramPortfolio
{
	/// <summary>
	/// ProgramLanguage 관련 Class
	/// </summary>
	public static class ProgramLanguage
	{
		/// <summary>
		/// Language Type
		/// </summary>
		public enum LanguageType : uint
		{
			/// <summary>	C# </summary>
			CSharp = 0,
			/// <summary>	C++ </summary>
			CPlusPlus,
		}

		/// <summary>
		/// Language Path(Resource 경로 찾을시 쓰임)
		/// </summary>
		public static readonly Dictionary<LanguageType, string> M_objLanguagePath = new Dictionary<LanguageType, string>()
		{
			{LanguageType.CSharp, "CSharp"},
			{LanguageType.CPlusPlus, "CPlusPlus"},
		};

		/// <summary>
		/// Language Name(화면에 표시될 이름)
		/// </summary>
		public static readonly Dictionary<LanguageType, string> M_objLanguageName = new Dictionary<LanguageType, string>()
		{
			{LanguageType.CSharp, "C#"},
			{LanguageType.CPlusPlus, "C++"},
		};
	}
}
