# TriviaGame
This is a trivia game that can be used to help students prepare for the tests.

Installation and usage instructions for TriviaGame:
- There is a SQL file named “QuestionsA4” in the debug folder. That SQL file contains ten insert
statements, which are the questions for the game. When the solution complies, make sure the
SQL file is placed at the same place as the executable. When the executable is running, the
program will use that SQL file to generate the game's questions.
- If you have your MySql server, you need to change the server IP address, uid, and pwd in the
connection string. And the connection string is in the “App.config” file. Make sure to type the
information of your MySQL server correctly.
- The database name is “gamerdba4”. The database will contain a set of collection tables such as
QuestionsA4, GameLeaderBoard, and the tables which have the player name contains player
records.
