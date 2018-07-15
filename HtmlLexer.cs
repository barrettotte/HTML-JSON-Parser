using System;
using System.Linq;
using System.Collections.Generic;

public class HtmlLexer{

    private String[] childlessTags = new String[] {"style", "script", "template"};
    private string htmlString;
    
    public HtmlLexer(){}

    public String[] Lexer(string str){
        this.htmlString = str;
        LexObj lexObj = new LexObj(new Position(0,0,0), new List<Token>());
        Lex(lexObj);
        
        foreach(Token token in lexObj.Tokens){
            Console.WriteLine(token.Content);
        }
        Console.WriteLine("\n" + lexObj.Tokens.Count + " tokens made!\n");
        return new String[1];
    }

    private void Lex(LexObj lexObj){
        while(lexObj.Position.Index < htmlString.Length){
            int startIndex = lexObj.Position.Index;
            LexText(lexObj);
            if(lexObj.Position.Index == startIndex){
                if(htmlString.Substring(startIndex + 1, 3) == "!--"){
                    LexComment(lexObj);
                } else{
                    string tag = LexTag(lexObj);
                    if(Array.IndexOf(childlessTags, tag.ToLower()) > -1){
                        LexSkipTag(tag, lexObj);
                    }
                }
            }
        }
    }

    private void LexText(LexObj lexObj){
        Position position = lexObj.Position;
        int textEnd = FindTextEnd(htmlString, position.Index);
        if(textEnd != position.Index){
            if(textEnd == -1){
                textEnd = htmlString.Length;
            }
            Position startPos = CopyPosition(position);
            string content = htmlString.Substring(position.Index, textEnd - position.Index);
            FeedPosition(position, htmlString, textEnd - position.Index);
            Position endPos = CopyPosition(position);
            lexObj.Tokens.Add(new Token("text", content, new PositionPair(startPos, endPos)));
        }
    }

    private void LexComment(LexObj lexObj){
        Position position = lexObj.Position;
        Position startPos = CopyPosition(position);
        FeedPosition(position, htmlString, 4); //<!---
        int contentEnd = htmlString.IndexOf("-->", position.Index);
        int commentEnd = contentEnd + 3; //-->
        if(contentEnd == -1){
            contentEnd = commentEnd = htmlString.Length;
        }
        string content = htmlString.Substring(position.Index, contentEnd - position.Index);
        FeedPosition(position, htmlString, commentEnd - position.Index);
        lexObj.Tokens.Add(new Token("comment", content, new PositionPair(startPos, CopyPosition(position))));
    }

    private string LexTag(LexObj lexObj){
        Position position = lexObj.Position;
        bool isClose = htmlString[position.Index + 1] == '/';
        Position startPos = CopyPosition(position);
        FeedPosition(position, htmlString, isClose ? 2 : 1);
        lexObj.Tokens.Add(new Token("tag-start", "" + isClose, new PositionPair(startPos, null)));
        string tagName = LexTagName(lexObj);
        LexTagAttributes(lexObj);
        return tagName;
    }


    private string LexTagName(LexObj lexObj){
        Position position = lexObj.Position;
        int start = position.Index;
        while(start < htmlString.Length){
            char c = htmlString[start];
            if(!(Char.IsWhiteSpace(c) || c == '/' || c == '>')){
                break;
            }
            start++;
        }
        int end = start + 1;
        while(end < htmlString.Length){
            char c = htmlString[end];
            if(!!(Char.IsWhiteSpace(c) || c == '/' || c == '>')){
                break;
            }
            end++;
        }
        FeedPosition(position, htmlString, end - position.Index);
        string tagName = htmlString.Substring(start, end - start);
        lexObj.Tokens.Add(new Token("tag", tagName, new PositionPair()));
        return tagName;
    }


    private void LexTagAttributes(LexObj lexObj){
        int cursor, wordBegin = cursor = lexObj.Position.Index;
        char quote = '\0';
        List<string> words = new List<string>();

        while(cursor < htmlString.Length){
            char c = htmlString[cursor];
            if(c == '/' || c == '>'){
                if(cursor != wordBegin){
                    words.Add(htmlString.Substring(wordBegin, cursor - wordBegin));
                }
                break;
            } else{
                if(quote != '\0'){
                    quote = (c == quote) ? '\0' : quote;
                } else if(Char.IsWhiteSpace(c)){
                    if(cursor != wordBegin){
                        words.Add(htmlString.Substring(wordBegin, cursor - wordBegin));
                    }
                    wordBegin = cursor + 1;
                } else if((c == '\'' || c == '"')){
                    quote = c;
                } 
                cursor++;
            } 
        }
        FeedPosition(lexObj.Position, htmlString, cursor - lexObj.Position.Index);
        LexTagAttributeWords(lexObj, words);
    }

    private void LexTagAttributeWords(LexObj lexObj, List<string> words){
        for(int i = 0; i < words.Count; i++){
            if(words[i].IndexOf("=") == -1){
                if(words.ElementAtOrDefault(i+1) != null && htmlString[1] == '='){
                    if(words[i+1].Length > 1){
                        lexObj.Tokens.Add(new Token("attribute", words[i] + words[i+1]));
                    } else if(words.ElementAtOrDefault(i+2) != null){
                        lexObj.Tokens.Add(new Token("attribute", words[i] + "=" + words[i+2]));
                    }
                }
            }
            if(EndsWith(words[i], "=", 0)){
                if(words.ElementAtOrDefault(i+1) != null && words[i+1].IndexOf("=", 0) != -1){
                    lexObj.Tokens.Add(new Token("attribute", words[i] + words[i+1]));
                } else{
                    lexObj.Tokens.Add(new Token("attribute", words[i].Substring(0, -1)));   //possible issue???
                }
            }
            lexObj.Tokens.Add(new Token("attribute", words[i]));
        }
    }

    private void LexSkipTag(string tagName, LexObj lexObj){
        int index = lexObj.Position.Index;
        while(index < htmlString.Length){
            int nextTag = htmlString.IndexOf("</", index);
            if(nextTag == -1){
                LexText(lexObj);
                break;
            }
            Position tagStartPos = CopyPosition(lexObj.Position);
            FeedPosition(lexObj.Position, htmlString, nextTag - lexObj.Position.Index);
            LexObj tagLexObj = new LexObj(tagStartPos, lexObj.Tokens);

            if(tagName.ToLower() != LexTag(tagLexObj).ToLower()){
                index = tagLexObj.Position.Index;
            } else if(nextTag != lexObj.Position.Index){
                Position textStart = CopyPosition(lexObj.Position);
                FeedPosition(lexObj.Position, htmlString, nextTag - lexObj.Position.Index);
                lexObj.Tokens.Add(new Token("text", htmlString.Substring(textStart.Index, nextTag - textStart.Index),
                    new PositionPair(textStart, CopyPosition(lexObj.Position))));
            } else{
                FeedPosition(lexObj.Position, htmlString, tagLexObj.Position.Index - lexObj.Position.Index);
                break;
            }
        }
    }

    private int FindTextEnd(string str, int index){
        while(true){
            int textEnd = str.IndexOf('<', index);
            if(textEnd != -1){
                char c = str[textEnd + 1];
                if(c == '/' || c == '!' || Char.IsLetterOrDigit(c)){
                    return textEnd;
                }
                index = textEnd + 1;
            } else{
                return textEnd;
            }
        }
    }

    private Position CopyPosition(Position position){
        return new Position(position.Index, position.Line, position.Column);
    }
    private void FeedPosition(Position position, string str, int len){
        int end = position.Index = position.Index + len;
        for(int i = position.Index; i < end; i++){
            position.Line += (str[i] == '\n') ? 1 : 0;
            position.Column = (str[i] == '\n') ? 0 : position.Column + 1;
        }
    }
    private bool EndsWith(string str, string search, int position){
        int index = (position == 0) ? str.Length : position;
        return (str.LastIndexOf(search, index) != -1) && (str.LastIndexOf(search, index) == index);
    }
}


class Token{
    private readonly string type;
    public string Type{ get{ return this.type; }}
    private readonly string content;
    public string Content{ get{ return this.content; }}
    private readonly PositionPair pair;
    public PositionPair Pair{ get{ return this.pair; }}

    public Token(){
        this.type = "undefined";
        this.content = "undefined";
        this.pair = new PositionPair();
    }
    public Token(string t, string c){
        type = t;
        content = c;
        pair = new PositionPair();
    }
    public Token(string t, string c, PositionPair p){
        type = t;
        content = c;
        pair = (p == null) ? new PositionPair() : p;
    }
}

class PositionPair{
    private readonly Position start;
    public Position Start{ get{ return this.start; }}
    private readonly Position end;
    public Position End{ get{ return this.end; }}

    public PositionPair(){
        start = new Position();
        end = new Position();
    }
    public PositionPair(Position s, Position e){
        start = (s == null) ? new Position() : s;
        end = (e == null) ? new Position() : e;
    }
}

class Position{
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

    public Position(){
        this.index = 0;
        this.column = 0;
        this.line = 0;
    }
    public Position(int i, int c, int l){
        this.index = i;
        this.column = c;
        this.line = l;
    }
}

class LexObj{
    private readonly Position position;
    public Position Position{ get{ return this.position; }}
    private readonly List<Token> tokens;
    public List<Token> Tokens{ get{ return this.tokens; }}

    public LexObj(){
        this.position = new Position();
        this.tokens = new List<Token>();
    }
    public LexObj(Position p, List<Token> t){
        this.position = (p == null) ? new Position() : p;
        this.tokens = (t == null) ? new List<Token>() : t;
    }
}