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
	List<GameObject> players = new List<GameObject>();

	private void Awake() {
		ins = this;
	}

	public void LocalPlayer () {
		FeedInfo(localPlayer, UIRoomManager.myInfos);
	}

	public void PlayerJoin(int index) {
		GameObject g = Instantiate(playerInfoPrefab, playersContent);
		FeedInfo(g.AddComponent<WaitRoomPlayer>(), UIRoomManager.playersInRoom[index]);
		players.Add(g);
	}

	public void PlayerLeave(int index) {
		Destroy(players[index]);
		players.RemoveAt(index);
	}

	void FeedInfo(WaitRoomPlayer obj, PlayerInfos pi) {
		//obj.nickNameText.text = pi.nickName;
		//obj.resText.text = pi.foodSelected;
		//obj.FeedColor(pi.colorIndex);
	}
}
