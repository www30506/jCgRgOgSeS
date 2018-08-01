using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaVikingCode.AssetPacker;
using System.IO;
using UnityEngine.UI;

public class AssetsPackerExample : MonoBehaviour {
	AssetPacker assetPacker;
	public Image[] images;
	public string[] imageNames;
	public string filePath;

	void Start () {
		string[] files = new string[imageNames.Length];
		for(int i=0; i< imageNames.Length; i++){
			files[i] = Application.dataPath + filePath + imageNames[i]+ ".png";
		}

		assetPacker = GetComponent<AssetPacker>();
		assetPacker.AddTexturesToPack(files);
		assetPacker.OnProcessCompleted.AddListener(LoadAnimation); //加載完畢的事件，將讀取圖片的事件放在這邊
		assetPacker.Process();
	}

	void Update () {
	}

	private void LoadAnimation() {
		print("加載完成時間 : " + Time.realtimeSinceStartup);
		Sprite[] sprites = new Sprite[imageNames.Length];
		for(int i=0; i<imageNames.Length; i++){
			sprites[i] =  assetPacker.GetSprite(imageNames[i]);
		}

		for(int i=0; i< sprites.Length; i++){
			images[i].sprite = sprites[i];
		}
	}
}
