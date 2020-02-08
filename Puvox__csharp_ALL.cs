using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Media;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Win32;
/*
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NinjaTrader.Cbi;
using NinjaTrader.Code;
using NinjaTrader.Core;
using NinjaTrader.Data;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.NinjaScript;
using NinjaTrader.Gui.Tools;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.DrawingTools;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.Strategies;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
*/

#region Program properties
namespace PuvoxLibrary
{ 
	public class Program
	{
		protected string baseCompanyName;
		protected string baseDomain;
		protected string Name;
		protected string Slug;
		protected double Version;
		protected string Language;
		protected string BaseProductUrl;
		protected string RegRootOfThisProg;
		protected string responseUrl;
		protected string baseResponsePath;
		protected string baseContactPath;
		protected string contactUrl;
		protected bool initialized;
		
		public void defaultInit(string companyName_, string domain_, string title, string slug, double version, string language, string productUrl)
		{
			baseCompanyName = companyName_;
			baseDomain = domain_;
			Name = title;
			Slug = slug;
			Version = version;
			Language = language;
			BaseProductUrl = productUrl;
			RegRootOfThisProg = string.Concat(new string[]{"SOFTWARE\\",baseCompanyName,"\\",	Slug,"\\"});
			responseUrl = ApiURL + "&action=check&licensekey=" + licensekeyGet();
			contactUrl = baseDomain + baseContactPath;
			initialized = true;
		}
 
		public string ApiURL
		{
			get
			{
				return string.Concat(new object[]{ baseDomain,	baseResponsePath, "program=", PuvoxLibrary.Methods.urlEncode(Slug), "&version=", Version	});
			}
		}
			
			
		
		public bool licensekeySet(string key)
		{
			PuvoxLibrary.Methods.setRegistryValue("licensekey", key);
			return true;
		}

		
		public string licensekeyGet()
		{
			return PuvoxLibrary.Methods.getSetRegistryValue("licensekey", "demo_" + PuvoxLibrary.Methods.RandomString(32));
		}

		public bool isDemo { get { return licensekeyGet().Contains("demo_"); } }
		
		public Dictionary<string,string> mainResponse;
		public Dictionary<string,string> MainResponse
		{
			get
			{
				getResponseData(false, 3);
				return mainResponse;
			}
			set
			{
				mainResponse = value;
			}
		}


		public bool getResponseData(bool forceNew, int cachedMinutes_)
		{
			bool flag = false;
			if (mainResponse == null)
			{
				flag = true;
			}
			else if (forceNew)
			{
				flag = true;
			}
			if (flag)
			{
				string text;
				if (DateTime.Now.Subtract(TimeSpan.FromMinutes((double)cachedMinutes_)) < DateTime.Parse( PuvoxLibrary.Methods.getSetRegistryValue("last_resp_time", DateTime.Now.ToString())))
				{
					text = PuvoxLibrary.Methods.getRegistryValue("last_resp_data");
				}
				else
				{
					text = PuvoxLibrary.Methods.urlRead(responseUrl);
					PuvoxLibrary.Methods.setRegistryValue("last_resp_time", DateTime.Now.ToString());
					PuvoxLibrary.Methods.setRegistryValue("last_resp_data", text);
				}
				if (text == "-1")
				{
					return false;
				}
				mainResponse_string = text;
				Dictionary<string, string> dictionary = PuvoxLibrary.Methods.deserialize(text);
				string str = (dictionary["enchint"] == "") ? dictionary["data"] : PuvoxLibrary.Methods.DecryptString(dictionary["data"], Slug + dictionary["enchint"] + Version.ToString());
				mainResponse = PuvoxLibrary.Methods.deserialize(str);
			}
			return true;
		}
		string mainResponse_string;
		
		internal bool licenseAllowed()
		{
			return MainResponse["status"] == "200";
		}

		private bool isDevelopment { get { return PuvoxLibrary.Methods.IsDeveloperMachine(); } }
	
		public bool updateAvailable()
		{
			bool result;
			try
			{
				result = (mainResponse["version"].ToString() != Version.ToString());
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}





		public string regBase = "SOFTWARE\\MyCompany\\";
		public string getRegistryValue(string key)
		{
			return PuvoxLibrary.Methods.getRegistryValue(regBase + "\\" + key);
		}
		public bool setRegistryValue(string key)
		{
			return PuvoxLibrary.Methods.setRegistryValue(regBase + "\\" + key);
		}
	}
}
#endregion







namespace PuvoxLibrary
{
	public partial class Methods
	{
		private string ProgramName_ = "";
		public static string ProgramName { get { if (ProgramName_ == "") { m("Please set .ProgramName property ( ), otherwise Library methods can't function normally."); } return ProgramName_; } set { ProgramName_ = value; } }

		public static string Language;
		public static string Name;
		public static string Slug;
		public static double Version;
		public static string BaseProductUrl;
		public static string responseUrl;
		public static string RegRootOfThisProg;
		public static string infoUrl;
		public static string contactUrl;
		public static string AppNameRegex = "(XYZXYZ)";
		public static bool isDevelopment;
		public static string baseDomain = "https://puvox.software/";
		public static string baseResponsePath = "program-responses.php?";
		public static string baseCompanyName = "Puvox";
		public static string baseContactPath = "contact";
		public static string developerMachineString = "puvox_development_machine";
		public static bool Development_Mode;
		public static bool initialized;
		public static Dictionary<string, string> TheNames;
		public static string mainResponse_string;
		private Dictionary<string, string> mainResponse;
		public static bool debug_show_response;
		public static string mainDrive = Path.GetPathRoot(Environment.SystemDirectory);

		private Dictionary<string, string> symbols = new Dictionary<string, string>
		{
			{ "checkmark", "?" },
			{ "checkmark2", "\ud83d\uddf9" }
		};






		#region DateTime

		//  0940 type time-ints
		public static bool isBetween(DateTime target, DateTime start, DateTime end, bool equality)
		{
			int num_middle = int.Parse(target.ToString("HHmm"));
			int num2 = int.Parse(start.ToString("HHmm"));
			int num3 = int.Parse(end.ToString("HHmm"));
			return (equality ? num_middle >= num2 && num_middle <= num3 : num_middle > num2 && num_middle < num3);
		}
		public static bool isBetween(DateTime target, int start, int end)
		{
			int num = int.Parse(target.ToString("HHmm"));
			return num >= start && num < end;
		}

		#region Demo(Trial) period checks
		public static bool workTillDate(string program_slug, string dateTill)
		{
			bool result;
			try
			{
				if (dateTill != "" && DateTime.Now > DateTime.ParseExact(dateTill, "yyyy-MM-dd", CultureInfo.InvariantCulture))
				{
					bool flag = initialized;
					if (mainResponse.ContainsKey("error_message") || mainResponse["error_message"] != "")
					{
						Methods.m(mainResponse["error_message"]);
						return false;
					}
				}
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}


		private bool demoPeriodGone_shown;
		private bool demoPeriodGone_disallow;
		// "yyyy-MM-dd"
		public static bool demoPeriodGone(string message, string dateTill)
		{
			if (demoPeriodGone_shown)
			{
				return demoPeriodGone_disallow;
			}
			demoPeriodGone_shown = true;
			demoPeriodGone_disallow = false;
			try
			{
				if (dateTill != "" && DateTime.Now > DateTime.ParseExact(dateTill, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture))
				{
					System.Windows.Forms.MessageBox.Show(message.ToString());
					demoPeriodGone_disallow = true;
				}
			}
			catch (Exception)
			{
				demoPeriodGone_disallow = true;
			}
			return demoPeriodGone_disallow;
		}
		#endregion


		public static string timeIntToString(int timenow)
		{
			// timenow is i.e. 061055
			string t = timenow.ToString();
			string str = t.Length <= 1 ? "00000" + t : (t.Length <= 2 ? "0000" + t : (t.Length <= 3 ? "000" + t : (t.Length <= 4 ? "00" + t : (t.Length <= 5 ? "0" + t : t))));
			return str;
		}

		public static TimeSpan timeIntToTimeSpan(int timenow)
		{
			DateTime dt;
			DateTime.TryParseExact(timeIntToString(timenow), "HHmmss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dt);
			return dt.TimeOfDay;
		}



		public static int CorrectTime(int timenow, int added_or_subtracted)
		{
			TimeSpan timeSpan = new TimeSpan(timenow / 100, timenow % 100, 0) + TimeSpan.FromMinutes((double)added_or_subtracted);
			return timeSpan.Hours * 100 + timeSpan.Minutes;
		}
		public static int CorrectTime1(int timenow, int added_or_subtracted)
		{
			return int.Parse(DateTime.ParseExact(timenow.ToString("0000"), "HHmm", null).AddMinutes((double)added_or_subtracted).ToString("HHmm"));
		}


		public static int DateToTime(DateTime dt) { return dt.Hour * 100 + dt.Minute; }
		public static DateTime ToDateTime(string s) { return ToDateTime(s, "ddMMyyyy", ""); }
		public static DateTime ToDateTime(string s, string format) { return ToDateTime(s, format, ""); }
		public static DateTime ToDateTime(string s, string format, string cultureString)
		{    //tr-TR
			CultureInfo _culture = (cultureString == "") ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(cultureString);
			try
			{
				return DateTime.ParseExact(s: s, format: format, provider: _culture);
			}
			catch (FormatException) { throw; }
			catch (Exception) { throw; } // Given Culture is not supported culture
		}


		//i.e.    IsTimePeriod("start"
		public static bool IsTimePeriod(string type_, DateTime time1, DateTime time0, int StartTime, int EndTime, bool useDayOrSession, bool Bars_IsFirstBarOfSession)
		{
			int curTime_0 = DateToTime(time0);
			int curTime_1 = DateToTime(time1);
			bool SameDay = StartTime < EndTime;
			bool NotSameDay = StartTime > EndTime;

			bool IsNewDay = useDayOrSession ? time0.DayOfYear != time1.DayOfYear : Bars_IsFirstBarOfSession;
			bool isStart =
				(curTime_0 >= StartTime && (curTime_1 < StartTime || IsNewDay))
							||
				(IsNewDay && curTime_1 <= StartTime);

			bool is_inside =
					(SameDay && (curTime_0 >= StartTime && curTime_0 <= EndTime))
						||
					(NotSameDay && (curTime_0 >= StartTime || curTime_0 <= EndTime));
			bool was_inside =
				(SameDay && (curTime_1 >= StartTime && curTime_1 <= EndTime))
					||
				(NotSameDay && (curTime_1 >= StartTime || curTime_1 <= EndTime));


			if (type_ == "start")
				return isStart;
			else if (type_ == "inside")
				return is_inside;
			else if (type_ == "end")
				return (!is_inside && was_inside);

			return false;
		}

		public static string DateToTimeString(DateTime dt)
		{
			return (dt.Hour < 10 ? "0" : "") + dt.Hour.ToString() + ":" + (dt.Minute < 10 ? "0" : "") + dt.Minute.ToString();
		}

		public static int HoursMinutes(DateTime time)
		{
			return time.Hour * 100 + time.Minute;
		}

		public static DateTime ToTimed_ToDate(DateTime cur_time, int timenow)
		{
			return DateTime.ParseExact(cur_time.ToString("yyyy-M-dd") + timenow.ToString(" 0000"), "yyyy-M-dd HHmm", null);
		}


		public static int GetWeekOfMonth(DateTime targetTime)
		{
			DateTime first = new DateTime(targetTime.Year, targetTime.Month, 1);
			return GetWeekOfYear(targetTime) - GetWeekOfYear(first) + 1;
		}

		public static int GetWeekOfYear(DateTime targetTime)
		{
			return (new System.Globalization.GregorianCalendar()).GetWeekOfYear(targetTime, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
		}

		public static int GetQuarter(DateTime targetTime)
		{
			int result = 0;
			int month = targetTime.Month;
			if (month <= 3) result = 1;
			else if (month > 3 && month <= 6) result = 2;
			else if (month > 6 && month <= 9) result = 3;
			else if (month > 9) result = 4;
			return result;
		}
		#endregion



		#region trading

		public static string opposite(string text)
		{
			if (text == "") return "";

			// ===== long / short
			else if (text == "LONG") return "SHORT";
			else if (text == "SHORT") return "LONG";
			else if (text == "Long") return "Short";
			else if (text == "Short") return "Long";
			else if (text == "long") return "short";
			else if (text == "short") return "long";
			//
			else if (text == "L") return "S";
			else if (text == "S") return "L";
			else if (text == "l") return "s";
			else if (text == "s") return "l";

			// ===== buy / sell
			else if (text == "BUY") return "SELL";
			else if (text == "SELL") return "BUY";
			else if (text == "Buy") return "Sell";
			else if (text == "Sell") return "Buy";
			else if (text == "buy") return "sell";
			else if (text == "sell") return "buy";
			//
			else if (text == "B") return "S";
			else if (text == "S") return "B";
			else if (text == "b") return "s";
			else if (text == "s") return "b";

			// ===== above / below
			else if (text == "ABOVE") return "BELOW";
			else if (text == "BELOW") return "ABOVE";
			else if (text == "Above") return "Below";
			else if (text == "Below") return "Above";
			else if (text == "above") return "below";
			else if (text == "below") return "above";

			return "";
		}

		public static bool is_Between(DateTime target, DateTime start, DateTime end)
		{
			int target_ = Int32.Parse(target.ToString("HHmm"));
			int start_ = Int32.Parse(start.ToString("HHmm"));
			int end_ = Int32.Parse(end.ToString("HHmm"));
			return target_ >= start_ && target_ < end_;
		}

		public static bool is_Between(DateTime target, int start, int end)
		{
			int target_ = Int32.Parse(target.ToString("HHmm"));
			int start_ = start;
			int end_ = end;
			return target_ >= start_ && target_ < end_;
		}

		#endregion



		public static void createDefaultValues(object obj)
		{
			foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(obj))
			{
				DefaultValueAttribute myAttribute = (DefaultValueAttribute)property.Attributes[typeof(DefaultValueAttribute)];

				if (myAttribute != null)
				{
					property.SetValue(this, myAttribute.Value);
				}
			}
		}



		public static int digitsInNumber(object obj)
		{
			return (int)Math.Ceiling(Math.Log10((double)obj));  //i.e. 12345.123 --->5
		}


		//https://stackoverflow.com/questions/13477689/find-number-of-decimal-places-in-decimal-value-regardless-of-culture | second variation: https://pastebin.com/KtwzB9Z6
		public static int digitsAfterDot(double tickSize)
		{
			var precision = 0;
			while (tickSize * Math.Pow(10, precision) != Math.Round(tickSize * Math.Pow(10, precision)))
				precision++;
			return precision;
		}

		public static int digitsAfterDot2(double tickSize)
		{
			double num = tickSize;
			int num2 = 0;
			while (num != Math.Round(num, 0)) { num *= 10.0; num2++; }
			return num2;
		}

		//else if (content == 1E-05)	result = 5;
		//else if (content == 5E-05)	result = 5;
		//else if (content == 1E-06)	result = 6;
		//else if (content == 5E-06)	result = 6;
		//else if (content == 1E-07)	result = 7;
		//else if (content == 5E-07)	result = 7;





		#region GlobalVariables
		public static Dictionary<string, object> GlobalVariables = new Dictionary<string, object>();
		public static object getGlobalVariable(string param)
		{
			return (GlobalVariables.ContainsKey(param) ? GlobalVariables[param] : null);
		}

		public static void setGlobalVar(string param, object value)
		{
			Methods.GlobalVariables[param] = value;
		}

		#endregion



		#region Dictionary

		/*
        public static Dictionary<string, string> CopyDictionary(Dictionary<string, string> dict)
        {
            return new Dictionary<string, string>(dict);  // //return dict.ToDictionary(entry => entry.Key,   entry => entry.Value);
        }
        public static Dictionary<string, int> CopyDictionary(Dictionary<string, int> dict)
        {
            return new Dictionary<string, int>(dict);  // //return dict.ToDictionary(entry => entry.Key,   entry => entry.Value);
        }
        public static Dictionary<string, double> CopyDictionary(Dictionary<string, double> dict)
        {
            return new Dictionary<string, double>(dict);  // //return dict.ToDictionary(entry => entry.Key,   entry => entry.Value);
        }
        public static Dictionary<string, bool> CopyDictionary(Dictionary<string, bool> dict)
        {
            return new Dictionary<string, bool>(dict);  // //return dict.ToDictionary(entry => entry.Key,   entry => entry.Value);
        }
        */




		private static string objectToXml(object output)
		{
			string result = "";
			try
			{
				using (StringWriter stringWriter = new StringWriter())
				{
					XmlSerializer xmlSerializer = new XmlSerializer(output.GetType());
					xmlSerializer.Serialize(stringWriter, output);
					result = stringWriter.ToString();
				}
			}
			catch (Exception)
			{
				try
				{
					result = output.ToString();
				}
				catch (Exception ex)
				{
					result = ex.ToString();
				}
			}
			return result;
		}

		public static Dictionary<string, string> objToDict(System.Windows.Forms.ComboBox.ObjectCollection objs)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (object obj in objs)
			{
				KeyValuePair<string, string> keyValuePair = (KeyValuePair<string, string>)obj;
				dictionary.Add(keyValuePair.Key, keyValuePair.Value);
			}
			return dictionary;
		}


		public static int getIndexByKey(Dictionary<string, string> dict, string key)
		{
			List<KeyValuePair<string, string>> list = dict.ToList<KeyValuePair<string, string>>();
			return list.FindIndex((KeyValuePair<string, string> x) => x.Key == key);
		}



		public static Dictionary<string, string> SortDict(Dictionary<string, string> dict)
		{
			// Order by values.
			//var items = from pair in dict      orderby pair.Value ascending       select pair;
			// foreach (KeyValuePair<string, int> pair in items)  {  Console.WriteLine("{0}: {1}", pair.Key, pair.Value);  }
			//return items.ToDictionary(x => x, x => x);try
			try
			{
				return (Dictionary<string, string>)(from entry in dict orderby entry.Value ascending select entry);
			}
			catch //(Exception e)
			{
				return new Dictionary<string, string> { };
			}

		}

		public static bool ContainsValue(Dictionary<int, double> myd, int indx) { return myd.ContainsKey(indx) && myd[indx] != double.NaN && HasValue(myd[indx]); }
		public static bool ContainsValue(Dictionary<int, bool> myd, int indx) { return myd.ContainsKey(indx); }
		public static bool ContainsValue(Dictionary<string, double> myd, string indx) { return myd.ContainsKey(indx) && myd[indx] != double.NaN && HasValue(myd[indx]); }
		public static bool ContainsValue(Dictionary<int, object> myd, int indx) { return myd.ContainsKey(indx) && myd[indx] != null; }

		public static bool ContainsKey(NameValueCollection collection, string key)
		{
			return collection.Get(key) != null || collection.AllKeys.Contains(key);
		}
		//(int)Enum.Parse(typeof(TestAppAreana.MovieList.Movies), KeyVal);

		public static bool HasValue(double value)
		{
			return !double.IsNaN(value) && !double.IsInfinity(value);
		}


		public static Dictionary<int, double> FindNearestValuesInDict(Dictionary<int, double> MyDict, double Target_value, int how_many_values_to_find)
		{
			Dictionary<int, double> result;
			try
			{
				bool flag = DictIsAscendingOrDescending(MyDict);
				int num = MyDict.Keys.Count<int>();
				int num2 = 0;
				int num3 = 0;
				Dictionary<int, double> dictionary = new Dictionary<int, double>(MyDict);
				for (int i = 0; i < num; i++)
				{
					int index = flag ? i : (num - 1 - i);
					int index2 = flag ? (num - 1 - i) : i;
					//remove all "grater than" occurences
					if (MyDict.Keys.ElementAtOrDefault(index) != 0)
					{
						int num4 = MyDict.Keys.ElementAt(index);
						if (ContainsValue(MyDict, num4) && MyDict[num4] > Target_value)
						{
							num2++;
							if (num2 > how_many_values_to_find)
							{
								dictionary.Remove(num4);
							}
						}
					}
					//remove all "lower than" occurences
					if (MyDict.Keys.ElementAtOrDefault(index2) != 0)
					{
						int num5 = MyDict.Keys.ElementAt(index2);
						if (ContainsValue(MyDict, num5) && MyDict[num5] < Target_value)
						{
							num3++;
							if (num3 > how_many_values_to_find)
							{
								dictionary.Remove(num5);
							}
						}
					}
				}
				result = dictionary;
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}


		public static bool DictIsAscendingOrDescending(Dictionary<int, double> MyDict)
		{
			int num = MyDict.Keys.Count<int>();
			double num2 = double.NaN;
			for (int i = 0; i < num; i++)
			{
				int num3 = MyDict.Keys.ElementAt(i);
				if (ContainsValue(MyDict, num3))
				{
					if (!double.IsNaN(num2))
					{
						return MyDict[num3] > num2;
					}
					num2 = MyDict[num3];
				}
			}
			return false;
		}



		// another: 3.5 (but not in NT without reference) http://procbits.com/2011/04/21/quick-json-serializationdeserialization-in-c
		// another: https://pastebin.com/raw/hAtAwA40



		public static string serialize2(object dict)
		{
			if (dict is Dictionary<string, string>) return new JavaScriptSerializer().Serialize(((Dictionary<string, string>)dict).ToDictionary((KeyValuePair<string, string> item) => item.Key.ToString(), (KeyValuePair<string, string> item) => item.Value.ToString()));
			//else Dictionary<string, object> dict
			return new JavaScriptSerializer().Serialize(((Dictionary<string, object>)dict).ToDictionary((KeyValuePair<string, object> item) => item.Key.ToString(), (KeyValuePair<string, object> item) => item.Value.ToString()));
		}

		public static string dictToString(object obj)
		{
			return string.Join("; ", (obj as Dictionary<object, object>).Select(x => x.Key.ToString() + "=" + x.Value.ToString()).ToArray());
		}


		//return ( NetFrameworkVersion() <= 3.5 ?  deserializer_35(str)  : deserializer(str) );
		public static Dictionary<string, string> deserialize(string str)
		{
			return deserializer_35(str);
		}

		public static bool deserialize(string str, ref Dictionary<string, string> dict)
		{
			Dictionary<string, string> dictionary = deserializer_35(str);
			if (dictionary != null)
			{
				dict = dictionary;
				return true;
			}
			return false;
		}

		public static Dictionary<string, string> deserializer_35(string str)
		{
			Dictionary<string, string> result;
			try
			{
				result = ConvertToStringDictionary(Methods.JsonMaker.ParseJSON(str));
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
			//Dictionary<string, string> new_dict = new Dictionary<string, string>();
			//foreach (KeyValuePair<string, object> eachObj in dict)
			// new_dict[eachObj.Key] = eachObj.Value.ToString();
			//return new_dict;
		}


		public static Dictionary<string, string> ConvertToStringDictionary(Dictionary<string, object> dicti)
		{
			Dictionary<string, string> result;
			try
			{
				result = dicti.ToDictionary((KeyValuePair<string, object> item) => item.Key, (KeyValuePair<string, object> item) => (string)item.Value);
			}
			catch (Exception)
			{
				result = new Dictionary<string, string>();
			}
			return result;
		}


		public static string getDictionaryKeyByValue(Dictionary<string, string> dict, string value)
		{
			return dict.FirstOrDefault((KeyValuePair<string, string> x) => x.Value == value).Key;
		}



		public static Dictionary<string, string> xml_to_dictionary(string xml_data)
		{

			//xml_data = "<data><test>foo</test><test>foobbbbb</test><bar>123</bar><username>foobar</username></data>";

			XDocument doc = XDocument.Parse(xml_data);
			Dictionary<string, string> dataDictionary = new Dictionary<string, string>();

			foreach (XElement element in doc.Descendants().Where(p => p.HasElements == false))
			{
				int keyInt = 0;
				string keyName = element.Name.LocalName;

				while (dataDictionary.ContainsKey(keyName))
				{
					keyName = element.Name.LocalName + "_" + keyInt++;
				}

				dataDictionary.Add(keyName, element.Value);
			}
			return dataDictionary;
		}

		#endregion








		#region Registry
		public static RegistryHive chosenRegHive = RegistryHive.CurrentUser;    //NT 8 Cannot implicitly convert type 'Microsoft.Win32.RegistryKey' to 'Microsoft.Win32.RegistryHive'	

		public static RegistryKey chosenRegKey = Registry.CurrentUser;

		// subkey like :  subKey = "SOFTWARE\\MyCompany\\programmmm;

		public static RegistryKey getRegistryHiveKey(RegistryHive rh)
		{
			//3.5
			//return rh; //RegistryKey.OpenSubKey(rh ) ;
			return RegistryKey.OpenBaseKey(rh, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
		}

		public static bool is64BitOS = Environment.Is64BitOperatingSystem;


		public static void Read64bitRegistryFrom32bitApp(string[] args)
		{
			string value64 = string.Empty;
			string value32 = string.Empty;

			RegistryKey localKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
			localKey = localKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
			if (localKey != null)
			{
				value64 = localKey.GetValue("RegisteredOrganization").ToString();
			}
			RegistryKey localKey32 = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry32);
			localKey32 = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
			if (localKey32 != null)
			{
				value32 = localKey32.GetValue("RegisteredOrganization").ToString();
			}
		}


		public static Dictionary<string, string> myregs = new Dictionary<string, string>();

		public static string regPartFromKey(string key, int partN)
		{
			if (partN == 1)
			{
				if (charsInPhrase(key, "\\") != 0)
				{
					return withoutLastDir(key, 1);
				}
				return key;
			}
			else
			{
				if (partN != 2)
				{
					return "";
				}
				if (charsInPhrase(key, "\\") != 0)
				{
					return lastPart(key);
				}
				return key;
			}
		}


		public static bool existsRegistryValue(string key)
		{
			return getRegistryValue(key) != null;
		}


		public static bool existsRegistryValue(string path, string key)
		{
			return getRegistryValue(path + key) != null;
		}


		public static string getRegistryValue(string key)
		{
			return getRegistryValue(regPartFromKey(key, 1), regPartFromKey(key, 2));
		}

		// // ----- 62 vs 32 :  https://apttech.wordpress.com/2012/01/06/difference-between-a-registry-hive-and-registry-key-2/
		public static string getRegistryValue(string path, string key)
		{
			string result;
			try
			{
				string text = "";
				if (myregs.ContainsKey(path + key))
				{
					result = myregs[path + key];
				}
				else
				{
					RegistryKey registryHiveKey = getRegistryHiveKey(chosenRegHive);
					if (registryHiveKey == null)
					{
						result = text;
					}
					else
					{
						RegistryKey registryKey = registryHiveKey.OpenSubKey(path);
						if (registryKey == null)
						{
							registryHiveKey.Close();
							result = text;
						}
						else
						{
							object value = registryKey.GetValue(key);
							text = ((value != null) ? ((string)value) : null);
							registryKey.Close();
							registryHiveKey.Close();
							myregs[path + key] = text;
							result = text;
						}
					}
				}
			}
			catch (Exception)
			{
				result = "";
			}
			return result;
		}


		public static string getRegistryValue(string key, string defaultVal, bool nothing)
		{
			string registryValue = getRegistryValue(regPartFromKey(key, 1), regPartFromKey(key, 2));
			if (string.IsNullOrEmpty(registryValue))
			{
				setRegistryValue(key, defaultVal);
				return defaultVal;
			}
			return registryValue;
		}


		public static void setRegistryValue(string key, string value)
		{
			setRegistryValue(regPartFromKey(key, 1), regPartFromKey(key, 2), value);
		}


		public static bool setRegistryValue(string path, string key, string value)
		{
			bool result;
			try
			{
				RegistryKey registryHiveKey = getRegistryHiveKey(chosenRegHive);
				if (registryHiveKey != null)
				{
					RegistryKey registryKey = registryHiveKey.OpenSubKey(path, true);
					if (registryKey == null)
					{
						registryKey = registryHiveKey.CreateSubKey(path);
					}
					myregs[path + key] = value;
					registryKey.SetValue(key, value);
					result = true;
				}
				else
				{
					Methods.m("| regHive is null");
					result = false;
				}
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}


		public static string getSetRegistryValue(string key, string value)
		{
			return getSetRegistryValue(regPartFromKey(key, 1), regPartFromKey(key, 2), value);
		}


		public static string getSetRegistryValue(string path, string key, string value)
		{
			string result;
			try
			{
				string registryValue = getRegistryValue(path, key);
				if (string.IsNullOrEmpty(registryValue))
				{
					setRegistryValue(path, key, value);
					result = value;
				}
				else
				{
					result = registryValue;
				}
			}
			catch (Exception ex)
			{
				Methods.m(string.Concat(new string[]
				{
					ex.Message,
					" | getSetRegistryValue ",
					path,
					" ->",
					key
				}));
				result = null;
			}
			return result;
		}



		// c3.5
		public static string Read(string subKey, string KeyName)
		{
			string result;
			try
			{
				RegistryKey registryKey = chosenRegKey.OpenSubKey(subKey);
				if (registryKey == null)
				{
					result = null;
				}
				else
				{
					result = (string)registryKey.GetValue(KeyName.ToUpper());
				}
			}
			catch (Exception ex)
			{
				Methods.m(ex.Message + " | Reading registry " + KeyName.ToUpper());
				result = "";
			}
			return result;
		}


		public static bool Write(string subKey, string KeyName, object Value)
		{
			bool result;
			try
			{
				RegistryKey registryKey = chosenRegKey;
				RegistryKey registryKey2 = registryKey.CreateSubKey(subKey);
				registryKey2.SetValue(KeyName.ToUpper(), Value);
				result = true;
			}
			catch (Exception ex)
			{
				Methods.m(ex.Message + " | Writing registry " + KeyName.ToUpper());
				result = false;
			}
			return result;
		}


		public static bool DeleteKey(string subKey, string KeyName)
		{
			bool result;
			try
			{
				RegistryKey registryKey = chosenRegKey;
				RegistryKey registryKey2 = registryKey.CreateSubKey(subKey);
				if (registryKey2 == null)
				{
					result = true;
				}
				else
				{
					registryKey2.DeleteValue(KeyName);
					result = true;
				}
			}
			catch (Exception ex)
			{
				Methods.m(ex.Message + " | Deleting SubKey " + subKey.ToUpper());
				result = false;
			}
			return result;
		}


		public static bool DeleteSubKeyTree(string subKey)
		{
			bool result;
			try
			{
				RegistryKey registryKey = chosenRegKey;
				RegistryKey registryKey2 = registryKey.OpenSubKey(subKey);
				if (registryKey2 != null)
				{
					registryKey.DeleteSubKeyTree(subKey);
				}
				result = true;
			}
			catch (Exception ex)
			{
				Methods.m(ex.Message + " | Deleting SubKey tree" + subKey.ToUpper());
				result = false;
			}
			return result;
		}


		public static int SubKeyCount(string subKey)
		{
			int result;
			try
			{
				RegistryKey registryKey = chosenRegKey;
				RegistryKey registryKey2 = registryKey.OpenSubKey(subKey);
				result = ((registryKey2 != null) ? registryKey2.SubKeyCount : 0);
			}
			catch (Exception ex)
			{
				Methods.m(ex.Message + " | Retriving subkeys of" + subKey.ToUpper());
				result = 0;
			}
			return result;
		}


		public static int ValueCount(string subKey)
		{
			int result;
			try
			{
				RegistryKey registryKey = chosenRegKey;
				RegistryKey registryKey2 = registryKey.OpenSubKey(subKey);
				result = ((registryKey2 != null) ? registryKey2.ValueCount : 0);
			}
			catch (Exception ex)
			{
				Methods.m(ex.Message + " | Retrieving subkeys of" + subKey.ToUpper());
				result = 0;
			}
			return result;
		}


		public static Dictionary<string, string> RegistryValuesInFolder(string keyroot)
		{
			Dictionary<string, string> result;
			try
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				string regRootOfThisProg = keyroot;
				using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(regRootOfThisProg))
				{
					foreach (string text in registryKey.GetValueNames())
					{
						dictionary[text] = (string)registryKey.GetValue(text);
					}
				}
				result = dictionary;
			}
			catch (Exception)
			{
				result = new Dictionary<string, string>();
			}
			return result;
		}


		public static bool FirstTimeAction(string regKey)
		{
			if (getRegistryValue("triggered_" + regKey) != "y")
			{
				setRegistryValue("triggered_" + regKey, "y");
				return true;
			}
			return false;
		}


		public static bool TimeGone(string Key, int minutes)
		{
			string registryValue = getRegistryValue("timegone_" + Key);
			if (string.IsNullOrEmpty(registryValue) || DateTime.Now > DateTime.Parse(registryValue).AddMinutes((double)minutes))
			{
				setRegistryValue("timegone_" + Key, DateTime.Now.ToString());
				return true;
			}
			return false;
		}
		#endregion












		/* doesnt work in C#4.5 native projects
		#region Color region Brushes
		public static System.Windows.Media.Color ChangeColorBrightness(System.Windows.Media.Color color, string correctionFactor)
		{
			if (!(correctionFactor == "dark")) correctionFactor = "light"; 
			return ChangeColorBrightness(color, correctionFactor);
		}
		

        public static System.Windows.Media.Color ChangeColorBrightness(System.Windows.Media.Color color, float correctionFactor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return System.Windows.Media.Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
        }

		
		public static System.Windows.Media.Color color_from_hex(String hex)
	    {
	        //remove the # at the front
	        hex = hex.Replace("#", "");

	        byte a = 255;
	        byte r = 255;
	        byte g = 255;
	        byte b = 255;

	        int start = 0; 

	        //handle ARGB strings (8 characters long)
	        if (hex.Length == 8)
	        {
	            a = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
	            start = 2;
	        }

	        //convert RGB characters to bytes
	        r = byte.Parse(hex.Substring(start, 2), System.Globalization.NumberStyles.HexNumber);
	        g = byte.Parse(hex.Substring(start + 2, 2), System.Globalization.NumberStyles.HexNumber);
	        b = byte.Parse(hex.Substring(start + 4, 2), System.Globalization.NumberStyles.HexNumber);

	        return System.Windows.Media.Color.FromArgb(a, r, g, b);
	    }
    
		#endregion
		*/








		#region String manipulations


		public static string lastPart(string path)
		{
			if (path.Contains("\\"))
			{
				return path.Split(new char[]
				{
					'\\'
				}).Last<string>();
			}
			return path.Split(new char[]
			{
				'/'
			}).Last<string>();
		}


		public static int charsInPhrase(string source, string char_)
		{
			int num = 0;
			for (int i = 0; i < source.Length; i++)
			{
				if (source[i].ToString() == char_)
				{
					num++;
				}
			}
			return num;
		}

		public static string sanitizer(string dirtyString)
		{
			string a = "replace";
			string text = " ?&^$#@!()+-,:;<>\\?'\"-_*";
			string text2 = dirtyString;
			if (a == "replace")
			{
				string text3 = text;
				for (int i = 0; i < text3.Length; i++)
				{
					text2 = text2.Replace(text3[i].ToString(), "_");
				}
			}
			else if (!(a == "regex"))
			{
				if (a == "ld")
				{
					text2 = new string(text2.Where(new Func<char, bool>(char.IsLetterOrDigit)).ToArray<char>());
				}
				else if (a == "hashset")
				{
					HashSet<char> hashSet = new HashSet<char>(text);
					StringBuilder stringBuilder = new StringBuilder(text2.Length);
					foreach (char c in text2)
					{
						if (hashSet.Contains(c))
						{
							stringBuilder.Append(c);
						}
					}
					text2 = stringBuilder.ToString();
				}
			}
			return text2;
		}


		public static bool stringInArrayStart(string str, object array)
		{
			bool result = false;
			foreach (object obj in (IList)array) if (str.StartsWith(obj.ToString())) result = true;
			return result;
		}

		public static bool stringInArray(string str, object array)
		{
			bool result = false;
			foreach (object obj in ((IList)array))
			{
				if (str == obj.ToString())
				{
					result = true;
				}
			}
			return result;
		}


		// https://stackoverflow.com/questions/3840762/how-do-you-urlencode-without-using-system-web 
		public static string urlEncode(string msg)
		{
			return Uri.EscapeDataString(msg); //.Replace("%20", "+");
		}

		public static int countWords(string str)
		{
			char[] separator = new char[] { ' ', '\r', '\n' };
			return str.Split(separator, StringSplitOptions.RemoveEmptyEntries).Length;
		}

		public static string string_repeat(string str, int level)
		{
			return new StringBuilder().Insert(0, str, level).ToString();
		}


		public static string pChars(string txt)
		{
			int num = Math.Max(1, 25 - txt.Length);
			string text = txt + new string(' ', num);
			if (num > 1)
			{
				text += '\t';
			}
			return text;
		}

		public static string textLengthen(string txt, int char_count, string letter)
		{
			while (txt.Length < char_count)
			{
				txt += letter;
			}
			return txt;
		}


		public static string SplitToLines(string text, char[] splitOnCharacters, int maxStringLength)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			while (text.Length > num)
			{
				if (num != 0)
				{
					stringBuilder.AppendLine();
				}
				int num2 = (num + maxStringLength <= text.Length) ? text.Substring(num, maxStringLength).LastIndexOfAny(splitOnCharacters) : (text.Length - num);
				num2 = ((num2 == -1) ? maxStringLength : num2);
				stringBuilder.Append(text.Substring(num, num2).Trim());
				num += num2;
			}
			return stringBuilder.ToString();
		}

		public static string RandomString(int length)
		{

			//Char[] pwdChars = new Char[62] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
			//string sometext = "";
			//string SiteURL = String.Format("http://www.test444.dreamhosters.com/tdprot/my_protector.php?id={0}&value={1}", sometext, sometext);
			//Random rand = new Random();
			//for (int i = 0; i < 20; i++)
			//    sometext += pwdChars[rand.Next(0, 62)];

			var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var stringChars = new char[length];
			var random = new Random();
			for (int i = 0; i < stringChars.Length; i++) { stringChars[i] = chars[random.Next(chars.Length)]; }
			var finalString = new String(stringChars);
			return finalString;
		}



		public static string DateToSeconds(DateTime date)
		{
			return date.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
		}

		public static string convertBackSlashes(string path)
		{
			return path.Replace("/", "\\");
		}

		public static string regexReplace(string input, string pattern, string with)
		{
			return new Regex(pattern).Replace(input, "");
		}


		public static string SanitizeSymbol(string s)
		{
			return s.Replace("/", "_").Replace("\\", "_").Replace("|", "_").Replace("*", "_").ToUpper();
		}


		public static string NotNull(string smth)
		{
			if (!string.IsNullOrEmpty(smth))
			{
				return smth;
			}
			return "";
		}


		public static bool empty(string text)
		{
			return text == null || text.Trim().Length == 0 || text.Trim() == "";
		}


		public static bool normalDouble(double val)
		{
			return !double.IsNaN(val) && val < double.MaxValue && val > double.MinValue;
		}

		public static string Repeat(string value, int count)
		{
			return new StringBuilder(value.Length * count).Insert(0, value, count).ToString();
		}

		public static string[] file_to_lines(string file_location)
		{
			string[] result = new string[0];
			if (File.Exists(file_location))
			{
				result = File.ReadAllText(file_location).Split(new string[]
				{
					Environment.NewLine,
					"\\r"
				}, StringSplitOptions.None);
			}
			return result;
		}

		public static string NewLinedString(string text, int after_how_many_words)
		{
			string text2 = "";
			string[] array = text.Split(new string[]
			{
				"\n",
				"\r\n"
			}, StringSplitOptions.RemoveEmptyEntries);
			int num = 0;
			foreach (string text3 in array)
			{
				num++;
				string[] array3 = text3.Split(new string[] { }, StringSplitOptions.RemoveEmptyEntries);
				int num2 = 0;
				foreach (string str in array3)
				{
					num2++;
					if (num2 < after_how_many_words)
					{
						text2 = text2 + str + " ";
					}
					else if (num2 == after_how_many_words)
					{
						text2 = text2 + str + Environment.NewLine;
						num2 = 0;
					}
				}
				if (num != array.Count<string>())
				{
					text2 += "\r\n";
				}
			}
			return text2;
		}



		#region Encrypt/Decrypt
		// encryption 
		public static string EncryptString(string plainText, string password)
		{
			try
			{
				SHA256 mySHA256 = SHA256Managed.Create();
				byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
				byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
				// string symmetric encryption
				Aes encryptor = Aes.Create();
				encryptor.Mode = CipherMode.CBC;
				//encryptor.KeySize = 256;    encryptor.BlockSize = 128;   encryptor.Padding = PaddingMode.Zeros;
				encryptor.Key = key;
				encryptor.IV = iv;

				MemoryStream memoryStream = new MemoryStream();
				ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();
				CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write); // write to memory stream
				byte[] plainBytes = Encoding.ASCII.GetBytes(plainText); // Convert the plainText string into a byte array
				cryptoStream.Write(plainBytes, 0, plainBytes.Length);   // Encrypt the input plaintext string
				cryptoStream.FlushFinalBlock();                         // Complete the encryption process
				byte[] cipherBytes = memoryStream.ToArray();            // Convert the encrypted data from a MemoryStream to a byte array

				memoryStream.Close();
				cryptoStream.Close();
				string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);  // Convert the encrypted byte array to a base64 encoded string

				return cipherText;
			}
			catch (Exception e)
			{
				return e.Message;
			}
		}


		public static string DecryptString(string cipherText, string password)
		{
			try
			{
				SHA256 mySHA256 = SHA256Managed.Create();
				byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
				byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
				// Instantiate a new Aes object to perform string symmetric encryption
				Aes encryptor = Aes.Create();
				encryptor.Mode = CipherMode.CBC;
				//encryptor.KeySize = 256;  encryptor.BlockSize = 128;  encryptor.Padding = PaddingMode.Zeros;
				encryptor.Key = key;
				encryptor.IV = iv;

				MemoryStream memoryStream = new MemoryStream();
				ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();
				CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);  //write it to  memory stream
				string plainText = String.Empty;
				try
				{
					byte[] cipherBytes = Convert.FromBase64String(cipherText);  // Convert the ciphertext string into a byte array
					cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);      // Decrypt the input ciphertext string
					cryptoStream.FlushFinalBlock();              // Complete the decryption process
					byte[] plainBytes = memoryStream.ToArray(); // Convert the decrypted data from a MemoryStream to a byte array
					plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length); // Convert the decrypted byte array to string
				}
				catch (Exception e) { System.Windows.Forms.MessageBox.Show("Problem in encryption. ErrorCode 274. " + e.Message); }
				finally
				{
					memoryStream.Close();
					cryptoStream.Close();
				}
				return plainText;
			}
			catch (Exception e)
			{
				return e.Message;
			}
		}
		#endregion

		//if (isDevelopment)  Console.OutputEncoding = Encoding.UTF8;   

		#region Base64 md5

		public static string md5(string input)
		{
			string result;
			try
			{
				using (MD5 md = MD5.Create())
				{
					byte[] bytes = Encoding.ASCII.GetBytes(input);
					byte[] array = md.ComputeHash(bytes);
					StringBuilder stringBuilder = new StringBuilder();
					for (int i = 0; i < array.Length; i++)
					{
						stringBuilder.Append(array[i].ToString("X2"));
					}
					result = stringBuilder.ToString();
				}
			}
			catch (Exception)
			{
				result = "";
			}
			return result;
		}

		public static string Base64Decode(string str)
		{
			string result;
			try
			{
				byte[] bytes = Convert.FromBase64String(str);
				string @string = Encoding.UTF8.GetString(bytes);
				result = @string;
			}
			catch (Exception)
			{
				result = "";
			}
			return result;
		}


		public static string Base64Encode(string toEncode)
		{
			string result;
			try
			{
				byte[] bytes = Encoding.UTF8.GetBytes(toEncode);
				string text = Convert.ToBase64String(bytes);
				result = text;
			}
			catch (Exception)
			{
				result = "";
			}
			return result;
		}


		public static string decode64(string str)
		{
			byte[] bytes = Convert.FromBase64String(str);
			return Encoding.UTF8.GetString(bytes);
		}


		#endregion

		#endregion



		private static object fileCheckObj = new object();
		public static string errorLogFile = Environment.GetEnvironmentVariable("tmp") + @"\_errorlogs_c#.log";

		public static void Dispose(IDisposable resource) { if (resource != null) resource.Dispose(); }



		#region Control region Form manipulations

		private delegate void SetTextCallback(Form f, System.Windows.Forms.Control ctrl, string text);

		private delegate bool Control_SetDelegate(System.Windows.Forms.Control ctrl, string what, object value);

		public static void PopupMessage(object obj_)
		{
			Form form = new Form();
			form.Width = 310;
			form.Height = 450;
			form.Text = "Popup Message";
			form.TopMost = true;
			form.StartPosition = FormStartPosition.Manual;
			form.Top = 1;
			form.Left = 1;
			form.Opacity = 0.95;
			System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox();
			form.Controls.Add(textBox);
			textBox.Multiline = true;
			textBox.ScrollBars = ScrollBars.Both;
			textBox.Text = obj_.ToString();
			form.ShowDialog();
		}

		public static string ShowDialog(string text)
		{
			return ShowDialog(text, "", "");
		}
		public static string ShowDialog(string text, string caption, string defaultValue)
		{
			string result;
			try
			{
				Form prompt = new Form
				{
					Width = 500,
					Height = 150,
					FormBorderStyle = FormBorderStyle.FixedDialog,
					Text = caption,
					StartPosition = FormStartPosition.CenterScreen
				};
				System.Windows.Forms.Label value = new System.Windows.Forms.Label
				{
					Left = 50,
					Top = 20,
					Text = text
				};
				System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox
				{
					Left = 50,
					Top = 50,
					Width = 400
				};
				textBox.Text = defaultValue;
				System.Windows.Forms.Button button = new System.Windows.Forms.Button
				{
					Text = "Ok",
					Left = 350,
					Width = 100,
					Top = 70,
					DialogResult = DialogResult.OK
				};
				button.Click += delegate (object sender, EventArgs e)
				{
					prompt.Close();
				};
				prompt.Controls.Add(textBox);
				prompt.Controls.Add(button);
				prompt.Controls.Add(value);
				prompt.AcceptButton = button;
				prompt.TopMost = true;
				result = ((prompt.ShowDialog() == DialogResult.OK) ? textBox.Text : "");
			}
			catch (Exception)
			{
				result = "";
			}
			return result;
		}




		public static string GetControlText(Form obj, string which)
		{
			System.Windows.Forms.Control[] array = obj.Controls.Find(which, true);
			if (array.Length <= 0)
			{
				return "cant find control";
			}
			return array[0].Text;
		}


		public static void CenterLabel(System.Windows.Forms.Control ctrl)
		{
			try
			{
				ctrl.AutoSize = false;
				ctrl.Dock = DockStyle.None;
				ctrl.Width = ctrl.Parent.Width;
				if (ctrl is System.Windows.Forms.Label)
				{
					//Needs reference to System.Drawing; (ctrl as System.Windows.Forms.Label).TextAlign = ContentAlignment.MiddleCenter;
				}
			}
			catch (Exception ex)
			{
				Methods.m(ex.Message);
			}
		}


		public static bool isUserInput(System.Windows.Forms.Control ct)
		{
			return ct is System.Windows.Forms.TextBox || ct is System.Windows.Forms.ComboBox || ct is System.Windows.Forms.CheckBox || ct is CheckedListBox || ct is DateTimePicker || ct is System.Windows.Forms.GroupBox || ct is System.Windows.Forms.ListBox || ct is MonthCalendar || ct is System.Windows.Forms.RadioButton || ct is System.Windows.Forms.RichTextBox || ct is TrackBar;
		}


		public static string optionsPrefix = "formoption_";

		public static bool fillFormOptions(System.Windows.Forms.Control.ControlCollection cts)
		{
			foreach (object obj in cts)
			{
				System.Windows.Forms.Control control = (System.Windows.Forms.Control)obj;
				if (control != null && isUserInput(control))
				{
					string defaultVal;
					if (control is System.Windows.Forms.CheckBox)
					{
						defaultVal = ((System.Windows.Forms.CheckBox)control).Checked.ToString();
					}
					else
					{
						defaultVal = control.Text;
					}
					control.Text = getRegistryValue(optionsPrefix + control.Name, defaultVal, false);
				}
			}
			return true;
		}


		public static bool saveFormOptions(System.Windows.Forms.Control.ControlCollection cts)
		{
			foreach (object obj in cts)
			{
				System.Windows.Forms.Control control = (System.Windows.Forms.Control)obj;
				if (control != null && isUserInput(control))
				{
					string value;
					if (control is System.Windows.Forms.CheckBox)
					{
						value = ((System.Windows.Forms.CheckBox)control).Checked.ToString();
					}
					else
					{
						value = control.Text;
					}
					setRegistryValue(optionsPrefix + control.Name, value);
				}
			}
			return true;
		}


		public static void SetComboboxByKey(System.Windows.Forms.ComboBox cb1, string key)
		{
			cb1.SelectedIndex = getIndexByKey(objToDict(cb1.Items), key);
		}



		public static void FillCombobox(System.Windows.Forms.ComboBox cb1, Dictionary<string, string> dict)
		{
			FillCombobox(cb1, dict, false, "");
		}


		public static void FillCombobox(System.Windows.Forms.ComboBox cb1, Dictionary<string, string> dict, bool sort, string SelectedValue)
		{
			cb1.DataSource = new BindingSource(sort ? SortDict(dict) : dict, null);
			cb1.DisplayMember = "Value";
			cb1.ValueMember = "Key";
			if (SelectedValue != "")
			{
				cb1.SelectedValue = SelectedValue;
			}
		}

		// compare the thread ID of the calling thread to the thread ID of the creating thread.
		public static void invokeSafe(System.Windows.Forms.Control uiElement, Action updater, bool forceSynchronous)
		{
			try
			{
				if (uiElement != null)
				{
					if (uiElement.InvokeRequired)
					{
						if (forceSynchronous)
						{
							uiElement.Invoke(new Action(delegate ()
							{
								invokeSafe(uiElement, updater, forceSynchronous);
							}));
						}
						else
						{
							uiElement.BeginInvoke(new Action(delegate ()
							{
								invokeSafe(uiElement, updater, forceSynchronous);
							}));
						}
					}
					else if (!uiElement.IsDisposed)
					{
						updater();
					}
				}
			}
			catch (Exception)
			{
			}
		}


		public static bool Control_Set(System.Windows.Forms.Control ctrl, string what, object value)
		{
			bool result;
			try
			{
				if (ctrl == null)
				{
					result = false;
				}
				else
				{
					if (ctrl.InvokeRequired)
					{
						Methods.Control_SetDelegate method = new Methods.Control_SetDelegate(Control_Set);
						ctrl.Invoke(method, new object[]
						{
							ctrl,
							what,
							value
						});
					}
					else if (what == "visibility")
					{
						ctrl.Visible = (bool)value;
					}
					else if (what == "text")
					{
						ctrl.Text = (string)value;
					}
					else
					{
						Methods.m("incorrect value in :" + MethodBase.GetCurrentMethod().Name + ":" + what);
					}
					result = true;
				}
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		public static bool closeForm(Form frm)
		{
			bool result;
			try
			{
				if (frm == null)
				{
					result = false;
				}
				else
				{
					if (frm.IsHandleCreated)
					{
						frm.Invoke(new MethodInvoker(delegate ()
						{
							frm.Close();
						}));
					}
					else
					{
						frm.Close();
					}
					result = true;
				}
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}


		public static void SetControlText(Form obj, string which, string txt)
		{
			try
			{
				if (obj != null)
				{
					if (obj.InvokeRequired)
					{
						obj.Invoke(new Action(delegate ()
						{
							SetControlText(obj, which, txt);  //new Action<string>(SetControlText), new object[] { which, txt }
						}));
					}
					else
					{
						System.Windows.Forms.Control[] array = obj.Controls.Find(which, true);
						if (array.Count<System.Windows.Forms.Control>() > 0)
						{
							array[0].Text = txt;
						}
					}
				}
			}
			catch (Exception)
			{
			}


			//  final_str = txt.ToString();
			//  var propInfo = GetType().GetProperty(which);
			//   if (propInfo != null)
			//  {
			//      propInfo.SetValue(this, txt, null);
			//  }
			//  textBox1.Invoke(new System.Action(() =>   {    textBox1.Text = txt.ToString();    }));
		}

		public static void SetTextControl(Form form, System.Windows.Forms.Control ctrl, string text)
		{
			try
			{
				if (ctrl != null)
				{
					if (ctrl.InvokeRequired)
					{
						Methods.SetTextCallback method = new Methods.SetTextCallback(SetTextControl);
						form.Invoke(method, new object[]
						{
							form,
							ctrl,
							text
						});
					}
					else
					{
						ctrl.Text = text;
					}
				}
			}
			catch (Exception)
			{
			}
		}
		#endregion










		#region Timer 

		// method 1:
		public TimerInterrupter SetInterval(int interval, Action function)
		{
			return StartTimer(interval, function, true);
		}

		public TimerInterrupter SetTimeout(int interval, Action function)
		{
			return StartTimer(interval, function, false);
		}

		public class TimerInterrupter
		{
			public TimerInterrupter(System.Timers.Timer timer)
			{
				if (timer == null)
				{
					throw new ArgumentNullException("timer");
				}
				_timer = timer;
			}


			public void Stop()
			{
				_timer.Stop();
			}


			private readonly System.Timers.Timer _timer;
		}

		private TimerInterrupter StartTimer(int interval, Action function, bool autoReset)
		{
			TimerInterrupter result;
			try
			{
				Action functionCopy = (Action)function.Clone();
				System.Timers.Timer timer = new System.Timers.Timer
				{
					Interval = (double)interval,
					AutoReset = autoReset
				};
				timer.Elapsed += delegate (object sender, ElapsedEventArgs e)
				{
					functionCopy();
				};
				timer.Start();
				result = new TimerInterrupter(timer);
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}


		// method 2:
		private HashSet<System.Threading.Timer> ExecuteAfter_timers = new HashSet<System.Threading.Timer>();

		public void ExecuteAfter(int milliseconds, Action action)
		{
			try
			{
				System.Threading.Timer timer = null;
				timer = new System.Threading.Timer(delegate (object s)
				{
					action();
					timer.Dispose();
					lock (ExecuteAfter_timers)
					{
						ExecuteAfter_timers.Remove(timer);
					}
				}, null, (long)milliseconds, -11);
				lock (ExecuteAfter_timers)
				{
					ExecuteAfter_timers.Add(timer);
				}
			}
			catch (Exception)
			{
			}
		}

		public static void Timer(Action<object, ElapsedEventArgs> myMethod, int interval, bool autoreset)
		{
			System.Timers.Timer timer = new System.Timers.Timer();
			timer.Elapsed += myMethod.Invoke;
			timer.Interval = (double)interval;
			timer.AutoReset = autoreset;
			timer.Start();
		}
		// Standalone:
		// System.Threading.Tasks.Task.Delay(3000).ContinueWith(t => init());
		//                     or  
		// ExecuteAfter(() => MessageBox.Show("hi"), 1000 ); 
		// SetTimeout(1000,   () => { MessageBox.Show("hi");  }   );
		#endregion
















		#region Reflection Manipulation
		public static List<string> PropertyInfoToList(PropertyInfo[] PropList)
		{
			List<string> list = new List<string>();
			foreach (PropertyInfo propertyInfo in PropList)
			{
				list.Add(propertyInfo.Name);
			}
			return list;
		}

		public static List<string> ObjectPropertyNames(object Obj)
		{
			return PropertyInfoToList(Obj.GetType().GetProperties());
		}


		public static bool MethodExists(object objectToCheck, string methodName)
		{
			return objectToCheck.GetType().GetMethod(methodName) != null;
		}


		public static string Trace(Exception ex)
		{
			StackTrace stackTrace = new StackTrace(ex, true);
			StackFrame frame = stackTrace.GetFrame(0);
			return frame.GetFileLineNumber().ToString();
		}
		#endregion











		#region Url Site actions

		public static string urlRead(string url)
		{
			string responseText = "-1";
			try
			{
				// HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
				// //req.AutomaticDecompression = DecompressionMethods.GZip;
				// using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
				// using (StreamReader reader = new StreamReader(res.GetResponseStream(), ASCIIEncoding.ASCII))
				// responseText = reader.ReadToEnd(); 
				using (WebClient wc = new WebClient())
				{
					wc.Encoding = Encoding.UTF8;
					//  new StreamReader(wc.OpenRead("http://your_website.com"));
					responseText = wc.DownloadString(url);
				}
			}
			catch (Exception e) { return ("Error:" + e.ToString()); }
			return responseText;
		}

		// wb.DocumentCompleted += wb_DocumentCompleted;
		//  private void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		//  {
		//      WebBrowser wb = sender as WebBrowser;
		//  }
		public static bool downloadFile(string url, string location)
		{
			bool result;
			try
			{
				using (WebClient webClient = new WebClient())
				{
					webClient.DownloadFile(url, location);
				}
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}



		public class WebDownload : WebClient
		{
			public int Timeout { get; set; }
			public WebDownload() : this(60000)
			{
			}
			public WebDownload(int timeout)
			{
				Timeout = timeout;
			}
			protected override WebRequest GetWebRequest(Uri address)
			{
				WebRequest result;
				try
				{
					WebRequest webRequest = base.GetWebRequest(address);
					if (webRequest != null)
					{
						webRequest.Timeout = Timeout;
					}
					result = webRequest;
				}
				catch (Exception)
				{
					result = null;
				}
				return result;
			}
		}

		//(System.IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/debug.txt"))


		public static string urlPost(string url)
		{
			string result;
			try
			{
				using (WebClient webClient = new WebClient())
				{
					byte[] bytes = webClient.UploadValues(url, "POST", new NameValueCollection
					{
						{
							"param1",
							"<any> kinds & of = ? strings"
						},
						{
							"param2",
							"escaping is already handled"
						}
					});
					// string HtmlResult = wc.UploadString(URI, myParameters);
					string @string = Encoding.UTF8.GetString(bytes);
					result = @string;
				}
			}
			catch (Exception ex)
			{
				result = "Error:" + ex.ToString();
			}
			return result;
		}

		public static string tempFilePathForUrlContent(string url)
		{
			Uri uri = new Uri(url);
			string text = convertBackSlashes(Path.GetTempPath() + "\\" + url.Replace(uri.Scheme, "").Replace("://", "").Replace("//", ""));
			if (!File.Exists(text))
			{
				string directoryName = Path.GetDirectoryName(text);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				DownloadFile(url, text);
			}
			return text;
		}

		public static void urlOpen(string url)
		{
			new System.Windows.Forms.WebBrowser
			{
				AllowNavigation = true
			}.Navigate(url);
		}

		public static bool internetAvailable(string messageShow)
		{
			try
			{
				using (var client = new WebClient())
				using (client.OpenRead("http://google.com/generate_204"))
					return true;
			}
			catch (Exception ex)
			{
				if (!string.IsNullOrEmpty(messageShow))
				{
					Methods.m(messageShow + ex.Message);
				}
			}
			return false;
		}
		#endregion















		#region File manipulations

		public static string withoutLastDir(string path, int HowManyLastDirs)
		{
			string result = path;
			if (charsInPhrase(path, "\\") > HowManyLastDirs)
			{
				result = string.Join("\\", path.Split(new char[]
				{
					'\\'
				}).Reverse<string>().Skip(HowManyLastDirs).Reverse<string>().ToArray<string>());
			}
			return result;
		}


		public static bool WriteFile(string fileName, string text, bool rewrite_or_add)
		{
			bool result;
			try
			{
				string path = Environment.GetEnvironmentVariable("tmp") + "\\" + fileName + ".txt";
				string str = rewrite_or_add ? "" : (File.Exists(path) ? File.ReadAllText(path) : "");
				File.WriteAllText(path, str + text);
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		public static void WriteTempFile(string fileName, string text, bool rewrite_or_add)
		{
			string path = Environment.GetEnvironmentVariable("tmp") + "\\" + fileName + ".txt";
			if (rewrite_or_add)
			{
				File.WriteAllText(path, text);
				return;
			}
			File.AppendAllText(path, text);
		}

		public static bool FileWriteSafe(string filePath, string text)
		{
			try
			{
				lock (filePath)
				{
					File.WriteAllText(filePath, text);
					return true;
				}
			}
			catch
			{
			}
			return false;
		}

		public static string ReadFile(string path)
		{
			if (File.Exists(path))
			{
				return File.ReadAllText(path);
			}
			return "";
		}

		public static string readFile(string filepath)
		{
			string result = "";
			if (File.Exists(filepath))
			{
				using (StreamReader streamReader = new StreamReader(filepath, Encoding.UTF8))
				{
					result = streamReader.ReadToEnd();
				}
			}
			return result;
		}


		public static bool createDir(string path)
		{
			bool result;
			try
			{
				result = (Directory.Exists(path) || Directory.CreateDirectory(path) != null);
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		public static bool deleteDir(string path)
		{
			bool result;
			try
			{
				if (!Directory.Exists(path))
				{
					result = false;
				}
				else
				{
					Directory.Delete(path);
					result = true;
				}
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		public static void list_all_files_in_dir(string base_path)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(base_path);
			FileInfo[] files = directoryInfo.GetFiles("*.*");
			string text = "";
			foreach (FileInfo fileInfo in files)
			{
				text = text + ", " + fileInfo.Name;
			}
			System.Windows.Forms.MessageBox.Show(text);
		}
		#endregion




		//Action<object,RoutedEventArgs,string>  x= (object sender, RoutedEventArgs e, string index )=>Print("foo") ;  
		//buttons[key].Click += (sender, EventArgs) => {  x(sender, EventArgs, (string) key.ToString()+" Hiii"); };



		// Assign the BitmapImage as the ImageSource of the ImageBrush
		// p.s. no need to download like : using (WebClient client = new WebClient())  { try{ 	client.DownloadFile(new Uri("http://tradeliveglobal.com/ninja/greenbutton.png"),   Path.GetTempPath()+@"greenbutton.png");

		/* Doesnt work in native C#4.5 without PresentationCore.dll
		public static System.Windows.Media.Imaging.BitmapImage SetBitmapForButton(ImageBrush imgBtn, string filePath)
		{
		    var bitmap = new System.Windows.Media.Imaging.BitmapImage();
		    bitmap.BeginInit();
		    bitmap.UriSource = new Uri(filePath);
		    bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
		    bitmap.EndInit();
			bitmap.Freeze(); // releases to thread 
			imgBtn.Dispatcher.Invoke( () => { imgBtn.ImageSource = bitmap; } );
		    return bitmap;
		}
		*/

		//public static string EmptyStringBreak(dynamic smth) { if(smth == null || !smth.GetType().GetProperties().Any()) { MessageBox.Show("empty value");); } }

		//public static bool XelementIsEmpty( System.Linq.XElement smth) { if (smth == null) return true; return false; }


		// https://ninjatrader.com/support/helpGuides/nt8/en-us/?alert_and_debug_concepts.htm
		//usage: ExceptionPrint(this, System.Reflection.MethodBase, e)
		//Print( "["+GetType().Name+"]["+System.Reflection.MethodBase.GetCurrentMethod().Name+"]-[ERROR at " + mymethodstt.Trace(e) +"] "+  e.ToString());



		public static string getDesktopPath()
		{
			return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		}




		#region Translation/Google
		public static string getLangFromIsoLang(string isoLang)
		{
			return new Regex("\\-(.*)", RegexOptions.IgnoreCase).Replace(isoLang, "");
		}


		public static void googleSetKeyFile(string FileContent)
		{
			string text = Environment.GetEnvironmentVariable("appdata") + "\\gsapk.xyz";
			setRegistryValue("gsapiKeyPath", text);
			File.WriteAllText(text, FileContent);
			SetGoogleCredentialsDefaults(text);
		}


		public static void SetGoogleCredentialsDefaults(string path)
		{
			Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
		}


		public static string selectedLang = "en";
		public static Dictionary<string, Dictionary<string, string>> transl_texts;
		public static string translate(string txt)
		{
			if (transl_texts.ContainsKey(txt) && transl_texts[txt].ContainsKey(selectedLang))
			{
				return transl_texts[txt][selectedLang];
			}
			return translatedReady(selectedLang, txt);
		}


		public static void setTexts(Dictionary<string, string> dict, string lang_2char, Form frm)
		{
			foreach (KeyValuePair<string, string> v in dict)
			{
				string keyNm_this = "trans_" + v.Value + "_" + lang_2char;    // keyname for that element
				string keyNm_ENG = "trans_" + v.Value + "_en";                // keyname for that element
				string val_this = getRegistryValue(keyNm_this);              // value from registry for element 
				string val_ENG = getRegistryValue(keyNm_ENG);              // value from registry for element 
				if (
					(String.IsNullOrEmpty(val_this))                            //if empty 
					||
					(!String.IsNullOrEmpty(val_ENG) && val_ENG != v.Key)     // or if english value differs from english stored value (happens when source was changed)
				)
				{
					if (lang_2char == "en")
					{
						val_this = v.Key;
					}
					else
					{
						val_this = GTranslate_checker(v.Key, lang_2char);
					}

					setRegistryValue(keyNm_this, val_this);
				}
				SetControlText(frm, v.Value, val_this);
			}
		}


		public static string translatedReady(string targetLang, string englishValue)
		{
			string text = englishValue;
			string key = "translReady_" + md5(englishValue) + "_" + targetLang;
			if (targetLang != "en")
			{
				string registryValue = getRegistryValue(key);
				if (registryValue == null)
				{
					text = GTranslate_checker(englishValue, targetLang);
					setRegistryValue(key, text);
				}
				else
				{
					text = registryValue;
				}
			}
			else
			{
				setRegistryValue(key, englishValue);
			}
			return text;
		}


		public static string GTranslate_checker(string what, string lang_target)
		{
			string result;
			if (MethodExists(this, "GTranslate_callback"))
			{
				Type typeFromHandle = typeof(Methods);
				MethodInfo method = typeFromHandle.GetMethod("GTranslate_callback", BindingFlags.Instance | BindingFlags.Public);
				result = (string)method.Invoke(this, new object[]
				{
					what,
					lang_target,
					"en"
				});
			}
			else
			{
				result = what;
			}
			return result;
		}


		private Dictionary<string, string> gtranslate_dict;
		public static string gTranslate(string what, string lang_target, bool useApi = false, string ApiURL = "")
		{
			string text = "en";
			string text2 = "";
			if (useApi)
			{
				string text3 = urlRead(string.Concat(new string[]
				{
					ApiURL,
					"&action=translate&lang=",
					lang_target,
					"&string[]=",
					urlEncode(what),
					DeveloperMachineAddition()
				}));
				//if (deserialize(text3, ref _dict) && _dict.ContainsKey(what))
				//text2 = _dict[what];
			}
			else
			{
				// https://stackoverflow.com/questions/26714426/what-is-the-meaning-of-google-translate-query-params
				string text3 = urlRead(string.Concat(new string[]
				{
					"https://translate.googleapis.com/translate_a/single?client=gtx&sl=",
					text,
					"&tl=",
					lang_target,
					"&dt=t&q=",
					urlEncode(what)
				}));
				Match match = Regex.Match(text3, "\\[\\[\\[\"(.*?)\",\"");
				if (match.Success)
				{
					text2 = match.Groups[1].Value;
					text2 = text2.Replace("\\", "");
				}
			}
			return text2;
		}
		#endregion











		public static bool IsDeveloperMachine()
		{
			return Environment.GetEnvironmentVariable(Methods.developerMachineString) != null;
		}

		public static string DeveloperMachineAddition()
		{
			if (Methods.IsDeveloperMachine())
			{
				return "&developerKey=" + Environment.GetEnvironmentVariable(Methods.developerMachineString).ToString();
			}
			return "";
		}



		public static string removeWhitespaces(string str)
		{
			return str.Replace("      ", " ").Replace("     ", " ").Replace("    ", " ").Replace("   ", " ").Replace("  ", " ").Replace(Environment.NewLine, "").Replace("\t", "");
		}


		#region debugger
		public static void m(int obj) { Methods.m(obj.ToString()); }
		public static void m(double obj) { Methods.m(obj.ToString()); }
		public static void m(bool obj) { Methods.m(obj.ToString()); }
		public static void m(Exception obj) { Methods.m(obj.ToString()); }
		public static void m(object obj) { Methods.m(obj.ToString()); }
		public static void m(object[] obj)
		{
			if (obj == null) { Methods.m("null"); return; }
			string text = ""; foreach (object obj2 in obj) text = text + ((obj2 == null) ? "null" : obj.ToString()) + Methods.nl_;
			Methods.m(text);
		}
		public static void m(string obj)
		{
			System.Windows.Forms.MessageBox.Show(new Form { TopMost = true }, (obj == null) ? "null" : obj.ToString());
		}

		public static void cw(int obj) { cw(obj.ToString()); }
		public static void cw(double obj) { cw(obj.ToString()); }
		public static void cw(bool obj) { cw(obj.ToString()); }
		public static void cw(Exception obj) { cw(obj.ToString()); }
		public static void cw(object obj) { cw(obj.ToString()); }
		public static void cw(object[] obj)
		{
			if (obj == null) { cw("null"); return; }
			string text = ""; foreach (object obj2 in obj) text = text + ((obj2 == null) ? "null" : obj.ToString()) + Methods.nl_;
			cw(text);
		}
		public static void cw(string obj)
		{
			Console.Write(obj == null ? "null" : obj.ToString());
		}



		// another kind of dumper (removed):  https://pastebin.com/KEzWthMp


		public static string dump(object obj) { return Methods.dump(obj, Methods.AllBindingFlags, false, 1, 1, ""); }
		public static string dump(object obj, int deep) { return Methods.dump(obj, Methods.AllBindingFlags, false, deep, 1, ""); }
		public static string dump(object obj, bool execMethods) { return Methods.dump(obj, Methods.AllBindingFlags, execMethods, 1, 1, ""); }
		public static string dump(object obj, int deep, bool execMethods) { return Methods.dump(obj, Methods.AllBindingFlags, execMethods, deep, 1, ""); }
		public static string dump(object obj, BindingFlags bindingFlags, bool execMethods) { return Methods.dump(obj, bindingFlags, execMethods, 1, 1, ""); }
		// ugly-fied
		private static string dump(object obj, BindingFlags bindingFlags, bool execMethods, int maxRecursiveDeep, int currentDeep, string prefix)
		{
			string text = prefix + " | ";
			string str = prefix + " -> ";
			string text2 = "";
			string newLine = Environment.NewLine;
			List<string> list = new List<string>();
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			string text3 = text2;
			text2 = string.Concat(new string[]
			{
				text3,
				newLine,
				text,
				(currentDeep == 1) ? ("<----------------- START (All Members : " + bindingFlags.ToString() + ") ----------------------->") : "-----------",
				newLine
			});
			string text4 = text2;
			text2 = string.Concat(new string[]
			{
				text4,
				"<--- OBJECT TYPE: ",
				obj.GetType().ToString(),
				" --->",
				newLine
			});
			try
			{
				foreach (MemberInfo memberInfo in Methods.GetMembersInclPrivateBase_static(obj.GetType(), bindingFlags))
				{
					string text5 = "__";
					string text6 = "__cant_detect1__";
					try
					{
						text5 = memberInfo.Name;
					}
					catch
					{
						text5 = "_name_not_detected_";
					}
					string text7 = memberInfo.MemberType.ToString();
					string text8 = Methods.nl_ + " ? " + str + text5;
					try
					{
						if (memberInfo.MemberType == MemberTypes.Field)
						{
							FieldInfo fieldInfo = (FieldInfo)memberInfo;
							object value = fieldInfo.GetValue(obj);
							if (value == null)
							{
								text6 = "null";
							}
							else
							{
								text6 = string.Concat(new string[]
								{
									value.ToString(),
									"   [",
									value.GetType().FullName,
									"]  <",
									Methods.AccessModifierType(fieldInfo),
									">"
								});
								if (!Methods.singleTypes.Contains(value.GetType().ToString()))
								{
									string text9 = Methods.tryEnumerabledString(value, text8);
									if (text9 != "")
									{
										text6 += text9;
									}
									else if (currentDeep < maxRecursiveDeep)
									{
										text6 += Methods.dump(value, bindingFlags, execMethods, maxRecursiveDeep, currentDeep + 1, text8);
									}
								}
							}
						}
						else if (memberInfo.MemberType == MemberTypes.Property)
						{
							PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
							object value2 = propertyInfo.GetValue(obj, null);
							if (value2 == null)
							{
								text6 = "null";
							}
							else
							{
								text6 = string.Concat(new string[]
								{
									value2.ToString(),
									"   [",
									value2.GetType().FullName,
									"]  <",
									Methods.AccessModifierType(propertyInfo),
									">"
								});
								if (!Methods.singleTypes.Contains(value2.GetType().ToString()))
								{
									string text10 = Methods.tryEnumerabledString(value2, text8);
									if (text10 != "")
									{
										text6 += text10;
									}
									else if (currentDeep < maxRecursiveDeep)
									{
										text6 += Methods.dump(value2, bindingFlags, execMethods, maxRecursiveDeep, currentDeep + 1, text8);
									}
								}
							}
						}
						else
						{
							if (memberInfo.MemberType == MemberTypes.Method)
							{
								MethodInfo methodInfo = (MethodInfo)memberInfo;
								string text11 = methodInfo.ReturnType.ToString();
								text6 = string.Concat(new string[] { "   [", text11.Replace("System.", ""), "]  (", Methods.tryEnumerabledString(methodInfo.GetParameters(), ", "), ")  <", Methods.AccessModifierType(methodInfo), ">" });
								if (!execMethods)
								{
									goto IL_52F;
								}
								string[] source = new string[]
								{ "System.Double", "System.Int32", "System.String", "System.Float", "System.Type" };
								if (!source.Contains(text11))
								{
									goto IL_52F;
								}
								try
								{
									object obj2 = methodInfo.Invoke(obj, null);
									if (obj2 != null)
									{
										object obj3 = text6;
										text6 = string.Concat(new object[] { obj3, "========", obj2.ToString(), "   [", obj2.GetType(), "]" });
									}
									goto IL_52F;
								}
								catch (Exception)
								{
									text6 += "--------------cant-Invoke";
									goto IL_52F;
								}
							}
							if (memberInfo.MemberType == MemberTypes.Constructor)
							{
								ConstructorInfo constructorInfo = (ConstructorInfo)memberInfo;
								ConstructorInfo left = constructorInfo;
								if (left == null)
								{
									text6 = "null";
								}
								else
								{
									ParameterInfo[] parameters = constructorInfo.GetParameters();
									text6 = string.Concat(new object[]
									{
										"params:",
										parameters.ToString(),
										"   [type:",
										parameters.GetType(),
										"]"
									});
								}
							}
							else if (memberInfo.MemberType == MemberTypes.Event)
							{
								EventInfo eventInfo = (EventInfo)memberInfo;
								EventInfo left2 = eventInfo;
								text6 = left2 == null ? "null" : "ToString: " + eventInfo.ToString();
							}
							else
							{
								text6 = "ToStringed: " + memberInfo.ToString();
							}
						}
					IL_52F:;
					}
					catch
					{
						text6 += "--------------cant-get-value";
					}
					string str2 = Methods.pChars(text5) + text6;
					string text12 = text + text7 + ":    " + str2;
					if (!list.Contains(text12))
					{
						list.Add(text12);
						text12 += newLine;
					}
					else
					{
						text12 = "";
					}
					if (!dictionary.ContainsKey(text7))
					{
						dictionary[text7] = new List<string>();
					}
					dictionary[text7].Add(text12);
				}
			}
			catch (Exception e)
			{
				text2 += Methods.ExceptionMessage(e, obj);
				Methods.m(text2);
			}
			string text13 = "";
			foreach (KeyValuePair<string, List<string>> keyValuePair in dictionary)
			{
				foreach (string str3 in (from q in keyValuePair.Value
										 orderby q
										 select q).ToList<string>())
				{
					text13 += str3;
				}
			}
			text2 += text13;
			text2 = text2 + text + ((currentDeep == 1) ? ("<----------------- END ---------------------->" + newLine + newLine) : "-----------") + newLine;
			return text2;
		}


		public static string AccessModifierType(object objInfo)
		{
			if (objInfo is FieldInfo)
			{
				FieldInfo fieldInfo = objInfo as FieldInfo;
				if (fieldInfo.IsPrivate) return "Private";
				if (fieldInfo.IsFamily) return "Protected";
				if (fieldInfo.IsAssembly) return "Internal";
				if (fieldInfo.IsPublic) return "Public";
			}
			else if (objInfo is PropertyInfo)
			{
				PropertyInfo propertyInfo = objInfo as PropertyInfo;
				if (propertyInfo.SetMethod == null)
				{
					return "GetOnly:" + Methods.AccessModifierType(propertyInfo.GetMethod);
				}
				if (propertyInfo.GetMethod == null)
				{
					return "SetOnly:" + Methods.AccessModifierType(propertyInfo.SetMethod);
				}
				return Methods.AccessModifierType(propertyInfo.GetMethod) + " & " + Methods.AccessModifierType(propertyInfo.GetMethod);
			}
			else if (objInfo is MethodInfo)
			{
				MethodInfo methodInfo = objInfo as MethodInfo;
				if (methodInfo.IsPrivate) return "Private";
				if (methodInfo.IsFamily) return "Protected";
				if (methodInfo.IsAssembly) return "Internal";
				if (methodInfo.IsPublic) return "Public";
			}
			return "Did not find access modifier";
		}

		public static string tryEnumerabledString(object obj, string prefix_)
		{
			string text = "";
			try
			{
				IEnumerable enumerable = obj as IEnumerable;
				if (enumerable != null)
				{
					bool flag = false;
					foreach (object obj2 in enumerable)
					{
						text = text + ((!flag && !prefix_.Contains(Methods.nl_)) ? "" : prefix_) + " ::: " + obj2.ToString();
						flag = true;
					}
				}
			}
			catch (Exception ex)
			{
				text = text + "[cant enumerate:" + ex.Message + "]";
			}
			return text;
		}

		// ===========================

		public static object callMethod(object o, string methodName, params object[] args)
		{
			MethodInfo method = o.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
			if (method != null)
			{
				return method.Invoke(o, args);
			}
			return null;
		}


		public static MemberInfo[] GetMembersInclPrivateBase_static(Type t, BindingFlags flags)
		{
			MemberInfo[] result;
			try
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.AddRange(t.GetMembers(flags));
				Type type = t;
				while ((type = type.BaseType) != null)
				{
					list.AddRange(type.GetMembers(flags));
				}
				result = list.ToArray();
			}
			catch (Exception)
			{
				result = new MemberInfo[0];
			}
			return result;
		}


		public static object propertyGet2(object obj_, string propName)
		{
			PropertyInfo property = obj_.GetType().GetProperty(propName);
			if (property != null)
			{
				return property.GetValue(obj_, null);
			}
			FieldInfo field = obj_.GetType().GetField(propName);
			if (field != null)
			{
				return field.GetValue(obj_);
			}
			return null;
		}


		// usage:  propertyGet<bool>(....)
		public static T propertyGet<T>(object obj_, string propName)
		{
			T result;
			try
			{
				PropertyInfo property = obj_.GetType().GetProperty(propName);
				if (property != null)
				{
					if (typeof(T) == typeof(string))
					{
						result = (T)((object)Convert.ChangeType((string)property.GetValue(obj_, null), typeof(T), CultureInfo.InvariantCulture));
					}
					else if (typeof(T) == typeof(bool))
					{
						result = (T)((object)Convert.ChangeType((bool)property.GetValue(obj_, null), typeof(T), CultureInfo.InvariantCulture));
					}
					else if (typeof(T) == typeof(Dictionary<string, string>))
					{
						result = (T)((object)Convert.ChangeType((Dictionary<string, string>)property.GetValue(obj_, null), typeof(T), CultureInfo.InvariantCulture));
					}
					else
					{
						result = (T)((object)Convert.ChangeType(property.GetValue(obj_, null), typeof(T), CultureInfo.InvariantCulture));
					}
				}
				else
				{
					FieldInfo field = obj_.GetType().GetField(propName);
					if (field != null)
					{
						if (typeof(T) == typeof(string))
						{
							result = (T)((object)Convert.ChangeType((string)field.GetValue(obj_), typeof(T), CultureInfo.InvariantCulture));
						}
						else if (typeof(T) == typeof(bool))
						{
							result = (T)((object)Convert.ChangeType((bool)field.GetValue(obj_), typeof(T), CultureInfo.InvariantCulture));
						}
						else if (typeof(T) == typeof(Dictionary<string, string>))
						{
							result = (T)((object)Convert.ChangeType((Dictionary<string, string>)field.GetValue(obj_), typeof(T), CultureInfo.InvariantCulture));
						}
						else
						{
							result = (T)((object)Convert.ChangeType(field.GetValue(obj_), typeof(T), CultureInfo.InvariantCulture));
						}
					}
					else
					{
						result = (T)((object)Convert.ChangeType(null, typeof(T), CultureInfo.InvariantCulture));
					}
				}
			}
			catch (Exception)
			{
				result = (T)((object)Convert.ChangeType("x", typeof(T), CultureInfo.InvariantCulture));
			}
			return result;
		}


		// older propertyset : https://pastebin.com/W3WUDsWg
		public static bool propertySet(object obj_, string propName, object value)
		{
			value.ToString();
			PropertyInfo propertyInfo = obj_.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault((PropertyInfo x) => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
			if (propertyInfo != null)
			{
				if (value is bool && ((bool)value || !(bool)value))
				{
					propertyInfo.SetValue(obj_, Convert.ToBoolean(value), null);
					return true;
				}
				if (Methods.isInt(value))
				{
					propertyInfo.SetValue(obj_, Convert.ToInt32(value), null);
					return true;
				}
				if (Methods.isDouble(value))
				{
					propertyInfo.SetValue(obj_, Convert.ToDouble(value), null);
					return true;
				}
				propertyInfo.SetValue(obj_, value);
			}
			FieldInfo fieldInfo = obj_.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault((FieldInfo x) => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
			if (propertyInfo != null)
			{
				if (value is bool && ((bool)value || !(bool)value))
				{
					fieldInfo.SetValue(obj_, Convert.ToBoolean(value));
					return true;
				}
				if (Methods.isInt(value))
				{
					fieldInfo.SetValue(obj_, Convert.ToInt32(value));
					return true;
				}
				if (Methods.isDouble(value))
				{
					fieldInfo.SetValue(obj_, Convert.ToDouble(value));
					return true;
				}
				fieldInfo.SetValue(obj_, value);
			}
			return false;
		}


		public static bool propertyExists(object obj_, string propName)
		{
			bool result;
			try
			{
				PropertyInfo left = obj_.GetType().GetProperties(Methods.AllBindingFlags).FirstOrDefault((PropertyInfo x) => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
				if (left != null)
				{
					result = true;
				}
				else
				{
					FieldInfo left2 = obj_.GetType().GetFields(Methods.AllBindingFlags).FirstOrDefault((FieldInfo x) => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
					if (left2 != null)
					{
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}


		public static bool propertySet_static(object obj_, string propName, string value)
		{
			bool result;
			try
			{
				PropertyInfo propertyInfo = obj_.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault((PropertyInfo x) => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
				if (propertyInfo != null)
				{
					if (value == "true" || value == "false")
					{
						propertyInfo.SetValue(obj_, Convert.ToBoolean(value), null);
						return true;
					}
					if (Methods.isInt(value))
					{
						propertyInfo.SetValue(obj_, Convert.ToInt32(value), null);
						return true;
					}
					if (Methods.isDouble(value))
					{
						propertyInfo.SetValue(obj_, Convert.ToDouble(value), null);
						return true;
					}
				}
				FieldInfo fieldInfo = obj_.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault((FieldInfo x) => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
				if (propertyInfo != null)
				{
					if (value == "true" || value == "false")
					{
						fieldInfo.SetValue(obj_, Convert.ToBoolean(value));
						return true;
					}
					if (Methods.isInt(value))
					{
						fieldInfo.SetValue(obj_, Convert.ToInt32(value));
						return true;
					}
					if (Methods.isDouble(value))
					{
						fieldInfo.SetValue(obj_, Convert.ToDouble(value));
						return true;
					}
				}
				result = false;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}


		public static bool propertyHideOrShow(object obj_, string propertyName, bool Show_or_Hide)
		{
			BrowsableAttribute browsableAttribute = TypeDescriptor.GetProperties(obj_.GetType())[propertyName].Attributes[typeof(BrowsableAttribute)] as BrowsableAttribute;
			FieldInfo field = browsableAttribute.GetType().GetField("Browsable", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);
			if (field != null)
			{
				field.SetValue(browsableAttribute, Show_or_Hide);
				return true;
			}
			return false;
		}


		// usage:  propertyGet<bool>(....)
		public static Type propertyType(object obj_, string propName)
		{
			PropertyInfo property = obj_.GetType().GetProperty(propName);
			if (property != null)
			{
				return property.GetType();
			}
			FieldInfo field = obj_.GetType().GetField(propName);
			if (field != null)
			{
				return field.GetType();
			}
			return null;
		}


		public static string PrintProperties(object obj)
		{
			return PrintProperties(obj, 0);
		}


		public static string PrintProperties(object obj, int indent)
		{
			string result;
			try
			{
				if (obj == null)
				{
					result = "";
				}
				else
				{
					string text = "";
					string text2 = Methods.Repeat("~", indent + 1);
					Type type = obj.GetType();
					foreach (PropertyInfo propertyInfo in type.GetProperties())
					{
						ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
						if (indexParameters.Count<ParameterInfo>() > 0 && !obj.ToString().Contains("+"))
						{
							object value = propertyInfo.GetValue(obj, null);
							if (propertyInfo.PropertyType.Assembly == type.Assembly && !propertyInfo.PropertyType.IsEnum)
							{
								text = text + text2 + propertyInfo.Name + ":";
								text += PrintProperties(value, indent + 2);
							}
							else
							{
								object obj2 = text;
								text = string.Concat(new object[]
								{
									obj2,
									text2,
									propertyInfo.Name,
									":",
									value
								});
							}
						}
					}
					result = text;
				}
			}
			catch (Exception)
			{
				result = "";
			}
			return result;
		}



		public static string DisplayObjectInfo(object o)
		{
			string result;
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				Type type = o.GetType();
				stringBuilder.Append("Type: " + type.Name);
				stringBuilder.Append("\r\n\r\nFields:");
				FieldInfo[] fields = type.GetFields();
				if (fields.Length > 0)
				{
					foreach (FieldInfo fieldInfo in fields)
					{
						stringBuilder.Append(string.Concat(new object[]
						{
							"\r\n ",
							fieldInfo.ToString(),
							" = ",
							fieldInfo.GetValue(o)
						}));
					}
				}
				else
				{
					stringBuilder.Append("\r\n None");
				}
				stringBuilder.Append("\r\n\r\nProperties:");
				PropertyInfo[] properties = type.GetProperties();
				if (properties.Length > 0)
				{
					foreach (PropertyInfo propertyInfo in properties)
					{
						stringBuilder.Append(string.Concat(new object[]
						{
							"\r\n ",
							propertyInfo.ToString(),
							" = ",
							propertyInfo.GetValue(o, null)
						}));
					}
				}
				else
				{
					stringBuilder.Append("\r\n None");
				}
				result = stringBuilder.ToString();
			}
			catch (Exception)
			{
				result = "";
			}
			return result;
		}

		#endregion

		// ======================= form functions ====================//

		/*
        private void RemoveEvents(System.Windows.Controls.Button b)
        {
            EventInfo[] info = typeof(System.Windows.Controls.Button).GetEvents(); //GetEvents(BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0; i < info.Length; i++)
            {
                FieldInfo f1 = typeof(System.Windows.Controls.Control).GetField(info[i].Name, BindingFlags.Static | BindingFlags.NonPublic);
                object obj = f1.GetValue(b);
                PropertyInfo pi = b.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
                EventHandlerList list = (EventHandlerList)pi.GetValue(b, null);
                list.RemoveHandler(obj, list[obj]);
            }

        }
        // RemoveRoutedEventHandlers(button, Button.ClickEvent);
        public static void RemoveRoutedEventHandlers(UIElement element, RoutedEvent routedEvent)
        {
            // Get the EventHandlersStore instance which holds event handlers for the specified element.
            // The EventHandlersStore class is declared as internal.
            var eventHandlersStoreProperty = typeof(UIElement).GetProperty(
                "EventHandlersStore", BindingFlags.Instance | BindingFlags.NonPublic);
            object eventHandlersStore = eventHandlersStoreProperty.GetValue(element, null);

            // If no event handlers are subscribed, eventHandlersStore will be null.
            // Credit: https://stackoverflow.com/a/16392387/1149773
            if (eventHandlersStore == null)
                return;

            // Invoke the GetRoutedEventHandlers method on the EventHandlersStore instance 
            // for getting an array of the subscribed event handlers.
            var getRoutedEventHandlers = eventHandlersStore.GetType().GetMethod(
                "GetRoutedEventHandlers", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var routedEventHandlers = (RoutedEventHandlerInfo[])getRoutedEventHandlers.Invoke(
                eventHandlersStore, new object[] { routedEvent });

            // Iteratively remove all routed event handlers from the element.
            foreach (var routedEventHandler in routedEventHandlers)
                element.RemoveHandler(routedEvent, routedEventHandler.Handler);
        }

		*/
		// ======================= form functions ====================//



		public static double NetFrameworkVersion()
		{
			double result;
			try
			{
				string version = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Where(x => x.Name == "System.Core").First().Version.ToString();
				result = Convert.ToDouble(version.Substring(0, version.IndexOf(".", 2)));
			}
			catch (Exception)
			{
				result = 0.0;
			}
			return result;
		}

		public static void NetFrameworkInstalledVersions()
		{
			string name = "SOFTWARE\\Microsoft\\NET Framework Setup\\NDP";
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name);
			string[] subKeyNames = registryKey.GetSubKeyNames();
			foreach (string text in subKeyNames)
			{
				Methods.m(text.ToString());
			}
		}


		public static bool isDouble(object Expression)
		{
			bool result;
			try
			{
				double num;
				result = double.TryParse(Convert.ToString(Expression), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out num);
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}


		public static bool isInt(object Expression)
		{
			bool result;
			try
			{
				string s = Expression.ToString();
				int num;
				result = int.TryParse(s, out num);
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}




		public static int decimalsAmount(double content)
		{
			string num = double_to_decimal(content).ToString();
			return (num.Length - num.IndexOf(".") - 1);
		}


		public static decimal double_to_decimal(double num)
		{
			//i.e "5E-07"  or  "1.2345E-02"
			return double_to_decimal_static(num);
		}
		public static decimal double_to_decimal_static(double num)
		{
			//i.e "5E-07"  or  "1.2345E-02" to correct
			return Decimal.Parse((num).ToString(), System.Globalization.NumberStyles.Float);
		}



		public static void DownloadFile(string url, string location)
		{
			using (WebClient client = new WebClient())
			{
				try
				{
					client.DownloadFile(new Uri(url), location);
				}
				catch (Exception e) { System.Windows.Forms.MessageBox.Show("Cant download " + url + "( " + e.Message + ")"); }
			}
		}


		public static string TempFile(string url)
		{
			Uri uri = new Uri(url);
			string tmp_path = convertBackSlashes(Path.GetTempPath() + @"\" + url.Replace(uri.Scheme, "").Replace("://", "").Replace("//", ""));
			if (!File.Exists(tmp_path))
			{
				string dir = Path.GetDirectoryName(tmp_path);
				if (!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}
				DownloadFile(url, tmp_path);
			}
			return tmp_path;
		}

		public static void errorlog(string message)
		{
		}







		public static int digitsPrecision(double tickSize)
		{
			int decimals = 0;
			if (tickSize == 1) { decimals = 0; }
			else if (tickSize == 0.5) { decimals = 1; }
			else if (tickSize == 0.25) { decimals = 1; }
			else if (tickSize == 0.1) { decimals = 1; }
			else if (tickSize == 0.01) { decimals = 2; }
			else if (tickSize == 0.001) { decimals = 3; }
			else if (tickSize == 0.005) { decimals = 3; }
			else if (tickSize == 0.025) { decimals = 3; }
			else if (tickSize == 0.03125) { decimals = 3; }
			else if (tickSize == 0.0001) { decimals = 4; }
			else if (tickSize == 1E-05) { decimals = 5; } //0.00001
			else if (tickSize == 5E-05) { decimals = 5; } //0.00005
			else if (tickSize == 1E-06) { decimals = 6; } //0.000001
			else if (tickSize == 1E-07) { decimals = 7; } //0.0000001
			else if (tickSize == 5E-07) { decimals = 7; } //0.0000005
			return decimals;
		}



		public static int rondTicksToDecimals(double content)
		{
			int response = -9999;
			if (content == 1) { response = 1; }
			else if (content == 0.5) { response = 2; }
			else if (content == 0.25) { response = 2; }
			else if (content == 0.1) { response = 2; }
			else if (content == 0.01) { response = 3; }
			else if (content == 0.001) { response = 3; }
			else if (content == 0.005) { response = 3; }
			else if (content == 0.025) { response = 3; }
			else if (content == 0.03125) { response = 5; }
			else if (content == 0.0001) { response = 4; }
			else if (content == 0.00001) { response = 5; }
			else if (content == 0.00005) { response = 5; }
			else if (content == 0.000001) { response = 6; }
			else if (content == 0.000005) { response = 6; }
			else if (content == 0.0000001) { response = 7; }
			else if (content == 0.0000005) { response = 7; }
			return response;


			if (content == 1) { response = 99; }
			else if (content == 0.5) { response = 99; }
			else if (content == 0.25) { response = 99; }
			else if (content == 0.1) { response = 99; }
			else if (content == 0.01) { response = 0; }
			else if (content == 0.001) { response = 1; }
			else if (content == 0.005) { response = 0; }
			else if (content == 0.025) { response = 0; }
			else if (content == 0.03125) { response = 0; }
			else if (content == 0.0001) { response = 2; }
			else if (content == 0.00001) { response = 3; }
			else if (content == 0.00005) { response = 3; }
			else if (content == 0.000001) { response = 4; }
			else if (content == 0.0000001) { response = 5; }
			else if (content == 0.0000005) { response = 5; }
			return response;
		}

		public static int AfterPointNumberOfDigits(double tickSize)
		{
			double t = tickSize;
			int i = 0;
			while (t != Math.Round(t, 0)) //better to use ApproxCompare, because of double in-precision
			{
				t = t * 10;
				i++;
			}

			return (i);
		}

		public static string roundTo(double num, double tickSize)
		{
			int digits = digitsAfterDot(tickSize);
			return num.ToString("N" + digits);
		}

		// C:\Windows\media    :   Windows Information Bar, Windows Ringout, Speech Sleep, Speech On, Windows Notify Calendar, Windows Unlock, tada, Alarm03, Alarm02, Ring02, notify, chimes
		//good: ding chimes chord

		public static string defaultSound = "Speech On";

		public static bool playSound(string path)
		{
			bool result;
			try
			{
				if (!path.Contains("."))
				{
					path = "C:\\Windows\\media\\" + path + ".wav";
				}
				SoundPlayer soundPlayer = new SoundPlayer(path);
				soundPlayer.Play();
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		public static void PopupForm_(string message)
		{
			System.Windows.Forms.Form form1 = new Form();
			form1.Width = 250;
			form1.Height = 130;
			//logForm.Resize  += resizeTextarea  ; 
			form1.Text = @"Alert";
			form1.TopMost = true;
			form1.TopLevel = true;
			form1.Opacity = 0.95;
			form1.StartPosition = FormStartPosition.CenterScreen;
			//
			System.Windows.Forms.TextBox textarea = new System.Windows.Forms.TextBox();
			form1.Controls.Add(textarea);
			textarea.Multiline = true;
			textarea.Width = textarea.Parent.Width;
			textarea.Height = textarea.Parent.Height;
			textarea.Text = message;
			form1.Show();
			//textarea.Font = new SimpleFont(textarea.Font.FontFamily, 11);
		}
		public static void destroyForm(bool dispose)
		{
			//if(logForm != null) logForm.Close();
			//if (dispose) logForm.Dispose();
			//logForm = null;
		}



		#region Log 

		public static void enableExceptions()
		{
			enableExceptions(false);
		}


		public static void enableExceptions(bool show = false)
		{
			AppDomain.CurrentDomain.FirstChanceException += delegate (object sender, FirstChanceExceptionEventArgs eventArgs)
			{
				StackTrace stackTrace = new StackTrace();
				MethodBase method = stackTrace.GetFrame(1).GetMethod();
				string text = Methods.ExceptionMessage(eventArgs.Exception, method, "");
				if (show || Methods.IsDeveloperMachine())
				{
					Methods.m(text);
					return;
				}
				log(text);
			};
		}

		public static void log(string text)
		{
			string location = Path.GetTempPath() + "_default_logs.txt"; //programName
			log(text, location);
		}

		public static void log(string text, string location)
		{
			text = string.Concat(new string[]
			{
				nl , "______________   ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"), "   ______________", nl, text
			});
			// delete if above 2mb
			if (new FileInfo(location).Length > 2000000L)
			{
				File.Delete(location);
			}
			if (!File.Exists(location))
			{
				File.WriteAllText(location, "");
			}
			File.AppendAllText(location, text);
		}

		[STAThread]
		public static void catchAllExceptions()
		{
			AppDomain.CurrentDomain.UnhandledException += Methods.HandleUnhandledException;
		}


		public static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (!Methods.inCatchAllLoop)
			{
				Methods.inCatchAllLoop = true;
				Methods.m(sender);
				Methods.inCatchAllLoop = false;
			}
		}


		public static void logError(object obj_)
		{
			FileOperations.Write(errorLogFile, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine + obj_.ToString() + Environment.NewLine);
		}

		public static bool isDeveloperMode(string key)
		{
			try
			{
				string path = "c:\\debug_c#_developer_mode.txt";
				return File.Exists(path) && File.ReadAllText(path).Contains(key + ",");
			}
			catch
			{
			}
			return false;
		}

		public static string ExceptionForDeveloper(Exception e, object targetObj)
		{
			string text = "";
			string result;
			try
			{
				text += Methods.ExceptionMessage(e, targetObj);
				if (Methods.isDeveloperMode("Exceptions"))
				{
					Methods.m(text);
				}
				Methods.FileWriteSafe(Methods.tmpDir + "_lastError_" + ((targetObj == null || targetObj.GetType() == null) ? "unknown" : targetObj.GetType().Name), text);
				result = text;
			}
			catch (Exception ex)
			{
				result = string.Concat(new string[]
				{
					"problem in ExceptionForDeveloper method. ",
					text,
					ex.Message,
					Methods.nl_,
					e.Message
				});
			}
			return result;
		}


		public static string ExceptionMessage(Exception e, object targetObj)
		{
			return Methods.ExceptionMessage(e, targetObj, "");
		}


		public static string ExceptionMessage(Exception e, object obj_, string customMessage)
		{
			string text = "";
			string text2 = Methods.nl_ + "_________________" + Methods.nl_;
			StackFrame frame = new StackTrace(e, true).GetFrame(0);
			int fileLineNumber = frame.GetFileLineNumber();
			int fileColumnNumber = frame.GetFileColumnNumber();
			string fileName = frame.GetFileName();
			text = text + "======================" + Methods.nl_;
			if (e.Source != null)
			{
				text = text + "• Source\t\t\t\t: " + e.Source.ToString() + text2;
			}
			if (obj_ != null && obj_.GetType() != null)
			{
				text = text + "• Target object\t\t: " + obj_.GetType().FullName + text2;
			}
			text += "• Method\t\t\t\t: ";
			IEnumerable<StackFrame> enumerable = new StackTrace(e, true).GetFrames().Reverse<StackFrame>();
			foreach (StackFrame stackFrame in enumerable)
			{
				MethodBase method = stackFrame.GetMethod();
				text = text + " > " + ((method != null) ? method.Name : "method_name_not_detected");
			}
			text += text2;
			string text3 = text;
			text = string.Concat(new string[]
			{
				text3,
				"• Line_Column\t\t: ",
				(fileLineNumber != 0) ? fileLineNumber.ToString() : "not detected",
				" : ",
				(fileColumnNumber != 0) ? fileColumnNumber.ToString() : ""
			});
			if (fileLineNumber != 0)
			{
				try
				{
					if (fileName.EndsWith(".cs"))
					{
						string text4 = File.ReadLines(fileName).Skip(fileLineNumber - 1).Take(1).First<string>();
						if (string.IsNullOrEmpty(text4))
						{
							return text4;
						}
						text4 = text4.Substring(fileColumnNumber - 1, Math.Min(text4.Length, 30));
						text = text + "--->   " + text4;
					}
				}
				catch
				{
				}
			}
			text += text2;
			text = text + "• Message\t\t\t: " + e.Message + text2;
			if (e.InnerException != null)
			{
				text = text + "• InnerException\t\t: " + e.InnerException.Message + text2;
			}
			text = text + "• User_message\t\t: " + customMessage + text2;
			string text5 = text;
			text = string.Concat(new string[]
			{
				text5, "• StackTrace\t\t\t: ", Methods.nl_, " ----- ", e.StackTrace, text2
			});
			text = text + "======================" + Methods.nl_;
			return text;
		}

		#endregion



		public static string appName()
		{
			string location = Assembly.GetEntryAssembly().Location;
			return Path.GetFileNameWithoutExtension(location);
		}

		public static void ExecuteCommand(string command)
		{
			try
			{
				new Process
				{
					StartInfo = new ProcessStartInfo
					{
						FileName = "cmd.exe",
						Arguments = command
					}
				}.Start();
			}
			catch (Exception)
			{
			}
		}






		public static string tmpFilePath = Environment.GetEnvironmentVariable("tmp") + "\\";
		public static string pathDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);


		public static string nl = Environment.NewLine;

		public static string[] singleTypes = new string[] { "System.Double", "System.Int32", "System.String", "System.Float", "System.Boolean" };


		internal static BindingFlags AllBindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty | BindingFlags.ExactBinding | BindingFlags.SuppressChangeType | BindingFlags.OptionalParamBinding | BindingFlags.IgnoreReturn;

		internal static BindingFlags AllBindingFlags_noinherit = BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty | BindingFlags.ExactBinding | BindingFlags.SuppressChangeType | BindingFlags.OptionalParamBinding | BindingFlags.IgnoreReturn;

		public static bool inCatchAllLoop = false;
		//private static object fileCheckObj = new object(); 
		//public static string errorLogFile = Environment.GetEnvironmentVariable("tmp") + "\\_errorlogs_c#.log"; 
		public static string nl_ = Environment.NewLine;
		public static string tmpDir = Environment.GetEnvironmentVariable("tmp") + "\\";



		// https://stackoverflow.com/questions/14967618/deserialize-json-to-class-manually-with-reflection/50492864#50492864
		//new update here, but doesnt work well : http://qaru.site/questions/6250657/deserialize-json-to-class-manually-with-reflection
		public static class JsonMaker
		{
			public static Dictionary<string, object> ParseJSON(string json)
			{
				int end;
				return ParseJSON(json, 0, out end);
			}
			private static Dictionary<string, object> ParseJSON(string json, int start, out int end)
			{
				Dictionary<string, object> dict = new Dictionary<string, object>();
				bool escbegin = false;
				bool escend = false;
				bool inquotes = false;
				string key = null;
				int cend;
				StringBuilder sb = new StringBuilder();
				Dictionary<string, object> child = null;
				List<object> arraylist = null;
				Regex regex = new Regex(@"\\u([0-9a-z]{4})", RegexOptions.IgnoreCase);
				int autoKey = 0;
				bool inSingleQuotes = false;
				bool inDoubleQuotes = false;
				for (int i = start; i < json.Length; i++)
				{
					char c = json[i];
					if (c == '\\') escbegin = !escbegin;
					if (!escbegin)
					{
						if (c == '"' && !inSingleQuotes)
						{
							inDoubleQuotes = !inDoubleQuotes;
							inquotes = !inquotes;
							if (!inquotes && arraylist != null)
							{
								arraylist.Add(DecodeString(regex, sb.ToString()));
								sb.Length = 0;
							}
							continue;
						}
						else if (c == '\'' && !inDoubleQuotes)
						{
							inSingleQuotes = !inSingleQuotes;
							inquotes = !inquotes;
							if (!inquotes && arraylist != null)
							{
								arraylist.Add(DecodeString(regex, sb.ToString()));
								sb.Length = 0;
							}
							continue;
						}
						if (!inquotes)
						{
							switch (c)
							{
								case '{':
									if (i != start)
									{
										child = ParseJSON(json, i, out cend);
										if (arraylist != null) arraylist.Add(child);
										else
										{
											dict.Add(key.Trim(), child);
											key = null;
										}
										i = cend;
									}
									continue;
								case '}':
									end = i;
									if (key != null)
									{
										if (arraylist != null) dict.Add(key.Trim(), arraylist);
										else dict.Add(key.Trim(), DecodeString(regex, sb.ToString().Trim()));
									}
									return dict;
								case '[':
									arraylist = new List<object>();
									continue;
								case ']':
									if (key == null)
									{
										key = "array" + autoKey.ToString();
										autoKey++;
									}
									if (arraylist != null && sb.Length > 0)
									{
										arraylist.Add(sb.ToString());
										sb.Length = 0;
									}
									dict.Add(key.Trim(), arraylist);
									arraylist = null;
									key = null;
									continue;
								case ',':
									if (arraylist == null && key != null)
									{
										dict.Add(key.Trim(), DecodeString(regex, sb.ToString().Trim()));
										key = null;
										sb.Length = 0;
									}
									if (arraylist != null && sb.Length > 0)
									{
										arraylist.Add(sb.ToString());
										sb.Length = 0;
									}
									continue;
								case ':':
									key = DecodeString(regex, sb.ToString());
									sb.Length = 0;
									continue;
							}
						}
					}
					sb.Append(c);
					if (escend) escbegin = false;
					if (escbegin) escend = true;
					else escend = false;
				}
				end = json.Length - 1;
				return dict; //theoretically shouldn't ever get here
			}

			private static string DecodeString(Regex regex, string str)
			{
				return Regex.Unescape(regex.Replace(str, match => char.ConvertFromUtf32(Int32.Parse(match.Groups[1].Value, System.Globalization.NumberStyles.HexNumber))));
			}
		}







		public static class JsonHelper
		{
			/// <summary>
			/// Parses the JSON.
			/// Thanks to http://stackoverflow.com/questions/14967618/deserialize-json-to-class-manually-with-reflection
			/// </summary>
			/// <param name="json">The json.</param>
			/// <returns></returns>
			public static Dictionary<string, object> DeserializeJson(string json)
			{
				int end;
				return DeserializeJson(json, 0, out end);
			}

			/// <summary>
			/// Parses the JSON.
			/// </summary>
			/// <param name="json">The json.</param>
			/// <param name="start">The start.</param>
			/// <param name="end">The end.</param>
			/// <returns></returns>
			private static Dictionary<string, object> DeserializeJson(string json, int start, out int end)
			{

				if (!IsJson(json))
				{
					end = 0;
					return null;
				}

				var dict = new Dictionary<string, object>();
				var escbegin = false;
				var escend = false;
				var inquotes = false;
				string key = null;
				var sb = new StringBuilder();
				List<object> arraylist = null;
				var regex = new Regex(@"\\u([0-9a-z]{4})", RegexOptions.IgnoreCase);
				var autoKey = 0;
				var inSingleQuotes = false;
				var inDoubleQuotes = false;

				for (var i = start; i < json.Length; i++)
				{
					var c = json[i];
					if (c == '\\') escbegin = !escbegin;
					if (!escbegin)
					{
						if (c == '"' && !inSingleQuotes)
						{
							inDoubleQuotes = !inDoubleQuotes;
							inquotes = !inquotes;
							if (!inquotes && arraylist != null)
							{
								arraylist.Add(DecodeString(regex, sb.ToString()));
								sb.Length = 0;
							}

							continue;
						}

						if (c == '\'' && !inDoubleQuotes)
						{
							inSingleQuotes = !inSingleQuotes;
							inquotes = !inquotes;
							if (!inquotes && arraylist != null)
							{
								arraylist.Add(DecodeString(regex, sb.ToString()));
								sb.Length = 0;
							}

							continue;
						}

						if (!inquotes)
						{
							switch (c)
							{
								case '{':
									if (i != start)
									{
										int cend;
										var child = DeserializeJson(json, i, out cend);
										if (arraylist != null)
										{
											arraylist.Add(child);
										}
										else
										{
											dict.Add(key.Trim(), child);
											key = null;
										}

										i = cend;
									}

									continue;

								case '}':
									end = i;

									if (key != null)
									{
										if (arraylist != null) dict.Add(key.Trim(), arraylist);
										else dict.Add(key.Trim(), DecodeString(regex, sb.ToString().Trim()));
									}

									return dict;

								case '[':
									arraylist = new List<object>();
									continue;

								case ']':
									if (key == null)
									{
										key = "array" + autoKey;
										autoKey++;
									}

									if (arraylist != null && sb.Length > 0)
									{
										arraylist.Add(sb.ToString());
										sb.Length = 0;
									}

									dict.Add(key.Trim(), arraylist);
									arraylist = null;
									key = null;
									continue;

								case ',':
									if (arraylist == null && key != null)
									{
										dict.Add(key.Trim(), DecodeString(regex, sb.ToString().Trim()));
										key = null;
										sb.Length = 0;
									}

									if (arraylist != null && sb.Length > 0)
									{
										arraylist.Add(sb.ToString());
										sb.Length = 0;
									}

									continue;

								case ':':
									key = DecodeString(regex, sb.ToString());
									sb.Length = 0;

									continue;
							}
						}
					}

					sb.Append(c);

					if (escend) escbegin = false;
					escend = escbegin;
				}

				end = json.Length - 1;
				return dict; // theoretically shouldn't ever get here
			}

			/// <summary>
			/// Decodes the string.
			/// </summary>
			/// <param name="regex">The regex.</param>
			/// <param name="str">The STR.</param>
			/// <returns></returns>
			private static string DecodeString(Regex regex, string str)
			{
				return
					Regex.Unescape(regex.Replace(str,
						match =>
							char.ConvertFromUtf32(Int32.Parse(match.Groups[1].Value,
								System.Globalization.NumberStyles
									.HexNumber))));
			}

			/// <summary>
			/// Returns true if string has an "appearance" of being JSON-like
			/// </summary>
			/// <param name="input"></param>
			/// <returns></returns>
			public static bool IsJson(string input)
			{
				input = input.Trim();
				return input.StartsWith("{") && input.EndsWith("}") || input.StartsWith("[") && input.EndsWith("]");
			}
		}







		/*
        public class DynamicJsonObject : System.Dynamic.DynamicObject
        {
            private readonly IDictionary<string, object> _dictionary;

            public DynamicJsonObject(IDictionary<string, object> dictionary)
            {
                if (dictionary == null)
                    throw new ArgumentNullException("dictionary");
                _dictionary = dictionary;
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                ToString(sb);
                return sb.ToString();
            }

            private void ToString(StringBuilder sb)
            {
                sb.Append("{");
                var firstInDictionary = true;
                foreach (var pair in _dictionary)
                {
                    if (!firstInDictionary)
                        sb.Append(",");
                    firstInDictionary = false;
                    var value = pair.Value;
                    var name = pair.Key;
                    if (value is string)
                    {
                        sb.AppendFormat("\"{0}\":\"{1}\"", name, value);
                    }
                    else if (value is IDictionary<string, object>)
                    {
                        sb.AppendFormat("\"{0}\":", name);
                        new DynamicJsonObject((IDictionary<string, object>)value).ToString(sb);
                    }
                    else if (value is ArrayList)
                    {
                        sb.Append("\"");
                        sb.Append(name);
                        sb.Append("\":[");
                        var firstInArray = true;
                        foreach (var arrayValue in (ArrayList)value)
                        {
                            if (!firstInArray)
                                sb.Append(",");
                            firstInArray = false;
                            if (arrayValue is IDictionary<string, object>)
                                new DynamicJsonObject((IDictionary<string, object>)arrayValue).ToString(sb);
                            else if (arrayValue is string)
                                sb.AppendFormat("\"{0}\"", arrayValue);
                            else
                                sb.AppendFormat("{0}", arrayValue);

                        }
                        sb.Append("]");
                    }
                    else
                    {
                        sb.AppendFormat("\"{0}\":{1}", name, value);
                    }
                }
                sb.Append("}");
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if (!_dictionary.TryGetValue(binder.Name, out result))
                {
                    // return null to avoid exception.  caller can check for null this way...
                    result = null;
                    return true;
                }

                var dictionary = result as IDictionary<string, object>;
                if (dictionary != null)
                {
                    result = new DynamicJsonObject(dictionary);
                    return true;
                }

                var arrayList = result as ArrayList;
                if (arrayList != null && arrayList.Count > 0)
                {
                    if (arrayList[0] is IDictionary<string, object>)
                        result = new List<object>(arrayList.Cast<IDictionary<string, object>>().Select(x => new DynamicJsonObject(x)));
                    else
                        result = new List<object>(arrayList.Cast<object>());
                }

                return true;
            }
        }
        */







		/*


				// %appdata%\Microsoft\Default
				// registry checkings
				// check if PROTOCOL url exists 
				// RegistryKey rKey = Registry.ClassesRoot.OpenSubKey(GloballlTsVar, false);
				// if (rKey != null)   {  RegistryKey reg = Registry.ClassesRoot.OpenSubKey(@"tossc\shell\open\command").GetValue(null).ToString();  }

				// regex samples: https://pastebin.com/raw/SCVaaiJ7
				//ExecuteCommand(@"/c start tossc:" + decodedString);






				//in main
				//ShowInTaskbar = false;
				//WindowState = FormWindowState.Minimized;
				//Load += new EventHandler(Window_Load);



				// public static bool setRegistryValue1(string parentKey, string key)
				// {
				//     registryEntry = registryBase.CreateSubKey(keyPath,
				//     RegistryKeyPermissionCheck.ReadWriteSubTree);
				//     registryEntry.SetValue(valueName, newValue);
				// }









				/*
				private void copyBat()
				{
					try
					{
						string source_dir = "c:\\Debug";
						string destination_dir = "C:\\Users\\pc\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\Startup";

						if (!System.IO.Directory.Exists(destination_dir))
						{
							System.IO.Directory.CreateDirectory(destination_dir);
						}

						// Create subdirectory structure in destination    
						foreach (string dir in Directory.GetDirectories(source_dir, "*", System.IO.SearchOption.AllDirectories))
						{
							Directory.CreateDirectory(destination_dir + dir.Substring(source_dir.Length));

						}

						foreach (string file_name in Directory.GetFiles(source_dir, "*.*", System.IO.SearchOption.AllDirectories))
						{
							File.Copy(file_name, destination_dir + file_name.Substring(source_dir.Length), true);
						}
					}

					catch (Exception e)
					{
						MessageBox.Show(e.Message, "HATA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}

				}           
				*/

		/*


				 if(x) {
		 result += nl + "------Attributes------" + nl;
		 //Type t = typeof(    );
		 Type t= obj.GetType();
		 result += "Att Members of  " + t.FullName + nl;
		 foreach (MemberInfo prop in t.GetMembers())
		 {
			 bool hasAttribute = false;
			 result +=  prop.Name + nl;
			 if (prop.GetCustomAttributes(true).Length > 0) {
				 result += "["+ prop.MemberType.ToString() +"]    " +  prop.MemberType + nl; 
			 }
		 }
					 }




 // cmd.StandardInput.WriteLine("powershell (Invoke-WebRequest http://ipinfo.io/ip).Content >> %filepath%");



 //  var dialogResult = MessageBox.Show("Message", "Title", MessageBoxButtons.OKCancel);
 //if (dialogResult == System.Windows.Forms.DialogResult.OK)
 //    MessageBox.Show("OK Clicked");
 //else
 //    MessageBox.Show("Cancel Clicked");



		 // [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0

 //  c# 4.5 --->  public static void ExceptionMessage(Exception e, object obj_, System.Reflection.MethodBase method, string msg, [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0) 


 // ObjectDumper.Dump(obj); https://stackoverflow.com/questions/852181/c-printing-all-properties-of-an-object
 public static string dump(object obj) { return ObjectDumper.Dump(obj); }
 public class ObjectDumper { private int _level; private readonly int _indentSize; private readonly StringBuilder _stringBuilder; private readonly List<int> _hashListOfFoundElements; private ObjectDumper(int indentSize) { _indentSize = indentSize; _stringBuilder = new StringBuilder(); _hashListOfFoundElements = new List<int>(); } public static string Dump(object element) { return Dump(element, 2); } public static string Dump(object element, int indentSize) { var instance = new ObjectDumper(indentSize); return instance.DumpElement(element); } private string DumpElement(object element) { if (element == null || element is ValueType || element is string) { Write(FormatValue(element)); } else { var objectType = element.GetType(); if (!typeof(IEnumerable).IsAssignableFrom(objectType)) { Write("{{{0}}}", objectType.FullName); _hashListOfFoundElements.Add(element.GetHashCode()); _level++; } var enumerableElement = element as IEnumerable; if (enumerableElement != null) { foreach (object item in enumerableElement) { if (item is IEnumerable && !(item is string)) { _level++; DumpElement(item); _level--; } else { if (!AlreadyTouched(item)) DumpElement(item); else Write("{{{0}}} <-- bidirectional reference found", item.GetType().FullName); } } } else { MemberInfo[] members = element.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance); foreach (var memberInfo in members) { var fieldInfo = memberInfo as FieldInfo; var propertyInfo = memberInfo as PropertyInfo; if (fieldInfo == null && propertyInfo == null) continue; var type = fieldInfo != null ? fieldInfo.FieldType : propertyInfo.PropertyType; object value = fieldInfo != null ? fieldInfo.GetValue(element) : propertyInfo.GetValue(element, null); if (type.IsValueType || type == typeof(string)) { Write("{0}: {1}", memberInfo.Name, FormatValue(value)); } else { var isEnumerable = typeof(IEnumerable).IsAssignableFrom(type); Write("{0}: {1}", memberInfo.Name, isEnumerable ? "..." : "{ }"); var alreadyTouched = !isEnumerable && AlreadyTouched(value); _level++; if (!alreadyTouched) DumpElement(value); else Write("{{{0}}} <-- bidirectional reference found", value.GetType().FullName); _level--; } } } if (!typeof(IEnumerable).IsAssignableFrom(objectType)) { _level--; } } return _stringBuilder.ToString(); } private bool AlreadyTouched(object value) { if (value == null) return false; var hash = value.GetHashCode(); for (var i = 0; i < _hashListOfFoundElements.Count; i++) { if (_hashListOfFoundElements[i] == hash) return true; } return false; } private void Write(string value, params object[] args) { var space = new string(' ', _level * _indentSize); if (args != null) value = string.Format(value, args); _stringBuilder.AppendLine(space + value); } private string FormatValue(object o) { if (o == null) return ("null"); if (o is DateTime) return (((DateTime)o).ToShortDateString()); if (o is string) return string.Format("\"{0}\"", o); if (o is char && (char)o == '\0') return string.Empty; if (o is ValueType) return (o.ToString()); if (o is IEnumerable) return ("..."); return ("{ }"); } }


*/


		//64 bit detection code:  https://pastebin.com/raw/UdhUteE3





	}
}









namespace PuvoxLibrary
{
	
	public static class FileOperations
	{
		
		public static void Write(string path, object obj_)
		{
			try
			{
				FileOperations.locker.AcquireWriterLock(100);
				File.WriteAllText(path, obj_.ToString());
			}
			catch (Exception)
			{
			}
			finally
			{
				FileOperations.locker.ReleaseWriterLock();
			}
		}

		
		public static void Hide(string path)
		{
			try
			{
				FileOperations.locker.AcquireWriterLock(100);
				FileInfo fileInfo = new FileInfo(path);
				if (!fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
				{
					FileAttributes attributes = File.GetAttributes(path);
					File.SetAttributes(path, attributes | FileAttributes.Hidden);
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				FileOperations.locker.ReleaseWriterLock();
			}
		}

		public static bool setAttribute(object obj_, string propertyName, string attributeName, object val_)
        {
            // doesnt work
            var attrs = obj_.GetType()
              .GetProperty(propertyName)
              .GetCustomAttributes(false)
              .ToDictionary(a => a.GetType().Name, a => a);
            return true;

            //var ba = (attrs["BrowsableAttribute"] as BrowsableAttribute);
            //var isBrowsable = ba.GetType().GetField("Browsable", System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            //if (isBrowsable != null) { isBrowsable.SetValue(ba, (bool)val_); return true; }
            //return false;

        }
		public static void Write(string path, object obj_, bool hiddenFile)
		{
			FileOperations.Write(path, obj_);
			if (hiddenFile)
			{
				FileOperations.Hide(path);
			}
		}

		
		private static ReaderWriterLock locker = new ReaderWriterLock();
	}
}

