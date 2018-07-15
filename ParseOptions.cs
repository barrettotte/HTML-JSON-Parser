using System;
using System.Collections.Generic;

public struct PreventAutoClose{
    private string tagName;
    private string[] tags;
    public PreventAutoClose(string tn, string[] ts){
        tagName = tn;
        tags = ts;
    }
}

public class ParseOptions{
    private readonly string[] childlessTags;
    public string[] ChildlessTags{ get{ return this.childlessTags; }}
    private readonly string[] closingTags;
    public string[] ClosingTags{ get{ return this.closingTags; }}
    private readonly List<PreventAutoClose> preventAutoCloseTags;
    public List<PreventAutoClose> PreventAutoCloseTags{ get{ return this.preventAutoCloseTags; }}
    private readonly string[] noClosingTags;
    public string[] NoClosingTags{ get{ return this.noClosingTags; }}

    public ParseOptions(){
        this.childlessTags = new string[] {"style", "script", "template"};
        this.closingTags = new string[] {
            "html", "head", "body", "p", "dt", "dd", "li", "option", "thead", "th", "tbody", 
            "tr", "td", "tfoot", "colgroup"
        };
        this.preventAutoCloseTags = new List<PreventAutoClose>();
        SetupPreventAutoCloseTags(new string[]{"li", "dt", "dd", "tbody", "thead", "tfoot", "tr", "td"});
        this.noClosingTags = new string[] {"!doctype", "area", "base", "br", "col", "command",
            "embed", "hr", "img", "input", "keygen", "link", "meta", "param", "source", "track", "wbr"
        };
    }

    private void SetupPreventAutoCloseTags(string[] tagNames){
        string[] breakers;
        foreach(string name in tagNames){
            switch(name){
                case "li":      breakers = new string[] {"ul", "ol", "menu"};   break;
                case "dt":      breakers = new string[] {"dl"};                 break;
                case "dd":      breakers = new string[] {"dl"};                 break;
                case "tbody":   breakers = new string[] {"table"};              break;
                case "thead":   breakers = new string[] {"table"};              break;
                case "tfoot":   breakers = new string[] {"table"};              break;
                case "tr":      breakers = new string[] {"table"};              break;
                case "td":      breakers = new string[] {"table"};              break;
                default:        return;
            }
            this.preventAutoCloseTags.Add(new PreventAutoClose(name, breakers));
        }
    }
}