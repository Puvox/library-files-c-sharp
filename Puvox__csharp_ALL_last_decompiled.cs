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
using System.Media;
using System.Net;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace PuvoxLibrary
{
	// Token: 0x0200000B RID: 11
	public class Methods
	{
		// Token: 0x06000070 RID: 112 RVA: 0x000051D8 File Offset: 0x000033D8
		public static bool isBetween(DateTime target, DateTime start, DateTime end, bool equality)
		{
			int num_middle = int.Parse(target.ToString("HHmm"));
			int num2 = int.Parse(start.ToString("HHmm"));
			int num3 = int.Parse(end.ToString("HHmm"));
			return equality ? (num_middle >= num2 && num_middle <= num3) : (num_middle > num2 && num_middle < num3);
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00005240 File Offset: 0x00003440
		public static bool isBetween(DateTime target, int start, int end)
		{
			int num = int.Parse(target.ToString("HHmm"));
			return num >= start && num < end;
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000072 RID: 114 RVA: 0x00005270 File Offset: 0x00003470
		public static string mainDrive
		{
			get
			{
				return Path.GetPathRoot(Environment.SystemDirectory);
			}
		}

		// Token: 0x06000073 RID: 115 RVA: 0x0000528C File Offset: 0x0000348C
		public static bool workTillDate(string program_slug, string dateTill)
		{
			bool result;
			try
			{
				bool flag2 = dateTill != "" && DateTime.Now > DateTime.ParseExact(dateTill, "yyyy-MM-dd", CultureInfo.InvariantCulture);
				if (flag2)
				{
					bool flag = Methods.initialized;
					return false;
				}
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x000052F8 File Offset: 0x000034F8
		public static bool demoPeriodGone(string message, string dateTill)
		{
			bool flag = Methods.demoPeriodGone_shown;
			bool result;
			if (flag)
			{
				result = Methods.demoPeriodGone_disallow;
			}
			else
			{
				Methods.demoPeriodGone_shown = true;
				Methods.demoPeriodGone_disallow = false;
				try
				{
					bool flag2 = dateTill != "" && DateTime.Now > DateTime.ParseExact(dateTill, "yyyy-MM-dd", CultureInfo.InvariantCulture);
					if (flag2)
					{
						MessageBox.Show(message.ToString());
						Methods.demoPeriodGone_disallow = true;
					}
				}
				catch (Exception)
				{
					Methods.demoPeriodGone_disallow = true;
				}
				result = Methods.demoPeriodGone_disallow;
			}
			return result;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00005390 File Offset: 0x00003590
		public static string timeIntToString(int timenow)
		{
			string t = timenow.ToString();
			return (t.Length <= 1) ? ("00000" + t) : ((t.Length <= 2) ? ("0000" + t) : ((t.Length <= 3) ? ("000" + t) : ((t.Length <= 4) ? ("00" + t) : ((t.Length <= 5) ? ("0" + t) : t))));
		}

		// Token: 0x06000076 RID: 118 RVA: 0x0000541C File Offset: 0x0000361C
		public static TimeSpan timeIntToTimeSpan(int timenow)
		{
			DateTime dt;
			DateTime.TryParseExact(Methods.timeIntToString(timenow), "HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
			return dt.TimeOfDay;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00005450 File Offset: 0x00003650
		public static int CorrectTime(int timenow, int added_or_subtracted)
		{
			TimeSpan timeSpan = new TimeSpan(timenow / 100, timenow % 100, 0) + TimeSpan.FromMinutes((double)added_or_subtracted);
			return timeSpan.Hours * 100 + timeSpan.Minutes;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00005490 File Offset: 0x00003690
		public static int CorrectTime1(int timenow, int added_or_subtracted)
		{
			return int.Parse(DateTime.ParseExact(timenow.ToString("0000"), "HHmm", null).AddMinutes((double)added_or_subtracted).ToString("HHmm"));
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000079 RID: 121 RVA: 0x000054D8 File Offset: 0x000036D8
		// (set) Token: 0x0600007A RID: 122 RVA: 0x0000550F File Offset: 0x0000370F
		public static string ProgramName
		{
			get
			{
				bool flag = Methods.ProgramName_ == "";
				if (flag)
				{
					Methods.m("Please set .ProgramName property ( ), otherwise Library methods can't function normally.");
				}
				return Methods.ProgramName_;
			}
			set
			{
				Methods.ProgramName_ = value;
			}
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00005518 File Offset: 0x00003718
		public static DateTime ToDateTime(string s)
		{
			DateTime result;
			try
			{
				result = Methods.ToDateTime(s, "ddMMyyyy", "");
			}
			catch (Exception e)
			{
				Methods.m(e.Message);
				result = DateTime.MinValue;
			}
			return result;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00005564 File Offset: 0x00003764
		public static DateTime ToDateTime(string s, string format)
		{
			DateTime result;
			try
			{
				result = Methods.ToDateTime(s, format, "");
			}
			catch (Exception e)
			{
				Methods.m(e.Message);
				result = DateTime.MinValue;
			}
			return result;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000055AC File Offset: 0x000037AC
		public static DateTime ToDateTime(string s, string format, string cultureString)
		{
			DateTime result;
			try
			{
				CultureInfo _culture = (cultureString == "") ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(cultureString);
				result = DateTime.ParseExact(s, format, _culture);
			}
			catch (FormatException)
			{
				throw;
			}
			catch (Exception)
			{
				throw;
			}
			return result;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00005608 File Offset: 0x00003808
		public static int DateToTime(DateTime dt)
		{
			return dt.Hour * 100 + dt.Minute;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x0000562C File Offset: 0x0000382C
		public static bool IsTimePeriod(string type_, DateTime time1, DateTime time0, int StartTime, int EndTime, bool useDayOrSession, bool Bars_IsFirstBarOfSession)
		{
			int curTime_0 = Methods.DateToTime(time0);
			int curTime_ = Methods.DateToTime(time1);
			bool SameDay = StartTime < EndTime;
			bool NotSameDay = StartTime > EndTime;
			bool IsNewDay = useDayOrSession ? (time0.DayOfYear != time1.DayOfYear) : Bars_IsFirstBarOfSession;
			bool isStart = (curTime_0 >= StartTime && (curTime_ < StartTime || IsNewDay)) || (IsNewDay && curTime_ <= StartTime);
			bool is_inside = (SameDay && curTime_0 >= StartTime && curTime_0 <= EndTime) || (NotSameDay && (curTime_0 >= StartTime || curTime_0 <= EndTime));
			bool was_inside = (SameDay && curTime_ >= StartTime && curTime_ <= EndTime) || (NotSameDay && (curTime_ >= StartTime || curTime_ <= EndTime));
			bool flag = type_ == "start";
			bool result;
			if (flag)
			{
				result = isStart;
			}
			else
			{
				bool flag2 = type_ == "inside";
				if (flag2)
				{
					result = is_inside;
				}
				else
				{
					bool flag3 = type_ == "end";
					result = (flag3 && (!is_inside && was_inside));
				}
			}
			return result;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00005730 File Offset: 0x00003930
		public static string DateToTimeString(DateTime dt)
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

		// Token: 0x06000081 RID: 129 RVA: 0x000057B0 File Offset: 0x000039B0
		public static int HoursMinutes(DateTime time)
		{
			return time.Hour * 100 + time.Minute;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000057D4 File Offset: 0x000039D4
		public static DateTime ToTimed_ToDate(DateTime cur_time, int timenow)
		{
			return DateTime.ParseExact(cur_time.ToString("yyyy-M-dd") + timenow.ToString(" 0000"), "yyyy-M-dd HHmm", null);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00005810 File Offset: 0x00003A10
		public static int GetWeekOfMonth(DateTime targetTime)
		{
			DateTime first = new DateTime(targetTime.Year, targetTime.Month, 1);
			return Methods.GetWeekOfYear(targetTime) - Methods.GetWeekOfYear(first) + 1;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00005848 File Offset: 0x00003A48
		public static int GetWeekOfYear(DateTime targetTime)
		{
			return new GregorianCalendar().GetWeekOfYear(targetTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00005868 File Offset: 0x00003A68
		public static int GetQuarter(DateTime targetTime)
		{
			int result = 0;
			int month = targetTime.Month;
			bool flag = month <= 3;
			if (flag)
			{
				result = 1;
			}
			else
			{
				bool flag2 = month > 3 && month <= 6;
				if (flag2)
				{
					result = 2;
				}
				else
				{
					bool flag3 = month > 6 && month <= 9;
					if (flag3)
					{
						result = 3;
					}
					else
					{
						bool flag4 = month > 9;
						if (flag4)
						{
							result = 4;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000058D4 File Offset: 0x00003AD4
		public static string opposite(string text)
		{
			bool flag = text == "";
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				bool flag2 = text == "LONG";
				if (flag2)
				{
					result = "SHORT";
				}
				else
				{
					bool flag3 = text == "SHORT";
					if (flag3)
					{
						result = "LONG";
					}
					else
					{
						bool flag4 = text == "Long";
						if (flag4)
						{
							result = "Short";
						}
						else
						{
							bool flag5 = text == "Short";
							if (flag5)
							{
								result = "Long";
							}
							else
							{
								bool flag6 = text == "long";
								if (flag6)
								{
									result = "short";
								}
								else
								{
									bool flag7 = text == "short";
									if (flag7)
									{
										result = "long";
									}
									else
									{
										bool flag8 = text == "L";
										if (flag8)
										{
											result = "S";
										}
										else
										{
											bool flag9 = text == "S";
											if (flag9)
											{
												result = "L";
											}
											else
											{
												bool flag10 = text == "l";
												if (flag10)
												{
													result = "s";
												}
												else
												{
													bool flag11 = text == "s";
													if (flag11)
													{
														result = "l";
													}
													else
													{
														bool flag12 = text == "BUY";
														if (flag12)
														{
															result = "SELL";
														}
														else
														{
															bool flag13 = text == "SELL";
															if (flag13)
															{
																result = "BUY";
															}
															else
															{
																bool flag14 = text == "Buy";
																if (flag14)
																{
																	result = "Sell";
																}
																else
																{
																	bool flag15 = text == "Sell";
																	if (flag15)
																	{
																		result = "Buy";
																	}
																	else
																	{
																		bool flag16 = text == "buy";
																		if (flag16)
																		{
																			result = "sell";
																		}
																		else
																		{
																			bool flag17 = text == "sell";
																			if (flag17)
																			{
																				result = "buy";
																			}
																			else
																			{
																				bool flag18 = text == "B";
																				if (flag18)
																				{
																					result = "S";
																				}
																				else
																				{
																					bool flag19 = text == "S";
																					if (flag19)
																					{
																						result = "B";
																					}
																					else
																					{
																						bool flag20 = text == "b";
																						if (flag20)
																						{
																							result = "s";
																						}
																						else
																						{
																							bool flag21 = text == "s";
																							if (flag21)
																							{
																								result = "b";
																							}
																							else
																							{
																								bool flag22 = text == "ABOVE";
																								if (flag22)
																								{
																									result = "BELOW";
																								}
																								else
																								{
																									bool flag23 = text == "BELOW";
																									if (flag23)
																									{
																										result = "ABOVE";
																									}
																									else
																									{
																										bool flag24 = text == "Above";
																										if (flag24)
																										{
																											result = "Below";
																										}
																										else
																										{
																											bool flag25 = text == "Below";
																											if (flag25)
																											{
																												result = "Above";
																											}
																											else
																											{
																												bool flag26 = text == "above";
																												if (flag26)
																												{
																													result = "below";
																												}
																												else
																												{
																													bool flag27 = text == "below";
																													if (flag27)
																													{
																														result = "above";
																													}
																													else
																													{
																														result = "";
																													}
																												}
																											}
																										}
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00005BCC File Offset: 0x00003DCC
		public static bool is_Between(DateTime target, DateTime start, DateTime end)
		{
			int target_ = int.Parse(target.ToString("HHmm"));
			int start_ = int.Parse(start.ToString("HHmm"));
			int end_ = int.Parse(end.ToString("HHmm"));
			return target_ >= start_ && target_ < end_;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00005C20 File Offset: 0x00003E20
		public static bool is_Between(DateTime target, int start, int end)
		{
			int target_ = int.Parse(target.ToString("HHmm"));
			return target_ >= start && target_ < end;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00005C54 File Offset: 0x00003E54
		public static void createDefaultValues(object obj)
		{
			foreach (object obj2 in TypeDescriptor.GetProperties(obj))
			{
				PropertyDescriptor property = (PropertyDescriptor)obj2;
				DefaultValueAttribute myAttribute = (DefaultValueAttribute)property.Attributes[typeof(DefaultValueAttribute)];
				bool flag = myAttribute != null;
				if (flag)
				{
				}
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00005CD4 File Offset: 0x00003ED4
		public static int digitsInNumber(object obj)
		{
			return (int)Math.Ceiling(Math.Log10((double)obj));
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00005CF8 File Offset: 0x00003EF8
		public static int digitsAfterDot(double tickSize)
		{
			int precision = 0;
			while (tickSize * Math.Pow(10.0, (double)precision) != Math.Round(tickSize * Math.Pow(10.0, (double)precision)))
			{
				precision++;
			}
			return precision;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00005D48 File Offset: 0x00003F48
		public static int digitsAfterDot2(double tickSize)
		{
			double num = tickSize;
			int num2 = 0;
			while (num != Math.Round(num, 0))
			{
				num *= 10.0;
				num2++;
			}
			return num2;
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00005D84 File Offset: 0x00003F84
		public static object getGlobalVariable(string param)
		{
			return Methods.GlobalVariables.ContainsKey(param) ? Methods.GlobalVariables[param] : null;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00005DB1 File Offset: 0x00003FB1
		public static void setGlobalVar(string param, object value)
		{
			Methods.GlobalVariables[param] = value;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00005DC4 File Offset: 0x00003FC4
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

		// Token: 0x06000090 RID: 144 RVA: 0x00005E58 File Offset: 0x00004058
		public static Dictionary<string, string> objToDict(ComboBox.ObjectCollection objs)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (object obj in objs)
			{
				KeyValuePair<string, string> keyValuePair = (KeyValuePair<string, string>)obj;
				dictionary.Add(keyValuePair.Key, keyValuePair.Value);
			}
			return dictionary;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00005ED4 File Offset: 0x000040D4
		public static int getIndexByKey(Dictionary<string, string> dict, string key)
		{
			List<KeyValuePair<string, string>> list = dict.ToList<KeyValuePair<string, string>>();
			return list.FindIndex((KeyValuePair<string, string> x) => x.Key == key);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00005F0C File Offset: 0x0000410C
		public static Dictionary<string, string> SortDict(Dictionary<string, string> dict)
		{
			Dictionary<string, string> result;
			try
			{
				result = (Dictionary<string, string>)dict.OrderBy(delegate(KeyValuePair<string, string> entry)
				{
					KeyValuePair<string, string> keyValuePair = entry;
					return keyValuePair.Value;
				});
			}
			catch
			{
				result = new Dictionary<string, string>();
			}
			return result;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00005F64 File Offset: 0x00004164
		public static bool ContainsValue(Dictionary<int, double> myd, int indx)
		{
			return myd.ContainsKey(indx) && myd[indx] != double.NaN && Methods.HasValue(myd[indx]);
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00005FA0 File Offset: 0x000041A0
		public static bool ContainsValue(Dictionary<int, bool> myd, int indx)
		{
			return myd.ContainsKey(indx);
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00005FBC File Offset: 0x000041BC
		public static bool ContainsValue(Dictionary<string, double> myd, string indx)
		{
			return myd.ContainsKey(indx) && myd[indx] != double.NaN && Methods.HasValue(myd[indx]);
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00005FF8 File Offset: 0x000041F8
		public static bool ContainsValue(Dictionary<int, object> myd, int indx)
		{
			return myd.ContainsKey(indx) && myd[indx] != null;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00006020 File Offset: 0x00004220
		public static bool ContainsKey(NameValueCollection collection, string key)
		{
			return collection.Get(key) != null || collection.AllKeys.Contains(key);
		}

		// Token: 0x06000098 RID: 152 RVA: 0x0000604C File Offset: 0x0000424C
		public static bool HasValue(double value)
		{
			return !double.IsNaN(value) && !double.IsInfinity(value);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00006074 File Offset: 0x00004274
		public static Dictionary<int, double> FindNearestValuesInDict(Dictionary<int, double> MyDict, double Target_value, int how_many_values_to_find)
		{
			Dictionary<int, double> result;
			try
			{
				bool flag = Methods.DictIsAscendingOrDescending(MyDict);
				int num = MyDict.Keys.Count<int>();
				int num2 = 0;
				int num3 = 0;
				Dictionary<int, double> dictionary = new Dictionary<int, double>(MyDict);
				for (int i = 0; i < num; i++)
				{
					int index = flag ? i : (num - 1 - i);
					int index2 = flag ? (num - 1 - i) : i;
					bool flag2 = MyDict.Keys.ElementAtOrDefault(index) != 0;
					if (flag2)
					{
						int num4 = MyDict.Keys.ElementAt(index);
						bool flag3 = Methods.ContainsValue(MyDict, num4) && MyDict[num4] > Target_value;
						if (flag3)
						{
							num2++;
							bool flag4 = num2 > how_many_values_to_find;
							if (flag4)
							{
								dictionary.Remove(num4);
							}
						}
					}
					bool flag5 = MyDict.Keys.ElementAtOrDefault(index2) != 0;
					if (flag5)
					{
						int num5 = MyDict.Keys.ElementAt(index2);
						bool flag6 = Methods.ContainsValue(MyDict, num5) && MyDict[num5] < Target_value;
						if (flag6)
						{
							num3++;
							bool flag7 = num3 > how_many_values_to_find;
							if (flag7)
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

		// Token: 0x0600009A RID: 154 RVA: 0x000061D4 File Offset: 0x000043D4
		public static bool DictIsAscendingOrDescending(Dictionary<int, double> MyDict)
		{
			int num = MyDict.Keys.Count<int>();
			double num2 = double.NaN;
			for (int i = 0; i < num; i++)
			{
				int num3 = MyDict.Keys.ElementAt(i);
				bool flag = Methods.ContainsValue(MyDict, num3);
				if (flag)
				{
					bool flag2 = !double.IsNaN(num2);
					if (flag2)
					{
						return MyDict[num3] > num2;
					}
					num2 = MyDict[num3];
				}
			}
			return false;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00006258 File Offset: 0x00004458
		public static string serialize2(object dict)
		{
			bool flag = dict is Dictionary<string, string>;
			string result;
			if (flag)
			{
				result = new JavaScriptSerializer().Serialize(((Dictionary<string, string>)dict).ToDictionary((KeyValuePair<string, string> item) => item.Key.ToString(), (KeyValuePair<string, string> item) => item.Value.ToString()));
			}
			else
			{
				result = new JavaScriptSerializer().Serialize(((Dictionary<string, object>)dict).ToDictionary((KeyValuePair<string, object> item) => item.Key.ToString(), (KeyValuePair<string, object> item) => item.Value.ToString()));
			}
			return result;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00006320 File Offset: 0x00004520
		public static string dictToString(object obj)
		{
			return string.Join("; ", (from x in obj as Dictionary<object, object>
			select x.Key.ToString() + "=" + x.Value.ToString()).ToArray<string>());
		}

		// Token: 0x0600009D RID: 157 RVA: 0x0000636C File Offset: 0x0000456C
		public static Dictionary<string, object> deserialize(string str)
		{
			return Methods.deserializer_35(str);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00006384 File Offset: 0x00004584
		public static Dictionary<string, object> deserializer_35(string str)
		{
			Dictionary<string, object> result;
			try
			{
				result = Methods.JsonMaker.ParseJSON(str);
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x000063B4 File Offset: 0x000045B4
		public static Dictionary<string, string> deserializer_35_stringed(string str)
		{
			Dictionary<string, string> result;
			try
			{
				result = Methods.ConvertToStringDictionary(Methods.JsonMaker.ParseJSON(str));
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x000063E8 File Offset: 0x000045E8
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

		// Token: 0x060000A1 RID: 161 RVA: 0x00006460 File Offset: 0x00004660
		public static string getDictionaryKeyByValue(Dictionary<string, string> dict, string value)
		{
			return dict.FirstOrDefault((KeyValuePair<string, string> x) => x.Value == value).Key;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x0000649C File Offset: 0x0000469C
		public static Dictionary<string, string> xml_to_dictionary(string xml_data)
		{
			Dictionary<string, string> result;
			try
			{
				XDocument doc = XDocument.Parse(xml_data);
				Dictionary<string, string> dataDictionary = new Dictionary<string, string>();
				foreach (XElement element in from p in doc.Descendants()
				where !p.HasElements
				select p)
				{
					int keyInt = 0;
					string keyName = element.Name.LocalName;
					while (dataDictionary.ContainsKey(keyName))
					{
						keyName = element.Name.LocalName + "_" + keyInt++.ToString();
					}
					dataDictionary.Add(keyName, element.Value);
				}
				result = dataDictionary;
			}
			catch (Exception e)
			{
				Methods.m(e.Message);
				result = new Dictionary<string, string>();
			}
			return result;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x000065A0 File Offset: 0x000047A0
		public static Color color_from_hex(string hex_str)
		{
			return ColorTranslator.FromHtml(hex_str);
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x000065B8 File Offset: 0x000047B8
		public static void blink(bool startOrStop, Control ctrl, Color c1, Color c2, int cycleLength_MS, bool BkClr)
		{
			Methods.<>c__DisplayClass64_0 CS$<>8__locals1 = new Methods.<>c__DisplayClass64_0();
			CS$<>8__locals1.startOrStop = startOrStop;
			CS$<>8__locals1.ctrl = ctrl;
			CS$<>8__locals1.c1 = c1;
			CS$<>8__locals1.c2 = c2;
			CS$<>8__locals1.cycleLength_MS = cycleLength_MS;
			CS$<>8__locals1.BkClr = BkClr;
			bool invokeRequired = CS$<>8__locals1.ctrl.InvokeRequired;
			if (invokeRequired)
			{
				CS$<>8__locals1.ctrl.Invoke(new Action(delegate()
				{
					Methods.blink(CS$<>8__locals1.startOrStop, CS$<>8__locals1.ctrl, CS$<>8__locals1.c1, CS$<>8__locals1.c2, CS$<>8__locals1.cycleLength_MS, CS$<>8__locals1.BkClr);
				}));
			}
			else
			{
				Methods.initialColor = (CS$<>8__locals1.BkClr ? CS$<>8__locals1.ctrl.BackColor : CS$<>8__locals1.ctrl.ForeColor);
				bool startOrStop2 = CS$<>8__locals1.startOrStop;
				if (startOrStop2)
				{
					bool flag = Methods.BlinkTimer == null;
					if (flag)
					{
						Methods.BlinkTimer = new System.Windows.Forms.Timer();
						Methods.BlinkTimer.Interval = CS$<>8__locals1.cycleLength_MS;
						Methods.BlinkTimer.Enabled = false;
						Methods.BlinkTimer.Start();
						Methods.BlinkTimer.Tick += CS$<>8__locals1.<blink>g__timer_Tick|1;
					}
				}
				else
				{
					bool bkClr = CS$<>8__locals1.BkClr;
					if (bkClr)
					{
						CS$<>8__locals1.ctrl.BackColor = Methods.initialColor;
					}
					else
					{
						CS$<>8__locals1.ctrl.ForeColor = Methods.initialColor;
					}
					Methods.BlinkTimer.Stop();
					Methods.BlinkTimer.Tick -= CS$<>8__locals1.<blink>g__timer_Tick|1;
					Methods.BlinkTimer = null;
				}
			}
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00006710 File Offset: 0x00004910
		public static string cachedCall(string MethodFullName, object[] parameters, int minutes)
		{
			string result = "";
			try
			{
				int idx = MethodFullName.LastIndexOf('.');
				string className = (idx < 0) ? MethodBase.GetCurrentMethod().DeclaringType.FullName : MethodFullName.Substring(0, idx);
				Type classType = Type.GetType(className);
				string methodName = (idx < 0) ? MethodFullName : MethodFullName.Replace(className + ".", "");
				string fullPathName = className + "." + methodName;
				string key = "Software\\Puvox\\cachedCalls\\aa" + fullPathName + Methods.md5(Methods.tryEnumerabledString(parameters, "")) + minutes.ToString();
				string cacheKey = key + "_cache";
				bool flag = Methods.isCacheSet(cacheKey, minutes);
				if (flag)
				{
					result = Methods.getRegistryValue(key, "nothin");
				}
				else
				{
					MethodInfo method = classType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
					bool flag2 = method == null;
					if (flag2)
					{
						Methods.m(string.Concat(new string[]
						{
							"You are trying to trigger non-existing method:",
							methodName,
							"(",
							className,
							")"
						}));
						return "";
					}
					result = (string)method.Invoke(null, parameters);
					Methods.setRegistryValue(key, result);
				}
			}
			catch (Exception e)
			{
				Methods.m(e);
			}
			return result;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00006878 File Offset: 0x00004A78
		public static RegistryKey getRegistryHiveKey(RegistryHive rh)
		{
			return RegistryKey.OpenBaseKey(rh, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x000068A4 File Offset: 0x00004AA4
		public static void Read64bitRegistryFrom32bitApp(string[] args)
		{
			string value64 = string.Empty;
			string value65 = string.Empty;
			RegistryKey localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
			localKey = localKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");
			bool flag = localKey != null;
			if (flag)
			{
				value64 = localKey.GetValue("RegisteredOrganization").ToString();
			}
			RegistryKey localKey2 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
			localKey2 = localKey2.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");
			bool flag2 = localKey2 != null;
			if (flag2)
			{
				value65 = localKey2.GetValue("RegisteredOrganization").ToString();
			}
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00006930 File Offset: 0x00004B30
		public static string regPartFromKey(string key, bool Dir_or_Key)
		{
			string result;
			if (Dir_or_Key)
			{
				bool flag = Methods.charsInPhrase(key, "\\") != 0;
				if (flag)
				{
					result = Methods.withoutLastDir(key, 1);
				}
				else
				{
					result = key;
				}
			}
			else
			{
				bool flag2 = Methods.charsInPhrase(key, "\\") != 0;
				if (flag2)
				{
					result = Methods.lastPart(key);
				}
				else
				{
					result = key;
				}
			}
			return result;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00006988 File Offset: 0x00004B88
		public static bool existsRegistryValue(string pathKey)
		{
			string path = Methods.regPartFromKey(pathKey, true);
			string key = Methods.regPartFromKey(pathKey, false);
			RegistryKey registryHiveKey = Methods.getRegistryHiveKey(Methods.chosenRegHive);
			bool flag = registryHiveKey == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				RegistryKey registryKey = registryHiveKey.OpenSubKey(path);
				bool flag2 = registryKey == null;
				if (flag2)
				{
					registryHiveKey.Close();
					result = false;
				}
				else
				{
					object value = registryKey.GetValue(key);
					registryKey.Close();
					registryHiveKey.Close();
					result = (value != null);
				}
			}
			return result;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00006A08 File Offset: 0x00004C08
		public static string getRegistryValue(string patKey, string defaultValue)
		{
			return Methods.getRegistryValue(Methods.regPartFromKey(patKey, true), Methods.regPartFromKey(patKey, false), defaultValue);
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00006A30 File Offset: 0x00004C30
		public static string getRegistryValue(string path, string key, object defaultValue)
		{
			try
			{
				bool flag = Methods.myregs.ContainsKey(path + key);
				if (flag)
				{
					return Methods.myregs[path + key];
				}
				RegistryKey registryHiveKey = Methods.getRegistryHiveKey(Methods.chosenRegHive);
				bool flag2 = registryHiveKey == null;
				if (flag2)
				{
					return (defaultValue == null) ? null : defaultValue.ToString();
				}
				RegistryKey registryKey = registryHiveKey.OpenSubKey(path);
				bool flag3 = registryKey == null;
				if (flag3)
				{
					return (defaultValue == null) ? null : defaultValue.ToString();
				}
				object value = registryKey.GetValue(key);
				registryKey.Close();
				registryHiveKey.Close();
				bool flag4 = value == null;
				if (flag4)
				{
					return (defaultValue == null) ? null : defaultValue.ToString();
				}
				Methods.myregs[path + key] = value.ToString();
				return Methods.myregs[path + key];
			}
			catch (Exception e)
			{
				Methods.m(e);
			}
			return null;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00006B40 File Offset: 0x00004D40
		public static void setRegistryValue(string patKey, string value)
		{
			Methods.setRegistryValue(Methods.regPartFromKey(patKey, true), Methods.regPartFromKey(patKey, false), value);
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00006B58 File Offset: 0x00004D58
		public static bool setRegistryValue(string path, string key, string value)
		{
			bool result;
			try
			{
				RegistryKey registryHiveKey = Methods.getRegistryHiveKey(Methods.chosenRegHive);
				bool flag = registryHiveKey != null;
				if (flag)
				{
					RegistryKey registryKey = registryHiveKey.OpenSubKey(path, true);
					bool flag2 = registryKey == null;
					if (flag2)
					{
						registryKey = registryHiveKey.CreateSubKey(path);
					}
					Methods.myregs[path + key] = value;
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

		// Token: 0x060000AE RID: 174 RVA: 0x00006BEC File Offset: 0x00004DEC
		public static string getSetRegistryValue(string key, string value)
		{
			return Methods.getSetRegistryValue(Methods.regPartFromKey(key, true), Methods.regPartFromKey(key, false), value);
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00006C14 File Offset: 0x00004E14
		public static string getSetRegistryValue(string path, string key, string value)
		{
			string result;
			try
			{
				string registryValue = Methods.getRegistryValue(path, key);
				bool flag = string.IsNullOrEmpty(registryValue);
				if (flag)
				{
					Methods.setRegistryValue(path, key, value);
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

		// Token: 0x060000B0 RID: 176 RVA: 0x00006C9C File Offset: 0x00004E9C
		public static string Read(string subKey, string KeyName)
		{
			string result;
			try
			{
				RegistryKey registryKey = Methods.chosenRegKey.OpenSubKey(subKey);
				bool flag = registryKey == null;
				if (flag)
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

		// Token: 0x060000B1 RID: 177 RVA: 0x00006D1C File Offset: 0x00004F1C
		public static bool Write(string subKey, string KeyName, object Value)
		{
			bool result;
			try
			{
				RegistryKey registryKey = Methods.chosenRegKey;
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

		// Token: 0x060000B2 RID: 178 RVA: 0x00006D88 File Offset: 0x00004F88
		public static bool DeleteKey(string subKey, string KeyName)
		{
			bool result;
			try
			{
				RegistryKey registryKey = Methods.chosenRegKey;
				RegistryKey registryKey2 = registryKey.CreateSubKey(subKey);
				bool flag = registryKey2 == null;
				if (flag)
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

		// Token: 0x060000B3 RID: 179 RVA: 0x00006E00 File Offset: 0x00005000
		public static bool DeleteSubKeyTree(string subKey)
		{
			bool result;
			try
			{
				RegistryKey registryKey = Methods.chosenRegKey;
				RegistryKey registryKey2 = registryKey.OpenSubKey(subKey);
				bool flag = registryKey2 != null;
				if (flag)
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

		// Token: 0x060000B4 RID: 180 RVA: 0x00006E70 File Offset: 0x00005070
		public static int SubKeyCount(string subKey)
		{
			int result;
			try
			{
				RegistryKey registryKey = Methods.chosenRegKey;
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

		// Token: 0x060000B5 RID: 181 RVA: 0x00006ED8 File Offset: 0x000050D8
		public static int ValueCount(string subKey)
		{
			int result;
			try
			{
				RegistryKey registryKey = Methods.chosenRegKey;
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

		// Token: 0x060000B6 RID: 182 RVA: 0x00006F40 File Offset: 0x00005140
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

		// Token: 0x060000B7 RID: 183 RVA: 0x00006FE0 File Offset: 0x000051E0
		public static bool isCacheSet(string PathKey, int minutes)
		{
			string registryValue = Methods.getRegistryValue(PathKey, "");
			bool flag = string.IsNullOrEmpty(registryValue) || DateTime.Now > DateTime.Parse(registryValue).AddMinutes((double)minutes);
			bool result;
			if (flag)
			{
				Methods.setRegistryValue(PathKey, DateTime.Now.ToString());
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00007044 File Offset: 0x00005244
		public string serialize(Dictionary<string, object> dict)
		{
			string result;
			try
			{
				result = "";
			}
			catch (Exception e)
			{
				Methods.m(e.Message);
				result = "";
			}
			return result;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00007084 File Offset: 0x00005284
		public string serialize(Dictionary<string, string> dict)
		{
			return "";
		}

		// Token: 0x060000BA RID: 186 RVA: 0x0000709C File Offset: 0x0000529C
		public string MyDictionaryToJson(Dictionary<string, int> dict)
		{
			IEnumerable<string> entries = from d in dict
			select string.Format("\"{0}\": [{1}]", d.Key, string.Join(",", new object[]
			{
				d.Value
			}));
			return "{" + string.Join(",", entries) + "}";
		}

		// Token: 0x060000BB RID: 187 RVA: 0x000070F0 File Offset: 0x000052F0
		public string regplace(string input, string pattern)
		{
			return this.regplace(input, pattern, "");
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00007110 File Offset: 0x00005310
		public string regplace(string input, string pattern, string with)
		{
			return new Regex(pattern).Replace(input, "");
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00007134 File Offset: 0x00005334
		public static string lastPart(string path)
		{
			bool flag = path.Contains("\\");
			string result;
			if (flag)
			{
				result = path.Split(new char[]
				{
					'\\'
				}).Last<string>();
			}
			else
			{
				result = path.Split(new char[]
				{
					'/'
				}).Last<string>();
			}
			return result;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00007188 File Offset: 0x00005388
		public static int charsInPhrase(string source, string char_)
		{
			int num = 0;
			for (int i = 0; i < source.Length; i++)
			{
				bool flag = source[i].ToString() == char_;
				if (flag)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x000071D8 File Offset: 0x000053D8
		public static string sanitizer(string dirtyString)
		{
			string a = "replace";
			string text = " ?&^$#@!()+-,:;<>\\?'\"-_*";
			string text2 = dirtyString;
			bool flag = a == "replace";
			if (flag)
			{
				string text3 = text;
				for (int i = 0; i < text3.Length; i++)
				{
					text2 = text2.Replace(text3[i].ToString(), "_");
				}
			}
			else
			{
				bool flag2 = !(a == "regex");
				if (flag2)
				{
					bool flag3 = a == "ld";
					if (flag3)
					{
						text2 = new string(text2.Where(new Func<char, bool>(char.IsLetterOrDigit)).ToArray<char>());
					}
					else
					{
						bool flag4 = a == "hashset";
						if (flag4)
						{
							HashSet<char> hashSet = new HashSet<char>(text);
							StringBuilder stringBuilder = new StringBuilder(text2.Length);
							foreach (char c in text2)
							{
								bool flag5 = hashSet.Contains(c);
								if (flag5)
								{
									stringBuilder.Append(c);
								}
							}
							text2 = stringBuilder.ToString();
						}
					}
				}
			}
			return text2;
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00007310 File Offset: 0x00005510
		public static bool stringInArrayStart(string str, object array)
		{
			bool result = false;
			foreach (object obj in ((IList)array))
			{
				bool flag = str.StartsWith(obj.ToString());
				if (flag)
				{
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00007380 File Offset: 0x00005580
		public static bool stringInArray(string str, object array)
		{
			bool result = false;
			foreach (object obj in ((IList)array))
			{
				bool flag = str == obj.ToString();
				if (flag)
				{
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x000073F4 File Offset: 0x000055F4
		public static string urlEncode(string msg)
		{
			return Uri.EscapeDataString(msg);
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x0000740C File Offset: 0x0000560C
		public static int countWords(string str)
		{
			char[] separator = new char[]
			{
				' ',
				'\r',
				'\n'
			};
			return str.Split(separator, StringSplitOptions.RemoveEmptyEntries).Length;
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x0000743C File Offset: 0x0000563C
		public static string string_repeat(string str, int level)
		{
			return new StringBuilder().Insert(0, str, level).ToString();
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00007460 File Offset: 0x00005660
		public static string pChars(string txt)
		{
			int num = Math.Max(1, 25 - txt.Length);
			string text = txt + new string(' ', num);
			bool flag = num > 1;
			if (flag)
			{
				text += "\t";
			}
			return text;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x000074A8 File Offset: 0x000056A8
		public static string textLengthen(string txt, int char_count, string letter)
		{
			while (txt.Length < char_count)
			{
				txt += letter;
			}
			return txt;
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x000074D8 File Offset: 0x000056D8
		public static string SplitToLines(string text, char[] splitOnCharacters, int maxStringLength)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			while (text.Length > num)
			{
				bool flag = num != 0;
				if (flag)
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

		// Token: 0x060000C8 RID: 200 RVA: 0x00007564 File Offset: 0x00005764
		public static string RandomString(int length)
		{
			string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			char[] stringChars = new char[length];
			Random random = new Random();
			for (int i = 0; i < stringChars.Length; i++)
			{
				stringChars[i] = chars[random.Next(chars.Length)];
			}
			return new string(stringChars);
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x000075C4 File Offset: 0x000057C4
		public static string DateToSeconds(DateTime date)
		{
			return date.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
		}

		// Token: 0x060000CA RID: 202 RVA: 0x000075E8 File Offset: 0x000057E8
		public static string convertBackSlashes(string path)
		{
			return path.Replace("/", "\\");
		}

		// Token: 0x060000CB RID: 203 RVA: 0x0000760C File Offset: 0x0000580C
		public static string regexReplace(string input, string pattern, string with)
		{
			return new Regex(pattern).Replace(input, "");
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00007630 File Offset: 0x00005830
		public static string SanitizeSymbol(string s)
		{
			return s.Replace("/", "_").Replace("\\", "_").Replace("|", "_").Replace("*", "_").ToUpper();
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00007684 File Offset: 0x00005884
		public static string NotNull(string smth)
		{
			bool flag = !string.IsNullOrEmpty(smth);
			string result;
			if (flag)
			{
				result = smth;
			}
			else
			{
				result = "";
			}
			return result;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x000076B0 File Offset: 0x000058B0
		public static bool empty(string text)
		{
			return text == null || text.Trim().Length == 0 || text.Trim() == "";
		}

		// Token: 0x060000CF RID: 207 RVA: 0x000076E8 File Offset: 0x000058E8
		public static bool normalDouble(double val)
		{
			return !double.IsNaN(val) && val < double.MaxValue && val > double.MinValue;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00007720 File Offset: 0x00005920
		public static string Repeat(string value, int count)
		{
			return new StringBuilder(value.Length * count).Insert(0, value, count).ToString();
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x0000774C File Offset: 0x0000594C
		public static string[] file_to_lines(string file_location)
		{
			string[] result = new string[0];
			bool flag = File.Exists(file_location);
			if (flag)
			{
				result = File.ReadAllText(file_location).Split(new string[]
				{
					Environment.NewLine,
					"\\r"
				}, StringSplitOptions.None);
			}
			return result;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00007798 File Offset: 0x00005998
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
				string[] array2 = text3.Split(new string[0], StringSplitOptions.RemoveEmptyEntries);
				int num2 = 0;
				foreach (string str in array2)
				{
					num2++;
					bool flag = num2 < after_how_many_words;
					if (flag)
					{
						text2 = text2 + str + " ";
					}
					else
					{
						bool flag2 = num2 == after_how_many_words;
						if (flag2)
						{
							text2 = text2 + str + Environment.NewLine;
							num2 = 0;
						}
					}
				}
				bool flag3 = num != array.Count<string>();
				if (flag3)
				{
					text2 += "\r\n";
				}
			}
			return text2;
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00007890 File Offset: 0x00005A90
		public static string EncryptString(string plainText, string password)
		{
			string result;
			try
			{
				SHA256 mySHA256 = SHA256.Create();
				byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
				byte[] iv = new byte[16];
				Aes encryptor = Aes.Create();
				encryptor.Mode = CipherMode.CBC;
				encryptor.Key = key;
				encryptor.IV = iv;
				MemoryStream memoryStream = new MemoryStream();
				ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();
				CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);
				byte[] plainBytes = Encoding.ASCII.GetBytes(plainText);
				cryptoStream.Write(plainBytes, 0, plainBytes.Length);
				cryptoStream.FlushFinalBlock();
				byte[] cipherBytes = memoryStream.ToArray();
				memoryStream.Close();
				cryptoStream.Close();
				string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);
				result = cipherText;
			}
			catch (Exception e)
			{
				result = e.Message;
			}
			return result;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x0000796C File Offset: 0x00005B6C
		public static string DecryptString(string cipherText, string password)
		{
			string result;
			try
			{
				SHA256 mySHA256 = SHA256.Create();
				byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
				byte[] iv = new byte[16];
				Aes encryptor = Aes.Create();
				encryptor.Mode = CipherMode.CBC;
				encryptor.Key = key;
				encryptor.IV = iv;
				MemoryStream memoryStream = new MemoryStream();
				ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();
				CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);
				string plainText = string.Empty;
				try
				{
					byte[] cipherBytes = Convert.FromBase64String(cipherText);
					cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
					cryptoStream.FlushFinalBlock();
					byte[] plainBytes = memoryStream.ToArray();
					plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
				}
				catch (Exception e)
				{
					MessageBox.Show("Problem in encryption. ErrorCode 274. " + e.Message);
				}
				finally
				{
					memoryStream.Close();
					cryptoStream.Close();
				}
				result = plainText;
			}
			catch (Exception e2)
			{
				result = e2.Message;
			}
			return result;
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00007A8C File Offset: 0x00005C8C
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

		// Token: 0x060000D6 RID: 214 RVA: 0x00007B38 File Offset: 0x00005D38
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

		// Token: 0x060000D7 RID: 215 RVA: 0x00007B80 File Offset: 0x00005D80
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

		// Token: 0x060000D8 RID: 216 RVA: 0x00007BC8 File Offset: 0x00005DC8
		public static string decode64(string str)
		{
			byte[] bytes = Convert.FromBase64String(str);
			return Encoding.UTF8.GetString(bytes);
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00007BEC File Offset: 0x00005DEC
		public static void Dispose(IDisposable resource)
		{
			bool flag = resource != null;
			if (flag)
			{
				resource.Dispose();
			}
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00007C0C File Offset: 0x00005E0C
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
			TextBox textBox = new TextBox();
			form.Controls.Add(textBox);
			textBox.Multiline = true;
			textBox.ScrollBars = ScrollBars.Both;
			textBox.Text = obj_.ToString();
			form.ShowDialog();
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00007CAC File Offset: 0x00005EAC
		public static string ShowDialog(string text)
		{
			return Methods.ShowDialog(text, "", "");
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00007CD0 File Offset: 0x00005ED0
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
				Label value = new Label
				{
					Left = 50,
					Top = 20,
					Text = text
				};
				TextBox textBox = new TextBox
				{
					Left = 50,
					Top = 50,
					Width = 400
				};
				textBox.Text = defaultValue;
				Button button = new Button
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

		// Token: 0x060000DD RID: 221 RVA: 0x00007E58 File Offset: 0x00006058
		public static string GetControlText(Form obj, string which)
		{
			Control[] array = obj.Controls.Find(which, true);
			bool flag = array.Length == 0;
			string result;
			if (flag)
			{
				result = "cant find control";
			}
			else
			{
				result = array[0].Text;
			}
			return result;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00007E94 File Offset: 0x00006094
		public static void CenterLabel(Control ctrl)
		{
			try
			{
				ctrl.AutoSize = false;
				ctrl.Dock = DockStyle.None;
				ctrl.Width = ctrl.Parent.Width;
				bool flag = ctrl is Label;
				if (flag)
				{
				}
			}
			catch (Exception ex)
			{
				Methods.m(ex.Message);
			}
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00007EF8 File Offset: 0x000060F8
		public static bool isUserInput(Control ct)
		{
			return ct is TextBox || ct is ComboBox || ct is CheckBox || ct is CheckedListBox || ct is DateTimePicker || ct is GroupBox || ct is ListBox || ct is MonthCalendar || ct is RadioButton || ct is RichTextBox || ct is TrackBar;
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00007F68 File Offset: 0x00006168
		public static string getFormOption(string key, string two, string regKeyBase)
		{
			return Methods.getRegistryValue(regKeyBase + Methods.optionsPrefix + key, two);
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00007F8C File Offset: 0x0000618C
		public static bool fillFormOptions(Control.ControlCollection cts, string regKeyBase)
		{
			foreach (object obj in cts)
			{
				Control control = (Control)obj;
				bool flag = control != null && Methods.isUserInput(control);
				if (flag)
				{
					string defaultVal = control.Text;
					string regValue = Methods.getRegistryValue(regKeyBase + Methods.optionsPrefix + control.Name, defaultVal).ToLower();
					bool flag2 = control is CheckBox;
					if (flag2)
					{
						(control as CheckBox).Checked = (regValue == "true");
					}
					else
					{
						control.Text = Methods.getRegistryValue(regKeyBase + Methods.optionsPrefix + control.Name, regValue);
					}
				}
			}
			return true;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00008078 File Offset: 0x00006278
		public static bool saveFormOptions(Control.ControlCollection cts, string regKeyBase)
		{
			foreach (object obj in cts)
			{
				Control control = (Control)obj;
				bool flag = control != null && Methods.isUserInput(control);
				if (flag)
				{
					string value = control.Text;
					bool flag2 = control is CheckBox;
					if (flag2)
					{
						value = (((CheckBox)control).Checked ? "true" : "false");
					}
					Methods.setRegistryValue(regKeyBase + Methods.optionsPrefix + control.Name, value);
				}
			}
			return true;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00008134 File Offset: 0x00006334
		public static void SetComboboxByKey(ComboBox cb1, string key)
		{
			cb1.SelectedIndex = Methods.getIndexByKey(Methods.objToDict(cb1.Items), key);
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x0000814F File Offset: 0x0000634F
		public static void FillCombobox(ComboBox cb1, Dictionary<string, string> dict)
		{
			Methods.FillCombobox(cb1, dict, false, "");
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00008160 File Offset: 0x00006360
		public static void FillCombobox(ComboBox cb1, Dictionary<string, string> dict, bool sort, string SelectedValue)
		{
			cb1.DataSource = new BindingSource(sort ? Methods.SortDict(dict) : dict, null);
			cb1.DisplayMember = "Value";
			cb1.ValueMember = "Key";
			bool flag = SelectedValue != "";
			if (flag)
			{
				cb1.SelectedValue = SelectedValue;
			}
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x000081B8 File Offset: 0x000063B8
		public static void invokeSafe(Control uiElement, Action updater, bool forceSynchronous)
		{
			try
			{
				bool flag = uiElement != null;
				if (flag)
				{
					bool invokeRequired = uiElement.InvokeRequired;
					if (invokeRequired)
					{
						bool forceSynchronous2 = forceSynchronous;
						if (forceSynchronous2)
						{
							uiElement.Invoke(new Action(delegate()
							{
								Methods.invokeSafe(uiElement, updater, forceSynchronous);
							}));
						}
						else
						{
							uiElement.BeginInvoke(new Action(delegate()
							{
								Methods.invokeSafe(uiElement, updater, forceSynchronous);
							}));
						}
					}
					else
					{
						bool flag2 = !uiElement.IsDisposed;
						if (flag2)
						{
							updater();
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00008280 File Offset: 0x00006480
		public static bool Control_Set(Control ctrl, string what, object value)
		{
			bool result;
			try
			{
				bool flag = ctrl == null;
				if (flag)
				{
					result = false;
				}
				else
				{
					bool invokeRequired = ctrl.InvokeRequired;
					if (invokeRequired)
					{
						Methods.Control_SetDelegate method = new Methods.Control_SetDelegate(Methods.Control_Set);
						ctrl.Invoke(method, new object[]
						{
							ctrl,
							what,
							value
						});
					}
					else
					{
						bool flag2 = what == "visibility";
						if (flag2)
						{
							ctrl.Visible = (bool)value;
						}
						else
						{
							bool flag3 = what == "text";
							if (flag3)
							{
								ctrl.Text = (string)value;
							}
							else
							{
								Methods.m("incorrect value in :" + MethodBase.GetCurrentMethod().Name + ":" + what);
							}
						}
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

		// Token: 0x060000E8 RID: 232 RVA: 0x00008360 File Offset: 0x00006560
		public static bool closeForm(Form frm)
		{
			bool result;
			try
			{
				bool flag = frm == null;
				if (flag)
				{
					result = false;
				}
				else
				{
					bool isHandleCreated = frm.IsHandleCreated;
					if (isHandleCreated)
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

		// Token: 0x060000E9 RID: 233 RVA: 0x000083F0 File Offset: 0x000065F0
		public static void SetControlText(Form obj, string which, string txt)
		{
			try
			{
				bool flag = obj != null;
				if (flag)
				{
					bool invokeRequired = obj.InvokeRequired;
					if (invokeRequired)
					{
						obj.Invoke(new Action(delegate()
						{
							Methods.SetControlText(obj, which, txt);
						}));
					}
					else
					{
						Control[] array = obj.Controls.Find(which, true);
						bool flag2 = array.Count<Control>() > 0;
						if (flag2)
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

		// Token: 0x060000EA RID: 234 RVA: 0x000084A8 File Offset: 0x000066A8
		public static void SetTextControl(Form form, Control ctrl, string text)
		{
			try
			{
				bool flag = ctrl != null;
				if (flag)
				{
					bool invokeRequired = ctrl.InvokeRequired;
					if (invokeRequired)
					{
						Methods.SetTextCallback method = new Methods.SetTextCallback(Methods.SetTextControl);
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

		// Token: 0x060000EB RID: 235 RVA: 0x00008518 File Offset: 0x00006718
		public Methods.TimerInterrupter SetInterval(int interval, Action function)
		{
			return this.StartTimer(interval, function, true);
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00008534 File Offset: 0x00006734
		public Methods.TimerInterrupter SetTimeout(int interval, Action function)
		{
			return this.StartTimer(interval, function, false);
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00008550 File Offset: 0x00006750
		private Methods.TimerInterrupter StartTimer(int interval, Action function, bool autoReset)
		{
			Methods.TimerInterrupter result;
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
				result = new Methods.TimerInterrupter(timer);
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060000EE RID: 238 RVA: 0x000085D0 File Offset: 0x000067D0
		public void ExecuteAfter(int milliseconds, Action action)
		{
			try
			{
				System.Threading.Timer timer = null;
				timer = new System.Threading.Timer(delegate(object s)
				{
					action();
					timer.Dispose();
					HashSet<System.Threading.Timer> executeAfter_timers2 = this.ExecuteAfter_timers;
					lock (executeAfter_timers2)
					{
						this.ExecuteAfter_timers.Remove(timer);
					}
				}, null, (long)milliseconds, -11L);
				HashSet<System.Threading.Timer> executeAfter_timers = this.ExecuteAfter_timers;
				lock (executeAfter_timers)
				{
					this.ExecuteAfter_timers.Add(timer);
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x060000EF RID: 239 RVA: 0x0000867C File Offset: 0x0000687C
		public static void Timer(Action<object, ElapsedEventArgs> myMethod, int interval, bool autoreset)
		{
			System.Timers.Timer timer = new System.Timers.Timer();
			timer.Elapsed += myMethod.Invoke;
			timer.Interval = (double)interval;
			timer.AutoReset = autoreset;
			timer.Start();
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x000086BC File Offset: 0x000068BC
		public static List<string> PropertyInfoToList(PropertyInfo[] PropList)
		{
			List<string> list = new List<string>();
			foreach (PropertyInfo propertyInfo in PropList)
			{
				list.Add(propertyInfo.Name);
			}
			return list;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x000086FC File Offset: 0x000068FC
		public static List<string> ObjectPropertyNames(object Obj)
		{
			return Methods.PropertyInfoToList(Obj.GetType().GetProperties());
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00008720 File Offset: 0x00006920
		public static bool MethodExists(object objectToCheck, string methodName)
		{
			return objectToCheck.GetType().GetMethod(methodName) != null;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00008744 File Offset: 0x00006944
		public static string Trace(Exception ex)
		{
			StackTrace stackTrace = new StackTrace(ex, true);
			StackFrame frame = stackTrace.GetFrame(0);
			return frame.GetFileLineNumber().ToString();
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00008774 File Offset: 0x00006974
		public static string urlRead(string url)
		{
			string responseText = "-1";
			try
			{
				using (WebClient wc = new WebClient())
				{
					wc.Encoding = Encoding.UTF8;
					responseText = wc.DownloadString(url);
				}
			}
			catch (Exception e)
			{
				return "Error:" + e.ToString();
			}
			return responseText;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x000087EC File Offset: 0x000069EC
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

		// Token: 0x060000F6 RID: 246 RVA: 0x00008848 File Offset: 0x00006A48
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

		// Token: 0x060000F7 RID: 247 RVA: 0x000088EC File Offset: 0x00006AEC
		public static string tempFilePathForUrlContent(string url)
		{
			Uri uri = new Uri(url);
			string text = Methods.convertBackSlashes(Path.GetTempPath() + "\\" + url.Replace(uri.Scheme, "").Replace("://", "").Replace("//", ""));
			bool flag = !File.Exists(text);
			if (flag)
			{
				string directoryName = Path.GetDirectoryName(text);
				bool flag2 = !Directory.Exists(directoryName);
				if (flag2)
				{
					Directory.CreateDirectory(directoryName);
				}
				Methods.DownloadFile(url, text);
			}
			return text;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00008982 File Offset: 0x00006B82
		public static void urlOpen(string url)
		{
			new WebBrowser
			{
				AllowNavigation = true
			}.Navigate(url);
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x0000899C File Offset: 0x00006B9C
		public static bool internetAvailable(string messageShow)
		{
			try
			{
				using (WebClient client = new WebClient())
				{
					using (client.OpenRead("http://google.com/generate_204"))
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				bool flag = !string.IsNullOrEmpty(messageShow);
				if (flag)
				{
					Methods.m(messageShow + ex.Message);
				}
			}
			return false;
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00008A2C File Offset: 0x00006C2C
		public static string withoutLastDir(string path, int HowManyLastDirs)
		{
			string result = path;
			bool flag = Methods.charsInPhrase(path, "\\") > HowManyLastDirs;
			if (flag)
			{
				result = string.Join("\\", path.Split(new char[]
				{
					'\\'
				}).Reverse<string>().Skip(HowManyLastDirs).Reverse<string>().ToArray<string>());
			}
			return result;
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00008A88 File Offset: 0x00006C88
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

		// Token: 0x060000FC RID: 252 RVA: 0x00008B04 File Offset: 0x00006D04
		public static void WriteTempFile(string fileName, string text, bool rewrite_or_add)
		{
			string path = Environment.GetEnvironmentVariable("tmp") + "\\" + fileName + ".txt";
			if (rewrite_or_add)
			{
				File.WriteAllText(path, text);
			}
			else
			{
				File.AppendAllText(path, text);
			}
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00008B48 File Offset: 0x00006D48
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

		// Token: 0x060000FE RID: 254 RVA: 0x00008BA4 File Offset: 0x00006DA4
		public static string ReadFile(string path)
		{
			bool flag = File.Exists(path);
			string result;
			if (flag)
			{
				result = File.ReadAllText(path);
			}
			else
			{
				result = "";
			}
			return result;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00008BD0 File Offset: 0x00006DD0
		public static string readFile(string filepath)
		{
			string result = "";
			bool flag = File.Exists(filepath);
			if (flag)
			{
				using (StreamReader streamReader = new StreamReader(filepath, Encoding.UTF8))
				{
					result = streamReader.ReadToEnd();
				}
			}
			return result;
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00008C28 File Offset: 0x00006E28
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

		// Token: 0x06000101 RID: 257 RVA: 0x00008C6C File Offset: 0x00006E6C
		public static bool deleteDir(string path)
		{
			bool result;
			try
			{
				bool flag = !Directory.Exists(path);
				if (flag)
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

		// Token: 0x06000102 RID: 258 RVA: 0x00008CB8 File Offset: 0x00006EB8
		public static void list_all_files_in_dir(string base_path)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(base_path);
			FileInfo[] files = directoryInfo.GetFiles("*.*");
			string text = "";
			foreach (FileInfo fileInfo in files)
			{
				text = text + ", " + fileInfo.Name;
			}
			MessageBox.Show(text);
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00008D18 File Offset: 0x00006F18
		public static string getDesktopPath()
		{
			return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00008D30 File Offset: 0x00006F30
		public static string getLangFromIsoLang(string isoLang)
		{
			return new Regex("\\-(.*)", RegexOptions.IgnoreCase).Replace(isoLang, "");
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00008D58 File Offset: 0x00006F58
		public static void googleSetKeyFile(string FileContent)
		{
			Methods.gsApkPath = Environment.GetEnvironmentVariable("appdata") + "\\gsapk.xyz";
			File.WriteAllText(Methods.gsApkPath, FileContent);
			Methods.SetGoogleCredentialsDefaults(Methods.gsApkPath);
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00008D8B File Offset: 0x00006F8B
		public static void SetGoogleCredentialsDefaults(string path)
		{
			Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00008D9C File Offset: 0x00006F9C
		public static void setTexts(Dictionary<string, string> dict, string lang_2char, Form frm)
		{
			foreach (KeyValuePair<string, string> v in dict)
			{
				string keyNm_this = "trans_" + v.Value + "_" + lang_2char;
				string keyNm_ENG = "trans_" + v.Value + "_en";
				string val_this = Methods.getRegistryValue(keyNm_this, "");
				string val_ENG = Methods.getRegistryValue(keyNm_ENG, "");
				bool flag = string.IsNullOrEmpty(val_this) || (!string.IsNullOrEmpty(val_ENG) && val_ENG != v.Key);
				if (flag)
				{
					bool flag2 = lang_2char == "en";
					if (flag2)
					{
						val_this = v.Key;
					}
					else
					{
						val_this = v.Key;
					}
					Methods.setRegistryValue(keyNm_this, val_this);
				}
				Methods.SetControlText(frm, v.Value, val_this);
			}
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00008EA8 File Offset: 0x000070A8
		public static string GTranslate_checker(string what, string lang_target)
		{
			bool flag = Methods.MethodExists(null, "GTranslate_callback");
			string result;
			if (flag)
			{
				Type typeFromHandle = typeof(Methods);
				MethodInfo method = typeFromHandle.GetMethod("GTranslate_callback", BindingFlags.Instance | BindingFlags.Public);
				result = (string)method.Invoke(null, new object[]
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

		// Token: 0x06000109 RID: 265 RVA: 0x00008F10 File Offset: 0x00007110
		public static string translate(string targetLang, string englishText)
		{
			bool flag = !Methods.transl_texts.ContainsKey(englishText);
			if (flag)
			{
				Methods.transl_texts[englishText] = new Dictionary<string, string>();
			}
			bool flag2 = Methods.transl_texts[englishText].ContainsKey(targetLang);
			string result2;
			if (flag2)
			{
				result2 = Methods.transl_texts[englishText][targetLang];
			}
			else
			{
				string key = string.Concat(new string[]
				{
					Methods.translationsBaseReg,
					"t_",
					Methods.md5(englishText),
					"_",
					targetLang
				});
				bool flag3 = targetLang != "en";
				string result;
				if (flag3)
				{
					string registryValue = Methods.getRegistryValue(key, "");
					bool flag4 = string.IsNullOrEmpty(registryValue);
					if (flag4)
					{
						result = englishText;
						Methods.setRegistryValue(key, result);
					}
					else
					{
						result = registryValue;
					}
				}
				else
				{
					result = englishText;
					Methods.setRegistryValue(key, englishText);
				}
				Methods.transl_texts[englishText][targetLang] = result;
				result2 = result;
			}
			return result2;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00009014 File Offset: 0x00007214
		public static string gTranslate(string what, string lang_target, bool useApi = false, string ApiURL = "")
		{
			string text = "en";
			string text2 = "";
			if (useApi)
			{
				string text3 = Methods.urlRead(string.Concat(new string[]
				{
					ApiURL,
					"&action=translate&lang=",
					lang_target,
					"&string[]=",
					Methods.urlEncode(what),
					Methods.DeveloperMachineAddition()
				}));
			}
			else
			{
				string text4 = Methods.urlRead(string.Concat(new string[]
				{
					"https://translate.googleapis.com/translate_a/single?client=gtx&sl=",
					text,
					"&tl=",
					lang_target,
					"&dt=t&q=",
					Methods.urlEncode(what)
				}));
				Match match = Regex.Match(text4, "\\[\\[\\[\"(.*?)\",\"");
				bool success = match.Success;
				if (success)
				{
					text2 = match.Groups[1].Value;
					text2 = text2.Replace("\\", "");
				}
			}
			return text2;
		}

		// Token: 0x0600010B RID: 267 RVA: 0x000090F8 File Offset: 0x000072F8
		public static bool IsDeveloperMachine()
		{
			return Environment.GetEnvironmentVariable(Methods.developerMachineString) != null;
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00009118 File Offset: 0x00007318
		public static string DeveloperMachineAddition()
		{
			bool flag = Methods.IsDeveloperMachine();
			string result;
			if (flag)
			{
				result = "&developerKey=" + Environment.GetEnvironmentVariable(Methods.developerMachineString).ToString();
			}
			else
			{
				result = "";
			}
			return result;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00009158 File Offset: 0x00007358
		public static string removeWhitespaces(string str)
		{
			return str.Replace("      ", " ").Replace("     ", " ").Replace("    ", " ").Replace("   ", " ").Replace("  ", " ").Replace(Environment.NewLine, "").Replace("\t", "");
		}

		// Token: 0x0600010E RID: 270 RVA: 0x000091D4 File Offset: 0x000073D4
		public static void m(int obj)
		{
			Methods.m(obj.ToString());
		}

		// Token: 0x0600010F RID: 271 RVA: 0x000091E4 File Offset: 0x000073E4
		public static void m(double obj)
		{
			Methods.m(obj.ToString());
		}

		// Token: 0x06000110 RID: 272 RVA: 0x000091F4 File Offset: 0x000073F4
		public static void m(bool obj)
		{
			Methods.m(obj.ToString());
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00009204 File Offset: 0x00007404
		public static void m(Exception obj)
		{
			Methods.m(obj.ToString());
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00009214 File Offset: 0x00007414
		public static void m(Dictionary<object, object> obj)
		{
			try
			{
				Methods.m(Methods.tryEnumerabledString(obj));
			}
			catch
			{
				Methods.m(obj.ToString());
			}
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00009204 File Offset: 0x00007404
		public static void m(object obj)
		{
			Methods.m(obj.ToString());
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00009254 File Offset: 0x00007454
		public static void m(object obj, bool showTrace)
		{
			Methods.m(obj.ToString() + Environment.NewLine + Methods.stackFramesString(new StackTrace()));
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00009278 File Offset: 0x00007478
		public static void m(object[] obj)
		{
			bool flag = obj == null;
			if (flag)
			{
				Methods.m("null");
			}
			else
			{
				string text = "";
				foreach (object obj2 in obj)
				{
					text = text + ((obj2 == null) ? "null" : obj.ToString()) + Methods.nl_;
				}
				Methods.m(text);
			}
		}

		// Token: 0x06000116 RID: 278 RVA: 0x000092DC File Offset: 0x000074DC
		public static void m(string obj)
		{
			MessageBox.Show(new Form
			{
				TopMost = true
			}, (obj == null) ? "null" : obj.ToString());
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00009302 File Offset: 0x00007502
		public static void cw(int obj)
		{
			Methods.cw(obj.ToString());
		}

		// Token: 0x06000118 RID: 280 RVA: 0x00009312 File Offset: 0x00007512
		public static void cw(double obj)
		{
			Methods.cw(obj.ToString());
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00009322 File Offset: 0x00007522
		public static void cw(bool obj)
		{
			Methods.cw(obj.ToString());
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00009332 File Offset: 0x00007532
		public static void cw(Exception obj)
		{
			Methods.cw(obj.ToString());
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00009332 File Offset: 0x00007532
		public static void cw(object obj)
		{
			Methods.cw(obj.ToString());
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00009344 File Offset: 0x00007544
		public static void cw(object[] obj)
		{
			bool flag = obj == null;
			if (flag)
			{
				Methods.cw("null");
			}
			else
			{
				string text = "";
				foreach (object obj2 in obj)
				{
					text = text + ((obj2 == null) ? "null" : obj.ToString()) + Methods.nl_;
				}
				Methods.cw(text);
			}
		}

		// Token: 0x0600011D RID: 285 RVA: 0x000093A8 File Offset: 0x000075A8
		public static void cw(string obj)
		{
			Console.Write((obj == null) ? "null" : obj.ToString());
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00004816 File Offset: 0x00002A16
		public static void cl(object obj)
		{
			Console.WriteLine((obj == null) ? "null" : obj.ToString());
			Debug.WriteLine((obj == null) ? "null" : obj.ToString());
		}

		// Token: 0x0600011F RID: 287 RVA: 0x000093C1 File Offset: 0x000075C1
		public static void dumpmsg(object obj)
		{
			Methods.m(Methods.dump(obj));
		}

		// Token: 0x06000120 RID: 288 RVA: 0x000093D0 File Offset: 0x000075D0
		public static string dump(object obj)
		{
			return Methods.dump(obj, Methods.AllBindingFlags, false, 1, 1, "");
		}

		// Token: 0x06000121 RID: 289 RVA: 0x000093F8 File Offset: 0x000075F8
		public static string dump(object obj, int deep)
		{
			return Methods.dump(obj, Methods.AllBindingFlags, false, deep, 1, "");
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00009420 File Offset: 0x00007620
		public static string dump(object obj, bool execMethods)
		{
			return Methods.dump(obj, Methods.AllBindingFlags, execMethods, 1, 1, "");
		}

		// Token: 0x06000123 RID: 291 RVA: 0x00009448 File Offset: 0x00007648
		public static string dump(object obj, int deep, bool execMethods)
		{
			return Methods.dump(obj, Methods.AllBindingFlags, execMethods, deep, 1, "");
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00009470 File Offset: 0x00007670
		public static string dump(object obj, BindingFlags bindingFlags, bool execMethods)
		{
			return Methods.dump(obj, bindingFlags, execMethods, 1, 1, "");
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00009494 File Offset: 0x00007694
		private static string dump(object obj, BindingFlags bindingFlags, bool execMethods, int maxRecursiveDeep, int currentDeep, string prefix)
		{
			string phraseSTR = prefix + " | ";
			string str = prefix + " -> ";
			string newLine = Environment.NewLine;
			List<string> list = new List<string>();
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			string text4 = newLine + phraseSTR + ((currentDeep == 1) ? ("<----------------- START (All Members : " + bindingFlags.ToString() + ") ----------------------->") : "-----------") + newLine;
			string result = string.Concat(new string[]
			{
				text4,
				"<--- OBJECT TYPE: ",
				obj.GetType().ToString(),
				" --->",
				newLine
			});
			try
			{
				IEnumerable enumerable = obj as IEnumerable;
				bool flag = enumerable != null;
				if (flag)
				{
					return Methods.tryEnumerabledString(obj, "__ ");
				}
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
						bool flag2 = memberInfo.MemberType == MemberTypes.Field;
						if (flag2)
						{
							FieldInfo fieldInfo = (FieldInfo)memberInfo;
							object value = fieldInfo.GetValue(obj);
							bool flag3 = value == null;
							if (flag3)
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
								bool flag4 = !Methods.singleTypes.Contains(value.GetType().ToString());
								if (flag4)
								{
									string text9 = Methods.tryEnumerabledString(value, text8);
									bool flag5 = text9 != "";
									if (flag5)
									{
										text6 += text9;
									}
									else
									{
										bool flag6 = currentDeep < maxRecursiveDeep;
										if (flag6)
										{
											text6 += Methods.dump(value, bindingFlags, execMethods, maxRecursiveDeep, currentDeep + 1, text8);
										}
									}
								}
							}
						}
						else
						{
							bool flag7 = memberInfo.MemberType == MemberTypes.Property;
							if (flag7)
							{
								PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
								object value2 = propertyInfo.GetValue(obj, null);
								bool flag8 = value2 == null;
								if (flag8)
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
									bool flag9 = !Methods.singleTypes.Contains(value2.GetType().ToString());
									if (flag9)
									{
										string text10 = Methods.tryEnumerabledString(value2, text8);
										bool flag10 = text10 != "";
										if (flag10)
										{
											text6 += text10;
										}
										else
										{
											bool flag11 = currentDeep < maxRecursiveDeep;
											if (flag11)
											{
												text6 += Methods.dump(value2, bindingFlags, execMethods, maxRecursiveDeep, currentDeep + 1, text8);
											}
										}
									}
								}
							}
							else
							{
								bool flag12 = memberInfo.MemberType == MemberTypes.Method;
								if (flag12)
								{
									MethodInfo methodInfo = (MethodInfo)memberInfo;
									string text11 = methodInfo.ReturnType.ToString();
									text6 = string.Concat(new string[]
									{
										"   [",
										text11.Replace("System.", ""),
										"]  (",
										Methods.tryEnumerabledString(methodInfo.GetParameters(), ", "),
										")  <",
										Methods.AccessModifierType(methodInfo),
										">"
									});
									bool flag13 = !execMethods;
									if (flag13)
									{
										goto IL_568;
									}
									string[] source = new string[]
									{
										"System.Double",
										"System.Int32",
										"System.String",
										"System.Float",
										"System.Type"
									};
									bool flag14 = !source.Contains(text11);
									if (flag14)
									{
										goto IL_568;
									}
									try
									{
										object obj2 = methodInfo.Invoke(obj, null);
										bool flag15 = obj2 != null;
										if (flag15)
										{
											object obj3 = text6;
											text6 = string.Concat(new object[]
											{
												obj3,
												"========",
												obj2.ToString(),
												"   [",
												obj2.GetType(),
												"]"
											});
										}
										goto IL_568;
									}
									catch (Exception)
									{
										text6 += "--------------cant-Invoke";
										goto IL_568;
									}
								}
								bool flag16 = memberInfo.MemberType == MemberTypes.Constructor;
								if (flag16)
								{
									ConstructorInfo constructorInfo = (ConstructorInfo)memberInfo;
									ConstructorInfo left = constructorInfo;
									bool flag17 = left == null;
									if (flag17)
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
								else
								{
									bool flag18 = memberInfo.MemberType == MemberTypes.Event;
									if (flag18)
									{
										EventInfo eventInfo = (EventInfo)memberInfo;
										EventInfo left2 = eventInfo;
										text6 = ((left2 == null) ? "null" : ("ToString: " + eventInfo.ToString()));
									}
									else
									{
										text6 = "ToStringed: " + memberInfo.ToString();
									}
								}
							}
						}
						IL_568:;
					}
					catch
					{
						text6 += "--------------cant-get-value";
					}
					string str2 = Methods.pChars(text5) + text6;
					string text12 = phraseSTR + text7 + ":    " + str2;
					bool flag19 = !list.Contains(text12);
					if (flag19)
					{
						list.Add(text12);
						text12 += newLine;
					}
					else
					{
						text12 = "";
					}
					bool flag20 = !dictionary.ContainsKey(text7);
					if (flag20)
					{
						dictionary[text7] = new List<string>();
					}
					dictionary[text7].Add(text12);
				}
			}
			catch (Exception e)
			{
				result += Methods.ExceptionMessage(e, obj);
				Methods.m(result);
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
			result += text13;
			result = result + phraseSTR + ((currentDeep == 1) ? ("<----------------- END ---------------------->" + newLine + newLine) : "-----------") + newLine;
			return result;
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00009C48 File Offset: 0x00007E48
		public static string AccessModifierType(object objInfo)
		{
			bool flag = objInfo is FieldInfo;
			if (flag)
			{
				FieldInfo fieldInfo = objInfo as FieldInfo;
				bool isPrivate = fieldInfo.IsPrivate;
				if (isPrivate)
				{
					return "Private";
				}
				bool isFamily = fieldInfo.IsFamily;
				if (isFamily)
				{
					return "Protected";
				}
				bool isAssembly = fieldInfo.IsAssembly;
				if (isAssembly)
				{
					return "Internal";
				}
				bool isPublic = fieldInfo.IsPublic;
				if (isPublic)
				{
					return "Public";
				}
			}
			else
			{
				bool flag2 = objInfo is PropertyInfo;
				if (flag2)
				{
					PropertyInfo propertyInfo = objInfo as PropertyInfo;
					bool flag3 = propertyInfo.SetMethod == null;
					if (flag3)
					{
						return "GetOnly:" + Methods.AccessModifierType(propertyInfo.GetMethod);
					}
					bool flag4 = propertyInfo.GetMethod == null;
					if (flag4)
					{
						return "SetOnly:" + Methods.AccessModifierType(propertyInfo.SetMethod);
					}
					return Methods.AccessModifierType(propertyInfo.GetMethod) + " & " + Methods.AccessModifierType(propertyInfo.GetMethod);
				}
				else
				{
					bool flag5 = objInfo is MethodInfo;
					if (flag5)
					{
						MethodInfo methodInfo = objInfo as MethodInfo;
						bool isPrivate2 = methodInfo.IsPrivate;
						if (isPrivate2)
						{
							return "Private";
						}
						bool isFamily2 = methodInfo.IsFamily;
						if (isFamily2)
						{
							return "Protected";
						}
						bool isAssembly2 = methodInfo.IsAssembly;
						if (isAssembly2)
						{
							return "Internal";
						}
						bool isPublic2 = methodInfo.IsPublic;
						if (isPublic2)
						{
							return "Public";
						}
					}
				}
			}
			return "Did not find access modifier";
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00009DE4 File Offset: 0x00007FE4
		public static string tryEnumerabledString(object obj)
		{
			return Methods.tryEnumerabledString(obj, "");
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00009E04 File Offset: 0x00008004
		public static string tryEnumerabledString(object obj, string prefix_)
		{
			string text = "";
			try
			{
				IEnumerable enumerable = obj as IEnumerable;
				bool flag2 = enumerable != null;
				if (flag2)
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

		// Token: 0x06000129 RID: 297 RVA: 0x00009ED4 File Offset: 0x000080D4
		public static object callMethod(object o, string methodName, params object[] args)
		{
			MethodInfo method = o.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
			bool flag = method != null;
			object result;
			if (flag)
			{
				result = method.Invoke(o, args);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00009F10 File Offset: 0x00008110
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

		// Token: 0x0600012B RID: 299 RVA: 0x00009F88 File Offset: 0x00008188
		public static object propertyGet2(object obj_, string propName)
		{
			PropertyInfo property = obj_.GetType().GetProperty(propName);
			bool flag = property != null;
			object result;
			if (flag)
			{
				result = property.GetValue(obj_, null);
			}
			else
			{
				FieldInfo field = obj_.GetType().GetField(propName);
				bool flag2 = field != null;
				if (flag2)
				{
					result = field.GetValue(obj_);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00009FE4 File Offset: 0x000081E4
		public static T propertyGet<T>(object obj_, string propName)
		{
			T result;
			try
			{
				PropertyInfo property = obj_.GetType().GetProperty(propName);
				bool flag = property != null;
				if (flag)
				{
					bool flag2 = typeof(T) == typeof(string);
					if (flag2)
					{
						result = (T)((object)Convert.ChangeType((string)property.GetValue(obj_, null), typeof(T), CultureInfo.InvariantCulture));
					}
					else
					{
						bool flag3 = typeof(T) == typeof(bool);
						if (flag3)
						{
							result = (T)((object)Convert.ChangeType((bool)property.GetValue(obj_, null), typeof(T), CultureInfo.InvariantCulture));
						}
						else
						{
							bool flag4 = typeof(T) == typeof(Dictionary<string, string>);
							if (flag4)
							{
								result = (T)((object)Convert.ChangeType((Dictionary<string, string>)property.GetValue(obj_, null), typeof(T), CultureInfo.InvariantCulture));
							}
							else
							{
								result = (T)((object)Convert.ChangeType(property.GetValue(obj_, null), typeof(T), CultureInfo.InvariantCulture));
							}
						}
					}
				}
				else
				{
					FieldInfo field = obj_.GetType().GetField(propName);
					bool flag5 = field != null;
					if (flag5)
					{
						bool flag6 = typeof(T) == typeof(string);
						if (flag6)
						{
							result = (T)((object)Convert.ChangeType((string)field.GetValue(obj_), typeof(T), CultureInfo.InvariantCulture));
						}
						else
						{
							bool flag7 = typeof(T) == typeof(bool);
							if (flag7)
							{
								result = (T)((object)Convert.ChangeType((bool)field.GetValue(obj_), typeof(T), CultureInfo.InvariantCulture));
							}
							else
							{
								bool flag8 = typeof(T) == typeof(Dictionary<string, string>);
								if (flag8)
								{
									result = (T)((object)Convert.ChangeType((Dictionary<string, string>)field.GetValue(obj_), typeof(T), CultureInfo.InvariantCulture));
								}
								else
								{
									result = (T)((object)Convert.ChangeType(field.GetValue(obj_), typeof(T), CultureInfo.InvariantCulture));
								}
							}
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

		// Token: 0x0600012D RID: 301 RVA: 0x0000A2B4 File Offset: 0x000084B4
		public static bool propertySet(object obj_, string propName, object value)
		{
			value.ToString();
			PropertyInfo propertyInfo = obj_.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault((PropertyInfo x) => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
			bool flag = propertyInfo != null;
			if (flag)
			{
				bool flag2 = value is bool && ((bool)value || !(bool)value);
				if (flag2)
				{
					propertyInfo.SetValue(obj_, Convert.ToBoolean(value), null);
					return true;
				}
				bool flag3 = Methods.isInt(value);
				if (flag3)
				{
					propertyInfo.SetValue(obj_, Convert.ToInt32(value), null);
					return true;
				}
				bool flag4 = Methods.isDouble(value);
				if (flag4)
				{
					propertyInfo.SetValue(obj_, Convert.ToDouble(value), null);
					return true;
				}
				propertyInfo.SetValue(obj_, value);
			}
			FieldInfo fieldInfo = obj_.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault((FieldInfo x) => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
			bool flag5 = propertyInfo != null;
			if (flag5)
			{
				bool flag6 = value is bool && ((bool)value || !(bool)value);
				if (flag6)
				{
					fieldInfo.SetValue(obj_, Convert.ToBoolean(value));
					return true;
				}
				bool flag7 = Methods.isInt(value);
				if (flag7)
				{
					fieldInfo.SetValue(obj_, Convert.ToInt32(value));
					return true;
				}
				bool flag8 = Methods.isDouble(value);
				if (flag8)
				{
					fieldInfo.SetValue(obj_, Convert.ToDouble(value));
					return true;
				}
				fieldInfo.SetValue(obj_, value);
			}
			return false;
		}

		// Token: 0x0600012E RID: 302 RVA: 0x0000A46C File Offset: 0x0000866C
		public static bool propertyExists(object obj_, string propName)
		{
			bool result;
			try
			{
				PropertyInfo left = obj_.GetType().GetProperties(Methods.AllBindingFlags).FirstOrDefault((PropertyInfo x) => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
				bool flag = left != null;
				if (flag)
				{
					result = true;
				}
				else
				{
					FieldInfo left2 = obj_.GetType().GetFields(Methods.AllBindingFlags).FirstOrDefault((FieldInfo x) => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
					bool flag2 = left2 != null;
					result = flag2;
				}
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x0000A51C File Offset: 0x0000871C
		public static bool propertySet_static(object obj_, string propName, string value)
		{
			bool result;
			try
			{
				PropertyInfo propertyInfo = obj_.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault((PropertyInfo x) => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
				bool flag = propertyInfo != null;
				if (flag)
				{
					bool flag2 = value == "true" || value == "false";
					if (flag2)
					{
						propertyInfo.SetValue(obj_, Convert.ToBoolean(value), null);
						return true;
					}
					bool flag3 = Methods.isInt(value);
					if (flag3)
					{
						propertyInfo.SetValue(obj_, Convert.ToInt32(value), null);
						return true;
					}
					bool flag4 = Methods.isDouble(value);
					if (flag4)
					{
						propertyInfo.SetValue(obj_, Convert.ToDouble(value), null);
						return true;
					}
				}
				FieldInfo fieldInfo = obj_.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault((FieldInfo x) => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
				bool flag5 = propertyInfo != null;
				if (flag5)
				{
					bool flag6 = value == "true" || value == "false";
					if (flag6)
					{
						fieldInfo.SetValue(obj_, Convert.ToBoolean(value));
						return true;
					}
					bool flag7 = Methods.isInt(value);
					if (flag7)
					{
						fieldInfo.SetValue(obj_, Convert.ToInt32(value));
						return true;
					}
					bool flag8 = Methods.isDouble(value);
					if (flag8)
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

		// Token: 0x06000130 RID: 304 RVA: 0x0000A6E0 File Offset: 0x000088E0
		public static bool propertyHideOrShow(object obj_, string propertyName, bool Show_or_Hide)
		{
			BrowsableAttribute browsableAttribute = TypeDescriptor.GetProperties(obj_.GetType())[propertyName].Attributes[typeof(BrowsableAttribute)] as BrowsableAttribute;
			FieldInfo field = browsableAttribute.GetType().GetField("Browsable", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);
			bool flag = field != null;
			bool result;
			if (flag)
			{
				field.SetValue(browsableAttribute, Show_or_Hide);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000131 RID: 305 RVA: 0x0000A750 File Offset: 0x00008950
		public static Type propertyType(object obj_, string propName)
		{
			PropertyInfo property = obj_.GetType().GetProperty(propName);
			bool flag = property != null;
			Type result;
			if (flag)
			{
				result = property.GetType();
			}
			else
			{
				FieldInfo field = obj_.GetType().GetField(propName);
				bool flag2 = field != null;
				if (flag2)
				{
					result = field.GetType();
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x06000132 RID: 306 RVA: 0x0000A7AC File Offset: 0x000089AC
		public static string PrintProperties(object obj)
		{
			return Methods.PrintProperties(obj, 0);
		}

		// Token: 0x06000133 RID: 307 RVA: 0x0000A7C8 File Offset: 0x000089C8
		public static string PrintProperties(object obj, int indent)
		{
			string result;
			try
			{
				bool flag = obj == null;
				if (flag)
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
						bool flag2 = indexParameters.Count<ParameterInfo>() > 0 && !obj.ToString().Contains("+");
						if (flag2)
						{
							object value = propertyInfo.GetValue(obj, null);
							bool flag3 = propertyInfo.PropertyType.Assembly == type.Assembly && !propertyInfo.PropertyType.IsEnum;
							if (flag3)
							{
								text = text + text2 + propertyInfo.Name + ":";
								text += Methods.PrintProperties(value, indent + 2);
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

		// Token: 0x06000134 RID: 308 RVA: 0x0000A934 File Offset: 0x00008B34
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
				bool flag = fields.Length != 0;
				if (flag)
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
				bool flag2 = properties.Length != 0;
				if (flag2)
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

		// Token: 0x06000135 RID: 309 RVA: 0x0000AAB4 File Offset: 0x00008CB4
		public static double NetFrameworkVersion()
		{
			double result;
			try
			{
				string version = (from x in Assembly.GetExecutingAssembly().GetReferencedAssemblies()
				where x.Name == "System.Core"
				select x).First<AssemblyName>().Version.ToString();
				result = Convert.ToDouble(version.Substring(0, version.IndexOf(".", 2)));
			}
			catch (Exception)
			{
				result = 0.0;
			}
			return result;
		}

		// Token: 0x06000136 RID: 310 RVA: 0x0000AB44 File Offset: 0x00008D44
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

		// Token: 0x06000137 RID: 311 RVA: 0x0000AB98 File Offset: 0x00008D98
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

		// Token: 0x06000138 RID: 312 RVA: 0x0000ABE0 File Offset: 0x00008DE0
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

		// Token: 0x06000139 RID: 313 RVA: 0x0000AC20 File Offset: 0x00008E20
		public static int decimalsAmount(double content)
		{
			string num = Methods.double_to_decimal(content).ToString();
			return num.Length - num.IndexOf(".") - 1;
		}

		// Token: 0x0600013A RID: 314 RVA: 0x0000AC58 File Offset: 0x00008E58
		public static decimal double_to_decimal(double num)
		{
			return Methods.double_to_decimal_static(num);
		}

		// Token: 0x0600013B RID: 315 RVA: 0x0000AC70 File Offset: 0x00008E70
		public static decimal double_to_decimal_static(double num)
		{
			return decimal.Parse(num.ToString(), NumberStyles.Float);
		}

		// Token: 0x0600013C RID: 316 RVA: 0x0000AC94 File Offset: 0x00008E94
		public static void DownloadFile(string url, string location)
		{
			using (WebClient client = new WebClient())
			{
				try
				{
					client.DownloadFile(new Uri(url), location);
				}
				catch (Exception e)
				{
					MessageBox.Show(string.Concat(new string[]
					{
						"Cant download ",
						url,
						"( ",
						e.Message,
						")"
					}));
				}
			}
		}

		// Token: 0x0600013D RID: 317 RVA: 0x0000AD20 File Offset: 0x00008F20
		public static string TempFile(string url)
		{
			Uri uri = new Uri(url);
			string tmp_path = Methods.convertBackSlashes(Path.GetTempPath() + "\\" + url.Replace(uri.Scheme, "").Replace("://", "").Replace("//", ""));
			bool flag = !File.Exists(tmp_path);
			if (flag)
			{
				string dir = Path.GetDirectoryName(tmp_path);
				bool flag2 = !Directory.Exists(dir);
				if (flag2)
				{
					Directory.CreateDirectory(dir);
				}
				Methods.DownloadFile(url, tmp_path);
			}
			return tmp_path;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00002530 File Offset: 0x00000730
		public static void errorlog(string message)
		{
		}

		// Token: 0x0600013F RID: 319 RVA: 0x0000ADB8 File Offset: 0x00008FB8
		public static int digitsPrecision(double tickSize)
		{
			int decimals = 0;
			bool flag = tickSize == 1.0;
			if (flag)
			{
				decimals = 0;
			}
			else
			{
				bool flag2 = tickSize == 0.5;
				if (flag2)
				{
					decimals = 1;
				}
				else
				{
					bool flag3 = tickSize == 0.25;
					if (flag3)
					{
						decimals = 1;
					}
					else
					{
						bool flag4 = tickSize == 0.1;
						if (flag4)
						{
							decimals = 1;
						}
						else
						{
							bool flag5 = tickSize == 0.01;
							if (flag5)
							{
								decimals = 2;
							}
							else
							{
								bool flag6 = tickSize == 0.001;
								if (flag6)
								{
									decimals = 3;
								}
								else
								{
									bool flag7 = tickSize == 0.005;
									if (flag7)
									{
										decimals = 3;
									}
									else
									{
										bool flag8 = tickSize == 0.025;
										if (flag8)
										{
											decimals = 3;
										}
										else
										{
											bool flag9 = tickSize == 0.03125;
											if (flag9)
											{
												decimals = 3;
											}
											else
											{
												bool flag10 = tickSize == 0.0001;
												if (flag10)
												{
													decimals = 4;
												}
												else
												{
													bool flag11 = tickSize == 1E-05;
													if (flag11)
													{
														decimals = 5;
													}
													else
													{
														bool flag12 = tickSize == 5E-05;
														if (flag12)
														{
															decimals = 5;
														}
														else
														{
															bool flag13 = tickSize == 1E-06;
															if (flag13)
															{
																decimals = 6;
															}
															else
															{
																bool flag14 = tickSize == 1E-07;
																if (flag14)
																{
																	decimals = 7;
																}
																else
																{
																	bool flag15 = tickSize == 5E-07;
																	if (flag15)
																	{
																		decimals = 7;
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return decimals;
		}

		// Token: 0x06000140 RID: 320 RVA: 0x0000AF4C File Offset: 0x0000914C
		public static int rondTicksToDecimals(double content)
		{
			int response = -9999;
			bool flag = content == 1.0;
			if (flag)
			{
				response = 1;
			}
			else
			{
				bool flag2 = content == 0.5;
				if (flag2)
				{
					response = 2;
				}
				else
				{
					bool flag3 = content == 0.25;
					if (flag3)
					{
						response = 2;
					}
					else
					{
						bool flag4 = content == 0.1;
						if (flag4)
						{
							response = 2;
						}
						else
						{
							bool flag5 = content == 0.01;
							if (flag5)
							{
								response = 3;
							}
							else
							{
								bool flag6 = content == 0.001;
								if (flag6)
								{
									response = 3;
								}
								else
								{
									bool flag7 = content == 0.005;
									if (flag7)
									{
										response = 3;
									}
									else
									{
										bool flag8 = content == 0.025;
										if (flag8)
										{
											response = 3;
										}
										else
										{
											bool flag9 = content == 0.03125;
											if (flag9)
											{
												response = 5;
											}
											else
											{
												bool flag10 = content == 0.0001;
												if (flag10)
												{
													response = 4;
												}
												else
												{
													bool flag11 = content == 1E-05;
													if (flag11)
													{
														response = 5;
													}
													else
													{
														bool flag12 = content == 5E-05;
														if (flag12)
														{
															response = 5;
														}
														else
														{
															bool flag13 = content == 1E-06;
															if (flag13)
															{
																response = 6;
															}
															else
															{
																bool flag14 = content == 5E-06;
																if (flag14)
																{
																	response = 6;
																}
																else
																{
																	bool flag15 = content == 1E-07;
																	if (flag15)
																	{
																		response = 7;
																	}
																	else
																	{
																		bool flag16 = content == 5E-07;
																		if (flag16)
																		{
																			response = 7;
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return response;
		}

		// Token: 0x06000141 RID: 321 RVA: 0x0000B100 File Offset: 0x00009300
		public static int AfterPointNumberOfDigits(double tickSize)
		{
			double t = tickSize;
			int i = 0;
			while (t != Math.Round(t, 0))
			{
				t *= 10.0;
				i++;
			}
			return i;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000B13C File Offset: 0x0000933C
		public static string roundTo(double num, double tickSize)
		{
			return num.ToString("N" + Methods.digitsAfterDot(tickSize).ToString());
		}

		// Token: 0x06000143 RID: 323 RVA: 0x0000B170 File Offset: 0x00009370
		public static bool playSound(string path)
		{
			bool result;
			try
			{
				bool flag = !path.Contains(".");
				if (flag)
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

		// Token: 0x06000144 RID: 324 RVA: 0x0000B1D4 File Offset: 0x000093D4
		public static void PopupForm_(string message)
		{
			Form form = new Form();
			form.Width = 250;
			form.Height = 130;
			form.Text = "Alert";
			form.TopMost = true;
			form.TopLevel = true;
			form.Opacity = 0.95;
			form.StartPosition = FormStartPosition.CenterScreen;
			TextBox textarea = new TextBox();
			form.Controls.Add(textarea);
			textarea.Multiline = true;
			textarea.Width = textarea.Parent.Width;
			textarea.Height = textarea.Parent.Height;
			textarea.Text = message;
			form.Show();
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00002530 File Offset: 0x00000730
		public static void destroyForm(bool dispose)
		{
		}

		// Token: 0x06000146 RID: 326 RVA: 0x0000B282 File Offset: 0x00009482
		public static void enableExceptions()
		{
			Methods.enableExceptions(false);
		}

		// Token: 0x06000147 RID: 327 RVA: 0x0000B28C File Offset: 0x0000948C
		public static void enableExceptions(bool show = false)
		{
			AppDomain.CurrentDomain.FirstChanceException += delegate(object sender, FirstChanceExceptionEventArgs eventArgs)
			{
				StackTrace stackTrace = new StackTrace();
				MethodBase method = stackTrace.GetFrame(1).GetMethod();
				string text = Methods.ExceptionMessage(eventArgs.Exception, method, "");
				bool flag = show || Methods.IsDeveloperMachine();
				if (flag)
				{
					Methods.m(text);
				}
				else
				{
					Methods.log(text);
				}
			};
		}

		// Token: 0x06000148 RID: 328 RVA: 0x0000B2C0 File Offset: 0x000094C0
		public static void log(string text)
		{
			string location = Path.GetTempPath() + "_default_logs.txt";
			Methods.log(text, location);
		}

		// Token: 0x06000149 RID: 329 RVA: 0x0000B2E8 File Offset: 0x000094E8
		public static void log(string text, string location)
		{
			text = string.Concat(new string[]
			{
				Methods.nl,
				"______________   ",
				DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"),
				"   ______________",
				Methods.nl,
				text
			});
			bool flag = new FileInfo(location).Length > 2000000L;
			if (flag)
			{
				File.Delete(location);
			}
			bool flag2 = !File.Exists(location);
			if (flag2)
			{
				File.WriteAllText(location, "");
			}
			File.AppendAllText(location, text);
		}

		// Token: 0x0600014A RID: 330 RVA: 0x0000B37F File Offset: 0x0000957F
		[STAThread]
		public static void catchAllExceptions()
		{
			AppDomain.CurrentDomain.UnhandledException += Methods.HandleUnhandledException;
		}

		// Token: 0x0600014B RID: 331 RVA: 0x0000B39C File Offset: 0x0000959C
		public static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			bool flag = !Methods.inCatchAllLoop;
			if (flag)
			{
				Methods.inCatchAllLoop = true;
				Methods.m(sender);
				Methods.inCatchAllLoop = false;
			}
		}

		// Token: 0x0600014C RID: 332 RVA: 0x0000B3CC File Offset: 0x000095CC
		public static void logError(object obj_)
		{
			FileOperations.Write(Methods.errorLogFile, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine + obj_.ToString() + Environment.NewLine);
		}

		// Token: 0x0600014D RID: 333 RVA: 0x0000B40C File Offset: 0x0000960C
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

		// Token: 0x0600014E RID: 334 RVA: 0x0000B460 File Offset: 0x00009660
		public static string ExceptionForDeveloper(Exception e, object targetObj)
		{
			string text = "";
			string result;
			try
			{
				text += Methods.ExceptionMessage(e, targetObj);
				bool flag = Methods.isDeveloperMode("Exceptions");
				if (flag)
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

		// Token: 0x0600014F RID: 335 RVA: 0x0000B528 File Offset: 0x00009728
		public static string ExceptionMessage(Exception e, object targetObj)
		{
			return Methods.ExceptionMessage(e, targetObj, "");
		}

		// Token: 0x06000150 RID: 336 RVA: 0x0000B548 File Offset: 0x00009748
		public static string ExceptionMessage(Exception e, object targetObj, bool echo_or_return)
		{
			string txt = Methods.ExceptionMessage(e, targetObj, "");
			string result;
			if (echo_or_return)
			{
				Methods.m(txt);
				result = "";
			}
			else
			{
				result = txt;
			}
			return result;
		}

		// Token: 0x06000151 RID: 337 RVA: 0x0000B580 File Offset: 0x00009780
		public static string ExceptionMessage(Exception e, object obj_, string customMessage)
		{
			string text = "";
			string text2 = Methods.nl_ + "_________________" + Methods.nl_;
			StackFrame frame = new StackTrace(e, true).GetFrame(0);
			int fileLineNumber = frame.GetFileLineNumber();
			int fileColumnNumber = frame.GetFileColumnNumber();
			string fileName = frame.GetFileName();
			text = text + "======================" + Methods.nl_;
			bool flag = e.Source != null;
			if (flag)
			{
				text = text + "• Source\t\t\t\t: " + e.Source.ToString() + text2;
			}
			bool flag2 = obj_ != null && obj_.GetType() != null;
			if (flag2)
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
			bool flag3 = fileLineNumber != 0;
			if (flag3)
			{
				try
				{
					bool flag4 = fileName.EndsWith(".cs");
					if (flag4)
					{
						string text4 = File.ReadLines(fileName).Skip(fileLineNumber - 1).Take(1).First<string>();
						bool flag5 = string.IsNullOrEmpty(text4);
						if (flag5)
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
			bool flag6 = e.InnerException != null;
			if (flag6)
			{
				text = text + "• InnerException\t\t: " + e.InnerException.Message + text2;
			}
			text = text + "• User_message\t\t: " + customMessage + text2;
			string text5 = text;
			text = string.Concat(new string[]
			{
				text5,
				"• StackTrace\t\t\t: ",
				Methods.nl_,
				" ----- ",
				e.StackTrace,
				text2
			});
			text = text + "======================" + Methods.nl_;
			return text;
		}

		// Token: 0x06000152 RID: 338 RVA: 0x0000B850 File Offset: 0x00009A50
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

		// Token: 0x06000153 RID: 339 RVA: 0x0000B8DC File Offset: 0x00009ADC
		public static string appName()
		{
			string location = Assembly.GetEntryAssembly().Location;
			return Path.GetFileNameWithoutExtension(location);
		}

		// Token: 0x06000154 RID: 340 RVA: 0x0000B900 File Offset: 0x00009B00
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

		// Token: 0x0400004D RID: 77
		public static bool initialized;

		// Token: 0x0400004E RID: 78
		public static string AppNameRegex = "(XYZXYZ)";

		// Token: 0x0400004F RID: 79
		public static string developerMachineString = "puvox_development_machine";

		// Token: 0x04000050 RID: 80
		private Dictionary<string, string> symbols = new Dictionary<string, string>
		{
			{
				"checkmark",
				"?"
			},
			{
				"checkmark2",
				"\ud83d\uddf9"
			}
		};

		// Token: 0x04000051 RID: 81
		private static bool demoPeriodGone_shown;

		// Token: 0x04000052 RID: 82
		private static bool demoPeriodGone_disallow;

		// Token: 0x04000053 RID: 83
		private static string ProgramName_ = "";

		// Token: 0x04000054 RID: 84
		public static Dictionary<string, object> GlobalVariables = new Dictionary<string, object>();

		// Token: 0x04000055 RID: 85
		public static System.Windows.Forms.Timer BlinkTimer;

		// Token: 0x04000056 RID: 86
		public static Color initialColor;

		// Token: 0x04000057 RID: 87
		public static RegistryHive chosenRegHive = RegistryHive.CurrentUser;

		// Token: 0x04000058 RID: 88
		public static RegistryKey chosenRegKey = Registry.CurrentUser;

		// Token: 0x04000059 RID: 89
		public static bool is64BitOS = Environment.Is64BitOperatingSystem;

		// Token: 0x0400005A RID: 90
		public static Dictionary<string, string> myregs = new Dictionary<string, string>();

		// Token: 0x0400005B RID: 91
		private static object fileCheckObj = new object();

		// Token: 0x0400005C RID: 92
		public static string errorLogFile = Environment.GetEnvironmentVariable("tmp") + "\\_errorlogs_c#.log";

		// Token: 0x0400005D RID: 93
		public static string optionsPrefix = "formoption_";

		// Token: 0x0400005E RID: 94
		private HashSet<System.Threading.Timer> ExecuteAfter_timers = new HashSet<System.Threading.Timer>();

		// Token: 0x0400005F RID: 95
		public static string gsApkPath = "";

		// Token: 0x04000060 RID: 96
		public static Dictionary<string, Dictionary<string, string>> transl_texts = new Dictionary<string, Dictionary<string, string>>();

		// Token: 0x04000061 RID: 97
		public static string translationsBaseReg = "Software\\Puvox\\TranslatedTexts\\";

		// Token: 0x04000062 RID: 98
		private Dictionary<string, string> gtranslate_dict;

		// Token: 0x04000063 RID: 99
		public static string defaultSound = "Speech On";

		// Token: 0x04000064 RID: 100
		public static string tmpFilePath = Environment.GetEnvironmentVariable("tmp") + "\\";

		// Token: 0x04000065 RID: 101
		public static string pathDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

		// Token: 0x04000066 RID: 102
		public static string nl = Environment.NewLine;

		// Token: 0x04000067 RID: 103
		public static string[] singleTypes = new string[]
		{
			"System.Double",
			"System.Int32",
			"System.String",
			"System.Float",
			"System.Boolean"
		};

		// Token: 0x04000068 RID: 104
		internal static BindingFlags AllBindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty | BindingFlags.ExactBinding | BindingFlags.SuppressChangeType | BindingFlags.OptionalParamBinding | BindingFlags.IgnoreReturn;

		// Token: 0x04000069 RID: 105
		internal static BindingFlags AllBindingFlags_noinherit = BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty | BindingFlags.ExactBinding | BindingFlags.SuppressChangeType | BindingFlags.OptionalParamBinding | BindingFlags.IgnoreReturn;

		// Token: 0x0400006A RID: 106
		public static bool inCatchAllLoop = false;

		// Token: 0x0400006B RID: 107
		public static string nl_ = Environment.NewLine;

		// Token: 0x0400006C RID: 108
		public static string tmpDir = Environment.GetEnvironmentVariable("tmp") + "\\";

		// Token: 0x02000039 RID: 57
		// (Invoke) Token: 0x060001EB RID: 491
		private delegate void SetTextCallback(Form f, Control ctrl, string text);

		// Token: 0x0200003A RID: 58
		// (Invoke) Token: 0x060001EF RID: 495
		private delegate bool Control_SetDelegate(Control ctrl, string what, object value);

		// Token: 0x0200003B RID: 59
		public class TimerInterrupter
		{
			// Token: 0x060001F2 RID: 498 RVA: 0x0000EF00 File Offset: 0x0000D100
			public TimerInterrupter(System.Timers.Timer timer)
			{
				bool flag = timer == null;
				if (flag)
				{
					throw new ArgumentNullException("timer");
				}
				this._timer = timer;
			}

			// Token: 0x060001F3 RID: 499 RVA: 0x0000EF30 File Offset: 0x0000D130
			public void Stop()
			{
				this._timer.Stop();
			}

			// Token: 0x040000AF RID: 175
			private readonly System.Timers.Timer _timer;
		}

		// Token: 0x0200003C RID: 60
		public class WebDownload : WebClient
		{
			// Token: 0x17000028 RID: 40
			// (get) Token: 0x060001F4 RID: 500 RVA: 0x0000EF3F File Offset: 0x0000D13F
			// (set) Token: 0x060001F5 RID: 501 RVA: 0x0000EF47 File Offset: 0x0000D147
			public int Timeout { get; set; }

			// Token: 0x060001F6 RID: 502 RVA: 0x0000EF50 File Offset: 0x0000D150
			public WebDownload() : this(60000)
			{
			}

			// Token: 0x060001F7 RID: 503 RVA: 0x0000EF5F File Offset: 0x0000D15F
			public WebDownload(int timeout)
			{
				this.Timeout = timeout;
			}

			// Token: 0x060001F8 RID: 504 RVA: 0x0000EF74 File Offset: 0x0000D174
			protected override WebRequest GetWebRequest(Uri address)
			{
				WebRequest result;
				try
				{
					WebRequest webRequest = base.GetWebRequest(address);
					bool flag = webRequest != null;
					if (flag)
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

		// Token: 0x0200003D RID: 61
		public static class JsonMaker
		{
			// Token: 0x060001F9 RID: 505 RVA: 0x0000EFC4 File Offset: 0x0000D1C4
			public static Dictionary<string, object> ParseJSON(string json)
			{
				int end;
				return Methods.JsonMaker.ParseJSON(json, 0, out end);
			}

			// Token: 0x060001FA RID: 506 RVA: 0x0000EFE4 File Offset: 0x0000D1E4
			private static Dictionary<string, object> ParseJSON(string json, int start, out int end)
			{
				Dictionary<string, object> dict = new Dictionary<string, object>();
				end = 0;
				try
				{
					bool escbegin = false;
					bool escend = false;
					bool inquotes = false;
					string key = null;
					StringBuilder sb = new StringBuilder();
					List<object> arraylist = null;
					Regex regex = new Regex("\\\\u([0-9a-z]{4})", RegexOptions.IgnoreCase);
					int autoKey = 0;
					bool inSingleQuotes = false;
					bool inDoubleQuotes = false;
					int i = start;
					while (i < json.Length)
					{
						char c = json[i];
						bool flag = c == '\\';
						if (flag)
						{
							escbegin = !escbegin;
						}
						bool flag2 = !escbegin;
						if (!flag2)
						{
							goto IL_32D;
						}
						bool flag3 = c == '"' && !inSingleQuotes;
						if (flag3)
						{
							inDoubleQuotes = !inDoubleQuotes;
							inquotes = !inquotes;
							bool flag4 = !inquotes && arraylist != null;
							if (flag4)
							{
								arraylist.Add(Methods.JsonMaker.DecodeString(regex, sb.ToString()));
								sb.Length = 0;
							}
						}
						else
						{
							bool flag5 = c == '\'' && !inDoubleQuotes;
							if (!flag5)
							{
								bool flag6 = !inquotes;
								if (flag6)
								{
									char c2 = c;
									char c3 = c2;
									if (c3 <= '[')
									{
										if (c3 == ',')
										{
											bool flag7 = arraylist == null && key != null;
											if (flag7)
											{
												dict.Add(key.Trim(), Methods.JsonMaker.DecodeString(regex, sb.ToString().Trim()));
												key = null;
												sb.Length = 0;
											}
											bool flag8 = arraylist != null && sb.Length > 0;
											if (flag8)
											{
												arraylist.Add(sb.ToString());
												sb.Length = 0;
											}
											goto IL_349;
										}
										if (c3 == ':')
										{
											key = Methods.JsonMaker.DecodeString(regex, sb.ToString());
											sb.Length = 0;
											goto IL_349;
										}
										if (c3 == '[')
										{
											arraylist = new List<object>();
											goto IL_349;
										}
									}
									else
									{
										if (c3 == ']')
										{
											bool flag9 = key == null;
											if (flag9)
											{
												key = "array" + autoKey.ToString();
												autoKey++;
											}
											bool flag10 = arraylist != null && sb.Length > 0;
											if (flag10)
											{
												arraylist.Add(sb.ToString());
												sb.Length = 0;
											}
											dict.Add(key.Trim(), arraylist);
											arraylist = null;
											key = null;
											goto IL_349;
										}
										if (c3 == '{')
										{
											bool flag11 = i != start;
											if (flag11)
											{
												int cend;
												Dictionary<string, object> child = Methods.JsonMaker.ParseJSON(json, i, out cend);
												bool flag12 = arraylist != null;
												if (flag12)
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
											goto IL_349;
										}
										if (c3 == '}')
										{
											end = i;
											bool flag13 = key != null;
											if (flag13)
											{
												bool flag14 = arraylist != null;
												if (flag14)
												{
													dict.Add(key.Trim(), arraylist);
												}
												else
												{
													dict.Add(key.Trim(), Methods.JsonMaker.DecodeString(regex, sb.ToString().Trim()));
												}
											}
											return dict;
										}
									}
								}
								goto IL_32D;
							}
							inSingleQuotes = !inSingleQuotes;
							inquotes = !inquotes;
							bool flag15 = !inquotes && arraylist != null;
							if (flag15)
							{
								arraylist.Add(Methods.JsonMaker.DecodeString(regex, sb.ToString()));
								sb.Length = 0;
							}
						}
						IL_349:
						i++;
						continue;
						IL_32D:
						sb.Append(c);
						bool flag16 = escend;
						if (flag16)
						{
							escbegin = false;
						}
						escend = escbegin;
						goto IL_349;
					}
					end = json.Length - 1;
				}
				catch (Exception e)
				{
					MessageBox.Show("JsonMarkerParser : " + e.ToString());
				}
				return dict;
			}

			// Token: 0x060001FB RID: 507 RVA: 0x0000F3A0 File Offset: 0x0000D5A0
			private static string DecodeString(Regex regex, string str)
			{
				return Regex.Unescape(regex.Replace(str, (Match match) => char.ConvertFromUtf32(int.Parse(match.Groups[1].Value, NumberStyles.HexNumber))));
			}
		}

		// Token: 0x0200003E RID: 62
		public static class JsonHelper
		{
			// Token: 0x060001FC RID: 508 RVA: 0x0000F3E0 File Offset: 0x0000D5E0
			public static Dictionary<string, object> DeserializeJson(string json)
			{
				int end;
				return Methods.JsonHelper.DeserializeJson(json, 0, out end);
			}

			// Token: 0x060001FD RID: 509 RVA: 0x0000F3FC File Offset: 0x0000D5FC
			private static Dictionary<string, object> DeserializeJson(string json, int start, out int end)
			{
				bool flag = !Methods.JsonHelper.IsJson(json);
				Dictionary<string, object> result;
				if (flag)
				{
					end = 0;
					result = null;
				}
				else
				{
					Dictionary<string, object> dict = new Dictionary<string, object>();
					bool escbegin = false;
					bool escend = false;
					bool inquotes = false;
					string key = null;
					StringBuilder sb = new StringBuilder();
					List<object> arraylist = null;
					Regex regex = new Regex("\\\\u([0-9a-z]{4})", RegexOptions.IgnoreCase);
					int autoKey = 0;
					bool inSingleQuotes = false;
					bool inDoubleQuotes = false;
					int i = start;
					while (i < json.Length)
					{
						char c = json[i];
						bool flag2 = c == '\\';
						if (flag2)
						{
							escbegin = !escbegin;
						}
						bool flag3 = !escbegin;
						if (!flag3)
						{
							goto IL_344;
						}
						bool flag4 = c == '"' && !inSingleQuotes;
						if (flag4)
						{
							inDoubleQuotes = !inDoubleQuotes;
							inquotes = !inquotes;
							bool flag5 = !inquotes && arraylist != null;
							if (flag5)
							{
								arraylist.Add(Methods.JsonHelper.DecodeString(regex, sb.ToString()));
								sb.Length = 0;
							}
						}
						else
						{
							bool flag6 = c == '\'' && !inDoubleQuotes;
							if (!flag6)
							{
								bool flag7 = !inquotes;
								if (flag7)
								{
									char c2 = c;
									char c3 = c2;
									if (c3 <= '[')
									{
										if (c3 == ',')
										{
											bool flag8 = arraylist == null && key != null;
											if (flag8)
											{
												dict.Add(key.Trim(), Methods.JsonHelper.DecodeString(regex, sb.ToString().Trim()));
												key = null;
												sb.Length = 0;
											}
											bool flag9 = arraylist != null && sb.Length > 0;
											if (flag9)
											{
												arraylist.Add(sb.ToString());
												sb.Length = 0;
											}
											goto IL_35A;
										}
										if (c3 == ':')
										{
											key = Methods.JsonHelper.DecodeString(regex, sb.ToString());
											sb.Length = 0;
											goto IL_35A;
										}
										if (c3 == '[')
										{
											arraylist = new List<object>();
											goto IL_35A;
										}
									}
									else
									{
										if (c3 == ']')
										{
											bool flag10 = key == null;
											if (flag10)
											{
												key = "array" + autoKey.ToString();
												autoKey++;
											}
											bool flag11 = arraylist != null && sb.Length > 0;
											if (flag11)
											{
												arraylist.Add(sb.ToString());
												sb.Length = 0;
											}
											dict.Add(key.Trim(), arraylist);
											arraylist = null;
											key = null;
											goto IL_35A;
										}
										if (c3 == '{')
										{
											bool flag12 = i != start;
											if (flag12)
											{
												int cend;
												Dictionary<string, object> child = Methods.JsonHelper.DeserializeJson(json, i, out cend);
												bool flag13 = arraylist != null;
												if (flag13)
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
											goto IL_35A;
										}
										if (c3 == '}')
										{
											end = i;
											bool flag14 = key != null;
											if (flag14)
											{
												bool flag15 = arraylist != null;
												if (flag15)
												{
													dict.Add(key.Trim(), arraylist);
												}
												else
												{
													dict.Add(key.Trim(), Methods.JsonHelper.DecodeString(regex, sb.ToString().Trim()));
												}
											}
											return dict;
										}
									}
								}
								goto IL_344;
							}
							inSingleQuotes = !inSingleQuotes;
							inquotes = !inquotes;
							bool flag16 = !inquotes && arraylist != null;
							if (flag16)
							{
								arraylist.Add(Methods.JsonHelper.DecodeString(regex, sb.ToString()));
								sb.Length = 0;
							}
						}
						IL_35A:
						i++;
						continue;
						IL_344:
						sb.Append(c);
						bool flag17 = escend;
						if (flag17)
						{
							escbegin = false;
						}
						escend = escbegin;
						goto IL_35A;
					}
					end = json.Length - 1;
					result = dict;
				}
				return result;
			}

			// Token: 0x060001FE RID: 510 RVA: 0x0000F790 File Offset: 0x0000D990
			private static string DecodeString(Regex regex, string str)
			{
				return Regex.Unescape(regex.Replace(str, (Match match) => char.ConvertFromUtf32(int.Parse(match.Groups[1].Value, NumberStyles.HexNumber))));
			}

			// Token: 0x060001FF RID: 511 RVA: 0x0000F7D0 File Offset: 0x0000D9D0
			public static bool IsJson(string input)
			{
				input = input.Trim();
				return (input.StartsWith("{") && input.EndsWith("}")) || (input.StartsWith("[") && input.EndsWith("]"));
			}
		}
	}
}
