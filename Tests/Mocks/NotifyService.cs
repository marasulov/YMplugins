using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using YMplugins.Contracts;

namespace Mocks
{
    public class NotifyService : INotifyService
    {
        public void Notify(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
