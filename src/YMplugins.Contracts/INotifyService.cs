using System;
using System.Collections.Generic;
using System.Text;

namespace YMplugins.Contracts
{
    public interface INotifyService
    {
        void Notify(string message);
        void Notify(string message, string title);
    }
}
