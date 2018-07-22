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
        this.currentPosition = new CursorPosition(0,0,1);
        this.tokens = new List<Token>();
        this.options = (ops == null) ? new ParseOptions() : ops;
        Lex();
        return tokens;
    }

    private void Lex(){
        while(currentPosition.Index < htmlString.Length - 1){
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
            textEnd = (textEnd == -1) ? htmlString.Length : textEnd;
            CursorPosition startPos = CopyPosition(currentPosition);
            currentPosition.Index += (currentPosition.Index + 1 >= htmlString.Length) ? 0 : 1;
            string content = htmlString.Substring(currentPosition.Index, textEnd - currentPosition.Index).Trim();
            FeedPosition(currentPosition, htmlString, textEnd - currentPosition.Index);
            if(content != ""){
                tokens.Add(new Token("text", content, startPos, CopyPosition(currentPosition)));
            }
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
        string type = (isClose) ? "tag-end" : "tag-start";
        string content = (isClose) ? "/>" : "<";
        tokens.Add(new Token(type, content, startPos, CopyPosition(currentPosition)));
        string tagName = LexTagName();
        LexTagAttributes();
        return tagName;
    }

    private string LexTagName(){
        CursorPosition startPos = CopyPosition(currentPosition);
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
            if(Char.IsWhiteSpace(c) || c == '/' || c == '>'){
                break;
            }
            end++;
        }
        FeedPosition(currentPosition, htmlString, end - currentPosition.Index);
        string tagName = htmlString.Substring(start, end - start);
        tokens.Add(new Token("tag", "" + tagName, startPos, CopyPosition(currentPosition)));
        return tagName;
    }

    private void LexTagAttributes(){
        int index, attrStart = index = currentPosition.Index;
        char quote = '\0';
        List<string> words = new List<string>();

        while(index < htmlString.Length){
            char c = htmlString[index];
            if(quote != '\0'){
                quote = (c == quote) ? '\0' : quote;
            } else if(c == '/' || c == '>'){
                if(index != attrStart){
                    words.Add(htmlString.Substring(attrStart, index - attrStart));
                }
                break;
            } else if(Char.IsWhiteSpace(c)){
                if(index != attrStart){
                    words.Add(htmlString.Substring(attrStart, index - attrStart));
                }
                attrStart = index + 1;
            } else if(c == '\'' || c == '"'){
                quote = c;
            }
            index++;
        }

        CursorPosition startPos = CopyPosition(currentPosition);
        FeedPosition(currentPosition, htmlString, index - currentPosition.Index);
        LexTagAttributeWords(words, startPos);
    }

    private void LexTagAttributeWords(List<string> words, CursorPosition startPos){
        bool[] alreadyUsed = new bool[words.Count];
        for(int i = 0; i < words.Count; i++){
            if(!alreadyUsed[i]){
                if(!words[i].Contains('=') && words.ElementAtOrDefault(i+1) != null && !alreadyUsed[i+1] && words[i+1] == "="){
                    if(words.ElementAtOrDefault(i+2) != null && !alreadyUsed[i+2]){
                        tokens.Add(new Token("attribute", words[i] + "=" + words[i+2], startPos, CopyPosition(currentPosition)));
                        alreadyUsed[i] = alreadyUsed[i+1] = alreadyUsed[i+2] = true;
                    } 
                } else{
                    tokens.Add(new Token("attribute", words[i], startPos, CopyPosition(currentPosition)));
                }
            }
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
                string content = htmlString.Substring(textStart.Index, nextTag - textStart.Index).Trim();
                if(content != ""){
                    tokens.Add(new Token("text", content, textStart, CopyPosition(currentPosition)));
                }
            } else{
                FeedPosition(currentPosition, htmlString, tagStartPos.Index - currentPosition.Index);
                break;
            }
        }
    }

    private int FindTextEnd(string str, int index){
        while(true){
            int textEnd = str.IndexOf('<', index);
            if(textEnd == -1){
                return textEnd;
            } else{
                char c = str[textEnd + 1];
                if(Char.IsLetterOrDigit(c) || c == '/' || c == '!'){
                    return textEnd;
                }
                index = textEnd + 1;
            }
        }
    }

    private CursorPosition CopyPosition(CursorPosition position){
        return new CursorPosition(position.Index, position.Column, position.Line);
    }

    private void FeedPosition(CursorPosition position, string str, int len){
        int start = position.Index;
        int end = position.Index = start + len;
        for(int i = start; i < end; i++){
            if(str[i] == '\n'){
                position.Line++;
                position.Column = 0;
            } else{
                position.Column++;
            }
        }
    }

    private bool EndsWith(string str, string search, int position){
        int index = (position == 0) ? str.Length : position;
        return (str.LastIndexOf(search, index) != -1) && (str.LastIndexOf(search, index) == index);
    }
}