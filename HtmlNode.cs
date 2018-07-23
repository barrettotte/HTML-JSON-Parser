using System.Collections.Generic;

public class HtmlNode{
    private string type;
    private string content;
    private List<KeyPair> attributes;
    private List<HtmlNode> children;

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

    public HtmlNode(){
        this.type = "";
        this.content = "";
        this.attributes = new List<KeyPair>();
        this.children = new List<HtmlNode>();
    }
    public HtmlNode(string t, string c, List<KeyPair> a, List<HtmlNode> child){
        this.type = t;
        this.content = c;
        this.attributes = (a == null) ? new List<KeyPair>() : a;
        this.children = (child == null) ? new List<HtmlNode>() : child;
    }
    public HtmlNode(Token t){
        this.type = t.Type;
        this.content = t.Content;
        this.attributes = new List<KeyPair>();
        this.children = new List<HtmlNode>();
    }

    public override string ToString(){
        string str = " {\n   type: " + this.type + ",\n";
        str += "   content: " + this.content.Replace("\r\n", "\\n");
        if(this.attributes.Count > 0){
            str += ",\n   attributes: \n";
            for(int i = 0; i < this.attributes.Count; i++){
                str += "      ";
                str += (i == this.attributes.Count - 1) ? (this.attributes[i] + "") : (this.attributes[i] + ", \n");
            }
        } 
        if(this.children.Count > 0){
            str += "   children: ";
            for(int i = 0; i < this.children.Count; i++){
                str += (i == this.children.Count - 1) ? (this.children[i] + "") : (this.children[i] + ", ");
            }
        }
        return str + "\n }";
    }

    public bool IsEmpty(){
        return (this.type == "" && this.content == "") && 
            (this.attributes.Count == 0) && (this.children.Count == 0);
    }
}