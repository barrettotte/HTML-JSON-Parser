using System;
using System.Collections.Generic;


public class HtmlJsonParser{

    private List<Token> tokens;
    private ParseOptions options;

    public HtmlJsonParser(){
        tokens = new List<Token>();
        options = new ParseOptions();
    }

    public void Parser(List<Token> t, ParseOptions o){
        this.tokens = t;
        this.options = o;
        Parse();
    }

    private void Parse(){
        
    }
}


class Node{
    public string type;
    public string content;
    public List<KeyPair> attributes;
    public List<Node> children;
    public CursorPosition[] position;

    public Node(){
        this.type = this.tagName = "undefined";
        this.attributes = new List<KeyPair>();
        this.children = new List<Node>();
        this.position = new CursorPosition[] {new CursorPosition(), new CursorPosition()};
    }
    public Node(string t, string tn){
        this.type = t;
        this.content = ct;
        this.attributes = new List<KeyPair>();
        this.children = new List<Node>();
        this.position = new CursorPosition[] {new CursorPosition(), new CursorPosition()};
    }
    public Node(string t, string tn, List<KeyPair> a, List<Node> c){
        this.type = t;
        this.content = ct;
        this.attributes = (a == null) ? new List<KeyPair>() : a;
        this.children = (c == null) ? new List<Node>() : c;
        this.position = new CursorPosition[] {new CursorPosition(), new CursorPosition()};
    }
    public Node(string t, string ct, List<KeyPair> a, List<Node> c, CursorPosition[] p){
        this.type = t;
        this.content = ct;
        this.attributes = (a == null) ? new List<KeyPair>() : a;
        this.children = (c == null) ? new List<Node>() : c;
        this.position = (p == null) ? new CursorPosition[] {new CursorPosition(), new CursorPosition()} : p;
        this.position[0] = (this.position[0] == null) ? new CursorPosition() : this.position[0];
        this.position[1] = (this.position[1] == null) ? new CursorPosition() : this.position[1];
    }
}

class KeyPair{
    private string key;
    public string Key{ get{ return key; }}
    private string value;
    public string Value{ get{ return value; }}

    public KeyPair(){
        key = value = "undefined";
    }
    public KeyPair(string k, string v){
        key = k;
        value = v;
    }
}