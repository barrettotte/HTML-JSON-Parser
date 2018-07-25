using System;
using System.Linq;
using System.Collections.Generic;


public class HtmlParse{
    private List<Token> tokens;
    private ParseOptions options;

    public HtmlParse(){
        this.tokens = new List<Token>();
        this.options = new ParseOptions();
    }

    public HtmlNode Parser(List<Token> t, ParseOptions o){
        Console.WriteLine("Parsing tokens...\n");
        this.tokens = t;
        this.options = o;
        return Parse();
    }

    
    public HtmlNode Parse(){
        int index = 0;
        Stack<HtmlNode> nodeStack = new Stack<HtmlNode>();
        Stack<Token> contentStack = new Stack<Token>();
        HtmlNode root = new HtmlNode("root", "root");
        nodeStack.Push(root);

        while(index < tokens.Count-1){
            Token token = tokens[index];

            if(token.Type == "tag-start"){
                if(contentStack.Count > 0){
                    while(contentStack.Count > 0){
                       if(contentStack.Peek().Type == "attribute"){
                           string[] split = contentStack.Pop().Content.Replace(" ", "").Split('=');
                           nodeStack.Peek().Attributes.Add(new KeyPair(split[0], split[1]));
                       } else{
                           nodeStack.Peek().Children.Add(new HtmlNode(contentStack.Pop(), nodeStack.Count));
                       }
                    }
                }
                index++;
                nodeStack.Push(new HtmlNode(tokens[index], nodeStack.Count));
            }
            else if(token.Type == "attribute"){
                contentStack.Push(token);
            }
            else if(token.Type == "tag-end" && tokens[index+1].Content == nodeStack.Peek().Content){
                if(contentStack.Count > 0){
                    while(contentStack.Count > 0){
                       if(contentStack.Peek().Type == "attribute"){
                           string[] split = contentStack.Pop().Content.Replace(" ", "").Split('=');
                           nodeStack.Peek().Attributes.Add(new KeyPair(split[0], split[1]));
                       } else{
                           nodeStack.Peek().Children.Add(new HtmlNode(contentStack.Pop(), nodeStack.Count));
                       }
                    }
                }
                HtmlNode node = nodeStack.Pop();
                nodeStack.Peek().Children.Add(node);
                index++;
            }
            else if(token.Type != "tag" && token.Type != "tag-end"){
                if(contentStack.Count > 0){
                    while(contentStack.Count > 0){
                       if(contentStack.Peek().Type == "attribute"){
                           string[] split = contentStack.Pop().Content.Replace(" ", "").Split('=');
                           nodeStack.Peek().Attributes.Add(new KeyPair(split[0], split[1]));
                       } else{
                           nodeStack.Peek().Children.Add(new HtmlNode(contentStack.Pop(), nodeStack.Count));
                       }
                    }
                }
                contentStack.Push(token);
            }

            index++;
        }
        return root;
    }
}