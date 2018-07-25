using System;
using System.IO;
using System.Collections.Generic;  

class Driver{
    
    private static void Main(){
        const string fileName = "files/Test-01";
        Driver d = new Driver();
        ParseOptions options = new ParseOptions();
        d.Start();

        HtmlLexer hLexer = new HtmlLexer();
        HtmlParse parser = new HtmlParse();
        JsonConverter converter = new JsonConverter();
        string htmlContent = d.LoadHTML(fileName);

        List<Token> tokens = hLexer.Lexer(htmlContent, options);
        //d.WriteTokensToConsole(tokens);
        d.WriteTokensToTextFile(tokens, fileName);
        //d.WriteCharsToTextFile(htmlContent, fileName);
        HtmlNode parseResult = parser.Parser(tokens, options);
        string json = converter.convertHtml(parseResult);
        d.WriteToJSON(json);
        d.Finish();
    }

    private string LoadHTML(String fileName){
        try{
            return File.ReadAllText(fileName + ".html");
        } catch (System.Exception){
            throw;
        }
    }

    private void WriteTokensToConsole(List<Token> tokens){
        for(int i = 0; i < tokens.Count; i++){
            Console.WriteLine(i + ".  " + tokens[i].ToString().Replace("\n", ""));
        }
        Console.WriteLine("\n" + tokens.Count + " tokens made!\n");
    }

    private void WriteTokensToTextFile(List<Token> tokens, string fileName){
        string content = "";
        for(int i = 0; i < tokens.Count; i++){
            content += tokens[i].ToString().Replace("\r\n", "").Replace("\n", "") + "\n\n";
        }
        WriteToTextFile(content, fileName, "tokens");
    }

    private void WriteCharsToTextFile(string str, string fileName){
        string content = "";
        for(int i = 0; i < str.Length; i++){
            content += (i + " --- " + str[i]);
            content += (str[i] == '\n') ? "" : "\n";
        }
        WriteToTextFile(content, fileName, "chars");
    }

    private void WriteToTextFile(string content, string fileName, string contentType){
        File.WriteAllText(fileName + "." + contentType + ".txt", content);
        Console.WriteLine("Wrote " + fileName + ".html " + contentType + " to " + fileName + "." + contentType + ".txt");
    }

    private void WriteToJSON(string content, string fileName, string contentType){
        File.WriteAllText(fileName + "." + contentType + ".json", content);
        Console.WriteLine("Wrote " + fileName + ".html " + contentType + " to " + fileName + "." + contentType + ".txt");
    }

    private void Start(){
        Console.WriteLine("\nStarted HTML To JSON Parser.\n");
    }

    private void Finish(){
        Console.WriteLine("\nPress any key to exit.");
        Console.ReadKey();
    }

}