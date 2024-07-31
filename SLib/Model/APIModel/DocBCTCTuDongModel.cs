using System.Collections.Generic;

namespace SLib.Model.APIModel
{
    public class BCTCFileUploadResponse
    {
        public string file { get; set; }
        public long size { get; set; }
        public string ctime { get; set; }
        public string host { get; set; }
        public string name { get; set; }
    }

    public class BCTCOCRResponse
    {
        public string cookieName { get; set; }
        public string cookieValue { get; set; }
        public string jobId { get; set; }
    }

    public class BCTCOCRInput
    {
        public List<BCTCFileUploadResponse> files { get; set; }
        public string langCode { get; set; } = "vie";
        public string outputType { get; set; } = "pdf";
        public string title { get; set; }
        public string author { get; set; }
        public string subject { get; set; }
        public string keywords { get; set; }
        public bool removeBackground { get; set; }
        public bool rotatePages { get; set; }
        public bool deskew { get; set; } = true;
        public bool clean { get; set; }
        public bool forceOcr { get; set; }
        public bool joinFiles { get; set; }
    }

    public class BCTCStatusResponse
    {
        public string cookieName { get; set; }
        public string cookieValue { get; set; }
        public string jobId { get; set; }
        public string status { get; set; }
    }
}
