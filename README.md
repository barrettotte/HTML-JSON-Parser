# HTML-JSON-Parser
## Run
* Verify all **.cs** files are in the same directory as **runCS.bat**
* Verify you have **csc.exe** in path  **(csc -v)**
* Run **runCs.bat**


## Summary :     Html --> Tokens --> JSON
* This program will take an html file and attempt to parse it to a simple JSON file.
* For simplicity this transformation will only go one way, no converting back.
* Later, this project will be adapted for a game development project involving a psuedo web browser.
* I also have plans to make a CSS to JSON parser depending on the success of this project, for a similar purpose.


## To Do:
 * Testing for li, dt, dd, tbody, thead, tfoot, tr, td, ul, ol, table
 * Testing for body, blockquote, br, button, canvas, caption, code, col , colgroup, data, dialog
 * Testing for dl, dt, embed, fieldset, figcaption, figure, footer, form, head, header, iframe, img
 * Testing for input, label, legend, link, main, map, meta, nav, noscript, object, picture, pre, span
 * Testing for source, string, summary, sup, template, textarea, time, var, video, wbr
 * Testing for style, script, template
 * Add better error handling and log it to file.
    * Check to see if anything is left on stacks after parse is "completed"
 * Code refactor/cleanup for { parser, lexer }


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
