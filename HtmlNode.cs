using System;
using System.Collections.Generic;

public class HtmlNode{
    private string content;
    private int generation;
    private string type;
    private List<KeyPair> attributes;
    private List<HtmlNode> children;
    
    public string Content{ 
        get{ return this.content; } 
    }
    public int Generation{
        get{ return this.generation; }
    }
    public string Type{ 
        get{ return this.type; } 
    }
    public List<KeyPair> Attributes{ 
        get{ return this.attributes; } 
    }
    public List<HtmlNode> Children{ 
        get{ return this.children; } 
    }
    
    public HtmlNode(){
        this.type = "";
        this.content = "";
        this.attributes = new List<KeyPair>();
        this.children = new List<HtmlNode>();
        this.generation = 0;
    }
    public HtmlNode(string t, string c){
        this.type = t;
        this.content = c;
        this.attributes = new List<KeyPair>();
        this.children = new List<HtmlNode>();
        this.generation = 0;
    }
    public HtmlNode(string t, string c, List<KeyPair> a, List<HtmlNode> child){
        this.type = t;
        this.content = c;
        this.attributes = (a == null) ? new List<KeyPair>() : a;
        this.children = (child == null) ? new List<HtmlNode>() : child;
        this.generation = 0;
    }
    public HtmlNode(Token t, int g){
        this.type = t.Type;
        this.content = t.Content;
        this.attributes = new List<KeyPair>();
        this.children = new List<HtmlNode>();
        this.generation = g;
    }

    public override string ToString(){
        return "{type: " + this.type + ", content:"+ this.content + ", generation:" + this.generation +
            ",attributes:[" + this.attributes.Count + "], children:[" + this.children.Count + "]}";
    }

    public bool IsEmpty(){
        return (this.type == "" && this.content == "") && 
            (this.attributes.Count == 0) && (this.children.Count == 0);
    }

    public bool HasChildren(){
        return (this.children.Count > 0);
    }

    public bool HasAttributes(){
        return (this.attributes.Count > 0);
    }
}