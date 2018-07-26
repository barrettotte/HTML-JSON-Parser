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

    
    //REFACTOR THIS AWFUL LOOKING FUNCTION
    public HtmlNode Parse(){
        int index = 0;
        Stack<HtmlNode> nodeStack = new Stack<HtmlNode>();
        Stack<Token> contentStack = new Stack<Token>();
        HtmlNode root = new HtmlNode("root", "root");
        nodeStack.Push(root);

        while(index < tokens.Count-1){
            Token token = tokens[index];
            if(token.Type == "tag-start"){
                if(Array.IndexOf(options.NoClosingTags, tokens[index+1].Content.ToLower()) > -1){
                    index++;
                    HtmlNode selfClosingNode = new HtmlNode(tokens[index], nodeStack.Count);
                    int i = index;

                    while(i < tokens.Count){
                        if(tokens[i].Type == "tag-end"){
                            index++;
                            break;
                        } else if(tokens[i].Type == "attribute"){
                            selfClosingNode.Attributes.Add(new KeyPair("attribute", tokens[i].Content));
                            index++;
                        }
                        i++;
                    }
                    nodeStack.Peek().Children.Add(selfClosingNode);
                }
                else{
                    if(contentStack.Count > 0){
                        while(contentStack.Count > 0){
                            if(contentStack.Peek().Type == "attribute"){
                                nodeStack.Peek().Attributes.Add(new KeyPair("attribute", contentStack.Pop().Content));
                            } else{
                                nodeStack.Peek().Children.Add(new HtmlNode(contentStack.Pop(), nodeStack.Count));
                            }
                        }   
                    }
                    index++;
                    nodeStack.Push(new HtmlNode(tokens[index], nodeStack.Count));
                }
            }
            else if(token.Type == "attribute"){
                contentStack.Push(token);
            }
            else if(token.Type == "tag-end" && tokens[index+1].Content == nodeStack.Peek().Content){
                if(contentStack.Count > 0){
                    while(contentStack.Count > 0){
                       if(contentStack.Peek().Type == "attribute"){
                            nodeStack.Peek().Attributes.Add(new KeyPair("attribute", contentStack.Pop().Content));
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
                            nodeStack.Peek().Attributes.Add(new KeyPair("attribute", contentStack.Pop().Content));
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