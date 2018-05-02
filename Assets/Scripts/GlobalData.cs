using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GlobalData : ScriptableObject {

	public string ServerIP = "127.0.0.1";
	public int ServerPort = 8056;

	[Header("開始遊戲前倒數幾秒")]
	public int startSec = 8;
	[Header("開始遊戲後幾秒結束")]
	public int overSec = 8;
	[Header("六角排列層數")]
	public int hexLayers = 9;
	[Header("餐廳被選擇時")]
	public Color resHighlighted;
	[Space]
	public Color[] colors;
}
