TranslatorFilter
================

Automatic translation of ASP.NET application. Not only from one language to
other. See sample application that replaces all strings to [n] text.

Text in web application will be translated using files: text_original.txt
(created automatically when navigating running application) and
text_translated.txt. This files located in App_Data folder.

To translate text in application: open text_original.txt, copy its content to
google translate, copy translation to text_translated.txt (save as UTF-8) and
run application again.

If you want translate only strings, that contains symbols from specific alphabet
(for example Russian) open XmlDictionary.cs at line 51 and set your alphabet,
then uncomment lines 56, 65 and 66. Currently program translate all text.

Content of project
------------------

-   Translator - Translator filter code

-   Mvc - Sample ASP.NET MVC application with filter

-   Classic - Sample ASP.NET Web Forms application with filter

-   packages - Visual Studio will automatically download all library to compile
    solution

Open TranslatorFilter.sln file and run Mvc or Classic project.

How to use in project
---------------------

### ASP.NET MVC application

-   Add Translator folder files to project (or add reference to Translator.dll)

-   Add attribute 'TranslatorFilter' to Controller to be translated:

    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ csharp
    [TranslatorFilter]
    public class CommonController : Controller
    {
    }
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

-   Run application, it generates text_original.txt and text_translated.txt in
    App_Data

-   Translate content of text_original.txt in google translate and paste in
    text_translated.txt (remove extra spaces inserted by Google)

-   Run application again, text will be translated

-   If some strings not translated add them in the begining of text_original.txt
    and text_translated.txt

### ASP.NET classic application or ASP.NET MVC global translation

-   Add Translator folder files to project (or add reference to Translator.dll)

-   Attach filter in Global.asax:

    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ csharp
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
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
