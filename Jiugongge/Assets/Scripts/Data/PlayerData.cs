using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

[System.Serializable]
public class PlayerData{
	/********	Data	********/
	public string language = "TraditionalChinese";//選擇的語言
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
