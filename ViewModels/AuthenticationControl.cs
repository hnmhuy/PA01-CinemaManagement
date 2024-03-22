using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaManagement.ViewModels
{
    public class AuthenticationControl
    {
        public static string STORAGE_UID = "uid";
        public static string STORAGE_EXPIRED_DATE = "expired_date";

        static public (bool, int, string?) RestoreSession()
        {
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(STORAGE_UID))
            {
                var value = (
                    true, 
                    (int)Windows.Storage.ApplicationData.Current.LocalSettings.Values[STORAGE_UID],
                    (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values[STORAGE_EXPIRED_DATE]);

                Debug.WriteLine("Restored session: " + value.Item1);
                Debug.WriteLine("Restored session: " + value.Item2);
                return value;

            }
            return (false, -1, null);
        }

        static public void SaveSession(int uid)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values[STORAGE_UID] = uid;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values[STORAGE_EXPIRED_DATE] = DateTime.Now.AddHours(1).ToString();
        }   

        static public void DestroySession()
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove(STORAGE_UID);
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove(STORAGE_EXPIRED_DATE);
        }
    }
}
