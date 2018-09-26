using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace FortuneLab.WebClient.Optimization.HttpCache
{
    /// <summary>
    /// 判断服务器上生成的内容是否有变化来确定是否给客户端返回内容
    /// </summary>
    public class ContentCacheFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;
            filterContext.HttpContext.Response.Filter =
                new ResponseWrapper(filterContext.HttpContext.Response.Filter,
                    filterContext.HttpContext.Request.Headers["If-None-Match"]);
        }
        private class ResponseWrapper : Stream
        {
            #region Fields
            private Stream _innerStream;
            private MemoryStream _memStream;
            private string _eTag = string.Empty;
            private bool _isCached = false;
            #endregion

            public Guid Key { get; private set; }

            #region Constructors
            public ResponseWrapper(Stream stream, string eTag)
            {
                this.Key = Guid.NewGuid();
                _innerStream = stream;
                _memStream = new MemoryStream();
                _eTag = eTag;
            }
            #endregion

            #region Properties
            public byte[] Data
            {
                get
                {
                    _memStream.Position = 0;
                    byte[] data = new byte[_memStream.Length];
                    _memStream.Read(data, 0, (int)_memStream.Length);
                    return data;
                }
            }
            #endregion

            #region overrides of Stream Class
            public override bool CanRead
            {
                get { return _innerStream.CanRead; }
            }

            public override bool CanSeek
            {
                get { return _innerStream.CanSeek; }
            }

            public override bool CanWrite
            {
                get { return _innerStream.CanWrite; }
            }

            public override void Flush()//可能会有这样一种情况：如果数据比较大则可能在未真正传输结束前就要Flush
            {
                if (_isCached) return;
                var httpContext = HttpContext.Current;
                string currentETag = generateETagValue(Data);
                if (_eTag != null)
                {
                    if (currentETag.Equals(_eTag))
                    {
                        httpContext.Response.StatusCode = 304;
                        httpContext.Response.StatusDescription = "Not Modified";
                        return;
                    }
                }
                httpContext.Response.Cache.SetCacheability(HttpCacheability.Public);
                httpContext.Response.Cache.SetETag(currentETag);
                httpContext.Response.Cache.SetLastModified(DateTime.Now);
                httpContext.Response.Cache.SetSlidingExpiration(true);
                copyStreamToStream(_memStream, _innerStream);
                _innerStream.Flush();
                _isCached = true;
            }

            public override long Length
            {
                get { return _innerStream.Length; }
            }

            public override long Position
            {
                get
                {
                    return _innerStream.Position;
                }
                set
                {
                    _innerStream.Position = value;
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return _innerStream.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return _innerStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                _innerStream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                //_innerStream.Write(buffer, offset, count);
                _memStream.Write(buffer, offset, count);
            }

            public override void Close()
            {
                _innerStream.Close();
            }
            #endregion

            #region private Helper Methods
            private void copyStreamToStream(Stream src, Stream target)
            {
                src.Position = 0;
                int nRead = 0;
                byte[] buf = new byte[128];
                while ((nRead = src.Read(buf, 0, 128)) != 0)
                {
                    target.Write(buf, 0, nRead);
                }
            }
            private string generateETagValue(byte[] data)
            {
                var encryptor = new System.Security.Cryptography.SHA1Managed();
                byte[] encryptedData = encryptor.ComputeHash(data);
                return Convert.ToBase64String(encryptedData);
            }
            #endregion
        }
    }

    public sealed class HeadResponseFilter : Stream
    {
        private const String TAG_HEAD_BEGIN = "<head";
        private const String TAG_HEAD_END = "</head>";

        private StringBuilder m_buffer;
        private Stream m_stream;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream">流</param>
        public HeadResponseFilter(Stream stream)
        {
            m_stream = stream;
            m_buffer = new StringBuilder();
        }

        /// <summary>
        /// 请求
        /// </summary>
        public HttpRequest Request
        {
            get
            {
                return HttpContext.Current.Request;
            }
        }

        /// <summary>
        /// 响应
        /// </summary>
        public HttpResponse Response
        {
            get
            {
                return HttpContext.Current.Response;
            }
        }

        /// <summary>
        /// 可读
        /// </summary>
        public override Boolean CanRead
        {
            get { return true; }
        }

        /// <summary>
        /// 可查询
        /// </summary>
        public override Boolean CanSeek
        {
            get { return true; }
        }

        /// <summary>
        /// 可写
        /// </summary>
        public override Boolean CanWrite
        {
            get { return true; }
        }

        /// <summary>
        /// 长度
        /// </summary>
        public override Int64 Length
        {
            get { return 0; }
        }

        /// <summary>
        /// 位置
        /// </summary>
        public override Int64 Position { get; set; }

        /// <summary>
        /// 刷新
        /// </summary>
        public override void Flush()
        {
            String rawHtml = m_buffer.ToString();
            Int32 headIndex = rawHtml.IndexOf(TAG_HEAD_BEGIN, StringComparison.CurrentCultureIgnoreCase);
            Int32 headIndex2 = rawHtml.IndexOf(TAG_HEAD_END, StringComparison.CurrentCultureIgnoreCase);

            if ((headIndex > 0) && (headIndex2 > 0) && (headIndex < headIndex2))
            {
                String rawHead = rawHtml.Substring(headIndex, headIndex2 - headIndex + TAG_HEAD_END.Length);
                XElement headElement = XElement.Parse(rawHead);
                IEnumerable<XElement> metaElement = headElement.Descendants("meta").Where(p => p.Attribute("http - equiv") != null);

                if (metaElement.Count() == 0)
                    headElement.Add(new XElement("meta", new XAttribute("http - equiv", "x - ua - compatible"), new XAttribute("content", "IE = 7")));

                rawHtml = String.Concat(rawHtml.Substring(0, headIndex), headElement.ToString(), rawHtml.Substring(headIndex + rawHead.Length));
            }

            this.Response.Write(rawHtml);
            this.Response.Flush();
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="buffer">缓冲区</param>
        /// <param name="offset">偏移量</param>
        /// <param name="count">数量</param>
        /// <returns>读取数量</returns>
        public override Int32 Read(Byte[] buffer, Int32 offset, Int32 count)
        {
            return m_stream.Read(buffer, offset, count);
        }

        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="offset">偏移量</param>
        /// <param name="origin">参考点</param>
        /// <returns>位置</returns>
        public override Int64 Seek(Int64 offset, SeekOrigin origin)
        {
            return m_stream.Seek(offset, origin);
        }

        /// <summary>
        /// 设置长度
        /// </summary>
        /// <param name="value">值</param>
        public override void SetLength(Int64 value)
        {
            m_stream.SetLength(value);
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="buffer">缓冲区</param>
        /// <param name="offset">偏移量</param>
        /// <param name="count">数量</param>
        public override void Write(Byte[] buffer, Int32 offset, Int32 count)
        {
            m_buffer.Append(HttpContext.Current.Response.ContentEncoding.GetString(buffer, offset, count));
        }
    }
}
