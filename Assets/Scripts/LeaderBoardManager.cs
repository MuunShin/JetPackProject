using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoardManager : MonoBehaviour
{
    string path;

    private void Start()
    {
        path = Application.persistentDataPath + "/HighScores";
        if (!Directory.Exists(path))
        {
            var folder = Directory.CreateDirectory(path);
        }

    }

    public ArrayList ReadScoreTxt(int level )
    {
        ArrayList returnAL = new ArrayList();

        path = Application.persistentDataPath + "/HighScores/highScores" + level;

        if (!File.Exists(path))
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                for(int i = 0; i < 6;i++)
                    sw.WriteLine("____/00:00:00/00:00:00");
            }
        }
        
        using (StreamReader sr = File.OpenText(path))
        {
            string s;
            while ((s = sr.ReadLine()) != null)
            {
                ArrayList addAr;
                addAr = new ArrayList();

                char[] delimiteurs = { '/', ':' };

                string[] line = s.Split(delimiteurs);

                foreach(string st in line)
                {
                    addAr.Add(st);
                }
                returnAL.Add(addAr);
            }
        }

        return returnAL;
    }

    public void SaveScoreTxt(int level, ArrayList arrayList )
    {

        path = Application.persistentDataPath + "/HighScores/highScores" + level;

        if (!File.Exists(path))
        {
            Debug.Log("PAS DE SAVE POUR CE NIVEAU");
        }
        else
        {

            while (arrayList.Count > 6)
            {
                arrayList.RemoveAt(arrayList.Count - 1);
            }

            using (StreamWriter sw = File.CreateText(path))
            {
                foreach(ArrayList ar in arrayList)
                {

                    string verifName = ar[0].ToString();
                    var verifMin = ar[1];
                    var verifSec = ar[2];
                    var verifCentS = ar[3];
                    var lapMin = ar[4];
                    var lapSec = ar[5];
                    var lapCentS = ar[6];

                    string s = verifName + "/" + verifMin + ":" + verifSec + ":" + verifCentS + "/" + lapMin + ":" + lapSec + ":" + lapCentS;

                    sw.WriteLine(s);

                }

            }
        }
    }

    public bool AddScore(string name, float min,float sec,float centS,float lapMin,float lapSec,float lapCentS,int level)
    {
        ArrayList highScores;
        highScores = ReadScoreTxt(level);

        bool validate = false;

        ArrayList addAr;
        addAr = new ArrayList();
        
        addAr.Add(name);
        addAr.Add(min);
        addAr.Add(sec);
        addAr.Add(centS);
        addAr.Add(lapMin);
        addAr.Add(lapSec);
        addAr.Add(lapCentS);

        if (highScores.Count > 0)
        {
            foreach (ArrayList ar in highScores)
            {
                string verifName = ar[0].ToString();
                float verifMin = float.Parse((string)ar[1]);
                float verifSec = float.Parse((string)ar[2]);
                float verifCentS = float.Parse((string)ar[3]);
                float compCentS = (sec * 100 + min * 6000 + centS);
                int verifRank = highScores.IndexOf(ar);



                verifCentS += (verifSec * 100 + verifMin * 6000);


                Debug.Log("Cent = " + compCentS + " vs nb" + verifRank + " = " + verifCentS);

                switch (compCentS)
                {
                    case float centSC when compCentS == verifCentS:
                        validate = true;

                        highScores.Insert(verifRank+1, addAr);

                        break;

                    case float centSC when ((compCentS < verifCentS) || (verifCentS == 0)):
                        validate = true;

                        highScores.Insert(verifRank, addAr);
                        break;
                }

                if(validate)
                    break;
            }

            SaveScoreTxt(level, highScores);

            while (highScores.Count > 6 )
            {
                highScores.RemoveAt(highScores.Count - 1);
            }

        }
        return validate;
    }

    public int Placed(float min, float sec, float centS, int level)
    {
        ArrayList highScores;
        highScores = ReadScoreTxt(level);

        bool validate = false;
        int verifRank = 0;

        if (highScores.Count > 0)
        {
            foreach (ArrayList ar in highScores)
            {

                float verifMin = float.Parse((string)ar[1]);
                float verifSec = float.Parse((string)ar[2]);
                float verifCentS = float.Parse((string)ar[3]);
                float compCentS = (sec * 100 + min * 6000 + centS);
                verifRank = highScores.IndexOf(ar);


                verifCentS += (verifSec * 100 + verifMin * 6000);


                Debug.Log("Cent = " + compCentS + " vs nb" + verifRank + " = " + verifCentS);

                switch (compCentS)
                {
                    case float centSC when compCentS == verifCentS:
                        validate = true;
                        verifRank += 2;
                        Debug.Log("egalit");
                        break;

                    case float centSC when ((compCentS < verifCentS) || (verifCentS == 0)):
                        validate = true;
                        verifRank += 1;
                        Debug.Log("better");
                        break;
                }

                if (validate)
                {
                    
                    break;
                }
                
            }
        }
        if (!validate)
            verifRank = 0;

        Debug.Log("Plance : " + verifRank);
        return verifRank;
    }

}
