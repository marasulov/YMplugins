using YMplugins.Contracts.Dto.Enums;

namespace YMplugins.Contracts.Dto
{
    public class PrintInfo
    {
        public int ObjectId { get; set; }
        public string Space { get; set; }  
        public string Format { get; set; }
        public string FileName { get; set; }

        public PrintInfo(int objectId, string space, string format, string fileName)
        {
            ObjectId = objectId;
            Space = space;
            Format = format;
            FileName = fileName;
        }
    }
}