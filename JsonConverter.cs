using System;
using System.Linq;
using System.Web.Script.Serialization;

public class JsonConverter{

    public JsonConverter(){}

    public string ConvertHtml(HtmlNode root){
        return BeautifyJson(ConvertToJsonString(root));
    }

    private string ConvertToJsonString(HtmlNode root){
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        return serializer.Serialize(root); 
    }

    /*  
        I got lazy and didnt want to have to reinvent the wheel or import a whole library (Newtonsoft/JSON.NET).
        I already wasted a lot of time trying to make my own serializer/beautifier so im moving on.
        I should probably look into what that big group of code actually does, but im satisified with its output.
        Source: https://stackoverflow.com/questions/4580397/json-formatter-in-c     <-- Stack Overflow of course, idk how to code actually
    */
    private string BeautifyJson(string json) {
        const string INDENT_STRING = "   ";
        int indentation = 0;
        int quoteCount = 0;
        var result = 
            from ch in json
            let quotes = ch == '"' ? quoteCount++ : quoteCount
            let lineBreak = ch == ',' && quotes % 2 == 0 ? ch + Environment.NewLine +  String.Concat(Enumerable.Repeat(INDENT_STRING, indentation)) : null
            let openChar = ch == '{' || ch == '[' ? ch + Environment.NewLine + String.Concat(Enumerable.Repeat(INDENT_STRING, ++indentation)) : ch.ToString()
            let closeChar = ch == '}' || ch == ']' ? Environment.NewLine + String.Concat(Enumerable.Repeat(INDENT_STRING, --indentation)) + ch : ch.ToString()
            select lineBreak == null ? openChar.Length > 1 ? openChar : closeChar : lineBreak;
        return String.Concat(result);
    }
}