using System;
using System.Linq;
using System.Collections.Generic;

public class HtmlLexer{

    private ParseOptions options;
    private string htmlString;
    private CursorPosition currentPosition;
    private List<Token> tokens;
    

    public List<Token> Lexer(string str, ParseOptions ops){
        this.htmlString = str;
        this.currentPosition = new CursorPosition();
        this.tokens = new List<Token>();
        this.options = (ops == null) ? new ParseOptions() : ops;

        LexObj lexObj = new LexObj(new CursorPosition());
        Lex(lexObj);

        return tokens;
    }

    private void Lex(LexObj lexObj){
        while(currentPosition.Index < htmlString.Length-1){
            int startIndex = currentPosition.Index;
            LexText(lexObj);
            if(currentPosition.Index == startIndex){
                if(htmlString.Substring(startIndex + 1, 3) == "!--"){
                    LexComment(lexObj);
                } else{
                    string tag = LexTag(lexObj);
                    if(Array.IndexOf(options.ChildlessTags, tag.ToLower()) > -1){
                        LexSkipTag(tag, lexObj);
                    }
                }
            }
        }
    }

    private void LexText(LexObj lexObj){
        CursorPosition position = currentPosition;
        int textEnd = FindTextEnd(htmlString, position.Index);
        if(textEnd != position.Index){
            if(textEnd == -1){
                textEnd = htmlString.Length;
            }
            CursorPosition startPos = CopyPosition(position);
            position.Index += (position.Index + 1 >= htmlString.Length) ? 0 : 1;
            string content = htmlString.Substring(position.Index, textEnd - position.Index);
            FeedPosition(position, htmlString, textEnd - position.Index);
            CursorPosition endPos = CopyPosition(position);
            tokens.Add(new Token("text", content, startPos, endPos));
        }
    }

    private void LexComment(LexObj lexObj){
        CursorPosition position = currentPosition;
        CursorPosition startPos = CopyPosition(position);
        FeedPosition(position, htmlString, 4); //<!---
        int contentEnd = htmlString.IndexOf("-->", position.Index);
        int commentEnd = contentEnd + 3; //-->
        if(contentEnd == -1){
            contentEnd = commentEnd = htmlString.Length;
        }
        string content = htmlString.Substring(position.Index, contentEnd - position.Index);
        FeedPosition(position, htmlString, commentEnd - position.Index);
        tokens.Add(new Token("comment", content, startPos, CopyPosition(position)));
    }

    private string LexTag(LexObj lexObj){
        CursorPosition position = currentPosition;
        bool isClose = htmlString[position.Index + 1] == '/';
        CursorPosition startPos = CopyPosition(position);
        FeedPosition(position, htmlString, isClose ? 2 : 1);
        tokens.Add(new Token("tag-start", "" + isClose, startPos, null));
        string tagName = LexTagName(lexObj);
        LexTagAttributes(lexObj);
        return tagName;
    }

    private string LexTagName(LexObj lexObj){
        CursorPosition position = currentPosition;
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
        tokens.Add(new Token("tag", tagName));
        return tagName;
    }

    private void LexTagAttributes(LexObj lexObj){
        int cursor, wordBegin = cursor = currentPosition.Index;
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
        FeedPosition(currentPosition, htmlString, cursor - currentPosition.Index);
        LexTagAttributeWords(lexObj, words);
    }

    private void LexTagAttributeWords(LexObj lexObj, List<string> words){
        for(int i = 0; i < words.Count; i++){
            if(words[i].IndexOf("=") == -1){
                if(words.ElementAtOrDefault(i+1) != null && htmlString[1] == '='){
                    if(words[i+1].Length > 1){
                        tokens.Add(new Token("attribute", words[i] + words[i+1]));
                    } else if(words.ElementAtOrDefault(i+2) != null){
                        tokens.Add(new Token("attribute", words[i] + "=" + words[i+2]));
                    }
                }
            }
            if(EndsWith(words[i], "=", 0)){
                if(words.ElementAtOrDefault(i+1) != null && words[i+1].IndexOf("=", 0) != -1){
                    tokens.Add(new Token("attribute", words[i] + words[i+1]));
                } else{
                    tokens.Add(new Token("attribute", words[i].Substring(0, -1)));   //possible issue???
                }
            }
            tokens.Add(new Token("attribute", words[i]));
        }
    }

    private void LexSkipTag(string tagName, LexObj lexObj){
        int index = currentPosition.Index;
        while(index < htmlString.Length){
            int nextTag = htmlString.IndexOf("</", index);
            if(nextTag == -1){
                LexText(lexObj);
                break;
            }
            CursorPosition tagStartPos = CopyPosition(currentPosition);
            FeedPosition(currentPosition, htmlString, nextTag - currentPosition.Index);
            LexObj tagLexObj = new LexObj(tagStartPos);

            if(tagName.ToLower() != LexTag(tagLexObj).ToLower()){
                index = tagLexObj.Position.Index;
            } else if(nextTag != currentPosition.Index){
                CursorPosition textStart = CopyPosition(currentPosition);
                FeedPosition(currentPosition, htmlString, nextTag - currentPosition.Index);
                tokens.Add(new Token("text", htmlString.Substring(textStart.Index, nextTag - textStart.Index), textStart, CopyPosition(currentPosition)));
            } else{
                FeedPosition(currentPosition, htmlString, tagLexObj.Position.Index - currentPosition.Index);
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

    private CursorPosition CopyPosition(CursorPosition position){
        return new CursorPosition(position.Index, position.Line, position.Column);
    }
    private void FeedPosition(CursorPosition position, string str, int len){
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

class LexObj{
    private readonly CursorPosition position;
    public CursorPosition Position{ get{ return this.position; }}

    public LexObj(){
        this.position = new CursorPosition();
    }
    public LexObj(CursorPosition p){
        this.position = (p == null) ? new CursorPosition() : p;
    }
}