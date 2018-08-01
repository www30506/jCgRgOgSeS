using DaVikingCode.RectanglePacking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace DaVikingCode.AssetPacker {

	public class AssetPacker : MonoBehaviour {

		public UnityEvent OnProcessCompleted;
		public float pixelsPerUnit = 100.0f;

		[Header("是否使用快取(會在本地端存一份圖片)")]
		public bool useCache = false;
		[Header("快取名稱")]
		public string cacheName = "";
		[Header("快取版本號")]
		public int cacheVersion = 1;
		public bool deletePreviousCacheVersion = true;

		protected Dictionary<string, Sprite> mSprites = new Dictionary<string, Sprite>();
		protected List<TextureToPack> itemsToRaster = new List<TextureToPack>();

		protected bool allow4096Textures = false;

		public void AddTextureToPack(string file, string customID = null) {

			itemsToRaster.Add(new TextureToPack(file, customID != null ? customID : Path.GetFileNameWithoutExtension(file)));
		}

		public void AddTexturesToPack(string[] files) {

			foreach (string file in files)
				AddTextureToPack(file);
		}

		public void Process(bool allow4096Textures = false) {

			this.allow4096Textures = allow4096Textures;

			if (useCache) {

				if (cacheName == "")
					throw new Exception("No cache name specified");

				string path = Application.persistentDataPath + "/AssetPacker/" + cacheName + "/" + cacheVersion + "/";

				bool cacheExist = Directory.Exists(path);

				if (!cacheExist)
					StartCoroutine(createPack(path));
				else
					StartCoroutine(loadPack(path));
				
			} else
				StartCoroutine(createPack());
			
		}

		protected IEnumerator createPack(string savePath = "") {

			if (savePath != "") {

				if (deletePreviousCacheVersion && Directory.Exists(Application.persistentDataPath + "/AssetPacker/" + cacheName + "/"))
					foreach (string dirPath in Directory.GetDirectories(Application.persistentDataPath + "/AssetPacker/" + cacheName + "/", "*", SearchOption.AllDirectories))
						Directory.Delete(dirPath, true);

				Directory.CreateDirectory(savePath);
			}

			List<Texture2D> textures = new List<Texture2D>();
			List<string> images = new List<string>();

			foreach (TextureToPack itemToRaster in itemsToRaster) {

				WWW loader = new WWW("file:///" + itemToRaster.file);

				yield return loader;

				textures.Add(loader.texture);
				images.Add(itemToRaster.id);
			}

			int textureSize = allow4096Textures ? 4096 : 2048;

			List<Rect> rectangles = new List<Rect>();
			for (int i = 0; i < textures.Count; i++)
				if (textures[i].width > textureSize || textures[i].height > textureSize)
					throw new Exception("A texture size is bigger than the sprite sheet size!");
				else
					rectangles.Add(new Rect(0, 0, textures[i].width, textures[i].height));

			const int padding = 1;

			int numSpriteSheet = 0;
			while (rectangles.Count > 0) {

				Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
				Color32[] fillColor = texture.GetPixels32();
				for (int i = 0; i < fillColor.Length; ++i)
					fillColor[i] = Color.clear;

				RectanglePacker packer = new RectanglePacker(texture.width, texture.height, padding);

				for (int i = 0; i < rectangles.Count; i++)
					packer.insertRectangle((int) rectangles[i].width, (int) rectangles[i].height, i);

				packer.packRectangles();

				if (packer.rectangleCount > 0) {

					texture.SetPixels32(fillColor);
					IntegerRectangle rect = new IntegerRectangle();
					List<TextureAsset> textureAssets = new List<TextureAsset>();

					List<Rect> garbageRect = new List<Rect>();
					List<Texture2D> garabeTextures = new List<Texture2D>();
					List<string> garbageImages = new List<string>();

					for (int j = 0; j < packer.rectangleCount; j++) {

						rect = packer.getRectangle(j, rect);

						int index = packer.getRectangleId(j);

						texture.SetPixels32(rect.x, rect.y, rect.width, rect.height, textures[index].GetPixels32());

						TextureAsset textureAsset = new TextureAsset();
						textureAsset.x = rect.x;
						textureAsset.y = rect.y;
						textureAsset.width = rect.width;
						textureAsset.height = rect.height;
						textureAsset.name = images[index];

						textureAssets.Add(textureAsset);

						garbageRect.Add(rectangles[index]);
						garabeTextures.Add(textures[index]);
						garbageImages.Add(images[index]);
					}

					foreach (Rect garbage in garbageRect)
						rectangles.Remove(garbage);

					foreach (Texture2D garbage in garabeTextures)
						textures.Remove(garbage);

					foreach (string garbage in garbageImages)
						images.Remove(garbage);

					texture.Apply();

					if (savePath != "") {

						File.WriteAllBytes(savePath + "/data" + numSpriteSheet + ".png", texture.EncodeToPNG());
						File.WriteAllText(savePath + "/data" + numSpriteSheet + ".json", JsonUtility.ToJson(new TextureAssets(textureAssets.ToArray())));
						++numSpriteSheet;
					}

					foreach (TextureAsset textureAsset in textureAssets)
						mSprites.Add(textureAsset.name, Sprite.Create(texture, new Rect(textureAsset.x, textureAsset.y, textureAsset.width, textureAsset.height), Vector2.zero, pixelsPerUnit, 0, SpriteMeshType.FullRect));
				}

			}

			OnProcessCompleted.Invoke();
		}

		protected IEnumerator loadPack(string savePath) {
			
			int numFiles = Directory.GetFiles(savePath).Length;

			for (int i = 0; i < numFiles / 2; ++i) {

				WWW loaderTexture = new WWW("file:///" + savePath + "/data" + i + ".png");
				yield return loaderTexture;

				WWW loaderJSON = new WWW("file:///" + savePath + "/data" + i + ".json");
				yield return loaderJSON;

				TextureAssets textureAssets = JsonUtility.FromJson<TextureAssets> (loaderJSON.text);

				Texture2D t = loaderTexture.texture; // prevent creating a new Texture2D each time.
				foreach (TextureAsset textureAsset in textureAssets.assets)
					mSprites.Add(textureAsset.name, Sprite.Create(t, new Rect(textureAsset.x, textureAsset.y, textureAsset.width, textureAsset.height), Vector2.zero, pixelsPerUnit, 0, SpriteMeshType.FullRect));
			}

			yield return null;

			OnProcessCompleted.Invoke();
		}

		public void Dispose() {

			foreach (var asset in mSprites)
				Destroy(asset.Value.texture);

			mSprites.Clear();
		}

		void Destroy() {

			Dispose();
		}

		public Sprite GetSprite(string id) {

			Sprite sprite = null;

			mSprites.TryGetValue (id, out sprite);

			return sprite;
		}

		public Sprite[] GetSprites(string prefix) {

			List<string> spriteNames = new List<string>();
			foreach (var asset in mSprites)
				if (asset.Key.StartsWith(prefix))
					spriteNames.Add(asset.Key);

			spriteNames.Sort(StringComparer.Ordinal);

			List<Sprite> sprites = new List<Sprite>();
			Sprite sprite;
			for (int i = 0; i < spriteNames.Count; ++i) {

				mSprites.TryGetValue(spriteNames[i], out sprite);

				sprites.Add(sprite);
			}

			return sprites.ToArray();
		}
	}
}

/*

行動	檢視	開啟	關閉	拿下	放入	完成

初始物品	我	檢視	開啟	拿下	放入	光
總共物品	我	房間	床	梳妝台	鏡子	門	收藏櫃	天花板	手電筒	信	底片(一)	底片(二)	底片(三)
衣架	衣服	畫


//動物	
//鹿	獅子	狼		熊		山羊
//豬	貓		狗		老鼠	兔子
//蜘蛛	蛇		蝙蝠	狐狸	豹
//犀牛	鱷魚
	

Start	我進入到這片黑暗的空間，忽然間背後的門忽然關上，在這黑暗的空間裡面我看到了一個發著【光】的東西。

我	檢視	光		太暗了我看不到清楚，我鼓起勇氣觸碰它了一下，忽然間整個空間亮了起來，我看了下手觸碰到的地方，原來這是燈的【開關】，我環顧了四周我似乎在一個【房間】內。(光消失)

我	檢視	房間	在我前面的是一張木製的【床】，而床的左邊有一個【梳妝台】，在梳妝台的對面有一扇【門】和一個【收藏櫃】。

我	檢視	床		床上放著一些衣服和...........

我	檢視	門		這是一個木製的門，在手把上面有一個密碼輸入的裝置(3x3)。
我	開啟	門		顯示密碼輸入的畫面。

我	檢視	梳妝台	梳妝台上面有一面【鏡子】，還有一支【手電筒】跟一封【信】放在梳妝台上，

我	檢視	信		「親愛的瑪莉安：我幫你準備了早餐，是你最喜歡的蜂蜜吐司，對了昨晚"格蘭特"跟"奈特"又寫信來了，他們真的是很棒的人呢，不過他們好像很忙，因為我一直沒見過他們。對了今天是妳的生日，我準備了禮物放在收藏櫃裡面，給妳個小提示，是我們的回憶喔。」 by 海德

我	檢視	手電筒	恩...跟一般手電筒沒有太多的差別，不過我在手電筒的前面發現一個凹槽，難道這有什麼特殊的作用嗎？
我	開啟	手電筒	我按下了手電筒上面的ON，手電筒發出了光照向【天花板】。
我	關閉	手電筒	我按下了手電筒上面的OFF，將手電筒的燈給關上。


我	檢視	天花板	我看了很久，但是天花板上面什麼都沒有。
/**************************************************************************************************
我	檢視	天花板	底片(一)	天花板上面出現圖像，看起來是海德跟瑪麗安的過往照片，他們應該是在森林裡面打獵吧。那是兔子?看起來抓到不少呢有1.2.3.4.5隻呢
我	檢視	天花板	底片(二)	在庭院養動物..
我	檢視	天花板	底片(三)	天花板上面出現圖像，照片應該是在溪邊附近，但是很奇怪全部的人都驚慌失措的樣子，是發生了什麼事情嗎? 這時候我的腦袋一陣疼痛湧了上來，因為有一隻鱷魚突然出現，所以全部人嚇得趕緊跑開...等等為什麼我會有這些記憶!?
/**************************************************************************************************

我	拿下	天花板	.........這不是正常人可以做到的吧。


我	檢視	鏡子	鏡子看起來沒什麼特別的，等等我看到了一個帥哥，原來是我自己。
我	拿下	鏡子	我將鏡子給拿了下來，發現後面放著一個圓形的【底片(一)】。


我	檢視	收藏櫃	上面有一個三位數的密碼鎖。密碼是底片的動物個數
我	開啟	收藏櫃	彈出密碼鎖。
我	完成	收藏櫃	得到戒指，跟一封給瑪莉安的信

我	檢視	開關	這是用來開啟或【關閉】房間燈的開關。
我	開啟	開關	我把開關切到ON，房間恢復了光亮。
我	關閉	開關	我把開轉切到OFF，房間陷入了黑暗。






【1】有一個底片是反過來的必須從鏡子裡面看

【2】底片的提示拿來解收藏櫃的密碼 ，櫃子裡面放著戒指，戒指是海德要送給瑪莉安的生日禮物
(這邊可以用一些文字帶出 主角是海德)

【3】底片的提示，「瑪莉安今天是妳的生日，我將我們的回憶做成了謎題，東西就放在收藏櫃裡面，祝妳生日快樂。」

【4】門一開始只會顯示九宮格
要放入電源九宮格才會顯示東西
未放入電源的時候 密碼不會成功

*/