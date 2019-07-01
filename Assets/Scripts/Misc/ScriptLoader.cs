//using System.Collections.Generic;

//using UnityEngine;

//using CSScriptLibrary;

//// Author: Vincent Versnel

///// <summary>
///// A simple factory to load state scripts and return them as objects by name.
///// </summary>
//public static class ScriptLoader
//{
//    public static void LoadScript<T>(string nameOfScript, Dictionary<string, State<T>> states)
//    {
//        if (!states.ContainsKey(nameOfScript))
//        {
//            System.Reflection.Assembly assembly = CSScript.Load(Application.dataPath + "/Scripts/Agents/Scripts/" + nameOfScript + ".txt", null, true);
//            AsmHelper helper = new AsmHelper(assembly);
//            State<T> state = (State<T>)helper.CreateObject(nameOfScript);
//            states.Add(nameOfScript, state);
//        }
//    }
//}
