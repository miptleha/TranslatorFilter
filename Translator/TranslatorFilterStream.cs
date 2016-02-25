using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace osai.Code.Translator
{
    public class TranslatorFilterStream : Stream
    // This filter changes all characters passed through it to uppercase.
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

        public override void Write(byte[] buffer, int offset, int count)
        {
            byte[] data = new byte[count];
            Buffer.BlockCopy(buffer, offset, data, 0, count);
            tmpBuffer.AddRange(data);
        }

        public override void Flush()
        {
            //string html = "<div><div>text1</div>text2</div>";
            //string html = "<nav class=\"navbar navbar-default navbar-fixed-top\" role=\"navigation\" style=\"margin-bottom: 0\"><div class=\"navbar-header\"><div id=\"header-logo\" title=\"НА ВНЕШНИЙ ПОРТАЛ АСУ ТК\"><img src=\"/Images/arm.png\" /></div><div id=\"header-title\">Информационно-аналитическая система регулирования на транспорте<br /><span class=\"sub-title1\">Подсистема обеспечения справочной и аналитической информацией</span><br /><span class=\"sub-title2\">АРМ Аналитика</span></div><div id=\"header-right\"><div id=\"header-usericon\">Аналитик<br/><a href=\"http://localhost:55747/\" title=\"НА ВНУТРЕННИЙ ПОРТАЛ АСУ ТК\">ВЫХОД В ЛИЧНЫЙ КАБИНЕТ</a></div></div></div></nav>";
            //string html = "[[{\"a\":[\"b\"]}, \"c\":\"d\", [1, \"e\"]], [{\"a\":[\"b2\"]}, \"c\":\"d2\", [1, \"e2\"]]]";
            string html = Encoding.UTF8.GetString(tmpBuffer.ToArray());

            var tp = new TranslatorParser();
            var list = new List<string>();
            if (html.Length > 0 && (html[0] == '{' || html[0] == '['))
                tp.FindTextJson(html, list);
            else
                tp.FindTextHtml(html, list);
            list = tp.FilterText(list);

            var dict = new XmlDictionary();
            string htmlNew = dict.Replace(html, list);

            var data = Encoding.UTF8.GetBytes(htmlNew);
            strSink.Write(data, 0, data.Length);
            strSink.Flush();
        }
    }
}