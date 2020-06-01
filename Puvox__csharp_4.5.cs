// =============  private everyday methods library by puvox.software  ==========//


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




		#region Color region Brushes
		public System.Windows.Media.Color ChangeColorBrightness(System.Windows.Media.Color color, string correctionFactor)
		{
			if (!(correctionFactor == "dark")) correctionFactor = "light";
			return ChangeColorBrightness(color, correctionFactor);
		}


		public System.Windows.Media.Color ChangeColorBrightness(System.Windows.Media.Color color, float correctionFactor)
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






		// Assign the BitmapImage as the ImageSource of the ImageBrush
		// p.s. no need to download like : using (WebClient client = new WebClient())  { try{ 	client.DownloadFile(new Uri("http://tradeliveglobal.com/ninja/greenbutton.png"),   Path.GetTempPath()+@"greenbutton.png");
		public static System.Windows.Media.Imaging.BitmapImage SetBitmapForButton(ImageBrush imgBtn, string filePath)
		{
			var bitmap = new System.Windows.Media.Imaging.BitmapImage();
			bitmap.BeginInit();
			bitmap.UriSource = new Uri(filePath);
			bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
			bitmap.EndInit();
			bitmap.Freeze(); // releases to thread 
			imgBtn.Dispatcher.Invoke(() => { imgBtn.ImageSource = bitmap; });
			return bitmap;
		}







		// ======================= form functions ====================//

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
		public void RemoveRoutedEventHandlers(UIElement element, RoutedEvent routedEvent)
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

		// ======================= form functions ====================//

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

        public static System.Windows.Media.Color color_from_hex2(String hex)
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


		
		
    }
}

// 64bit  detection previous : https://pastebin.com/raw/nLXbaQ5Y