using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LodgeCommunication
{
    public class LogsManager : BindingList<Log>
    {

        private static LogsManager instance = new LogsManager();
        private static readonly object padlock = new object();

        public static event Action<Log> OnLogAdd;
        public static int LogMax = 100;

        public static BindingList<Log> GetLogs() { return Instance; }

        LogsManager() { }

        private static LogsManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new LogsManager();
                    }
                    return instance;
                }
            }
        }

        public static void Add(EnumCategory category, string type, string description)
        {

            Log log = new Log(DateTime.Now.ToString(), category, type, description);

            instance.Add(log);

            OnLogAdd?.Invoke(log);

            if (LogMax > 0)
            {
                if (instance.Count > LogMax)
                {
                    instance.RemoveAt(0);
                }
            }
        }

        public static new void Clear()
        {
            for (int i = Instance.Count - 1; i >= 0; i--)
            {
                Instance.RemoveAt(i);
            }

        }

    }
}
