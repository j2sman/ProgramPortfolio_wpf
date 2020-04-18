using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Resources;
using System.Xml;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace ProgramPortfolio
{
	/// <summary>
	/// XML 해석
	/// </summary>
	public static class InterpretClass
	{
		/// <summary>
		/// Program List XML Document 얻는 함수
		/// </summary>
		/// <returns></returns>
		private static XDocument GetProgramList()
		{
			XDocument objResult = null;
			try
			{
				StreamResourceInfo info = Application.GetResourceStream(new Uri("/ProgramPortfolio;component/ProgramList.xml", UriKind.Relative));
				XmlReader reader = XmlReader.Create(info.Stream);
				objResult = XDocument.Load(reader);
				reader.Close();
			}
			catch (Exception)
			{
			}
			finally
			{
			}

			return objResult;
		}

		/// <summary>
		/// 상세정보 가져오기
		/// </summary>
		/// <param name="pi_strLanguageType">언어 Type</param>
		/// <param name="pi_strLink">DetailInfoLink</param>
		/// <returns>상세정보</returns>
		private static string GetDetailInfo(
			string pi_strLanguageType,
			string pi_strLink)
		{
			string strResult = string.Empty;

			try
			{
				StreamResourceInfo info = Application.GetResourceStream(
					new Uri(string.Format("pack://application:,,,/ProgramPortfolio;component/Resources/{0}/DetailInfo/{1}",
						pi_strLanguageType, pi_strLink), UriKind.Absolute));
				byte[] objResult = new byte[info.Stream.Length];
				info.Stream.Read(objResult, 0, objResult.Length);
				strResult = Encoding.Default.GetString(objResult);

				objResult = null;
				info = null;
			}
			catch (Exception)
			{
			}
			finally
			{
			}

			return strResult;
		}

		/// <summary>
		/// Image Uri 얻는 함수
		/// </summary>
		/// <param name="pi_strLanguageType">언어 Type</param>
		/// <param name="pi_strImageName">Image Name</param>
		/// <returns>Uri</returns>
		private static Uri GetBitmapUri(
			string pi_strLanguageType,
			string pi_strImageName)
		{
			if (pi_strImageName == null || pi_strImageName.Length == 0)
			{
				return null;
			}

			string pi_strImagePath = string.Format("pack://application:,,,/ProgramPortfolio;component/Resources/{0}/Images/{1}",
				pi_strLanguageType,
				pi_strImageName);

			return new Uri(pi_strImagePath, UriKind.Absolute);
		}

		///
		/// <summary>
		/// Bitmap 얻는 함수
		/// </summary>
		/// 
		/// <remarks>
		/// remark
		/// </remarks>
		/// 
		/// <date>2012-01-03</date>
		/// <author>정대성</author>
		/// 
		/// <returns>
		/// Data type : static BitmapImage , BitmapImage 객체
		/// </returns>
		/// <param name="pi_strLanguageType">언어 Type</param>
		/// <param name="pi_strImageName">Image Name</param>
		/// 
		public static BitmapImage GetBitmap(
			string pi_strLanguageType,
			string pi_strImageName)
		{
			if (pi_strImageName == null || pi_strImageName == string.Empty)
			{
				return null;
			}
			else
			{
				BitmapImage bitmap = new BitmapImage(GetBitmapUri(pi_strLanguageType, pi_strImageName));

				if (bitmap.CanFreeze)
				{
					bitmap.Freeze();
				}

				return bitmap;
			}
		}

		/// <summary>
		/// 이미지 가져오기
		/// </summary>
		/// <param name="pi_objProgramElement">Program Element</param>
		/// <returns>상세정보</returns>
		private static List<BitmapImage> GetImageList(
			XElement pi_objProgramElement)
		{
			List<BitmapImage> objResult = new List<BitmapImage>();

			try
			{
				string strLanguage = pi_objProgramElement.Attribute("Language").Value;

				foreach (XElement objImageElement in pi_objProgramElement.Elements("ImageLink"))
				{
					objResult.Add(GetBitmap(strLanguage, objImageElement.Attribute("ImageName").Value));
				}
			}
			catch (Exception)
			{
			}
			finally
			{
			}

			return objResult;
		}
		
		/// <summary>
        /// Program List 얻는 함수
        /// </summary>
        /// <returns>Program List</returns>
        public static List<ProgramInfoClass> InitializeProgramList()
        {
            List<ProgramInfoClass> objList = new List<ProgramInfoClass>();
			XDocument objDocument = GetProgramList();

			try
			{
				foreach (XElement objProgramElement in objDocument.Element("ProgramList").Elements("Program"))
				{
					ProgramInfoClass objCurInfo = new ProgramInfoClass();
					objCurInfo.M_strName = objProgramElement.Attribute("Name").Value;
					objCurInfo.M_strProjectName = objProgramElement.Attribute("ProjectName").Value;
					objCurInfo.M_objLanguageType = ProgramLanguage.M_objLanguagePath.Where(w => w.Value == objProgramElement.Attribute("Language").Value).First().Key;
					objCurInfo.M_strDetailInfo = GetDetailInfo(objProgramElement.Attribute("Language").Value, objProgramElement.Attribute("DetailInfoLink").Value);
					objCurInfo.M_objImageList = GetImageList(objProgramElement);

					objList.Add(objCurInfo);
				}
			}
			catch (Exception)
			{
			}

            return objList;
        }
	}
}
