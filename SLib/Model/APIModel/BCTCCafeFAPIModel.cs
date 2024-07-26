using System.IO;

namespace SLib.Model.APIModel
{
    public class BCTCCafeFAPIModel
    {
        public int Year { get; set; }
        public int Quarter { get; set; }
        public Stream Source { get; set; }
    }
}
