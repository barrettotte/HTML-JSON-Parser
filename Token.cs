using System;

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
        this.type = "undefined";
        this.content = "undefined";
        this.startPos = new CursorPosition();
        this.endPos = new CursorPosition();
    }
    public Token(string t, string c){
        type = t;
        content = c;
        startPos = new CursorPosition();
        endPos = new CursorPosition();
    }
    public Token(string t, string c, CursorPosition s, CursorPosition e){
        type = t;
        content = c;
        startPos = (s == null) ? new CursorPosition() : s;
        endPos = (e == null) ? new CursorPosition() : e;
    }
}