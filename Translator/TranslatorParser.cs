using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Translator
{
    public class TranslatorParser
    {
        //parse json
        public void FindTextJson(string json, List<string> list)
        {
            json = ReplaceChars(json);
            FindTextJson2(json, list);
        }

        private void FindTextJson2(string json, List<string> list)
        {
            var index1 = json.IndexOf('[');
            var index2 = json.IndexOf('{');
            if (index1 != -1 || index2 != -1)
            {
                var texts = FindTextBound(json, new char[] { '[', '{' }, new char[] { ']', '}' });
                foreach (var text in texts)
                {
                    FindTextJson2(text, list);
                }
            }

            if (!string.IsNullOrWhiteSpace(json) && index1 == -1 && index2 == -1)
            {
                var items = json.Split(',');
                foreach (var item in items)
                {
                    if (item.Length > 0)
                    {
                        var item2 = ReturnChars(item);
                        var subitems = item2.Split(':');
                        if (subitems.Length > 1)
                            AddStringItem(subitems[1], list);
                        else
                            AddStringItem(item2, list);
                    }
                }
            }
        }

        //parse html
        public string FindTextHtml(string html, List<string> list)
        {
            var ms = Regex.Matches(html, @">([^<>]+)<", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            List<string> badStrings = new List<string>();
            foreach (Match m in ms)
            {
                var text = m.Groups[1].Value;
                text = text.Replace('\r', ' ').Replace('\n', ' ').Trim();
                if (text.Length == 0)
                    continue;
                list.Add(text);
            }

            string htmlNew = Regex.Replace(html, @">([^<>]+)<", delegate (Match m)
                {
                    var text = m.Groups[1].Value;
                    text = text.Replace('\r', ' ').Replace('\n', ' ').Trim();
                    if (text.Length == 0)
                        return m.Groups[0].Value;
                    return ">#(#" + text + "#)#<";
                }, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            return htmlNew;
        }

        //decode, remove dublicates and empties
        public List<string> FilterText(List<string> list)
        {
            var list2 = new List<string>();
            foreach (string text in list)
            {
                var text2 = HttpUtility.HtmlDecode(text).Trim();
                if (text2.Length != 0 && text2 != "<!--" && text2 != "-->" && !list2.Contains(text2))
                    list2.Add(text2);
            }
            return list2;
        }

        private string ReplaceChars(string json)
        {
            json = json.Replace("\\\"", "$$QUOTE$$");
            var jsonItems = json.Split('"');
            string json2 = "";
            for (int i = 0; i < jsonItems.Length; i++)
            {
                if (i % 2 == 0)
                    json2 += jsonItems[i];
                else
                    json2 += jsonItems[i].Replace(",", "$$COMMA$$").Replace("{", "$$OPEN1$$").Replace("}", "$$CLOSE1$$").Replace("[", "$$OPEN2$$").Replace("]", "$$CLOSE2$$").Replace(":", "$$COLON$$");
                if (i != jsonItems.Length - 1)
                    json2 += "\"";
            }
            return json2;
        }

        private string ReturnChars(string item)
        {
            return item.Replace("$$COMMA$$", ",").Replace("$$OPEN1$$", "{").Replace("$$CLOSE1$$", "}").Replace("$$OPEN2$$", "[").Replace("$$CLOSE2$$", "]").Replace("$$COLON$$", ":").Replace("$$QUOTE$$", "\\\"");
        }

        private void AddStringItem(string item, List<string> list)
        {
            var item1 = item.Trim();
            if (item1.Length > 0 && item1[0] == '"')
            {
                var item2 = item1.Trim('"');
                var html = UnicodeToCharacter(item2);
                if (item2 != html)
                    FindTextHtml(html, list);
                else
                    list.Add(item2);
            }
        }

        private string UnicodeToCharacter(string inStr)
        {
            string result = inStr;
            if (Regex.Match(inStr, @".*\\u.*", RegexOptions.IgnoreCase).Success)
            {
                Regex rx = new Regex(@"\\[uU]([0-9A-F]{4})", RegexOptions.IgnoreCase);
                result = rx.Replace(result, match => { 
                    return ((char)Int32.Parse(match.Value.Substring(2), NumberStyles.HexNumber)).ToString(); 
                });
            }
            return result;
        }

        private List<string> FindTextBound(string text, char[] start, char[] end)
        {
            int index = -1;
            int count = 0;
            int startPos = -1;
            int endPos = -1;
            var list = new List<string>();
            for (int i = 0; i < text.Length; i++)
            {
                var index1 = FindChar(text[i], start);
                if (index1 != -1)
                {
                    if (index == -1)
                    {
                        index = index1;
                        startPos = i;
                        if (startPos - endPos - 1 > 0)
                            list.Add(text.Substring(endPos + 1, startPos - endPos - 1));
                        count++;
                    }
                    else if (index1 == index)
                    {
                        count++;
                    }
                }

                index1 = FindChar(text[i], end);
                if (index1 != -1 && index == index1)
                {
                    count--;
                    if (count == 0)
                    {
                        endPos = i;
                        if (endPos - startPos - 1 > 0)
                            list.Add(text.Substring(startPos + 1, endPos - startPos - 1));
                        index = -1;
                    }
                }
            }
            if (endPos > 0 && endPos + 1 < text.Length)
                list.Add(text.Substring(endPos + 1));
            return list;
        }

        private int FindChar(char c, char[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (c == list[i])
                    return i;
            }
            return -1;
        }
    }
}