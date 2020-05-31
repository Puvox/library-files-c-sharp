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

//using System.ComponentModel.DataAnnotations;
//using System.Web.Script.Serialization;
//using System.Windows.Controls;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;


namespace PuvoxLibrary
{

    public partial class Methods 
    {
         
        public static Color color_from_hex(string hex_str) {
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


       

    }


}
 