// =============  Everyday methods library for me.  tazotodua@gmail.com ==========//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Net;
using System.Globalization;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Drawing;
using Microsoft.Win32;
using System.Runtime.InteropServices;






#region Puvox_CSHARP_ALL
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
    public partial class Methods
    { 
        public string baseDomain = @"https://puvox.software/";
        public string baseResponsePath = @"program-responses.php?";
        public string baseCompanyName = "Puvox"; 
        public string baseContactPath = "contact";
        public static string developerMachineString = "puvox_development_machine";

        //variables
        public string Language;
        public string Name;
        public string Slug;
        public double Version;
        public string BaseProductUrl;

        public string apiURL;
        public string responseUrl;
        public string RegRootOfThisProg;
        public string licenseKey;
        public string langCheckUrl;
        public string infoUrl;
        public string contactUrl;
        public string AppNameRegex = "(XYZXYZ)";
        public bool isDevelopment;

        public string main_response_string;
        public Dictionary<string,string> main_response;
        public bool Development_Mode;
        public bool initialized;
        public Dictionary<string, string> TheNames;

        public Methods()
        {
            isDevelopment = IsDeveloperMachine();
            enableExceptions();
        }

        public void defaultInit(Dictionary<string,string> dict, string theSlug, double version, string language, string productUrl)
        {
            defaultInit (dict[language], theSlug, version, language, productUrl);
        }
        public void defaultInit(string theName, string theSlug, double version, string language, string productUrl)
        {
            this.Language = language;
            this.Name = theName; // theNames[Language];
            this.Version = version;
            this.Slug = theSlug; // sanitizer(theName);
            this.BaseProductUrl = productUrl;
            //
            RegRootOfThisProg = "SOFTWARE\\" + baseCompanyName + "\\" + Slug + "\\";
            apiURL = ApiURL(Slug, Version);
            responseUrl = apiURL + "&action=check" + "&license=" + licensekey();
            contactUrl = baseDomain + baseContactPath;
            //if (isDevelopment)  Console.OutputEncoding = Encoding.UTF8;   //Encoding.Unicode is utf 16
			initialized= true;
        }

		public string ApiURL(string progName, double version){
			return baseDomain + baseResponsePath + "program=" + UrlEncode(progName) + "&version=" + version;
        }
        
         
		public double NetFrameworkVersion()
		{
            try
            {
                string version = Assembly
                        .GetExecutingAssembly()
                        .GetReferencedAssemblies()
                        .Where(x => x.Name == "System.Core").First().Version.ToString();
                return Convert.ToDouble(version.Substring(0, version.IndexOf(("."), 2)));
            }
            catch(Exception e)
            { 
                return 0.0;
            }
        }

        public void NetFrameworkInstalledVersions()
		{
			var baseKeyName = @"SOFTWARE\Microsoft\NET Framework Setup\NDP";
			var installedFrameworkVersions = Registry.LocalMachine.OpenSubKey(baseKeyName);

			var versionNames = installedFrameworkVersions.GetSubKeyNames();
			foreach(var v in versionNames)
				m(  v.ToString() );
		}
		
        public int cachedMinutes = 5;
        public bool setInitialResponse(  )
        {
            return setInitialResponse(cachedMinutes);
        }

        public bool debug_show_response = false;
        public bool setInitialResponse(int cachedMinutes_)
        { 
            string initial_html = "";

            string lastRespTime = getRegistryValue("last_resp_time");

            if (false && !String.IsNullOrEmpty(lastRespTime) && DateTime.Now.Subtract(TimeSpan.FromMinutes(cachedMinutes_)) < DateTime.Parse(lastRespTime))
            {
                initial_html = getRegistryValue("last_resp_data");
            }
            else
            { 
                initial_html = ReadUrl(responseUrl);
                setRegistryValue("last_resp_time", DateTime.Now.ToString() );
                setRegistryValue("last_resp_data", initial_html);
            }

            setRegistryValue("last_resp_time", DateTime.Now.ToString()); 
            if (initial_html == "-1")
                return false;
            main_response_string = initial_html;
            Dictionary<string, string> response = deserialize(initial_html);
            string got_data = (response["enchint"] == "") ? response["data"] : DecryptString(response["data"], this.Slug + response["enchint"] + this.Version.ToString());
            main_response = deserialize(got_data);
            return true;
        }

        public bool deserialize(string str, ref Dictionary<string, string> dict)
        {
            Dictionary<string, string> dict1 = deserializer_35(str);
            if (dict1 != null)
            {
                dict = dict1;
                return true;
            }
            return false;
        }

        public Dictionary<string,string> deserialize(string str)
        {
            //return ( NetFrameworkVersion() <= 3.5 ?  deserializer_35(str)  : deserializer(str) );
            return  deserializer_35(str); //
		}



        // another: 3.5 (but not in NT without reference) http://procbits.com/2011/04/21/quick-json-serializationdeserialization-in-c
        // another: https://pastebin.com/raw/hAtAwA40
        public Dictionary<string,string> deserializer_35(string str)
        {
            try
            {
                return ConvertToStringDictionary(JsonMaker.ParseJSON(str));
            }
            catch (Exception e)
            {
                return null; //new Dictionary<string, string>{ };
            }
        }
		    
        public Dictionary<string, string> ConvertToStringDictionary( Dictionary<string, object> dicti)
        {
            try
            {
                return dicti.ToDictionary(item => ((string)item.Key), item => ((string)item.Value));
            }
            catch (Exception e)
            { 
                return new Dictionary<string, string>{ };
            }
           
            //Dictionary<string, string> new_dict = new Dictionary<string, string>();
            //foreach (KeyValuePair<string, object> eachObj in dict)
            // new_dict[eachObj.Key] = eachObj.Value.ToString();
            //return new_dict;
        }

        public string getDictionaryKeyByValue(Dictionary<string,string> dict, string value)
        {
            return dict.FirstOrDefault(x => x.Value == value).Key;
        }


        public Dictionary<string, string> getInitialResponse()
        { 
			setInitialResponse(cachedMinutes);
			return main_response;
		}
		
        public string getLangUrlValue()
        {
            return ReadUrl(langCheckUrl);
        }
        public bool empty(String text)
        {
            return (text == null || text.Trim().Length == 0 || text.Trim() == "");
        }

        public bool updateAvailable()
        {
            try
            {
                return main_response["version"].ToString() != this.Version.ToString();
            }
            catch (Exception e)
            { 
                return false;
            }
           
        }

        public string licensekey() { 
            return getRegistryValue( "licenseKey", "demo_"+RandomString(32), false );
        }

        public bool isDemo()
        {
            return licensekey().Contains("demo_");
        }

        public static string Repeat(string value, int count)
        {
            return new StringBuilder(value.Length * count).Insert(0, value, count).ToString();
        }

        public string NewLinedString(string text, int after_how_many_words)
        {
            string finalStr = "";
            string[] lines = text.Split(new string[] { "\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            int lineNum = 0;
            foreach (string eachLine in lines)
            {
                lineNum++;
                string[] arr = eachLine.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                int wordNum = 0;
                foreach (string str in arr)
                {
                    wordNum++;
                    if (wordNum < after_how_many_words)
                    {
                        finalStr += str + " ";
                    }
                    else if (wordNum == after_how_many_words)
                    {
                        finalStr += str + Environment.NewLine;
                        wordNum = 0;
                    }
                }
                if (lineNum != lines.Count())
                {
                    finalStr += "\r\n";
                }
            }
            return finalStr;
        }

        public string NewLinedString_static(string text, int after_how_many_words)
        {
            return NewLinedString(text, after_how_many_words);
        } 

        public string GetControlText(Form obj, string which)
        {
            System.Windows.Forms.Control[] cs = obj.Controls.Find(which, true);
            return (cs.Length > 0) ? cs[0].Text : "cant find";
        }

        public void SetControlText(Form obj, string which, string txt)
        {
            try
            {
                if (obj == null) return;
                //if (!obj.IsHandleCreated) return;

                if (obj.InvokeRequired)
                {
                    obj.Invoke(new System.Action(() => SetControlText(obj, which, txt)));   //new Action<string>(SetControlText), new object[] { which, txt }
                    return;
                }

                System.Windows.Forms.Control[] cs = obj.Controls.Find(which, true);
                if (cs.Count() > 0)
                {
                    cs[0].Text = txt;
                }
                return;
            }
            catch (Exception e)
            { 
                return;
            } 


            //  final_str = txt.ToString();
            //  var propInfo = this.GetType().GetProperty(which);
            //   if (propInfo != null)
            //  {
            //      propInfo.SetValue(this, txt, null);
            //  }
            //  textBox1.Invoke(new System.Action(() =>   {    textBox1.Text = txt.ToString();    }));
        }

        delegate void SetTextCallback(Form f, Control ctrl, string text);

        public void SetTextControl(Form form, Control ctrl, string text)
        {
            try
            {
                if (ctrl == null) return;

                // compare the thread ID of the calling thread to the thread ID of the creating thread.
                if (ctrl.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(SetTextControl);
                    form.Invoke(d, new object[] { form, ctrl, text });
                }
                else
                {
                    ctrl.Text = text;
                }
            }
            catch (Exception e)
            {
                //m(e.Message);
                return;
            }
        }




        // =================== timer ================//
        // Standalone:
        // System.Threading.Tasks.Task.Delay(3000).ContinueWith(t => init());
        //                     or  
        // ExecuteAfter(() => MessageBox.Show("hi"), 1000 ); 
        // SetTimeout(1000,   () => { MessageBox.Show("hi");  }   );


        public void ExecuteAfter(int milliseconds, Action action)
        {
            try
            {
                System.Threading.Timer timer = null;
                timer = new System.Threading.Timer(s =>
                {
                    action();
                    timer.Dispose();
                    lock (timers)
                        timers.Remove(timer);
                }, null, milliseconds, UInt32.MaxValue - 10);
                lock (timers)
                    timers.Add(timer);
            }
            catch (Exception e)
            { 
                return;
            }
        }
        private HashSet<System.Threading.Timer> timers = new HashSet<System.Threading.Timer>();


        public TimerInterrupter SetInterval(int interval, Action function)  { return StartTimer(interval, function, true); } 
        public TimerInterrupter SetTimeout(int interval, Action function)   { return StartTimer(interval, function, false);  }

        private TimerInterrupter StartTimer(int interval, Action function, bool autoReset)
        {
            try
            {
                Action functionCopy = (Action)function.Clone();
                System.Timers.Timer timer = new System.Timers.Timer { Interval = interval, AutoReset = autoReset };
                timer.Elapsed += (sender, e) => functionCopy();
                timer.Start();

                return new TimerInterrupter(timer);
            }
            catch (Exception e)
            { 
                return null;
            }
        }

        public class TimerInterrupter
        {
            private readonly System.Timers.Timer _timer;
            public TimerInterrupter(System.Timers.Timer timer)
            {
                if (timer == null) throw new ArgumentNullException(nameof(timer));
                _timer = timer;
            }

            public void Stop(){   _timer.Stop();   }
        }
        // =================== timer ================//






        public string EncodeUrl(string msg) { return Uri.EscapeDataString(msg); }
        public string SanitizeSymbol(string s) { return (s.Replace("/", "_").Replace("\\", "_").Replace("|", "_").Replace("*", "_")).ToUpper(); }


        public string NotNull(string smth) { return (string.IsNullOrEmpty(smth) ? "" : smth); }
        public bool Empty(object smth) { return smth == null || !smth.GetType().GetProperties().Any(); }
        //public string EmptyStringBreak(dynamic smth) { if(smth == null || !smth.GetType().GetProperties().Any()) { MessageBox.Show("empty value");); } }

        //public bool XelementIsEmpty( System.Linq.XElement smth) { if (smth == null) return true; return false; }
		
        private int LastBarNum;
        public bool IsNewBar(int currentBar) { bool IsNew = LastBarNum != currentBar; LastBarNum = currentBar; return IsNew; }
        // lastBarOnChart = (State != State.Historical) || (Calculate == Calculate.OnBarClose && Count - 2 <= CurrentBar) || (Calculate!= Calculate.OnBarClose && Count - 1 <= CurrentBar);

        public int HoursMinutes(DateTime time) { return time.Hour * 100 + time.Minute; }

        public bool HasValue(double value) { return !Double.IsNaN(value) && !Double.IsInfinity(value); }
        public bool HasValue(bool value) { return value != null; }
        public bool ContainsValue(Dictionary<int, double> myd, int indx) { return myd.ContainsKey(indx) && myd[indx] != double.NaN && HasValue(myd[indx]); }
        public bool ContainsValue(Dictionary<int, bool> myd, int indx) { return myd.ContainsKey(indx) && HasValue(myd[indx]); }
        public bool ContainsValue(Dictionary<string, double> myd, string indx) { return myd.ContainsKey(indx) && myd[indx] != double.NaN && HasValue(myd[indx]); }

        private Dictionary<string, string> symbols = new Dictionary<string, string>() {
            {"checkmark", "âœ”"},
            {"checkmark2", "ðŸ—¹"},
            //{"checkmark", "Ã¢Å“â€"},
            //{"checkmark2", "Ã°Å¸â€”Â¹"},
        };
 

        public List<string> PropertyInfoToList(PropertyInfo[] PropList) { List<string> x = new List<string>(); foreach (PropertyInfo each in PropList) { x.Add(each.Name); } return x; }
        public List<string> ObjectPropertyNames(object Obj) { return PropertyInfoToList(Obj.GetType().GetProperties()); }

        public string Trace(Exception ex)
        {
            try
            {
                var st = new StackTrace(ex, true);
                var frame = st.GetFrame(0);
                var line = frame.GetFileLineNumber();
                return line.ToString();
            }
            catch (Exception e)
            { 
                return "";
            }
        }

        // https://ninjatrader.com/support/helpGuides/nt8/en-us/?alert_and_debug_concepts.htm
        //usage: ExceptionPrint(this, System.Reflection.MethodBase, e)
        //Print( "["+this.GetType().Name+"]["+System.Reflection.MethodBase.GetCurrentMethod().Name+"]-[ERROR at " + mymethodstt.Trace(e) +"] "+  e.ToString());

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

 
		
        public void errorlog(string message) { }

        public string RandomString(int length)
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

        //(int)Enum.Parse(typeof(TestAppAreana.MovieList.Movies), KeyVal);

		public string DateToSeconds(DateTime date){ return DateToSeconds_static(date); }
		public static string DateToSeconds_static(DateTime date){ return date.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture); }
		
        public bool ContainsKey(NameValueCollection collection, string key)
        {
            return (collection.Get(key) == null) ? collection.AllKeys.Contains(key)  : true;
        }

        // ============================== find nearest values ====================================== //
        public Dictionary<int, double> FindNearestValuesInDict(Dictionary<int, double> MyDict, double Target_value, int how_many_values_to_find)
        {
            try
            {
                bool Is_Ascending = DictIsAscendingOrDescending(MyDict);
                int KeysAmount = MyDict.Keys.Count();
                int greater_count = 0, lower_count = 0;
                int idx_pos = 0, idx_neg = 0;
                Dictionary<int, double> MyDictFinal = new Dictionary<int, double>(MyDict);

                for (int i = 0; i < KeysAmount; i++)
                {
                    idx_pos = Is_Ascending ? i : KeysAmount - 1 - i;
                    idx_neg = Is_Ascending ? KeysAmount - 1 - i : i;
                    //remove all "grater than" occurences
                    if (MyDict.Keys.ElementAtOrDefault(idx_pos) != 0)
                    {
                        int keyNm_pos = MyDict.Keys.ElementAt(idx_pos);
                        if (ContainsValue(MyDict, keyNm_pos))
                        {
                            if (MyDict[keyNm_pos] > Target_value)
                            {
                                greater_count++;
                                if (greater_count > how_many_values_to_find)
                                {
                                    MyDictFinal.Remove(keyNm_pos);
                                }
                            }
                        }
                    }

                    //remove all "lower than" occurences
                    if (MyDict.Keys.ElementAtOrDefault(idx_neg) != 0)
                    {
                        int keyNm_neg = MyDict.Keys.ElementAt(idx_neg);
                        if (ContainsValue(MyDict, keyNm_neg))
                        {
                            if (MyDict[keyNm_neg] < Target_value)
                            {
                                lower_count++;
                                if (lower_count > how_many_values_to_find)
                                {
                                    MyDictFinal.Remove(keyNm_neg);
                                }
                            }
                        }
                    }
                }
                return MyDictFinal;
            }
            catch (Exception e)
            { 
                return null;
            }
        }

        // helper function to above
        public bool DictIsAscendingOrDescending(Dictionary<int, double> MyDict)
        {
            // Note: some keys might not exist at all //
            int KeysAmount = MyDict.Keys.Count();
            double last_obtained_value = double.NaN;
            //find if ascending or descending
            for (int i = 0; i < KeysAmount; i++)
            {
                int keyNm = MyDict.Keys.ElementAt(i);
                if (ContainsValue(MyDict, keyNm))
                {
                    if (!double.IsNaN(last_obtained_value))
                    {
                        return MyDict[keyNm] > last_obtained_value;
                    }
                    last_obtained_value = MyDict[keyNm];
                }
            }
            return false;
        }
        // ============================================================================ //


	

        public class WebDownload : WebClient
        {
            /// <summary>
            /// Time in milliseconds
            /// </summary>
            public int Timeout { get; set; }

            public WebDownload() : this(60000) { }

            public WebDownload(int timeout)
            {
                this.Timeout = timeout;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                try
                {
                    var request = base.GetWebRequest(address);
                    if (request != null)
                    {
                        request.Timeout = this.Timeout;
                    }
                    return request;
                }
                catch (Exception e)
                { 
                    return null;
                }
            }
        }


        public string ReadUrl(string url)
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
            catch (Exception e)
            { 
                return ("Error:" + e.ToString());
            }
            return responseText;
        }

        //(System.IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/debug.txt"))

        public bool DownloadFile(string url, string location)
        {
            try { 
                using (var client = new System.Net.WebClient())
                {
                    client.DownloadFile(url, location);
                }
                return true;
            }
            catch(Exception e)
            { 
                return false;
            }
        }

        public string PostToUrl(string url){
			string responseText = "-1";
            try
            {
                using(WebClient client = new WebClient())
				{
					var reqparm = new System.Collections.Specialized.NameValueCollection();
					reqparm.Add("param1", "<any> kinds & of = ? strings");
					reqparm.Add("param2", "escaping is already handled");
					byte[] responsebytes = client.UploadValues(url, "POST", reqparm);
					// string HtmlResult = wc.UploadString(URI, myParameters);
					responseText = Encoding.UTF8.GetString(responsebytes);
                    return responseText;
                }
            }
            catch (Exception e)
            { 
                return ("Error:" + e.ToString());
            }
		}


        public void openUrl(string url)
        {
            System.Windows.Forms.WebBrowser wb = new System.Windows.Forms.WebBrowser();
            wb.AllowNavigation = true;
            wb.Navigate(url);
            // wb.DocumentCompleted += wb_DocumentCompleted;
            //  private void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
            //  {
            //      WebBrowser wb = sender as WebBrowser;
            //  }
        }

        public bool internetAvailable()
        {
            return internetAvailable("");
        }

        public bool internetAvailable(string messageShow)
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (client.OpenRead("http://clients3.google.com/generate_204"))
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                if (!String.IsNullOrEmpty(messageShow))
                {
                    m(messageShow + (e.Message) );
                }
            }
			return false;
        }

        public bool closeForm(Form frm)
        {
            try
            {
                if (frm == null) return false;
                //if (!frm.IsHandleCreated) return;

                if (frm.IsHandleCreated)
                {
                    frm.Invoke((MethodInvoker)delegate
                    {
                        // close the form on the forms thread
                        frm.Close();
                    });
                }
                else
                {
                    frm.Close();
                }
                return true;
            }
            catch (Exception e)
            { 
                return false;
            }
        }

		public bool WriteFile(string fileName, string text, bool rewrite_or_add)
        {
            try
            {
                string fullPath = Environment.GetEnvironmentVariable("tmp") + @"\" + fileName + ".txt";
                string existing = rewrite_or_add ? "" : (File.Exists(fullPath) ? File.ReadAllText(fullPath) : "");
                File.WriteAllText(fullPath, existing + text);
                return true;
            }
            catch (Exception e)
            { 
                return false;
            }
		}


        public string ReadFile(string path)
        {
            return !File.Exists(path) ? "" : File.ReadAllText(path);
        }

        public bool createDir(string path)
        {
            try
            {
                return Directory.Exists(path) ? true : Directory.CreateDirectory(path) != null;
            }
            catch (Exception e)
            { 
                return false;
            }
        }

        public bool deleteDir(string path)
        {
            try
            {
                if (!Directory.Exists(path)) return false;
                else
                {
                    Directory.Delete(path);
                    return true;
                }
            }
            catch (Exception e)
            { 
                return false;
            }
        }




        public static string SplitToLines(string text, char[] splitOnCharacters, int maxStringLength)
        {
            var sb = new StringBuilder();
            var index = 0;

            while (text.Length > index)
            {
                // start a new line, unless we've just started
                if (index != 0)
                    sb.AppendLine();

                // get the next substring, else the rest of the string if remainder is shorter than `maxStringLength`
                var splitAt = index + maxStringLength <= text.Length
                    ? text.Substring(index, maxStringLength).LastIndexOfAny(splitOnCharacters)
                    : text.Length - index;

                // if can't find split location, take `maxStringLength` characters
                splitAt = (splitAt == -1) ? maxStringLength : splitAt;

                // add result to collection & increment index
                sb.Append(text.Substring(index, splitAt).Trim());
                index += splitAt;
            }

            return sb.ToString();
        }


        delegate bool Control_SetDelegate(Control ctrl, string what, object value);
        public bool Control_Set(Control ctrl, string what, object value)
        {
            try
            {
                if (ctrl == null) return false;
                //if (!ctrl.IsHandleCreated) return;

                if (ctrl.InvokeRequired)
                {
                    Control_SetDelegate d = new Control_SetDelegate(Control_Set);
                    ctrl.Invoke(d, new object[] { ctrl, what, value });
                }
                else
                {
                    if (what == "visibility")
                        ctrl.Visible = (bool)value;
                    else if (what == "text")
                        ctrl.Text = (string)value;
                    else
                        m("incorrect value in :"+ MethodBase.GetCurrentMethod().Name +":"+ what);
                }
                return true;
            }
            catch (Exception e)
            { 
                return false;
            }
        }


        public void playSound()
        {
            playSound("Speech On");
        }
        // C:\Windows\media    :   Windows Information Bar, Windows Ringout, Speech Sleep, Speech On, Windows Notify Calendar, Windows Unlock, tada, Alarm03, Alarm02, Ring02, notify, chimes
        // ding chimes chord
        public bool playSound(string path)
        {
            try
            {
                if (!path.Contains("."))
                {
                    path = @"C:\Windows\media\" + path + ".wav";
                }
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(path);
                player.Play();
                return true;
            }
            catch (Exception e)
            { 
                return false;
            }
        }
		
        public string getDesktopPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }



        // ================================================================================
        // ================================ Registry Functions ============================
        // ================================================================================

      //  public RegistryHive chosenRegHive = RegistryHive.CurrentUser;
        public RegistryKey chosenRegHive =Registry.CurrentUser;
        public Dictionary<string, string> myregs = new Dictionary<string, string>();


        public string pathFilter(string path)
	    {
	        if (!path.StartsWith("Software\\"))
	        {

	        }
	        return path;
	    }
        public string regPartFromKey(string key, int partN)
        {
            if (partN == 1) return charsInPhrase(key, "\\") == 0 ? RegRootOfThisProg : withoutLastDir(key, 1);
            if (partN == 2) return charsInPhrase(key, "\\") == 0 ? key : lastPart(key);
            return "";
        }


        //================= CHECK =====================//
        public bool existsRegistryValue(string key)
        {
            return (getRegistryValue(key) != null);
        }

        public bool existsRegistryValue(string path, string key)
        {
            return (getRegistryValue(path + key) != null);
        }


        //================= GET =====================//
        public string getRegistryValue(string key)
        {
            return getRegistryValue( regPartFromKey(key,1) , regPartFromKey(key, 2)  );
        }

        public string getRegistryValue(string key, string defaultVal, bool nothing)
        {
            string res = getRegistryValue(regPartFromKey(key, 1), regPartFromKey(key, 2));
            if (String.IsNullOrEmpty(res))
            {
                setRegistryValue(key, defaultVal);
                return defaultVal;
            }
            else
                return res;
        }

        public string getRegistryValue(string path, string key)
        {
            try
            {
                string res = "";
                if (myregs.ContainsKey(path + key)) { return myregs[path + key]; }
                // ----- 62 vs 32 :  https://apttech.wordpress.com/2012/01/06/difference-between-a-registry-hive-and-registry-key-2/ 
                RegistryKey registryBase = getRegistryHiveKey(chosenRegHive);
                if (registryBase == null) { return res; }
                RegistryKey regEntry = registryBase.OpenSubKey(path);
                if (regEntry == null) { registryBase.Close(); return res; }
                var rk = regEntry.GetValue(key);
                res = (rk != null) ? (string)rk : null;
                regEntry.Close();
                registryBase.Close();
                myregs[path + key] = res;
                return res;
            }
            catch (Exception e)
            { 
                return "";
            }
        }

        //================= SET =====================//
        public void setRegistryValue(string key, string value)
        {
            setRegistryValue( regPartFromKey(key,1) , regPartFromKey(key, 2), value);
        }

        public bool setRegistryValue(string path, string key, string value)
        {
			try {
				RegistryKey regHive = getRegistryHiveKey(chosenRegHive);
				if(regHive!= null) 
				{
					var reg = regHive.OpenSubKey(path, true);
					if (reg == null)
					    reg = regHive.CreateSubKey(path); 

				// if (reg.GetValue(key) == null) 
					myregs[path + key] = value;
					reg.SetValue(key, value);  
                    return true;
				}
				else{
					m("| regHive is null");
                    return false;
				}
			}
			catch(Exception e){ 
                return false;
            }
        }
 
		// c3.5
		public RegistryKey getRegistryHiveKey(RegistryKey rh ){
			return rh; //RegistryKey.OpenSubKey(rh ) ;
		}

		// subkey like :  subKey = "SOFTWARE\\MyCompany\\programmmm;
		public string Read(string subKey, string KeyName)
		{
			try 
			{
                RegistryKey sk1 = chosenRegHive.OpenSubKey(subKey);     // Open a subKey as read-only
                if (sk1 == null)
                {
                    return null;
                }
                else
                {
                    return (string)sk1.GetValue(KeyName.ToUpper());	//value or null is returned.
				}
            }
            catch (Exception e)
            {  
                return "";
            }
        }	

		// subkey like :  subKey = "SOFTWARE\\MyCompany\\programmmm;
		public bool Write(string subKey, string KeyName, object Value)
		{
			try
			{
				RegistryKey rk = chosenRegHive ;
				// I have to use CreateSubKey   (create or open it if already exits),   'cause OpenSubKey open a subKey as read-only
				RegistryKey sk1 = rk.CreateSubKey(subKey);
				sk1.SetValue(KeyName.ToUpper(), Value);
				return true;
			}
			catch (Exception e){ 
                return false;  
			}
		}


		// subkey like :  subKey = "SOFTWARE\\MyCompany\\programmmm;
		public bool DeleteKey(string subKey, string KeyName)
		{
			try
			{
				RegistryKey rk = chosenRegHive ;
				RegistryKey sk1 = rk.CreateSubKey(subKey);
				if ( sk1 == null )
					return true;
				else
					sk1.DeleteValue(KeyName);
				return true;
			}
			catch (Exception e){ 
                return false;
            }
		}

		// subkey like :  subKey = "SOFTWARE\\MyCompany\\programmmm;
		public bool DeleteSubKeyTree(string subKey)
		{
			try
			{
				RegistryKey rk = chosenRegHive ;
				RegistryKey sk1 = rk.OpenSubKey(subKey);
				if ( sk1 != null )   rk.DeleteSubKeyTree(subKey);
				return true;
			}
			catch (Exception e)
			{ 
                return false;
            }
		}

		// Retrive the count of subkeys at the current key.
		// subkey like :  subKey = "SOFTWARE\\MyCompany\\programmmm;
		public int SubKeyCount(string subKey)
		{
			try
			{
				// Setting
				RegistryKey rk = chosenRegHive ;
				RegistryKey sk1 = rk.OpenSubKey(subKey);
				// If the RegistryKey exists...
				return (sk1 != null ? sk1.SubKeyCount : 0 );
			}
			catch (Exception e){
				m(e.Message +  " | Retriving subkeys of" + subKey.ToUpper());
                return 0;  
			}
		}

		// Retrive the count of values in the key.
		// subkey like :  subKey = "SOFTWARE\\MyCompany\\programmmm;
		public int ValueCount(string subKey)
		{
			try
			{
				// Setting
				RegistryKey rk = chosenRegHive ;
				RegistryKey sk1 = rk.OpenSubKey(subKey);
				// If the RegistryKey exists...
				return sk1 != null ? sk1.ValueCount : 0;
			}
			catch (Exception e)
			{
				m(e.Message +  " | Retrieving subkeys of" + subKey.ToUpper());
                return 0;    
			}
        }

        public Dictionary<string, string> RegistryValuesInFolder()
        {
            try
            {
                var dict = new Dictionary<string, string>();
                string path = RegRootOfThisProg; // charsInPhrase(key, "\\") == 0 ? RegRootOfThisProg : withoutLastDir(key, 1);  //regPartFromKey("blabla", 1);

                using (RegistryKey key1 = Registry.CurrentUser.OpenSubKey(path))
                {
                    foreach (string ValueOfName in key1.GetValueNames())
                    {
                        dict[ValueOfName] = (string)key1.GetValue(ValueOfName);
                    }
                }
                return dict;
            }
            catch (Exception ex) {
                return new Dictionary<string, string>{ };
            }
        }

        public bool FirstTimeAction(string regKey)
        {
            if (getRegistryValue("triggered_"+regKey) != "y")
            {
                setRegistryValue("triggered_" + regKey, "y");
                return true;
            }
            return false;
        }
        
        // ================================================================================
        // =========================== #### Registry Functions###========================
        // ================================================================================



        public string pathDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public bool TimeGone(string Key, int minutes)
        {
            string last_upd_str = getRegistryValue("timegone_" + Key);
            if (String.IsNullOrEmpty(last_upd_str) ||  DateTime.Now > DateTime.Parse(last_upd_str).AddMinutes(minutes) )
            {
                setRegistryValue("timegone_" + Key, DateTime.Now.ToString() );
                return true;
            }
            return false;
        }

        public string md5(string input)
        {
            try
            {
                // Use input string to calculate MD5 hash
                using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                {
                    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);

                    // Convert the byte array to hexadecimal string
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("X2"));
                    }
                    return sb.ToString();
                }
            }
            catch (Exception e)
            { 
                return "";
            }
        }


        public void SafeInvoke(Control uiElement, Action updater, bool forceSynchronous)
        {
            try
            {
                if (uiElement == null)
                {
                    //throw new ArgumentNullException("uiElement");
                    return;
                }
                // if (!uiElement.IsHandleCreated) return;

                if (uiElement.InvokeRequired)
                {
                    if (forceSynchronous)
                    {
                        uiElement.Invoke((Action)delegate { SafeInvoke(uiElement, updater, forceSynchronous); });
                    }
                    else
                    {
                        uiElement.BeginInvoke((Action)delegate { SafeInvoke(uiElement, updater, forceSynchronous); });
                    }
                }
                else
                {
                    if (uiElement.IsDisposed)
                    {
                        //throw new ObjectDisposedException("Control is already disposed.");
                        return;
                    }

                    updater();
                }
            }
            catch (Exception e)
            { 
            }
            
        }






        public bool MethodExists(object objectToCheck, string methodName)
        {
            return objectToCheck.GetType().GetMethod(methodName) != null;
        }





        // encryption 
        public string EncryptString(string plainText, string password)
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
                return "";
            }
        }


        public string DecryptString(string cipherText){ return DecryptString( cipherText, this.Version.ToString());}

        public string DecryptString(string cipherText, string password)
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
                catch (Exception e) { System.Windows.Forms.MessageBox.Show("Problem in encryption. ErrorCode 274"); }
                finally
                {
                    memoryStream.Close();
                    cryptoStream.Close();
                }
                return plainText;
            }
            catch (Exception e)
            { 
                return "";
            }
        }

        // ===========

        //for(int i=0; i<buttons_info.Keys.Count(); i++){
        //	string key = buttons_info.Keys ;   //i.ToString()
        //	Dictionary b_info = new Dictionary<string, object> { b_KVP };
        //if(b_info == null) continue;

        //Action<object,RoutedEventArgs,string>  x= (object sender, RoutedEventArgs e, string index )=>Print("foo") ;  
        //buttons[key].Click += (sender, EventArgs) => {  x(sender, EventArgs, (string) key.ToString()+" Hiii"); };


        public Dictionary<string, string> SortDict(Dictionary<string, string> dict)
        {
            // Order by values.
            //var items = from pair in dict      orderby pair.Value ascending       select pair;
            // foreach (KeyValuePair<string, int> pair in items)  {  Console.WriteLine("{0}: {1}", pair.Key, pair.Value);  }
            //return items.ToDictionary(x => x, x => x);try
            try {
                return (Dictionary<string, string>)(from entry in dict orderby entry.Value ascending select entry);
            }
            catch (Exception e)
            { 
                return new Dictionary<string, string>{ };
            }
            
        }


        public void FillCombobox(System.Windows.Forms.ComboBox cb1, Dictionary<string, string> dict)
        {
            FillCombobox(cb1, dict, false, "");
        }
        public void FillCombobox(System.Windows.Forms.ComboBox cb1, Dictionary<string, string> dict, bool sort, string SelectedValue)
        {
            cb1.DataSource = new BindingSource((sort ? SortDict(dict) : dict), null);
            cb1.DisplayMember = "Value";
            cb1.ValueMember = "Key";
            if (SelectedValue != "") {
                cb1.SelectedValue = SelectedValue;
            }
        }





        // ============== translations ============== //  
        public string getLangFromIsoLang(string isoLang)
        {
            return new Regex(@"\-(.*)", RegexOptions.IgnoreCase).Replace(isoLang, "");
        }

        // only needed for google translate or similar apps 
        public void SetGoogleKeyFile(string FileContent)
        {
            // set any path
            string gsapiKeyPath = System.Environment.GetEnvironmentVariable("appdata") + "\\gsapk.xyz";
            setRegistryValue("gsapiKeyPath", gsapiKeyPath);
            File.WriteAllText(gsapiKeyPath, FileContent);
            SetGoogleCredentialsDefaults(gsapiKeyPath);
        }
        public void SetGoogleCredentialsDefaults(string path)
        {
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
        }
        // ===========================



        public string selectedLang = "en";
        public Dictionary<string, Dictionary<string, string>> transl_texts;

        public string translate(string txt)
        {
            if (transl_texts.ContainsKey(txt) && transl_texts[txt].ContainsKey(selectedLang))
                return transl_texts[txt][selectedLang];
            else
                return translatedReady(selectedLang, txt);
        }



        public void setTexts(Dictionary<string, string> dict, string lang_2char, Form frm)
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

        public string translatedReady(string targetLang, string englishValue)
        {
            string defVal = englishValue;
            string regKeyName = "translReady_" + md5(englishValue) + "_" + targetLang;
            if (targetLang != "en")
            {
                string reg_val = getRegistryValue(regKeyName);
                if (reg_val == null)
                {
                    defVal = GTranslate_checker(englishValue, targetLang);
                    setRegistryValue(regKeyName, defVal);
                }
                else
                {
                    defVal = reg_val;
                }
            }
            else
            {
                setRegistryValue(regKeyName, englishValue);
            }
            return defVal;
        }

        public string GTranslate_checker(string what, string lang_target)
        {
            string res = "";

            if (MethodExists(this, "GTranslate_callback"))
            {
                var type = typeof(Methods); //Type.GetType(type_name);
                var method = type.GetMethod("GTranslate_callback", BindingFlags.Public | BindingFlags.Instance);
                res = (string)method.Invoke(this, new object[] { what, lang_target, "en" });
            }
            else
            {
                res = what;
            }
            return res;
        }

        Dictionary<string, string> _dict;
        public string gTranslate(string what, string lang_target, bool useApi=false)
        {
            string url          = "";
            string sourceLang   = "en";
            string response     = "";
            string translated   = ""; 
            if (useApi)
            {
                response = ReadUrl(apiURL + "&action=translate&lang=" + lang_target + "&string[]=" + UrlEncode(what) + DeveloperMachineMode());
                if( deserialize(response, ref _dict) )
                {
                    if (_dict.ContainsKey(what))
                    {
                        translated = _dict[what] as string;
                    }
                }
            }
            else
            {
                // https://stackoverflow.com/questions/26714426/what-is-the-meaning-of-google-translate-query-params
                response = ReadUrl("https://translate.googleapis.com/translate_a/single?client=gtx&sl=" + sourceLang + "&tl=" + lang_target + "&dt=t&q=" + UrlEncode(what));
                var result = System.Text.RegularExpressions.Regex.Match(response, @"\[\[\[""(.*?)"",""");
                if (result.Success)
                {
                    translated = result.Groups[1].Value;
                    translated = translated.Replace(@"\", "");
                }
            }
            return translated;
        }

        public bool IsDeveloperMachine()
        {
            return IsDeveloperMachine_static();
        }
        public static bool IsDeveloperMachine_static()
        {
            return (Environment.GetEnvironmentVariable(developerMachineString) != null);
        }
        public string DeveloperMachineMode()
        { 
            return (!IsDeveloperMachine()) ? "" : "&developerKey="+ Environment.GetEnvironmentVariable(developerMachineString).ToString();
        }
        //============================================ //
















        //============================================ //
        // https://stackoverflow.com/questions/3840762/how-do-you-urlencode-without-using-system-web 
        public string UrlEncode(string str)
        {
            return Uri.EscapeDataString(str).Replace("%20", "+");
        }
            

        public void CenterLabel(System.Windows.Forms.Control ctrl)
        {
            try
            {
                ctrl.AutoSize = false;
                ctrl.Dock = DockStyle.None;
                ctrl.Width = ctrl.Parent.Width;
                if (ctrl is System.Windows.Forms.Label) (ctrl as System.Windows.Forms.Label).TextAlign = ContentAlignment.MiddleCenter;
            }
            catch (Exception e)
            {
                m(e.Message);
                return;
            }
        }


        public bool isUserInput(Control ct)
        {
            if (ct is TextBox || ct is ComboBox || ct is CheckBox || ct is ColorDialog || ct is CheckedListBox || ct is DateTimePicker || ct is GroupBox || ct is ListBox || ct is MonthCalendar || ct is RadioButton || ct is RichTextBox || ct is TrackBar || ct is FileDialog)
            {
                return true;
            }
            return false;
        }



        public string optionsPrefix = "formoption_";

        public bool fillFormOptions(Control.ControlCollection cts)
        {
            foreach (Control ct in cts)
            {
                if (ct == null || !isUserInput(ct)) continue;
                string txt = "";
                if (ct is CheckBox)
                    txt = ((CheckBox)ct).Checked.ToString();
                else
                    txt = ct.Text;
                ct.Text=getRegistryValue(optionsPrefix + ct.Name, txt, false);
            }
            return true;
        }
        public bool saveFormOptions(Control.ControlCollection cts)
        {
            foreach (Control ct in cts)
            {
                if (ct != null && isUserInput(ct))
                {
                    string txt = "";
                    if (ct is CheckBox)
                        txt = ((CheckBox)ct).Checked.ToString();
                    else
                        txt = ct.Text;
                    setRegistryValue(optionsPrefix + ct.Name, txt);
                }
            }
            return true;
        }




        public void SetComboboxByKey(System.Windows.Forms.ComboBox cb1, string key)
        {
            cb1.SelectedIndex = getIndexByKey(objToDict(cb1.Items), key);
        }

        //============================================ //


        public Dictionary<string, string> objToDict(System.Windows.Forms.ComboBox.ObjectCollection objs)
        {
            var dict = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> each in objs)
            {
                dict.Add(each.Key, each.Value);
            }
            return dict;
        }

        public int getIndexByKey(Dictionary<string, string> dict, string key)
        {
            List<KeyValuePair<string, string>> values = dict.ToList();
            return values.FindIndex(x => x.Key == key);
        }
 
        // 
        public string debugType = "visualstudio";

		public string nl = System.Environment.NewLine;
        public void debug(string txt)       { Debug.Write(nl + txt); }
        public void debug(float txt)        { Debug.Write(nl + txt.ToString() ); }
        public void debug(double txt)       { Debug.Write(nl + txt.ToString() ); }
        public void debug(int txt)          { Debug.Write(nl + txt.ToString() ); }
        public void debug(string[] strar)   { foreach (string a in strar)   { debug(nl + a); }              }
        public void debug(float[] strar)    { foreach (float a in strar)    { debug(nl + a.ToString()); }   }
        public void debug(double[] strar)   { foreach (double a in strar)   { debug(nl + a.ToString()); }   }
        public void debug(int[] strar)      { foreach (int a in strar)      { debug(nl + a.ToString()); }   }

        public void debug1(object obj)
        {
            debug1(obj,  false);
        }
        public void debug1(object obj, bool popup)
        {
            if (debugType == "visualstudio")
            {
                Debug.Write(PrintProperties(obj,0));
            }
        }

        public void m(int tt)          	    { m_static(tt.ToString()); }
        public void m(string tt)            { m_static(tt.ToString()); }
        public void m(double tt)            { m_static(tt.ToString()); }
        public void m(object tt)            { m_static(PrintProperties(tt, 0).ToString()); }
        public void m(Exception tt)         { m_static(tt.Message); }
        public static void m_static(object tt) { m_static_callback(tt); }
        public static void m_static_callback(object tt) {
            string message = tt == null ? "null" : tt.ToString();
            //System.Windows.Forms.MessageBox.Show();
            System.Windows.Forms.MessageBox.Show(new System.Windows.Forms.Form { TopMost = true, Width = 300, MaximumSize = new Size(500, Convert.ToInt32(Screen.PrimaryScreen.Bounds.Height * 0.7) ) }, message);
        }
        public void cw(int tt) { cw_static(tt.ToString()); }
        public void cw(string tt) { cw_static(tt.ToString()); }
        public void cw(double tt) { cw_static(tt.ToString()); }
        public void cw(object tt) { cw_static(PrintProperties(tt, 0).ToString()); }
        public void cw(Exception tt) { cw_static(tt.Message); }
        public void cw_static(object tt) { Console.Write(tt.ToString()); }
        public void dw(int tt) { dw_static(tt.ToString()); }
        public void dw(string tt) { dw_static(tt.ToString()); }
        public void dw(double tt) { dw_static(tt.ToString()); }
        public void dw(object tt) { dw_static(PrintProperties(tt, 0).ToString()); }
        public void dw(Exception tt) { dw_static(tt.Message); }
        public void dw_static(object tt) { Debug.Write(tt.ToString()); }

        //  var dialogResult = MessageBox.Show("Message", "Title", MessageBoxButtons.OKCancel);
        //if (dialogResult == System.Windows.Forms.DialogResult.OK)
        //    MessageBox.Show("OK Clicked");
        //else
        //    MessageBox.Show("Cancel Clicked");
        public bool IsDouble(object Expression)
		{
			return IsDouble_static(Expression);
		}
		public static bool IsDouble_static(object Expression)
		{
            try
            {
                double retNum;
                return Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            }
            catch (Exception e)
            { 
                return false;
            }
        }
		
		public bool IsInt(string Expression)
		{
			return IsInt_static(Expression);
		}
		public static bool IsInt_static(string Expression)
		{
            try
            {
                int n; return int.TryParse(Expression, out n);
            }
            catch (Exception e)
            { 
                return false;
            }
		}
 

        static internal BindingFlags AllBindingFlags			= BindingFlags.CreateInstance | BindingFlags.ExactBinding |  BindingFlags.FlattenHierarchy | BindingFlags.GetField |  BindingFlags.GetProperty |  BindingFlags.IgnoreCase |  BindingFlags.IgnoreReturn |  BindingFlags.Instance | BindingFlags.InvokeMethod |  BindingFlags.NonPublic |  BindingFlags.OptionalParamBinding |  BindingFlags.Public |  BindingFlags.PutDispProperty |  BindingFlags.PutRefDispProperty | BindingFlags.SetField |  BindingFlags.SetProperty | BindingFlags.Static | BindingFlags.SuppressChangeType ;  // BindingFlags.DeclaredOnly 
		static internal BindingFlags AllBindingFlags_noinherit	= BindingFlags.CreateInstance | BindingFlags.ExactBinding |  								 BindingFlags.GetField |  BindingFlags.GetProperty |  BindingFlags.IgnoreCase |  BindingFlags.IgnoreReturn |  BindingFlags.Instance | BindingFlags.InvokeMethod |  BindingFlags.NonPublic |  BindingFlags.OptionalParamBinding |  BindingFlags.Public |  BindingFlags.PutDispProperty |  BindingFlags.PutRefDispProperty | BindingFlags.SetField |  BindingFlags.SetProperty | BindingFlags.Static | BindingFlags.SuppressChangeType |  BindingFlags.DeclaredOnly  ;  //
		//foreach (BindingFlags bf in  Enum.GetValues(typeof(BindingFlags))){
		
		public static string pChars(string txt){  int added= Math.Max(1, 25-txt.Length);  string newTxt = txt+ new string(' ', added );  if(added>1) newTxt += '\t'; return  newTxt;  }
				//public static string pChars(string txt){  int added= Math.Max(1, 25-txt.Length); added = Convert.ToInt32(added/6);  string newTxt = txt+ new string('\t', added );  return  newTxt;  }
		
 		public string PrintAll(object obj)												{ return PrintAll_static(obj);  }
 		public string PrintAll(object obj, BindingFlags bindingFlags)					{ return PrintAll_static(obj, bindingFlags);  }
		public string PrintAll(object obj, bool inherit)								{ return PrintAll_static(obj, inherit, false );  }
		public string PrintAll(object obj, bool inherit, bool execMethods )				{ return PrintAll_static(obj, inherit, execMethods );  }
 		  public string PrintAll(object obj, BindingFlags bindingFlags, bool execMethods ){ return PrintAll_static(obj, bindingFlags, execMethods );  }
		
		public static string PrintAll_static(object obj)									{ return PrintAll_static(obj, AllBindingFlags );  		}  
		public static string PrintAll_static(object obj, BindingFlags bindingFlags)			{ return PrintAll_static(obj, AllBindingFlags, false );	}  
		public static string PrintAll_static(object obj, bool inherit )						{ return PrintAll_static(obj, ( inherit ? AllBindingFlags : AllBindingFlags_noinherit ), false  ); }
		public static string PrintAll_static(object obj, bool inherit, bool execMethods )	{ return PrintAll_static(obj, ( inherit ? AllBindingFlags : AllBindingFlags_noinherit ), execMethods  ); } 
				
		public static string PrintAll_static( object obj, BindingFlags bindingFlags, bool execMethods){
			string result = "";
			string nl = System.Environment.NewLine; 
			string line_key		= "";
			string line_value	= "";
			string line_full	= "";
			List<string> all_lines = new List<string> ();
				
			result += "<----------------- START (All Members : "+ bindingFlags.ToString() +") ---------------------->" + nl;
			try{
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
				*/


				foreach( MemberInfo prop in  GetMembersInclPrivateBase_static(obj.GetType(), bindingFlags  )  ) {   //obj.GetType().GetMembers( bindingFlags  )
					string nm ="__";
					string val = "__cant_detect__";
					try{ nm = prop.Name; } catch{}		
					try{ 
						if(prop.MemberType  == MemberTypes.Field  ){
							FieldInfo p = ((FieldInfo)(object)prop);
							if( p.GetValue(obj) != null) 
								val = p.GetValue(obj).ToString();
						}
						else if(prop.MemberType ==  MemberTypes.Property  ){
							PropertyInfo p = ((PropertyInfo)(object)prop);
							if( p.GetValue(obj, null) != null) 
								val = p.GetValue(obj, null).ToString();
						}
						else if(prop.MemberType == MemberTypes.Method  ){
							MethodInfo p = ((MethodInfo)(object)prop); execMethods=true;
							string ret_type = p.ReturnType.ToString();
							val = ret_type;		////+ "    (body: "+p.GetMethodBody().ToString() + ")";//+ " || ILS:"+ p.GetMethodBody().GetILAsByteArray().ToString();
							if(execMethods){
								string[] cont_strs		= new String[]{ "System.Boolean", "System.Double", "System.Int32", "System.String", "System.Float", "System.Type"};
								string[] not_cont_strs	= new String[]{ "System.Void"};
								if(  !not_cont_strs.Contains(ret_type)   ){
									try {
										object objVal= p.Invoke(obj, null);
										if( objVal != null) 
											val += "========" + objVal.ToString();
									}
									catch(Exception e){ val +=  "--------------cant-Invoke; " + e.Message; }
								}
							}
						}
						else if(prop.MemberType ==  MemberTypes.Constructor  ){
							ConstructorInfo p = ((ConstructorInfo)(object)prop);
							if( p.GetParameters() != null) 
								val = "params:"+ p.GetParameters().ToString(); //s + nl + "body:"+p.GetMethodBody().ToString();
						}
						else if(prop.MemberType ==  MemberTypes.Event   ){
							EventInfo p = ((EventInfo)(object)prop);
							if( p != null) 
								val = "ToString: "+ p.ToString();
						}
						else{
							val = "ToStringed: "+ prop.ToString();
						}
					} catch{ val +=  "--------------cant-get-value";}
					
					line_key	= prop.MemberType.ToString();
					line_value	= pChars(nm) + val;
					line_full	= line_key +":    " + line_value;
					if(  ! all_lines.Contains( line_full ) ){
						all_lines.Add(line_full);
						line_full = line_full  + nl;
					}
					else{
						line_full =  "";
					}
					result +=    line_full;
				}	
			}
			catch (Exception e){
				result += ExceptionMessage_static( e, System.Reflection.MethodBase.GetCurrentMethod() );
                m_static(result);
			}
			result += "<----------------- END ---------------------->"  + nl + nl + nl;
			return result;
		}
		
		
		public static MemberInfo[] GetMembersInclPrivateBase_static(Type t, BindingFlags flags)
		{
            try
            {
                var memberList = new List<MemberInfo>();
                memberList.AddRange(t.GetMembers(flags));
                Type currentType = t;
                while ((currentType = currentType.BaseType) != null)
                    memberList.AddRange(currentType.GetMembers(flags));
                return memberList.ToArray();
            }
            catch (Exception e)
            { 
                return new MemberInfo[] { };
            }
		}
		
		
		
		//
		public T propertyGet<T>(object obj_, string propName)
		{
            return propertyGet_static<T>(obj_, propName);

        }


		public static T propertyGet_static<T>( object obj_, string propName )
        {
            try
            {
                var propInfo = obj_.GetType().GetProperty(propName);
                if (propInfo != null)
                {
                    if (typeof(T) == typeof(string))
                    {
                        return (T)Convert.ChangeType(((string)propInfo.GetValue(obj_, null)), typeof(T), CultureInfo.InvariantCulture);
                    }
                    else if (typeof(T) == typeof(bool))
                    {
                        return (T)Convert.ChangeType(((bool)propInfo.GetValue(obj_, null)), typeof(T), CultureInfo.InvariantCulture);
                    }
                    else if (typeof(T) == typeof(Dictionary<string, string>))
                    {
                        return (T)Convert.ChangeType(((Dictionary<string, string>)propInfo.GetValue(obj_, null)), typeof(T), CultureInfo.InvariantCulture);
                    }
                    return (T)Convert.ChangeType(propInfo.GetValue(obj_, null), typeof(T), CultureInfo.InvariantCulture);
                }

                var fieldInfo = obj_.GetType().GetField(propName);
                if (fieldInfo != null)
                {
                    if (typeof(T) == typeof(string))
                    {
                        return (T)Convert.ChangeType(((string)fieldInfo.GetValue(obj_)), typeof(T), CultureInfo.InvariantCulture);
                    }
                    else if (typeof(T) == typeof(bool))
                    {
                        return (T)Convert.ChangeType(((bool)fieldInfo.GetValue(obj_)), typeof(T), CultureInfo.InvariantCulture);
                    }
                    else if (typeof(T) == typeof(Dictionary<string, string>))
                    {
                        return (T)Convert.ChangeType(((Dictionary<string, string>)fieldInfo.GetValue(obj_)), typeof(T), CultureInfo.InvariantCulture);
                    }
                    return (T)Convert.ChangeType(fieldInfo.GetValue(obj_), typeof(T), CultureInfo.InvariantCulture);
                }
                //
                return (T)Convert.ChangeType(null, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            { 
                return (T)Convert.ChangeType("x", typeof(T), CultureInfo.InvariantCulture);
            }
        } 

		public static bool propertySet_static(object obj_, string propName, string value){
            try
            { 
                PropertyInfo propInfo
                            = obj_.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            .FirstOrDefault(x => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
                if (propInfo != null)
                {
                    if (value == "true" || value == "false")
                    {
                        propInfo.SetValue(obj_, Convert.ToBoolean(value), null);
                        return true;
                    }
                    else if (IsInt_static(value))
                    {
                        propInfo.SetValue(obj_, Convert.ToInt32(value), null);
                        return true;
                    }
                    else if (IsDouble_static(value))
                    {
                        propInfo.SetValue(obj_, Convert.ToDouble(value), null);
                        return true;
                    }
                }


                FieldInfo fieldInfo
                            = obj_.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
                            .FirstOrDefault(x => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
                if (propInfo != null)
                {
                    if (value == "true" || value == "false")
                    {

                        fieldInfo.SetValue(obj_, Convert.ToBoolean(value));
                        return true;
                    }
                    else if (IsInt_static(value))
                    {
                        fieldInfo.SetValue(obj_, Convert.ToInt32(value));
                        return true;
                    }
                    else if (IsDouble_static(value))
                    {
                        fieldInfo.SetValue(obj_, Convert.ToDouble(value));
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            { 
                return false;
            }

		}  
		
				
		public bool propertyExists(object obj_, string propName){ return propertyExists_static( obj_,  propName); }

		public static bool propertyExists_static(object obj_, string propName)
        {
            try
            {
                PropertyInfo propInfo =
                    obj_.GetType().GetProperties(AllBindingFlags).FirstOrDefault(x => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
                if (propInfo != null)
                {
                    return true;
                    //propInfo.SetValue(table, DateTime.Now);
                }
                FieldInfo fieldInfo =
                    obj_.GetType().GetFields(AllBindingFlags).FirstOrDefault(x => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
                if (fieldInfo != null)
                {
                    return true;
                    //propInfo.SetValue(table, DateTime.Now);
                }
                return false;
            }
            catch (Exception e)
            { 
                return false;
            }
		}
		
				
		 public bool objectContains(object obj_, string propName){
			 return objectContains_static( obj_,  propName);
		 }

		public static bool objectContains_static(object obj_, string propName){ 
			return ( propertyExists_static( obj_,  propName) ) ;
		}

        //============  reflection properties  get/set ===========//

        // cmd.StandardInput.WriteLine("powershell (Invoke-WebRequest http://ipinfo.io/ip).Content >> %filepath%");




        //  c# 4.5 --->  public static void ExceptionMessage(Exception e, object obj_, System.Reflection.MethodBase method, string msg, [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0) 


        public static string nl_static = System.Environment.NewLine;

        public string ExceptionMessage(Exception e) {
            StackTrace stackTrace = new StackTrace();
            MethodBase method = stackTrace.GetFrame(1).GetMethod();
            return ExceptionMessage_static(e, null );
        }

        public static string ExceptionMessage_static(Exception e)
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase method = stackTrace.GetFrame(1).GetMethod();
            return ExceptionMessage_static(e, method);
        }
		
		public static string ExceptionMessage_static(Exception e, System.Reflection.MethodBase method)
		{
			return ExceptionMessage_static( e, method, "");
		}

		public static string ExceptionMessage_static(Exception ex, System.Reflection.MethodBase method, string customMessage)
	    {
            try
            {
                var result = "";
                // Get stack trace for the exception with source file information
                //	       var trace = new System.Diagnostics.StackTrace(e, true);
                //			for(int j=0; j<trace.FrameCount; j++){
                //				p(" ---"+trace.GetFrame( j ).GetFileLineNumber() + " ---"); 
                //			}
                // p(lineNumber);
                // [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0
                // ==================		
                //				 	StackTrace st = new System.Diagnostics.StackTrace(ex);
                //			        string stackTrace = "";
                //			        foreach (StackFrame frame in st.GetFrames())
                //			        {
                //			            stackTrace = "at " + frame.GetMethod().Module.Name + "." + 
                //			                frame.GetMethod().ReflectedType.Name + "." 
                //			                + frame.GetMethod().Name 
                //			                + "  (IL offset: 0x" + frame.GetILOffset().ToString("x") + ")\n" + stackTrace;
                //			        }
                //			        p(stackTrace);
                //			        p("Message: " + ex.Message);

                result += "======================" + nl_static;
                result += "• Target Element:  " + (method.DeclaringType != null ? method.DeclaringType.Name : "") + nl_static;
                result += "• Method:          " + (method    != null ? method.Name : "") + nl_static;
                result += "• StackTrace:      " + ex.StackTrace + nl_static;
                result += "• Message:         " + ex.Message + nl_static;
                result += "• InnerException:  " + (ex.InnerException != null ? ex.InnerException.Message : "") + nl_static;
                result += "• USER_MESSAGE:    " + customMessage + nl_static;
                result += "======================" + nl_static;
                //p("Line/Number: " 	+ lineNumber.ToString() );
                return result;
            }
            catch (Exception e)
            {
                return System.Reflection.MethodBase.GetCurrentMethod().Name + "__" + e.Message;
            }
        }


        public void m_ex(Exception ex)
        {
            m_ex(ex, new StackTrace().GetFrame(1).GetMethod());
        }
        public void m_ex(Exception ex, System.Reflection.MethodBase method)
        {
            m_ex_static(ex, method);
        }

        public static void m_ex_static(Exception ex)
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase method = stackTrace.GetFrame(1).GetMethod();
            m_ex_static( ex, method);
        }

        public static void m_ex_static( Exception ex, System.Reflection.MethodBase method)
        {
            m_static(ExceptionMessage_static( ex, method, ""));
        }
        // ===

        public void enableExceptions()
        {
            enableExceptions(false);
        }
        public void enableExceptions(bool show)
        {
                enableExceptions_static( show );
        }
        public static void enableExceptions_static(bool show=false)
        {
            AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
            {
                StackTrace stackTrace = new StackTrace();
                MethodBase method = stackTrace.GetFrame(1).GetMethod();
                string message = ExceptionMessage_static(eventArgs.Exception, method, "");

                if (show || IsDeveloperMachine_static() )
                {
                    m_static(message);
                    log_static(message);
                }
                else
                {
                    log_static(message);
                }
            };
        }


        public void log(string text)
        { 
            log(text, log_location_static(this.Name) );
        }
        public void log(string text, string location)
        {
            log_static(text, location);
        }
        public static void log_static(string text)
        {
            log_static( text, log_location_static(""));
        }
        public static void log_static(string text, string location)
        {
            text = 
                Environment.NewLine + "______________   " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")  + "   ______________"
                + Environment.NewLine + text;
            // delete if above 2mb
            if ( (new System.IO.FileInfo(location).Length) > 2000000 )
                File.Delete(location);
            File.AppendAllText(location, text);
        }



        public static string log_location_static(string programName)
        {
           return Path.GetTempPath() + "\\puvox_" + programName + "_default_logs.txt";
        }

        public string PrintProperties(object obj){ return PrintProperties(obj,0); }
        public string PrintProperties(object obj, int indent)
        {
            try
            {
                if (obj == null) return "";
                string final = "";
                string indentString = Repeat("~", indent + 1); // new string('>', indent);  //instead of space
                Type objType = obj.GetType();
                foreach (PropertyInfo property in objType.GetProperties())
                {
                    ParameterInfo[] ps = property.GetIndexParameters();
                    if ((int)ps.Count() > 0)
                    {
                        if (obj.ToString().Contains("+"))
                        {
                            Debug.Write("object is multi-type");
                            Debug.Write(ObjectToXml(obj));
                        }
                        else
                        {
                            var propValue = property.GetValue(obj,null);
                            if (property.PropertyType.Assembly == objType.Assembly && !property.PropertyType.IsEnum)
                            {
                                final += indentString + property.Name + ":";
                                final += PrintProperties(propValue, indent + 2);
                            }
                            else
                            {
                                final += indentString + property.Name + ":" + propValue;
                            }
                        }
                    }
                }
                return final;
            }
            catch(Exception e)
            { 
                return "";
            }
        }

        public string DisplayObjectInfo(Object o)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                System.Type type = o.GetType();
                sb.Append("Type: " + type.Name);
                sb.Append("\r\n\r\nFields:");
                System.Reflection.FieldInfo[] fi = type.GetFields();
                if (fi.Length > 0)
                {
                    foreach (FieldInfo f in fi) { sb.Append("\r\n " + f.ToString() + " = " + f.GetValue(o)); }
                }
                else { sb.Append("\r\n None"); }
                sb.Append("\r\n\r\nProperties:");
                System.Reflection.PropertyInfo[] pi = type.GetProperties();
                if (pi.Length > 0)
                {
                    foreach (PropertyInfo p in pi) { sb.Append("\r\n " + p.ToString() + " = " + p.GetValue(o, null)); }
                }
                else { sb.Append("\r\n None"); }

                return sb.ToString();
            }
            catch (Exception e)
            { 
                return "";
            }
        }
        private string ObjectToXml(object output)
        {
            try
            {
                string objectAsXmlString; System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(output.GetType());
                using (System.IO.StringWriter sw = new System.IO.StringWriter()) {
                    try { xs.Serialize(sw, output); objectAsXmlString = sw.ToString(); } catch (Exception ex) { objectAsXmlString = ex.ToString(); }
                }
                return objectAsXmlString;
            }
            catch (Exception e)
            { 
                return "";
            } 
        }


        // ObjectDumper.Dump(obj); https://stackoverflow.com/questions/852181/c-printing-all-properties-of-an-object
        public string dump(object obj) { return ObjectDumper.Dump(obj); }
        public class ObjectDumper { private int _level; private readonly int _indentSize; private readonly StringBuilder _stringBuilder; private readonly List<int> _hashListOfFoundElements; private ObjectDumper(int indentSize) { _indentSize = indentSize; _stringBuilder = new StringBuilder(); _hashListOfFoundElements = new List<int>(); } public static string Dump(object element) { return Dump(element, 2); } public static string Dump(object element, int indentSize) { var instance = new ObjectDumper(indentSize); return instance.DumpElement(element); } private string DumpElement(object element) { if (element == null || element is ValueType || element is string) { Write(FormatValue(element)); } else { var objectType = element.GetType(); if (!typeof(IEnumerable).IsAssignableFrom(objectType)) { Write("{{{0}}}", objectType.FullName); _hashListOfFoundElements.Add(element.GetHashCode()); _level++; } var enumerableElement = element as IEnumerable; if (enumerableElement != null) { foreach (object item in enumerableElement) { if (item is IEnumerable && !(item is string)) { _level++; DumpElement(item); _level--; } else { if (!AlreadyTouched(item)) DumpElement(item); else Write("{{{0}}} <-- bidirectional reference found", item.GetType().FullName); } } } else { MemberInfo[] members = element.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance); foreach (var memberInfo in members) { var fieldInfo = memberInfo as FieldInfo; var propertyInfo = memberInfo as PropertyInfo; if (fieldInfo == null && propertyInfo == null) continue; var type = fieldInfo != null ? fieldInfo.FieldType : propertyInfo.PropertyType; object value = fieldInfo != null ? fieldInfo.GetValue(element) : propertyInfo.GetValue(element, null); if (type.IsValueType || type == typeof(string)) { Write("{0}: {1}", memberInfo.Name, FormatValue(value)); } else { var isEnumerable = typeof(IEnumerable).IsAssignableFrom(type); Write("{0}: {1}", memberInfo.Name, isEnumerable ? "..." : "{ }"); var alreadyTouched = !isEnumerable && AlreadyTouched(value); _level++; if (!alreadyTouched) DumpElement(value); else Write("{{{0}}} <-- bidirectional reference found", value.GetType().FullName); _level--; } } } if (!typeof(IEnumerable).IsAssignableFrom(objectType)) { _level--; } } return _stringBuilder.ToString(); } private bool AlreadyTouched(object value) { if (value == null) return false; var hash = value.GetHashCode(); for (var i = 0; i < _hashListOfFoundElements.Count; i++) { if (_hashListOfFoundElements[i] == hash) return true; } return false; } private void Write(string value, params object[] args) { var space = new string(' ', _level * _indentSize); if (args != null) value = string.Format(value, args); _stringBuilder.AppendLine(space + value); } private string FormatValue(object o) { if (o == null) return ("null"); if (o is DateTime) return (((DateTime)o).ToShortDateString()); if (o is string) return string.Format("\"{0}\"", o); if (o is char && (char)o == '\0') return string.Empty; if (o is ValueType) return (o.ToString()); if (o is IEnumerable) return ("..."); return ("{ }"); } }

        public int countWords(string str)
        {
            char[] delimiters = new char[] { ' ', '\r', '\n' };
            return str.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
        }


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
		public string ShowDialog(string text)
	    {
			return ShowDialog(text, "", "");
		}
		public string ShowDialog(string text, string caption, string defaultValue)
	    {
            try
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
                textBox.Text = defaultValue;
                Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
                confirmation.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(textLabel);
                prompt.AcceptButton = confirmation;
                //prompt.BringToFront() ;
                prompt.TopMost = true;
                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
            }
            catch (Exception e)
            { 
                return "";
            }
	    }


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


        // helper during development from thieve		
        public bool	workTillDate(string program_slug, string dateTill)
		{
            try
            {
                if (dateTill != "")
                {
                    if (DateTime.Now > DateTime.ParseExact(dateTill, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture))
                    {
                        if (!initialized)
                        {
                            defaultInit(program_slug, program_slug, 1.0, "en", "");
                            setInitialResponse();
                        }
                        if (main_response.ContainsKey("error_message") || main_response["error_message"] != "")
                        {
                            m(main_response["error_message"]);
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            { 
                return false;
            }
		}
		
		
		
        static public string sanitizer_static(string strIn)
        {
            return sanitizer(strIn, "replace");
        }
		
        public string sanitizer(string strIn)
        {
            return sanitizer(strIn, "replace");
        }

        static public string sanitizer(string dirtyString, string usingMethod)
        {
            try
            {
                string removeChars = " ?&^$#@!()+-,:;<>\\’\'\"-_*";
                string result = dirtyString;

                if (usingMethod == "replace")
                {
                    foreach (char c in removeChars) result = result.Replace(c.ToString(), "_");
                }
                else if (usingMethod == "regex")
                {
                    //result = new Regex.Replace(result, removeChars);
                }
                else if (usingMethod == "ld")
                {
                    result = new String(result.Where(Char.IsLetterOrDigit).ToArray());
                }
                else if (usingMethod == "hashset")
                {
                    HashSet<char> removeChars2 = new HashSet<char>(removeChars);
                    StringBuilder result1 = new StringBuilder(result.Length);
                    foreach (char c in result)
                        if (removeChars2.Contains(c))
                            result1.Append(c);
                    result = result1.ToString();
                }

                return result;
            }
            catch (Exception e)
            { 
                return "";
            }
        }

        public string exeName()
        {
            string filelocat = System.Reflection.Assembly.GetEntryAssembly().Location;
            return Path.GetFileNameWithoutExtension(filelocat);
        }

        // %appdata%\Microsoft\Default
        // registry checkings
        // check if PROTOCOL url exists 
        // RegistryKey rKey = Registry.ClassesRoot.OpenSubKey(GloballlTsVar, false);
        // if (rKey != null)   {  RegistryKey reg = Registry.ClassesRoot.OpenSubKey(@"tossc\shell\open\command").GetValue(null).ToString();  }

        // regex samples: https://pastebin.com/raw/SCVaaiJ7
        //ExecuteCommand(@"/c start tossc:" + decodedString);


        public string Base64Encode(string toEncode)
        {
            try
            {
                byte[] toEncodeAsBytes = Encoding.UTF8.GetBytes(toEncode); //System.Text.ASCIIEncoding.ASCII.GetBytes
                string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
                return returnValue;
            }
            catch (Exception e)
            { 
                return "";
            }
        }

        public string Base64Decode(string str)
        {
            try
            {
                byte[] data = Convert.FromBase64String(str);
                string decodedString = Encoding.UTF8.GetString(data);
                return decodedString;
            }
            catch (Exception e)
            { 
                return "";
            }
        }

        public void ExecuteCommand(String command)
        {
            try
            {
                Process p = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = command;
                p.StartInfo = startInfo;
                p.Start();
            }
            catch (Exception e)
            { 
                return;
            }
        }

        //in main
        //ShowInTaskbar = false;
        //WindowState = FormWindowState.Minimized;
        //Load += new EventHandler(Window_Load);

        public string readFile(string filepath)
        {
            string readContents = "";
            if (File.Exists(filepath))
            {
                using (StreamReader streamReader = new StreamReader(filepath, Encoding.UTF8))
                {
                    readContents = streamReader.ReadToEnd();
                }
            }
            return readContents;
        }

        public string lastPart(string path)
        {
            return (path.Contains(@"\") ? path.Split('\\').Last() : path.Split('/').Last() );
        }







        // public bool setRegistryValue1(string parentKey, string key)
        // {
        //     registryEntry = registryBase.CreateSubKey(keyPath,
        //     RegistryKeyPermissionCheck.ReadWriteSubTree);
        //     registryEntry.SetValue(valueName, newValue);
        // }


        public int charsInPhrase(string source, string char_)
        {
            int cnt = 0;
            foreach (char c in source)
                if (c.ToString() == char_)
                    cnt++;
            return cnt;
            // return source.Count(f => f ==  char_);
        }

        public string withoutLastDir(string path, int HowManyLastDirs)
        {
            string final = path;
            if (charsInPhrase(path, "\\") > HowManyLastDirs)
            {
                final=String.Join(@"\", path.Split('\\').Reverse().Skip(HowManyLastDirs).Reverse().ToArray());
            }
            return final;
        }

		
		
		
    } // SharedMethods
	
	
}














#endregion