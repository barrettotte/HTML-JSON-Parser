using System;
using System.Collections.Generic;

public class HtmlNode{
    private string type;
    private string content;
    private List<KeyPair> attributes;
    private List<HtmlNode> children;
    private bool hasParent;
    private int generation;

    public string Type{ 
        get{ return this.type; } 
    }
    public string Content{ 
        get{ return this.content; } 
    }
    public List<KeyPair> Attributes{ 
        get{ return this.attributes; } 
    }
    public List<HtmlNode> Children{ 
        get{ return this.children; } 
    }
    public int Generation{
        get{ return this.generation; }
    }
    public bool HasParent{
        get{ return this.hasParent; }
    }

    public HtmlNode(){
        this.type = "";
        this.content = "";
        this.attributes = new List<KeyPair>();
        this.children = new List<HtmlNode>();
        this.generation = 0;
        this.hasParent = false;
    }
    public HtmlNode(string t, string c){
        this.type = t;
        this.content = c;
        this.attributes = new List<KeyPair>();
        this.children = new List<HtmlNode>();
        this.generation = 0;
        this.hasParent = false;
    }
    public HtmlNode(string t, string c, List<KeyPair> a, List<HtmlNode> child){
        this.type = t;
        this.content = c;
        this.attributes = (a == null) ? new List<KeyPair>() : a;
        this.children = (child == null) ? new List<HtmlNode>() : child;
        this.generation = 0;
        this.hasParent = false;
    }
    public HtmlNode(Token t, int g){
        this.type = t.Type;
        this.content = t.Content;
        this.attributes = new List<KeyPair>();
        this.children = new List<HtmlNode>();
        this.generation = g;
        this.hasParent = true;
    }

    public override ToString(){
        return "{type: " + this.type + ", content:"+ this.content + ", generation:" + this.generation +
            ",attributes:[" + this.attributes.Count + "], children:[" + this.children.Count + "]}";
    }

    public bool IsEmpty(){
        return (this.type == "" && this.content == "") && 
            (this.attributes.Count == 0) && (this.children.Count == 0);
    }
    private string getPad(int i){
        return new String(' ', (i + 1) * 4);
    }
}