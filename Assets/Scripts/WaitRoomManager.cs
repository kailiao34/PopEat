using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitRoomManager : MonoBehaviour {
	[SerializeField]
	WaitRoomPlayer localPlayer;
	[SerializeField]
	Transform playersContent;
	[SerializeField]
	GameObject playerInfoPrefab;
	public static WaitRoomManager ins;
	List<WaitRoomPlayer> players = new List<WaitRoomPlayer>();

	private void Awake() {
		ins = this;
	}

	public void LocalPlayer () {
		FeedInfo(localPlayer, UIRoomManager.myInfos);
	}

	public void Ready(int index, bool ready) {
		if (index == -1) {          // 本地玩家 Ready
			localPlayer.readyUI.SetActive(ready);
		} else {
			players[index].readyUI.SetActive(ready);
		}
	}

	public void PlayerJoin(PlayerInfos pi) {
		Ticker.StartTicker(0, () => {
			WaitRoomPlayer g = Instantiate(playerInfoPrefab, playersContent).GetComponent<WaitRoomPlayer>();
			FeedInfo(g, pi);
			players.Add(g);
		});
	}

	public void PlayerLeave(int index) {
		Destroy(players[index].gameObject);
		players.RemoveAt(index);
	}

	public void ClearAllPlayer() {
		foreach (WaitRoomPlayer w in players) {
			Destroy(w.gameObject);
		}
		players.Clear();
	}

	void FeedInfo(WaitRoomPlayer obj, PlayerInfos pi) {
		obj.nickNameText.text = pi.nickName;
		obj.resText.text = pi.foodSelected;
		obj.FeedColor(pi.colorIndex);
		obj.readyUI.SetActive(pi.ready);
	}
}
