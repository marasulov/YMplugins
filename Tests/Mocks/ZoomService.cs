using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using YMplugins.Contracts;

namespace Mocks
{
    public class ZoomService : IZoomEntity
    {
        public void Zoom(int id, string fileName)
        {
            Debug.Print($"zoom to {id} - {fileName}");
        }
    }
}
