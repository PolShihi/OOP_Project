using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.Services
{
    public class SettingsService : ISettingsService
    {
        public string Host
        {
            get => Preferences.Get("Host", "localhost");
            set => Preferences.Set("Host", value);
        }
        public int Port
        {
            get => Preferences.Get("Port", 7200);
            set => Preferences.Set("Port", value);
        }
    }
}
