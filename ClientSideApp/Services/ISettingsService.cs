using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.Services
{
    public interface ISettingsService
    {
        string Host { get; set; }
        int Port { get; set; }
    }
}
