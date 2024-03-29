﻿using System;
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
using Google.Cloud.Speech.V1;
using System.Threading;

namespace PuvoxLibrary
{

    // here is the name:   (we can use: private NinjaTrader.NinjaScript.AddOns.mymethodstt my = new NinjaTrader.NinjaScript.AddOns.mymethodstt();
    public partial class GoogleSpeech_ 
    {
		private SpeechTyperForWord.Form1 frm;
        PuvoxLibrary.Program program;

        public GoogleSpeech_(SpeechTyperForWord.Form1 frm_, PuvoxLibrary.Program program_)
        {
            frm = frm_;
            program = program_;
        }


        // https://cloud.google.com/speech/reference/rest/v1/Code
        public Dictionary<string, string> GoogleVoiceLangs(int type)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            if (type == 1)
            {
                dict = new Dictionary<string, string>()
              {
                { "af-ZA" , "Afrikaans (South Africa)|Afrikaans (Suid-Afrika)"}, { "am-ET" , "Amharic (Ethiopia)|አማርኛ (ኢትዮጵያ)"}, { "hy-AM" , "Armenian (Armenia)|Հայ (Հայաստան)"}, { "az-AZ" , "Azerbaijani (Azerbaijan)|Azərbaycan (Azərbaycan)"}, { "id-ID" , "Indonesian (Indonesia)|Bahasa Indonesia (Indonesia)"}, { "ms-MY" , "Malay (Malaysia)|Bahasa Melayu (Malaysia)"}, { "bn-BD" , "Bengali (Bangladesh)|বাংলা (বাংলাদেশ)"}, { "bn-IN" , "Bengali (India)|বাংলা (ভারত)"}, { "ca-ES" , "Catalan (Spain)|Català (Espanya)"}, { "cs-CZ" , "Czech (Czech Republic)|Čeština (Česká republika)"}, { "da-DK" , "Danish (Denmark)|Dansk (Danmark)"}, { "de-DE" , "German (Germany)|Deutsch (Deutschland)"}, { "en-AU" , "English (Australia)|English (Australia)"}, { "en-CA" , "English (Canada)|English (Canada)"}, { "en-GH" , "English (Ghana)|English (Ghana)"}, { "en-GB" , "English (United Kingdom)|English (Great Britain)"}, { "en-IN" , "English (India)|English (India)"}, { "en-IE" , "English (Ireland)|English (Ireland)"}, { "en-KE" , "English (Kenya)|English (Kenya)"}, { "en-NZ" , "English (New Zealand)|English (New Zealand)"}, { "en-NG" , "English (Nigeria)|English (Nigeria)"}, { "en-PH" , "English (Philippines)|English (Philippines)"}, { "en-ZA" , "English (South Africa)|English (South Africa)"}, { "en-TZ" , "English (Tanzania)|English (Tanzania)"}, { "en-US" , "English (United States)|English (United States)"}, { "es-AR" , "Spanish (Argentina)|Español (Argentina)"}, { "es-BO" , "Spanish (Bolivia)|Español (Bolivia)"}, { "es-CL" , "Spanish (Chile)|Español (Chile)"}, { "es-CO" , "Spanish (Colombia)|Español (Colombia)"}, { "es-CR" , "Spanish (Costa Rica)|Español (Costa Rica)"}, { "es-EC" , "Spanish (Ecuador)|Español (Ecuador)"}, { "es-SV" , "Spanish (El Salvador)|Español (El Salvador)"}, { "es-ES" , "Spanish (Spain)|Español (España)"}, { "es-US" , "Spanish (United States)|Español (Estados Unidos)"}, { "es-GT" , "Spanish (Guatemala)|Español (Guatemala)"}, { "es-HN" , "Spanish (Honduras)|Español (Honduras)"}, { "es-MX" , "Spanish (Mexico)|Español (México)"}, { "es-NI" , "Spanish (Nicaragua)|Español (Nicaragua)"}, { "es-PA" , "Spanish (Panama)|Español (Panamá)"}, { "es-PY" , "Spanish (Paraguay)|Español (Paraguay)"}, { "es-PE" , "Spanish (Peru)|Español (Perú)"}, { "es-PR" , "Spanish (Puerto Rico)|Español (Puerto Rico)"}, { "es-DO" , "Spanish (Dominican Republic)|Español (República Dominicana)"}, { "es-UY" , "Spanish (Uruguay)|Español (Uruguay)"}, { "es-VE" , "Spanish (Venezuela)|Español (Venezuela)"}, { "eu-ES" , "Basque (Spain)|Euskara (Espainia)"}, { "fil-PH" , "Filipino (Philippines)|Filipino (Pilipinas)"}, { "fr-CA" , "French (Canada)|Français (Canada)"}, { "fr-FR" , "French (France)|Français (France)"}, { "gl-ES" , "Galician (Spain)|Galego (España)"}, { "ka-GE" , "Georgian (Georgia)|ქართული (საქართველო)"}, { "gu-IN" , "Gujarati (India)|ગુજરાતી (ભારત)"}, { "hr-HR" , "Croatian (Croatia)|Hrvatski (Hrvatska)"}, { "zu-ZA" , "Zulu (South Africa)|IsiZulu (Ningizimu Afrika)"}, { "is-IS" , "Icelandic (Iceland)|Íslenska (Ísland)"}, { "it-IT" , "Italian (Italy)|Italiano (Italia)"}, { "jv-ID" , "Javanese (Indonesia)|Jawa (Indonesia)"}, { "kn-IN" , "Kannada (India)|ಕನ್ನಡ (ಭಾರತ)"}, { "km-KH" , "Khmer (Cambodia)|ភាសាខ្មែរ (កម្ពុជា)"}, { "lo-LA" , "Lao (Laos)|ລາວ (ລາວ)"}, { "lv-LV" , "Latvian (Latvia)|Latviešu (latviešu)"}, { "lt-LT" , "Lithuanian (Lithuania)|Lietuvių (Lietuva)"}, { "hu-HU" , "Hungarian (Hungary)|Magyar (Magyarország)"}, { "ml-IN" , "Malayalam (India)|മലയാളം (ഇന്ത്യ)"}, { "mr-IN" , "Marathi (India)|मराठी (भारत)"}, { "nl-NL" , "Dutch (Netherlands)|Nederlands (Nederland)"}, { "ne-NP" , "Nepali (Nepal)|नेपाली (नेपाल)"}, { "nb-NO" , "Norwegian Bokmål (Norway)|Norsk bokmål (Norge)"}, { "pl-PL" , "Polish (Poland)|Polski (Polska)"}, { "pt-BR" , "Portuguese (Brazil)|Português (Brasil)"}, { "pt-PT" , "Portuguese (Portugal)|Português (Portugal)"}, { "ro-RO" , "Romanian (Romania)|Română (România)"}, { "si-LK" , "Sinhala (Sri Lanka)|සිංහල (ශ්රී ලංකාව)"}, { "sk-SK" , "Slovak (Slovakia)|Slovenčina (Slovensko)"}, { "sl-SI" , "Slovenian (Slovenia)|Slovenščina (Slovenija)"}, { "su-ID" , "Sundanese (Indonesia)|Urang (Indonesia)"}, { "sw-TZ" , "Swahili (Tanzania)|Swahili (Tanzania)"}, { "sw-KE" , "Swahili (Kenya)|Swahili (Kenya)"}, { "fi-FI" , "Finnish (Finland)|Suomi (Suomi)"}, { "sv-SE" , "Swedish (Sweden)|Svenska (Sverige)"}, { "ta-IN" , "Tamil (India)|தமிழ் (இந்தியா)"}, { "ta-SG" , "Tamil (Singapore)|தமிழ் (சிங்கப்பூர்)"}, { "ta-LK" , "Tamil (Sri Lanka)|தமிழ் (இலங்கை)"}, { "ta-MY" , "Tamil (Malaysia)|தமிழ் (மலேசியா)"}, { "te-IN" , "Telugu (India)|తెలుగు (భారతదేశం)"}, { "vi-VN" , "Vietnamese (Vietnam)|Tiếng Việt (Việt Nam)"}, { "tr-TR" , "Turkish (Turkey)|Türkçe (Türkiye)"}, { "ur-PK" , "Urdu (Pakistan)|اردو (پاکستان)"}, { "ur-IN" , "Urdu (India)|اردو (بھارت)"}, { "el-GR" , "Greek (Greece)|Ελληνικά (Ελλάδα)"}, { "bg-BG" , "Bulgarian (Bulgaria)|Български (България)"}, { "ru-RU" , "Russian (Russia)|Русский (Россия)"}, { "sr-RS" , "Serbian (Serbia)|Српски (Србија)"}, { "uk-UA" , "Ukrainian (Ukraine)|Українська (Україна)"}, { "he-IL" , "Hebrew (Israel)|עברית (ישראל)"}, { "ar-IL" , "Arabic (Israel)|العربية (إسرائيل)"}, { "ar-JO" , "Arabic (Jordan)|العربية (الأردن)"}, { "ar-AE" , "Arabic (United Arab Emirates)|العربية (الإمارات)"}, { "ar-BH" , "Arabic (Bahrain)|العربية (البحرين)"}, { "ar-DZ" , "Arabic (Algeria)|العربية (الجزائر)"}, { "ar-SA" , "Arabic (Saudi Arabia)|العربية (السعودية)"}, { "ar-IQ" , "Arabic (Iraq)|العربية (العراق)"}, { "ar-KW" , "Arabic (Kuwait)|العربية (الكويت)"}, { "ar-MA" , "Arabic (Morocco)|العربية (المغرب)"}, { "ar-TN" , "Arabic (Tunisia)|العربية (تونس)"}, { "ar-OM" , "Arabic (Oman)|العربية (عُمان)"}, { "ar-PS" , "Arabic (State of Palestine)|العربية (فلسطين)"}, { "ar-QA" , "Arabic (Qatar)|العربية (قطر)"}, { "ar-LB" , "Arabic (Lebanon)|العربية (لبنان)"}, { "ar-EG" , "Arabic (Egypt)|العربية (مصر)"}, { "fa-IR" , "Persian (Iran)|فارسی (ایران)"}, { "hi-IN" , "Hindi (India)|हिन्दी (भारत)"}, { "th-TH" , "Thai (Thailand)|ไทย (ประเทศไทย)"}, { "ko-KR" , "Korean (South Korea)|한국어 (대한민국)"}, { "cmn-Hant-TW" , "Chinese, Mandarin (Traditional, Taiwan)|國語 (台灣)"}, { "yue-Hant-HK" , "Chinese, Cantonese (Traditional, Hong Kong)|廣東話 (香港)"}, { "ja-JP" , "Japanese (Japan)|日本語（日本）"}, { "cmn-Hans-HK" , "Chinese, Mandarin (Simplified, Hong Kong)|普通話 (香港)"}, { "cmn-Hans-CN" , "Chinese, Mandarin (Simplified, China)|普通话 (中国大陆)"}
              };
            }

            else if (type == 2)
            {
                dict = new Dictionary<string, string>()
              {
                   {"af-ZA" , "Afrikaans (Suid-Afrika)"}, {"am-ET" , "አማርኛ (ኢትዮጵያ)"}, {"hy-AM" , "Հայ (Հայաստան)"}, {"az-AZ" , "Azərbaycan (Azərbaycan)"}, {"id-ID" , "Bahasa Indonesia (Indonesia)"}, {"ms-MY" , "Bahasa Melayu (Malaysia)"}, {"bn-BD" , "বাংলা (বাংলাদেশ)"}, {"bn-IN" , "বাংলা (ভারত)"}, {"ca-ES" , "Català (Espanya)"}, {"cs-CZ" , "Čeština (Česká republika)"}, {"da-DK" , "Dansk (Danmark)"}, {"de-DE" , "Deutsch (Deutschland)"}, {"en-AU" , "English (Australia)"}, {"en-CA" , "English (Canada)"}, {"en-GH" , "English (Ghana)"}, {"en-GB" , "English (Great Britain)"}, {"en-IN" , "English (India)"}, {"en-IE" , "English (Ireland)"}, {"en-KE" , "English (Kenya)"}, {"en-NZ" , "English (New Zealand)"}, {"en-NG" , "English (Nigeria)"}, {"en-PH" , "English (Philippines)"}, {"en-ZA" , "English (South Africa)"}, {"en-TZ" , "English (Tanzania)"}, {"en-US" , "English (United States)"}, {"es-AR" , "Español (Argentina)"}, {"es-BO" , "Español (Bolivia)"}, {"es-CL" , "Español (Chile)"}, {"es-CO" , "Español (Colombia)"}, {"es-CR" , "Español (Costa Rica)"}, {"es-EC" , "Español (Ecuador)"}, {"es-SV" , "Español (El Salvador)"}, {"es-ES" , "Español (España)"}, {"es-US" , "Español (Estados Unidos)"}, {"es-GT" , "Español (Guatemala)"}, {"es-HN" , "Español (Honduras)"}, {"es-MX" , "Español (México)"}, {"es-NI" , "Español (Nicaragua)"}, {"es-PA" , "Español (Panamá)"}, {"es-PY" , "Español (Paraguay)"}, {"es-PE" , "Español (Perú)"}, {"es-PR" , "Español (Puerto Rico)"}, {"es-DO" , "Español (República Dominicana)"}, {"es-UY" , "Español (Uruguay)"}, {"es-VE" , "Español (Venezuela)"}, {"eu-ES" , "Euskara (Espainia)"}, {"fil-PH" , "Filipino (Pilipinas)"}, {"fr-CA" , "Français (Canada)"}, {"fr-FR" , "Français (France)"}, {"gl-ES" , "Galego (España)"}, {"ka-GE" , "ქართული (საქართველო)"}, {"gu-IN" , "ગુજરાતી (ભારત)"}, {"hr-HR" , "Hrvatski (Hrvatska)"}, {"zu-ZA" , "IsiZulu (Ningizimu Afrika)"}, {"is-IS" , "Íslenska (Ísland)"}, {"it-IT" , "Italiano (Italia)"}, {"jv-ID" , "Jawa (Indonesia)"}, {"kn-IN" , "ಕನ್ನಡ (ಭಾರತ)"}, {"km-KH" , "ភាសាខ្មែរ (កម្ពុជា)"}, {"lo-LA" , "ລາວ (ລາວ)"}, {"lv-LV" , "Latviešu (latviešu)"}, {"lt-LT" , "Lietuvių (Lietuva)"}, {"hu-HU" , "Magyar (Magyarország)"}, {"ml-IN" , "മലയാളം (ഇന്ത്യ)"}, {"mr-IN" , "मराठी (भारत)"}, {"nl-NL" , "Nederlands (Nederland)"}, {"ne-NP" , "नेपाली (नेपाल)"}, {"nb-NO" , "Norsk bokmål (Norge)"}, {"pl-PL" , "Polski (Polska)"}, {"pt-BR" , "Português (Brasil)"}, {"pt-PT" , "Português (Portugal)"}, {"ro-RO" , "Română (România)"}, {"si-LK" , "සිංහල (ශ්රී ලංකාව)"}, {"sk-SK" , "Slovenčina (Slovensko)"}, {"sl-SI" , "Slovenščina (Slovenija)"}, {"su-ID" , "Urang (Indonesia)"}, {"sw-TZ" , "Swahili (Tanzania)"}, {"sw-KE" , "Swahili (Kenya)"}, {"fi-FI" , "Suomi (Suomi)"}, {"sv-SE" , "Svenska (Sverige)"}, {"ta-IN" , "தமிழ் (இந்தியா)"}, {"ta-SG" , "தமிழ் (சிங்கப்பூர்)"}, {"ta-LK" , "தமிழ் (இலங்கை)"}, {"ta-MY" , "தமிழ் (மலேசியா)"}, {"te-IN" , "తెలుగు (భారతదేశం)"}, {"vi-VN" , "Tiếng Việt (Việt Nam)"}, {"tr-TR" , "Türkçe (Türkiye)"}, {"ur-PK" , "اردو (پاکستان)"}, {"ur-IN" , "اردو (بھارت)"}, {"el-GR" , "Ελληνικά (Ελλάδα)"}, {"bg-BG" , "Български (България)"}, {"ru-RU" , "Русский (Россия)"}, {"sr-RS" , "Српски (Србија)"}, {"uk-UA" , "Українська (Україна)"}, {"he-IL" , "עברית (ישראל)"}, {"ar-IL" , "العربية (إسرائيل)"}, {"ar-JO" , "العربية (الأردن)"}, {"ar-AE" , "العربية (الإمارات)"}, {"ar-BH" , "العربية (البحرين)"}, {"ar-DZ" , "العربية (الجزائر)"}, {"ar-SA" , "العربية (السعودية)"}, {"ar-IQ" , "العربية (العراق)"}, {"ar-KW" , "العربية (الكويت)"}, {"ar-MA" , "العربية (المغرب)"}, {"ar-TN" , "العربية (تونس)"}, {"ar-OM" , "العربية (عُمان)"}, {"ar-PS" , "العربية (فلسطين)"}, {"ar-QA" , "العربية (قطر)"}, {"ar-LB" , "العربية (لبنان)"}, {"ar-EG" , "العربية (مصر)"}, {"fa-IR" , "فارسی (ایران)"}, {"hi-IN" , "हिन्दी (भारत)"}, {"th-TH" , "ไทย (ประเทศไทย)"}, {"ko-KR" , "한국어 (대한민국)"}, {"cmn-Hant-TW" , "國語 (台灣)"}, {"yue-Hant-HK" , "廣東話 (香港)"}, {"ja-JP" , "日本語（日本）"}, {"cmn-Hans-HK" , "普通話 (香港)"}, {"cmn-Hans-CN" , "普通话 (中国大陆)"}
              };
            }
            else if (type == 3)
            {
                dict = new Dictionary<string, string>()
              {
                 {"ka-GE" , "ქართული (საქართველო)"}
              };
            }
            return dict;

        }


        public string getIsoLangFromLang(string lang, int method)
        {
            string isolang = "en-US";
            string containsWhat = method == 1 ? lang + "-" : "-" + lang;
            foreach (KeyValuePair<string, string> each in GoogleVoiceLangs(1))
            {
                if (each.Key.Contains(containsWhat))
                {
                    isolang = each.Key;
                    break;
                }
            }
            return isolang;
        }




        public async Task<object> StreamingMicRecognizeAsync()
        {
            return StreamingMicRecognizeAsync(secondsLong);
        }

        public bool isActive = false;
        public NAudio.Wave.WaveInEvent waveIn;
        internal int secondsLong = 0;
        public string phrase_final = "";
        public string phrase_interim = "";
        private Google.Protobuf.ByteString accumulatedAudio;
        private List<Google.Protobuf.ByteString> accumulatedAudioAr = new List< Google.Protobuf.ByteString>();
        public bool streamerSet = false;
        private bool backFromRestart = false;
        private SpeechClient.StreamingRecognizeStream streamingCall ;
        public bool StoppButtoned = false;
        private bool writeMore;
        private Task prinResp;
        private object writeLock;
        private int totalSecondsGone;
        public bool isStreamActive = false;
        private static System.Threading.Timer aTimer;
        private DateTime time_;

        public async Task<object> StreamingMicRecognizeAsync(int seconds)
        {
            StoppButtoned = false;
            isStreamActive = true;
            accumulatedAudioAr.Clear();
            totalSecondsGone = 0;

            secondsLong = seconds;
            frm.GS_Record("initiated");
            if (NAudio.Wave.WaveIn.DeviceCount < 1) { PuvoxLibrary.Methods.m("No microphone!"); return -1; }

            streamingCall = SpeechClient.Create().StreamingRecognize();

            await streamingCall.WriteAsync(
                new StreamingRecognizeRequest()
                {
                    StreamingConfig = new StreamingRecognitionConfig()
                    {
                        Config = new RecognitionConfig()
                        {
                            Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                            MaxAlternatives = 1,
                            SampleRateHertz = 16000,
                            LanguageCode = frm.selectedIsoLang, //"en-US" https://cloud.google.com/speech/docs/languages
                        },
                        InterimResults = true,
                        SingleUtterance = false
                    }
                }
            );
            // Print responses as they arrive 
            prinResp = Task.Run(async () =>
            {
                try
                {
                    time_ = DateTime.Now;
                    while (isActive && isStreamActive && await streamingCall.ResponseStream.MoveNext(default(CancellationToken)))
                    {
                        foreach (var result in streamingCall.ResponseStream.Current.Results)
                        {
                            string phras = result.Alternatives[0].Transcript.ToString();
                            phras = replacePunctuations(phras, frm.selectedLang);
                            if (backFromRestart)
                            {
                                backFromRestart = false;
                                phras = " " + phras;
                            }
                            if (result.IsFinal)
                            {
                                //if (my.countWords(phras) > 1)
                                phrase_final = phras;
                                frm.GS_Record("phrase_final");
                            }
                            else
                            {
                                phrase_interim = phras;
                                frm.GS_Record("phrase_interim");
                            }
                        }
                    }
                }
                catch (Grpc.Core.RpcException e)
                {
                    //if 65 seconds passed, then restart
                    if (e.Status.StatusCode == Grpc.Core.StatusCode.OutOfRange) {
                       // Speech_Restart();
                        isStreamActive = false;
                    }

                }
            });

            // Read from the microphone and stream to API.
            writeLock = new object();
            writeMore = true;
            waveIn = new NAudio.Wave.WaveInEvent();
            waveIn.DeviceNumber = 0;
            waveIn.WaveFormat = new NAudio.Wave.WaveFormat(16000, 1);
            waveIn.DataAvailable +=
                (object sender, NAudio.Wave.WaveInEventArgs args) =>
                {
                    lock (writeLock)
                    {
                        if (!writeMore || !isActive) return;
                        try
                        {
                            accumulatedAudio = Google.Protobuf.ByteString.CopyFrom(args.Buffer, 0, args.BytesRecorded);
                            //accumulatedAudioAr.Add(accumulatedAudio);
                            // foreach (Google.Protobuf.ByteString eachAudio in accumulatedAudioAr)
                            //Task.Delay(TimeSpan.FromSeconds(1));
                            streamingCall.WriteAsync(
                                new StreamingRecognizeRequest()
                                {
                                    AudioContent = accumulatedAudio
                                }
                            ).Wait();
                        }
                        catch (Exception e) { PuvoxLibrary.Methods.m(e.Message); }
                    }
                };

            waveIn.StartRecording();
            frm.GS_Record("start");   //MessageBox.Show("Speak now.");
            //delay execution
            totalSecondsGone = (int) ((DateTime.Now - time_).TotalSeconds);
            while (isActive)
            {
                if(totalSecondsGone < seconds)
                {
                    totalSecondsGone++;
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    d(totalSecondsGone.ToString());
                }
                else{
                    isActive = false;
                }
            }
            //await Task.Delay(TimeSpan.FromSeconds(seconds));
            frm.GS_Record("stop");
            //shutdown
            waveIn.StopRecording();
            lock (writeLock) writeMore = false;
            await streamingCall.WriteCompleteAsync();
            await prinResp; //wait before printout finished
            frm.GS_Record("finished");
            if (!StoppButtoned)
            {
                backFromRestart = true;
                reSTART();
            }
            return 0;
        }

        public void shutDown()
        {
            shutDown(false);
        }

        public void shutDown(bool hard)
        {
            if (hard) StoppButtoned = true;
            isActive = false;
        }



        public void ButtonClicked_main()
        {
            isActive = ! isActive;
            Clicked_main(isActive);
        }

        private void d(string t) { System.Diagnostics.Debug.Write(t); }
        public void Clicked_main(bool isStart)
        {
            if (isStart)
            {
                isActive = true;
                StoppButtoned = false;
                reSTART();
            }
            else
            {
                isActive = false;
                StoppButtoned = true;
            }
        }

        public int HowManySeconds = 10;  //max allowed in asyncronous is 180 *60
        private void reSTART()
        {
            isActive = true;
            StreamingMicRecognizeAsync(HowManySeconds); 
        }

        //    System.Windows.Forms.Timer tmr = new System.Windows.Forms.Timer(); tmr.Interval = 100; tmr.Tick += timerHandler; tmr.Start();
        private void timerHandler(object sender, EventArgs e)
        {

        } 

        internal Dictionary<string, string> punctuations = new Dictionary<string, string>()
        {
            { ".", "dot" },
            { ",", "comma" },
            { "!", "exclamation mark" },
            { "?", "question mark" },
            { "-", "dash" },
            { "⏎", "new line" }
        };

        internal string replacePunctuations(string text, string lang)
        { 
            string output = text;
            foreach (KeyValuePair<string, string> kv in punctuations)
            {
                string regval = my.regval(kv.Key + "_" + lang);
                if (!String.IsNullOrEmpty(regval)) { 
                    output = output.Replace(regval, kv.Key+" ");
                    output = output.Replace("⏎", Environment.NewLine);
                    
                }
            }
            return output;
        }


        internal byte[] Combine_bytestring(params byte[][] arrays)
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
		
    }

}
