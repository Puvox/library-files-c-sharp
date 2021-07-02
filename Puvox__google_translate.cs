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
using System.Threading.Tasks;
using System.Security.Cryptography;
using Google.Cloud.Translation.V2;

namespace PuvoxLibrary
{

    public partial class GoogleTranslate_ 
    {
        private void m(dynamic x) { System.Windows.Forms.MessageBox.Show(x); }
        TranslationClient gTranslateClient;

        public GoogleTranslate_(string gsApiKey)
        {
            try
            {
                gTranslateClient = TranslationClient.Create();
            }
            catch(Exception e)
            {
                m(e);
            }
        }

        public string GTranslate(string text, string targetLanguageCode, string sourceLanguageCode)
        {
            var response = gTranslateClient.TranslateText(text, targetLanguageCode, sourceLanguageCode);
            return response.TranslatedText;
        }


        /*
		
		
		public string selectedLang = "en";
		public Dictionary<string, Dictionary<string, string>> transl_texts;
		public string translate(string txt)
		{
			if (transl_texts.ContainsKey(txt) && transl_texts[txt].ContainsKey(selectedLang))
			{
				return transl_texts[txt][selectedLang];
			}
			return translatedReady(selectedLang, txt);
		}




		public string translatedReady(string targetLang, string englishValue)
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
		
		
       public string getTranslateCode(string desiredLang)
       {
           if (!desiredLang)
           {
               return false;
           }
           desiredLang = desiredLang.toLowerCase();

           if (langs[desiredLang])
           {
               return desiredLang;
           }

           var keys = Object.keys(langs).filter(function(key) {
               if (typeof langs[key] !== 'string')
               {
                   return false;
               }

               return langs[key].toLowerCase() === desiredLang;
           });

           return keys[0] || false;
       }

       public bool istranslateLangSupported(desiredLang)
       {
           return Boolean(getCode(desiredLang));
       }
        */
		
		
		
    }


}
