using System;
using System.Linq;
using System.Collections.Generic;


public class HtmlParse{
    private List<Token> tokens;
    private ParseOptions options;
    private Stack<Token> tagStack;
    private List<HtmlNode> nodes;
    private List<bool> usedTokens;

    public HtmlParse(){
        this.tokens = new List<Token>();
        this.options = new ParseOptions();
        this.tagStack = new Stack<Token>();
        this.nodes = new List<HtmlNode>();
        this.usedTokens = new List<bool>();
    }

    public void Parse(){
        int i = 0;

        while(i < tokens.Count){
            
            i++;
        } 
    }

    public void Parser(List<Token> t, ParseOptions o){
        Console.WriteLine("Parsing tokens...\n");
        this.tokens = t;
        this.usedTokens = Enumerable.Repeat(false, tokens.Count).ToList();
        this.options = o;
        Parse();

        Console.WriteLine("\n------tagStack:------");
        while(tagStack.Count > 0){
            Console.WriteLine(tagStack.Pop());
        }
        if(tagStack.Count == 0){
            Console.WriteLine("\nNothing is left on the tag stack.");
        }

        Console.WriteLine("\n------Node List:------");
        Console.WriteLine("[");
        for(int i = 0; i < nodes.Count; i++){
            Console.WriteLine(nodes[i]);
        }
        Console.WriteLine("]");
    }
}