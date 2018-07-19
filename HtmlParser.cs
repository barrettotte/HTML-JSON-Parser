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
        this.tokens = t;
        Console.WriteLine("Parsing tokens...");

        Stack<Token> tagStartStack = new Stack<Token>();
        Stack<Token> tokenStack = new Stack<Token>();

        bool isClosingTag;
        for(int i = 0; i < tokens.Count; i++){
            if(tokens[i].Type == "tag-start" && Boolean.TryParse(tokens[i].Content, out isClosingTag)){
                if(isClosingTag){
                    Console.WriteLine("Found tag end");    
                } else{
                    Console.WriteLine("Found tag start");    
                }
            }
        }
    }



}