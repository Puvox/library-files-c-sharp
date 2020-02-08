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


namespace PuvoxLibrary
{
    public partial class Transliterate
    {
         
        // https://en.wikipedia.org/wiki/Keyboard_layout

        Dictionary<string, Dictionary<string, string>> dicts = new Dictionary<string, Dictionary<string, string>>{
            { "ka_phonetic" , new Dictionary<string, string>
                {
                    {"a","ა"}, {"b","ბ"}, {"c","ც"}, {"d","დ"}, {"e","ე"}, {"f","ფ"}, {"g","გ"}, {"h","ჰ"}, {"i","ი"},
                    {"j","ჯ"}, {"k","კ"}, {"l","ლ"}, {"m","მ"}, {"n","ნ"}, {"o","ო"}, {"p","პ"}, {"q","ქ"}, {"r","რ"}, {"s","ს"},
                    {"t","ტ"}, {"u","უ"}, {"v","ვ"}, {"w","წ"}, {"x","ხ"}, {"y","ყ"}, {"z","ზ"},
                    {"A",""}, {"B",""}, {"C","ჩ"}, {"D",""}, {"E",""}, {"F",""}, {"G",""}, {"H",""}, {"I",""},
                    {"J","ჟ"}, {"K",""}, {"L","₾"}, {"M",""}, {"N",""}, {"O",""}, {"P",""}, {"Q",""}, {"R","ღ"}, {"S","შ"},
                    {"T","თ"}, {"U",""}, {"V",""}, {"W","ჭ"}, {"X",""}, {"Y",""}, {"Z","ძ"},
                    // other rows in UPPERCASE and in LOWERCASE
                    {"1",""},  {"2",""},   {"3",""},  {"4",""},  {"5",""},  {"6",""},  {"7",""}, {"8",""},  {"9",""},  {"0",""},
                    {"!",""},  {"@",""},   {"#",""},  {"$",""},  {"%",""},  {"^",""},  {"&",""}, {"*",""},  {"(",""},  {")",""},

                    {"`",""},  {"-",""},  {"=",""},
                    {"~",""},  {"_",""},  {"+",""},

                    {"[",""},  {"]",""},  {"\\",""},
                    {"{",""},  {"}",""},  {"|",""},

                    {";",""},  {"'",""},
                    {":",""},  {"\"",""},

                    {",",""},  {".",""}, {"/",""},
                    {"<",""},  {">",""}, {"?",""}
                }
            },
            { "ru_phonetic" , new Dictionary<string, string>
                {
                    {"a","а"}, {"b","б"}, {"c","ц"}, {"d","д"}, {"e","е"}, {"f","ф"}, {"g","г"}, {"h","ч"}, {"i","и"},
                    {"j","й"}, {"k","к"}, {"l","л"}, {"m","м"}, {"n","н"}, {"o","о"}, {"p","п"}, {"q","я"}, {"r","р"}, {"s","с"},
                    {"t","т"}, {"u","у"}, {"v","в"}, {"w","ш"}, {"x","х"}, {"y","ы"}, {"z","з"},
                    {"A","А"}, {"B","Б"}, {"C","Ц"}, {"D","Д"}, {"E","Е"}, {"F","Ф"}, {"G","Г"}, {"H","Ч"}, {"I","И"},
                    {"J","Й"}, {"K","К"}, {"L","Л"}, {"M","М"}, {"N","Н"}, {"O","О"}, {"P","П"}, {"Q","Я"}, {"R","Р"}, {"S","С"},
                    {"T","Т"}, {"U","У"}, {"V","В"}, {"W","Ш"}, {"X","Х"}, {"Y","Ы"}, {"Z","З"},
                    // other rows in UPPERCASE and in LOWERCASE
                    {"1",""},  {"2",""},   {"3",""},  {"4",""},   {"5",""},   {"6",""},  {"7",""}, {"8",""},  {"9",""},  {"0",""},
                    {"!",""},  {"@",""},   {"#",""},  {"$","\""}, {"%",":"},  {"^",""},  {"&",""}, {"*",""},  {"(",""},  {")",""},

                    {"`","ё"}, {"-",""},  {"=","ъ"},
                    {"~","Ё"},  {"_",""}, {"+","Ъ"},

                    {"[","ю"},  {"]","щ"},  {"\\","э"},
                    {"{","Ю"},  {"}","Щ"},  {"|","Э"},

                    {";","ь"},  {"'","ж"},
                    {":","Ь"},  {"\"","Ж"},

                    {",",""},  {".",""}, {"/",""},
                    {"<",""},  {">",""}, {"?",""}
                }
            }
        };


        public string transliterate(bool to_or_from, string targetLang, string input)
        {
            string result = input;
            if (dicts.ContainsKey(targetLang+"_phonetic") )
            {
                foreach (KeyValuePair<string, string> allKeys in dicts[targetLang + "_phonetic"])
                {
                    if (allKeys.Value != "")
                    {
                        if (to_or_from)
                        {
                            result = result.Replace(allKeys.Key, allKeys.Value);
                        }
                        else
                        {
                            result = result.Replace(allKeys.Value, allKeys.Key);
                        }
                    }
                }
            }
            return result;
        }
    }
}
