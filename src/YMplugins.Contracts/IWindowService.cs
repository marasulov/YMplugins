using System;
using System.Collections.Generic;
using System.Text;

namespace YMplugins.Contracts
{
    public interface IWindowService
    {
        void ShowLoadingWindow();
        void CloseLoadingWindow();
    }
}
