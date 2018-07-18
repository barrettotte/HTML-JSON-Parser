public class Token{
    private readonly string type;
    public string Type{ get{ return this.type; }}
    private readonly string content;
    public string Content{ get{ return this.content; }}
    private readonly CursorPosition startPos;
    public CursorPosition StartPos{ get{ return this.startPos; }}
    private readonly CursorPosition endPos;
    public CursorPosition EndPos{ get{ return this.endPos; }}

    public Token(){
        this.type = "";
        this.content = "";
        this.startPos = new CursorPosition();
        this.endPos = new CursorPosition();
    }
    
    public Token(string t, string c, CursorPosition s, CursorPosition e){
        this.type = t;
        this.content = c;
        this.startPos = (s == null) ? new CursorPosition() : s;
        this.endPos = (e == null) ? new CursorPosition() : e;
    }

    public override string ToString(){
        string str = "{ type: " + this.type + ", content: " + this.content + ", ";
        str += "startPos: " + this.startPos.ToString() + ", endPos: " + this.endPos.ToString() + " }";
        return str;
    }
}