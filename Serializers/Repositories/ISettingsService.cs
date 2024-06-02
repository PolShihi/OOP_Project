using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serializers.Repositories
{
    public interface ISettingsService
    {
        string Host { get; set; }
        int Port { get; set; }
    }
}
