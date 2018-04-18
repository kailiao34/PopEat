using System.Collections.Generic;
using UnityEngine;

namespace AlgoClasses {
	public class Probability {

		float[] pro;

		/// <summary>
		/// 給一表示機率百分比浮點數陣列，設定機率，以陣列大小決定有幾種狀態
		/// 設定方式如下:
		/// --- float[] p = new float[] { 20.7f, 29, 50.3f };
		/// --- SetProb(p); 
		/// --- // 陣列有3個元索，所以會有三種狀態，呼叫 Get() 時，有 20% 機率會得到 0, 30% 機率會得到1, 50% 機率會得到2
		/// 註: 如果其中有負數則當作 0
		/// 註: 所有數字加起來需等於 100，若小於100，則剩餘的部份機率都給最後一個元素
		///		若加起來大於 100，則大於100之後的永遠不會被選到，或機率被前面的瓜分掉
		/// </summary>
		public void SetProb(float[] probabilities) {
			pro = probabilities;
		}
		/// <summary>
		/// 給一表示比重的整數陣列，設定機率，以陣列大小決定有幾種狀態
		/// 設定方式如下:
		/// --- int[] p = new int[] { 1, 1, 3, 7, 8 };		// 這些數字代表他們在整個陣列中的比重
		/// --- SetProb(p); 
		/// 則各元素的機率為: [ 5%, 5%, 15%, 35%, 40% ]
		/// 註: 如果其中有負數則當作 0
		/// </summary>
		public void SetProb(int[] weights) {
			if (weights == null) return;

			float sum = 0;
			for (int i = 0; i < weights.Length; i++) {
				if (weights[i] <= 0) weights[i] = 0;
				sum += weights[i];
			}
			if (sum <= 0) return;

			pro = new float[weights.Length];
			for (int i = 0; i < weights.Length; i++) {
				pro[i] = weights[i] / sum * 100;
			}
		}
		/// <summary>
		/// 回傳的是 Index: 0, 1, 2, 3, ....
		/// </summary>
		public int Get() {
			if (pro == null || pro.Length == 0) return -1;
			int n = pro.Length;
			if (n == 1) return 0;

			float r = Random.Range(0f, 100f);

			n--;
			float sum = 0;
			for (int i = 0; i < n; i++) {
				if (pro[i] <= 0) continue;

				if (r >= sum && r < sum + pro[i]) {
					return i;
				}
				sum += pro[i];
			}

			return n;
		}
	}
}

public class Algorithms {
	/// <summary>
	/// 傳入一個陣列，打亂所有元素後回傳
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="array"></param>
	/// <returns></returns>
	public static T[] GetRandomArray<T>(T[] array) {
		T tmp;

		int halfN = array.Length / 2;
		int r;

		for (int i = 0; i < halfN; i++) {
			// 跟隨機一個互換位置
			r = Random.Range(0, array.Length);
			tmp = array[i];
			array[i] = array[r];
			array[r] = tmp;
		}

		return array;
	}
}
