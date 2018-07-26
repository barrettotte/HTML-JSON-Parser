using System;
using System.Collections.Generic;



public class ParseOptions{
    private readonly string[] childlessTags;
    public string[] ChildlessTags{ get{ return this.childlessTags; }}
    private readonly string[] noClosingTags;
    public string[] NoClosingTags{ get{ return this.noClosingTags; }}

    public ParseOptions(){
        this.childlessTags = new string[] {"style", "script", "template"};
        this.noClosingTags = new string[] {"!doctype", "area", "base", "br", "col", "command",
            "embed", "hr", "img", "input", "keygen", "link", "meta", "param", "source", "track", "wbr"
        };
    }
}