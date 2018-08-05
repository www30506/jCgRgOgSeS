using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

public class PD : MonoBehaviour {
	[System.Serializable]
	public class FileClass{
		public string m_path;
		public string m_name;

		public FileClass(){
			m_path = "";
			m_name = "";
		}
	}
	public static Dictionary<string, Dictionary<string, Dictionary<string, object>>> DATA;
	private Dictionary<string, Dictionary<string, Dictionary<string, object>>> data;
	public static Dictionary<string, Dictionary<string, Dictionary<string, object>>> DEFINEDATA;
	private Dictionary<string, Dictionary<string, Dictionary<string, object>>> defineData;
	public FileClass[] m_file;
	private static PD instance;

	void Awake () {
		Init();
	}

	//這個接口是給 EditorWindow建立使用
	public void SetPDTxt(string[] p_name, string[] p_path){
		m_file = new FileClass[p_path.Length];
		for(int i=0; i< m_file.Length; i++){
			m_file[i] = new FileClass();
			m_file[i].m_name = p_name[i];
			m_file[i].m_path = p_path[i];
		}

		Init();
	}

	private void Init(){
		data = null;
		data = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
		defineData = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();

		//DefineData
		for (int i = 0; i < m_file.Length; i++) {
			FileStream file = new FileStream(Application.streamingAssetsPath + "/" + m_file[i].m_path + "/" + m_file[i].m_name + ".txt", FileMode.Open, FileAccess.Read);
			StreamReader sr = new StreamReader(file);
			DoParseDefineData(i, sr.ReadToEnd());
			file.Close();
			sr.Close();
		}
		PD.DEFINEDATA = defineData;


		//Data
		for (int i = 0; i < m_file.Length; i++) {
			FileStream file = new FileStream(Application.streamingAssetsPath + "/" + m_file[i].m_path + "/" + m_file[i].m_name + ".txt", FileMode.Open, FileAccess.Read);
			StreamReader sr = new StreamReader(file);
			DoParse(i, sr.ReadToEnd());
			file.Close();
			sr.Close();
		}

		PD.DATA = data;
		instance = this;
	}

	void Update () {

	}

	public static void Save(Dictionary<string, Dictionary<string, object>> p_data, string p_key){
		PD.instance.M_Save(p_data, p_key);
	}

	private void M_Save(Dictionary<string, Dictionary<string, object>> p_data, string p_key){
		string _path = "";
		string _name = "";

		//取得傳入key 所對應的檔案位子和名稱(名稱跟key相同)
		for(int i=0; i< m_file.Length; i++){
			if(p_key == m_file[i].m_name){
				_path = m_file[i].m_path;
				_name = m_file[i].m_name;
			}
		}

		if(File.Exists(Application.streamingAssetsPath + "/" + _path + "/" + _name + ".txt")){
			File.Delete(Application.streamingAssetsPath + "/" + _path + "/" + _name + ".txt");
		}

		FileStream file = new FileStream(Application.streamingAssetsPath + "/" + _path + "/" + _name + ".txt", FileMode.Create, FileAccess.Write);
		StreamWriter sw = new StreamWriter(file, Encoding.UTF8);
		//寫入Define
		foreach(string _key in defineData[p_key].Keys){
			string _writeLineStr = "";
			int _key_II_Count = 0;

			foreach(string _key_II in defineData[p_key][_key].Keys){
				_writeLineStr += defineData[p_key][_key][_key_II].ToString();

				if(++_key_II_Count < defineData[p_key][_key].Count){
					_writeLineStr +="\t";
				}
			}

			sw.WriteLine(_writeLineStr);
		}

		//寫入 key查詢
		foreach(string _key in p_data.Keys){
			string _writeLineStr = "";
			int _key_II_Count = 0;

			foreach(string _key_II in p_data[_key].Keys){
				_writeLineStr += _key_II;

				if(++_key_II_Count < p_data[_key].Count){
					_writeLineStr +="\t";
				}
			}

			sw.WriteLine(_writeLineStr);
			break;
		}

		int _key_Count = 0;
		//寫入 資料內容
		foreach(string _key in p_data.Keys){
			string _writeLineStr = "";
			int _key_II_Count = 0;

			foreach(string _key_II in p_data[_key].Keys){
				_writeLineStr += p_data[_key][_key_II];
				if(++_key_II_Count < p_data[_key].Count){
					_writeLineStr +="\t";
				}
			}

			if(++_key_Count < p_data.Count){
				sw.WriteLine(_writeLineStr);
			}
			else{
				sw.Write(_writeLineStr);
			}
		}
		sw.Close();
		file.Close();
	}

	private void DoParse(int p_number, string p_data){
		Dictionary<string, Dictionary<string, object>> data_2 = new Dictionary<string, Dictionary<string, object>>();
		int _startCount = defineData[m_file[p_number].m_name].Count;

		string[] _allLine = p_data.Split('\n');

		string[] _keyData = _allLine[_startCount].Split('\t');
		string[] _typeKey = new string[_keyData.Length];

		for (int i = 0; i < _keyData.Length; i++) {
			_keyData [i] = Regex.Replace (_keyData [i], @"[^a-zA-Z0-9]", "");
			_typeKey [i] = _keyData [i];
		}

		for(int i=_startCount+1; i< _allLine.Length; i++){
			char[] _chardata = _allLine[i].ToCharArray();

			if( _chardata.Length < 1) continue;

			string[] _strData = _allLine[i].Split('\t');

			int _length = _strData.Length;
			Dictionary<string, object> _data = new Dictionary<string, object> ();

			for (int j = 0; j < _typeKey.Length; j++) {
				_strData[j] = Regex.Replace(_strData[j], "\\s", "");
				_data.Add (_typeKey [j], _strData [j]);
			}
			data_2.Add (_strData [0], _data);
		}

		data.Add (m_file[p_number].m_name, data_2);
	}

	private void DoParseDefineData(int p_number, string p_data){
		Dictionary<string, Dictionary<string, object>> _data_2 = new Dictionary<string, Dictionary<string, object>>();

		string[] _allLine = p_data.Split('\n');
		string[] _key = null;
		//紀錄 key
		for(int i=0; i< _allLine.Length; i++){
			string _str = _allLine[i].ToLower();
			if(_str.Contains("[define]")) 
				continue;

			_key = _allLine[i].Split('\t');
			break;
		}

		//DefineData
		int _count = 0;

		for(int i=0; i< _allLine.Length; i++){
			string _str = _allLine[i].ToLower();
			if(_str.Contains("[define]")){
				string[] _strData = _allLine[i].Split('\t');
				Dictionary<string, object> _data = new Dictionary<string, object> ();

				for(int j=0; j<_strData.Length; j++){
					_strData[j] = Regex.Replace(_strData[j], "\\s", "");
					_data.Add (_key [j], _strData [j]);
				}

				_data_2.Add(_count.ToString(), _data);
				_count++;
			}
		}

		defineData.Add(m_file[p_number].m_name, _data_2);
	}
}