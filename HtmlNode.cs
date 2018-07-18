using System.Collections.Generic;

public class HtmlNode{

    private string element;
    private string content;
    private List<KeyPair> attributes;
    private List<HtmlNode> children;


    #region Properties
    public string Element{ 
        get{ return this.element; } 
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
    #endregion


    public HtmlNode(){
        this.element = "";
        this.content = "";
        this.attributes = new List<KeyPair>();
        this.children = new List<HtmlNode>();
    }
    public HtmlNode(string e, string c, List<KeyPair> a, List<HtmlNode> child){
        this.element = e;
        this.content = c;
        this.attributes = (a == null) ? new List<KeyPair>() : a;
        this.children = (child == null) ? new List<HtmlNode>() : child;
    }


    public override string ToString(){
        string str = "{ element: " + this.element + ", content: " + this.content + ", attributes: ";
        for(int i = 0; i < this.attributes.Count; i++){
            str += (i == this.attributes.Count - 1) ? (this.attributes[i] + "") : (this.attributes[i] + ", ");
        }
        str += "children: ";
        for(int i = 0; i < this.children.Count; i++){
            str += (i == this.children.Count - 1) ? (this.children[i] + "") : (this.children[i] + ", ");
        }
        return str + " }";
    }
}