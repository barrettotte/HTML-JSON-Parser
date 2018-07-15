using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class HtmlLexer{

    private String htmlString;
    private String[] childlessTags;
    private String[] closingTags;
    private String[] nonCloseTags;
    private Position currentPosition;
    private List<Token> tokens;
    
    private struct Position{
        public int index;
        public int column;
        public int line;
        public Position(int i, int c, int l){
            index = i;
            column = c;
            line = l;
        }
    }
    private struct Token{
        public string type;
        public string content;
        public Position start;
        public Position end;
        
        public Token(string t, string c){
            type = t;
            content = c;
            start = new Position();
            end = new Position();
        }
        public Token(string t, string c, Position s, Position e){
            type = t;
            content = c;
            start = s;
            end = e;
        }
    }


    public HtmlLexer(String str){
        this.htmlString = str;
        Console.WriteLine(htmlString.Length);
        //Put these tag arrays into a config file later.
        this.childlessTags = new String[] {"style", "script", "template"};
        this.closingTags = new String[] {"html", "head", "body", "p"};
        this.nonCloseTags = new String[] {"!doctype", "br", "hr", "img", "meta"};
        this.currentPosition = new Position(0,0,0);
        this.tokens = new List<Token>();
    }


    public String[] Lex(){
        while(currentPosition.index < htmlString.Length){
            int startIndex = currentPosition.index;
            LexText(htmlString, currentPosition);
            if(currentPosition.index == startIndex){
                if(StartsWith(htmlString, "!--", startIndex + 1)){
                    LexComment(htmlString, currentPosition);
                } else{
                    string tagName = LexTag(htmlString, currentPosition);
                    if(Array.Exists(childlessTags, elem => elem == tagName.ToLower())){
                        LexSkipTag(tagName, htmlString, currentPosition);
                    }
                }
            }
        }
        return new String[1];
        //return this.tokens.ToArray();
    }

    private void LexText(String str, Position pos){
        int textEnd = FindTextEnd(str, pos.index);
        if(textEnd == pos.index){
            return;
        }
        if(textEnd == -1){
            textEnd = str.Length;
        }
        Position startPosition = CopyPosition(pos);
        string content = " ";
        try{
            content = str.Substring(pos.index, textEnd);
            Console.WriteLine(pos.index + " --- " + textEnd + " ----- " + Regex.Replace(content, @"\t|\n|\r", ""));
        }
        catch (System.Exception){
            Console.WriteLine("!!!!!!!!!!!!!!!!!!" + pos.index + " --- " + textEnd + " ----- " + Regex.Replace(content, @"\t|\n|\r", ""));
            throw;
        }
        
        JumpPosition(textEnd);
        Position endPosition = CopyPosition(pos);
        tokens.Add(new Token("text", content, startPosition, endPosition));
    }

    private void LexComment(String str, Position pos){
        Position startPosition = CopyPosition(pos);
        FeedPosition(4);    // <!--
        int contentEndIndex = str.IndexOf("-->", pos.index);
        int commentEndIndex = contentEndIndex + 3; // -->
        if(contentEndIndex == -1){
            contentEndIndex = str.Length;
            commentEndIndex = str.Length;
        }
        string content = str.Substring(pos.index, contentEndIndex);
        JumpPosition(commentEndIndex);
        tokens.Add(new Token("comment", content, startPosition, CopyPosition(pos)));
    }

    private string LexTag(String str, Position pos){
        Position startPosition = CopyPosition(pos);
        int closeTag = str[pos.index + 1] == '/' ? 2 : 1;
        FeedPosition(closeTag);
        tokens.Add(new Token("tag-start", "" + closeTag, startPosition, new Position()));
        
        string tagName = LexTagName(str, pos);
        LexTagAttributes(str, pos);
        FeedPosition(str[pos.index] == '/' ? 2 : 1);
        tokens.Add(new Token("tag-end", "" + closeTag, new Position(), CopyPosition(pos)));
        return tagName;
    }

    private string LexTagName(String str, Position pos){
        int startIndex = pos.index;
        while(startIndex < str.Length){
            char c = str[startIndex];
            bool isTagChar = !(Char.IsWhiteSpace(c) || c == '/' || c == '>');
            if(isTagChar){ break;  }
            startIndex++;
        }
        int endIndex = startIndex + 1;
        while(endIndex < str.Length){
            char c = str[endIndex];
            bool isTagChar = !(Char.IsWhiteSpace(c) || c == '/' || c == '>');
            if(!isTagChar){ break; }
            endIndex++;
        }
        JumpPosition(endIndex);
        string tagName = str.Substring(startIndex, endIndex);
        tokens.Add(new Token("tag", tagName, new Position(), new Position()));
        return tagName;
    }


    private void LexTagAttributes(String str, Position pos){
        int cursor = pos.index;
        char quote = '\0';
        int wordBeginIndex = cursor;
        List<String> words = new List<String>();
        while(cursor < str.Length){
            char c = str[cursor];
            if(quote != '\0'){
                if(c == quote){
                    quote = '\0';
                }
                cursor++;
                continue;
            }
            if(c == '/' || c == '>'){
                if(cursor != wordBeginIndex){
                    words.Add(str.Substring(wordBeginIndex, cursor));
                }
                break;
            }
            if(Char.IsWhiteSpace(c)){
                if(cursor != wordBeginIndex){
                    words.Add(str.Substring(wordBeginIndex, cursor));
                }
                cursor++;
                wordBeginIndex = cursor;
                continue;
            }
            if(c == '\'' || c == '"'){
                quote = c;
                cursor++;
                continue;
            }
            cursor++;   //Change this to for loop PLEASE
        }
        FeedPosition(cursor);
        for(int i = 0; i < words.Count; i++){
            string word = words[i];
            if(word.IndexOf('=') == -1 && words.ElementAtOrDefault(i+1) != null){
                string secondWord = words[i+1];
                if(StartsWith(secondWord, "=", 0)){
                    if(secondWord.Length > 1){
                        tokens.Add(new Token("attribute", word + secondWord, new Position(), new Position()));
                        i++;
                        continue;
                    }
                    i++;
                    if(words.ElementAtOrDefault(i+1) != null){
                        string thirdWord = words[i+1];
                        tokens.Add(new Token("attribute", word + "=" + thirdWord));
                        i++;
                        continue;
                    }
                }
            } 
            if(EndsWith(word, "=", null) && words.ElementAtOrDefault(i+1) != null){
                string secondWord = words[i+1];
                if(secondWord != null && !secondWord.Contains("=")){
                    tokens.Add(new Token("attribute", word + secondWord, new Position(), new Position()));
                    i++;
                    continue;
                }
                tokens.Add(new Token("attribute", word.Substring(0,-1), new Position(), new Position()));
                continue;
            }
            tokens.Add(new Token("attribute", word, new Position(), new Position()));
        }
    }

    private void LexSkipTag(string tagName, string str, Position pos){
        string safeTag = tagName.ToLower();
        int index = pos.index;
        while(index < str.Length){
            int nextTagIndex = str.IndexOf("</", index);
            if(nextTagIndex == -1){
                LexText(str, pos);
                break;
            }
            JumpPosition(nextTagIndex);
            Position tagPos = CopyPosition(pos);
            if(safeTag != LexTag(str, tagPos).ToLower()){
                index = tagPos.index;
                continue;
            }
            if(nextTagIndex != pos.index){
                Position textStartPos = CopyPosition(pos);
                JumpPosition(nextTagIndex);
                tokens.Add(new Token("text", str.Substring(textStartPos.index, nextTagIndex), textStartPos, CopyPosition(pos)));
            }
            JumpPosition(tagPos.index);
            break;
        }
    }


    private Position CopyPosition(Position p){
        return new Position(p.index, p.column, p.line);
    }
    private bool StartsWith(string str, string search, int pos){
        return str.Substring(pos, search.Length) == search;
    }
    private bool EndsWith(string str, string search, Nullable<int> pos){
        int index = (pos == null) ? (str.Length - search.Length) : (((int)pos) - search.Length);
        int lastIndex = str.LastIndexOf(search, index);
        return (lastIndex != -1) && (lastIndex == index);
    }
    private int FindTextEnd(String str, int index){
        while(true){
            int textEnd = str.IndexOf('<', index);
            if(textEnd == -1){
                return textEnd;
            }
            char c = str[textEnd + 1];
            if(c == '/' || c == '!' || Char.IsLetterOrDigit(c)){
                return textEnd;
            }
            index = textEnd + 1;
        }
    }

    //Continue feeding the lexer an additional character
    private void FeedPosition(int length){
        int startIndex = currentPosition.index;
        currentPosition.index = startIndex + length;
        int endIndex = currentPosition.index;

        for(int i = startIndex; i < endIndex; i++){
            char c = htmlString[i];
            if(c == '\n'){
                currentPosition.line++;
                currentPosition.column++;
            } else{
                currentPosition.column++;
            }
        }
    }

    private void JumpPosition(int end){
        FeedPosition(end - currentPosition.index);
    }

}