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
        Lex();
        return tokens;
    }

    private void Lex(){
        while(currentPosition.Index < htmlString.Length){
            int startIndex = currentPosition.Index;
            LexText();
            if(currentPosition.Index == startIndex){
                if(htmlString.Substring(startIndex + 1, 3) == "!--"){
                    LexComment();
                } else{
                    string tag = LexTag();
                    if(Array.IndexOf(options.ChildlessTags, tag.ToLower()) > -1){
                        LexSkipTag(tag);
                    }
                }
            }
        }
    }

    private void LexText(){
        int textEnd = FindTextEnd(htmlString, currentPosition.Index);
        if(textEnd != currentPosition.Index){
            if(textEnd == -1){
                textEnd = htmlString.Length;
            }
            CursorPosition startPos = CopyPosition(currentPosition);
            currentPosition.Index += (currentPosition.Index + 1 >= htmlString.Length) ? 0 : 1;
            string content = htmlString.Substring(currentPosition.Index, textEnd - currentPosition.Index);
            FeedPosition(currentPosition, htmlString, textEnd - currentPosition.Index);
            CursorPosition endPos = CopyPosition(currentPosition);
            tokens.Add(new Token("text", content, startPos, endPos));
        }
    }

    private void LexComment(){
        CursorPosition startPos = CopyPosition(currentPosition);
        FeedPosition(currentPosition, htmlString, 4); //<!---
        int contentEnd = htmlString.IndexOf("-->", currentPosition.Index);
        int commentEnd = contentEnd + 3; //-->
        if(contentEnd == -1){
            contentEnd = commentEnd = htmlString.Length;
        }
        string content = htmlString.Substring(currentPosition.Index, contentEnd - currentPosition.Index);
        FeedPosition(currentPosition, htmlString, commentEnd - currentPosition.Index);
        tokens.Add(new Token("comment", content, startPos, CopyPosition(currentPosition)));
    }

    private string LexTag(){
        bool isClose = htmlString[currentPosition.Index + 1] == '/';
        CursorPosition startPos = CopyPosition(currentPosition);
        FeedPosition(currentPosition, htmlString, isClose ? 2 : 1);
        tokens.Add(new Token("tag-start", "" + isClose, startPos, null));
        string tagName = LexTagName();
        LexTagAttributes();
        return tagName;
    }

    private string LexTagName(){
        int start = currentPosition.Index;
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
            if(!(Char.IsWhiteSpace(c) || c == '/' || c == '>')){
                break;
            }
            end++;
        }
        FeedPosition(currentPosition, htmlString, end - currentPosition.Index);
        string tagName = htmlString.Substring(start, end - start);
        tokens.Add(new Token("tag", tagName, null, null));
        return tagName;
    }

    private void LexTagAttributes(){
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
        LexTagAttributeWords(words);
    }

    private void LexTagAttributeWords(List<string> words){
        for(int i = 0; i < words.Count; i++){
            if(words[i].IndexOf("=") == -1){
                if(words.ElementAtOrDefault(i+1) != null && htmlString[1] == '='){
                    if(words[i+1].Length > 1){
                        tokens.Add(new Token("attribute", words[i] + words[i+1], null, null));
                    } else if(words.ElementAtOrDefault(i+2) != null){
                        tokens.Add(new Token("attribute", words[i] + "=" + words[i+2], null, null));
                    }
                } // Kind of messy around here...think about more refactoring
            }
            if(EndsWith(words[i], "=", 0)){
                if(words.ElementAtOrDefault(i+1) != null && words[i+1].IndexOf("=", 0) != -1){
                    tokens.Add(new Token("attribute", words[i] + words[i+1], null, null));
                } else{
                    tokens.Add(new Token("attribute", words[i].Substring(0, -1), null, null));
                }
            }
            tokens.Add(new Token("attribute", words[i], null, null));
        }
    }

    private void LexSkipTag(string tagName){
        int index = currentPosition.Index;
        while(index < htmlString.Length){
            int nextTag = htmlString.IndexOf("</", index);
            if(nextTag != -1){
                LexText();
                break;
            }
            CursorPosition tagStartPos = CopyPosition(currentPosition);
            FeedPosition(currentPosition, htmlString, nextTag - currentPosition.Index);
            if(tagName.ToLower() != LexTag().ToLower()){
                index = tagStartPos.Index;
            } else if(nextTag != currentPosition.Index){
                CursorPosition textStart = CopyPosition(currentPosition);
                FeedPosition(currentPosition, htmlString, nextTag - currentPosition.Index);
                tokens.Add(new Token("text", htmlString.Substring(textStart.Index, nextTag - textStart.Index), textStart, CopyPosition(currentPosition)));
            } else{
                FeedPosition(currentPosition, htmlString, tagStartPos.Index - currentPosition.Index);
                break;
            }
        }
    }

    private int FindTextEnd(string str, int index){
        while(true){
            int textEnd = str.IndexOf('<', index);
            if(textEnd != -1){
                char c = str[textEnd + 1];
                if(Char.IsLetterOrDigit(c) || c == '/' || c == '!'){
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