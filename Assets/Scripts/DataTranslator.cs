using UnityEngine;
using System;


public class DataTranslator : MonoBehaviour {

    private static string KILLS_PREFIX = "[KILLS]";
    private static string DEATH_PREFIX = "[DEATHS]";

       public static int DataToKills(string data)
        {

            return Int32.Parse(DataToValue(data, KILLS_PREFIX));
        }
        
    public static int DataToDeaths(string data)
       {
           return Int32.Parse(DataToValue(data, DEATH_PREFIX));
       }

    private static string DataToValue(string data,string symbol)
    {
        string[] splitData = data.Split('/');
        foreach (String piece in splitData)
        {
            if (piece.StartsWith(symbol))
                return piece.Substring(symbol.Length);

        }
        
        Debug.LogError(symbol+" not found in "+data);
        return "";

    }

    public static string ValuesToData(int kills,int deaths)
    {
        return "[KILLS]" + kills + "/[DEATHS]" + deaths;
    }

	
}
