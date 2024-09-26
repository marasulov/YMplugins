using YMplugins.Contracts.Dto.Enums;

namespace YMplugins.Contracts.Dto
{
    public class PrintInfo
    {
        public long ObjectId { get; set; }
        public string Space { get; set; }  
        public string Format { get; set; }
        public string FileName { get; set; }
        public PointDTO Position { get; set; }
        public PointDTO Dimension { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double ScaleX { get; set; }

        public PrintInfo(long objectId, string space, string format, PointDTO dimension,  double scaleX, PointDTO position, string fileName  = "")
        {
            ObjectId = objectId;
            Space = space;
            Format = format;
            Dimension = dimension;
            ScaleX = scaleX;
            Position = position;
            FileName = fileName;
        }

        public bool IsFormatHorizontal()
        {
            return Width > Height;
        }
    }
}