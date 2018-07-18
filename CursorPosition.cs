public class CursorPosition{
    private int index;
    public int Index{ 
        get{ return this.index; }
        set{ this.index = value; }
    }
    private int column;
    public int Column{ 
        get{ return this.column; }
        set{ this.column = value; }
    }
    private int line;
    public int Line{ 
        get{ return this.line; }
        set{ this.line = value; }    
    }

    public CursorPosition(){
        this.index = 0;
        this.column = 0;
        this.line = 0;
    }
    public CursorPosition(int i, int c, int l){
        this.index = i;
        this.column = c;
        this.line = l;
    }

    public override string ToString(){
        return "{ index: " + this.index + ", column: " + this.column + ", line: " + this.line + " }";
    }
}