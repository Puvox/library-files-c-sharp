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
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Win32;
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

#region Program properties
namespace PuvoxLibrary
{ 
	public class Program
	{
		public void defaultInit(string companyName_, string domain_, string title, string slug, double version, string language, string productUrl)
		{
			this.baseCompanyName = companyName_;
			this.baseDomain = domain_;
			this.Name = title;
			this.Slug = slug;
			this.Version = version;
			this.Language = language;
			this.BaseProductUrl = productUrl;
			this.RegRootOfThisProg = string.Concat(new string[]{"SOFTWARE\\",this.baseCompanyName,"\\",	this.Slug,"\\"});
			this.responseUrl = this.ApiURL + "&action=check&licensekey=" + this.licensekeyGet();
			this.contactUrl = this.baseDomain + this.baseContactPath;
			this.initialized = true;
		}
 
		public string ApiURL
		{
			get
			{
				return string.Concat(new object[]{ this.baseDomain,	this.baseResponsePath, "program=", this.UrlEncode(this.Slug), "&version=", this.Version	});
			}
		}
			
			
		
		public bool licensekeySet(string key)
		{
			this.setRegistryValue("licensekey", key);
			return true;
		}

		
		public string licensekeyGet()
		{
			return this.getSetRegistryValue("licensekey", "demo_" + this.RandomString(32));
		}

		public bool isDemo
		{
			get
			{
				return this.licensekeyGet().Contains("demo_");
			}
		}
		
		
		public Dictionary<string, string> MainResponse
		{
			get
			{
				this.getResponseData(false, 3);
				return this.mainResponse;
			}
			set
			{
				this.mainResponse = value;
			}
		}


		public bool getResponseData(bool forceNew, int cachedMinutes_)
		{
			bool flag = false;
			if (this.mainResponse == null)
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
				if (DateTime.Now.Subtract(TimeSpan.FromMinutes((double)cachedMinutes_)) < DateTime.Parse(this.getSetRegistryValue("last_resp_time", DateTime.Now.ToString())))
				{
					text = this.getRegistryValue("last_resp_data");
				}
				else
				{
					text = this.readUrl(this.responseUrl);
					this.setRegistryValue("last_resp_time", DateTime.Now.ToString());
					this.setRegistryValue("last_resp_data", text);
				}
				if (text == "-1")
				{
					return false;
				}
				this.mainResponse_string = text;
				Dictionary<string, string> dictionary = this.deserialize(text);
				string str = (dictionary["enchint"] == "") ? dictionary["data"] : this.DecryptString(dictionary["data"], this.Slug + dictionary["enchint"] + this.Version.ToString());
				this.mainResponse = this.deserialize(str);
			}
			return true;
		}

		
		internal bool licenseAllowed()
		{
			return this.MainResponse["status"] == "200";
		}

		private bool isDevelopment { get { return IsDeveloperMachine(); } }
	
		public bool updateAvailable()
		{
			bool result;
			try
			{
				result = (this.mainResponse["version"].ToString() != this.Version.ToString());
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		
	}
}
#endregion


namespace PuvoxLibrary
{
	public class Methods
	{
		private string ProgramName_ ="";
		public string ProgramName { get { if (ProgramName_=="") { m("Please set .ProgramName property ( for "+ typeof(MyClass).Name +"), otherwise Library methods can't function normally."); }  return ProgramName_; } set{ProgramName_ = value;} }
		
		public string Language;
		public string Name;
		public string Slug;
		public double Version;
		public string BaseProductUrl;
		public string responseUrl;
		public string RegRootOfThisProg;
		public string infoUrl;
		public string contactUrl;
		public string AppNameRegex = "(XYZXYZ)";
		public bool isDevelopment;
		public string baseDomain = "https://puvox.software/";
		public string baseResponsePath = "program-responses.php?";
		public string baseCompanyName = "Puvox";
		public string baseContactPath = "contact";
		public static string developerMachineString = "puvox_development_machine";
		public bool Development_Mode;
		public bool initialized;
		public Dictionary<string, string> TheNames;
		public string mainResponse_string;
		private Dictionary<string, string> mainResponse;
		public bool debug_show_response;
		public static string mainDrive = Path.GetPathRoot(Environment.SystemDirectory);
		private Dictionary<string, string> symbols = new Dictionary<string, string>
		{
			{ "checkmark", "?" },
			{ "checkmark2", "\ud83d\uddf9" }
		};
		
		
		
		
		
		
		#region DateTime
			
		//  0940 type time-ints
		public bool isBetween(DateTime target, DateTime start, DateTime end, bool equality)
		{
			int num_middle  = int.Parse(target.ToString("HHmm"));
			int num2 = int.Parse(start.ToString("HHmm"));
			int num3 = int.Parse(end.ToString("HHmm"));
			return ( equality ? num_middle >= num2 && num_middle <= num3 :  num_middle > num2 && num_middle < num3);
		}
		public bool isBetween(DateTime target, int start, int end)
		{
			int num = int.Parse(target.ToString("HHmm"));
			return num >= start && num < end;
		}

			#region Demo(Trial) period checks
		public bool workTillDate(string program_slug, string dateTill)
		{
			bool result;
			try
			{
				if (dateTill != "" && DateTime.Now > DateTime.ParseExact(dateTill, "yyyy-MM-dd", CultureInfo.InvariantCulture))
				{
					bool flag = this.initialized;
					if (this.mainResponse.ContainsKey("error_message") || this.mainResponse["error_message"] != "")
					{
						Methods.m(this.mainResponse["error_message"]);
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
		public bool demoPeriodGone(string message, string dateTill)
		{
			if (this.demoPeriodGone_shown)
			{
				return this.demoPeriodGone_disallow;
			}
			this.demoPeriodGone_shown = true;
			this.demoPeriodGone_disallow = false;
			try
			{
				if (dateTill != "" && DateTime.Now > DateTime.ParseExact(dateTill, "yyyy-MM-dd", CultureInfo.InvariantCulture))
				{
					System.Windows.Forms.MessageBox.Show(message.ToString());
					this.demoPeriodGone_disallow = true;
				}
			}
			catch (Exception)
			{
				this.demoPeriodGone_disallow = true;
			}
			return this.demoPeriodGone_disallow;
		}
			#endregion
		
		public string timeIntToString(int timenow)
		{
			string text = timenow.ToString();
			return (text.Length <= 1) ? ("00000" + text) : ((text.Length <= 2) ? ("0000" + text) : ((text.Length <= 3) ? ("000" + text) : ((text.Length <= 4) ? ("00" + text) : ((text.Length <= 5) ? ("0" + text) : text))));
		}
		public TimeSpan timeIntToTimeSpan(int timenow)
		{
			DateTime dateTime;
			DateTime.TryParseExact(this.timeIntToString(timenow), "HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
			return dateTime.TimeOfDay;
		}

		public int CorrectTime(int timenow, int added_or_subtracted)
		{
			TimeSpan timeSpan = new TimeSpan(timenow / 100, timenow % 100, 0) + TimeSpan.FromMinutes((double)added_or_subtracted);
			return timeSpan.Hours * 100 + timeSpan.Minutes;
		}
		public int CorrectTime1(int timenow, int added_or_subtracted)
		{
			return int.Parse(DateTime.ParseExact(timenow.ToString("0000"), "HHmm", null).AddMinutes((double)added_or_subtracted).ToString("HHmm"));
		}

		
		public int DateToTime(DateTime dt)		{			return dt.Hour * 100 + dt.Minute;		}
		public DateTime ToDateTime(string s) { return this.ToDateTime(s, "ddMMyyyy", "");		} 
		public DateTime ToDateTime(string s, string format)		{			return this.ToDateTime(s, format, "");		}
 		public DateTime ToDateTime(string s, string format, string cultureString)
		{
			try { 
				CultureInfo provider = (cultureString == "") ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(cultureString);
				DateTime result;
				result = DateTime.ParseExact(s, format, provider); 
			}
			catch (FormatException) { throw; }
			catch (Exception) { throw; }
			return result;
		}
 
		public bool IsTimePeriod(string type_, NinjaScriptBase NS, int index, int StartTime, int EndTime, bool useDayOrSession)
		{
			int num = this.DateToTime(NS.Time[index]);
			int num2 = this.DateToTime(NS.Time[index + 1]);
			bool flag = StartTime < EndTime;
			bool flag2 = StartTime > EndTime;
			bool flag3 = useDayOrSession ? (NS.Time[0].DayOfYear != NS.Time[1].DayOfYear) : NS.Bars.IsFirstBarOfSession;
			bool result = (num >= StartTime && (num2 < StartTime || flag3)) || (flag3 && num2 <= StartTime);
			bool flag4 = (flag && num >= StartTime && num <= EndTime) || (flag2 && (num >= StartTime || num <= EndTime));
			bool flag5 = (flag && num2 >= StartTime && num2 <= EndTime) || (flag2 && (num2 >= StartTime || num2 <= EndTime));
			if (type_ == "start")
			{
				return result;
			}
			if (type_ == "inside")
			{
				return flag4;
			}
			return type_ == "end" && !flag4 && flag5;
		}

		
		public int HoursMinutes(DateTime time)
		{
			return time.Hour * 100 + time.Minute;
		}
		
		public string DateToTimeString(DateTime dt)
		{
			return string.Concat(new string[]
			{
				(dt.Hour < 10) ? "0" : "",
				dt.Hour.ToString(),
				":",
				(dt.Minute < 10) ? "0" : "",
				dt.Minute.ToString()
			});
		}

		public DateTime ToTimed_ToDate(DateTime cur_time, int timenow)
		{
			return DateTime.ParseExact(cur_time.ToString("yyyy-M-dd") + timenow.ToString(" 0000"), "yyyy-M-dd HHmm", null);
		}
	
		public static int GetWeekOfMonth(DateTime targetTime)
		{
			DateTime targetTime2 = new DateTime(targetTime.Year, targetTime.Month, 1);
			return Methods.GetWeekOfYear(targetTime) - Methods.GetWeekOfYear(targetTime2) + 1;
		}
		
		public static int GetWeekOfYear(DateTime targetTime)
		{
			return new GregorianCalendar().GetWeekOfYear(targetTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
		}

		public static int GetQuarter(DateTime targetTime)
		{
			int result = 0;
			int month = targetTime.Month;
			if 		(month <= 3) 				result = 1; 
			else if (month > 3 && month <= 6) 	result = 2; 
			else if (month > 6 && month <= 9)	result = 3;
			else if (month > 9)					result = 4;
			return result;
		}
		#endregion
		
		
		
		#region trading
		public string opposite(string text)
		{
			if (text == "") 		return ""; 
			if (text == "LONG")		return "SHORT";
			if (text == "SHORT")	return "LONG";
			if (text == "Long")		return "Short";
			if (text == "Short")	return "Long";
			if (text == "long")		return "short";
			if (text == "short")	return "long";
			if (text == "L")		return "S";
			if (text == "S")		return "L";
			if (text == "l")		return "s";
			if (text == "s")		return "l";
			if (text == "BUY")		return "SELL";
			if (text == "SELL")		return "BUY";
			if (text == "Buy")		return "Sell";
			if (text == "Sell")		return "Buy";
			if (text == "buy")		return "sell";
			if (text == "sell")		return "buy";
			if (text == "B")		return "S";
			if (text == "S")		return "B";
			if (text == "b")		return "s";
			if (text == "s")		return "b";
			if (text == "ABOVE")	return "BELOW";
			if (text == "BELOW")	return "ABOVE";
			if (text == "Above")	return "Below";
			if (text == "Below")	return "Above";
			if (text == "above")	return "below";
			if (text == "below")	return "above"; 
			return "";
		}
		#endregion
		
		
		public void createDefaultValues(object obj)
		{
			foreach (object obj2 in TypeDescriptor.GetProperties(obj))
			{
				PropertyDescriptor propertyDescriptor = (PropertyDescriptor)obj2;
				DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)propertyDescriptor.Attributes[typeof(DefaultValueAttribute)];
				if (defaultValueAttribute != null)
				{
					propertyDescriptor.SetValue(this, defaultValueAttribute.Value);
				}
			}
		}


		public static int digitsInNumber(object obj) { 
			return (int)Math.Ceiling(Math.Log10((double)obj)); 
		}
		
		public static int digitsAfterDot(double tickSize)
		{
			int num = 0;
			while (tickSize * Math.Pow(10.0, (double)num) != Math.Round(tickSize * Math.Pow(10.0, (double)num)))  num++; 
			return num;
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
			
		public int decimalsAmount(double content)
		{
			string text = this.double_to_decimal(content).ToString();
			return text.Length - text.IndexOf(".") - 1;
		}

		public static decimal double_to_decimal(double num)
		{
			return decimal.Parse(num.ToString(), NumberStyles.Float);
		}

		
		
		

		#region GlobalVariables
		public static Dictionary<string, object> GlobalVariables = new Dictionary<string, object>();
		public static object getGlobalVariable(string param)
		{
			if (!Methods.GlobalVariables.ContainsKey(param))
			{
				return null;
			}
			return Methods.GlobalVariables[param];
		}
		
		public static void setGlobalVar(string param, object value)
		{
			Methods.GlobalVariables[param] = value;
		}
		#endregion
		

		public static BitmapImage SetBitmapForButton(ImageBrush imgBtn, string filePath)
		{
			BitmapImage bitmap = new BitmapImage();
			bitmap.BeginInit();
			bitmap.UriSource = new Uri(filePath);
			bitmap.CacheOption = BitmapCacheOption.OnLoad;
			bitmap.EndInit();
			bitmap.Freeze();
			imgBtn.Dispatcher.Invoke(delegate()
			{
				imgBtn.ImageSource = bitmap;
			});
			return bitmap;
		}

		
		
		
		
		#region Dictionary
		private string objectToXml(object output)
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

		public Dictionary<string, string> objToDict(System.Windows.Forms.ComboBox.ObjectCollection objs)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (object obj in objs)
			{
				KeyValuePair<string, string> keyValuePair = (KeyValuePair<string, string>)obj;
				dictionary.Add(keyValuePair.Key, keyValuePair.Value);
			}
			return dictionary;
		}

		
		public int getIndexByKey(Dictionary<string, string> dict, string key)
		{
			List<KeyValuePair<string, string>> list = dict.ToList<KeyValuePair<string, string>>();
			return list.FindIndex((KeyValuePair<string, string> x) => x.Key == key);
		}



		public Dictionary<string, string> SortDict(Dictionary<string, string> dict)
		{
			Dictionary<string, string> result;
			try
			{
				result = (Dictionary<string, string>)(from entry in dict
				orderby entry.Value
				select entry);
			}
			catch (Exception)
			{
				result = new Dictionary<string, string>();
			}
			return result;
		}
		
        public bool ContainsValue(Dictionary<int, double> myd, int indx) { return myd.ContainsKey(indx) && myd[indx] != double.NaN && HasValue(myd[indx]); }
        public bool ContainsValue(Dictionary<int, bool> myd, int indx) { return myd.ContainsKey(indx) && HasValue(myd[indx]); }
        public bool ContainsValue(Dictionary<string, double> myd, string indx) { return myd.ContainsKey(indx) && myd[indx] != double.NaN && HasValue(myd[indx]); }

		public bool ContainsKey(NameValueCollection collection, string key)
		{
			return collection.Get(key) != null || collection.AllKeys.Contains(key);
		}

		public Dictionary<int, double> FindNearestValuesInDict(Dictionary<int, double> MyDict, double Target_value, int how_many_values_to_find)
		{
			Dictionary<int, double> result;
			try
			{
				bool flag = this.DictIsAscendingOrDescending(MyDict);
				int num = MyDict.Keys.Count<int>();
				int num2 = 0;
				int num3 = 0;
				Dictionary<int, double> dictionary = new Dictionary<int, double>(MyDict);
				for (int i = 0; i < num; i++)
				{
					int index = flag ? i : (num - 1 - i);
					int index2 = flag ? (num - 1 - i) : i;
					if (MyDict.Keys.ElementAtOrDefault(index) != 0)
					{
						int num4 = MyDict.Keys.ElementAt(index);
						if (this.ContainsValue(MyDict, num4) && MyDict[num4] > Target_value)
						{
							num2++;
							if (num2 > how_many_values_to_find)
							{
								dictionary.Remove(num4);
							}
						}
					}
					if (MyDict.Keys.ElementAtOrDefault(index2) != 0)
					{
						int num5 = MyDict.Keys.ElementAt(index2);
						if (this.ContainsValue(MyDict, num5) && MyDict[num5] < Target_value)
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

		
		public bool DictIsAscendingOrDescending(Dictionary<int, double> MyDict)
		{
			int num = MyDict.Keys.Count<int>();
			double num2 = double.NaN;
			for (int i = 0; i < num; i++)
			{
				int num3 = MyDict.Keys.ElementAt(i);
				if (this.ContainsValue(MyDict, num3))
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

		public bool containsUserInput(dynamic type_, string whichName)
		{
			if (Methods.<containsUserInput>o__SiteContainer2b.<>p__Site2c == null)
			{
				Methods.<containsUserInput>o__SiteContainer2b.<>p__Site2c = CallSite<Func<CallSite, object, NinjaScriptBase>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, typeof(NinjaScriptBase), typeof(Methods)));
			}
			NinjaScriptBase ninjaScriptBase = Methods.<containsUserInput>o__SiteContainer2b.<>p__Site2c.Target(Methods.<containsUserInput>o__SiteContainer2b.<>p__Site2c, type_);
			foreach (PropertyInfo propertyInfo in ninjaScriptBase.GetType().GetProperties())
			{
				DisplayAttribute displayAttribute = propertyInfo.GetCustomAttributesData().Any((CustomAttributeData a) => a.AttributeType.Name == "DisplayAttribute") ? propertyInfo.GetCustomAttribute<DisplayAttribute>() : null;
				if (displayAttribute != null && propertyInfo.Name == whichName)
				{
					return true;
				}
			}
			return false;
		} 
		public string serialize2(object dict)
		{
			if (dict is Dictionary<string, string>) return new JavaScriptSerializer().Serialize(dict.ToDictionary((KeyValuePair<string, string> item) => item.Key.ToString(), (KeyValuePair<string, string> item) => item.Value.ToString()));
			//else Dictionary<string, object> dict
			return new JavaScriptSerializer().Serialize(dict.ToDictionary((KeyValuePair<string, object> item) => item.Key.ToString(), (KeyValuePair<string, object> item) => item.Value.ToString()));
		}
 
		public static string dictToString(object obj)
		{
			return string.Join("; ", (from x in obj as Dictionary<object, object>
			select x.Key.ToString() + "=" + x.Value.ToString()).ToArray<string>());
		}

		public Dictionary<string, string> deserialize(string str)
		{
			return this.deserializer_35(str);
		}
 
		public bool deserialize(string str, ref Dictionary<string, string> dict)
		{
			Dictionary<string, string> dictionary = this.deserializer_35(str);
			if (dictionary != null)
			{
				dict = dictionary;
				return true;
			}
			return false;
		}
 
		public Dictionary<string, string> deserializer_35(string str)
		{
			Dictionary<string, string> result;
			try
			{
				result = this.ConvertToStringDictionary(Methods.JsonMaker.ParseJSON(str));
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		
		public Dictionary<string, string> ConvertToStringDictionary(Dictionary<string, object> dicti)
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

		
		public string getDictionaryKeyByValue(Dictionary<string, string> dict, string value)
		{
			return dict.FirstOrDefault((KeyValuePair<string, string> x) => x.Value == value).Key;
		}

		
		public Dictionary<string, string> xml_to_dictionary(string xml_data)
		{
			XDocument xdocument = XDocument.Parse(xml_data);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (XElement xelement in from p in xdocument.Descendants()
			where !p.HasElements
			select p)
			{
				int num = 0;
				string key = xelement.Name.LocalName;
				while (dictionary.ContainsKey(key))
				{
					key = xelement.Name.LocalName + "_" + num++;
				}
				dictionary.Add(key, xelement.Value);
			}
			return dictionary;
		}
		#endregion
		

		#region Registry
		public RegistryKey getRegistryHiveKey(RegistryHive rh)
		{
			return RegistryKey.OpenBaseKey(rh, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
		}

		public static void Read64bitRegistryFrom32bitApp(string[] args)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
			registryKey = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");
			if (registryKey != null)
			{
				registryKey.GetValue("RegisteredOrganization").ToString();
			}
			RegistryKey registryKey2 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
			registryKey2 = registryKey2.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");
			if (registryKey2 != null)
			{
				registryKey2.GetValue("RegisteredOrganization").ToString();
			}
		}
		
		
		public RegistryKey chosenRegHive = Registry.CurrentUser;

		public Dictionary<string, string> myregs = new Dictionary<string, string>();
		
		public string regPartFromKey(string key, int partN)
		{
			if (partN == 1)
			{
				if (this.charsInPhrase(key, "\\") != 0)
				{
					return this.withoutLastDir(key, 1);
				}
				return this.RegRootOfThisProg;
			}
			else
			{
				if (partN != 2)
				{
					return "";
				}
				if (this.charsInPhrase(key, "\\") != 0)
				{
					return this.lastPart(key);
				}
				return key;
			}
		}

		
		public bool existsRegistryValue(string key)
		{
			return this.getRegistryValue(key) != null;
		}

		
		public bool existsRegistryValue(string path, string key)
		{
			return this.getRegistryValue(path + key) != null;
		}

		
		public string getRegistryValue(string key)
		{
			return this.getRegistryValue(this.regPartFromKey(key, 1), this.regPartFromKey(key, 2));
		}

		
		public string getRegistryValue(string path, string key)
		{
			string result;
			try
			{
				string text = "";
				if (this.myregs.ContainsKey(path + key))
				{
					result = this.myregs[path + key];
				}
				else
				{
					RegistryKey registryHiveKey = this.getRegistryHiveKey(this.chosenRegHive);
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
							this.myregs[path + key] = text;
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

		
		public string getRegistryValue(string key, string defaultVal, bool nothing)
		{
			string registryValue = this.getRegistryValue(this.regPartFromKey(key, 1), this.regPartFromKey(key, 2));
			if (string.IsNullOrEmpty(registryValue))
			{
				this.setRegistryValue(key, defaultVal);
				return defaultVal;
			}
			return registryValue;
		}

		
		public void setRegistryValue(string key, string value)
		{
			this.setRegistryValue(this.regPartFromKey(key, 1), this.regPartFromKey(key, 2), value);
		}

		
		public bool setRegistryValue(string path, string key, string value)
		{
			bool result;
			try
			{
				RegistryKey registryHiveKey = this.getRegistryHiveKey(this.chosenRegHive);
				if (registryHiveKey != null)
				{
					RegistryKey registryKey = registryHiveKey.OpenSubKey(path, true);
					if (registryKey == null)
					{
						registryKey = registryHiveKey.CreateSubKey(path);
					}
					this.myregs[path + key] = value;
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

		
		public string getSetRegistryValue(string key, string value)
		{
			return this.getSetRegistryValue(this.regPartFromKey(key, 1), this.regPartFromKey(key, 2), value);
		}

		
		public string getSetRegistryValue(string path, string key, string value)
		{
			string result;
			try
			{
				string registryValue = this.getRegistryValue(path, key);
				if (string.IsNullOrEmpty(registryValue))
				{
					this.setRegistryValue(path, key, value);
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

		
		public RegistryKey getRegistryHiveKey(RegistryKey rh)
		{
			return rh;
		}

		
		public string Read(string subKey, string KeyName)
		{
			string result;
			try
			{
				RegistryKey registryKey = this.chosenRegHive.OpenSubKey(subKey);
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

		
		public bool Write(string subKey, string KeyName, object Value)
		{
			bool result;
			try
			{
				RegistryKey registryKey = this.chosenRegHive;
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

		
		public bool DeleteKey(string subKey, string KeyName)
		{
			bool result;
			try
			{
				RegistryKey registryKey = this.chosenRegHive;
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

		
		public bool DeleteSubKeyTree(string subKey)
		{
			bool result;
			try
			{
				RegistryKey registryKey = this.chosenRegHive;
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

		
		public int SubKeyCount(string subKey)
		{
			int result;
			try
			{
				RegistryKey registryKey = this.chosenRegHive;
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

		
		public int ValueCount(string subKey)
		{
			int result;
			try
			{
				RegistryKey registryKey = this.chosenRegHive;
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

		
		public Dictionary<string, string> RegistryValuesInFolder()
		{
			Dictionary<string, string> result;
			try
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				string regRootOfThisProg = this.RegRootOfThisProg;
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

		
		public bool FirstTimeAction(string regKey)
		{
			if (this.getRegistryValue("triggered_" + regKey) != "y")
			{
				this.setRegistryValue("triggered_" + regKey, "y");
				return true;
			}
			return false;
		}

		
		public bool TimeGone(string Key, int minutes)
		{
			string registryValue = this.getRegistryValue("timegone_" + Key);
			if (string.IsNullOrEmpty(registryValue) || DateTime.Now > DateTime.Parse(registryValue).AddMinutes((double)minutes))
			{
				this.setRegistryValue("timegone_" + Key, DateTime.Now.ToString());
				return true;
			}
			return false;
		}
		#endregion


		
		
		
		#region Color region Brushes
		public System.Windows.Media.Color ChangeColorBrightness(System.Windows.Media.Color color, string correctionFactor)
		{
			if (!(correctionFactor == "dark")) correctionFactor == "light"; 
			return this.ChangeColorBrightness(color, correctionFactor);
		}
		
		public System.Windows.Media.Color ChangeColorBrightness(System.Windows.Media.Color color, float correctionFactor)
		{
			float num = (float)color.R;
			float num2 = (float)color.G;
			float num3 = (float)color.B;
			if (correctionFactor < 0f)
			{
				correctionFactor = 1f + correctionFactor;
				num *= correctionFactor;
				num2 *= correctionFactor;
				num3 *= correctionFactor;
			}
			else
			{
				num = (255f - num) * correctionFactor + num;
				num2 = (255f - num2) * correctionFactor + num2;
				num3 = (255f - num3) * correctionFactor + num3;
			}
			return System.Windows.Media.Color.FromArgb(color.A, (byte)num, (byte)num2, (byte)num3);
		}

		public System.Windows.Media.Color color_from_hex(string hex)
		{
			hex = hex.Replace("#", "");
			byte a = byte.MaxValue;
			int num = 0;
			if (hex.Length == 8)
			{
				a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
				num = 2;
			}
			byte r = byte.Parse(hex.Substring(num, 2), NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(num + 2, 2), NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(num + 4, 2), NumberStyles.HexNumber);
			return System.Windows.Media.Color.FromArgb(a, r, g, b);
		}
		#endregion









		#region String manipulations
		
		
		public string lastPart(string path)
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

		
		public int charsInPhrase(string source, string char_)
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
			foreach (object obj in (IList)array ) if ( str.StartsWith(obj.ToString()) ) result = true;  
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
		public string urlEncode(string msg)
		{
			return Uri.EscapeDataString(msg); //.Replace("%20", "+");
		}

		public int countWords(string str)
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

		public string RandomString(int length)
		{
			string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			char[] array = new char[length];
			Random random = new Random();
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = text[random.Next(text.Length)];
			}
			return new string(array);
		}

		
		public string RandomString()
		{
			return this.RandomString(8);
		}

		public static string DateToSeconds(DateTime date)
		{
			return date.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
		} 
		
		public string convertBackSlashes(string path)
		{
			return path.Replace("/", "\\");
		}

		public string regexReplace(string input, string pattern, string with)
		{
			return new Regex(pattern).Replace(input, "");
		}

		
		public string SanitizeSymbol(string s)
		{
			return s.Replace("/", "_").Replace("\\", "_").Replace("|", "_").Replace("*", "_").ToUpper();
		}

		
		public string NotNull(string smth)
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
				string[] array3 = text3.Split(null, StringSplitOptions.RemoveEmptyEntries);
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
		public string EncryptString(string plainText, string password)
		{
			string result;
			try
			{
				SHA256 sha = SHA256.Create();
				byte[] key = sha.ComputeHash(Encoding.ASCII.GetBytes(password));
				byte[] array = new byte[16];
				byte[] iv = array;
				Aes aes = Aes.Create();
				aes.Mode = CipherMode.CBC;
				aes.Key = key;
				aes.IV = iv;
				MemoryStream memoryStream = new MemoryStream();
				ICryptoTransform transform = aes.CreateEncryptor();
				CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
				byte[] bytes = Encoding.ASCII.GetBytes(plainText);
				cryptoStream.Write(bytes, 0, bytes.Length);
				cryptoStream.FlushFinalBlock();
				byte[] array2 = memoryStream.ToArray();
				memoryStream.Close();
				cryptoStream.Close();
				string text = Convert.ToBase64String(array2, 0, array2.Length);
				result = text;
			}
			catch (Exception)
			{
				result = "";
			}
			return result;
		}

		//return this.DecryptString(cipherText, this.Version.ToString());
		public string DecryptString(string cipherText, string password)
		{
			string result;
			try
			{
				SHA256 sha = SHA256.Create();
				byte[] key = sha.ComputeHash(Encoding.ASCII.GetBytes(password));
				byte[] array = new byte[16];
				byte[] iv = array;
				Aes aes = Aes.Create();
				aes.Mode = CipherMode.CBC;
				aes.Key = key;
				aes.IV = iv;
				MemoryStream memoryStream = new MemoryStream();
				ICryptoTransform transform = aes.CreateDecryptor();
				CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
				string text = string.Empty;
				try
				{
					byte[] array2 = Convert.FromBase64String(cipherText);
					cryptoStream.Write(array2, 0, array2.Length);
					cryptoStream.FlushFinalBlock();
					byte[] array3 = memoryStream.ToArray();
					text = Encoding.ASCII.GetString(array3, 0, array3.Length);
				}
				catch (Exception)
				{
					System.Windows.Forms.MessageBox.Show("Problem in encryption. ErrorCode 274");
				}
				finally
				{
					memoryStream.Close();
					cryptoStream.Close();
				}
				result = text;
			}
			catch (Exception)
			{
				result = "";
			}
			return result;
		}
			#endregion
		
		
		
		
		
			#region Base64 md5
			
		public string md5(string input)
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

		public string Base64Decode(string str)
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

		
		public string Base64Encode(string toEncode)
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

		
		public string decode64(string str)
		{
			byte[] bytes = Convert.FromBase64String(str);
			return Encoding.UTF8.GetString(bytes);
		}

		
			#endregion
			
		#endregion
		
		
		
		 
		
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

		public string ShowDialog(string text)
		{
			return this.ShowDialog(text, "", "");
		}
		public string ShowDialog(string text, string caption, string defaultValue)
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
				button.Click += delegate(object sender, EventArgs e)
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

		
		
		
		public string GetControlText(Form obj, string which)
		{
			System.Windows.Forms.Control[] array = obj.Controls.Find(which, true);
			if (array.Length <= 0)
			{
				return "cant find control";
			}
			return array[0].Text;
		}


		public void CenterLabel(System.Windows.Forms.Control ctrl)
		{
			try
			{
				ctrl.AutoSize = false;
				ctrl.Dock = DockStyle.None;
				ctrl.Width = ctrl.Parent.Width;
				if (ctrl is System.Windows.Forms.Label)
				{
					(ctrl as System.Windows.Forms.Label).TextAlign = ContentAlignment.MiddleCenter;
				}
			}
			catch (Exception ex)
			{
				Methods.m(ex.Message);
			}
		}

		
		public bool isUserInput(System.Windows.Forms.Control ct)
		{
			return ct is System.Windows.Forms.TextBox || ct is System.Windows.Forms.ComboBox || ct is System.Windows.Forms.CheckBox || ct is CheckedListBox || ct is DateTimePicker || ct is System.Windows.Forms.GroupBox || ct is System.Windows.Forms.ListBox || ct is MonthCalendar || ct is System.Windows.Forms.RadioButton || ct is System.Windows.Forms.RichTextBox || ct is TrackBar;
		}

		
		public string optionsPrefix = "formoption_"; 
		
		public bool fillFormOptions(System.Windows.Forms.Control.ControlCollection cts)
		{
			foreach (object obj in cts)
			{
				System.Windows.Forms.Control control = (System.Windows.Forms.Control)obj;
				if (control != null && this.isUserInput(control))
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
					control.Text = this.getRegistryValue(this.optionsPrefix + control.Name, defaultVal, false);
				}
			}
			return true;
		}

		
		public bool saveFormOptions(System.Windows.Forms.Control.ControlCollection cts)
		{
			foreach (object obj in cts)
			{
				System.Windows.Forms.Control control = (System.Windows.Forms.Control)obj;
				if (control != null && this.isUserInput(control))
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
					this.setRegistryValue(this.optionsPrefix + control.Name, value);
				}
			}
			return true;
		}

		
		public void SetComboboxByKey(System.Windows.Forms.ComboBox cb1, string key)
		{
			cb1.SelectedIndex = this.getIndexByKey(this.objToDict(cb1.Items), key);
		}

	

		public void FillCombobox(System.Windows.Forms.ComboBox cb1, Dictionary<string, string> dict)
		{
			this.FillCombobox(cb1, dict, false, "");
		}

		
		public void FillCombobox(System.Windows.Forms.ComboBox cb1, Dictionary<string, string> dict, bool sort, string SelectedValue)
		{
			cb1.DataSource = new BindingSource(sort ? this.SortDict(dict) : dict, null);
			cb1.DisplayMember = "Value";
			cb1.ValueMember = "Key";
			if (SelectedValue != "")
			{
				cb1.SelectedValue = SelectedValue;
			}
		}

		public void invokeSafe(System.Windows.Forms.Control uiElement, Action updater, bool forceSynchronous)
		{
			try
			{
				if (uiElement != null)
				{
					if (uiElement.InvokeRequired)
					{
						if (forceSynchronous)
						{
							uiElement.Invoke(new Action(delegate()
							{
								this.SafeInvoke(uiElement, updater, forceSynchronous);
							}));
						}
						else
						{
							uiElement.BeginInvoke(new Action(delegate()
							{
								this.SafeInvoke(uiElement, updater, forceSynchronous);
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


		public bool Control_Set(System.Windows.Forms.Control ctrl, string what, object value)
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
						Methods.Control_SetDelegate method = new Methods.Control_SetDelegate(this.Control_Set);
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
		
		public bool closeForm(Form frm)
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
						frm.Invoke(new MethodInvoker(delegate()
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

		
		public void SetControlText(Form obj, string which, string txt)
		{
			try
			{
				if (obj != null)
				{
					if (obj.InvokeRequired)
					{
						obj.Invoke(new Action(delegate()
						{
							this.SetControlText(obj, which, txt);
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
		}

		public void SetTextControl(Form form, System.Windows.Forms.Control ctrl, string text)
		{
			try
			{
				if (ctrl != null)
				{
					if (ctrl.InvokeRequired)
					{
						Methods.SetTextCallback method = new Methods.SetTextCallback(this.SetTextControl);
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
			return this.StartTimer(interval, function, true);
		}

		public TimerInterrupter SetTimeout(int interval, Action function)
		{
			return this.StartTimer(interval, function, false);
		}
			
			public class TimerInterrupter
			{
				public TimerInterrupter(System.Timers.Timer timer)
				{
					if (timer == null)
					{
						throw new ArgumentNullException("timer");
					}
					this._timer = timer;
				}

				
				public void Stop()
				{
					this._timer.Stop();
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
					timer.Elapsed += delegate(object sender, ElapsedEventArgs e)
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
				timer = new System.Threading.Timer(delegate(object s)
				{
					action();
					timer.Dispose();
					lock (this.ExecuteAfter_timers)
					{
						this.ExecuteAfter_timers.Remove(timer);
					}
				}, null, (long)milliseconds, (long)((ulong)-11));
				lock (this.ExecuteAfter_timers)
				{
					this.ExecuteAfter_timers.Add(timer);
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

		#endregion
		
		
		
		
		#region Reflection Manipulation
		public List<string> PropertyInfoToList(PropertyInfo[] PropList)
		{
			List<string> list = new List<string>();
			foreach (PropertyInfo propertyInfo in PropList)
			{
				list.Add(propertyInfo.Name);
			}
			return list;
		}

		public List<string> ObjectPropertyNames(object Obj)
		{
			return this.PropertyInfoToList(Obj.GetType().GetProperties());
		}

		
		public bool MethodExists(object objectToCheck, string methodName)
		{
			return objectToCheck.GetType().GetMethod(methodName) != null;
		}

		
		public string Trace(Exception ex)
		{
			StackTrace stackTrace = new StackTrace(ex, true);
			StackFrame frame = stackTrace.GetFrame(0);
			return frame.GetFileLineNumber().ToString();
		}
		#endregion
		
		
		
		#region Url Site actions
		public string urlRead(string url)
		{
			string result = "-1";
			try
			{
				using (WebClient webClient = new WebClient())
				{
					webClient.Encoding = Encoding.UTF8;
					result = webClient.DownloadString(url);
				}
			}
			catch (Exception ex) { return "Error:" + ex.ToString(); }
			return result;
		}

		public bool downloadFile(string url, string location)
		{
			bool result;
			try
			{
				using (WebClient webClient = new WebClient())
				{
					webClient.downloadFile(url, location);
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
				this.Timeout = timeout;
			}
			protected override WebRequest GetWebRequest(Uri address)
			{
				WebRequest result;
				try
				{
					WebRequest webRequest = base.GetWebRequest(address);
					if (webRequest != null)
					{
						webRequest.Timeout = this.Timeout;
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

		
		public string urlPost(string url)
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
			string text = this.convertBackSlashes(Path.GetTempPath() + "\\" + url.Replace(uri.Scheme, "").Replace("://", "").Replace("//", ""));
			if (!File.Exists(text))
			{
				string directoryName = Path.GetDirectoryName(text);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				this.downloadFile(url, text);
			}
			return text;
		}
		
		public void urlOpen(string url)
		{
			new System.Windows.Forms.WebBrowser
			{
				AllowNavigation = true
			}.Navigate(url);
		}

		public bool internetAvailable(string messageShow)
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
		
		public string withoutLastDir(string path, int HowManyLastDirs)
		{
			string result = path;
			if (this.charsInPhrase(path, "\\") > HowManyLastDirs)
			{
				result = string.Join("\\", path.Split(new char[]
				{
					'\\'
				}).Reverse<string>().Skip(HowManyLastDirs).Reverse<string>().ToArray<string>());
			}
			return result;
		}

		
		public bool WriteFile(string fileName, string text, bool rewrite_or_add)
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
		
		public void WriteTempFile(string fileName, string text, bool rewrite_or_add)
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
		
		public string ReadFile(string path)
		{
			if (File.Exists(path))
			{
				return File.ReadAllText(path);
			}
			return "";
		}

		public string readFile(string filepath)
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


		public bool createDir(string path)
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

		public bool deleteDir(string path)
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

		public void list_all_files_in_dir(string base_path)
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



		

		public bool HasValue(double value)
		{
			return !double.IsNaN(value) && !double.IsInfinity(value);
		}

		public void errorlog(string message)
		{
		}

		public void playSound()
		{
			this.playSound("Speech On");
		}

		public bool playSound(string path)
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

		public string getDesktopPath()
		{
			return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		}


		
		
		#region Translation/Google
		public string getLangFromIsoLang(string isoLang)
		{
			return new Regex("\\-(.*)", RegexOptions.IgnoreCase).Replace(isoLang, "");
		}

		
		public void googleSetKeyFile(string FileContent)
		{
			string text = Environment.GetEnvironmentVariable("appdata") + "\\gsapk.xyz";
			this.setRegistryValue("gsapiKeyPath", text);
			File.WriteAllText(text, FileContent);
			this.SetGoogleCredentialsDefaults(text);
		}

		
		public void SetGoogleCredentialsDefaults(string path)
		{
			Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
		}

		
		public string selectedLang = "en";
		public Dictionary<string, Dictionary<string, string>> transl_texts;
		public string translate(string txt)
		{
			if (this.transl_texts.ContainsKey(txt) && this.transl_texts[txt].ContainsKey(this.selectedLang))
			{
				return this.transl_texts[txt][this.selectedLang];
			}
			return this.translatedReady(this.selectedLang, txt);
		}

		
		public void setTexts(Dictionary<string, string> dict, string lang_2char, Form frm)
		{
			foreach (KeyValuePair<string, string> keyValuePair in dict)
			{
				string key = "trans_" + keyValuePair.Value + "_" + lang_2char;
				string key2 = "trans_" + keyValuePair.Value + "_en";
				string text = this.getRegistryValue(key);
				string registryValue = this.getRegistryValue(key2);
				if (string.IsNullOrEmpty(text) || (!string.IsNullOrEmpty(registryValue) && registryValue != keyValuePair.Key))
				{
					if (lang_2char == "en")
					{
						text = keyValuePair.Key;
					}
					else
					{
						text = this.GTranslate_checker(keyValuePair.Key, lang_2char);
					}
					this.setRegistryValue(key, text);
				}
				this.SetControlText(frm, keyValuePair.Value, text);
			}
		}

		
		public string translatedReady(string targetLang, string englishValue)
		{
			string text = englishValue;
			string key = "translReady_" + this.md5(englishValue) + "_" + targetLang;
			if (targetLang != "en")
			{
				string registryValue = this.getRegistryValue(key);
				if (registryValue == null)
				{
					text = this.GTranslate_checker(englishValue, targetLang);
					this.setRegistryValue(key, text);
				}
				else
				{
					text = registryValue;
				}
			}
			else
			{
				this.setRegistryValue(key, englishValue);
			}
			return text;
		}

		
		public string GTranslate_checker(string what, string lang_target)
		{
			string result;
			if (this.MethodExists(this, "GTranslate_callback"))
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
		public string gTranslate(string what, string lang_target, bool useApi = false)
		{
			string text = "en";
			string text2 = "";
			if (useApi)
			{
				string text3 = this.readUrl(string.Concat(new string[]
				{
					this.ApiURL,
					"&action=translate&lang=",
					lang_target,
					"&string[]=",
					this.UrlEncode(what),
					this.DeveloperMachineAddition()
				}));
				if (this.deserialize(text3, ref this._dict) && this._dict.ContainsKey(what))
				{
					text2 = this._dict[what];
				}
			}
			else
			{
                // https://stackoverflow.com/questions/26714426/what-is-the-meaning-of-google-translate-query-params
				string text3 = this.readUrl(string.Concat(new string[]
				{
					"https://translate.googleapis.com/translate_a/single?client=gtx&sl=",
					text,
					"&tl=",
					lang_target,
					"&dt=t&q=",
					this.UrlEncode(what)
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

		public string DeveloperMachineAddition()
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
		public static void m(int obj) 		{ Methods.m(obj.ToString()); }
		public static void m(double obj)	{ Methods.m(obj.ToString()); }
		public static void m(bool obj)		{ Methods.m(obj.ToString()); }
		public static void m(Exception obj)	{ Methods.m(obj.ToString()); }
		public static void m(object obj)	{ Methods.m(obj.ToString()); }
		public static void m(object[] obj)
		{
			if (obj == null){	Methods.m("null");	return;	}
			string text = ""; foreach (object obj2 in obj) text = text + ((obj2 == null) ? "null" : obj.ToString()) + Methods.nl_; 
			Methods.m(text);
		}
		public static void m(string obj)
		{
			System.Windows.Forms.MessageBox.Show(new Form { TopMost = true }, (obj == null) ? "null" : obj.ToString());
		}

		public static void cw(int obj) 		{ cw(obj.ToString()); }
		public static void cw(double obj)	{ cw(obj.ToString()); }
		public static void cw(bool obj)		{ cw(obj.ToString()); }
		public static void cw(Exception obj){ cw(obj.ToString()); }
		public static void cw(object obj)	{ cw(obj.ToString()); }
		public static void cw(object[] obj)
		{
			if (obj == null){ cw("null");	return;	}
			string text = ""; foreach (object obj2 in obj) text = text + ((obj2 == null) ? "null" : obj.ToString()) + Methods.nl_; 
			cw(text);
		}
		public static void cw(string obj)
		{
			Console.Write(obj== null ? "null" : obj.ToString());
		}
		
		

		// another kind of dumper (removed):  https://pastebin.com/KEzWthMp
		
		
		public static string dump(object obj) 			{ return Methods.dump(obj, Methods.AllBindingFlags, false, 1, 1, ""); }
		public static string dump(object obj, int deep) { return Methods.dump(obj, Methods.AllBindingFlags, false, deep, 1, ""); }
		public static string dump(object obj, bool execMethods)			 { return Methods.dump(obj, Methods.AllBindingFlags, execMethods, 1, 1, ""); }
		public static string dump(object obj, int deep, bool execMethods){ return Methods.dump(obj, Methods.AllBindingFlags, execMethods, deep, 1, ""); }
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
								text6 = string.Concat(new string[] {"   [", text11.Replace("System.", ""), "]  (", Methods.tryEnumerabledString(methodInfo.GetParameters(), ", "), ")  <", Methods.AccessModifierType(methodInfo), ">" });
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
								text6 = left2 == null ? "null" :  "ToString: " + eventInfo.ToString();
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
				if (fieldInfo.IsPrivate)	return "Private";
				if (fieldInfo.IsFamily)		return "Protected";
				if (fieldInfo.IsAssembly)	return "Internal";
				if (fieldInfo.IsPublic)		return "Public";
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
				if (methodInfo.IsPrivate) 	return "Private"; 
				if (methodInfo.IsFamily)  	return "Protected"; 
				if (methodInfo.IsAssembly)	return "Internal";
				if (methodInfo.IsPublic)	return "Public";
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


		public string PrintProperties(object obj)
		{
			return this.PrintProperties(obj, 0);
		}

		
		public string PrintProperties(object obj, int indent)
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
								text += this.PrintProperties(value, indent + 2);
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

		

		public string DisplayObjectInfo(object o)
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

		

		private static void RemoveEvents(System.Windows.Controls.Button b)
		{
			EventInfo[] events = typeof(System.Windows.Controls.Button).GetEvents();
			for (int i = 0; i < events.Length; i++)
			{
				FieldInfo field = typeof(System.Windows.Controls.Control).GetField(events[i].Name, BindingFlags.Static | BindingFlags.NonPublic);
				object value = field.GetValue(b);
				PropertyInfo property = b.GetType().GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic);
				EventHandlerList eventHandlerList = (EventHandlerList)property.GetValue(b, null);
				eventHandlerList.RemoveHandler(value, eventHandlerList[value]);
			}
		}

		public static void RemoveRoutedEventHandlers(UIElement element, RoutedEvent routedEvent)
		{
			PropertyInfo property = typeof(UIElement).GetProperty("EventHandlersStore", BindingFlags.Instance | BindingFlags.NonPublic);
			object value = property.GetValue(element, null);
			if (value == null)
			{
				return;
			}
			MethodInfo method = value.GetType().GetMethod("GetRoutedEventHandlers", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			RoutedEventHandlerInfo[] array = (RoutedEventHandlerInfo[])method.Invoke(value, new object[]
			{
				routedEvent
			});
			foreach (RoutedEventHandlerInfo routedEventHandlerInfo in array)
			{
				element.RemoveHandler(routedEvent, routedEventHandlerInfo.Handler);
			}
		}
 

		public static double NetFrameworkVersion()
		{
			double result;
			try
			{
				string version = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Where(x => x.Name == "System.Core").First().Version.ToString();
                return Convert.ToDouble(version.Substring(0, version.IndexOf(("."), 2)));
				result = Convert.ToDouble(text.Substring(0, text.IndexOf(".", 2)));
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

	
		public static void playSound_static(string path)
		{
			if (!path.Contains("."))
			{
				path += ".wav";
			}
			if (!path.Contains("\\"))
			{
				path = "C:\\Windows\\media\\" + path;
			}
			try
			{
				SoundPlayer soundPlayer = new SoundPlayer(path);
				soundPlayer.Play();
			}
			catch (Exception ex)
			{
				Methods.m(ex.Message);
			}
		}


		
		

		#region Log
		public static void m_ex(Exception ex)
		{
			StackTrace stackTrace = new StackTrace();
			MethodBase method = stackTrace.GetFrame(1).GetMethod();
			m(ex, method);
		}

		public static void enableExceptions()
		{
			this.enableExceptions(false);
		}

		public static void enableExceptions(bool show)
		{
			Methods.enableExceptions_static(show);
		}

		public static void enableExceptions(bool show = false)
		{
			AppDomain.CurrentDomain.FirstChanceException += delegate(object sender, FirstChanceExceptionEventArgs eventArgs)
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

		public void log(string text)
		{
			string location = Path.GetTempPath() + programName + "_default_logs.txt";
			this.log(text, location);
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
			FileOperations.Write(Methods.errorLogFile, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine + obj_.ToString() + Environment.NewLine);
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
		
		
		
		public string appName()
		{
			string location = Assembly.GetEntryAssembly().Location;
			return Path.GetFileNameWithoutExtension(location);
		}

		public void ExecuteCommand(string command)
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


		


		public bool is64BitOS = Environment.Is64BitOperatingSystem;


		public static string tmpFilePath = Environment.GetEnvironmentVariable("tmp") + "\\"; 
		public string pathDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
 
		
		public string nl = Environment.NewLine;

		public static string[] singleTypes = new string[]{	"System.Double","System.Int32","System.String","System.Float","System.Boolean"	};

		
		internal static BindingFlags AllBindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty | BindingFlags.ExactBinding | BindingFlags.SuppressChangeType | BindingFlags.OptionalParamBinding | BindingFlags.IgnoreReturn;

		internal static BindingFlags AllBindingFlags_noinherit = BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty | BindingFlags.ExactBinding | BindingFlags.SuppressChangeType | BindingFlags.OptionalParamBinding | BindingFlags.IgnoreReturn;

		public static bool inCatchAllLoop = false; 
		private static object fileCheckObj = new object(); 
		public static string errorLogFile = Environment.GetEnvironmentVariable("tmp") + "\\_errorlogs_c#.log"; 
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

		





        public class JsonHelper
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

