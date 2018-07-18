using System;
using System.IO;
using System.Collections.Generic;  


class Driver{
    
    private static void Main(){
        Driver d = new Driver();
        d.Start();

        bool isDebug = true;
        bool writeToConsole = true;
        bool writeToTextFile = true;
        string fileName = "files/Test-01";
        string htmlContent = d.LoadHTML(fileName);

        HtmlLexer hLexer = new HtmlLexer();
        ParseOptions options = new ParseOptions();
        List<Token> tokens = hLexer.Lexer(htmlContent, options);
        
        if(isDebug){
            if(writeToConsole){
                d.WriteTokensToConsole(tokens);
            }
            if(writeToTextFile){
                d.WriteTokensToTextFile(tokens, fileName);
            }
        }
        HtmlParse parser = new HtmlParse();
        parser.Parse(tokens);
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
        File.WriteAllText(fileName + ".tokens.txt", content); 
        Console.WriteLine("Wrote " + fileName + ".html tokens to " + fileName + ".tokens.txt");
    }

    private void Start(){
        Console.WriteLine("\nStarted HTML To JSON Parser.\n");
    }

    private void Finish(){
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }
}