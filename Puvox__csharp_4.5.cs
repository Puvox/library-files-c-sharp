// =============  private everyday methods library by tazotodua@gmail.com ==========//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Xml.Linq; 
using System.Reflection;
using System.Diagnostics;
using System.Net;
using System.Globalization;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.Security.Cryptography;
 
using System.Threading.Tasks;
 
// ===============================================================================//
// ===============================================================================//
// ===============================================================================//
// ===============================================================================//
// ===============================================================================//
// ===============================================================================//
// ================================== C# generic  ================================//
// ===============================================================================//
// ===============================================================================//
// ===============================================================================//
// ===============================================================================//
// ===============================================================================//
// ===============================================================================//



namespace PuvoxLibrary
{ 
    // here is the name:   (we can use: private NinjaTrader.NinjaScript.AddOns.mymethodstt my = new NinjaTrader.NinjaScript.AddOns.mymethodstt();
    public partial class Methods
    {
        public bool is64BitOS =  Environment.Is64BitOperatingSystem;
		
		public RegistryKey getRegistryHiveKey(RegistryHive rh ){
            try
            {
                return RegistryKey.OpenBaseKey(rh, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32));
            }
            catch (Exception e)
            {
                m(e.Message);
                return (RegistryKey) null;
            }
          
		}

		public static void Read64bitRegistryFrom32bitApp(string[] args)
		{
            try
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
            catch (Exception e)
            {
                m_static(e.Message);
                return;
            }
		}
		
		
   		public Dictionary<string, string> xml_to_dictionary(string xml_data)
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






        public static Color color_from_hex(string hex_str)
        {
            try
            {
                return (Color)System.Drawing.ColorTranslator.FromHtml(hex_str);
            }
            catch (Exception e)
            {
                m(e.Message);
                return Color.Transparent;
            }
        }

        public static bool blinkActive = false;
        public static async void Blink_start(System.Windows.Forms.Control ctrl, Color c1, Color c2, short CycleTime_ms, bool BkClr)
        {
            try
            {
                blinkActive = true;
                var sw = new Stopwatch(); sw.Start();
                short halfCycle = (short)Math.Round(CycleTime_ms * 0.5);
                while (blinkActive)
                {
                    await Task.Delay(1);
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
            catch (Exception e)
            {
                m(e.Message);
                return;
            }

        }

        public static void Blink_stop(System.Windows.Forms.Control ctrl)
        {
            blinkActive = false;
        }




        //  =============== Datetime Extensions   ================//
        // 
        public DateTime ToDateTime(string s)
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
		public DateTime ToDateTime(string s, string format)
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
        public DateTime ToDateTime(string s, string format, string cultureString )
        {  
            try
            {  //tr-TR
                CultureInfo _culture = (cultureString == "") ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(cultureString);
                return DateTime.ParseExact(s: s, format: format, provider: _culture);
            }
            catch (FormatException) { throw; }
            catch (Exception) { throw; } // Given Culture is not supported culture
        }

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
        //  =============== ##### Datetime Extensions ####  ================//

		
		 
		

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
		 
		
	
    }
}



















// works in NET 3.5, but doesnt work in NinjaTrader 3.5 net

namespace PuvoxLibrary
{

    // here is the name:   (we can use: private NinjaTrader.NinjaScript.AddOns.mymethodstt my = new NinjaTrader.NinjaScript.AddOns.mymethodstt();
    public partial class Methods
    {


		// has problems with NinjaTrader 3.5 framework
     //   public Dictionary<string, string> deserialize(string str)
     //   {
     //       System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
    //        return StringedDictionary((Dictionary<string, object>)serializer.DeserializeObject(str));
    //    }


        // Dictionary<string, string> result = dObject.ToDictionary(kvp => kvp.Key, kvp => Convert.ToString(kvp.Value));

        public string serialize(Dictionary<string, object> dict)
        {
            try
            {
                return "";
                //return (new System.Web.Script.Serialization.JavaScriptSerializer()).Serialize(dict.ToDictionary(item => item.Key.ToString(), item => item.Value.ToString()));
            }
            catch (Exception e)
            {
                m(e.Message);
                return "";
            }
            
        }

        public string serialize(Dictionary<string, string> dict)
        {
            return "";
            //return (new System.Web.Script.Serialization.JavaScriptSerializer()).Serialize(dict.ToDictionary(item => item.Key.ToString(), item => item.Value.ToString()));
        }


        public string MyDictionaryToJson(Dictionary<string, int> dict)
        {
            var entries = dict.Select(d =>
                string.Format("\"{0}\": [{1}]", d.Key, string.Join(",", d.Value )));
            return "{" + string.Join(",", entries) + "}"; 
        }

		
		public string regplace(string input, string pattern){
			return	regplace(input, pattern, "");
		}

		public string regplace(string input, string pattern, string with){
			return new System.Text.RegularExpressions.Regex(pattern).Replace(input, "" );
		}

		
		
    }
}





/*
public class Is64BitDetection
{
    public static bool Is64BitOperatingSystem()
    {
        if (IntPtr.Size == 8)  // 64-bit programs run only on Win64
        {
            return true;
        }
        else  // 32-bit programs run on both 32-bit and 64-bit Windows
        {
            // Detect whether the current process is a 32-bit process running on a 64-bit system.
            bool flag;
            return ((DoesWin32MethodExist("kernel32.dll", "IsWow64Process") &&
                IsWow64Process(GetCurrentProcess(), out flag)) && flag);
        }
    }

    static bool DoesWin32MethodExist(string moduleName, string methodName)
    {
        IntPtr moduleHandle = GetModuleHandle(moduleName);
        if (moduleHandle == IntPtr.Zero)
        {
            return false;
        }
        return (GetProcAddress(moduleHandle, methodName) != IntPtr.Zero);
    }

    [DllImport("kernel32.dll")]
    static extern IntPtr GetCurrentProcess();

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    static extern IntPtr GetModuleHandle(string moduleName);

    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    static extern IntPtr GetProcAddress(IntPtr hModule,
        [MarshalAs(UnmanagedType.LPStr)]string procName);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);

    static bool is64BitOS = Is64BitOperatingSystem();
    // ===========================

}
*/


// ============ 64 bit detection ===========//

/*
static bool is64BitProcess = (IntPtr.Size == 8);
static bool is64BitOS = is64BitProcess || IsWow64(); 
[DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
[return: MarshalAs(UnmanagedType.Bool)]
private static extern bool IsWow64Process(   [In] IntPtr hProcess, [Out] out bool wow64Process  );
//                                                                                    lpSystemInfo);

public static bool IsWow64()
{
    //IntPtr.Size == 4  && 
    if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
        Environment.OSVersion.Version.Major >= 6)
    {
        using (Process p = Process.GetCurrentProcess())
        {
            bool retVal;
            if (!IsWow64Process(p.Handle, out retVal))
            {
                return false;
            }
            return retVal;
        }
    }
    else
    {
        return false;
    }
}
*/
