# TriviaGame
This is a trivia game that can be used to help students prepare for the tests.\

Installation and usage instructions for nnguyen6831-A4.sln:\
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- There is a SQL file named “QuestionsA4” in the debug folder. That SQL file contains ten insert\
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;statements, which are the questions for the game. When the solution complies, make sure the
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SQL file is placed at the same place as the executable. When the executable is running, the
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;program will use that SQL file to generate the game's questions.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- If you have your MySql server, you need to change the server IP address, uid, and pwd in the\
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;connection string. And the connection string is in the “App.config” file. Make sure to type the
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;information of your MySQL server correctly.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- The database name is “gamerdba4”. The database will contain a set of collection tables such as\
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;QuestionsA4, GameLeaderBoard, and the tables which have the player name contains player
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;records.
