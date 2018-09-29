using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

[System.Serializable]
public class PlayerData{
	[System.Serializable]public class LevelData{
		public bool getStart = false; //是否得到星星
		public float BestTime = 0; //最快時間
		public bool isComplete = false; //是否完成
	}

	[System.Serializable]public class EndlessModeData{
		public float bestTime = 0;
		public int maxScore = 0;
	}

	/********	Data	********/
	public string language = "TraditionalChinese";//選擇的語言
	public LevelData[] levelDatas = new LevelData[500];
	public EndlessModeData endlessModeData;
	public bool isShowBeginnerGuide = false;

	/*********************/
	private static string pathName = "PlayerData";
	private static PlayerData instance;


	public void Save(){
		string _path = Application.persistentDataPath + "/Data/" + PlayerData.pathName + ".json";
		string _json = JsonUtility.ToJson(this);

		if (Directory.Exists (Application.persistentDataPath + "/Data") == false) {
			Directory.CreateDirectory(Application.persistentDataPath + "/Data");
		}

		File.WriteAllText(_path, _json);
	}

	public static PlayerData Create(){
		if(instance == null){
			string _path = Application.persistentDataPath + "/Data/" + pathName + ".json";

			if(File.Exists(_path) == false){
				instance = new PlayerData();
			}
			else{
				string _json = File.ReadAllText(_path, Encoding.UTF8);
				instance = JsonUtility.FromJson<PlayerData>(_json);
			}
		}

		return instance;
	}

	public static bool HasePlayerData(){
		bool _has = false;
		string _path = Application.persistentDataPath + "/Data/" + pathName + ".json";
		if (File.Exists (_path)) {
			_has = true;
		}
		return _has;
	}
}
