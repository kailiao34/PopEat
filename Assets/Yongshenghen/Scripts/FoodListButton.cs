using UnityEngine;
using UnityEngine.UI;
public class FoodListButton : MonoBehaviour {
    public Text UItext;
	[HideInInspector]
	public int resIndex;
	
	public void Select()
    {
		print("您選擇了: " + UItext.text);
		GameManager.playerInfos.foodSelected = UItext.text;
	}
    public void Info()
    {
		Details d = FoodLis.resList[resIndex];

		System.Text.StringBuilder str = new System.Text.StringBuilder();

		FoodLis.resInfos[0].text = d.name;
		FoodLis.resInfos[1].text = d.phoneNum;
		FoodLis.resInfos[2].text = d.address;

		foreach (string s in d.openingHours) {
			str.AppendLine(s);
		}
		FoodLis.resInfos[3].text = str.ToString();

		FoodLis.resInfos[4].text = d.rating.ToString();
		FoodLis.resInfos[5].text = d.permanentlyClosed.ToString();

		str = new System.Text.StringBuilder();

		foreach (Reviews s in d.reviews) {
			str.AppendLine("評價者: " + s.name);
			str.AppendLine("評價分數: " + s.rating);
			str.AppendLine("留言: " + s.text);
			str.AppendLine("留言時間: " + s.time.ToString("yyyy-MM-dd HH:mm:ss"));
		}
		FoodLis.resInfos[6].text = str.ToString();
	}
}
