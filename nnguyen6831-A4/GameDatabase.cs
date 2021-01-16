/**
 *      FILE            :       GameDatabase.cs
 *      PROJECT         :       Relational Databases Assignment #4 
 *      PROGRAMMER      :       NGHIA NGUYEN 8616831
 *      DESCRIPTION     :       The purpose of this class is to create database for the gameplay
 *      FIRST VERSION   :       2020-12-08
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Data;

namespace nnguyen6831_A4
{
    enum SIZE_OF_BUFFER
    {
        MAX_OF_TEXT = 65535,
        MAX_OF_PLAYER_NAME = 255
    }

    /** \class      GameDatabase
   * 
   *   \brief      The purpose of this class is to create database for the gameplay
   * 
   *   \author     <i>Nghia Nguyen</i>
   */
    class GameDatabase
    {
        private string sConnection = null; // connection string
        private string sDatabaseName = null;
        private string sStorageQuestionName = null;
        private string exeLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private string sqlFileName = null;
        private string sqlFileLocation = null;
        private string sLeaderboard = null;
        private string[] sqlCommandDelim = { ";\n" };
        const int MAX_OF_TEXT = (int)SIZE_OF_BUFFER.MAX_OF_TEXT;
        const int MAX_OF_PLAYER_NAME = (int)SIZE_OF_BUFFER.MAX_OF_PLAYER_NAME;

        /**
        * \brief To initialize database for the gameplay
        *
        * \param Not receive anything
        *
        * \return Not return anything
        *
        */
        public GameDatabase()
        {
            string[] mySqlCommands = null;
            var result = 0;
            string query = null;
            int iLeaderboardExisted = 0;
            sConnection = ConfigurationManager.ConnectionStrings["QuestionGameDatabase"].ConnectionString;
            sDatabaseName = "GameRDBA4";
            sStorageQuestionName = "QuestionsA4";
            sqlFileName = "QuestionsA4.sql";
            sLeaderboard = "GameLeaderBoard";
            using(MySqlConnection connection = new MySqlConnection(sConnection))
            {
                MySqlCommand mySqlCommand = new MySqlCommand($"SELECT COUNT(SCHEMA_NAME) FROM information_schema.SCHEMATA WHERE SCHEMA_NAME = @DatabaseName;", connection);

                connection.Open();

                mySqlCommand.Parameters.AddWithValue("@DatabaseName", sDatabaseName);
                int iDatabaseExisted = Convert.ToInt32(mySqlCommand.ExecuteScalar());
                // Check whether the database is existed
                if (iDatabaseExisted == 0)
                {
                    // Create database
                    query = $"CREATE DATABASE {sDatabaseName};";
                    mySqlCommand = new MySqlCommand(query, connection);
                    using (MySqlDataReader mdrDataReader = mySqlCommand.ExecuteReader())
                    {

                    }

                    
                }

                // Use GameDatabase
                query = $"USE {sDatabaseName};";
                mySqlCommand = new MySqlCommand(query, connection);
                using (MySqlDataReader mdrDataReader = mySqlCommand.ExecuteReader())
                {

                }

                query = $"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = '{sDatabaseName}' AND table_name = '{sStorageQuestionName}';";
                mySqlCommand = new MySqlCommand(query, connection);
                int iStorageExisted = Convert.ToInt32(mySqlCommand.ExecuteScalar());
                // check whether the questions storage for gameplay is existed
                if(iStorageExisted == 0)
                {
                    // create question table
                    query = $"CREATE TABLE {sStorageQuestionName}(QuestionID INT NOT NULL AUTO_INCREMENT, Question TEXT({MAX_OF_TEXT}), FirstAnswer TEXT({MAX_OF_TEXT}), SecondAnswer TEXT({MAX_OF_TEXT}), ThirdAnswer TEXT({MAX_OF_TEXT}), FourthAnswer TEXT({MAX_OF_TEXT}), CorrectAnswer TEXT({MAX_OF_TEXT}), PRIMARY KEY (QuestionID));";
                    mySqlCommand = new MySqlCommand(query, connection);
                    result = mySqlCommand.ExecuteNonQuery();

                    sqlFileLocation = Path.Combine(exeLocation, Path.GetFileName(sqlFileName)); // read all sql statements
                    mySqlCommands = File.ReadAllText(sqlFileLocation).Split(sqlCommandDelim, StringSplitOptions.RemoveEmptyEntries); // split string into sql statement elments
                    // execute each statement elment
                    foreach(string cmd in mySqlCommands)
                    {
                        mySqlCommand.CommandText = cmd;
                        mySqlCommand.CommandType = System.Data.CommandType.Text;
                        result = mySqlCommand.ExecuteNonQuery();
                    }


                }

                // check whether the questions has enough question (at lease 10 questions)
                query = $"SELECT * FROM {sStorageQuestionName}";
                mySqlCommand = new MySqlCommand(query, connection);
                using(MySqlDataReader mdrDataReader = mySqlCommand.ExecuteReader())
                {
                    if(mdrDataReader.HasRows == false)
                    {
                        mdrDataReader.Close();
                        sqlFileLocation = Path.Combine(exeLocation, Path.GetFileName(sqlFileName)); // read all sql statements
                        mySqlCommands = File.ReadAllText(sqlFileLocation).Split(sqlCommandDelim, StringSplitOptions.RemoveEmptyEntries); // split string into sql statement elments
                        // execute each statement elment
                        foreach (string cmd in mySqlCommands)
                        {
                            mySqlCommand.CommandText = cmd;
                            mySqlCommand.CommandType = System.Data.CommandType.Text;
                            result = mySqlCommand.ExecuteNonQuery();
                        }
                    }
                }

                // check whether the leader is existed
                query = $"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = '{sDatabaseName}' AND table_name = '{sLeaderboard}';";
                mySqlCommand = new MySqlCommand(query, connection);
                iLeaderboardExisted = Convert.ToInt32(mySqlCommand.ExecuteScalar());
                if(iLeaderboardExisted == 0)
                {
                    // create leaderboard table
                    query = $"CREATE TABLE {sLeaderboard}(PlayerID INT NOT NULL AUTO_INCREMENT, PlayerName VARCHAR({MAX_OF_PLAYER_NAME}), Points INT, PRIMARY KEY (PlayerID));";
                    mySqlCommand = new MySqlCommand(query, connection);
                    result = mySqlCommand.ExecuteNonQuery();
                }
            }
        }

        /**
        * \brief To create a table to record information for the player
        *
        * \param sPlayerName - <b>string</b> - player name
        *
        * \return Return true if it is created successfully. Otherwise, false
        *
        */
        public bool CreatePlayerTable(string sPlayerName)
        {
            bool retCode = false;
            string query = null;

            // check whether the player is existed
            query = $"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = '{sDatabaseName}' AND table_name = '{sPlayerName}';";
            using (MySqlConnection connection = new MySqlConnection(sConnection))
            {
                MySqlCommand mySqlCommand = new MySqlCommand(query, connection);

                connection.Open();

                int iPlayerExisted = Convert.ToInt32(mySqlCommand.ExecuteScalar());
                if(iPlayerExisted == 0)
                {
                    query = $"CREATE TABLE {sPlayerName} (QuestionID INT, Answer TEXT({MAX_OF_TEXT}), Points INT, FOREIGN KEY (QuestionID) REFERENCES {sStorageQuestionName}(QuestionID));";
                    mySqlCommand = new MySqlCommand(query, connection);
                    using(MySqlDataReader mdrDataReader = mySqlCommand.ExecuteReader())
                    {

                    }
                    retCode = true;
                }
                else
                {
                    retCode = false;
                }
                
            }

            return retCode;

        }

        /**
        * \brief To return the storage questions for game 
        *
        * \param Not receive anything
        *
        * \return Return the storage question for game
        *
        */
        public DataTable RetrieveStorageQuestions()
        {
            DataTable StorageQuestion = null;
            string query = null;
            using(MySqlConnection connection = new MySqlConnection(sConnection))
            {
                query = $"SELECT * FROM {sStorageQuestionName};";
                MySqlDataAdapter mdaDataAdapter = new MySqlDataAdapter(query, connection);
                StorageQuestion = new DataTable();
                mdaDataAdapter.Fill(StorageQuestion);
            }
            return StorageQuestion;
        }

        /**
        * \brief To record answers from player
        *
        * \param sPlayName - <b>string</b> - player name or table
        * \param iQuestionId - <b>int</b> - question ID
        * \param sAnswer - <b>string</b> - answer of the question
        * \param Points - <b>int</b> - total point player get
        *
        * \return Return 1 if it is inserted successfully. Otherwise, 0
        *
        */
        public int RecordPlayerAnswers(string sPlayerName,int iQuestionId, string sAnswer, int Points)
        {
            string query = null;
            var result = 0;
            query = $"INSERT INTO {sPlayerName} VALUES('{iQuestionId}','{sAnswer}',{Points});";
            using(MySqlConnection connection = new MySqlConnection(sConnection))
            {
                MySqlCommand mySqlCommand = new MySqlCommand(query, connection);
                connection.Open();
                result = mySqlCommand.ExecuteNonQuery();
            }
            return (int)result;
        }

        /**
        * \brief To retrieve answers and points from player
        *
        * \param sPlayName - <b>string</b> - player name or table
        *
        * \return Return the player's records
        *
        */
        public DataTable RetrievePlayerAnswers(string sPlayerName)
        {
            DataTable StorageAnswers = null;
            string query = null;
            using (MySqlConnection connection = new MySqlConnection(sConnection))
            {
                query = $"SELECT * FROM {sPlayerName};";
                MySqlDataAdapter mdaDataAdapter = new MySqlDataAdapter(query, connection);
                StorageAnswers = new DataTable();
                mdaDataAdapter.Fill(StorageAnswers);
            }
            return StorageAnswers;
        }

        /**
        * \brief To calculate the total point of the player
        *
        * \param sPlayName - <b>string</b> - player name or table
        *
        * \return Return the player's total point
        *
        */
        public int TotalPoints(string sPlayerName)
        {
            int total = 0;
            string query = null;
            using (MySqlConnection connection = new MySqlConnection(sConnection))
            {
                query = $"SELECT SUM(Points) FROM {sPlayerName};";
                MySqlCommand mySqlCommand = new MySqlCommand(query, connection);
                connection.Open();
                total = Convert.ToInt32(mySqlCommand.ExecuteScalar());
            }
            return total;
        }


        /**
        * \brief To return record player total points
        *
        * \param sPlayName - <b>string</b> - player name or table
        * \param iPoints - <b>int</b> - total points
        * 
        * \return Return 1 if it is inserted successfully. Otherwise, 0
        *
        */
        public int RecordPlayerPoints(string sPlayerName, int iPoints)
        {
            string query = null;
            var result = 0;
            query = $"INSERT INTO {sLeaderboard}(PlayerName,Points) VALUES('{sPlayerName}',{iPoints});";
            using (MySqlConnection connection = new MySqlConnection(sConnection))
            {
                MySqlCommand mySqlCommand = new MySqlCommand(query, connection);
                connection.Open();
                result = mySqlCommand.ExecuteNonQuery();
            }
            return (int)result;
        }

        /**
        * \brief To retrieve leaderboard
        *
        * \param sPlayName - <b>string</b> - player name or table
        *
        * \return Return the player's total point
        *
        */
        public DataTable GameLeaderBoard()
        {
            DataTable leaderBoard = null;
            string query = null;
            using (MySqlConnection connection = new MySqlConnection(sConnection))
            {
                query = $"SELECT * FROM {sLeaderboard} ORDER BY Points DESC;";
                MySqlDataAdapter mdaDataAdapter = new MySqlDataAdapter(query, connection);
                leaderBoard = new DataTable();
                mdaDataAdapter.Fill(leaderBoard);
            }
            return leaderBoard;
        }
    }
}
