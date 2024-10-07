using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace YMplugins.Contracts.Dto
{
    public class PrintInfo : INotifyPropertyChanged
    {
        private bool _isPrint;
        public long ObjectId { get; set; }
        public string Space { get; set; }
        public string Format { get; set; }
        public string FileName { get; set; }
        public PointDTO Position { get; set; }
        public PointDTO Position2 { get; set; }
        public double YDmim { get; set; }
        public double XDim { get; set; }
        public double ScaleX { get; set; }

        public bool IsPrint
        {
            get => _isPrint;
            set
            {
                _isPrint = value;
                OnPropertyChanged();
            }
        }

        //public bool IsCheckedNumbering { get; set; }

        public PrintInfo(long objectId, string space, string format, double scaleX, double xDim, double yDmim, PointDTO position, bool isPrint, string fileName = "")
        {
            ObjectId = objectId;
            Space = space;
            Format = format;
            ScaleX = scaleX;
            XDim = xDim;
            YDmim = yDmim;
            Position = position;
            IsPrint = isPrint;
            FileName = fileName;
        }

        public bool IsFormatHorizontal()
        {
            return XDim > YDmim;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}