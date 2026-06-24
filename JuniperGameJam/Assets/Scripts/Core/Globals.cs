using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Globals is a raw c# class that is used to hold static, never changing variables that may need to be acessed from anywhere
/// it is particularly useful for scene names, they may be referenced in many areas and would be a real pain to change manually, 
/// but when stored here we only need to change one thing
/// </summary>
public static class Globals //raw C# class
{
    public static string MAIN_MENU_SCENE_NAME = "MainMenuScene";
    public static string GAMEPLAY_SCENE_NAME = "GameplayScene";


    public static float BOX_MALFUNCTION_CHANCE_MULT_NORMAL = 0.1f;
    public static float BOX_MALFUNCTION_CHANCE_MULT_HARD = 0.45f;

    public static float BOX_INVALIDITY_TIME_MULT_NORMAL = 1f;
    public static float BOX_INVALIDITY_TIME_MULT_HARD = 0.45f;

    public static float INITIAL_SPAWN_DELAY_MULT_NORMAL = 2f;
    public static float INITIAL_SPAWN_DELAY_MULT_HARD = 1f;
}
