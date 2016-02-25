# TranslatorFilter
Automatic translation of ASP.NET application

### ASP.NET MVC application translation
 
 * Add Translator folder to project
 * Add attribute 'TranslatorFilter' to Controller to be translated:
```csharp
[TranslatorFilter]
public class CommonController : Controller
{
}
```
 * Run application, it generates text_original.txt and text_translated.txt in App_Data
 * Translate content of text_original.txt in google translate and paste in text_translated.txt (remove extra spaces inserted by Google)
 * Run application again, text will be translated
 * If some strings not translated add them in the begining of text_original.txt and text_translated.txt

### ASP.NET classic application or ASP.NET MVC global translation

* Remove TranslatorFilter.cs
* Attach filter in Global.asax:
```csharp
public class Global : System.Web.HttpApplication
{
    public Global()
    {
       this.PostRequestHandlerExecute += Global_PostRequestHandlerExecute;
    }

    void Global_PostRequestHandlerExecute(object sender, EventArgs e)
    {
        Response.Filter = new TranslatorFilterStream(Response.Filter);
    }
}
```

Feel free to use TranslatorFilter in your projects :+1:
