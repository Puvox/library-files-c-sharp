
/*
using System;
using System.Runtime.InteropServices;
using System.Text;
 
namespace Read64bitRegistryFrom32bitApp
{
    public enum RegSAM
    {
        QueryValue = 0x0001,
        SetValue = 0x0002,
        CreateSubKey = 0x0004,
        EnumerateSubKeys = 0x0008,
        Notify = 0x0010,
        CreateLink = 0x0020,
        WOW64_32Key = 0x0200,
        WOW64_64Key = 0x0100,
        WOW64_Res = 0x0300,
        Read = 0x00020019,
        Write = 0x00020006,
        Execute = 0x00020019,
        AllAccess = 0x000f003f
    }
 
    public static class RegHive
    {
        public static UIntPtr HKEY_LOCAL_MACHINE = new UIntPtr(0x80000002u);
        public static UIntPtr HKEY_CURRENT_USER = new UIntPtr(0x80000001u);
    }
 
    public static class RegistryWOW6432
    {
        #region Member Variables
        #region Read 64bit Reg from 32bit app
        [DllImport("Advapi32.dll")]
        static extern uint RegOpenKeyEx(
            UIntPtr hKey,
            string lpSubKey,
            uint ulOptions,
            int samDesired,
            out int phkResult);
 
        [DllImport("Advapi32.dll")]
        static extern uint RegCloseKey(int hKey);
 
        [DllImport("advapi32.dll", EntryPoint = "RegQueryValueEx")]
        public static extern int RegQueryValueEx(
            int hKey, string lpValueName,
            int lpReserved,
            ref uint lpType,
            System.Text.StringBuilder lpData,
            ref uint lpcbData);
        #endregion
        #endregion
 
        #region Functions
        static public string GetRegKey64(UIntPtr inHive, String inKeyName, String inPropertyName)
        {
            return GetRegKey64(inHive, inKeyName, RegSAM.WOW64_64Key, inPropertyName);
        }
 
        static public string GetRegKey32(UIntPtr inHive, String inKeyName, String inPropertyName)
        {
            return GetRegKey64(inHive, inKeyName, RegSAM.WOW64_32Key, inPropertyName);
        }
 
        static public string GetRegKey64(UIntPtr inHive, String inKeyName, RegSAM in32or64key, String inPropertyName)
        {
            //UIntPtr HKEY_LOCAL_MACHINE = (UIntPtr)0x80000002;
            int hkey = 0;
 
            try
            {
                uint lResult = RegOpenKeyEx(RegHive.HKEY_LOCAL_MACHINE, inKeyName, 0, (int)RegSAM.QueryValue | (int)in32or64key, out hkey);
                if (0 != lResult) return null;
                uint lpType = 0;
                uint lpcbData = 1024;
                StringBuilder AgeBuffer = new StringBuilder(1024);
                RegQueryValueEx(hkey, inPropertyName, 0, ref lpType, AgeBuffer, ref lpcbData);
                string Age = AgeBuffer.ToString();
                return Age;
            }
            finally
            {
                if (0 != hkey) RegCloseKey(hkey);
            }
        }
        #endregion
 
        #region Enums
        #endregion
    }
}





// =====  usage : 

using System;
using System.Runtime.InteropServices;
using System.Text;
 
namespace Read64bitRegistryFrom32bitApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string value64 = RegistryWOW6432.GetRegKey64(RegHive.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "RegisteredOrganization");
            string value32 = RegistryWOW6432.GetRegKey32(RegHive.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "RegisteredOrganization");
        }
    }
}
*/


/*


public static class RegistryExtensions
{

    public enum RegistryHiveType
    {
        X86,
        X64
    }

    static Dictionary<RegistryHive, UIntPtr> _hiveKeys = new Dictionary<RegistryHive, UIntPtr> 
    {
        { RegistryHive.ClassesRoot, new UIntPtr(0x80000000u) },
        { RegistryHive.CurrentConfig, new UIntPtr(0x80000005u) },
        { RegistryHive.CurrentUser, new UIntPtr(0x80000001u) },
        { RegistryHive.DynData, new UIntPtr(0x80000006u) },
        { RegistryHive.LocalMachine, new UIntPtr(0x80000002u) },
        { RegistryHive.PerformanceData, new UIntPtr(0x80000004u) },
        { RegistryHive.Users, new UIntPtr(0x80000003u) }
    };

    static Dictionary<RegistryHiveType, RegistryAccessMask> _accessMasks = new Dictionary<RegistryHiveType, RegistryAccessMask> 
    {
        { RegistryHiveType.X64, RegistryAccessMask.Wow6464 },
        { RegistryHiveType.X86, RegistryAccessMask.WoW6432 }
    };

    [Flags]
    public enum RegistryAccessMask
    {
        QueryValue          = 0x0001,
        SetValue            = 0x0002,
        CreateSubKey        = 0x0004,
        EnumerateSubKeys    = 0x0008,
        Notify              = 0x0010,
        CreateLink          = 0x0020,
        WoW6432             = 0x0200,
        Wow6464             = 0x0100,
        Write               = 0x20006,
        Read                = 0x20019,
        Execute             = 0x20019,
        AllAccess           = 0xF003F
    }

    [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
    public static extern int RegOpenKeyEx
    (
      UIntPtr hKey,
      string subKey,
      uint ulOptions,
      uint samDesired,
      out IntPtr hkResult
    );

      public static RegistryKey OpenBaseKey(RegistryHive registryHive, RegistryHiveType registryType)
    {
        int result = -1;
        UIntPtr hiveKey = _hiveKeys[registryHive];
        if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major > 5)
        {
            RegistryAccessMask flags = RegistryAccessMask.QueryValue | RegistryAccessMask.EnumerateSubKeys | RegistryAccessMask.Read;
            IntPtr keyHandlePointer = IntPtr.Zero;
            result = RegOpenKeyEx(hiveKey, String.Empty, 0, (uint) flags, out keyHandlePointer);
            if (result == 0)
            {
                var safeRegistryHandleType = typeof (SafeHandleZeroOrMinusOneIsInvalid).Assembly.GetType("Microsoft.Win32.SafeHandles.SafeRegistryHandle");
                var safeRegistryHandleConstructor = safeRegistryHandleType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] {typeof (IntPtr), typeof (bool)}, null); // .NET < 4
                if (safeRegistryHandleConstructor == null)
                    safeRegistryHandleConstructor = safeRegistryHandleType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] {typeof (IntPtr), typeof (bool)}, null); // .NET >= 4
                var keyHandle = safeRegistryHandleConstructor.Invoke(new object[] {keyHandlePointer, true});
                var net3Constructor = typeof (RegistryKey).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] {safeRegistryHandleType, typeof (bool)}, null);
                var net4Constructor = typeof (RegistryKey).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] {typeof (IntPtr), typeof (bool), typeof (bool), typeof (bool), typeof (bool)}, null);
                object key;
                if (net4Constructor != null)
                    key = net4Constructor.Invoke(new object[] {keyHandlePointer, true, false, false, hiveKey == _hiveKeys[RegistryHive.PerformanceData]});
                else if (net3Constructor != null)
                    key = net3Constructor.Invoke(new object[] {keyHandle, true});
                else
                {
                    var keyFromHandleMethod = typeof (RegistryKey).GetMethod("FromHandle", BindingFlags.Static | BindingFlags.Public, null, new[] {safeRegistryHandleType}, null);
                    key = keyFromHandleMethod.Invoke(null, new object[] {keyHandlePointer});
                }
                var field = typeof (RegistryKey).GetField("keyName", BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                    field.SetValue(key, String.Empty);
                return (RegistryKey) key;
            }
            else if (result == 2) // The key does not exist.
                return null;
            throw new Win32Exception(result);
        }
        return null;
    }
}
Example of usage:

var key64 = RegistryExtensions.OpenBaseKey(RegistryHive.LocalMachine, RegistryExtensions.RegistryHiveType.X64);
var keyPath = @"Software\\Bentley\\AutoPIPE\\V8i Ribbon and Reporting";
var key = key64.OpenSubKey(keyPath);



*/