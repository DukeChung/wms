using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FortuneLab.WebApiClient
{
    public class FileUpload
    {
        public FileUpload(string originalFileName, string contentType, Stream stream)
        {
            OriginalFileName = originalFileName;
            ContentType = contentType;
            Stream = stream;
        }

        public string NewFileName { get; set; }
        public string OriginalFileName { get; set; }
        public string ContentType { get; set; }
        public Stream Stream { get; set; }
        public int FileType { get; set; }

        public static IEnumerable<FileUpload> GetUploadFiles(HttpRequestBase request)
        {
            foreach (string uploadFile in request.Files)
            {
                var file = request.Files[uploadFile];

                if (!(file != null && file.ContentLength > 0)) continue;

                yield return new FileUpload(file.FileName, file.ContentType, file.InputStream); ;
            }
        }
    }

    public class FileDownload
    {
        public Stream Stream { get; set; }
        public HttpResponseHeaders ResponseHeaders { get; set; }
        public HttpContentHeaders ContentHeader { get; set; }

        /// <summary>
        /// 从ResponseHeaders中解析文件扩展名
        /// </summary>
        public string FileExtension
        {
            get
            {
                return ResponseHeaders.Contains("File-Extension") ? ResponseHeaders.GetValues("File-Extension").SingleOrDefault() : string.Empty;
            }
        }

        /// <summary>
        /// 从ResponseHeaders中解析文件名
        /// </summary>
        public string FileName
        {
            get
            {
                return ResponseHeaders.Contains("File-Name") ? ResponseHeaders.GetValues("File-Name").SingleOrDefault() : string.Empty;
            }
        }
    }
}
