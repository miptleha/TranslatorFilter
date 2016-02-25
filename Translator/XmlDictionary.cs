using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Xml;
using System.Xml.XPath;

namespace osai.Code.Translator
{
    public class XmlDictionary
    {
        public string Replace(string text, List<string> list)
        {
            Load();

            //replace strings using files
            List<string> keys = _ht.Keys.ToList();
            //long strings first
            keys.Sort((s1, s2) => { return -s1.Length.CompareTo(s2.Length); });
            foreach (var s in keys)
            {
                if (!string.IsNullOrWhiteSpace(_ht[s]) && _ht[s] != s)
                    text = Replace(text, s);
            }

            //add strings to file (if not added)
            foreach (var s in list)
            {
                Get(s);
            }

            Save();
            return text;
        }

        private string Replace(string text, string key)
        {
            string sNew = Get(key);
            if (sNew != null && sNew != key)
                text = text.Replace(key, sNew);
            return text;
        }

        Dictionary<string, string> _ht = new Dictionary<string, string>();
        const string _fileOriginal = "text_original.txt"; //auto generated file, add own strings to it
        const string _fileTranslated = "text_translated.txt"; //translate content of text_original.txt in google translate and paste translation in text_translated.txt
        const string _chars = "абвгдеёжзиклмнопрстуфхцчшщъыьэюя"; //string must contain at least one of this char, add characters from your charset (or comment comparison with _chars in code)

        public string Get(string s)
        {
            bool found = false;
            foreach (char c in s.ToLower())
            {
                if (_chars.Contains(c))
                {
                    found = true;
                    break;
                }
            }
            if (!found)
                return null; //comment here if want parse all strings, not only russian

            if (!_ht.ContainsKey(s))
            {
                _ht.Add(s, "");
                return null;
            }

            string output = _ht[s];
            if (string.IsNullOrWhiteSpace(output))
                return null;
            return output;
        }

        static ReaderWriterLock _lock = new ReaderWriterLock();

        public void Load()
        {
            _ht.Clear();

            _lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                string pathOriginal = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", _fileOriginal);
                if (!File.Exists(pathOriginal))
                    return;

                string pathTranslated = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", _fileTranslated);
                if (!File.Exists(pathTranslated))
                    return;

                var listOriginal = ReadFile(pathOriginal);
                var listTranslated = ReadFile(pathTranslated);
                for (int i = 0; i < listOriginal.Count; i++)
                {
                    if (listTranslated.Count > i)
                        _ht.Add(listOriginal[i], listTranslated[i]);
                }
            }
            finally
            {
                _lock.ReleaseReaderLock();
            }
        }

        public void Save()
        {
            _lock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                string pathOriginal = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", _fileOriginal);
                if (!File.Exists(pathOriginal))
                    File.Create(pathOriginal).Close();

                string pathTranslated = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", _fileTranslated);
                if (!File.Exists(pathTranslated))
                    File.Create(pathTranslated).Close();

                var listOriginal = new List<string>();
                var listTranslated = new List<string>();
                foreach (var key in _ht.Keys)
                {
                    listOriginal.Add(key);
                    listTranslated.Add(_ht[key]);
                }

                WriteFile(pathOriginal, listOriginal);
                WriteFile(pathTranslated, listTranslated);
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }

        private List<string> ReadFile(string path)
        {
            var list = new List<string>();
            using (var sr = new StreamReader(path))
            {
                string s = null;
                while ((s = sr.ReadLine()) != null)
                    list.Add(s.Trim());
            }
            return list;
        }

        private void WriteFile(string path, List<string> list)
        {
            using (var sw = new StreamWriter(path))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    sw.WriteLine(list[i]);
                }
            }
        }

    }
}