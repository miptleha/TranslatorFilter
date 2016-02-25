# TranslatorFilter
Automatic translation of ASP.NET application

# ASP.NET MVC application translation

1. Add Translator folder to project
2. Add attribute to Controller to be translated:
    [TranslatorFilter]
    public class CommonController : Controller
    {
    }
3. Run application, it generates text_original.txt and text_translated.txt
4. Translate text_original.txt in google translate and paste in text_translated.txt (remove extra spaces inserted by Google)
5. Run application again. It will be translated.
6. If some strings not translated add them in text_original.txt and text_translated.txt


# ASP.NET classic application translation

The same but 
1. Remove TranslatorFilter.cs
2. Attach filter in Global.asax:

    public class Global : System.Web.HttpApplication
    {
        public Global()
        {
            //this.PostRequestHandlerExecute += Global_PostRequestHandlerExecute;
        }

        void Global_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            Response.Filter = new TranslatorFilterStream(Response.Filter);
        }
    }