using System;
using System.Collections.Generic;


public class HtmlParse{

    private List<Token> tokens;
    public List<Token> Tokens{
        get{ return this.tokens; }
    }

    public HtmlParse(){
        this.tokens = new List<Token>();
    }

    public void Parse(List<Token> t){
        
    }

}