using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game{

	public static int NOWLEVEL;

	public static void LoadScene(string p_sceneName){
		SceneManager.LoadScene (p_sceneName);
	}
}
