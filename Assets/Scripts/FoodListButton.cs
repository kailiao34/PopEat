using UnityEngine;
using UnityEngine.UI;
public class FoodListButton : MonoBehaviour {
	public Text UItext;
	public Button button;
	public int buttonIndex;

	public void Select() {
		//print("您選擇了: " + UItext.text);
		UIRoomManager.myInfos.foodSelected = UItext.text;

		// Highlight Selected
		FoodLis.HighlightButton(buttonIndex);
	}
	public void Info() {
		ButtonManager.ins.UIswitcher.SetBool("ResInfo", true);

		GetRes.ins.GetResDetail(UItext.text, ShowResDetailInfo);
	}

	public void InfoOff() {
		ButtonManager.ins.UIswitcher.SetBool("ResInfo", false);
	}

	void ShowResDetailInfo(Details d) {
		if (d == null) {
			LogUI.Show("無法取得這間餐廳的資料");
			return;
		}

		string closed = "";
		System.Text.StringBuilder str = new System.Text.StringBuilder();

		FoodLis.resInfoText[0].text = d.name;
		FoodLis.resInfoText[1].text = d.phoneNum;
		FoodLis.resInfoText[2].text = d.address;

		if (d.openingHours == null) {
			str = new System.Text.StringBuilder("無資料 _(´ཀ`」 ∠)_");
		} else {
			foreach (string s in d.openingHours) {
				str.AppendLine(s);
			}
		}

		if (str.ToString().Length <= 1) {
			FoodLis.resInfoText[3].text = "無資料 _(´ཀ`」 ∠)_";
		} else FoodLis.resInfoText[3].text = str.ToString();

		FoodLis.resInfoText[4].text = "評價：" + d.rating.ToString() + "/5";

		if (d.permanentlyClosed) {
			closed = "已歇業";
		} else if (!d.permanentlyClosed) {
			closed = "";
		}

		FoodLis.resInfoText[5].text = closed;

		str = new System.Text.StringBuilder();

		foreach (Reviews s in d.reviews) {
			str.AppendLine("評價者: " + s.name);
			str.AppendLine("評價分數: " + s.rating);
			str.AppendLine("留言: " + s.text);
			str.AppendLine("留言時間: " + s.time.ToString("yyyy-MM-dd HH:mm:ss \n\n\n"));
		}
		FoodLis.resInfoText[6].text = str.ToString();

		if (d.openNow == 0) {
			FoodLis.resInfoText[7].text = "營業中";
		} else if (d.openNow == 1) {
			FoodLis.resInfoText[7].text = "已打烊/休息中";
		} else {
			FoodLis.resInfoText[7].text = "無營業時段資訊";
		}
	}
}
