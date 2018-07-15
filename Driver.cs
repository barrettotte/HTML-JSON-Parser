using System;
using System.IO;
using System.Collections.Generic;  


class Driver{
    
    private static void Main(){
        Driver d = new Driver();
        d.Start();
        String path = "Test-01.html";
        d.Parse(d.LoadHTML(path));
        d.Finish();
    }
    
    private void Parse(String htmlString){
        HtmlLexer hLexer = new HtmlLexer();
        String[] tokens = hLexer.Lexer(htmlString);
    }

    private String LoadHTML(String path){
        try{
            return File.ReadAllText(path);
        } catch (System.Exception){
            throw;
        }
    }
    private void Start(){
        Console.WriteLine("\nStarted HTML To JSON Parser.\n");
    }
    private void Finish(){
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }
}