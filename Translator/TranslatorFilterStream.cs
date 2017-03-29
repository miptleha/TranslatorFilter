using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Translator
{
    public class TranslatorFilterStream : Stream
    {
        List<byte> tmpBuffer = new List<byte>();
        private Stream strSink;
        private long lngPosition;

        public TranslatorFilterStream(Stream sink)
        {
            strSink = sink;
        }

        // The following members of Stream must be overriden.
        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { return 0; }
        }

        public override long Position
        {
            get { return lngPosition; }
            set { lngPosition = value; }
        }

        public override long Seek(long offset, System.IO.SeekOrigin direction)
        {
            return strSink.Seek(offset, direction);
        }

        public override void SetLength(long length)
        {
            strSink.SetLength(length);
        }

        public override void Close()
        {
            strSink.Close();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return strSink.Read(buffer, offset, count);
        }

        bool _wasFlush = false;

        public override void Write(byte[] buffer, int offset, int count)
        {
            byte[] data = new byte[count];
            Buffer.BlockCopy(buffer, offset, data, 0, count);
            tmpBuffer.AddRange(data);
            string html = Encoding.UTF8.GetString(tmpBuffer.ToArray());
            _wasFlush = false;
        }

        public override void Flush()
        {
            if (!_wasFlush)
            {
                string html = Encoding.UTF8.GetString(tmpBuffer.ToArray());

                var tp = new TranslatorParser();
                var list = new List<string>();
                if (html.Length > 0 && (html[0] == '{' || html[0] == '['))
                    tp.FindTextJson(html, list);
                else
                    html = tp.FindTextHtml(html, list);
                list = tp.FilterText(list);

                var dict = new XmlDictionary();
                string htmlNew = dict.Replace(html, list);

                var data = Encoding.UTF8.GetBytes(htmlNew);
                strSink.Write(data, 0, data.Length);
                _wasFlush = true;
            }
            strSink.Flush();
        }
    }
}