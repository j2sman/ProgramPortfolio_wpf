using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ProgramPortfolio
{
	/// <summary>
	/// Implements IValueConverter to convert from double rating values to friendly string names.
	/// </summary>
	public class LanguageTypeConverter : IValueConverter
	{
		/// <summary>
		/// Convert
		/// </summary>
		/// <param name="value">Value</param>
		/// <param name="targetType">Target Type</param>
		/// <param name="parameter">Parameter</param>
		/// <param name="culture">Culture</param>
		/// <returns>변환된 값</returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ProgramLanguage.M_objLanguageName[(ProgramLanguage.LanguageType)value];
		}

		/// <summary>
		/// Convert Callback
		/// </summary>
		/// <param name="value">Value</param>
		/// <param name="targetType">Target Type</param>
		/// <param name="parameter">Parameter</param>
		/// <param name="culture">Culture</param>
		/// <returns>변환된 값</returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
