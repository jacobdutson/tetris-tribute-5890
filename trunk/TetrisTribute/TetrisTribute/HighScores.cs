//Jason Newbold
//High Scores

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text;

namespace TetrisTribute
{
    class HighScores
    {
        const int NAME = 0;
        const int SCORE = 1;

        //file location of the highscores file 
        //TODO NEED TO CHANGE FILEPATH
        string filePath = "";//Directory.GetCurrentDirectory() + "//HighScores.txt";

        string[][] scores;
        private XmlDocument file;

        public HighScores()
        {
            file = new XmlDocument();

            //initialize the scores array
            scores = new string[10][];
            for (int i = 0; i < 10; i++)
            {
                scores[i] = new string[2];
            }

            //load the file with the scores
            try
            {
                file.Load(filePath);

                XmlNodeList list = file.GetElementsByTagName("player");
                int t = list.Count;
                for (int i = 0; i < 10; i++)
                {
                    scores[i][NAME] = list.Item(i).Attributes.GetNamedItem("name").Value;
                    scores[i][SCORE] = list.Item(i).Attributes.GetNamedItem("score").Value;
                }
            }
            //file does not exist use default scores
            catch (System.IO.FileNotFoundException error)
            {
                Console.WriteLine(error);
                defaultScores();
            }
            catch (System.IO.DirectoryNotFoundException error)
            {
                Console.WriteLine(error);
                defaultScores();
            }
            catch (System.Exception error)
            {
                Console.WriteLine(error);
                defaultScores();
            }

        }

        //save the high scores to a file
        public void save()
        {
            try
            {
                file = new XmlDocument();
                XmlNode docNode = file.CreateXmlDeclaration("1.0", "UTF-8", null);
                file.AppendChild(docNode);

                XmlNode productsNode = file.CreateElement("HighScores");
                file.AppendChild(productsNode);

                for (int i = 0; i < 10; i++)
                {
                    XmlNode productNode = file.CreateElement("player");
                    XmlAttribute productAttribute = file.CreateAttribute("id");
                    productAttribute.Value = i.ToString();
                    productNode.Attributes.Append(productAttribute);

                    productAttribute = file.CreateAttribute("name");
                    productAttribute.Value = scores[i][NAME];
                    productNode.Attributes.Append(productAttribute);

                    productAttribute = file.CreateAttribute("score");
                    productAttribute.Value = scores[i][SCORE];
                    productNode.Attributes.Append(productAttribute);
                    productsNode.AppendChild(productNode);
                }

                // Save the document (to the Console window rather than a file).
                file.Save(filePath);
            }
            catch (System.Exception error)
            {
                Console.WriteLine(error);
                //do nothing
            }
        }

        //default scores if the highscore file has not been saved
        private void defaultScores()
        {
            scores[0][NAME] = "Mario";
            scores[0][SCORE] = "10000";
            scores[1][NAME] = "Luigi";
            scores[1][SCORE] = "8000";
            scores[2][NAME] = "Toad";
            scores[2][SCORE] = "6000";
            scores[3][NAME] = "Peach";
            scores[3][SCORE] = "5000";
            scores[4][NAME] = "Wario";
            scores[4][SCORE] = "4100";
            scores[5][NAME] = "Yoshi";
            scores[5][SCORE] = "3000";
            scores[6][NAME] = "Waluigi";
            scores[6][SCORE] = "2500";
            scores[7][NAME] = "Koopa Troopa";
            scores[7][SCORE] = "2000";
            scores[8][NAME] = "Donkey Kong";
            scores[8][SCORE] = "1500";
            scores[9][NAME] = "Diddy Kong";
            scores[9][SCORE] = "50";
        }

        //returns high scores
        public string[][] getHighScores()
        {
            return scores;
        }

        //returns the rank of the score, or -1 if fail to rank
        public int getRanking(int currentScore)
        {
            for (int i = 0; i < 10; i++)
            {
                if (currentScore >= int.Parse(scores[i][SCORE]))
                {
                    return i;
                }
            }
            return -1;
        }

        /**
         * Updates the highScores
         * Param string[2] currentScore
         * current
         */
        public string[][] upDateScores(string[] currentScore){

            bool update = false;
            string[] temp = new string[2];
            for (int i = 0; i < 10; i++)
            {
                if (update)
                {
                    string[] temp2 = new string[2];
                    temp2[NAME] = scores[i][NAME];
                    temp2[SCORE] = scores[i][SCORE];
                    scores[i][NAME] = temp[NAME];
                    scores[i][SCORE] = temp[SCORE];
                    temp[NAME] = temp2[NAME];
                    temp[SCORE] = temp2[SCORE];
                }
                else if (int.Parse(currentScore[SCORE]) >= int.Parse(scores[i][SCORE]))
                {
                    temp[NAME] = scores[i][NAME];
                    temp[SCORE] = scores[i][SCORE];
                    scores[i][NAME] = currentScore[NAME];
                    scores[i][SCORE] = currentScore[SCORE];
                    update = true;
                }
            }

           
            return scores;
        }

        public void saveScores(string[][] currentScores)
        {
            //TODO make sure current score has same size
            scores = currentScores;

            //if (update)
            {
                //TODO save file.
            }
        }

        public void clearScore(int rank, int updated)
        {
            scores[rank][NAME] = "";
            scores[rank][SCORE] = updated.ToString();
        }

        public void updateScore(int rank, string updatedName)
        {
            scores[rank][NAME] = updatedName;
        }

    }
}
