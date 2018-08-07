# HTML-JSON-Parser
## Run
* Verify all **.cs** files are in the same directory as **runCS.bat**
* Verify you have **csc.exe** in path  **(csc -v)**
* Run **runCs.bat**


## Summary :     Html --> Tokens --> JSON
* This program will take an html file and attempt to parse it to a simple JSON file.
* For simplicity this transformation will only go one way, no converting back.
* To my disbelief this actually works with every tag I tested and I have yet to find a major issue with nested tags.
* This parser assumes you know how to write html as there is no syntax analysis done before lexing.
* Later, this project will be adapted for a game development project involving a psuedo web browser.
* I also have plans to make a CSS to JSON parser depending on the success of this project, for a similar purpose.


## To Do:
 * Throw error if anything is left on stacks after parse is "completed" and log to file
 * Code refactor/cleanup for { parser, lexer }

## Tests:
| **File Name**  | **Description** |
| -------------- | --------------- |
| Test-01.html   | Very basic html test |
| Test-02.html   | Multiple attributes, multiple of same tag nested in each other |
| Test-03.html   | Comment |
| Test-04.html   | Comment before a tag, non-closing tag (DOCTYPE) |
| Test-05.html   | ul, li, ol, nested lists with varying format, hr, br |
| Test-06.html   | head, meta, script, noscript, body, header, a, img, footer, span |
| Test-07.html   | dl, dt, dd, strong, table, tr, td, thead tbody, tfoot, blockquote, data |
| Test-08.html   | commented out tag, form, legend, fieldset, input, embed, button, code, h1-n, textarea, label |
| Test-09.html   | canvas, col, colgroup, figure, figcaption, iframe, nav, picture, source, object, time, param |
| Test-10.html   | style, link, script with CDATA |
  
## Limitations \ Existing Issues:
  * Does not have extensive error handling, I assume that people don't need a lexical analyzer for something as simple as html.
  * Did not check every single tag, but I believe I checked around 80% of them.
  * Does not work with XHTML, I don't know why you would want to use it anyway in 2018, but that's just my opinion.
  * I found an error with the following code. Just don't do this please, it saves me some time on this project.
    I'm probably going to be the only one using it anyway.
  ```HTML
<script type="text/javascript">
   //<![CDATA[
      var x = 12345;
   //]]>
</script>
  ```


## HTML
``` HTML
<!--This is another
	Test File
-->
<!DOCTYPE html>
<html>
	<div class = "test">
		<p class="asd">
			Text text text text
		</p>
	</div>
	<!--I am a comment-->
</html>
```

## Tokens
``` text
{ type: comment, content: This is another	Test File, startPos: { index: 0, column: 0, line: 1 }, endPos: { index: 36, column: 3, line: 3 } }
{ type: tag-start, content: <, startPos: { index: 38, column: 0, line: 4 }, endPos: { index: 39, column: 1, line: 4 } }
{ type: tag, content: !DOCTYPE, startPos: { index: 39, column: 1, line: 4 }, endPos: { index: 47, column: 9, line: 4 } }
{ type: attribute, content: html, startPos: { index: 47, column: 9, line: 4 }, endPos: { index: 52, column: 14, line: 4 } }
{ type: tag-end, content: />, startPos: { index: 52, column: 14, line: 4 }, endPos: { index: 52, column: 14, line: 4 } }
{ type: tag-start, content: <, startPos: { index: 55, column: 0, line: 5 }, endPos: { index: 56, column: 1, line: 5 } }
{ type: tag, content: html, startPos: { index: 56, column: 1, line: 5 }, endPos: { index: 60, column: 5, line: 5 } }
{ type: tag-start, content: <, startPos: { index: 64, column: 1, line: 6 }, endPos: { index: 65, column: 2, line: 6 } }
{ type: tag, content: div, startPos: { index: 65, column: 2, line: 6 }, endPos: { index: 68, column: 5, line: 6 } }
{ type: attribute, content: class="test", startPos: { index: 68, column: 5, line: 6 }, endPos: { index: 83, column: 20, line: 6 } }
{ type: tag-start, content: <, startPos: { index: 88, column: 2, line: 7 }, endPos: { index: 89, column: 3, line: 7 } }
{ type: tag, content: p, startPos: { index: 89, column: 3, line: 7 }, endPos: { index: 90, column: 4, line: 7 } }
{ type: attribute, content: class="asd", startPos: { index: 90, column: 4, line: 7 }, endPos: { index: 102, column: 16, line: 7 } }
{ type: text, content: Text text text text, startPos: { index: 102, column: 16, line: 7 }, endPos: { index: 131, column: 2, line: 9 } }
{ type: tag-end, content: />, startPos: { index: 131, column: 2, line: 9 }, endPos: { index: 133, column: 4, line: 9 } }
{ type: tag, content: p, startPos: { index: 133, column: 4, line: 9 }, endPos: { index: 134, column: 5, line: 9 } }
{ type: tag-end, content: />, startPos: { index: 138, column: 1, line: 10 }, endPos: { index: 140, column: 3, line: 10 } }
{ type: tag, content: div, startPos: { index: 140, column: 3, line: 10 }, endPos: { index: 143, column: 6, line: 10 } }
{ type: comment, content: I am a comment, startPos: { index: 147, column: 1, line: 11 }, endPos: { index: 168, column: 22, line: 11 } }
{ type: tag-end, content: />, startPos: { index: 170, column: 0, line: 12 }, endPos: { index: 172, column: 2, line: 12 } }
{ type: tag, content: html, startPos: { index: 172, column: 2, line: 12 }, endPos: { index: 176, column: 6, line: 12 } }
```
## JSON
``` json
   {
   "Content":"root",
   "Generation":0,
   "Type":"root",
   "Attributes":[
      
   ],
   "Children":[
      {
         "Content":"!DOCTYPE",
         "Generation":1,
         "Type":"tag",
         "Attributes":[
            {
               "Key":"attribute",
               "Value":"html"
            }
         ],
         "Children":[
            
         ]
      },
      {
         "Content":"This is another\r\n\tTest File\r\n",
         "Generation":1,
         "Type":"comment",
         "Attributes":[
            
         ],
         "Children":[
            
         ]
      },
      {
         "Content":"html",
         "Generation":1,
         "Type":"tag",
         "Attributes":[
            
         ],
         "Children":[
            {
               "Content":"div",
               "Generation":2,
               "Type":"tag",
               "Attributes":[
                  {
                     "Key":"attribute",
                     "Value":"class=\"test\""
                  }
               ],
               "Children":[
                  {
                     "Content":"p",
                     "Generation":3,
                     "Type":"tag",
                     "Attributes":[
                        {
                           "Key":"attribute",
                           "Value":"class=\"asd\""
                        }
                     ],
                     "Children":[
                        {
                           "Content":"Text text text text",
                           "Generation":4,
                           "Type":"text",
                           "Attributes":[
                              
                           ],
                           "Children":[
                              
                           ]
                        }
                     ]
                  }
               ]
            },
            {
               "Content":"I am a comment",
               "Generation":2,
               "Type":"comment",
               "Attributes":[
                  
               ],
               "Children":[
                  
               ]
            }
         ]
      }
   ]
}
```
