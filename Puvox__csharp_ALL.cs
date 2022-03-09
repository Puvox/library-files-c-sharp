
// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives/preprocessor-if
// preprocessor directives: https://stackoverflow.com/questions/38476796/how-to-set-net-core-in-if-statement-for-compilation
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Win32;
using System.Threading.Tasks;


/*

https://ninjatrader.com/support/helpGuides/nt8/?multi-threading.htm
https://stackoverflow.com/questions/16760689/how-many-threads-can-ran-on-a-cpu-at-a-time


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





namespace PuvoxLibrary
{
	public partial class Methods
	{
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

		public static string mainDrive { get { return Path.GetPathRoot(Environment.SystemDirectory); } }



		public static string AppNameRegex = "(XYZXYZ)";
		public static string developerMachineString = "puvox_development_machine";


		private Dictionary<string, string> symbols = new Dictionary<string, string>
		{
			{ "checkmark", "?" },
			{ "checkmark2", "\ud83d\uddf9" }
		};



		#region Demo(Trial) period checks
		// if (demoPeriodGone("demo expired", "2020-05-13")) return;
		private bool demoPeriodGone_shown;
		private bool demoPeriodGone_disallow;
		// "yyyy-MM-dd"
		public bool demoPeriodGone(string message, string dateTill)
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


		public static bool initialized;
		public static bool workTillDate(string program_slug, string dateTill)
		{
			bool result;
			try
			{
				if (dateTill != "" && DateTime.Now > DateTime.ParseExact(dateTill, "yyyy-MM-dd", CultureInfo.InvariantCulture))
				{
					bool flag = initialized;
					//if (mainResponse.ContainsKey("error_message") || mainResponse["error_message"] != "")
					{
						//	m(mainResponse["error_message"]);
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





		//  =============== Datetime Extensions   ================//
		// 
		/*
                static void Main(string[] args) {
                    var mydate = "29021996";
                    var date = mydate.ToDateTime(format: "ddMMyyyy"); // {29.02.1996 00:00:00}
                    mydate = "2016 3";      		 date = mydate.ToDateTime("yyyy M"); // {01.03.2016 00:00:00}
                    mydate = "2016 12";     		 date = mydate.ToDateTime("yyyy d"); // {12.01.2016 00:00:00}
                    mydate = "2016/31/05 13:33";     date = mydate.ToDateTime("yyyy/d/M HH:mm"); // {31.05.2016 13:33:00}
                    mydate = "2016/31 Ocak";         date = mydate.ToDateTime("yyyy/d MMMM"); // {31.01.2016 00:00:00}
                    mydate = "2016/31 January";      date = mydate.ToDateTime("yyyy/d MMMM", cultureString: "en-US");     // {31.01.2016 00:00:00}
                    mydate = "11/?????/1437";        date = mydate.ToDateTime( culture: CultureInfo.GetCultureInfo("ar-SA"),  format: "dd/MMMM/yyyy");   // Weird :) I supposed dd/yyyy/MMMM but that did not work !?$^&*
                    System.Diagnostics.Debug.Assert(  date.Equals(new DateTime(year: 2016, month: 5, day: 18)));
                }


                        foreach (TimeZoneInfo timeZone in TimeZoneInfo.GetSystemTimeZones())	Print(  "'"+ timeZone.DisplayName + "' => '"+ timeZone.Id + "'" );
        */
		public static DateTime ToDateTime(string s)
		{
			try
			{
				return ToDateTime(s, "ddMMyyyy", "");
			}
			catch (Exception e)
			{
				m(e.Message);
				return DateTime.MinValue;
			}
		}
		public static DateTime ToDateTime(string s, string format)
		{
			try
			{
				return ToDateTime(s, format, "");
			}
			catch (Exception e)
			{
				m(e.Message);
				return DateTime.MinValue;
			}
		}
		public static DateTime ToDateTime(string s, string format, string cultureString)
		{
			try
			{  //tr-TR
				CultureInfo _culture = (cultureString == "") ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(cultureString);
				return DateTime.ParseExact(s: s, format: format, provider: _culture);
			}
			catch (FormatException) { throw; }
			catch (Exception) { throw; } // Given Culture is not supported culture
		}


		public static int DateToTime(DateTime dt) { return dt.Hour * 100 + dt.Minute; }


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
					//property.SetValue(this, myAttribute.Value);
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


		public string objectDump2(object obj)
		{
			string output = "";
			foreach (PropertyDescriptor descriptor in System.ComponentModel.TypeDescriptor.GetProperties(obj))
			{
				string name = descriptor.Name;
				object value = descriptor.GetValue(obj);
				output += name + "=" + value;
			}
			return output;
		}

		// https://stackoverflow.com/questions/852181/c-printing-all-properties-of-an-object
		public static string objectDump3(object obj) { return ObjectDumper.Dump(obj); }
		public class ObjectDumper { private int _level; private readonly int _indentSize; private readonly StringBuilder _stringBuilder; private readonly List<int> _hashListOfFoundElements; private ObjectDumper(int indentSize) { _indentSize = indentSize; _stringBuilder = new StringBuilder(); _hashListOfFoundElements = new List<int>(); } public static string Dump(object element) { return Dump(element, 2); } public static string Dump(object element, int indentSize) { var instance = new ObjectDumper(indentSize); return instance.DumpElement(element); } private string DumpElement(object element) { if (element == null || element is ValueType || element is string) { Write(FormatValue(element)); } else { var objectType = element.GetType(); if (!typeof(IEnumerable).IsAssignableFrom(objectType)) { Write("{{{0}}}", objectType.FullName); _hashListOfFoundElements.Add(element.GetHashCode()); _level++; } var enumerableElement = element as IEnumerable; if (enumerableElement != null) { foreach (object item in enumerableElement) { if (item is IEnumerable && !(item is string)) { _level++; DumpElement(item); _level--; } else { if (!AlreadyTouched(item)) DumpElement(item); else Write("{{{0}}} <-- bidirectional reference found", item.GetType().FullName); } } } else { MemberInfo[] members = element.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance); foreach (var memberInfo in members) { var fieldInfo = memberInfo as FieldInfo; var propertyInfo = memberInfo as PropertyInfo; if (fieldInfo == null && propertyInfo == null) continue; var type = fieldInfo != null ? fieldInfo.FieldType : propertyInfo.PropertyType; object value = fieldInfo != null ? fieldInfo.GetValue(element) : propertyInfo.GetValue(element, null); if (type.IsValueType || type == typeof(string)) { Write("{0}: {1}", memberInfo.Name, FormatValue(value)); } else { var isEnumerable = typeof(IEnumerable).IsAssignableFrom(type); Write("{0}: {1}", memberInfo.Name, isEnumerable ? "..." : "{ }"); var alreadyTouched = !isEnumerable && AlreadyTouched(value); _level++; if (!alreadyTouched) DumpElement(value); else Write("{{{0}}} <-- bidirectional reference found", value.GetType().FullName); _level--; } } } if (!typeof(IEnumerable).IsAssignableFrom(objectType)) { _level--; } } return _stringBuilder.ToString(); } private bool AlreadyTouched(object value) { if (value == null) return false; var hash = value.GetHashCode(); for (var i = 0; i < _hashListOfFoundElements.Count; i++) { if (_hashListOfFoundElements[i] == hash) return true; } return false; } private void Write(string value, params object[] args) { var space = new string(' ', _level * _indentSize); if (args != null) value = string.Format(value, args); _stringBuilder.AppendLine(space + value); } private string FormatValue(object o) { if (o == null) return ("null"); if (o is DateTime) return (((DateTime)o).ToShortDateString()); if (o is string) return string.Format("\"{0}\"", o); if (o is char && (char)o == '\0') return string.Empty; if (o is ValueType) return (o.ToString()); if (o is IEnumerable) return ("..."); return ("{ }"); } }



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


		public static bool existsRegistryValue(string path, string key)
		{
			return getRegistryValue(path + key) != null;
		}


		public static string getRegistryValue(string key)
		{
			return getRegistryValue(regPartFromKey(key, 1), regPartFromKey(key, 2));
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




		//new NameValueCollection {  {"param1", "<any> kinds & of = ? strings" },  {"param2", "escaping is already handled" }   }
		public string urlPost(string url, NameValueCollection nv)
		{
			string result;
			try
			{
				using (WebClient webClient = new WebClient())
				{
					byte[] bytes = webClient.UploadValues(url, "POST", nv);
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




		public static Object fileAccessLocker = new Object();
		public static void FileAppend(string file, string text)
		{
			lock (fileAccessLocker)
			{
				System.IO.File.AppendAllText(file, text);
			}
		}

		public static string sendMailCached(string username, string password, string toAddress, string subject, string message, string messageId, string cacheFileName)
		{
			string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/c_sharp_mails/emails_sent/";
			if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
			string filePath = dir + "/" + cacheFileName + "__.txt";
			string prefix = Environment.NewLine;
			string message_Key = prefix + messageId;
			string send = "";
			if (!System.IO.File.Exists(filePath))
			{
				send = "_Trigger_: New file is being created.";
			}
			else
			{
				string content = System.IO.File.ReadAllText(filePath);
				send = !content.Contains(message_Key) ? "_Trigger_: ID is new." : "Message with this ID was already sent";
			}
			if (send.Contains("_Trigger_"))
			{
				System.IO.File.AppendAllText(filePath, message_Key);
				return sendYahooMail(username, password, toAddress, subject, message);
			}
			return send;
		}



		// RUN in TASK.NEW!!! sendYahooMail("yourmail@yahoo.com", "app-password", "someone@gmail.com", "subj", "txtttt");   https://stackoverflow.com/a/54137454/2377343
		public static string sendYahooMail(string username, string password, string toAddress, string subject, string text)
		{
			using (System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient())
			{
				client.Host = "smtp.mail.yahoo.com";
				client.EnableSsl = true;
				client.Port = 587;
				client.UseDefaultCredentials = false;
				client.Credentials = new System.Net.NetworkCredential(username, password);

				using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
				{
					mail.Sender = new System.Net.Mail.MailAddress(username, "First Name");
					mail.From = new System.Net.Mail.MailAddress(username, "Company Name");
					mail.To.Add(new System.Net.Mail.MailAddress(toAddress));
					mail.Subject = subject;
					mail.Body = text;
					mail.IsBodyHtml = true;
					try
					{
						client.Send(mail);
						return "success";
					}
					catch (Exception ex)
					{
						return (ex.Message);
					}

				}
			}
		}


		public static bool sendMailCom(string username, string password, string toMail, string subject, string message)
		{
			try
			{
				using (System.Net.Mail.MailMessage mm = new System.Net.Mail.MailMessage(username, toMail))
				{
					mm.Subject = subject;
					mm.Body = message;
					mm.IsBodyHtml = false;
					System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
					smtp.Host = "smtp.mail.com";
					smtp.EnableSsl = true;
					System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential(username, password);
					smtp.UseDefaultCredentials = false;
					smtp.Credentials = NetworkCred;
					smtp.Port = 587;
					smtp.EnableSsl = false;
					smtp.Send(mm);
				}
				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}

		public static string sendNotificationCached(string message, string messageId, string cacheFileName)
		{
			string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/c_sharp_messages/sent/";
			if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
			string filePath = dir + "/" + cacheFileName + "__.txt";
			string prefix = Environment.NewLine;
			string message_Key = prefix + messageId;
			string send = "";
			if (!System.IO.File.Exists(filePath))
			{
				send = "_Trigger_: New file is being created.";
			}
			else
			{
				string content = System.IO.File.ReadAllText(filePath);
				send = !content.Contains(message_Key) ? "_Trigger_: ID is new." : "Message with this ID was already sent";
			}
			if (send.Contains("_Trigger_"))
			{
				FileAppend(filePath, message_Key);
			}
			return send;
		}


		// SendTelegramCached("XXXXXXXXXXXXX", "-100123123123", "Hi messageee", DateTime.Now.ToString(), "filee");
		public static string SendTelegramCached(string botApiKey, string toChannel, string message, string messageId, string cacheFileName)
		{
			var result = sendNotificationCached(message, messageId, cacheFileName);
			if (result.Contains("_Trigger_"))
			{
				result = SendTelegram("chat_id=" + toChannel + "&text=" + Uri.EscapeDataString(message) + "&parse_mode=html&disable_web_page_preview=true", botApiKey);
			}
			return result;
		}
		public static string SendTelegram(string query, string botApiKey)
		{
			return urlRead("https://api.telegram.org/bot" + botApiKey + "/sendMessage?" + query);  //
		}





		//Action<object,RoutedEventArgs,string>  x= (object sender, RoutedEventArgs e, string index )=>Print("foo") ;  
		//buttons[key].Click += (sender, EventArgs) => {  x(sender, EventArgs, (string) key.ToString()+" Hiii"); };

		// https://ninjatrader.com/support/helpGuides/nt8/en-us/?alert_and_debug_concepts.htm
		//usage: ExceptionPrint(this, System.Reflection.MethodBase, e)
		//Print( "["+GetType().Name+"]["+System.Reflection.MethodBase.GetCurrentMethod().Name+"]-[ERROR at " + mymethodstt.Trace(e) +"] "+  e.ToString());


		public static string sanitize_filename(string dirtyString)
		{
			string invalid = new string(System.IO.Path.GetInvalidFileNameChars()) + new string(System.IO.Path.GetInvalidPathChars());
			foreach (char c in invalid) dirtyString = dirtyString.Replace(c.ToString(), "");
			return dirtyString;
		}



		public static string QueryStringFromDictionary(IDictionary<string, object> dict)
		{
			var list = new List<string>();
			foreach (var item in dict)
			{
				list.Add(item.Key + "=" + item.Value);
			}
			return string.Join("&", list);
		}



        public static string UppercaseFirst(string str)
        {
            var arr = str.ToCharArray(); arr[0] = Char.ToUpperInvariant(arr[0]); return new String(arr);
		}
		public static List<double> ArrayGetLastItems(List<double> list, int amount)
        {
			return Enumerable.Reverse(list).Take(amount).Reverse().ToList();
		}


		public static void ListViewAsListBox(ListView listView)
		{
			listView.View = View.Details;
			listView.HeaderStyle = ColumnHeaderStyle.None;
			listView.FullRowSelect = true;
			listView.Columns.Add("", -2);
		}






		public static int getIndexByKey(Dictionary<string, string> dict, string key)
		{
			List<KeyValuePair<string, string>> list = dict.ToList<KeyValuePair<string, string>>();
			return list.FindIndex((KeyValuePair<string, string> x) => x.Key == key);
		}




		// Order by values.
		//var items = from pair in dict      orderby pair.Value ascending       select pair;
		// foreach (KeyValuePair<string, int> pair in items)  {  Console.WriteLine("{0}: {1}", pair.Key, pair.Value);  }
		//return items.ToDictionary(x => x, x => x);try
		public static Dictionary<string, string> SortDict(Dictionary<string, string> dict)
		{

			return (Dictionary<string, string>)(from entry in dict orderby entry.Value ascending select entry);
			//catch //(Exception e) { return new Dictionary<string, string> { }; }
		}

		public static Dictionary<string, int> SortDict(Dictionary<string, int> dict)
		{
			return dict.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
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



		// https://github.com/jprichardson/FridayThe13th/blob/master/FridayThe13th/JsonParser.cs
		// another: https://pastebin.com/raw/hAtAwA40

		/*
		// referene:    System.Web.Extensions.dll  , #3.5+
		public static string serialize2(object dict)
		{
			if (dict is Dictionary<string, string>) return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(((Dictionary<string, string>)dict).ToDictionary((KeyValuePair<string, string> item) => item.Key.ToString(), (KeyValuePair<string, string> item) => item.Value.ToString()));
			//else Dictionary<string, object> dict
			return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(((Dictionary<string, object>)dict).ToDictionary((KeyValuePair<string, object> item) => item.Key.ToString(), (KeyValuePair<string, object> item) => item.Value.ToString()));
		}



	public static class JsonHelper
	{
		public static string ToJson<T>(T instance)
		{
			var serializer = new DataContractJsonSerializer(typeof(T));
			using (var tempStream = new MemoryStream())
			{
				serializer.WriteObject(tempStream, instance);
				return System.Text.Encoding.Default.GetString(tempStream.ToArray());
			}
		}

		public static T FromJson<T>(string json)
		{
			var serializer = new DataContractJsonSerializer(typeof(T));
			using (var tempStream = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(json)))
			{
				return (T)serializer.ReadObject(tempStream);
			}
		}
	}
		*/


		public static string dictToString(object obj)
		{
			return string.Join("; ", (obj as Dictionary<object, object>).Select(x => x.Key.ToString() + "=" + x.Value.ToString()).ToArray());
		}


		//return ( NetFrameworkVersion() <= 3.5 ?  deserializer_35(str)  : deserializer(str) );
		public static Dictionary<string, object> deserialize(string str)
		{
			return deserializer_35(str);
		}

		public static Dictionary<string, object> deserializer_35(string str)
		{
			try
			{
				return HelperLibraries.JsonMaker.DeserializeJson(str);
			}
			catch (Exception)
			{
				return null;
			}
		}
		public static Dictionary<string, string> deserializer_35_stringed(string str)
		{
			try
			{
				return ConvertToStringDictionary(HelperLibraries.JsonMaker.DeserializeJson(str));
			}
			catch (Exception)
			{
				return null;
			}
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




		#endregion





		// This doesn't work in all of my C# 4.0 and 4.5 projects, so making it conditional
#if Use_BLINK
		public static System.Drawing.Color color_from_hex(string hex_str)
		{
			return (System.Drawing.Color)System.Drawing.ColorTranslator.FromHtml(hex_str);
		}
		//Color.FromArgb(newAlpha, mycolor);   Color.FromArgb(A, color.R, color.G, color.B);
		public static System.Windows.Forms.Timer BlinkTimer;
		public static System.Drawing.Color initialColor;
		public static void blink(bool startOrStop, System.Windows.Forms.Control ctrl,  System.Drawing.Color c1,  System.Drawing.Color c2, int cycleLength_MS, bool BkClr)
		{
			if (ctrl.InvokeRequired)
			{
				ctrl.Invoke(new System.Action(() => blink(startOrStop, ctrl, c1, c2, cycleLength_MS, BkClr)));
			}
			else
			{
				initialColor = BkClr ? ctrl.BackColor : ctrl.ForeColor;
 
				if (startOrStop)
				{
					if (BlinkTimer == null)
					{
						//is called in new thread in eventhandler
						var Blink_timer_Tick = new Action<object, EventArgs>((object sender, EventArgs e)=> { 
							{
								if (BkClr)
								{
									var color = ctrl.BackColor != c1 ? c1 : c2;
									for(var i=0; i<10; i++)
									{ 
										System.Threading.Thread.Sleep(cycleLength_MS/10);
										ctrl.BackColor = System.Drawing.Color.FromArgb((255/10)*i, color.R, color.G, color.B);
									}
									ctrl.BackColor = color;
								}
								else
								{
									var color = ctrl.ForeColor != c1 ? c1 : c2; 
									ctrl.ForeColor = color;
								}
							}
						});
						BlinkTimer = new System.Windows.Forms.Timer();
						BlinkTimer.Interval = cycleLength_MS;
						BlinkTimer.Enabled = false;
						BlinkTimer.Start();
						BlinkTimer.Tick += new EventHandler(Blink_timer_Tick);
					}
				}
				else
				{
					if (BkClr) ctrl.BackColor = initialColor; else ctrl.ForeColor = initialColor;
					BlinkTimer.Stop();
					//BlinkTimer.Tick -= Blink_timer_Tick;
					BlinkTimer = null;
				}
			}
		}
		
#endif

		/*
				public static bool blinkActive = false;
				public static async void Blink_start(System.Windows.Forms.Control ctrl, Color c1, Color c2, short CycleTime_ms, bool BkClr, bool StartOrStop)
				{
					try
					{
						if(StartOrStop)
						{
							blinkActive = true;
							var sw = new Stopwatch(); sw.Start();
							short halfCycle = (short)Math.Round(CycleTime_ms * 0.5);
							while (blinkActive)
							{
								await System.Threading.Tasks.Task.Delay(100);
								var n = sw.ElapsedMilliseconds % CycleTime_ms;
								var per = (double)Math.Abs(n - halfCycle) / halfCycle;
								var red = (short)Math.Round((c2.R - c1.R) * per) + c1.R;
								var grn = (short)Math.Round((c2.G - c1.G) * per) + c1.G;
								var blw = (short)Math.Round((c2.B - c1.B) * per) + c1.B;
								var clr = Color.FromArgb(red, grn, blw);
								if (BkClr) ctrl.BackColor = clr; else ctrl.ForeColor = clr;
							}
							ctrl.BackColor = Color.Transparent;
						}
						else
						{
							blinkActive = false;
						}

					}
					catch (Exception e)
					{
						m(e.Message);
						return;
					}

				}
				*/





		/*
	public  bool propertyExists(object obj_, string propName){
		PropertyInfo propInfo = 
			obj_.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
					.FirstOrDefault(x => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
		if(propInfo != null)
		{
			return true;
			//propInfo.SetValue(table, DateTime.Now);
		} 
		return false;
	}

	public static bool propertyExists_static(object obj_, string propName){
		PropertyInfo propInfo = 
			obj_.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
					.FirstOrDefault(x => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
		if(propInfo != null)
		{
			return true;
			//propInfo.SetValue(table, DateTime.Now);
		} 
		return false;
	}

	public static bool fieldExists(object obj_, string fieldName){
		FieldInfo fieldInfo 
			= obj_.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
					.FirstOrDefault(x => x.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
		if(fieldInfo != null)
		{
			return true;
			//propInfo.SetValue(table, DateTime.Now);
		}
		return false;
	}



	public bool objectContains(object obj_, string propName){
		 return objectContains_static( obj_,  propName);
	}

	public static bool objectContains_static(object obj_, string propName){ 
		return ( propertyExists_static( obj_,  propName)  || fieldExists( obj_,  propName) ) ;
	}




	public dynamic propertyGet( object obj_, string propName, string type){
		return propertyGet_static(  obj_,  propName, type);
	}


	public static dynamic propertyGet_static( object obj_, string propName){
		return propertyGet_static(  obj_,  propName,  "");
	}

	public static dynamic propertyGet_static( object obj_, string propName, string type){

		var propInfo =  obj_.GetType().GetProperty(propName);
		if(propInfo != null)
		{ 
			if( type == "string" ){
				return  ((string) propInfo.GetValue(obj_, null));
			}
			else if( type == "bool"){
				return  ((bool) propInfo.GetValue(obj_, null));
			}
			else if( type == "dictionary_string_string"){
				return  ((Dictionary<string,string>) propInfo.GetValue(obj_, null));
			}
			return (dynamic) propInfo.GetValue(obj_, null)  ;
		}

		var fieldInfo =  obj_.GetType().GetField(propName);
		if(fieldInfo != null)
		{ 
			if( type == "string"){
				return  ((string) fieldInfo.GetValue(obj_));
			}
			else if( type == "bool"){
				return  ((bool) fieldInfo.GetValue(obj_));
			}
			else if( type == "dictionary_string_string"){
				return  ((Dictionary<string,string>) fieldInfo.GetValue(obj_));
			}
			return ((dynamic) propInfo.GetValue(obj_) );
		}

		return "";
	} 

	public static bool propertySet_static(object obj_, string propName, dynamic value){

		PropertyInfo propInfo 
					= obj_.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
					.FirstOrDefault(x => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
		if(propInfo != null)
		{
			propInfo.SetValue(obj_, value);
			return true;
		}


		FieldInfo fieldInfo 
					= obj_.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
					.FirstOrDefault(x => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
		if(propInfo != null)
		{
			fieldInfo.SetValue(obj_, value);
			return true;
		}			

		return false;

	}  
	//============  reflection properties  get/set ===========//

	 // cmd.StandardInput.WriteLine("powershell (Invoke-WebRequest http://ipinfo.io/ip).Content >> %filepath%");
	 */


		//cachedCall(class, "MessageBox", new object[] { par1 }
		public static string cachedCall(string MethodFullName, object[] parameters, int minutes)
		{
			string result = "";
			try
			{
				int idx = MethodFullName.LastIndexOf('.');
				string className = idx < 0 ? MethodBase.GetCurrentMethod().DeclaringType.FullName : MethodFullName.Substring(0, idx);
				Type classType = Type.GetType(className);
				string methodName = idx < 0 ? MethodFullName : MethodFullName.Replace(className + ".", "");

				string fullPathName = className + "." + methodName;
				string key = @"Software\Puvox\cachedCalls\aa" + fullPathName + md5(PuvoxLibrary.Methods.tryEnumerabledString(parameters, "")) + minutes.ToString();
				string cacheKey = key + "_cache";

				if (isCacheSet(cacheKey, minutes))
				{
					result = getRegistryValue(key, "nothin");
				}
				else
				{
					MethodInfo method = classType.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
					if (method == null)
					{
						m("You are trying to trigger non-existing method:" + methodName + "(" + className + ")");
						return "";
					}
					result = (string)method.Invoke(null, parameters);
					setRegistryValue(key, result);
				}
			}
			catch (Exception e)
			{
				m(e);
			}
			return result;
		}


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

		//64 bit detection  https://pastebin.com/raw/yw4gSLK0   |  https://pastebin.com/raw/UdhUteE3   |   https://pastebin.com/raw/9fXbwu9C

		public static bool is64BitOS = Environment.Is64BitOperatingSystem;

		// https://docs.microsoft.com/en-us/dotnet/framework/get-started/system-requirements

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

		public static string regPartFromKey(string key, bool Dir_or_Key)
		{
			if (Dir_or_Key)
			{
				if (charsInPhrase(key, "\\") != 0)
				{
					return withoutLastDir(key, 1);
				}
				return key;
			}
			else
			{
				if (charsInPhrase(key, "\\") != 0)
				{
					return lastPart(key);
				}
				return key;
			}
		}

		public static bool existsRegistryValue(string pathKey)
		{
			string path = regPartFromKey(pathKey, true);
			string key = regPartFromKey(pathKey, false);
			RegistryKey registryHiveKey = getRegistryHiveKey(chosenRegHive);
			if (registryHiveKey == null)
			{
				return false;
			}
			else
			{
				RegistryKey registryKey = registryHiveKey.OpenSubKey(path);
				// if path empty
				if (registryKey == null)
				{
					registryHiveKey.Close();
					return false;
				}
				else
				{
					object value = registryKey.GetValue(key);
					registryKey.Close();
					registryHiveKey.Close();
					return value != null;
				}
			}
		}

		public static string getRegistryValue(string patKey, string defaultValue)
		{
			return getRegistryValue(regPartFromKey(patKey, true), regPartFromKey(patKey, false), defaultValue);
		}

		// // ----- 62 vs 32 :  https://apttech.wordpress.com/2012/01/06/difference-between-a-registry-hive-and-registry-key-2/
		public static string getRegistryValue(string path, string key, object defaultValue)
		{
			try
			{
				//if cached
				if (myregs.ContainsKey(path + key))
				{
					return myregs[path + key];
				}
				else
				{
					RegistryKey registryHiveKey = getRegistryHiveKey(chosenRegHive);
					if (registryHiveKey == null)
					{
						return defaultValue == null ? null : defaultValue.ToString();
					}
					else
					{
						if (path == "")
							path = RegBaseKey;
						RegistryKey registryKey = registryHiveKey.OpenSubKey(path);
						// if path empty
						if (registryKey == null)
						{
							return defaultValue == null ? null : defaultValue.ToString();
							registryHiveKey.Close();
						}
						else
						{
							object value = registryKey.GetValue(key);
							registryKey.Close();
							registryHiveKey.Close();
							if (value == null)
							{
								return defaultValue == null ? null : defaultValue.ToString();
							}
							else
							{
								myregs[path + key] = value.ToString();
								return myregs[path + key];
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				m(e);
			}
			return null;
		}


		public static void setRegistryValue(string patKey, string value)
		{
			setRegistryValue(regPartFromKey(patKey, true), regPartFromKey(patKey, false), value);
		}


		public static bool setRegistryValue(string path, string key, string value)
		{
			bool result;
			try
			{
				RegistryKey registryHiveKey = getRegistryHiveKey(chosenRegHive);
				if (registryHiveKey != null)
				{
					if (path == "")
						path = RegBaseKey;
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
					m("| regHive is null");
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
			return getSetRegistryValue(regPartFromKey(key, true), regPartFromKey(key, false), value);
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
				m(ex.Message + " | getSetRegistryValue " + path + " ->" + key);
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
				m(ex.Message + " | Reading registry " + KeyName.ToUpper());
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
				m(ex.Message + " | Writing registry " + KeyName.ToUpper());
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
				m(ex.Message + " | Deleting SubKey " + subKey.ToUpper());
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
				m(ex.Message + " | Deleting SubKey tree" + subKey.ToUpper());
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
				m(ex.Message + " | Retriving subkeys of" + subKey.ToUpper());
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
				m(ex.Message + " | Retrieving subkeys of" + subKey.ToUpper());
				result = 0;
			}
			return result;
		}

		public static string RegRootOfThisProg = "";
		public static Dictionary<string, string> RegistryValuesInFolder(string keyroot)
		{
			Dictionary<string, string> result;
			try
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(keyroot))
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


		public static bool isCacheSet(string PathKey, int minutes)
		{
			string registryValue = getRegistryValue(PathKey, "");
			if (string.IsNullOrEmpty(registryValue) || DateTime.Now > DateTime.Parse(registryValue).AddMinutes((double)minutes))
			{
				setRegistryValue(PathKey, DateTime.Now.ToString());
				return false;
			}
			return true;
		}
		#endregion



		public string MyDictionaryToJson(Dictionary<string, int> dict)
		{
			var entries = dict.Select(d =>
				string.Format("\"{0}\": [{1}]", d.Key, string.Join(",", d.Value)));
			return "{" + string.Join(",", entries) + "}";
		}


		public string regplace(string input, string pattern)
		{
			return regplace(input, pattern, "");
		}

		public string regplace(string input, string pattern, string with)
		{
			return new System.Text.RegularExpressions.Regex(pattern).Replace(input, "");
		}


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

    
		#endregion
		*/



		/*
        public Dictionary<string, string> CopyDictionary(Dictionary<string, string> dict)
        {
            return new Dictionary<string, string>(dict);  // //return dict.ToDictionary(entry => entry.Key,   entry => entry.Value);
        }
        public Dictionary<string, int> CopyDictionary(Dictionary<string, int> dict)
        {
            return new Dictionary<string, int>(dict);  // //return dict.ToDictionary(entry => entry.Key,   entry => entry.Value);
        }
        public Dictionary<string, double> CopyDictionary(Dictionary<string, double> dict)
        {
            return new Dictionary<string, double>(dict);  // //return dict.ToDictionary(entry => entry.Key,   entry => entry.Value);
        }
        public Dictionary<string, bool> CopyDictionary(Dictionary<string, bool> dict)
        {
            return new Dictionary<string, bool>(dict);  // //return dict.ToDictionary(entry => entry.Key,   entry => entry.Value);
        }
        */


		//(int)Enum.Parse(typeof(TestAppAreana.MovieList.Movies), KeyVal);



		// // ----- 62 vs 32 :  https://apttech.wordpress.com/2012/01/06/difference-between-a-registry-hive-and-registry-key-2/



		// https://stackoverflow.com/questions/14967618/deserialize-json-to-class-manually-with-reflection/50492864#50492864
		//new update here, but doesnt work well : http://qaru.site/questions/6250657/deserialize-json-to-class-manually-with-reflection

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
			// System.Web.HttpUtility.UrlEncode
			return Uri.EscapeDataString(msg); //.Replace("%20", "+"); //4.5+ WebUtility.UrlEncode
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

		public static int GenerateRandom(int min, int max)
		{
			return Random(min, max);
		}
		public static int Random(int min, int max)
		{
			var seed = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(Guid.NewGuid().ToString(), @"\d+").Value);
			return new Random(seed).Next(min, max);
		}


		public static string DateToSeconds(DateTime date)
		{
			return date.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
		}
		public static string DateString(DateTime date)
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



		// https://stackoverflow.com/a/6457416/2377343    // https://stackoverflow.com/questions/1024070/c-sharp-eval-support

		//  MessageBox.Show(evaluate(new string[] { "system.dll", "system.xml.dll", "system.windows.forms.dll", "system.drawing.dll" }, "string", "return \"Hello World\"").ToString() );
		public static object evaluate(string[] dlls, string returnType, string sCSCode)  //
		{
			Microsoft.CSharp.CSharpCodeProvider c = new Microsoft.CSharp.CSharpCodeProvider();
			System.CodeDom.Compiler.ICodeCompiler icc = c.CreateCompiler();
			System.CodeDom.Compiler.CompilerParameters cp = new System.CodeDom.Compiler.CompilerParameters();

			if (dlls == null || dlls.Length == 0)
				cp.ReferencedAssemblies.Add("system.dll");
			else foreach (var str in dlls)
					cp.ReferencedAssemblies.Add(str);

			cp.CompilerOptions = "/t:library";
			cp.GenerateInMemory = true;

			//  using System.Xml;  using System.Data; using System.Data.SqlClient; using System.Windows.Forms; using System.Drawing;
			string content = @"
                using System;  
                namespace CSCodesAddition{
                    public class CSCodeMethods{
                        public " + returnType + @" Execute(){
                            " + sCSCode + @";
                        }
                    }
                }"
			;
			System.CodeDom.Compiler.CompilerResults cr = icc.CompileAssemblyFromSource(cp, content);
			if (cr.Errors.Count > 0)
			{
				System.Windows.Forms.MessageBox.Show("ERROR: " + cr.Errors[0].ErrorText, "Error evaluating .cs code", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return null;
			}
			System.Reflection.Assembly a = cr.CompiledAssembly;
			object o = a.CreateInstance("CSCodesAddition.CSCodeMethods");
			System.Reflection.MethodInfo mi = o.GetType().GetMethod("Execute");
			object s = mi.Invoke(o, null);
			return s;

		}




		public static Dictionary<string, string> xml_to_dictionary(string xml_data)
		{
			try
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
			catch (Exception e)
			{
				m(e.Message);
				return new Dictionary<string, string>();
			}
		}









		private static object fileCheckObj = new object();
		public static string errorLogFile = Environment.GetEnvironmentVariable("tmp") + @"\_errorlogs_c#.log";

		public static void Dispose(IDisposable resource) { if (resource != null) resource.Dispose(); }



		#region Control region Form manipulations

		private delegate void SetTextCallback(Form f, System.Windows.Forms.Control ctrl, string text);


		public static void PopupMessage(object obj_)
		{
			Form form = new Form();
			form.Width = 310;
			form.Height = 450;
			form.Text = "Popup Message";
			form.TopMost = true;
			form.StartPosition = FormStartPosition.Manual;
			controlSetLocation(form, 1, 1);
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
				m(ex.Message);
			}
		}


		public static bool isUserInput(System.Windows.Forms.Control ct)
		{
			return ct is System.Windows.Forms.TextBox || ct is System.Windows.Forms.ComboBox || ct is System.Windows.Forms.CheckBox || ct is CheckedListBox || ct is DateTimePicker || ct is System.Windows.Forms.GroupBox || ct is System.Windows.Forms.ListBox || ct is MonthCalendar || ct is System.Windows.Forms.RadioButton || ct is System.Windows.Forms.RichTextBox || ct is TrackBar;
		}


		public static string optionsPrefix = "formoption_";


		public static string getFormOption(string key, string two, string regKeyBase)
		{
			return getRegistryValue(regKeyBase + optionsPrefix + key, two);
		}
		private void button_opts_save_Click(Form form)
		{
			foreach (Control c in form.Controls)
			{
				//pm.setRegistryValue(c.Name, c.Text);
			}
			m("saved");
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

		// public void invokeSafe(Action callback, bool forceSynchronous) { invokeSafe(this, callback, true); }
		// compare the thread ID of the calling thread to the thread ID of the creating thread.
		public static object invokeSafe(System.Windows.Forms.Control uiElement, Func<object> callback, bool forceSynchronous)
		{
			try
			{
				if (uiElement != null)
				{
					if (uiElement.InvokeRequired)
					{
						if (forceSynchronous)
						{
							return uiElement.Invoke(new Func<object>(delegate ()
							{
								return (object)callback();//  invokeSafe(uiElement, callback, forceSynchronous);
							}));
						}
						else
						{
							uiElement.BeginInvoke(new Func<object>(delegate ()
							{
								return (object)callback(); //invokeSafe(uiElement, callback, forceSynchronous);
							}));
						}
					}
					else if (!uiElement.IsDisposed)
					{
						return ((Func<object>)callback)();
					}
				}
			}
			catch (Exception ex)
			{
				m(ex);
			}
			return null;
		}

		public static void invokeSafe(System.Windows.Forms.Control uiElement, Action callback, bool forceSynchronous)
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
								callback();//invokeSafe(uiElement, callback, forceSynchronous);
							}));
						}
						else
						{
							uiElement.BeginInvoke(new Action(delegate ()
							{
								callback(); //invokeSafe(uiElement, callback, forceSynchronous);
							}));
						}
					}
					else if (!uiElement.IsDisposed)
					{
						callback();
					}
				}
			}
			catch (Exception ex)
			{
				m(ex);
			}
		}


		private delegate bool Control_SetDelegate(System.Windows.Forms.Control ctrl, string what, object value);
		public static bool Control_Set(System.Windows.Forms.Control ctrl, string what, object value)
		{
			return Control_Set(ctrl, what, value, false);
		}
		public static bool Control_Set(System.Windows.Forms.Control ctrl, string what, object value, bool append) //apend does not work!
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
						Control_SetDelegate method = new Control_SetDelegate(Control_Set);
						ctrl.Invoke(method, new object[] { ctrl, what, value, append });
					}
					else if (what == "visibility")
					{
						ctrl.Visible = (bool)value;
					}
					else if (what == "text")
					{
						ctrl.Text = (append ? ctrl.Text : "") + value.ToString();
					}
					else
					{
						m("incorrect value in :" + MethodBase.GetCurrentMethod().Name + ":" + what);
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
		 
		private void copyableListView(ListView listView)
		{
			listView.KeyDown += (object sender, System.Windows.Forms.KeyEventArgs e) =>
			{
				if (!(sender is ListView)) return;

				if (e.Control && e.KeyCode == Keys.C)
				{
					var builder = new StringBuilder();
					foreach (ListViewItem item in (sender as ListView).SelectedItems)
						builder.AppendLine(item.Text + Environment.NewLine);
					System.Windows.Forms.Clipboard.SetText(builder.ToString());
				}
			};
		}


		public static string GetControlText(Form obj, object which)
		{
			if (which is Control)
			{
				Control which_ = (Control)which;
				if (which_.InvokeRequired)
				{

					return which_.Invoke(new Func<string>(() => { return which_.Text; })).ToString();
				}
				else
				{
					return which_.Text;
				}
			}
			else
			{
				string which_ = (string)which;
				if (obj.InvokeRequired)
				{
					return obj.Invoke(new Func<string>(() => {
						System.Windows.Forms.Control[] array = obj.Controls.Find(which_, true);
						return (array.Length <= 0) ? "cant find control" : array[0].Text;
					})).ToString();
				}
				else
				{
					System.Windows.Forms.Control[] array = obj.Controls.Find(which_, true);
					return (array.Length <= 0) ? "cant find control" : array[0].Text;
				}
			}
		}


		// BASIC GET & SET
		public static string controlValueGet(Control control)
		{
			string value;
			//checkbox checking need to be saved as string
			if (control is System.Windows.Forms.CheckBox)
			{
				value = ((System.Windows.Forms.CheckBox)control).Checked ? "true" : "false";
			}
			else
			{
				value = control.Text;
			}
			return value;
		}
		public static bool controlValueSet(Control control, string value)
		{
			//checkbox checking need to be obtained as string
			if (control is System.Windows.Forms.CheckBox)
			{
				(control as System.Windows.Forms.CheckBox).Checked = value == "true";
			}
			else
			{
				control.Text = value;
			}
			return true;
		}
		// #############
		public static List<Control> GetAllControls(Control container)
		{
			List<Control> ControlList = new List<Control>();
			foreach (Control control in container.Controls)
			{
				if (control.Controls.Count > 0)
				{
					ControlList.AddRange(GetAllControls(control));
				}

				ControlList.Add(control);
			}
			return ControlList;
		}

		public static Control ControlGetByName(Control form, string name)
        {
			var allCtrls = GetAllControls(form);
			Control result = null;
			foreach (Control control in allCtrls)
			{
				if (control.Name == name)
				{
					result = control;
				}
				if (control.Controls.Count > 0)
				{
					var result1 = ControlGetByName(control, name);
					if (result1 != null)
						result = result1;
				}
			}
			return result;
		}

		public static void SetControlTexts_Translated(Form form, Dictionary<string, Dictionary<string, string>> controlsTexts, string selectedLanguage)
		{
			foreach (KeyValuePair<string, Dictionary<string, string>> kv in controlsTexts)
			{
				string key = kv.Key;
				if (kv.Value.ContainsKey(selectedLanguage))
					SetControlText(form, key, kv.Value[selectedLanguage]);
				else if (kv.Value.ContainsKey("en"))
					SetControlText(form, key, kv.Value["en"]);
			}
		}


		public static bool GetUninstallString2(string ProductName)
		{
			try
			{
				RegistryKey localKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
				var key = localKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall") ??
					localKey.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");

				if (key == null)
					return false;
				foreach (var x in key.GetSubKeyNames()
						.Select(keyName => key.OpenSubKey(keyName))
						.Select(subkey => subkey.GetValue("DisplayName") as string))
					Debug.WriteLine(x);

				return key.GetSubKeyNames()
						.Select(keyName => key.OpenSubKey(keyName))
						.Select(subkey => subkey.GetValue("DisplayName") as string)
						.Any(displayName => displayName != null && displayName.Contains(ProductName));
			}
			catch
			{
				// Log message                  
				return false;
			}
		}

		public static string GetUninstallString1(string msiName, bool exactMatch_Or_Contains)
		{
			string uninstallString = string.Empty;
			try
			{
				string path = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Installer\\UserData\\S-1-5-18\\Products";

				RegistryKey key = Registry.LocalMachine.OpenSubKey(path); //RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(path)
				foreach (string tempKeyName in key.GetSubKeyNames())
				{
					RegistryKey tempKey = key.OpenSubKey(tempKeyName + "\\InstallProperties");
					if (tempKey != null)
					{
						var name = tempKey.GetValue("DisplayName").ToString();
						if ((exactMatch_Or_Contains && name == msiName) || (!exactMatch_Or_Contains && name.Contains(msiName)))
						{
							uninstallString = Convert.ToString(tempKey.GetValue("UninstallString")); //uninstallString.Replace("/I", "/X");////+= " /quiet /qn";
							uninstallString = uninstallString.Replace("MsiExec.exe", "").Replace(" /X", "").Trim();
							break;
						}
					}
				}
				return uninstallString;
			}
			catch (Exception ex)
			{
				throw new ApplicationException(ex.Message);
			}
		}


		private Dictionary<string, string> gtranslate_dict;
		public static string gTranslate(string what, string toLanguage, bool useApi = false, string ApiURL = "")
		{
			var fromLanguage = "en";
			string result = what;
			if (useApi)
			{
				string text3 = urlRead(ApiURL + "&action=translate&lang=" + toLanguage + "&string[]=" + urlEncode(what) + DeveloperMachineAddition());

				//if (deserialize(text3, ref _dict) && _dict.ContainsKey(what))
				//text2 = _dict[what];
			}
			else
			{
				// https://stackoverflow.com/questions/26714426/what-is-the-meaning-of-google-translate-query-params


				var url = @"https://translate.googleapis.com/translate_a/single?client=gtx&sl=" + fromLanguage + "&tl=" + toLanguage + "&dt=t&q=" + urlEncode(what);
				var content = urlRead(url);





				Match match = Regex.Match(content, "\\[\\[\\[\"(.*?)\",\"");
				if (match.Success)
				{
					result = match.Groups[1].Value.Replace("\\", "");

				}
				/*
				try
				{
					result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
					return result;
				}
				catch
				{
					return word;
				}
				*/
			}
			return result;
		}



		public static void dumpPopup(object obj_)
		{
			PopupForm_(dump(obj_));
		}


		/*
        private void texbox1_TextChanged(object sender, EventArgs e)
        {
            saveCallback(sender);
        }
        //
        private void setKeys()
        {
            foreach (Control ctl in this.GetAllControls(this))
            {
                if (ctl is TextBox)
                    setKey(ctl);
            }
        }
        string tempFile(string which) { return Path.GetTempPath() + "td_api_temp_" + which; }
        private string getKey(string which)
        {
            string file = tempFile(which);
            if (!File.Exists(file)) File.WriteAllText(file, "");
            return File.ReadAllText(file);
        }
        private void setKey(Control ctl)
        {
            ctl.Text = getKey(ctl.Name);
        }
        private void saveCallback(object sender)
        {
            var tb = (sender as Control);
            File.WriteAllText(tempFile(tb.Name), (sender as Control).Text);
        }


        public  List<Control> GetAllControls(Control container)
        {
            List<Control> ControlList = new List<Control>();
            foreach (Control control in container.Controls)
            {
                if (control.Controls.Count > 0)
                {
                    ControlList.AddRange(GetAllControls(control));
                }

                ControlList.Add(control);
            }
            return ControlList;
        }
		*/


		public static void FormOption_Fill(Control control, string regKeyBase)
		{
			if (control != null)
			{
				invokeSafe(control, () =>
				{
					if (isUserInput(control)) 

					{
						string targetName = control.Name;
						if (targetName.StartsWith("option_"))
						{
							string regValue = getRegistryValue(regKeyBase + optionsPrefix + control.Name, control.Text).ToLower();
							controlValueSet(control, regValue);
						}
					}
				}, true);
			}
		}
		public static bool FormOptions_Fill(object FormOrControlCollection, string regKeyBase)
		{
			if (FormOrControlCollection is Form)
			{
				foreach (Control ctl in GetAllControls((Form)FormOrControlCollection))
				{
					FormOption_Fill(ctl, regKeyBase);
				}
			}
			if (FormOrControlCollection is System.Windows.Forms.Control.ControlCollection)
			{
				foreach (object obj in (System.Windows.Forms.Control.ControlCollection)FormOrControlCollection)
				{
					if (obj is Control)
					{
						System.Windows.Forms.Control control = (System.Windows.Forms.Control)obj;
						FormOption_Fill(control, regKeyBase);
					}
				}
			}
			return true;
		}
		public static bool saveFormOptions(System.Windows.Forms.Control.ControlCollection cts, string regKeyBase)
		{
			foreach (object obj in cts)
			{
				System.Windows.Forms.Control control = (System.Windows.Forms.Control)obj;
				if (control != null && isUserInput(control))
				{
					string value = control.Text;
					//checkbox checking need to be saved as string
					if (control is System.Windows.Forms.CheckBox) value = ((System.Windows.Forms.CheckBox)control).Checked ? "true" : "false";
					setRegistryValue(regKeyBase + optionsPrefix + control.Name, value);

				}
			}
			return true;
		}

		public bool saveFormOptions_old(System.Windows.Forms.Control.ControlCollection cts)
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


		public static bool fillFormOptions(System.Windows.Forms.Control.ControlCollection cts, string regKeyBase)
		{
			foreach (object obj in cts)
			{
				System.Windows.Forms.Control control = (System.Windows.Forms.Control)obj;
				if (control != null && isUserInput(control))
				{
					string defaultVal = control.Text;
					string regValue = getRegistryValue(regKeyBase + optionsPrefix + control.Name, defaultVal).ToLower();
					//checkbox checking need to be obtained as string
					if (control is System.Windows.Forms.CheckBox)
					{
						(control as System.Windows.Forms.CheckBox).Checked = regValue == "true";
					}
					else
					{
						control.Text = getRegistryValue(regKeyBase + optionsPrefix + control.Name, regValue);

					}
				}
			}
			return true;
		}




		public static void FormOption_Save(Control control, string regKeyBase)
		{
			if (control != null)
			{
				invokeSafe(control, () =>
				{
					if (isUserInput(control))
					{
						string targetName = control.Name;
						if (targetName.StartsWith("option_"))
						{
							setRegistryValue(regKeyBase + optionsPrefix + control.Name, controlValueGet(control));
						}
					}
				}, true);
			}
		}

		public static bool FormOptions_Save(object FormOrControlCollection, string regKeyBase)
		{
			if (FormOrControlCollection is Form)
			{
				foreach (Control ctl in GetAllControls((Form)FormOrControlCollection))

				{
					FormOption_Save(ctl, regKeyBase);
				}
			}
			if (FormOrControlCollection is System.Windows.Forms.Control.ControlCollection)
			{
				foreach (object obj in (System.Windows.Forms.Control.ControlCollection)FormOrControlCollection)
				{
					if (obj is Control)
					{
						System.Windows.Forms.Control control = (System.Windows.Forms.Control)obj;
						FormOption_Save(control, regKeyBase);
					}
				}
			}
			return true;
		}



		// public static string AppName = "my_sample_app";
		// public void formOptionsSaveLoad(bool SaveOrRestore) { formOptionsSaveLoad(this, SaveOrRestore, AppName); }
		// text should contain "option_"
		public static void formOptionsSaveLoad(Form form, bool SaveOrRestore, string optionsFileName)
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
			string optionsFilePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\options_" + appName() + " " + (optionsFileName != "" ? optionsFileName : form.GetType().Name);
			string initialXml = "<all_options></all_options>";
			if (!File.Exists(optionsFilePath)) { File.WriteAllText(optionsFilePath, initialXml); }
			if (SaveOrRestore)
			{
				doc.LoadXml(initialXml);
				doc.PreserveWhitespace = true;
			}
			else
			{
				doc.LoadXml(File.ReadAllText(optionsFilePath));
			}
			foreach (Control control in GetAllControls(form))
			{
				string targetName = control.Name;
				if (targetName.StartsWith("option_"))
				{
					if (SaveOrRestore)
					{
						System.Xml.XmlElement newElem = doc.CreateElement(targetName);
						newElem.InnerText = controlValueGet(control);
						doc.DocumentElement.AppendChild(newElem);
					}
					else
					{
						var xmlNodes = doc.GetElementsByTagName(targetName);
						if (xmlNodes.Count > 0)
						{
							string val = xmlNodes[0].InnerText;
							controlValueSet(control, val);

						}
					}
				}
			}
			if (SaveOrRestore)
			{
				doc.Save(optionsFilePath);
			}
		}

		public static void FillIfEmpty(Control ctrl, string str)
		{
			if (ctrl.Text == "") ctrl.Text = str;
		}

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

		// 			 Timer( () => { }, 30 * 1000, false);
		//i.e.        Timer( new Action<object, System.Timers.ElapsedEventArgs>((object sender, System.Timers.ElapsedEventArgs e) => { }), 30 * 1000, false);
		// 			 Timer( new Action<object, object>((object sender, object e) => { }), 30 * 1000, false);
		public static System.Timers.Timer Timer(Action myMethod, int intervalMS, bool autoreset)
		{
			System.Timers.Timer timer = new System.Timers.Timer();
			//timer.Elapsed += myMethod.Invoke;   //Action<object, System.Timers.ElapsedEventArgs> myMethod
			//timer.Elapsed += new System.Timers.ElapsedEventHandler((object a, System.Timers.ElapsedEventArgs b) => myMethod());
			timer.Elapsed += new Action<object, System.Timers.ElapsedEventArgs>((object a, System.Timers.ElapsedEventArgs b) => myMethod()).Invoke;
			timer.Interval = (double)intervalMS;
			timer.AutoReset = autoreset;
			timer.Start();
			return timer;
		}
		// Standalone:
		// System.Threading.Tasks.Task.Delay(3000).ContinueWith(t => init());
		//                     or  
		// ExecuteAfter(() => MessageBox.Show("hi"), 1000 ); 
		// SetTimeout(1000,   () => { MessageBox.Show("hi");  }   );
		#endregion






		public class DateUtils
		{
			public static DateTime ParseGMTString(string gmtStr)
			{
				return DateTime.ParseExact(gmtStr, "yyyy-MM-dd HH:mm:ss Z", CultureInfo.InvariantCulture);
			}

			public static DateTime ParseDatePartFromGMTString(string gmtStr)
			{
				return DateTime.ParseExact(gmtStr.Substring(0, 10), "yyyy-MM-dd", CultureInfo.InvariantCulture);
			}

			public static bool IsMidnightOfToday(DateTime time)
			{
				DateTime now = DateTime.Now;
				return time.Year == now.Year && time.Month == now.Month && time.Day == now.Day && time.Hour == 0 && time.Minute == 0 && time.Second == 0;
			}

			public static DateTime ConvertToUtc(DateTime time, bool unspecificAsLocal)
			{
				if (time.Kind == DateTimeKind.Utc)
				{
					return time;
				}
				if (time.Kind == DateTimeKind.Unspecified)
				{
					return new DateTime(time.Ticks, unspecificAsLocal ? DateTimeKind.Local : DateTimeKind.Utc).ToUniversalTime();
				}
				if (time.Kind == DateTimeKind.Local)
				{
					return time.ToUniversalTime();
				}
				throw new Exception(string.Format("Unsupported DateTime.Kind {0}", time.Kind));
			}

			public static long ConvertToTimestamp(DateTime time, bool unspecificAsLocal)
			{
				return (long)DateUtils.ConvertToUtc(time, unspecificAsLocal).Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
			}

			public static long ConvertToTimestampMili(DateTime time, bool unspecificAsLocal)
			{
				return (long)DateUtils.ConvertToUtc(time, unspecificAsLocal).Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
			}

			public static DateTime TimestampToUtcDateTime(long timestamp)
			{
				return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds((double)timestamp);
			}

			public static DateTime TimestampMiliToUtcDateTime(long timestampMili)
			{
				return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds((double)timestampMili);
			}

			public static DateTime TimestampToUtcDateTimeUnspecified(long timestamp)
			{
				return new DateTime(DateUtils.TimestampToUtcDateTime(timestamp).Ticks, DateTimeKind.Unspecified);
			}

			public static DateTime TimestampMiliToUtcDateTimeUnspecified(long timestampMili)
			{
				return new DateTime(DateUtils.TimestampMiliToUtcDateTime(timestampMili).Ticks, DateTimeKind.Unspecified);
			}

			public static DateTime TimestampToLocalDateTime(long timestamp)
			{
				return DateUtils.TimestampToUtcDateTime(timestamp).ToLocalTime();
			}

			public static DateTime TimestampMiliToLocalDateTime(long timestampMili)
			{
				return DateUtils.TimestampMiliToUtcDateTime(timestampMili).ToLocalTime();
			}

			public static DateTime TimestampToLocalDateTimeUnspecified(long timestamp)
			{
				return new DateTime(DateUtils.TimestampToLocalDateTime(timestamp).Ticks, DateTimeKind.Unspecified);
			}

			public static DateTime TimestampMiliToLocalDateTimeUnspecified(long timestampMili)
			{
				return new DateTime(DateUtils.TimestampMiliToLocalDateTime(timestampMili).Ticks, DateTimeKind.Unspecified);
			}

			public static long epochMS(){
				return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
				// TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
				// int secondsSinceEpoch = (int)t.TotalSeconds;
				// return (UInt64)secondsSinceEpoch * 1000;
			}
		}









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
			try
			{
				System.Net.ServicePointManager.Expect100Continue = true;
				System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
				var request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
				request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E)";
				using (var response = request.GetResponse())
				{
					return new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
				}
			}
			catch (System.Net.WebException ex)
			{
				return ex.Response == null ? "" : new System.IO.StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
			}
			catch (Exception e)
			{
				return "UrlRead Error:" + e.Message;
			}
		}

		//(System.IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/debug.txt"))
		public static string urlRead2(string url)
		{
			string responseText = "-1";
			try
			{
				// HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
				// //req.AutomaticDecompression = DecompressionMethods.GZip;
				// using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
				// using (StreamReader reader = new StreamReader(res.GetResponseStream(), ASCIIEncoding.ASCII))
				// responseText = reader.ReadToEnd(); 
				using (System.Net.WebClient wc = new System.Net.WebClient())
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


		// very slow
		public static string urlPost(string url)
		{
			string result;
			try
			{
				// System.Net.WebRequest.DefaultWebProxy = null;
				// System.Net.ServicePointManager.DefaultConnectionLimit = 1000;
				using (var webClient = new System.Net.WebClient())
				{
                    webClient.Proxy = null;
					byte[] bytes = webClient.UploadValues(url, "POST", new System.Collections.Specialized.NameValueCollection
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

		#if RESTSHARP
		public static string urlPost(string url, Dictionary<string,string> dict)
        {
            var client = new RestClient(url);
            // client.Authenticator = new HttpBasicAuthenticator(username, password);
            var request = new RestRequest();
            foreach (var kvp in dict)
                request.AddParameter(kvp.Key, kvp.Value);
            //request.AddHeader("header", "value");
            var response = client.Post(request);  //client.Post<Person>(request);  --.response2.Data.Name;
            var content = response.Content; // Raw content as string
            return content;
        }
		#endif

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
					m(messageShow + ex.Message);
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

		public static bool WriteTempFile(string fileName, string text, bool append)
		{
			string path = Environment.GetEnvironmentVariable("tmp") + "\\" + fileName + ".txt";
			return FileWrite(path, text, append);
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
		private static readonly object fileLock = new object();
		private static bool FileWrite(string filename, string text, bool append = false)
		{
			try
			{
				if (string.IsNullOrEmpty(text))
				{
					return false;
				}
				lock (fileLock)
				{
					using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filename, append, System.Text.Encoding.UTF8))
					{
						writer.WriteLine(text);
					}
					return true;
				}
			}
			catch
			{
				throw;
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
		public static string getKeyFromIsoLang(string isoLang)
		{
			var parts = isoLang.Split('-'); //i.e.  en-US
			return parts[0];
		}



		public static string gsApkPath = "";
		public static void googleSetKeyFile(string FileContent)
		{
			gsApkPath = Environment.GetEnvironmentVariable("appdata") + "\\gsapk.xyz";
			File.WriteAllText(gsApkPath, FileContent);
			SetGoogleCredentialsDefaults(gsApkPath);
		}


		public static void SetGoogleCredentialsDefaults(string path)
		{
			Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
		}




		public static void setTexts(Dictionary<string, string> dict, string lang_2char, Form frm)
		{
			foreach (KeyValuePair<string, string> v in dict)
			{
				string keyNm_this = "trans_" + v.Value + "_" + lang_2char;    // keyname for that element
				string keyNm_ENG = "trans_" + v.Value + "_en";                // keyname for that element
				string val_this = getRegistryValue(keyNm_this, "");              // value from registry for element 
				string val_ENG = getRegistryValue(keyNm_ENG, "");              // value from registry for element 
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
						val_this = v.Key; //GTranslate_checker(v.Key, lang_2char);
					}

					setRegistryValue(keyNm_this, val_this);
				}
				SetControlText(frm, v.Value, val_this);
			}
		}

		public static string GTranslate_checker(string what, string lang_target)
		{
			string result;
			if (MethodExists(null, "GTranslate_callback"))
			{
				Type typeFromHandle = typeof(Methods);
				MethodInfo method = typeFromHandle.GetMethod("GTranslate_callback", BindingFlags.Instance | BindingFlags.Public);
				result = (string)method.Invoke(null, new object[]  //this instead of null
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

		public static Dictionary<string, Dictionary<string, string>> transl_texts = new Dictionary<string, Dictionary<string, string>>();
		public static string translationsBaseReg = @"Software\Puvox\TranslatedTexts\";
		public static string translate(string targetLang, string englishText)
		{
			string result = "";
			//cache
			if (!transl_texts.ContainsKey(englishText)) transl_texts[englishText] = new Dictionary<string, string>();

			if (transl_texts[englishText].ContainsKey(targetLang))
			{
				return transl_texts[englishText][targetLang];
			}
			else
			{
				string key = translationsBaseReg + "t_" + md5(englishText) + "_" + targetLang;
				if (targetLang != "en")
				{
					string registryValue = getRegistryValue(key, "");
					if (string.IsNullOrEmpty(registryValue))
					{
						result = gTranslate(englishText, targetLang);
						setRegistryValue(key, result);
					}
					else
					{
						result = registryValue;
					}
				}
				else
				{
					result = englishText;
					setRegistryValue(key, englishText);
				}
				transl_texts[englishText][targetLang] = result;
			}
			return result;
		}



		#endregion











		public static bool IsDeveloperMachine()
		{
			return Environment.GetEnvironmentVariable(developerMachineString) != null;
		}

		public static string DeveloperMachineAddition()
		{
			if (IsDeveloperMachine())
			{
				return "&developerKey=" + Environment.GetEnvironmentVariable(developerMachineString).ToString();
			}
			return "";
		}



		public static string removeWhitespaces(string str)
		{
			return str.Replace("      ", " ").Replace("     ", " ").Replace("    ", " ").Replace("   ", " ").Replace("  ", " ").Replace(Environment.NewLine, "").Replace("\t", "");
		}

		public static string PregReplace( String input, string pattern, string replacement)
		{
			input = Regex.Replace(input, pattern, replacement);

			return input;
		}



		#region debugger
		public static void m(int obj) { m(obj.ToString()); }
		public static void m(double obj) { m(obj.ToString()); }
		public static void m(bool obj) { m(obj.ToString()); }
		public static void m(Exception obj) { m(obj.ToString()); }
		public static void m(Dictionary<object, object> obj) { try { m(tryEnumerabledString(obj)); } catch { m(obj.ToString()); } }
		public static void m(object obj) { m(obj == null ? "object is null" : obj.ToString()); }
		public static void m(object obj, bool showTrace) { m((obj == null ? "object is null" : obj.ToString()) + (!showTrace ? "" : Environment.NewLine + stackFramesString(new StackTrace()))); }
		public static void m(object[] obj)
		{
			if (obj == null) { m("null"); return; }
			string text = ""; foreach (object obj2 in obj) text = text + ((obj2 == null) ? "null" : obj.ToString()) + Methods.nl_;
			m(text);
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


		public static void cl(object obj) { Console.WriteLine(obj == null ? "null" : obj.ToString()); System.Diagnostics.Debug.WriteLine(obj == null ? "null" : obj.ToString()); }


		// another kind of dumper (removed):  https://pastebin.com/KEzWthMp


		public static void dumpmsg(object obj) { m(dump(obj)); }
		public static string dump(object obj) { return dump(obj, AllBindingFlags, false, 1, 1, ""); }
		public static string dump(object obj, int deep) { return dump(obj, AllBindingFlags, false, deep, 1, ""); }
		public static string dump(object obj, bool execMethods) { return dump(obj, AllBindingFlags, execMethods, 1, 1, ""); }
		public static string dump(object obj, int deep, bool execMethods) { return dump(obj, AllBindingFlags, execMethods, deep, 1, ""); }
		public static string dump(object obj, BindingFlags bindingFlags, bool execMethods) { return dump(obj, bindingFlags, execMethods, 1, 1, ""); }
		// ugly-fied




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
					return "GetOnly:" + AccessModifierType(propertyInfo.GetMethod);
				}
				if (propertyInfo.GetMethod == null)
				{
					return "SetOnly:" + AccessModifierType(propertyInfo.SetMethod);
				}
				return AccessModifierType(propertyInfo.GetMethod) + " & " + AccessModifierType(propertyInfo.GetMethod);
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
		public static string tryEnumerabledString(object obj) { return tryEnumerabledString(obj, ""); }
		public static string tryEnumerabledString(object obj, string prefix_)
		{
			string text = "";
			try
			{
				IEnumerable enumerable = obj as IEnumerable;
				if (enumerable != null)
				{
					bool prefixed = false;
					foreach (object obj2 in enumerable)
					{
						text = text + ((!prefixed && !prefix_.Contains(nl_)) ? "" : prefix_) + " ::: " + (obj2==null ? "null" : obj2.ToString());
						prefixed = true;
					}
				}
			}
			catch (Exception ex)
			{
				text = text + "[cant enumerate:" + ex.Message + "]";
			}
			return text;
		}
		/*
		public static IEnumerable<MemberInfo> GetMembers(Type type, bool getStatic=true, bool getPrivate=true, bool getBases=true) {
			var memberList = ImmutableList<MemberInfo>.Empty;
			if (type == typeof(Object)) return memberList;

			var bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;
			if (getStatic) bindingFlags |= BindingFlags.Static;
			if (getPrivate) bindingFlags |= BindingFlags.NonPublic;

			memberList = memberList.AddRange(type.GetMembers(bindingFlags));
			if (getBases) memberList = memberList.AddRange(GetMembers(type.BaseType, getStatic, getPrivate, getBases));
			return memberList.Where(memberInfo => memberInfo is PropertyInfo || (memberInfo is FieldInfo && memberInfo.GetCustomAttribute<CompilerGeneratedAttribute>() == null)); // filter out property with backing fields
		}
		*/
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
			PropertyInfo propertyInfo = obj_.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault((PropertyInfo x) => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
			if (propertyInfo != null)
			{
				if (value is bool && ((bool)value || !(bool)value))
				{
					propertyInfo.SetValue(obj_, Convert.ToBoolean(value), null);
					return true;
				}
				if (isInt(value))
				{
					propertyInfo.SetValue(obj_, (int) Convert.ToInt32(value), null);
					return true;
				}
				if (isFloat(value))
				{
					propertyInfo.SetValue(obj_, value, null);
					return true;
				}
				if (isDouble(value))
				{
					propertyInfo.SetValue(obj_, (double)Convert.ToDouble(value), null);
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
				if (isInt(value))
				{
					fieldInfo.SetValue(obj_, Convert.ToInt32(value));
					return true;
				}
				if (isFloat(value))
				{
					fieldInfo.SetValue(obj_, value);
					return true;
				}
				if (isDouble(value))
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
				PropertyInfo left = obj_.GetType().GetProperties(AllBindingFlags).FirstOrDefault((PropertyInfo x) => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
				if (left != null)
				{
					result = true;
				}
				else
				{
					FieldInfo left2 = obj_.GetType().GetFields(AllBindingFlags).FirstOrDefault((FieldInfo x) => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
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
					if (isInt(value))
					{
						propertyInfo.SetValue(obj_, Convert.ToInt32(value), null);
						return true;
					}
					if (isDouble(value))
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
					if (isInt(value))
					{
						fieldInfo.SetValue(obj_, Convert.ToInt32(value));
						return true;
					}
					if (isDouble(value))
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
					string text2 = Repeat("~", indent + 1);
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




		public class CompareObjects
		{
			class IgnorePropertyCompareAttribute : Attribute { }
 

			class PropertyCompareResult
			{
				public string Name { get; private set; }
				public object OldValue { get; private set; }
				public object NewValue { get; private set; }

				public PropertyCompareResult(string name, object oldValue, object newValue)
				{
					Name = name;
					OldValue = oldValue;
					NewValue = newValue;
				}
			}

			private static List<PropertyCompareResult> DoCompare<T>(T oldObject, T newObject)
			{
				PropertyInfo[] properties = typeof(T).GetProperties();
				List<PropertyCompareResult> result = new List<PropertyCompareResult>();

				foreach (PropertyInfo pi in properties)
				{
					if (pi.CustomAttributes.Any(ca => ca.AttributeType == typeof(IgnorePropertyCompareAttribute)))
					{
						continue;
					}

					object oldValue = pi.GetValue(oldObject), newValue = pi.GetValue(newObject);

					if (!object.Equals(oldValue, newValue))
					{
						result.Add(new PropertyCompareResult(pi.Name, oldValue, newValue));
					}
				}

				return result;
			}

			public static List<string> Compare(object a1, object a2)
            {
				List<string> lst = new List<string>();
				foreach (PropertyCompareResult resultItem in DoCompare(a1, a2))
				{
					lst.Add("  Property name: "+ resultItem.Name+"-- old: " + (resultItem.OldValue ?? "<null>")+", new: "+ (resultItem.NewValue ?? "<null>"));
				}
				return lst;
			}
		}
		#endregion








		public static Dictionary<string, string> GoogleTranslateLangs()
		{
			// See https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes
			return new Dictionary<string, string>() { { "auto", "Automatic" }, { "af", "Afrikaans" }, { "sq", "Albanian" }, { "am", "Amharic" }, { "ar", "Arabic" }, { "hy", "Armenian" }, { "az", "Azerbaijani" }, { "eu", "Basque" }, { "be", "Belarusian" }, { "bn", "Bengali" }, { "bs", "Bosnian" }, { "bg", "Bulgarian" }, { "ca", "Catalan" }, { "ceb", "Cebuano" }, { "ny", "Chichewa" }, { "zh-cn", "Chinese Simplified" }, { "zh-tw", "Chinese Traditional" }, { "co", "Corsican" }, { "hr", "Croatian" }, { "cs", "Czech" }, { "da", "Danish" }, { "nl", "Dutch" }, { "en", "English" }, { "eo", "Esperanto" }, { "et", "Estonian" }, { "tl", "Filipino" }, { "fi", "Finnish" }, { "fr", "French" }, { "fy", "Frisian" }, { "gl", "Galician" }, { "ka", "Georgian" }, { "de", "German" }, { "el", "Greek" }, { "gu", "Gujarati" }, { "ht", "Haitian Creole" }, { "ha", "Hausa" }, { "haw", "Hawaiian" }, { "iw", "Hebrew" }, { "hi", "Hindi" }, { "hmn", "Hmong" }, { "hu", "Hungarian" }, { "is", "Icelandic" }, { "ig", "Igbo" }, { "id", "Indonesian" }, { "ga", "Irish" }, { "it", "Italian" }, { "ja", "Japanese" }, { "jw", "Javanese" }, { "kn", "Kannada" }, { "kk", "Kazakh" }, { "km", "Khmer" }, { "ko", "Korean" }, { "ku", "Kurdish (Kurmanji)" }, { "ky", "Kyrgyz" }, { "lo", "Lao" }, { "la", "Latin" }, { "lv", "Latvian" }, { "lt", "Lithuanian" }, { "lb", "Luxembourgish" }, { "mk", "Macedonian" }, { "mg", "Malagasy" }, { "ms", "Malay" }, { "ml", "Malayalam" }, { "mt", "Maltese" }, { "mi", "Maori" }, { "mr", "Marathi" }, { "mn", "Mongolian" }, { "my", "Myanmar (Burmese)" }, { "ne", "Nepali" }, { "no", "Norwegian" }, { "ps", "Pashto" }, { "fa", "Persian" }, { "pl", "Polish" }, { "pt", "Portuguese" }, { "ma", "Punjabi" }, { "ro", "Romanian" }, { "ru", "Russian" }, { "sm", "Samoan" }, { "gd", "Scots Gaelic" }, { "sr", "Serbian" }, { "st", "Sesotho" }, { "sn", "Shona" }, { "sd", "Sindhi" }, { "si", "Sinhala" }, { "sk", "Slovak" }, { "sl", "Slovenian" }, { "so", "Somali" }, { "es", "Spanish" }, { "su", "Sundanese" }, { "sw", "Swahili" }, { "sv", "Swedish" }, { "tg", "Tajik" }, { "ta", "Tamil" }, { "te", "Telugu" }, { "th", "Thai" }, { "tr", "Turkish" }, { "uk", "Ukrainian" }, { "ur", "Urdu" }, { "uz", "Uzbek" }, { "vi", "Vietnamese" }, { "cy", "Welsh" }, { "xh", "Xhosa" }, { "yi", "Yiddish" }, { "yo", "Yoruba" }, { "zu", "Zulu" } };
		}

		public static bool IsSimpleType(object obj)
        {
			var type = obj.GetType();
			return type.IsPrimitive || type.IsValueType || new Type[] { typeof(string), typeof(decimal), typeof(DateTime), typeof(DateTimeOffset), typeof(TimeSpan), typeof(Guid) }.Contains(type) || type.IsEnum || Convert.GetTypeCode(type) != TypeCode.Object || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && IsSimpleType(type.GetGenericArguments()[0]));
			// obj is Boolean || obj is bool || obj is string || obj is double || obj is float || obj is long || obj is decimal || obj is int);
		}
		private static string dump(object obj, BindingFlags bindingFlags, bool execMethods, int maxRecursiveDeep, int currentDeep, string prefix)
		{
			string indentPhrase = prefix + " | ";
			string arrowPointer = prefix + " -> ";

			string NL = Environment.NewLine;
			List<string> list = new List<string>();
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			; //System.Collections.ObjectModel.Collection 

			string result = "";
			result = NL + indentPhrase + (currentDeep == 1 ? ("<----------------- START ----------------->") : "-----------") + " >>>>> TYPE: " + obj.GetType().ToString() + " --->" + NL;

			if (obj == null)
			{
				result += "object is NULL";
			}
			else if (IsSimpleType(obj))
			{
				result += obj.ToString();
			}
			else try
			{
				string tmp1 = tryEnumerabledString(obj, " ");
				if (tmp1 != "")
				{
					result += tmp1;
				}
				else
				{
					foreach (MemberInfo memberInfo in GetMembersInclPrivateBase_static(obj.GetType(), bindingFlags))
					{
						string name_ = "__cant_detect__"; try { name_ = memberInfo.Name; } catch { }
						string value_ = "__cant_detect__";


						string type_ = memberInfo.MemberType.ToString();
						string prefix2_ = nl_ + " ? " + arrowPointer + name_;
						try
						{

							if (memberInfo.MemberType == MemberTypes.Field || memberInfo.MemberType == MemberTypes.Property)
							{

								object value = null;
								if (memberInfo.MemberType == MemberTypes.Field)
								{
									FieldInfo fieldInfo = (FieldInfo)memberInfo;
									value = fieldInfo.GetValue(obj);
								}
								else if (memberInfo.MemberType == MemberTypes.Property)
								{
									PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
									value = propertyInfo.GetValue(obj, null);
								}
								// 
								if (value == null)
								{
									value_ = "null";
								}
								else
								{
									value_ = "   [" + value.GetType().FullName + "] " + value.ToString() + " < " + AccessModifierType(memberInfo) + " > ";
									if (!singleTypes.Contains(value.GetType().ToString()))
									{
										string text9 = tryEnumerabledString(value, prefix2_);
										if (text9 != "")
										{
											value_ += text9;
										}
										else if (currentDeep < maxRecursiveDeep)
										{
											value_ += dump(value, bindingFlags, execMethods, maxRecursiveDeep, currentDeep + 1, prefix2_);
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
									value_ = string.Concat(new string[] { "   [", text11.Replace("System.", ""), "]  (", tryEnumerabledString(methodInfo.GetParameters(), ", "), ")  <", AccessModifierType(methodInfo), ">" });
									var noDeleteWords = !name_.Contains("elete") && !name_.Contains("emove") && !name_.Contains("erase") && !name_.Contains("Erase");
									if (execMethods && noDeleteWords)
									{
										string[] source = new string[]
										{ "System.Double", "System.Int32", "System.String", "System.Float", "System.Type" };
										if (source.Contains(text11))
										{
											try
											{
												object obj2 = methodInfo.Invoke(obj, null);
												if (obj2 != null)
												{
													object obj3 = value_;
													value_ = string.Concat(new object[] { obj3, "========", obj2.ToString(), "   [", obj2.GetType(), "]" });
												}
											}
											catch (Exception)
											{
												value_ += "--------------cant-Invoke";
											}
										}

									}
								}
								else if (memberInfo.MemberType == MemberTypes.Constructor)
								{
									ConstructorInfo constructorInfo = (ConstructorInfo)memberInfo;
									ConstructorInfo left = constructorInfo;
									if (left == null)
									{
										value_ = "null";
									}
									else
									{
										ParameterInfo[] parameters = constructorInfo.GetParameters();
										value_ = "params:" + parameters.ToString() + "   [type:" + parameters.GetType() + "]";
									}
								}
								else if (memberInfo.MemberType == MemberTypes.Event)
								{
									EventInfo eventInfo = (EventInfo)memberInfo;

									value_ = eventInfo == null ? "null" : "ToString: " + eventInfo.ToString();

								}
								else
								{
									value_ = "ToStringed: " + memberInfo.ToString();
								}
							}
						}
						catch
						{
							value_ += "--------------error-getting-value";
						}
						string str2 = pChars(name_) + value_;
						string line = indentPhrase + type_ + ":    " + str2;
						if (!list.Contains(line))
						{
							list.Add(line);
							line += NL;
						}
						else
						{
							line = "";
						}
						if (!dictionary.ContainsKey(type_))
						{
							dictionary[type_] = new List<string>();
						}
						dictionary[type_].Add(line);
					}
				}
			}
			catch (Exception e)
			{
				result += ExceptionMessage(e, obj);
				m(result);
			}
			string finalTxt = "";
			foreach (KeyValuePair<string, List<string>> keyValuePair in dictionary)
			{
				foreach (string str3 in (from q in keyValuePair.Value orderby q select q).ToList<string>())
				{
					finalTxt += str3;
				}
			}
			result += finalTxt;
			result = result + indentPhrase + ((currentDeep == 1) ? (NL+"<----------------- END ---------------------->" + NL + NL) : "-----------") + NL;
			return result;
		}


		private static string ProgramName_ = "";
		public static string ProgramName
		{
			get
			{
				if (ProgramName_ == "")
				{
					ProgramName_ = assemblyTitle() + "--" + assemblyGuid();
					//m("Please set .ProgramName property ( ), otherwise Library methods can't function normally."); } 
				}
				return ProgramName_;
			}
			set { ProgramName_ = value; }
		}
		public static string assemblyName()
		{
			return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
		}
		public static string assemblyTitle()
		{
			return System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;
		}
		public static string assemblyCompany()
		{
			return System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company;
		}
		public static string assemblyGuid()
		{
			// (GuidAttribute)typeof(Program).Assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0].value;
			//return System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes( System.Runtime.InteropServices.GuidAttribute), true)[0].ToString();
			var currAssembly = System.Reflection.Assembly.GetExecutingAssembly();
			return System.Runtime.InteropServices.Marshal.GetTypeLibGuidForAssembly(currAssembly).ToString();
		}



		private static string RegBaseKey_ = "";
		public static string RegBaseKey
		{
			get
			{
				if (RegBaseKey_ == "")
				{
					RegBaseKey_ = "SOFTWARE\\ExampleCompany\\" + ProgramName + "\\";
					//m("Please set .ProgramName property ( ), otherwise Library methods can't function normally."); } 
				}
				return RegBaseKey_;
			}
			set { RegBaseKey_ = value; }
		}

		public static string[] removeEmptyStrings(string[] strings)
		{
			return strings.Where(x => !string.IsNullOrEmpty(x)).ToArray();
		}
		public static string[] removeEmptyStringsAndTrim(string[] strings)
		{
			return trim(strings.Where(x => !string.IsNullOrEmpty(x)).ToArray());
		}
		public static string[] trim(string[] strings)
		{
			return strings.Select(x => x.Trim()).ToArray();
		}


		public static string urlPost_Form(string url, Dictionary<string, string> paramss)
		{

			var client = new System.Net.Http.HttpClient();
			client.BaseAddress = new Uri(url);
			var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, ""); //i.e./v1/oauth2/token
																									   //var paramss = new List<KeyValuePair<string, string>>();
																									   //paramss.Add(new KeyValuePair<string, string>("site", "http://www.google.com"));
			request.Content = new System.Net.Http.FormUrlEncodedContent(paramss);
			var response = client.SendAsync(request).Result.Content.ReadAsStringAsync().Result;
			return response;
		}

		public static void clearRegistryCached()
		{
			myregs.Clear();
		}

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

		#region Encrypt/Decrypt  (another : https://pastebin.com/85yKZUnk )
		public static class EncryptDecrypt
		{
			// encryption 
			public static string EncryptString(string messageToEncrypt, string key_)
			{
				// Create sha256 hash
				SHA256 mySHA256 = SHA256Managed.Create();
				byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(key_));
				// Create secret IV
				byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
				// Instantiate a new Aes object to perform string symmetric encryption
				Aes encryptor = Aes.Create();
				encryptor.Mode = CipherMode.CBC;
				// Set key and IV
				byte[] aesKey = new byte[32];
				Array.Copy(key, 0, aesKey, 0, 32);
				encryptor.Key = aesKey;
				encryptor.IV = iv;
				// Instantiate a new MemoryStream object to contain the encrypted bytes
				MemoryStream memoryStream = new MemoryStream();
				// Instantiate a new encryptor from our Aes object
				ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();
				// Instantiate a new CryptoStream object to process the data and write it to the  memory stream
				CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);
				// Convert the plainText string into a byte array
				byte[] plainBytes = Encoding.ASCII.GetBytes(messageToEncrypt);
				// Encrypt the input plaintext string
				cryptoStream.Write(plainBytes, 0, plainBytes.Length);
				// Complete the encryption process
				cryptoStream.FlushFinalBlock();
				// Convert the encrypted data from a MemoryStream to a byte array
				byte[] cipherBytes = memoryStream.ToArray();
				// Close both the MemoryStream and the CryptoStream
				memoryStream.Close();
				cryptoStream.Close();
				// Convert the encrypted byte array to a base64 encoded string
				string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);
				// Return the encrypted data as a string
				return cipherText;
			}


			public static string DecryptString(string cipherText, string key_)
			{
				// Create sha256 hash
				SHA256 mySHA256 = SHA256Managed.Create();
				byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(key_));
				// Create secret IV
				byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
				// Instantiate a new Aes object to perform string symmetric encryption
				Aes encryptor = Aes.Create();
				encryptor.Mode = CipherMode.CBC;
				// Set key and IV
				byte[] aesKey = new byte[32];
				Array.Copy(key, 0, aesKey, 0, 32);
				encryptor.Key = aesKey;
				encryptor.IV = iv;
				// Instantiate a new MemoryStream object to contain the encrypted bytes
				MemoryStream memoryStream = new MemoryStream();
				// Instantiate a new encryptor from our Aes object
				ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();
				// Instantiate a new CryptoStream object to process the data and write it to the 
				// memory stream
				CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);
				// Will contain decrypted plaintext
				string plainText = String.Empty;
				try
				{
					// Convert the ciphertext string into a byte array
					byte[] cipherBytes = Convert.FromBase64String(cipherText);
					// Decrypt the input ciphertext string
					cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
					// Complete the decryption process
					cryptoStream.FlushFinalBlock();
					// Convert the decrypted data from a MemoryStream to a byte array
					byte[] plainBytes = memoryStream.ToArray();
					// Convert the decrypted byte array to string
					plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
				}
				finally
				{
					// Close both the MemoryStream and the CryptoStream
					memoryStream.Close();
					cryptoStream.Close();
				}
				// Return the decrypted data as a string
				return plainText;
			}

			public static void helper__encrypt_decrypt_stream(out MemoryStream memoryStream, out CryptoStream cryptoStream, string secterKey)
			{
				SHA256 mySHA256 = SHA256Managed.Create();
				byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(secterKey));
				byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
				// string symmetric encryption
				Aes encryptor = Aes.Create();
				encryptor.Mode = CipherMode.CBC;
				//encryptor.KeySize = 256;    encryptor.BlockSize = 128;   encryptor.Padding = PaddingMode.Zeros;
				encryptor.Key = key;
				encryptor.IV = iv;

				memoryStream = new MemoryStream();
				cryptoStream = new CryptoStream(memoryStream, encryptor.CreateEncryptor(), CryptoStreamMode.Write); // write to memory stream
			}
		}
		#endregion

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
				m(text.ToString());
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
		public static bool isFloat(object value)
		{
			return value is float;
		}
		public static bool IsNumber(object value)
		{
			return value is sbyte
					|| value is byte
					|| value is short
					|| value is ushort
					|| value is int
					|| value is uint
					|| value is long
					|| value is ulong
					|| value is float
					|| value is double
					|| value is decimal;
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
				if (path == "") path = "tada";
				if (!path.Contains("."))
				{
					path = "C:\\Windows\\media\\" + path + ".wav";
				}
				//if (File.Exists(path))
				{
					SoundPlayer soundPlayer = new SoundPlayer(path);
					soundPlayer.Play();
					result = true;
				}
			}
			catch (Exception ex)
			{
				result = false;
				m(ex.ToString());
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

		public static void PopupTextboxOnControlClick(Control control_)
		{
			control_.MouseClick += (sender, mouseEventArgs) => {
				PopupTextbox(control_);
			};
		}
		public static void PopupTextboxOnControlClick(Control[] controls)
		{
			foreach (var control_ in controls)
				control_.MouseClick += (sender, mouseEventArgs) => {
					PopupTextbox(control_);
				};
		}

		public static void PopupTextbox(Control sender_)
		{
			System.Windows.Forms.Form form1 = new Form();
			form1.Width = 500;
			form1.Height = 600;
			//logForm.Resize += resizeTextarea; 
			form1.Text = @"Textarea";
			form1.TopMost = true;
			form1.TopLevel = true;
			form1.Opacity = 0.95;
			form1.StartPosition = FormStartPosition.CenterScreen;
			//
			var marg_bottom = 30;
			var hPadd = 10;
			System.Windows.Forms.RichTextBox textarea = new System.Windows.Forms.RichTextBox();
			form1.Controls.Add(textarea);
			textarea.Multiline = true;
			textarea.Text = sender_.Text;
			controlSetSize(textarea, textarea.Parent.Width - hPadd * 3, textarea.Parent.Height - marg_bottom * 3);
			controlSetLocation(textarea, hPadd, hPadd);

			//
			System.Windows.Forms.Button saveButton = new System.Windows.Forms.Button();
			form1.Controls.Add(saveButton);
			saveButton.Text = "OK";
			controlSetLocation(saveButton, textarea.Parent.Width / 2 - saveButton.Width / 2, form1.Height - saveButton.Height * 3);
			// object sender, EventArgs e   // new EventHandler(delegate (Object o, EventArgs a) { //snip });
			// EventHandler handler = (s, e) => MessageBox.Show("Woho"); 
			saveButton.Click += (sender, args) => { sender_.Text = textarea.Text; form1.Close(); };
			//
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



		//Microsoft.VisualBasic.Interaction.InputBox("Question?","Title","Default Text");

		public static string tempLocation_set = "";
		public static string showFormLocation()
		{
			if (tempLocation_set != "") return tempLocation_set;
			Form form = new Form(); form.Width = 400; form.Height = 50;
			tempLocation_set = System.IO.Path.GetTempPath() + "/temp_log_" + DateTime.Now.ToShortDateString();
			var textbox = new TextBox(); textbox.Top = 0; textbox.Left = 0; form.Controls.Add(textbox); textbox.Text = tempLocation_set;
			// Show testDialog as a modal dialog and determine if DialogResult = OK.
			if (form.ShowDialog() == DialogResult.OK)
			{
			}
			tempLocation_set = textbox.Text;
			form.Dispose();
			return tempLocation_set;
		}


		/*
		Thread mThread = new Thread(delegate ()
		{
			StratControlBox = new StratControl(StratIDs);
			StratControlBox.ShowDialog();
		});

		mThread.SetApartmentState(ApartmentState.STA);

        mThread.Start();
		*/
		public static Task StartSTATask(Action func)
		{
			var tcs = new TaskCompletionSource<object>();
			var thread = new Thread(() =>
			{
				try
				{
					func();
					tcs.SetResult(null);
				}
				catch (Exception e)
				{
					tcs.SetException(e);
				}
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			return tcs.Task;
		}



		private static bool CatchAllExceptions_EnabledAlready = false;
		private static bool CatchAllExceptions_FileOrWindow = true;

		//[STAThread]
		public static void CatchAllExceptions()
		{
			CatchAllExceptions(true, false);
		}
		public static void CatchAllExceptions(bool firstChaneException)
		{
			CatchAllExceptions(firstChaneException, false);
		}
		public static void CatchAllExceptions(bool firstChaneException, bool FileOrWindow)
		{
			if (CatchAllExceptions_EnabledAlready) return;
			CatchAllExceptions_EnabledAlready = true;

			if (firstChaneException) 
				AppDomain.CurrentDomain.FirstChanceException += HandleUnhandledException;
			else
				AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException; 
			CatchAllExceptions_FileOrWindow = FileOrWindow;
		}

		public static bool inCatchAllLoop = false;
		public static void HandleUnhandledException(object sender, object e)
		{
			if (!inCatchAllLoop)
			{
				// new System.Diagnostics.StackTrace() GetFrame(1).GetMethod();
				inCatchAllLoop = true;
				var finalStr = "";
				if (e is UnhandledExceptionEventArgs)
					finalStr = ((UnhandledExceptionEventArgs)e).ExceptionObject.ToString();
				else if (e is System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs)
					finalStr = ExceptionMessage( ((System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs)e).Exception ) ;
				else
					finalStr = e.ToString();

				AddTextToPopup("catchExceptionsForm", finalStr);
				inCatchAllLoop = false;
			}
		}

		private static Dictionary<string, Form> allPopupForms = new Dictionary<string, Form>();
		private static Dictionary<string, string> allPopupTexts = new Dictionary<string, string>();
		private static readonly object allFormLock = new object();
		private static bool popupform_exists(string uniqName) { return allPopupForms.ContainsKey(uniqName) && allPopupForms[uniqName] != null && !allPopupForms[uniqName].IsDisposed; }
		public static void AddTextToPopup(string containerKey, string message)
		{
			try
			{
				//Task.Run(() => {
				lock(allFormLock)
				{
					var uniqString = containerKey;
					var textareaName = uniqString + "_textarea";

					var newTxt = Environment.NewLine + Environment.NewLine + "•" + DatetimeText() + " ::: "+ message;

					if (!allPopupTexts.ContainsKey(uniqString)) allPopupTexts[uniqString] = "";
					allPopupTexts[uniqString] =   allPopupTexts[uniqString] + newTxt;

					//

					if ( !popupform_exists (uniqString) )
					{
						var padding = 30;
						allPopupForms[uniqString] = new Form() { Width = 600, Height = 600 };
						allPopupForms[uniqString].TopMost = true;

						var TX = new RichTextBox();
						TX.Name = textareaName;

						TX.Width = allPopupForms[uniqString].Width - padding;
						TX.Height = allPopupForms[uniqString].Height - padding-20;
						TX.Top = 5;
						TX.Left = 5;
						allPopupForms[uniqString].Controls.Add(TX);
						/*
							//all other approaches (task.run & StaThread, with show/showdialog...), fail beacuse of thread lock
							var act = new Action(() =>
							{
								if (popupform_exists(uniqString))
								{
									var ctrl = ControlGetByName(allPopupForms[uniqString], textareaName);
									Control_Set(ctrl, "text", allPopupTexts[uniqString] );

								}
							});
							Timer(act, 2 * 1000, true);
						*/
					}


					if (CatchAllExceptions_FileOrWindow)
                    {
						WriteTempFile("puvox_c#_exceptions", allPopupTexts[uniqString], false);
					}
                    else
                    {
						var ctrl = ControlGetByName(allPopupForms[uniqString], textareaName);
						Control_Set(ctrl, "text", allPopupTexts[uniqString], false);
						if (!allPopupForms[uniqString].Visible) { allPopupForms[uniqString].Show(); }
					}
				}
				//});
			}
			catch (Exception ex2) { }
		}


		public static void controlSetLocation(Control ctrl, int top, int left)
		{
			ctrl.Top = top;
			ctrl.Left = left;
		}
		public static void controlSetSize(Control ctrl, int width, int height)
		{
			ctrl.Width = width;
			ctrl.Height = height;
		}

		/*
		public static DialogResult ShowInputDialog(ref string input)
		{
		   System.Drawing.Size size = new System.Drawing.Size(200, 70);
		   Form inputBox = new Form();

		   inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		   inputBox.ClientSize = size;
		   inputBox.Text = "Name";

		   System.Windows.Forms.TextBox textBox = new TextBox();
		   textBox.Size = new System.Drawing.Size(size.Width - 10, 23);
		   textBox.Location = new System.Drawing.Point(5, 5);
		   textBox.Text = input;
		   inputBox.Controls.Add(textBox);

		   Button okButton = new Button();
		   okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
		   okButton.Name = "okButton";
		   okButton.Size = new System.Drawing.Size(75, 23);
		   okButton.Text = "&OK";
		   okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 39);
		   inputBox.Controls.Add(okButton);

		   Button cancelButton = new Button();
		   cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		   cancelButton.Name = "cancelButton";
		   cancelButton.Size = new System.Drawing.Size(75, 23);
		   cancelButton.Text = "&Cancel";
		   cancelButton.Location = new System.Drawing.Point(size.Width - 80, 39);
		   inputBox.Controls.Add(cancelButton);

		   inputBox.AcceptButton = okButton;
		   inputBox.CancelButton = cancelButton;

		   DialogResult result = inputBox.ShowDialog();
		   input = textBox.Text;
		   return result;
	   }
		*/
	   //string input="hede";
	   //ShowInputDialog(ref input);

	   public static class Prompt
	   {
		   public static string ShowDialog(string text, string caption)
		   {
			   Form prompt = new Form()
			   {
				   Width = 500,
				   Height = 150,
				   FormBorderStyle = FormBorderStyle.FixedDialog,
				   Text = caption,
				   StartPosition = FormStartPosition.CenterScreen
			   };
			   Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
			   TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
			   Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
			   confirmation.Click += (sender, e) => { prompt.Close(); };
			   prompt.Controls.Add(textBox);
			   prompt.Controls.Add(confirmation);
			   prompt.Controls.Add(textLabel);
			   prompt.AcceptButton = confirmation;

			   return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
		   }
	   }



	   /*
			   AppDomainSetup mysetup = AppDomain.CurrentDomain.SetupInformation;
			   mysetup.ApplicationBase = PEPatch.GetDirectoryFromPath(MI.inputfile);
			   mysetup.ConfigurationFile = MI.inputfile + ".config";

			   string cpath = Environment.GetEnvironmentVariable("Path");
			   if (!cpath.Contains(mysetup.ApplicationBase))
			   {
				   cpath = cpath + ";" + mysetup.ApplicationBase;
				   Environment.SetEnvironmentVariable("Path", cpath);
			   }
	   */

		public static string ReadFileSafe(string path)
		{
			if (File.Exists(path))
			{
				lock (path)
				{
					return File.ReadAllText(path);
				}
			}
			return "";
		}
		public static void WriteFileSafe(string path, string txt)
		{
			if (File.Exists(path))
			{
				lock (path)
				{
					File.WriteAllText(path, txt);
				}
			}
		}
		public static void AppendFileSafe(string path, string txt)
		{
			if (File.Exists(path))
			{
				lock (path)
				{
					File.AppendAllText(path, txt);
				}
			}
		}

		public static string DatetimeText()
        {
			return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

		}
		public static void logError(object obj_)
		{
			FileOperations.Write(errorLogFile, DatetimeText() + Environment.NewLine + obj_.ToString() + Environment.NewLine);
		}

		private static string isDeveloperMode_contents = "-1";
		public static bool isDeveloperMode(string key) { try { string file = "c:\\debug_c#_developer_mode.txt"; if (isDeveloperMode_contents == "-1") { if (File.Exists(file)) isDeveloperMode_contents = ReadFileSafe(file); } bool enabled = isDeveloperMode_contents.Contains(key + "+"); if (!enabled && !isDeveloperMode_contents.Contains(key + "-")) { isDeveloperMode_contents += Environment.NewLine + key + "-"; WriteFileSafe(file, isDeveloperMode_contents); } return enabled; } catch { return true; } return true; }



		public static string ExceptionForDeveloper(Exception e, object targetObj)
		{
			string text = "";
			string result;
			try
			{
				text += Methods.ExceptionMessage(e, targetObj);
				if (Methods.isDeveloperMode("Exceptions"))
				{
					m(text);
				}
				Methods.FileWriteSafe(Methods.tmpDir + "_lastError_" + ((targetObj == null || targetObj.GetType() == null) ? "unknown" : targetObj.GetType().Name), text);
				result = text;
			}
			catch (Exception ex)
			{
				result = "problem in ExceptionForDeveloper method. " + text + ex.Message + nl_ + e.Message;
			}
			return result;
		}

		//  public static void ExceptionMessage(Exception e, object obj_, System.Reflection.MethodBase method, string msg, [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)

		public static void ExcPopup(Exception e, object obj_)
		{
			m(ExceptionMessage(e, obj_));
		}
		public static void OpenLinkInBrowser(string url)
		{
			System.Diagnostics.Process.Start(url);
		}
		public static void OpenUrl(string url)
		{
			System.Diagnostics.Process.Start(url);
		}
		public static string ExceptionMessage(Exception e)
		{
			return ExceptionMessage(e, null);
		}

		public static string ExceptionMessage(Exception e, object obj_)
		{
			string result = "";
			string divider = nl_;//  + "_________________" + nl_;
			//
			var stack = new StackTrace(e, true);
			StackFrame errFrame = stack.GetFrame(0);
			int line = errFrame.GetFileLineNumber();
			int column = errFrame.GetFileColumnNumber();
			string fileName = errFrame.GetFileName();
			//
			result += "======================" + nl_;

				if (e.Source != null)
			result += "• Source				: " + e.Source.ToString() + divider; //dll name. full path: e.TargetSite.ReflectedType.Assembly.Location
				if (obj_ != null && obj_.GetType() != null)
			result += "• Target object		: " + obj_.GetType().FullName + divider;
			result += "• Method				: ";
			foreach (var frame in stack.GetFrames().Reverse())
			{
				var method_ = frame.GetMethod();
				result += " > " + (method_ != null ? method_.Name : "method_name_not_detected");
			} 
			result += " [Line:" + (line != 0 ? line.ToString() : "") +  (column != 0 ? column.ToString() : "")+"]";
			if (line != 0)
			{
#if !NET20 && !NET35
				try { if (fileName.EndsWith(".cs")) { string str1 = System.IO.File.ReadLines(fileName).Skip(line - 1).Take(1).First(); if (string.IsNullOrEmpty(str1)) return str1; str1 = str1.Substring(column - 1, Math.Min(str1.Length, 30)); result += "--->   " + str1; } } catch { }
#endif
			}
			result += divider;

			result += "• Message			: " + e.Message + divider;
			if (e.InnerException != null)
				result += "• InnerException		: " + e.InnerException.Message + divider;
			result += "• StackTrace			: " + nl_ + " ----- " + e.StackTrace + divider;
			// Target Method info
			//      if (e.TargetSite != null)
			//result += "• TargetSite:     " + e.TargetSite.ToString() + divider;

			result += "======================" + nl_;
			return result;
		}

		public static string stackFramesString(StackTrace stktr)
		{
			IEnumerable<StackFrame> enumerable = stktr.GetFrames().Reverse<StackFrame>();
			string text = "";
			foreach (StackFrame stackFrame in enumerable)
			{
				MethodBase method = stackFrame.GetMethod();
				text = text + " > " + ((method != null) ? method.Name : "method_name_not_detected");
			}
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


		/*
		 * 
		 * 
        internal byte[] Combine_bytestring(params byte[][] arrays)
        {
            try
            {
                byte[] rv = new byte[arrays.Sum(a => a.Length)];
                int offset = 0;
                foreach (byte[] array in arrays)
                {
                    System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                    offset += array.Length;
                }
                return rv;
            }
            catch (Exception e)
            {
                ex(e);
                return new byte[0];
            }
        }

        internal byte[] addByteToArray(byte[] bArray, byte newByte)
        {
            try
            {
                byte[] newArray = new byte[bArray.Length + 1];
                bArray.CopyTo(newArray, 0);
                newArray[bArray.Length - 1] = newByte;
                return newArray;
            }
            catch (Exception e)
            {
                ex(e);
                return new byte[0];
            }
        }

        internal byte[] combineByteArrays(byte[] ba1, byte[] ba2)
        {
            try
            {
                byte[] rv = new byte[ba1.Length + ba2.Length];
                System.Buffer.BlockCopy(ba1, 0, rv, 0, ba1.Length);
                System.Buffer.BlockCopy(ba2, 0, rv, ba1.Length, ba2.Length);
                return rv;
            }
            catch (Exception e)
            {
                ex(e);
                return new byte[0];
            }
        }*/



		public static string tmpFilePath = Environment.GetEnvironmentVariable("tmp") + "\\";
		public static string pathDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);


		public static string nl = Environment.NewLine;

		public static string[] singleTypes = new string[] { "System.Double", "System.Int32", "System.String", "System.Float", "System.Boolean" };


		internal static BindingFlags AllBindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty | BindingFlags.ExactBinding | BindingFlags.SuppressChangeType | BindingFlags.OptionalParamBinding | BindingFlags.IgnoreReturn;

		internal static BindingFlags AllBindingFlags_noinherit = BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty | BindingFlags.ExactBinding | BindingFlags.SuppressChangeType | BindingFlags.OptionalParamBinding | BindingFlags.IgnoreReturn;

		//private static object fileCheckObj = new object(); 
		//public static string errorLogFile = Environment.GetEnvironmentVariable("tmp") + "\\_errorlogs_c#.log"; 
		public static string nl_ = Environment.NewLine;
		public static string tmpDir = Environment.GetEnvironmentVariable("tmp") + "\\";




		public void dialogMessage()
		{
			var dialogResult = System.Windows.Forms.MessageBox.Show("Message", "Title", MessageBoxButtons.OKCancel);
			if (dialogResult == System.Windows.Forms.DialogResult.OK)
				System.Windows.Forms.MessageBox.Show("OK Clicked");
			else
				System.Windows.Forms.MessageBox.Show("Cancel Clicked");
		}


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


				// public bool setRegistryValue1(string parentKey, string key)
				// {
				//     registryEntry = registryBase.CreateSubKey(keyPath,
				//     RegistryKeyPermissionCheck.ReadWriteSubTree);
				//     registryEntry.SetValue(valueName, newValue);
				// }



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



		http://ipinfo.io/ip
		
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




namespace HelperLibraries
{

	#region JSON
	// https://stackoverflow.com/questions/14967618/deserialize-json-to-class-manually-with-reflection/50492864#50492864
	//new update here, but doesnt work well : http://qaru.site/questions/6250657/deserialize-json-to-class-manually-with-reflection

	// other1: https://pastebin.com/raw/JGWHCRJv
	// other2: https://github.com/ttodua/C-sharp-useful-scripts/blob/master/json-serializer-deserializer.cs

	public static class JsonMaker
	{
		public static Dictionary<string, object> DeserializeJson(string json)
		{
			int end;
			return DeserializeJson(json, 0, out end);

		}

		private static Dictionary<string, object> DeserializeJson(string json, int start, out int end)
		{
			Dictionary<string, object> dict = new Dictionary<string, object>();
			end = 0;
			try
			{

				if (!IsJson(json))
				{
					end = 0;
					return null;
				}

				bool escbegin = false;
				bool escend = false;
				bool inquotes = false;
				string key = null;

				StringBuilder sb = new StringBuilder();
				List<object> arraylist = null;
				Regex regex = new Regex(@"\\u([0-9a-z]{4})", RegexOptions.IgnoreCase);
				int autoKey = 0;
				bool inSingleQuotes = false;
				bool inDoubleQuotes = false;

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
										key = "array" + autoKey.ToString();  //.ToString()
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
			}
			catch (Exception e)
			{
				System.Windows.Forms.MessageBox.Show("JsonMarkerParser : " + e.ToString());
			}

			return dict; // theoretically shouldn't ever get here
		}


		private static string DecodeString(Regex regex, string str)
		{
			return Regex.Unescape(regex.Replace(str, match => char.ConvertFromUtf32(Int32.Parse(match.Groups[1].Value, System.Globalization.NumberStyles.HexNumber))));
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
	#endregion

}


































namespace PuvoxLibrary
{
	public class Program
	{
		public string baseDomain;// = "https://puvox.software/";
		public string apiBaseDomain = "https://license.puvox.software/";
		public string baseResponsePath = "responses.php?";
		public string baseCompanyName = "Puvox";
		public string baseContactPath = "contact";
		public string Name;
		public string Slug;
		public double Version;
		public string Language;
		public string BaseProductUrl;
		public string RegRootOfThisProg;
		public string responseUrl;
		public string contactUrl;
		public bool initialized;

		public static void m(object obj) { PuvoxLibrary.Methods.m(obj.ToString()); }
		public static void cl(object obj) { Console.WriteLine(obj == null ? "null" : obj.ToString()); System.Diagnostics.Debug.WriteLine(obj == null ? "null" : obj.ToString()); }
		private bool isDevelopment { get { return PuvoxLibrary.Methods.IsDeveloperMachine(); } }

		public Program()
		{
		}

		public Program(string companyName_, string domain_, string title, string slug, double version, string language, string productUrl)
		{
			baseCompanyName = companyName_;
			baseDomain = domain_;
			Name = title;
			Slug = slug;
			Version = version;
			Language = language;
			BaseProductUrl = productUrl;
			RegRootOfThisProg = "SOFTWARE\\" + baseCompanyName + "\\" + Slug + "\\";

			contactUrl = baseDomain + baseContactPath;
			initialized = true;
		}


		public string ResponseString = "";
		public Dictionary<string, object> ResponseHeaders = new Dictionary<string, object>();
		Dictionary<string, object> ResponseData = new Dictionary<string, object>();
		public Dictionary<string, object> IpInfo;

		public Dictionary<string, object> initialize_response()
		{
			getResponseData(true, 0);
			return ResponseData;
		}

		//props
		public string ApiURL() { return apiBaseDomain + baseResponsePath + "program=" + PuvoxLibrary.Methods.urlEncode(Slug) + "&version=" + Version; }
		public bool licensekeySet(string key) { setRegistryValue("licensekey", key); return true; }
		public string licensekeyGet() { return getRegistryValue("licensekey", "demo_" + PuvoxLibrary.Methods.RandomString(32)); }
		internal string status() { return ResponseHeaders["status"] as string; }
		internal bool licenseAllowed() { return ResponseHeaders["status"] as string == "success"; }  //|| ResponseData["confirm_answer"].ToString().Contains("u2")
		public string licenseErrorMessage() { return (ResponseHeaders["status"] as string) + ":" + (ResponseHeaders["data"] as string); }
		public bool isDemo() { return licensekeyGet().Contains("demo_"); }
		public string version() { return ResponseData["version"] as string; }
		internal string confirmAnswer() { return ResponseData["confirm_answer"] as string; }
		internal string country() { return IpInfo["country"] as string; }


		public bool getResponseData(bool forceNew, int cachedMinutes_)
		{
			bool fire = false;
			if (ResponseData == null || ResponseData.Count == 0)
			{
				fire = true;
			}
			else if (forceNew)
			{
				fire = true;
			}
			//
			if (fire)
			{
				string currentIPinfo = getRegistryValue("ipinfo", "");
				string responseUrl = ApiURL() + "&action=check&license=" + licensekeyGet() + "&ipinfo=" + (currentIPinfo == "" ? "1" : "0");
				var text = PuvoxLibrary.Methods.urlRead(responseUrl);// Methods.cachedCall("PuvoxLibrary.Methods.urlRead", new object[] { responseUrl }, 20);
				if (text == "-1") return false;
				ResponseString = text;
				ResponseHeaders = PuvoxLibrary.Methods.deserialize(text);
				if (ResponseHeaders == null)
				{
					m("Can not get response. 581");
				}
				else
				{
					if (!ResponseHeaders.ContainsKey("status"))
					{
						m("Response from server doesnt have status. 582");
					}
					else
					{
						if (ResponseHeaders["status"] as string != "success")
						{
							m("Response from server doesnt have successfull status. 583");
						}
						else
						{
							var data = ResponseHeaders["data"] as string;
							var encrypt_hint = ResponseHeaders["enchint"] as string;
							string dataString = encrypt_hint == "" ? data : PuvoxLibrary.Methods.EncryptDecrypt.DecryptString(data, Slug + encrypt_hint + Version.ToString());
							ResponseData = PuvoxLibrary.Methods.deserialize(dataString);
							string ipinfoString = (currentIPinfo != "") ? currentIPinfo : ResponseData["ipinfo"].ToString();
							IpInfo = PuvoxLibrary.Methods.deserialize(ipinfoString);
							setRegistryValue("ipinfo", ipinfoString);
						}
					}
				}
			}
			return true;
		}

		public bool updateAvailable()
		{
			bool result;
			try
			{
				result = (ResponseData["version"].ToString() != Version.ToString());
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}


		public Dictionary<string, string> RegistryValuesInFolder()
		{
			return PuvoxLibrary.Methods.RegistryValuesInFolder(RegRootOfThisProg);
		}
		public string getRegistryValue(string key, string two)
		{
			return PuvoxLibrary.Methods.getRegistryValue(RegRootOfThisProg + key, two);
		}
		public bool setRegistryValue(string key, string value)
		{
			PuvoxLibrary.Methods.setRegistryValue(RegRootOfThisProg + key, value);
			return true;
		}

		public bool isCacheSet(string key, int minutes)
		{
			return PuvoxLibrary.Methods.isCacheSet(@"Software\Puvox\" + Slug + @"\cachedTimegones\" + key, minutes);
		}

		public bool fillFormOptions(System.Windows.Forms.Control.ControlCollection coll)
		{
			return PuvoxLibrary.Methods.FormOptions_Fill(coll, RegRootOfThisProg);
		}
		public bool saveFormOptions(System.Windows.Forms.Control.ControlCollection cts)
		{
			return PuvoxLibrary.Methods.FormOptions_Save(cts, RegRootOfThisProg);
		}

		public string getFormOption(string key, string two)
		{
			return PuvoxLibrary.Methods.getFormOption(key, two, RegRootOfThisProg);
		}


	}
}

#endregion