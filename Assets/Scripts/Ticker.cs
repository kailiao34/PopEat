using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticker : MonoBehaviour {
	class TimeRecord {
		public float t = 0;					// 從開始計時到目前經過的時間 (Second)
		public float targetSec;			// 目標秒數
		public Del callback;            // t = target 後要做的事

		public TimeRecord(float targetSec, Del callback) {
			this.targetSec = targetSec;
			this.callback = callback;
		}
	}
	[Header("如果 True，當沒有計時器在正使用時會關閉此腳本的 Update 以節省效能")]
	public bool autoEnabled;
	List<TimeRecord> ticker = new List<TimeRecord>();

	public delegate void Del();
	static Ticker ins;

	private void Awake() {
		// 如果 ins 不是 null，代表有人早於這個 Awake 先呼叫 StartTicker 了
		if (ins == null) {
			if (autoEnabled) enabled = false;
			ins = this;
		}
	}


	public static void StartTicker(float sec, Del callback) {
		if (ins == null) ins = FindObjectOfType<Ticker>();
		if (ins == null) return;

		ins.ticker.Add(new TimeRecord(sec, callback));
		if (ins.autoEnabled) ins.enabled = true;
	}
	
	void Update () {
		for (int i = 0; i < ticker.Count; i++) {
			if (ticker[i] == null) {
				ticker.RemoveAt(i);
				i--;
				continue;
			}

			ticker[i].t += Time.deltaTime;

			// 秒數到達
			if (ticker[i].t >= ticker[i].targetSec) {
				if (ticker[i].callback != null) ticker[i].callback();           // 呼叫 CallBack 函數
				ticker.RemoveAt(i);                                             // 反註冊該物件
				if (ticker.Count == 0) {        // 如果沒有計時器了，關閉 Update 以節省效能
					if (autoEnabled) enabled = false;
					return;
				}
				i--;
			}
		}
	}
}
